using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDLibrary.UnitsOfMeasure
{
    public interface ICouldBeUnproportional
    {
        /// <summary>
        /// Gets whether this unit is unproportional. <c>true</c>, if a proportional transformation to the coherent unit is not possible, otherwise <c>false</c>.
        /// </summary>
        bool IsUnproportional { get; }

        /// <summary>
        /// Gets a proportional transformation to the most possible coherent unit.
        /// So k°F (kilo fahrenheit) will be transformed to °F, but not to K (which is the actual coherent unit).
        /// </summary>
        QuantityTransformation ProportionalTransformationToCoherent { get; }
    }
}
