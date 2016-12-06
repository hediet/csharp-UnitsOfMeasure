using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// A derived unit. See http://en.wikipedia.org/wiki/SI_derived_unit.
    /// </summary>
    public sealed class DerivedUnit : Unit
    {
        /// <summary>
        /// Gets a new unit from unit parts.
        /// </summary>
        /// <param name="unitAbbreviation">The abbreviation of the new unit.</param>
        /// <param name="unitName">The name of the new unit.</param>
        /// <param name="parts">The unit parts.</param>
        /// <returns>Unit.DimensionlessUnit, if the length of parts is 0. If the length is 1 and the exponent is 1, the first part is returned, otherwise a DerivedUnit.</returns>
        public static Unit GetUnitFromParts(string unitAbbreviation, string unitName, UnitPart[] parts)
        {
            if (parts.Length == 0)
                return Unit.Dimensionless;
            else if (parts.Length == 1 && parts.First().Exponent == 1)
                return parts.First().Unit;
            else
                return new DerivedUnit(unitAbbreviation, unitName, parts);
        }

        /// <summary>
        /// Gets a new unit from unit parts.
        /// </summary>
        /// <param name="parts">The unit parts.</param>
        /// <returns>Unit.DimensionlessUnit, if the length of parts is 0. If the length is 1 and the exponent is 1, the first part is returned, otherwise a DerivedUnit.</returns>
        public static Unit GetUnitFromParts(UnitPart[] parts)
        {
            return GetUnitFromParts(null, null, parts);
        }


        private DerivedUnit(UnitPart[] parts) : this(null, null, parts) { }

        private DerivedUnit(string abbreviation, string name, UnitPart[] parts)
            : base(abbreviation, name)
        {
            if (parts == null)
                throw new ArgumentNullException("parts");

            Parts = parts.OrderBy(u => u.Exponent).ThenBy(u => u.Unit.ToString()).ToArray();
        }

        public UnitPart[] Parts { get; private set; }

        protected override bool GetIsCoherent()
        {
            return IsSame(GetTransformationToCoherent().TargetUnit);
        }

        protected override QuantityTransformation GetTransformationToCoherent()
        {
            //sqrkm(sqrm) = (km(sqrt(sqrm)))^2
            //km(m) := x*m; h(s) := y*s
            //kmh(m/s) = km(m)/hour(s) = km(1)*m/(hour(1)*s) = m/s * km(1)/h(1)
            // e.g. x * km^3 * hour^2 => m^3 * s^2 (=> cm^3 * ms^2)

            double factor = 1;

            UnitPartCollection parts = new UnitPartCollection();

            foreach (var part
                in Parts.Select(p =>
                    new
                    {
                        Transformation = (p.Unit is ICouldBeUnproportional) ?
                            (LinearQuantityTransformation)((ICouldBeUnproportional)p.Unit).ProportionalTransformationToCoherent
                            : (LinearQuantityTransformation)p.Unit.TransformationToCoherent,
                        Exponent = p.Exponent
                    }))
            {
                factor *= Math.Pow(part.Transformation.Factor, part.Exponent);
                parts.Add(part.Transformation.TargetUnit, part.Exponent);
            }

            return new LinearQuantityTransformation(this, GetUnitFromParts(parts.ToArray()), factor, 0);
        }

        public override int GetHashCode()
        {
            return Parts.Aggregate(0, (hashcode, p) => hashcode ^ p.GetHashCode());
        }


        protected override bool IsSame(Unit other)
        {
            DerivedUnit du = other as DerivedUnit;
            if (du == null)
                return false;

            if (Parts.Length != du.Parts.Length)
                return false;

            for (int i = 0; i < Parts.Length; i++)
                if (!Parts[i].Equals(du.Parts[i]))
                    return false;

            return true;
        }


        protected override string DefinitionToString(string format, IFormatProvider formatProvider)
        {
            //  e/E: Enclose Denominator in braces. d/D use denominator. i/I ignore exponent if equal to 1. 
            //  l/L use abbreviation/long form. f/F use definition

            bool encloseDenominatorInBraces = format.Contains("E");
            bool useDenominator = format.Contains("D");
            bool ignoreExponentIfEqualTo1 = format.Contains("I");

            StringBuilder sb = new StringBuilder();

            var positive = Parts.Where(p => p.Exponent > 0 | !useDenominator).Select(p => p.ToString(false, format.Contains("I")));
            var negative = Parts.Where(p => p.Exponent < 0 && useDenominator).Select(p => p.ToString(true, format.Contains("I")));

            string[] positiveStr = positive.ToArray();
            string[] negativeStr = negative.ToArray();

            string result = "";

            if (positiveStr.Length > 0)
                result = string.Join(" * ", positiveStr);
            else
                result = "1";

            if (negativeStr.Length > 0)
            {
                result += " / ";
                if (negativeStr.Length > 1 && encloseDenominatorInBraces)
                    result += "(";

                result += string.Join(" * ", negativeStr);

                if (negativeStr.Length > 1 && encloseDenominatorInBraces)
                    result += ")";
            }

            return result;
        }
    }
}
