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
    /// Parses the following formats: km/h; m^3; A^1*s^1*V^-1*m^-1; A * s / V * m.
    /// </summary>
    public class DerivedUnitParser : IUnitParser
    {
        IUnitParser abbreviatedUnitParser;

        /// <summary>
        /// Creates a new DerivedUnitParser.
        /// </summary>
        /// <param name="abbreviatedUnitParser">The parser to parse all abbreviated units, e.g. m or V.</param>
        public DerivedUnitParser(IUnitParser abbreviatedUnitParser)
        {
            if (abbreviatedUnitParser == null)
                throw new ArgumentNullException("abbreviatedUnitParser");
            this.abbreviatedUnitParser = abbreviatedUnitParser;
        }

        private UnitPart GetUnitPart(string str, bool negative = false)
        {
            string[] items = str.Split('^');
            if (items.Length < 3)
            {
                Unit u = abbreviatedUnitParser.ParseUnit(items[0], null, null, false);
                if (u != null)
                {
                    int exponent = 1;

                    if (items.Length == 1 || int.TryParse(items[1], out exponent))
                    {
                        if (negative)
                            exponent *= -1;

                        return new UnitPart(u, exponent);
                    }
                }
            }

            return null;
        }

        public Unit ParseUnit(string unitStr, string resultUnitAbbreviation, string resultUnitName, bool throwFormatException)
        {
            if (unitStr == null)
                throw new ArgumentNullException("unitStr");

            unitStr = unitStr.Replace("/", " / ").Replace("*", " * ");
            string[] items = unitStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //km * m^2 / h^-1 * s^2
            if (items.Length % 2 == 0)
                return UnitParserExtension.ReturnNullOrThrowFormatException(string.Format("Invalid format: \"{0}\"", unitStr), throwFormatException);

            bool onDenominator = false;

            List<UnitPart> parts = new List<UnitPart>();

            for (int i = 0; i < items.Length; i += 2)
            {
                UnitPart part = GetUnitPart(items[i], onDenominator);
                if (part == null)
                    return UnitParserExtension.ReturnNullOrThrowFormatException(null, throwFormatException);

                if (i != items.Length - 1)
                {
                    string op = items[i + 1];

                    if (op != "*" && op != "/")
                        return UnitParserExtension.ReturnNullOrThrowFormatException(null, throwFormatException);

                    if (op == "/")
                    {
                        if (onDenominator)
                            return UnitParserExtension.ReturnNullOrThrowFormatException(null, throwFormatException);
                        else
                            onDenominator = true;
                    }
                }

                parts.Add(part);
            }

            return DerivedUnit.GetUnitFromParts(resultUnitAbbreviation, resultUnitName, parts.ToArray());
        }
    }

}
