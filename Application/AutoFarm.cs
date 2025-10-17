using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Models;
using Infrastructure;
using Infrastructure.Database;
using Infrastructure.Discord;
using Infrastructure.Discord.Announcments;

namespace Application
{
    public class AutoFarm
    {
        public bool IsRunning { get; private set; }
        public string? PauseReason { get; private set; } = null;
        public event EventHandler<string>? OnPause;

        private readonly DiscordBot _discordBot;

        private readonly MemoryManager _memoryManager;
        private readonly float _startPos;
        private readonly float _endPos;
        private readonly bool _isYAxis;
        private readonly PokemonTargetModel _pokemonTargetModel;
        private readonly Thread _thread;
        private readonly SynchronizationContext? _syncContext;
        private readonly Random random = new();
        private readonly List<ushort> pressedKeys = [];

        private float _curPos;
        public int _curDirection = 0;
        private BotState _curBotState = BotState.Walking;

        private DateTime? _encounterTime = null;
        private bool _sentDiscordDm = false;

        public AutoFarm(
            DiscordBot discordBot,
            MemoryManager memoryManager,
            float StartPos,
            float EndPos,
            bool IsYAxis = false,
            PokemonTargetModel? pokemonTargetModel = null,
            SynchronizationContext? synchronizationContext = null)
        {
            _discordBot = discordBot;
            _memoryManager = memoryManager;
            if (StartPos > EndPos)
            {
                _startPos = EndPos;
                _endPos = StartPos;
            }
            else if (EndPos > StartPos)
            {
                _startPos = StartPos;
                _endPos = EndPos;
            }
            else
            {
                throw new ArgumentException("StartPos Can't be equal to EndPos!");
            }

            _isYAxis = IsYAxis;
            _pokemonTargetModel = pokemonTargetModel ?? Constants.PokemonTargetModel.DefaultTarget();
            _syncContext = synchronizationContext ?? SynchronizationContext.Current;
            _thread = new(ThreadWork);
            _thread.Start();
        }

        public bool Start()
        {
            if (IsRunning)
            {
                return true;
            }

            var pos = GetCurPos();
            _curPos = pos;
            IsRunning = true;
            PauseReason = null;
            return true;
        }

        private void Pause(string reason)
        {
            IsRunning = false;
            PauseReason = reason;
            var proc = _memoryManager.Process;
            if (proc != null)
            {
                ReleaseAllKeys(proc);
            }
            OnPause?.Invoke(this, reason);
        }

        public void StopThread()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _thread.Join();
            }
            if (_memoryManager.Process != null)
            {
                ReleaseAllKeys(_memoryManager.Process);
            }
        }

        private enum BotState
        {
            Walking,
            RunninAway,
            Catching,
        }

        private float GetCurPos()
        {
            return _isYAxis ? _memoryManager.PlayerYPos : _memoryManager.PlayerXPos;
        }

        private Keys GetIncreasePosKey()
        {
            return _isYAxis ? Keys.W : Keys.D;
        }
        private Keys GetDecreasePosKey()
        {
            return _isYAxis ? Keys.S : Keys.A;
        }

        private ushort GetKeyCodeFromDirection(int direction)
        {
            return direction switch
            {
                1 => (ushort)GetIncreasePosKey(),
                -1 => (ushort)GetDecreasePosKey(),
                _ => throw new ArgumentException($"Got invalid direction {direction}! Direction must either be 1 or -1!"),
            };
        }

        private int GetNextDirection(int direction)
        {
            if (_curPos >= _endPos)
                return -1;
            else if (_curPos <= _startPos)
                return 1;

            if (direction == 0)
                return -1;

            if (random.Next(60) == 0)
                return direction * -1;
            else
                return direction;
        }

        private void PressKey(Process proc, ushort keyCode)
        {
            if (!pressedKeys.Any(k => k == keyCode))
            {
                pressedKeys.Add(keyCode);
                Controller.SendKeyDown(proc, keyCode);
            }
        }

        private void ReleaseAllKeys(Process proc)
        {
            foreach (ushort keyCode in pressedKeys)
            {
                Controller.SendKeyUp(proc, keyCode);
            }
            pressedKeys.Clear();
        }

        private void ThreadWork()
        {
            try
            {
                while (IsRunning)
                {
                    var proc = _memoryManager.Process;
                    if (proc == null)
                    {
                        Pause("Couldn't access process.");
                        continue;
                    }

                    if (_memoryManager.IsBattling)
                    {
                        _curBotState = BotState.Catching;
                        _curDirection = 0;
                        ReleaseAllKeys(proc);
                    }
                    else
                    {
                        _curBotState = BotState.Walking;
                        _encounterTime = null;
                        _sentDiscordDm = false;
                    }

                    if (_curBotState == BotState.Catching)
                    {
                        var encounterId = _memoryManager.CurrentEncounterId;
                        var isSpecial = _memoryManager.IsSpecial;

                        if (_encounterTime == null)
                        {
                            _encounterTime = DateTime.Now;
                            _addNewEncounter(encounterId, isSpecial, _encounterTime.Value);
                        }

                        var matches = _pokemonTargetModel.MatchesTarget(encounterId, isSpecial);

                        if (matches)
                        {
                            Console.Beep();
                        }

                        if (matches && !_sentDiscordDm)
                        {
                            Thread.Sleep(3000);
                            _sentDiscordDm = true;
                            _ = _discordBot.SendAnnouncement(new CaughtAnnouncement(isSpecial, encounterId));
                        }

                        if (_memoryManager.IsNoMenuSelected)
                        {
                            if (!matches)
                            {
                                Thread.Sleep(random.Next(30, 60));
                                Controller.SendKeyPress(proc, (ushort)Keys.D4);
                            }
                        }
                    }
                    else if (_curBotState == BotState.Walking)
                    {
                        _curPos = GetCurPos();

                        if (!(_startPos - 1 <= _curPos && _curPos <= _endPos + 1))
                        {
                            Pause("Current position is not inside the set bounds.");
                            continue;
                        }

                        var nextDirection = GetNextDirection(_curDirection);
                        if (nextDirection == 0)
                        {
                            Pause("Bounds too small! Couldn't find next direction.");
                            continue;
                        }

                        ushort nextKeyCode = GetKeyCodeFromDirection(nextDirection);

                        if (nextDirection != _curDirection)
                        {
                            ReleaseAllKeys(proc);
                            PressKey(proc, nextKeyCode);
                        }

                        _curDirection = nextDirection;
                    }
                    else
                    {
                        Pause("Unknown bot state");
                    }
                    Thread.Sleep(8);
                }
            }
            catch (Exception e)
            {
                var str = $"Got exeption in thread: {e}";
                Console.WriteLine(str);
                Debug.WriteLine(str);
            }
        }

        private void _addNewEncounter(int encounteredPokemonId, bool isSpecial, DateTime encounterTime)
        {
            void AddEncounter()
            {
                Database.Tables.EncounterStatsModels.Add(new EncounterStatsModel()
                {
                    EncounteredPokemonId = encounteredPokemonId,
                    IsSpecial = isSpecial,
                    EncounterTime = encounterTime,
                });
                Database.Save();
            }

            if (_syncContext != null && _syncContext != SynchronizationContext.Current)
            {
                _syncContext.Post(_ => AddEncounter(), null);
            }
            else
            {
                AddEncounter();
            }
        }
    }
}
