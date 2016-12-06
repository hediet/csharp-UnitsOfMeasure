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
    /// Parses the following formats: km, MHz, µs (m, Hz and s have to be registered).
    /// All avaible prefixes are defined in <see cref="Prefix"/>.
    /// </summary>
    public class PrefixedUnitParser : IUnitParser
    {
        private IUnitParser registeredUnitParser;

        /// <summary>
        /// Creates a new PrefixedUnitParser.
        /// </summary>
        /// <param name="registeredUnitParser">The parser used for resolving the underlaying units.</param>
        public PrefixedUnitParser(IUnitParser registeredUnitParser)
        {
            if (registeredUnitParser == null)
                throw new ArgumentNullException("registeredUnitParser");

            this.registeredUnitParser = registeredUnitParser;
        }

        public Unit ParseUnit(string unitStr, string resultUnitAbbreviation, string resultUnitName, bool throwFormatException)
        {
            if (unitStr == null)
                throw new ArgumentNullException("unitStr");

            if (unitStr.Length > 1)
            {
                Prefix prefix = Prefix.All.FirstOrDefault(p => unitStr.StartsWith(p.Abbreviation));

                if (prefix == null)
                    return UnitParserExtension.ReturnNullOrThrowFormatException(string.Format("Prefix '{0}' not found", prefix), throwFormatException);

                Unit unit = registeredUnitParser.ParseUnit(unitStr.Substring(prefix.Abbreviation.Length), null, null, throwFormatException);

                if (unit == null) //if throwException, registeredUnitParser.ParseUnit will throw the exception
                    return null;

                if (resultUnitName == null && unit.Name != null)
                    resultUnitName = prefix.Name + unit.Name.ToLowerInvariant();
                if (resultUnitAbbreviation == null)
                    resultUnitAbbreviation = unitStr;

                return new ScaledShiftedUnit(resultUnitAbbreviation, resultUnitName, unit, prefix.Factor);
            }
            else
                return UnitParserExtension.ReturnNullOrThrowFormatException(string.Format("Input string \"{0}\" is to short", unitStr), throwFormatException);
        }
    }
}
