using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Globalization;

namespace HDLibrary.UnitsOfMeasure
{
    /// <summary>
    /// Imports unit definitions from xml documents to IUnitRegistry objects.
    /// </summary>
    public class XmlUnitLibrary
    {
        private XDocument document;

        /// <summary>
        /// Creates a default unit library, using the inbuilt xml document.
        /// </summary>
        /// <returns>The library.</returns>
        public static XmlUnitLibrary NewDefaultLibrary()
        {
#if DEBUG
            return new XmlUnitLibrary(Assembly.GetExecutingAssembly().GetManifestResourceStream("HDLibrary.UnitsOfMeasure.DefaultUnitLibrary.xml"));
#else
            return new XmlUnitLibrary(); //more performant
#endif
        }

        /// <summary>
        /// Creates a new >XmlUnitLibrary, loading the xml document from a stream.
        /// </summary>
        /// <param name="xmlData">The stream</param>
        public XmlUnitLibrary(Stream xmlData) : this(XmlReader.Create(xmlData)) { }

        /// <summary>
        /// Creates a new XmlUnitLibrary from the passed xml document.
        /// The document has to be compliant with the XSD schema (namespace http://www.hediet.de/xsd/unitlibrary/1.0).
        /// </summary>
        /// <param name="xmlReader">The xml reader.</param>
        public XmlUnitLibrary(XmlReader xmlReader)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            XmlSchema schema = XmlSchema.Read(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("HDLibrary.UnitsOfMeasure.UnitsOfMeasureLibrary.xsd"),
                (object obj, ValidationEventArgs args) =>
                {
                    throw args.Exception;
                });

            settings.Schemas.Add(schema);

            Exception lastException = null;

            settings.ValidationEventHandler += (object sender, ValidationEventArgs e) =>
                {
                    lastException = e.Exception;
                };

            XmlReader validationReader = XmlReader.Create(xmlReader, settings);

            document = XDocument.Load(validationReader);

            if (lastException != null)
                throw lastException;
        }

        private XmlUnitLibrary() //private constructor for loading without verification
        {
            var reader = XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream("HDLibrary.UnitsOfMeasure.DefaultUnitLibrary.xml"));
            document = XDocument.Load(reader);
        }

        /// <summary>
        /// Loads all unit definitions within this xml document to the specified IUnitRegistry.
        /// Multiple libraries can be loaded into one registry.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="parser">The parser used for resolving unit references. The parser should be able to parse all units passed to the registry.</param>
        public void LoadTo(IUnitRegistry registry, IUnitParser unitParser)
        {
            if (registry == null)
                throw new ArgumentNullException("registry");
            if (unitParser == null)
                throw new ArgumentNullException("unitParser");

            foreach (var item in document.Elements().First().Elements())
            {
                string name = item.Attribute("Name").Value;
                string abbr = item.Attribute("Abbr").Value;
                if (item.Name.LocalName == "BaseUnit")
                {
                    registry.RegisteredUnits[abbr] = new BaseUnit(abbr, name);
                }
                else if (item.Name.LocalName == "ScaledShiftedUnit")
                {
                    Unit underlayingUnit = unitParser.ParseUnit(item.Attribute("UnderlayingUnit").Value);

                    double offset = 0;
                    if (item.Attribute("Offset") != null)
                        offset = double.Parse(item.Attribute("Offset").Value, NumberFormatInfo.InvariantInfo);

                    registry.RegisteredUnits[abbr] = new ScaledShiftedUnit(abbr, name, underlayingUnit, double.Parse(item.Attribute("Factor").Value, NumberFormatInfo.InvariantInfo), offset);
                }
                else if (item.Name.LocalName == "DerivedUnit")
                {
                    List<UnitPart> unitParts = new List<UnitPart>();
                    foreach (var unitPartElement in item.Elements())
                    {
                        if (unitPartElement.Name.LocalName == "UnitPart")
                        {
                            Unit unitPart = unitParser.ParseUnit(unitPartElement.Attribute("Unit").Value);

                            int exponent = int.Parse(unitPartElement.Attribute("Exponent").Value, NumberFormatInfo.InvariantInfo);
                            unitParts.Add(new UnitPart(unitPart, exponent));
                        }
                    }

                    registry.RegisteredUnits[abbr] = DerivedUnit.GetUnitFromParts(abbr, name, unitParts.ToArray());
                }
            }
        }
    }
}
