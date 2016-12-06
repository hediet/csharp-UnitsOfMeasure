using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDLibrary.UnitsOfMeasure
{
    public interface IUnitRegistry
    {
        IDictionary<string, Unit> RegisteredUnits { get; }
    }
}
