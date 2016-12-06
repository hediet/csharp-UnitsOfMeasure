using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// Parses the following formats: 60 s; 60 min;
    /// </summary>
    public class ScaledShiftedUnitParser : IUnitParser
    {
        IUnitParser abbreviatedUnitParser;

        /// <summary>
        /// Creates a new ScaledShiftedUnitParser.
        /// </summary>
        /// <param name="abbreviatedUnitParser">The parser used for resolving the underlaying units.</param>
        public ScaledShiftedUnitParser(IUnitParser abbreviatedUnitParser)
        {
            if (abbreviatedUnitParser == null)
                throw new ArgumentNullException("abbreviatedUnitParser");
            this.abbreviatedUnitParser = abbreviatedUnitParser;
        }

        public Unit ParseUnit(string unitStr, string resultUnitAbbreviation, string resultUnitName, bool throwException)
        {
            string[] items = unitStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (items.Length != 2)
                return UnitParserExtension.ReturnNullOrThrowFormatException(null, throwException);

            Unit unit = abbreviatedUnitParser.ParseUnit(items[1], null, null, throwException);
            if (unit == null)
                return null;
            try
            {
                double factor = double.Parse(items[0], NumberStyles.Float, CultureInfo.InvariantCulture); //double azAZ
                return new ScaledShiftedUnit(resultUnitAbbreviation, resultUnitName, unit, factor);
            }
            catch (FormatException ex)
            {
                return UnitParserExtension.ReturnNullOrThrowFormatException(null, ex, throwException);
            }
        }
    }
}
