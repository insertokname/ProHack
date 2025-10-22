using Domain.Models;

namespace Infrastructure.Database
{
    public class DatabaseTables
    {
        public List<EncounterStatsModel> EncounterStatsModels { get; set; } = [];
        public string? DiscordBotToken { get; set; } = null;
        public ulong? AuthorizedUserId { get; set; } = null;
        public ulong? AnnouncementChannelId { get; set; } = null;
    }
}