using System;

namespace ExpenseManager.BusinessLogic
{
    /// <summary>
    ///     Simple class for generating nice colors for graphs
    /// </summary>
    public class ColorGeneratorService
    {
        public const string White = "#FFFFFF";
        public const string Black = "#000000";
        public const string Transparent= "rgba(0,0,0,0)";
        

        private static readonly string[] ColourValues =
        {
            "#727272", "#f1595f", "#79c36a", "#599ad3", "#f9a65a", "#9e66ab", "#cd7058", "#d77fb3"
        };

        private int _index;

        /// <summary>
        ///     Generate color for provided index
        /// </summary>
        /// <param name="index"> index for generation</param>
        /// <returns>code of color in #RRGGBB format</returns>
        public string GenerateColorForIndex(int index)
        {
            var normalizedIndex = 1;
            if (index != int.MinValue)
            {
                normalizedIndex = Math.Abs(index)%ColourValues.Length;
            }

            // make number in range of array (currently used for color generation)

            return ColourValues[normalizedIndex];
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
            return ColourValues[this._index++];
        }
    }
}