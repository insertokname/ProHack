using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain;
using SimpleMem;

namespace Application
{
    public class MemoryManager
    {
        private static class Offsets
        {
            public static readonly MultiLevelPtr<float> PlayerXPos = new(0x01515A48, 0x20, 0x50, 0xB8, 0x0, 0x19C);
            public static readonly MultiLevelPtr<float> PlayerYPos = new(0x01515A48, 0x20, 0x50, 0xB8, 0x0, 0x1A0);
            public static readonly MultiLevelPtr<int> CurrentEncounterId = new(0x01515A48, 0x20, 0x350, 0xB8, 0x0, 0x8B4);
            public static readonly MultiLevelPtr<int> IsBattling = new(0x01515A48, 0x20, 0x350, 0xB8, 0x0, 0x838);
            public static readonly MultiLevelPtr<int> IsSpecial1 = new(0x01515A48, 0x20, 0x350, 0xB8, 0x0, 0x8C0);
            public static readonly MultiLevelPtr<int> IsSpecial2 = new(0x01515A48, 0x20, 0x350, 0xB8, 0x0, 0x8C4);
            public static readonly MultiLevelPtr<int> SelectedMenu = new(0x0150E6C8, 0xB8, 0x0, 0xF0, 0xD0, 0X10, 0xDF8);
        }

        private Memory? _memory;
        private Memory _getMemory()
        {
            if (_memory == null)
            {
                throw new MemoryAccessException("Game Was not loaded while accessed!");
            }
            return _memory;
        }
        public bool IsGameOpened { get => _memory != null && !_memory.Process.HasExited; }
        public Process? Process { get => _memory?.Process; }

        public bool LoadGame()
        {
            try
            {
                _memory = new Memory("PROClient", "GameAssembly.dll");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private T MemoryAccess<T>(Func<T> f, string accessorName, int tries = 3)
        {
            Exception lastE = null!;
            for (int i = 1; i <= tries; i++)
            {
                try
                {
                    return f();
                }
                catch (Exception e)
                {
                    lastE = e;
                    Thread.Sleep(10);
                }
            }
            _memory = null;
            throw new MemoryAccessException(
                $"Got error while accessing memory with {accessorName}! Make sure you are logged in and on the latest version of ProHack! Error was {lastE}",
                lastE);
        }

        public float GetPlayerPosX()
        {
            return MemoryAccess(() => _getMemory().ReadValueFromMlPtr(Offsets.PlayerXPos), nameof(GetPlayerPosX));
        }
        public float GetPlayerPosY()
        {
            return MemoryAccess(() => _getMemory().ReadValueFromMlPtr(Offsets.PlayerYPos), nameof(GetPlayerPosY));
        }

        public PointF GetPlayerPos()
        {
            float x = GetPlayerPosX();
            float y = GetPlayerPosY();

            return new PointF(x, y);
        }

        public int GetCurrentEncounterId()
        {
            return MemoryAccess(() => _getMemory().ReadValueFromMlPtr(Offsets.CurrentEncounterId), nameof(GetCurrentEncounterId));
        }

        public bool GetIsBattling()
        {
            var res = MemoryAccess(() => _getMemory().ReadValueFromMlPtr(Offsets.IsBattling), nameof(GetIsBattling));
            return res != 0;
        }

        public bool GetIsSpecialEncounter()
        {
            int res1 = MemoryAccess(() => _getMemory().ReadValueFromMlPtr(Offsets.IsSpecial1), nameof(Offsets.IsSpecial1));
            int res2 = MemoryAccess(() => _getMemory().ReadValueFromMlPtr(Offsets.IsSpecial2), nameof(Offsets.IsSpecial2));
            return res1 != 0
                || res2 != 0;
        }

        public SelectedMenuEnum GetSelectedMenu()
        {
            return MemoryAccess(
                () => SelectedMenuTools.FromMemory(_getMemory().ReadValueFromMlPtr(Offsets.SelectedMenu)), nameof(GetSelectedMenu));
        }

        public bool GetIsItemMenuSelected()
        {
            var res = GetSelectedMenu();
            return res == SelectedMenuEnum.ItemsMenu;
        }

        public bool GetIsNoMenuSelected()
        {
            var res = GetSelectedMenu();
            return res == SelectedMenuEnum.FightOrNoneMenu;
        }
    }
}
