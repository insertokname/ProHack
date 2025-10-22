namespace Domain
{
    public class PokemonTargetModel
    {
        public required int? Id { get; set; } // null = catch anything
        public required bool MustBeShiny { get; set; }
        public required bool MustBeEvent { get; set; }

        public bool MatchesTarget(int otherId, bool isEvent, bool isShiny)
        {
            bool matchesId;
            if (Id == null)
                matchesId = true;
            else
                matchesId = otherId == Id;

            if (MustBeEvent)
                return matchesId && isEvent;
            if (MustBeShiny)
                return matchesId && isShiny;

            return matchesId;
        }
    }
}
