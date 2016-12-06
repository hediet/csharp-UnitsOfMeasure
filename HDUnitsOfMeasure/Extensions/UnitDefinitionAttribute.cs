using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// Defines a new unit
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class UnitDefinitionAttribute : Attribute
    {
        /// <summary>
        /// Defines a new base unit
        /// </summary>
        /// <param name="unitAbbr">The abbreviation of the unit, e.g. "m"</param>
        /// <param name="unitName">The name of the unit, e.g. "Meter"</param>
        public UnitDefinitionAttribute(string unitAbbr, string unitName)
        {
            UnitAbbr = unitAbbr;
            UnitName = unitName;
            Definition = null;
        }

        /// <summary>
        /// Defines a new scaled, shifted or derived unit
        /// </summary>
        /// <param name="unitAbbr">The abbreviation of the unit, e.g. "m"</param>
        /// <param name="unitName">The name of the unit, e.g. "Meter"</param>
        /// <param name="definition">The definition, e.g. "1000 m" or "m / s"</param>
        public UnitDefinitionAttribute(string unitAbbr, string unitName, string definition)
        {
            UnitAbbr = unitAbbr;
            UnitName = unitName;
            Definition = definition;
        }

        public string UnitAbbr { get; private set; }
        public string UnitName { get; private set; }
        public string Definition { get; private set; }
    }
}
