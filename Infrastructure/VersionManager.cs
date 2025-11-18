using System.Reflection;

namespace Infrastructure
{
    public class VersionManager
    {

        public static string GetVersionCode()
        {
            return Assembly.GetExecutingAssembly()
                .GetName().Version!
                .ToString();
        }

        public static bool IsSmallerThan(string first, string second)
        {
            var first_sections = first.Split('.').Select(int.Parse).ToArray();
            var second_sections = second.Split('.').Select(int.Parse).ToArray();
            var maxLen = Math.Max(first_sections.Length, second_sections.Length);

            for (int i = 0; i < maxLen; i++)
            {
                int f = i < first_sections.Length ? first_sections[i] : 0;
                int s = i < second_sections.Length ? second_sections[i] : 0;

                if (f != s)
                    return f < s;
            }
            return false;
        }
    }
}