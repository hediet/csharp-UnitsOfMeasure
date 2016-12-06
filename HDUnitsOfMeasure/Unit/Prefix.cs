using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// Represents a mathematical prefix like "kilo" or "Mega".
    /// </summary>
    public sealed class Prefix
    {
        /// <summary>
        /// Gets the abbreviation of the prefix.
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Gets the full name of the prefix.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the factor of the prefix, e.g. 1000 for "kilo".
        /// </summary>
        public double Factor { get; private set; }

        public static implicit operator double(Prefix p)
        {
            return p.Factor;
        }

        private Prefix(string abbreviation, string name, double factor)
        {
            Abbreviation = abbreviation;
            Name = name;
            Factor = factor;

            prefixes.Add(abbreviation, this);
        }

        private static Prefix FromPower(string abbreviation, string name, int tenthPower)
        {
            return new Prefix(abbreviation, name, Math.Pow(10, tenthPower));
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Abbreviation);
        }


        private static readonly Dictionary<string, Prefix> prefixes = new Dictionary<string, Prefix>();

        //taken from http://en.wikipedia.org/wiki/Metric_prefix
        public static readonly Prefix Yotta = Prefix.FromPower("Y", "Yotta", 24);
        public static readonly Prefix Zetta = Prefix.FromPower("Z", "Zetta", 21);
        public static readonly Prefix Exa = Prefix.FromPower("E", "Exa", 18);
        public static readonly Prefix Peta = Prefix.FromPower("P", "Peta", 15);
        public static readonly Prefix Tera = Prefix.FromPower("T", "Tera", 12);
        public static readonly Prefix Giga = Prefix.FromPower("G", "Giga", 9);
        public static readonly Prefix Mega = Prefix.FromPower("M", "Mega", 6);
        public static readonly Prefix Kilo = Prefix.FromPower("k", "Kilo", 3);
        public static readonly Prefix Hecto = Prefix.FromPower("h", "Hecto", 2);
        public static readonly Prefix Deca = Prefix.FromPower("da", "Deca", 1);
        public static readonly Prefix Deci = Prefix.FromPower("d", "Deci", -1);
        public static readonly Prefix Centi = Prefix.FromPower("c", "Centi", -2);
        public static readonly Prefix Milli = Prefix.FromPower("m", "Milli", -3);
        public static readonly Prefix Micro = Prefix.FromPower("µ", "Micro", -6);
        public static readonly Prefix Nano = Prefix.FromPower("n", "Nano", -9);
        public static readonly Prefix Pico = Prefix.FromPower("p", "Pico", -12);
        public static readonly Prefix Femto = Prefix.FromPower("f", "Femto", -15);
        public static readonly Prefix Atto = Prefix.FromPower("a", "Atto", -18);
        public static readonly Prefix Zepto = Prefix.FromPower("z", "Zepto", -21);
        public static readonly Prefix Yocto = Prefix.FromPower("y", "Yocto", -24);

        /// <summary>
        /// Gets a prefix by its abbreviation.
        /// </summary>
        /// <param name="abbreviation">The abbreviation of the prefix.</param>
        /// <returns>The associated prefix, if no prefix was found, <c>null</c>.</returns>
        public static Prefix Get(string abbreviation)
        {
            Prefix p;
            if (prefixes.TryGetValue(abbreviation, out p))
                return p;
            else
                return null;
        }

        /// <summary>
        /// Gets all SI prefixes (http://en.wikipedia.org/wiki/Metric_prefix).
        /// </summary>
        public static IEnumerable<Prefix> All { get { return prefixes.Values; } }
    }
}
