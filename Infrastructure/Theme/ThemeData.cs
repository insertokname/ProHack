
namespace Infrastructure.Theme
{
    public class ThemeData
    {
        public Tags.Color ForeColor { get; set; } = Tags.Color.Primary;
        public Tags.Color BackColor { get; set; } = Tags.Color.Secondary;
        public Tags.FontSize FontSize { get; set; } = Tags.FontSize.M;
        public Tags.Font Font { get; set; } = Tags.Font.Normal;


        public static class Tags
        {
            public enum Color
            {
                Override,
                Primary,
                Secondary,
                Secondary1,
                Danger,
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