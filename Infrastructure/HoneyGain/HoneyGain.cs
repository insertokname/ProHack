using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Infrastructure.HoneyGain
{
    public static class HoneyGain
    {
        static HoneyGain()
        {
            try
            {
                AppDomain.CurrentDomain.ProcessExit += OnExit!;
                AppDomain.CurrentDomain.DomainUnload += OnExit!;
#if HGSdk
                LoadEmbeddedDll();
#endif
            }
            catch { }
        }

        public static bool IsSdkAvailable()
        {
#if HGSdk
            return true;
#else
            return false;
#endif
        }

        public static bool Start(string api_key)
        {
            ThrowIfSdkNotAvailable();
            int result = Start64(api_key, out bool consent);
            if (result < 0)
                throw new Exception("Failed to start HG SDK");
            return consent;
        }

        public static void Stop()
        {
            ThrowIfSdkNotAvailable();
            int result = Stop64();
            if (result < 0)
                throw new Exception("Failed to stop HG SDK");
        }

        public static bool IsRunning()
        {
            ThrowIfSdkNotAvailable();
            int result = IsRunning64(out bool running);
            if (result < 0)
                throw new Exception("Failed to check if HG SDK is running");
            return running;
        }

        public static void OptIn()
        {
            ThrowIfSdkNotAvailable();
            int result = OptIn64();
            if (result < 0)
                throw new Exception("Failed to opt in user");
        }

        public static void OptOut()
        {
            ThrowIfSdkNotAvailable();
            int result = OptOut64();
            if (result < 0)
                throw new Exception("Failed to opt out user");
        }

        public static bool IsOptedIn()
        {
            ThrowIfSdkNotAvailable();
            int result = IsOptedIn64(out bool consent);
            if (result < 0)
                throw new Exception("Failed to check if user is opted in");
            return consent;
        }

        public static bool RequestConsent()
        {
            ThrowIfSdkNotAvailable();
            int result = RequestConsent64(out bool consent);
            if (result < 0)
                throw new Exception("Failed to request user consent");
            return consent;
        }

        public static void Log(string dir)
        {
            ThrowIfSdkNotAvailable();
            int result = Log64(dir);
            if (result < 0)
                throw new Exception("Failed to enable logging for HG SDK");
        }

        public static void Mute()
        {
            ThrowIfSdkNotAvailable();
            int result = Mute64();
            if (result < 0)
                throw new Exception("Failed to disable logging for HG SDK");
        }

        private static void ThrowIfSdkNotAvailable()
        {
            if (!IsSdkAvailable())
            {
                throw new Exception("Honeygain sdk not available!");
            }
        }

        private static void OnExit(object sender, EventArgs e)
        {
            try
            {
                Stop();
            }
            catch { }
        }

        private static void LoadEmbeddedDll()
        {
            var assembly = typeof(HoneyGain).Assembly;
            using var stream = assembly.GetManifestResourceStream("hgsdk.dll");
            if (stream == null) return;
            var tempPath = Path.Combine(FolderManager.LocalDownloads(), "hgsdk.dll");
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
            using (var fileStream = File.Create(tempPath))
            {
                stream.CopyTo(fileStream);
            }
            LoadLibrary(tempPath);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("hgsdk.dll", EntryPoint = "hgsdk_start", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Start64(string api_key, out bool consent);

        [DllImport("hgsdk.dll", EntryPoint = "hgsdk_stop", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Stop64();

        [DllImport("hgsdk.dll", EntryPoint = "hgsdk_is_running", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IsRunning64(out bool running);

        [DllImport("hgsdk.dll", EntryPoint = "hgsdk_opt_in", CallingConvention = CallingConvention.Cdecl)]
        private static extern int OptIn64();

        [DllImport("hgsdk.dll", EntryPoint = "hgsdk_opt_out", CallingConvention = CallingConvention.Cdecl)]
        private static extern int OptOut64();

        [DllImport("hgsdk.dll", EntryPoint = "hgsdk_is_opted_in", CallingConvention = CallingConvention.Cdecl)]
        private static extern int IsOptedIn64(out bool consent);

        [DllImport("hgsdk.dll", EntryPoint = "hgsdk_request_consent", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RequestConsent64(out bool consent);

        [DllImport("hgsdk.dll", EntryPoint = "hgsdk_log", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Log64(string dir);

        [DllImport("hgsdk.dll", EntryPoint = "hgsdk_mute", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mute64();
    }
}
