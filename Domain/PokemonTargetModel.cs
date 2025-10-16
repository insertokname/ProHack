namespace Domain
{
    public class PokemonTargetModel
    {
        public required int? Id { get; set; } // null = catch anything
        public required IsSpecialTargeting specialTargeting { get; set; }

        public bool MatchesTarget(int otherId, bool isSpecial)
        {
            bool matchesId;
            if (Id == null)
                matchesId = true;
            else
                matchesId = otherId == Id;

            return specialTargeting switch
            {
                IsSpecialTargeting.CatchOnlyTargetedSpecials => matchesId && isSpecial,
                IsSpecialTargeting.CatchTargetedNormalsAndTargetedSpecials => matchesId,
                IsSpecialTargeting.CatchTargetedNormalsAndAnySpecial => matchesId || isSpecial,
                _ => throw new ArgumentException("Invalid specialTarget selection!"),
            };
        }

        public enum IsSpecialTargeting
        {
            CatchOnlyTargetedSpecials,
            CatchTargetedNormalsAndTargetedSpecials,
            CatchTargetedNormalsAndAnySpecial,
        }
    }
}
