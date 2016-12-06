using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HDLibrary.UnitsOfMeasure;
using System.Net;

namespace ConsoleApplicationUnitsOfMeasureDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ComposedUnitParser parser = ComposedUnitParser.NewDefaultUnitParser(); //the parser for parsing any format

            //Load the inbuilt units
            //They are defined in "DefaultUnitLibrary.xml", inside the HDUnitsOfMeasure project
            XmlUnitLibrary library = XmlUnitLibrary.NewDefaultLibrary();
            library.LoadTo(parser.GetUnitRegistry(), parser);


            WebRequest.DefaultWebProxy = null; //Retrieving the system proxy would be very slow...
            OnlineCurrencyUnitLibrary currencyLibrary = new OnlineCurrencyUnitLibrary();
            currencyLibrary.LoadTo(parser.GetUnitRegistry());

            
            while (true)
            {
                double value = 0;
                Unit sourceUnit = null;
                while (sourceUnit == null)
                {
                    Console.Write("Enter a value and a unit (e.g. 10 km/h): ");
                    string unitStr = Console.ReadLine();
                    string[] parts = unitStr.Split(new char[] { ' ' }, 2);

                    if (parts.Length == 2) // a very primitiv method for separating the value and the unit
                    {
                        value = double.Parse(parts[0]); //caution: think of your current CultureInfo!
                        sourceUnit = parser.TryParseUnit(parts[1]);
                    }
                }

                Unit targetUnit = null;
                while (targetUnit == null)
                {
                    Console.Write("Enter a valid target unit: ");
                    string unitStr = Console.ReadLine();
                    targetUnit = parser.TryParseUnit(unitStr);
                }

                var transformation = sourceUnit.GetTransformationTo(targetUnit);

                if (transformation == null)
                    Console.WriteLine("Units are incompatible");
                else
                    Console.WriteLine(string.Format("Value: {0} {1}", transformation.Transform(value), targetUnit));                  

                Console.WriteLine();
            }
        }
    }
}
