
namespace Infrastructure.Theme
{
    public static class Themes
    {
        public interface IDefaultFontSizes : ITheme
        {
            float? ITheme.GetFontSize(ThemeData.Tags.FontSize? fontSize)
            {
                return fontSize switch
                {
                    ThemeData.Tags.FontSize.XS => 8F,
                    ThemeData.Tags.FontSize.S => 10F,
                    ThemeData.Tags.FontSize.M => 14F,
                    ThemeData.Tags.FontSize.L => 18F,
                    ThemeData.Tags.FontSize.XL => 24F,
                    _ => null,
                };
            }
        }

        public interface IMonoTheme : ITheme
        {
            Font? ITheme.GetFont(ThemeData.Tags.Font? font, ThemeData.Tags.FontSize? fontSize)
            {
                var fontSizeF = GetFontSize(fontSize);
                if (fontSizeF == null)
                {
                    return null;
                }

                return font switch
                {
                    ThemeData.Tags.Font.Normal => new Font("Cascadia Code", fontSizeF.Value, FontStyle.Bold),
                    _ => null
                };
            }
        }

        public interface ISegoeUITheme : ITheme
        {
            Font? ITheme.GetFont(ThemeData.Tags.Font? font, ThemeData.Tags.FontSize? fontSize)
            {
                var fontSizeF = GetFontSize(fontSize);
                if (fontSizeF == null)
                {
                    return null;
                }

                return font switch
                {
                    ThemeData.Tags.Font.Normal => new Font("Segoe UI Semibold", fontSizeF.Value, FontStyle.Bold),
                    _ => null
                };
            }
        }

        public class Gruvbox : IMonoTheme, IDefaultFontSizes
        {
            public Color? GetColor(ThemeData.Tags.Color? color)
            {
                return color switch
                {
                    ThemeData.Tags.Color.Primary => ColorTranslator.FromHtml("#98971a"),
                    ThemeData.Tags.Color.Secondary => ColorTranslator.FromHtml("#282828"),
                    ThemeData.Tags.Color.Secondary1 => ColorTranslator.FromHtml("#32302f"),
                    ThemeData.Tags.Color.Danger => ColorTranslator.FromHtml("#cc241d"),
                    ThemeData.Tags.Color.Ok => Color.Green,
                    _ => null
                };
            }
        }

        public class GruvboxLight : IMonoTheme, IDefaultFontSizes
        {
            public Color? GetColor(ThemeData.Tags.Color? color)
            {
                return color switch
                {
                    ThemeData.Tags.Color.Primary => ColorTranslator.FromHtml("#98971a"),
                    ThemeData.Tags.Color.Secondary => ColorTranslator.FromHtml("#fbf1c7"),
                    ThemeData.Tags.Color.Secondary1 => ColorTranslator.FromHtml("#f2e5bc"),
                    ThemeData.Tags.Color.Danger => ColorTranslator.FromHtml("#cc241d"),
                    ThemeData.Tags.Color.Ok => Color.Green,
                    _ => null
                };
            }
        }

        public class Classic : ISegoeUITheme, IDefaultFontSizes
        {
            public Color? GetColor(ThemeData.Tags.Color? color)
            {
                return color switch
                {
                    ThemeData.Tags.Color.Primary => Color.Crimson,
                    ThemeData.Tags.Color.Secondary => Color.LavenderBlush,
                    ThemeData.Tags.Color.Secondary1 => Color.Pink,
                    ThemeData.Tags.Color.Danger => Color.Crimson,
                    ThemeData.Tags.Color.Ok => Color.Green,
                    _ => null
                };
            }
        }
    }
}