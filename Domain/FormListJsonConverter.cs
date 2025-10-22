using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain
{
    internal sealed class FormListJsonConverter : JsonConverter<List<string>?>
    {
        public override List<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected array while parsing forms.");

            var forms = new List<string>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                switch (reader.TokenType)
                {
                    case JsonTokenType.Number:
                        forms.Add(reader.GetInt32().ToString());
                        break;
                    case JsonTokenType.String:
                        forms.Add(reader.GetString() ?? string.Empty);
                        break;
                    default:
                        throw new JsonException("Unsupported token inside form list.");
                }
            }

            return forms;
        }

        public override void Write(Utf8JsonWriter writer, List<string>? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();
            foreach (var entry in value)
            {
                if (int.TryParse(entry, out var number) && entry == number.ToString())
                {
                    writer.WriteNumberValue(number);
                }
                else
                {
                    writer.WriteStringValue(entry);
                }
            }

            writer.WriteEndArray();
        }
    }
}
