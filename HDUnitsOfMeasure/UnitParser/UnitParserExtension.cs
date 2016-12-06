using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace HDLibrary.UnitsOfMeasure
{
    public static class UnitParserExtension
    {
        public static Unit ReturnNullOrThrowFormatException(string exceptionMessage, Exception innerException, bool throwFormatException)
        {
            if (throwFormatException)
                throw new FormatException(exceptionMessage, innerException);
            else
                return null;
        }

        public static Unit ReturnNullOrThrowFormatException(string exceptionMessage, bool throwException)
        {
            return ReturnNullOrThrowFormatException(exceptionMessage, null, throwException);
        }



        /// <summary>
        /// Parses the unit string. If unsuccessful, an exception will be thrown.
        /// </summary>
        /// <param name="parser">this</param>
        /// <param name="unitStr">The string to parse.</param>
        /// <returns>The parsed unit.</returns>
        public static Unit ParseUnit(this IUnitParser parser, string unitStr)
        {
            return parser.ParseUnit(unitStr, null, null, true);
        }

        /// <summary>
        /// Parses the unit string. If unsuccessful, no exception will be thrown.
        /// </summary>
        /// <param name="parser">this</param>
        /// <param name="unitStr">The string to parse.</param>
        /// <returns>The parsed unit or null if unsuccessful.</returns>
        public static Unit TryParseUnit(this IUnitParser parser, string unitStr)
        {
            return TryParse(parser, unitStr, null, null);
        }

        /// <summary>
        /// Parses the unit string. If unsuccessful, no exception will be thrown.
        /// </summary>
        /// <param name="parser">this</param>
        /// <param name="unitStr">The string to parse.</param>
        /// <param name="resultUnitAbbreviation">The abbreviation of the returned unit. Will be ignored, if unitStr references a registered unit.</param>
        /// <param name="resultUnitName">The name of the returned unit. Will be ignored, if unitStr references a registered unit.</param>
        /// <returns>The parsed unit or null if unsuccessful.</returns>
        public static Unit TryParse(this IUnitParser parser, string unitStr, string resultUnitAbbreviation, string resultUnitName)
        {
            return parser.ParseUnit(unitStr, resultUnitAbbreviation, resultUnitName, false);
        }
    }
}
