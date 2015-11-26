using System;

namespace ExpenseManager.BusinessLogic.CommonServices
{
    /// <summary>
    ///     Simple class for generating nice colors for graphs
    /// </summary>
    public class ColorGeneratorService
    {
        public const string White = "#FFFFFF";
        public const string Black = "#000000";

        private static readonly string[] ColourValues =
        {
            "FF0000", "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "000000",
            "800000", "008000", "000080", "808000", "800080", "008080", "808080",
            "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
            "400000", "004000", "000040", "404000", "400040", "004040", "404040",
            "200000", "002000", "000020", "202000", "200020", "002020", "202020",
            "600000", "006000", "000060", "606000", "600060", "006060", "606060",
            "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
            "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0"
        };

        private int _index;

        /// <summary>
        ///     Generate color for provided index
        /// </summary>
        /// <param name="index"> index for generation</param>
        /// <returns>code of color in #RRGGBB format</returns>
        public string GenerateColorForIndex(int index)
        {
            // make number in range of array (currently used for color generation)
            var normalizedIndex = Math.Abs(index)%ColourValues.Length;
            return "#" + ColourValues[normalizedIndex];
        }

        /// <summary>
        ///     Generate random color
        /// </summary>
        /// <returns>code of color in #RRGGBB format</returns>
        public string GenerateColor()
        {
            if (this._index >= ColourValues.Length)
            {
                this._index = 0;
            }
            return "#" + ColourValues[this._index++];
        }
    }
}