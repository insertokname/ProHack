using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

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