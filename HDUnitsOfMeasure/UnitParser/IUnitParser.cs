using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace HDLibrary.UnitsOfMeasure
{
    public interface IUnitParser
    {
        /// <summary>
        /// Parses a unit string.
        /// </summary>
        /// <param name="unitStr">The string to parse.</param>
        /// <param name="resultUnitAbbreviation">The abbreviation of the returned unit. Will be ignored, if unitStr references a registered unit.</param>
        /// <param name="resultUnitName">The name of the returned unit. Will be ignored, if unitStr references a registered unit.</param>
        /// <param name="throwFormatException">Specifies whether exceptions should be thrown or null should be returned.</param>
        /// <returns>The parsed unit, or null if parsing failed and throwFormatException is false.</returns>
        Unit ParseUnit(string unitStr, string resultUnitAbbreviation, string resultUnitName, bool throwFormatException);
    }
}
