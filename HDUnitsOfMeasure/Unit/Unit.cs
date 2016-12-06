using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// Represents the base of all units of measure.
    /// </summary>
    public abstract class Unit : IEquatable<Unit>, IFormattable
    {
        /// <summary>
        /// The DimensionlessUnit is a special unit which is a neutral element for multiplication and division. 
        /// </summary>
        public static readonly BaseUnit Dimensionless = new BaseUnit("1", "Dimensionless");

        public Unit(string abbreviation, string name)
        {
            Abbreviation = abbreviation;
            this.name = name;
        }



        private object syncRoot = new object();

        #region Coherence

        protected void RenderCacheInvalid()
        {
            lock (syncRoot)
            {
                isCoherent = null;
                transformationToCoherent = null;
            }
        }


        protected abstract bool GetIsCoherent();

        bool? isCoherent;

        /// <summary>
        /// Gets whether this unit is coherent (see http://en.wikipedia.org/wiki/Metric_system#Coherence).
        /// </summary>
        public bool IsCoherent
        {
            get
            {
                lock (syncRoot)
                {
                    if (!isCoherent.HasValue)
                        isCoherent = GetIsCoherent();
                    return isCoherent.Value;
                }
            }
        }

        protected abstract QuantityTransformation GetTransformationToCoherent();

        private QuantityTransformation transformationToCoherent;

        /// <summary>
        /// Gets a transformation which converts a value in this unit to the coherent unit.
        /// For example, the unit "kilometer" will return a transformation to "meter".
        /// </summary>
        /// <remarks>
        /// Because some units (°F) are not proportional to their base unit, derived units assume these nonproportional units as coherent.
        /// So °F will return a transformation to K, but km/°F will return a transformation only to m/°F.
        /// This is, since the transformation from °F to K can be expressed with K(°F) = a*°F+b and from km to m with m(km) = c*km.
        /// So km/°F to m/K would be m/K(km/°F) = m(km)/K(°F) = c*km / (a*°F+b). 
        /// Only if b = 0, this expression can be formed to: m/K(km/°F) = c/a * km/°F. Because km/°F is a single value (e.g. 5 km/°F), only in this case
        /// this value can be converted to m/K: m/K(5) = c/a * 5. In all other cases, this is not possible, as 5 km/1°F is different from 10 km/2°F.
        /// </remarks>
        public QuantityTransformation TransformationToCoherent
        {
            get
            {
                lock (syncRoot)
                {
                    if (transformationToCoherent == null)
                        transformationToCoherent = GetTransformationToCoherent();
                    return transformationToCoherent;
                }
            }
        }

        #endregion

        /// <summary>
        /// Checks whether this unit can be converted to <paramref name="targetUnit"/>.
        /// </summary>
        /// <param name="targetUnit">The target unit.</param>
        /// <returns><c>true</c>, if they can be converted, otherwise <c>false</c>.</returns>
        public bool IsUnit(Unit targetUnit)
        {
            return GetTransformationTo(targetUnit) != null;
        }

        /// <summary>
        /// Converts <paramref name="value"/> from this unit to <paramref name="targetUnit"/>.
        /// </summary>
        /// <param name="targetUnit">The target unit.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns><c>true</c>, if successful, otherwise <c>false</c></returns>
        public bool TryConvertTo(Unit targetUnit, ref double value)
        {
            QuantityTransformation transformation = GetTransformationTo(targetUnit);
            if (transformation != null)
            {
                value = transformation.Transform(value);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets a transformation from this unit to <paramref name="targetUnit"/>
        /// </summary>
        /// <param name="targetUnit">The target unit.</param>
        /// <returns>A transformation to convert quantities from this unit to <paramref name="targetUnit"/></returns>
        public virtual QuantityTransformation GetTransformationTo(Unit targetUnit)
        {
            QuantityTransformation thisToCoherent = this.GetTransformationToCoherent();
            QuantityTransformation targetToCoherent = targetUnit.GetTransformationToCoherent();

            QuantityTransformation coherentToTarget = targetToCoherent.Reverse();

            return thisToCoherent.Chain(coherentToTarget);
        }

        #region Equality

        /// <summary>
        /// Checks wether this unit is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns><c>true</c>, if equal, otherwise <c>false</c>. See <see cref="Equals(Unit)"/>.</returns>
        public sealed override bool Equals(object obj)
        {
            Unit u = obj as Unit;
            return u != null && Equals(u);
        }

        public abstract override int GetHashCode();

        /// <summary>
        /// Checks wether this unit is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The other unit.</param>
        /// <returns><c>true</c>, if both units have the same coherent unit and their transformation is equal.</returns>
        public bool Equals(Unit other)
        {
            if (other == null)
                return false;

            if (IsCoherent && other.IsCoherent)
                return IsSame(other);
            else
                return TransformationToCoherent.Equals(other.TransformationToCoherent);
        }

        protected abstract bool IsSame(Unit other);

        public static bool Equals(Unit u1, Unit u2)
        {
            if (object.Equals(u1, null) || object.Equals(u2, null))
                return object.Equals(u1, u2);
            return u1.Equals(u2);
        }

        public static bool operator ==(Unit u1, Unit u2)
        {
            return Equals(u1, u2);
        }

        public static bool operator !=(Unit u1, Unit u2)
        {
            return !Equals(u1, u2);
        }

        #endregion

        /// <summary>
        /// Performs a power operation.
        /// </summary>
        /// <example>dm.Pow(3) is equal to liter.</example>
        /// <param name="exponent">the exponent.</param>
        /// <returns>The new unit.</returns>
        public Unit Pow(int exponent)
        {
            UnitPartCollection parts = new UnitPartCollection();

            parts.Add(this, exponent);

            return DerivedUnit.GetUnitFromParts(parts.ToArray());
        }

        public static Unit operator *(Unit u1, Unit u2)
        {
            if (u1 == null)
                throw new ArgumentNullException("u1");
            if (u2 == null)
                throw new ArgumentNullException("u2");

            UnitPartCollection parts = new UnitPartCollection();

            parts.Add(u1, 1);
            parts.Add(u2, 1);

            return DerivedUnit.GetUnitFromParts(parts.ToArray());
        }

        public static Unit operator /(Unit u1, Unit u2)
        {
            if (u1 == null)
                throw new ArgumentNullException("u1");
            if (u2 == null)
                throw new ArgumentNullException("u2");

            UnitPartCollection parts = new UnitPartCollection();

            parts.Add(u1, 1);
            parts.Add(u2, -1);

            return DerivedUnit.GetUnitFromParts(parts.ToArray());
        }

        /// <summary>
        /// The abbreviation of this unit.
        /// </summary>
        public string Abbreviation { get; private set; }

        private string name;

        /// <summary>
        /// The localized name of this unit.
        /// </summary>
        public string Name { get { return GetName(CultureInfo.CurrentCulture); } }


        public override sealed string ToString()
        {
            return this.ToString(null, CultureInfo.CurrentCulture);
        }

        protected virtual string GetName(IFormatProvider formatProvider)
        {
            return name;
        }

        protected virtual string DefinitionToString(string format, IFormatProvider formatProvider)
        {
            return null;
        }

        public string ToString(string format, IFormatProvider formatProvider)
            //  e/E: Enclose Denominator in braces. d/D use denominator. i/I ignore exponent if equal to 1. 
            //  l/L use abbreviation/long form. f/F use definition
        {
            if (String.IsNullOrEmpty(format)) format = "eDIlF";
            if (formatProvider == null) formatProvider = CultureInfo.CurrentCulture;

            string result = null;

            if (format.Contains('L'))
                result = GetName(formatProvider);
            
            if (result == null)
                result = Abbreviation;

            if (result == null && !format.Contains("f"))
                result = DefinitionToString(format, formatProvider);

            return result;

            //    throw new FormatException(String.Format("The {0} format string is not supported.", format));
        }
    }
}
