using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.ComponentModel;
using System.Globalization;

namespace HDLibrary.UnitsOfMeasure
{
    public sealed class CurrencyUnit : Unit, INotifyPropertyChanged
    {
        public static readonly BaseUnit CurrencyRoot = new BaseUnit("Currency", null);

        protected override bool GetIsCoherent()
        {
            return false;
        }

        protected override QuantityTransformation GetTransformationToCoherent()
        {
            return new LinearQuantityTransformation(this, CurrencyRoot, (double)Factor, 0);
        }

        public override int GetHashCode()
        {
            return Factor.GetHashCode() ^ typeof(CurrencyUnit).GetHashCode();
        }

        protected override bool IsSame(Unit other)
        {
            CurrencyUnit cu = other as CurrencyUnit;
            return cu != null && cu.Factor == Factor;
        }

        protected override string GetName(IFormatProvider formatProvider)
        {
            string lang = (formatProvider as CultureInfo).TwoLetterISOLanguageName;
            if (!names.ContainsKey(lang))
                lang = "en";

            return names[lang];
        }

        private Dictionary<string, string> names;

        public CurrencyUnit(string abbreviation, decimal factor, Dictionary<string, string> names)
            : base(abbreviation, null)
        {
            Factor = factor;
            this.names = names;
        }

        private object syncRoot = new object();
        private decimal factor;
        public decimal Factor
        {
            get { lock (syncRoot) { return factor; } }
            private set { lock (syncRoot) { factor = value; } }
        }

        internal void SetNewFactor(decimal factor)
        {
            if (Factor != factor)
            {
                Factor = factor;
                RenderCacheInvalid();

                var temp = PropertyChanged;
                if (temp != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Factor"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class OnlineCurrencyUnitLibrary 
    {
        ManualResetEvent handle = new ManualResetEvent(false);

        CurrencyUnit[] units = new CurrencyUnit[0];

        private void Work()
        {
            while (true)
            {
                XDocument document;
                var request = WebRequest.Create("http://www.afd.admin.ch/publicdb/newdb/mwst_kurse/wechselkurse.php");

                using (var response = request.GetResponse())
                {
                    document = XDocument.Load(response.GetResponseStream());
                }

                List<CurrencyUnit> current = new List<CurrencyUnit>();

                XContainer root = document.Elements().First();

                foreach (var devise in root.Elements())
                {
                    if (devise.Name.LocalName == "devise")
                    {
                        Dictionary<string, string> names = new Dictionary<string, string>();
                        string abbreviation = null;
                        int amount = 0;
                        decimal factor = 0;

                        foreach (var item in devise.Elements())
                        {
                            if (item.Name.LocalName.StartsWith("land_"))
                            {
                                names[item.Name.LocalName.Substring(5)] = item.Value;
                            }
                            else if (item.Name.LocalName == "waehrung")
                            {
                                string currency = item.Value;
                                string[] elements = currency.Split(' ');
                                amount = int.Parse(elements[0]);
                                abbreviation = elements[1];
                            }
                            else if (item.Name.LocalName == "kurs")
                            {
                                factor = decimal.Parse(item.Value, CultureInfo.InvariantCulture);
                            }
                        }

                        var unit = units.FirstOrDefault(u => u.Abbreviation == abbreviation);
                        if (unit == null)
                            unit = new CurrencyUnit(abbreviation, factor / amount, names);
                        else
                            unit.SetNewFactor(factor / amount);

                        current.Add(unit);
                    }
                }

                units = current.ToArray();

                handle.Set();

                Thread.Sleep(TimeSpan.FromHours(1));
            }


        }

        public OnlineCurrencyUnitLibrary()
        {
            Thread thread = new Thread(Work);
            thread.IsBackground = true;
            thread.Start();
        }

        public void LoadTo(IUnitRegistry registry)
        {
            handle.WaitOne();

            CurrencyUnit[] units = this.units;
            foreach (var unit in units)
                registry.RegisteredUnits[unit.Abbreviation] = unit;
        }
    }
}
