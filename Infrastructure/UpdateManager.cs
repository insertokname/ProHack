using System.Net.Http.Headers;
using System.Text.Json;
using System.Diagnostics;


namespace Infrastructure
{
    public class UpdateManager : IDisposable
    {
        private readonly HttpClient client;

        private const string _owner = "insertokname";
        private const string _repo = "ProHack";
        private const string _apiUrl = $"https://api.github.com/repos/{_owner}/{_repo}/releases/latest";

        private readonly Task<string> _response;
        private JsonDocument? _responseJson = null;

        public UpdateManager(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ProHack", VersionManager.GetVersionCode()));
            _response = client.GetStringAsync(_apiUrl);
        }
        public void Dispose()
        {
            _responseJson?.Dispose();
        }


        public static async Task UpdateAndReboot(string curVersionPath, string newVersionPath)
        {
            var proc = Process.GetCurrentProcess();

            Debug.WriteLine($"proc: {proc.Id}");

            var processInfo = new ProcessStartInfo
            {
                LoadUserProfile = true,
                FileName = "powershell.exe",
                Arguments = $"-Command $ErrorActionPreference = 'Stop'; Stop-Process -Id {proc.Id} -Force; while (Get-Process -Id {proc.Id} -ErrorAction SilentlyContinue) {{ Start-Sleep -Seconds 1 }}; Remove-Item '{curVersionPath}'; Move-Item '{newVersionPath}' '{curVersionPath}'; Start-Process '{curVersionPath}'",
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            var _ = Process.Start(processInfo);
            // The process will get killed by the update script.
            await Task.Delay(5000);
        }

        public AvailableUpdateState CheckAvailableUpdate(UpdateInfo? updateInfo)
        {
            if (updateInfo == null)
            {
                return AvailableUpdateState.FetchError;
            }
            if (VersionManager.IsSmallerThan(VersionManager.GetVersionCode(), updateInfo.VersionCode))
            {
                return AvailableUpdateState.UpdateAvailable;
            }
            return AvailableUpdateState.LatestVersion;
        }

        public async Task<UpdateInfo?> GetDownloadInfo()
        {
            try
            {
                var root = (await GetJsonResponse()).RootElement;

                var assets = root.GetProperty("assets");
                if (assets.GetArrayLength() == 0)
                {
                    return null;
                }

                return new()
                {
                    DownloadUrl = assets[0].GetProperty("browser_download_url").GetString()!,
                    VersionCode = root.GetProperty("tag_name").GetString()!,
                    Body = root.GetProperty("body").GetString()!,
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> DownloadNewVersion(UpdateInfo updateInfo, IProgress<double>? progress = null)
        {
            var localDownloadsDir = FolderManager.LocalDownloads();

            var targetPath = Path.Combine(localDownloadsDir, $"PROHack-{updateInfo.VersionCode}.exe");
            using var response = await client.GetAsync(updateInfo.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
                return null;

            var contentLength = response.Content.Headers.ContentLength ?? -1L;
            await using var source = await response.Content.ReadAsStreamAsync();

            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            await using var destination = File.Create(targetPath);

            var buffer = new byte[81920];
            long totalRead = 0;
            int read;
            while ((read = await source.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, read));
                totalRead += read;

                if (contentLength > 0)
                    progress?.Report((double)totalRead / contentLength);
            }

            progress?.Report(1);
            return targetPath;
        }

        private async Task<JsonDocument> GetJsonResponse()
        {
            var res = await _response;
            _responseJson ??= JsonDocument.Parse(res);

            return _responseJson;
        }


        public class UpdateInfo
        {
            public required string VersionCode { get; set; }
            public required string DownloadUrl { get; set; }
            public required string Body { get; set; }
        }

        public enum AvailableUpdateState
        {
            UpdateAvailable,
            LatestVersion,
            FetchError,
        }
    }
}
