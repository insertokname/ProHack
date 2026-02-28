
namespace Infrastructure.Theme
{
    public class ThemeData
    {
        public required Tags.Color? ForeColor { get; set; }
        public const Tags.Color DEFAULT_FORE_COLOR = Tags.Color.Primary;
        public required Tags.Color? BackColor { get; set; }
        public const Tags.Color DEFAULT_BACK_COLOR = Tags.Color.Secondary;
        public required Tags.FontSize? FontSize { get; set; }
        public const Tags.FontSize DEFAULT_FONT_SIZE = Tags.FontSize.M;
        public required Tags.Font? Font { get; set; }
        public const Tags.Font DEFAULT_FONT = Tags.Font.Normal;


        public static class Tags
        {
            public enum Color
            {
                Override,
                Primary,
                Secondary,
                Secondary1,
                Danger,
                Ok,
            }

            public enum FontSize
            {
                Override,
                XS,
                S,
                M,
                L,
                XL,
            }

            public enum Font
            {
                Override,
                Normal,
            }
        }
    }
}