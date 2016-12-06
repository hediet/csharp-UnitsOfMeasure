using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// Specifies a unit for the corresponding element
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue,
        Inherited = false, AllowMultiple = false)]
    public sealed class UnitAttribute : Attribute
    {
        /// <summary>
        /// Specifies a unit for the corresponding element
        /// </summary>
        /// <param name="unit">The unit, e.g. "m" or "Meter"</param>
        public UnitAttribute(string unit)
        {
            Unit = unit;
        }

        public string Unit { get; private set; }
    }
}
