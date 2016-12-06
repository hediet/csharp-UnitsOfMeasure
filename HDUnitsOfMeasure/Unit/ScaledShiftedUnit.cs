using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDLibrary.UnitsOfMeasure
{
    public sealed class ScaledShiftedUnit : Unit, ICouldBeUnproportional
    {
        public ScaledShiftedUnit(string abbreviation, string name, Unit underlayingUnit, double factor, double offset)
            : base(abbreviation, name)
        {
            if (underlayingUnit == null)
                throw new ArgumentNullException("underlayingUnit");
            if (Factor == 1 && Offset == 0)
                throw new ArgumentOutOfRangeException("Factor", "Either factor or offset has to be different from default!");

            UnderlayingUnit = underlayingUnit;
            Factor = factor;
            Offset = offset;
        }

        public ScaledShiftedUnit(Unit underlayingUnit, double factor, double offset)
            : this(null, null, underlayingUnit, factor, offset) { }

        public ScaledShiftedUnit(string abbreviation, string name, Unit underlayingUnit, double factor)
            : this(abbreviation, name, underlayingUnit, factor, 0) { }


        public ScaledShiftedUnit(Unit underlayingUnit, double factor)
            : this(null, null, underlayingUnit, factor, 0) { }

        public Unit UnderlayingUnit { get; private set; }

        public double Factor { get; private set; }
        public double Offset { get; private set; }

        protected override string DefinitionToString(string format, IFormatProvider formatProvider)
        {
            string factor = "";
            if (Factor != 1)
                factor = " * " + Factor.ToString();

            string offset = "";
            if (Offset != 0)
                offset = " + " + Offset.ToString();

            return string.Format("({0}){1}{2}", UnderlayingUnit, factor, offset);
        }

        protected override bool GetIsCoherent()
        {
            return false;
        }

        protected override QuantityTransformation GetTransformationToCoherent()
        {
            var transformationToUnderlayingUnit = new LinearQuantityTransformation(this, UnderlayingUnit, Factor, Offset);
            return transformationToUnderlayingUnit.Chain(UnderlayingUnit.TransformationToCoherent);
        }

        public override int GetHashCode()
        {
            return Factor.GetHashCode() ^ Offset.GetHashCode() ^ UnderlayingUnit.GetHashCode();
        }

        protected override bool IsSame(Unit other)
        {
            ScaledShiftedUnit ssu = other as ScaledShiftedUnit;
            return ssu != null && ssu.Factor == Factor && ssu.Offset == Offset && ssu.UnderlayingUnit == UnderlayingUnit;
        }

        public bool IsUnproportional
        {
            get { return (Offset != 0); }
        }


        public QuantityTransformation ProportionalTransformationToCoherent
        {
            get
            {
                 //if this is unproportional, this unit is assumed to be coherent 
                 //(because a proportional transformation to the underlaying unit is not possible)
                 if (IsUnproportional)
                        return new LinearQuantityTransformation(this, this, 1, 0);

                if (UnderlayingUnit is ICouldBeUnproportional)
                {
                    var transformationToUnderlayingUnit = new LinearQuantityTransformation(this, UnderlayingUnit, Factor, Offset);
                    return transformationToUnderlayingUnit.Chain(((ICouldBeUnproportional)UnderlayingUnit).ProportionalTransformationToCoherent);
                }
                else
                    return TransformationToCoherent;
            }
        }
    }
}
