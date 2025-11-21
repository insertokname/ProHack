using Domain.Models;

namespace Infrastructure.Database
{
    public class DatabaseTables
    {
        public List<EncounterStatsModel> EncounterStatsModels { get; set; } = [];
        public string? DiscordBotToken { get; set; } = null;
        public ulong? AuthorizedUserId { get; set; } = null;
        public ulong? AnnouncementChannelId { get; set; } = null;
        public bool ShowBuyMeACoffee { get; set; } = true;
        public bool ShowWarningMessage { get; set; } = true;
        public string? SkipUpdateVersion { get; set; } = null;
        public bool CheckUpdates { get; set; } = true;
        public bool AutomaticallyUpdate { get; set; } = false;
        public string? CurrentTheme { get; set; } = null;
        public string? DataDownloadUrl { get; set; } = null;
        public int LoginCount { get; set; } = 0;
    }
}