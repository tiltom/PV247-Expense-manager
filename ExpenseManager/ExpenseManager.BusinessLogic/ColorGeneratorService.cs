using System;
using ExpenseManager.BusinessLogic.ServicesConstants;

namespace ExpenseManager.BusinessLogic
{
    /// <summary>
    ///     Simple class for generating nice colors for graphs
    /// </summary>
    public class ColorGeneratorService
    {
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
                normalizedIndex = Math.Abs(index)%ColorGeneratorConstants.ColourValues.Length;
            }

            // make number in range of array (currently used for color generation)

            return ColorGeneratorConstants.ColourValues[normalizedIndex];
        }

        /// <summary>
        ///     Generate random color
        /// </summary>
        /// <returns>code of color in #RRGGBB format</returns>
        public string GenerateColor()
        {
            if (this._index >= ColorGeneratorConstants.ColourValues.Length)
            {
                this._index = 0;
            }
            return ColorGeneratorConstants.ColourValues[this._index++];
        }
    }
}