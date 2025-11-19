using static Infrastructure.Theme.ThemeData;

namespace Infrastructure.Theme
{
    public interface ITheme
    {
        Color? GetColor(Tags.Color? color);
        Font? GetFont(Tags.Font? font, Tags.FontSize? fontSize);
        float? GetFontSize(Tags.FontSize? fontSize);
    }
}