namespace Infrastructure.Theme
{
    public static class ThemeManager
    {
        private static ITheme _selectedTheme = new Themes.Gruvbox();
        public static ITheme SelectedTheme
        {
            get
            {
                return _selectedTheme;
            }
            set
            {
                _selectedTheme = value;
                ThemeChanged?.Invoke();
            }
        }
        public static event Action? ThemeChanged;

        public static ITheme FromString(string? str)
        {
            return str switch
            {
                "gruvbox" => new Themes.Gruvbox(),
                "light-gruvbox" => new Themes.GruvboxLight(),
                "classic" => new Themes.Classic(),
                _ => new Themes.Gruvbox(),
            };
        }

        public static string ToString(ITheme theme)
        {
            if (theme is Themes.Gruvbox)
            {
                return "gruvbox";
            }
            else if (theme is Themes.GruvboxLight)
            {
                return "light-gruvbox";
            }
            else if (theme is Themes.Classic)
            {
                return "classic";
            }
            else
            {
                return "gruvbox";
            }
        }
    }
}