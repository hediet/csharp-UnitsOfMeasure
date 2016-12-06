using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDLibrary.UnitsOfMeasure
{
    public static class NumericExtension
    {
        #region Meter

        /// <summary>
        /// Specifies the value as meter.
        /// Note: No conversion is made!
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The value in [m]</returns>
        [return: Unit("m")]
        public static int Meter(this int value)
        {
            return value * Units.Meter;
        }

        /// <summary>
        /// Specifies the value as meter.
        /// Note: No conversion is made!
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The value in [m]</returns>
        [return: Unit("m")]
        public static double Meter(this double value)
        {
            return value * Units.Meter;
        }

        #endregion

        #region Second

        /// <summary>
        /// Specifies the value as second.
        /// Note: No conversion is made!
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The value in [s]</returns>
        [return: Unit("s")]
        public static int Second(this int value)
        {
            return value * Units.Second;
        }

        /// <summary>
        /// Specifies the value as second.
        /// Note: No conversion is made!
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The value in [s]</returns>
        [return: Unit("s")]
        public static double Second(this double value)
        {
            return value * Units.Second;
        }

        #endregion

        #region Kilogram

        /// <summary>
        /// Specifies the value as kilogram.
        /// Note: No conversion is made!
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The value in [kg</returns>
        [return: Unit("kg")]
        public static int Kilogram(this int value)
        {
            return value * Units.Kilogram;
        }

        /// <summary>
        /// Specifies the value as kilogram.
        /// Note: No conversion is made!
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The value in [kg]</returns>
        [return: Unit("kg")]
        public static double Kilogram(this double value)
        {
            return value * Units.Kilogram;
        }

        #endregion
    }
}
