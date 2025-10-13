
using System.Data.Common;
using System.Drawing;

namespace Application
{
    public class Constants
    {
        public static class GameStatus
        {
            public const string FOUND_TEXT = "Game found!";
            public static readonly Color FOUND_COLOR = Color.Green;
            public const string NOT_FOUND_TEXT = "Game not found!";
            public static readonly Color NOT_FOUND_COLOR = Color.Red;
        }

        public static class Default
        {
            public const string POSITION_NOT_FOUND = "N/A";
            public const string NO_CURRENT_ENCOUNTER = "none";
        }

        public static class Keyboard
        {
            public const uint INPUT_KEYBOARD = 1;
            public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
            public const uint KEYEVENTF_KEYUP = 0x0002;
            public const uint KEYEVENTF_SCANCODE = 0x0008;
            public const uint MAPVK_VK_TO_VSC = 0x00;
        }

        public static class PokemonTargetModel
        {
            public static Domain.PokemonTargetModel DefaultTarget()
            {
                return new()
                {
                    Id = null,
                    MustBeSpecial = false,
                };
            }
        }
    }
}
