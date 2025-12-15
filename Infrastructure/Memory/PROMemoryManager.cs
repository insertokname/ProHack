using Domain;
using Infrastructure.Exceptions;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Infrastructure.Memory
{
    public class PROMemoryManager
    {
        private static class Offsets
        {
            public static readonly long[] PlayerXPos = [0x01538268, 0x20, 0x50, 0xB8, 0x0, 0x19C];
            public static readonly long[] PlayerYPos = [0x01538268, 0x20, 0x50, 0xB8, 0x0, 0x1A0];
            public static readonly long[] CurrentEncounterId = [0x01528C00, 0x20, 0x350, 0xB8, 0x0, 0x8C4];
            public static readonly long[] IsBattling = [0x01528C00, 0x20, 0x350, 0xB8, 0x0, 0x840];
            public static readonly long[] ShinyForm = [0x01528C00, 0x20, 0x350, 0xB8, 0x0, 0x8D0];
            public static readonly long[] EventForm = [0x01528C00, 0x20, 0x350, 0xB8, 0x0, 0x8D4];
            public static readonly long[] SelectedMenu = [0x01536DF8, 0xB8, 0x0, 0x10, 0x20, 0x88, 0x428, 0x18];
            public static readonly long[] TextSpeed = [0x01503DB8, 0x40, 0xB8, 0x20, 0xD0, 0x108, 0x40];
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

        public bool IsGameOpened { get => _processMemory != null && !_processMemory.Process.HasExited; }
        public Process? Process { get => _processMemory?.Process; }

        public bool LoadGame()
        {
            try
            {
                _processMemory = new ProcessMemory("PROClient", "GameAssembly.dll");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private ProcessMemory? _processMemory;
        private ProcessMemory _getProcessMemory()
        {
            if (_processMemory == null)
                throw new MemoryAccessException("Game Was not loaded while accessed!");
            return _processMemory;
        }

        private T _get<T>([CallerMemberName] string propertyName = "")
        where T : struct =>
            _memoryGetter<T>(_resolvePtr<T>(propertyName), propertyName);

        private void _set<T>(T value, [CallerMemberName] string propertyName = "")
        where T : struct =>
            _memorySetter<T>(_resolvePtr<T>(propertyName), value, propertyName);

        private static long[] _resolvePtr<T>(string offsetName)
        where T : struct
        {
            var field = typeof(Offsets).GetField(offsetName, BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic) ?? throw new ArgumentException($"No offset registered for {offsetName} ({typeof(Offsets)}).");

            if (field.GetValue(null) is long[] value)
                return value;

            throw new ArgumentException($"Offset {offsetName} was registered with incompatible type {field.FieldType}.");
        }

        private T _memoryGetter<T>(long[] pointerChain, string offsetName)
        where T : struct
        {
            var tries = 3;
            Exception lastE = null!;
            for (int i = 1; i <= tries; i++)
            {
                try
                {
                    return _getProcessMemory().ReadPointerChain<T>(pointerChain);
                }
                catch (Exception e)
                {
                    lastE = e;
                    Thread.Sleep(10);
                }
            }
            _processMemory = null;
            throw new MemoryAccessException(
                $"Got error while accessing memory for '{offsetName}'! Make sure you are logged in and on the latest version of ProHack! Error was {lastE}", lastE);
        }

        private void _memorySetter<T>(long[] pointerChain, T value, string offsetName)
        where T : struct
        {
            var tries = 3;
            Exception lastE = null!;
            for (int i = 1; i <= tries; i++)
            {
                try
                {
                    _getProcessMemory().WritePointerChain<T>(pointerChain, value);
                }
                catch (Exception e)
                {
                    lastE = e;
                    Thread.Sleep(10);
                }
            }
            _processMemory = null;
            throw new MemoryAccessException(
                $"Got error while accessing memory for '{offsetName}'! Make sure you are logged in and on the latest version of ProHack! Error was {lastE}", lastE);
        }
    }
}
