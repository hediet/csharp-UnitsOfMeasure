using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// A cached unit parser, which delegates the parsing and caches the result.
    /// </summary>
    public class CachedUnitParser : IUnitParser
    {
        IUnitParser underlayingUnitParser;

        /// <summary>
        /// Creates a new cached unit parser.
        /// </summary>
        /// <param name="underlayingUnitParser">The underlaying unit parser.</param>
        public CachedUnitParser(IUnitParser underlayingUnitParser)
        {
            if (underlayingUnitParser == null)
                throw new ArgumentNullException("underlayingUnitParser");
            this.underlayingUnitParser = underlayingUnitParser;
        }

        private Dictionary<string, Unit> cache = new Dictionary<string, Unit>();

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCache()
        {
            lock (cache)
            {
                cache.Clear();
            }
        }

        public Unit ParseUnit(string unitStr, string resultUnitAbbreviation, string resultUnitName, bool throwFormatException)
        {
            lock (cache)
            {
                Unit result;
                if (!cache.TryGetValue(unitStr, out result))
                {
                    result = underlayingUnitParser.ParseUnit(unitStr, resultUnitAbbreviation, resultUnitName, throwFormatException);
                    cache[unitStr] = result;
                }
                if (result == null && throwFormatException)
                    throw new FormatException(string.Format("Could not parse unit \"{0}\"", unitStr));

                return result;
            }
        }
    }
}
