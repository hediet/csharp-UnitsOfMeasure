using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// Describes a linear transformation f(value) = Factor * value + Offset.
    /// </summary>
    public class LinearQuantityTransformation : QuantityTransformation
    {
        public LinearQuantityTransformation(Unit sourceUnit, Unit targetUnit, double factor, double offset)
            : base(sourceUnit, targetUnit)
        {
            Factor = factor;
            Offset = offset;
        }

        /// <summary>
        /// The factor.
        /// </summary>
        public double Factor { get; private set; }

        /// <summary>
        /// The offset.
        /// </summary>
        public double Offset { get; private set; }

        /// <summary>
        /// Transforms a value from source unit to target unit.
        /// </summary>
        /// <param name="value">The value to transform in the source unit.</param>
        /// <returns>The transformed value in the target unit.</returns>
        public override double Transform(double value)
        {
            return Factor * value + Offset;
        }

        public override QuantityTransformation Chain(QuantityTransformation transformation)
        {
            if (TargetUnit == transformation.SourceUnit)
            {
                if (transformation.GetType() == typeof(LinearQuantityTransformation)) //a.Combine(other) : a.Source->a.Target=other.Source->other.Target
                {
                    LinearQuantityTransformation lqt = transformation as LinearQuantityTransformation;

                    // this(x) := (o + f*x)
                    // lqt(x) := (lqt.o + lqt.f*x)
                    // this.Chain(lqt) = lqt(this(x)) = f*lqt.f*x + (lqt.o + lqt.f*o)

                    return new LinearQuantityTransformation(SourceUnit, lqt.TargetUnit, Factor * lqt.Factor, lqt.Offset + lqt.Factor * Offset);
                }
            }

            return null;
        }

        public override QuantityTransformation Reverse()
        {
            // this(x) := ax + b; reversed(x) := cx + d
            // reversed(this(x)) = x
            // c(ax + b) + d = x = cax + cb + d 
            // c = 1 / a; d = -cb = -b/a

            return new LinearQuantityTransformation(TargetUnit, SourceUnit, 1 / Factor, -Offset / Factor);
        }

        public override bool Equals(QuantityTransformation other)
        {
            LinearQuantityTransformation lqt = other as LinearQuantityTransformation;

            return lqt != null && Factor == lqt.Factor && Offset == lqt.Offset && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Factor.GetHashCode() ^ Offset.GetHashCode();
        }

        public override string ToString()
        {
            //TargetUnit = SourceUnit * Factor + Offset;
            string factor = "";
            if (Factor != 1)
                factor = " * " + Factor.ToString();

            string offset = "";
            if (Offset != 0)
                offset = " + " + Offset.ToString();

            return string.Format("{0}{2}{3} => {1}", SourceUnit, TargetUnit, factor, offset);
        }
    }
}
