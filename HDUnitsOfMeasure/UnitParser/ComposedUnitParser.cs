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
    /// A composed parser, which delegates the parsing to multiple other parsers.
    /// </summary>
    public class ComposedUnitParser : IUnitParser
    {
        /// <summary>
        /// Returns a preconfigured composed parser with all necessary parsers.
        /// </summary>
        /// <remarks>
        /// The registered parsers are: RegisteredUnitParser, PrefixedUnitParser, ScaledShiftedUnitParser and DerivedUnitParser.
        /// </remarks>
        /// <returns>The composed unit parser.</returns>
        public static ComposedUnitParser NewDefaultUnitParser()
        {
            ComposedUnitParser parser = new ComposedUnitParser();
            RegisteredUnitParser rup = new RegisteredUnitParser();
            PrefixedUnitParser pup = new PrefixedUnitParser(rup);
            
            ComposedUnitParser basicParser = new ComposedUnitParser();
            basicParser.UnitParsers.Add(rup);
            basicParser.UnitParsers.Add(pup);

            parser.UnitParsers.Add(rup);
            parser.UnitParsers.Add(pup);

            parser.UnitParsers.Add(new ScaledShiftedUnitParser(basicParser));
            parser.UnitParsers.Add(new DerivedUnitParser(basicParser));

            return parser;
        }

        /// <summary>
        /// Creates a new, empty composed unit parser.
        /// </summary>
        public ComposedUnitParser()
        {
            UnitParsers = new List<IUnitParser>();
        }

        /// <summary>
        /// Gets the first parser within UnitParsers which implements IUnitRegistry.
        /// </summary>
        /// <returns>The first parser within UnitParsers which implements IUnitRegistry. If no parser is found, null is returned.</returns>
        public IUnitRegistry GetUnitRegistry() 
        {
            return UnitParsers.OfType<IUnitRegistry>().FirstOrDefault();
        }

        /// <summary>
        /// All registered parsers.
        /// </summary>
        public IList<IUnitParser> UnitParsers { get; private set; }

        public Unit ParseUnit(string unitStr, string resultUnitAbbreviation, string resultUnitName, bool throwFormatException)
        {
            foreach (IUnitParser parser in UnitParsers)
            {
                Unit unit = parser.ParseUnit(unitStr, null, null, false);
                if (unit != null)
                    return unit;
            }

            return UnitParserExtension.ReturnNullOrThrowFormatException(
                string.Format("None of the registered parsers is able to parse the string \"{0}\"", unitStr), throwFormatException);
        }
    }
}
