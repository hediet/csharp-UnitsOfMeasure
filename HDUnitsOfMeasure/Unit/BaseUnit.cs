using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// A base unit, which can be only identified by its abbreviation. See http://en.wikipedia.org/wiki/SI_base_unit.
    /// </summary>
    public sealed class BaseUnit : Unit
    {
        public BaseUnit(string unitAbbreviation, string unitName)
            : base(unitAbbreviation, unitName)
        {
            if (unitAbbreviation == null)
                throw new ArgumentNullException("unitAbbreviation");
        }

        public BaseUnit(string unitAbbreviation) : this(unitAbbreviation, null) { }

        protected override bool GetIsCoherent()
        {
            return true;
        }

        protected override QuantityTransformation GetTransformationToCoherent()
        {
            return new LinearQuantityTransformation(this, this, 1, 0);
        }

        public override int GetHashCode()
        {
            return Abbreviation.GetHashCode();
        }

        protected override bool IsSame(Unit other)
        {
            BaseUnit bu = other as BaseUnit;
            if (bu != null)
            {
                if (other.Abbreviation == Abbreviation)
                    return true;
            }
            return false;
        }
    }
}
