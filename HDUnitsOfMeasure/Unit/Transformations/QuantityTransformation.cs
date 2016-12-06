using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// Describes a transformation for quantities. A quantity is a number with an unit.
    /// </summary>
    public abstract class QuantityTransformation : IEquatable<QuantityTransformation>
    {
        public QuantityTransformation(Unit sourceUnit, Unit targetUnit)
        {
            if (sourceUnit == null)
                throw new ArgumentNullException("sourceUnit");

            if (targetUnit == null)
                throw new ArgumentNullException("targetUnit");

            SourceUnit = sourceUnit;
            TargetUnit = targetUnit;
        }

        /// <summary>
        /// The source unit.
        /// </summary>
        public Unit SourceUnit { get; private set; }

        /// <summary>
        /// The target unit.
        /// </summary>
        public Unit TargetUnit { get; private set; }

        /// <summary>
        /// Transforms a value from source unit to target unit.
        /// </summary>
        /// <param name="value">The value to transform in the source unit.</param>
        /// <returns>The transformed value in the target unit.</returns>
        public abstract double Transform(double value);

        /// <summary>
        /// Chains two transformations. This is only possible if this target unit is equal to the others source unit.
        /// </summary>
        /// <param name="other">The other transformation.</param>
        /// <returns>The chained transformation.</returns>
        public abstract QuantityTransformation Chain(QuantityTransformation other);

        /// <summary>
        /// Reverses this transformation, so that reversed(this(value)) = value. The source and target units are exchanged.
        /// </summary>
        /// <returns>The reversed transformation.</returns>
        public abstract QuantityTransformation Reverse();


        public sealed override bool Equals(object obj)
        {
            QuantityTransformation qt = obj as QuantityTransformation;
            if (qt != null)
                return Equals(qt);
            return false;
        }

        public override int GetHashCode()
        {
            return SourceUnit.GetHashCode() ^ TargetUnit.GetHashCode();
        }

        public virtual bool Equals(QuantityTransformation other)
        {
            return TargetUnit.Equals(other.TargetUnit);
        }

        public override string ToString()
        {
            return string.Format("{0:eDIlf} => {1:eDIlf}", SourceUnit, TargetUnit);
        }
    }
}
