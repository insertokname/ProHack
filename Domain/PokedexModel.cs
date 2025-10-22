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

            [JsonConverter(typeof(FormListJsonConverter))]
            public List<string>? NormalForms { get; set; }

            [JsonConverter(typeof(FormListJsonConverter))]
            public List<string>? ShinyForms { get; set; }
        }
    }
}
