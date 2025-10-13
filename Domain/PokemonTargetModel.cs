namespace Domain
{
    public class PokemonTargetModel
    {
        public required int? Id { get; set; } // null = catch anything
        public required bool MustBeSpecial { get; set; }

        public bool MatchesTarget(int targetId, bool targetIsSpecial)
        {
            bool matchesId;
            if (Id == null)
                matchesId = true;
            else
                matchesId = targetId == Id;

            var matchesSpecial = targetIsSpecial == MustBeSpecial;

            return matchesSpecial && matchesId;
        }
    }
}
