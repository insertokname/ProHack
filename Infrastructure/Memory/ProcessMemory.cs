using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Infrastructure.Memory
{
    public class ProcessMemory
    {
        public Process Process { get; }
        public IntPtr Handle { get; }
        public IntPtr ModuleAddress { get; }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        const int PROCESS_ALL_ACCESS = 0x000F0000 | 0x00100000 | 0xFFFF;

        public ProcessMemory(string processName, string moduleName)
        {
            try
            {
                Process = Process.GetProcessesByName(processName)[0];
                Handle = OpenProcess(PROCESS_ALL_ACCESS, false, Process.Id);
                ModuleAddress = Process.Modules
                    .Cast<ProcessModule>()
                    .FirstOrDefault(module => module.ModuleName == moduleName)!.BaseAddress;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get the process and the module!", e);
            }
        }

        /// <summary>
        /// Returns the value at the "absolute" address
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <returns></returns>
        public T ReadAbsolute<T>(long address)
        where T : struct
        {
            byte[] bytes = new byte[Marshal.SizeOf<T>()];
            var res = ReadProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out IntPtr _);
            if (!res)
            {
                throw new Exception($"Couldn't read process memory from address {address:X}! Got error code: {GetLastError()}");
            }
            return MemoryMarshal.Read<T>(bytes.AsSpan());
        }

        /// <summary>
        /// Writes a value of type T to the "absolute" address
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public void WriteAbsolute<T>(long address, T value)
        where T : struct
        {
            byte[] bytes = new byte[Marshal.SizeOf<T>()];
            MemoryMarshal.Write(bytes.AsSpan(), value);

            var res = WriteProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out IntPtr _);
            if (!res)
            {
                throw new Exception($"Couldn't write process memory at address {address:X}! Got error code: {GetLastError()}");
            }
        }

        /// <summary>
        /// Returns stored at the end of a pointer chain. This is offset by the module address.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offsets"></param>
        /// <returns></returns>
        public T ReadPointerChain<T>(long[] offsets)
        where T : struct
        {
            return ReadAbsolute<T>(_evalPointerChain(offsets));
        }

        /// <summary>
        /// Writes a value at the end of a pointer chain. This is offset by the module address.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offsets"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void WritePointerChain<T>(long[] offsets, T value)
        where T : struct
        {
            WriteAbsolute<T>(_evalPointerChain(offsets), value);
        }


        private long _evalPointerChain(long[] offsets)
        {
            long address = ModuleAddress;

            foreach (long offset in offsets)
            {
                address += offset;
                if (offset == offsets.LastOrDefault())
                {
                    return address;
                }
                address = ReadAbsolute<long>(address);
            }
            return address;
        }
    }
}