using System.IO.Compression;
using System.Security.Cryptography;

namespace Infrastructure
{
    public class UpdateDataManager
    {
        private readonly HttpClient client;

        public UpdateDataManager(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
        }

        public async Task DownloadLatestIfAvailable(IProgress<double>? progress = null)
        {
            var local = GetDataLocalHash();
            var latest = await GetDataLatestHash();
            if (latest == null)
            {
                return;
            }
            var curExtractedPath = Path.Combine(Environment.CurrentDirectory, "DATA");
            var curZipPath = Path.Combine(FolderManager.LocalDownloads(), "PokemonData.zip");
            if (local == latest && Directory.Exists(curExtractedPath) && File.Exists(curZipPath))
            {
                return;
            }


            if (Directory.Exists(curExtractedPath))
            {
                Directory.Delete(curExtractedPath, true);
            }
            if (File.Exists(curZipPath))
            {
                File.Delete(curZipPath);
            }

            var latestZipPath = await DownloadDataLatestVersion(progress);
            var latestExtractedPath = Path.Combine(FolderManager.LocalDownloads(), "ExtractedLatestData");

            if (latestZipPath == null)
            {
                return;
            }
            if (Directory.Exists(latestExtractedPath))
            {
                Directory.Delete(latestExtractedPath, true);
            }

            await Task.Run(() => UnzipFolder(latestZipPath, latestExtractedPath));

            Directory.Move(Path.Combine(latestExtractedPath, "DATA"), curExtractedPath);
        }

        public static string? GetDataLocalHash()
        {
            var path = Path.Combine(FolderManager.LocalDownloads(), "PokemonData.zip");
            if (!File.Exists(path))
            {
                return null;
            }
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(fileStream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        public async Task<string?> GetDataLatestHash()
        {
            if (Database.Database.Tables.DataDownloadUrl == null)
            {
                return null;
            }
            string hashUrl = $"{Database.Database.Tables.DataDownloadUrl}.sha256";
            return await client.GetStringAsync(hashUrl);
        }

        public async Task<string?> DownloadDataLatestVersion(IProgress<double>? progress = null)
        {
            var downloadUrl = Database.Database.Tables.DataDownloadUrl;
            if (downloadUrl == null)
            {
                return null;
            }

            var latestPath = Path.Combine(FolderManager.LocalDownloads(), $"PokemonData.zip");
            using var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
                return null;

            var contentLength = response.Content.Headers.ContentLength ?? -1L;
            await using var source = await response.Content.ReadAsStreamAsync();

            if (File.Exists(latestPath))
            {
                File.Delete(latestPath);
            }

            await using var destination = File.Create(latestPath);

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
            return latestPath;
        }

        public static void UnzipFolder(string path, string extractPath)
        {
            ZipFile.ExtractToDirectory(path, extractPath);
        }
    }
}