using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// This class represents a part of a derived unit, e.g. km/h or m^3.
    /// </summary>
    public sealed class UnitPart : IEquatable<UnitPart>, IFormattable
    {
        /// <summary>
        /// Creates a new UnitPart
        /// </summary>
        /// <param name="unit">The unit of this part</param>
        /// <param name="exponent">The exponent of this part</param>
        public UnitPart(Unit unit, int exponent)
        {
            if (exponent == 0)
                throw new ArgumentOutOfRangeException("exponent");
            if (unit == null)
                throw new ArgumentNullException("unit");

            Unit = unit;
            Exponent = exponent;
        }

        /// <summary>
        /// The underlaying unit
        /// </summary>
        public Unit Unit { get; private set; }

        /// <summary>
        /// The exponent of the unit.
        /// </summary>
        /// <example>
        /// An exponent of -2 means 1/Unit^2
        /// </example>
        public int Exponent { get; private set; }

        /// <summary>
        /// Formats this part into a readable string.
        /// </summary>
        /// <param name="invertExponent">This parameter inverts the exponent. This is useful if this unit part is part of a denominator.</param>
        /// <returns>The formatted string in the following format: unit[^exponent]. The last term is skipped if exponent is 1</returns>
        public string ToString(bool invertExponent, bool ignoreExponentOne)
        {
            int p = invertExponent ? -Exponent : Exponent;

            string result = Unit.ToString();
            if (p == 1 && ignoreExponentOne)
                return result;
            else
                return result + "^" + p;                
        }

        public override string ToString()
        {
            return ToString(false, true);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            bool ignoreExponentOne = format.Contains("I");
            return ToString(false, ignoreExponentOne);
        }

        public override bool Equals(object obj)
        {
            return obj is UnitPart && Equals((UnitPart)obj);
        }

        public bool Equals(UnitPart other)
        {
            return other.Exponent == Exponent && other.Unit.Equals(Unit);
        }

        public override int GetHashCode()
        {
            return Unit.GetHashCode() ^ Exponent;
        }
    }

}
