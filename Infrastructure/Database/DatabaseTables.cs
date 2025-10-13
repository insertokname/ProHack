using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    public class DatabaseTables
    {
        public List<EncounterStatsModel> EncounterStatsModels { get; set; } = [];
    }
}