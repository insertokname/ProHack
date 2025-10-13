namespace Domain.Models
{
    public class EncounterStatsModel : Entity
    {
        public required int EncounteredPokemonId { get; set; }
        public required bool IsSpecial { get; set; }
        public required DateTime EncounterTime { get; set; }
    }
}