using System;
using System.Collections.Generic;
using static Infrastructure.Theme.ThemeData;

namespace Infrastructure.Theme
{
    /// <summary>
    /// Provides a lightweight key/value serializer for Theme instances so we can persist
    /// strings like "ForeColor=Override;BackColor=Secondary;" and hydrate them back later.
    /// </summary>
    public static class ThemeSerializer
    {
        private static readonly char[] Separator = new[] { ';' };

        /// <summary>
        /// Parses the serialized form ("Key=Value;Key2=Value2;") into a Theme instance.
        /// Missing keys fall back to the defaults declared on <see cref="ThemeData"/>.
        /// </summary>
        public static ThemeData Deserialize(object? serializedObj)
        {
            var theme = new ThemeData();

            if (serializedObj == null || !(serializedObj is string))
            {
                return theme;
            }

            var serialized = serializedObj as string;

            if (string.IsNullOrWhiteSpace(serialized))
            {
                return theme;
            }

            var pairs = serialized.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var kvp = pair.Split('=', 2);
                if (kvp.Length != 2)
                {
                    continue;
                }

                var key = kvp[0].Trim();
                var value = kvp[1].Trim();

                ApplyProperty(theme, key, value);
            }

            return theme;
        }

        /// <summary>
        /// Serializes a Theme back into the key=value; format so it can be persisted.
        /// </summary>
        public static string Serialize(ThemeData theme)
        {
            if (theme == null)
            {
                throw new ArgumentNullException(nameof(theme));
            }

            var pairs = new List<string>
            {
                $"{nameof(ThemeData.ForeColor)}={theme.ForeColor}",
                $"{nameof(ThemeData.BackColor)}={theme.BackColor}",
                $"{nameof(ThemeData.FontSize)}={theme.FontSize}",
                $"{nameof(ThemeData.Font)}={theme.Font}"
            };

            return string.Join(';', pairs) + ';';
        }

        private static void ApplyProperty(ThemeData theme, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (string.Equals(key, nameof(ThemeData.ForeColor), StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(value, true, out Tags.Color result))
                {
                    theme.ForeColor = result;
                }
                return;
            }

            if (string.Equals(key, nameof(ThemeData.BackColor), StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(value, true, out Tags.Color result))
                {
                    theme.BackColor = result;
                }
                return;
            }

            if (string.Equals(key, nameof(ThemeData.FontSize), StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(value, true, out Tags.FontSize result))
                {
                    theme.FontSize = result;
                }
                return;
            }

            if (string.Equals(key, nameof(ThemeData.Font), StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(value, true, out Tags.Font result))
                {
                    theme.Font = result;
                }
            }
        }
    }
}
