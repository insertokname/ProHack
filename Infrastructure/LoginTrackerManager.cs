using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class LoginTrackerManager
    {
        private readonly HttpClient client;
        const string WorkerUrl = "https://pro-hack-activity-dashboard.insertokname.workers.dev/track/login";

        public LoginTrackerManager(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
        }

        public async Task TrackLogin()
        {
            try
            {
                using StringContent jsonContent = new(
                    JsonSerializer.Serialize(new
                    {
                        version = VersionManager.GetVersionCode(),
                    }),
                    Encoding.UTF8,
                "application/json");
                await client.PostAsync(WorkerUrl, jsonContent);
            }
            catch (Exception) { }
        }
    }
}