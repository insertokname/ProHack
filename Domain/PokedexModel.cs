using System.Text.Json.Serialization;

namespace Domain
{
    public class PokedexModel
    {
        public required List<PokedexEntryModel> Entries { get; set; }

        public class PokedexEntryModel
        {
            public required int ID { get; set; }
            public required string Name { get; set; }

            public string? DisplayName { get; set; }

            [JsonConverter(typeof(FormListJsonConverter))]
            public List<string>? NormalForms { get; set; }

            [JsonConverter(typeof(FormListJsonConverter))]
            public List<string>? ShinyForms { get; set; }

            public List<int>? LinkedDexEntries { get; set; }
        }
    }
}
