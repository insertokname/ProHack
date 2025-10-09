using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProQol
{
    internal class Constants
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
    }
}
