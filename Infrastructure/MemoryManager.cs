using Domain;
using Infrastructure.Exceptions;
using SimpleMem;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Infrastructure
{
    public class MemoryManager
    {
        private static class Offsets
        {
            public static readonly MultiLevelPtr<float> PlayerXPos = new(0x01515A48, 0x20, 0x50, 0xB8, 0x0, 0x19C);
            public static readonly MultiLevelPtr<float> PlayerYPos = new(0x01515A48, 0x20, 0x50, 0xB8, 0x0, 0x1A0);
            public static readonly MultiLevelPtr<int> CurrentEncounterId = new(0x01515A48, 0x20, 0x350, 0xB8, 0x0, 0x8B4);
            public static readonly MultiLevelPtr<int> IsBattling = new(0x01515A48, 0x20, 0x350, 0xB8, 0x0, 0x838);
            public static readonly MultiLevelPtr<int> ShinyForm = new(0x01515A48, 0x20, 0x350, 0xB8, 0x0, 0x8C0);
            public static readonly MultiLevelPtr<int> EventForm = new(0x01515A48, 0x20, 0x350, 0xB8, 0x0, 0x8C4);
            public static readonly MultiLevelPtr<int> SelectedMenu = new(0x0150E6C8, 0xB8, 0x0, 0xF0, 0xD0, 0X10, 0xDF8);
            public static readonly MultiLevelPtr<float> TextSpeed = new(0x01503DB8, 0x40, 0xB8, 0x20, 0xD0, 0x108, 0x40);
        }

        public float PlayerXPos { get => _get<float>(); }
        public float PlayerYPos { get => _get<float>(); }
        public PointF PlayerPos { get => new(PlayerXPos, PlayerYPos); }
        public int CurrentEncounterId { get => _get<int>(); }
        public bool IsBattling { get => _get<int>() != 0; }
        public int ShinyForm { get => _get<int>(); }
        public int EventForm { get => _get<int>(); }
        public bool IsSpecial { get => ShinyForm != 0 || EventForm != 0; }
        public SelectedMenuEnum SelectedMenu { get => SelectedMenuTools.FromMemory(_get<int>()); }
        public bool IsItemMenuSelected { get => SelectedMenu == SelectedMenuEnum.ItemsMenu; }
        public bool IsNoMenuSelected { get => SelectedMenu == SelectedMenuEnum.FightOrNoneMenu; }
        public float TextSpeed { get => _get<float>(); set => _set(value); }

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

        private T _memoryAccess<T>(Func<T> f, string accessorName, int tries = 3)
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

        private Memory? _memory;
        private Memory _getMemory()
        {
            if (_memory == null)
                throw new MemoryAccessException("Game Was not loaded while accessed!");
            return _memory;
        }

        private T _get<T>([CallerMemberName] string propertyName = "")
        where T : struct =>
            _memoryGetter(_resolvePtr<T>(propertyName));

        private void _set<T>(T value, [CallerMemberName] string propertyName = "")
        where T : struct =>
            _memorySetter(_resolvePtr<T>(propertyName), value);

        private static MultiLevelPtr<T> _resolvePtr<T>(string offsetName)
        where T : struct
        {
            var field = typeof(Offsets).GetField(offsetName, BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic) ?? throw new ArgumentException($"No offset registered for {offsetName} ({typeof(Offsets)}).");

            if (field.GetValue(null) is MultiLevelPtr<T> value)
                return value;

            throw new ArgumentException($"Offset {offsetName} was registered with incompatible type {field.FieldType}.");
        }

        private T _memoryGetter<T>(MultiLevelPtr<T> multiLevelPtr)
        where T : struct =>
            _memoryAccess(() => multiLevelPtr.ReadValue<T>(_getMemory()), nameof(multiLevelPtr));

        private void _memorySetter<T>(MultiLevelPtr<T> multiLevelPtr, T value)
        where T : struct =>
            _memoryAccess(() => multiLevelPtr.WriteValue<T>(_getMemory(), value), nameof(multiLevelPtr));
    }
}
