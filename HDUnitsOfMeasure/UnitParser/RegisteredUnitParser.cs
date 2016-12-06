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
    /// A parser which compares the string to parse with all abbreviations of the registered units.
    /// </summary>
    public class RegisteredUnitParser : IUnitParser, IUnitRegistry
    {
        /// <summary>
        /// Creates a new RegisteredUnitParser. 1 is registered by default to  <see cref="Unit.Dimensionless"/>.
        /// </summary>
        public RegisteredUnitParser()
        {
            RegisteredUnits = new Dictionary<string, Unit>();
            RegisteredUnits["1"] = Unit.Dimensionless;
        }

        /// <summary>
        /// All registered units.
        /// </summary>
        public IDictionary<string, Unit> RegisteredUnits { get; private set; }

        public Unit ParseUnit(string unitStr, string resultUnitAbbreviation, string resultUnitName, bool throwException)
        {
            Unit unit;
            if (RegisteredUnits.TryGetValue(unitStr, out unit))
                return unit;
            return UnitParserExtension.ReturnNullOrThrowFormatException(string.Format("Unit with abbreviation \"{0}\" not found", unitStr), throwException);
        }
    }
}
