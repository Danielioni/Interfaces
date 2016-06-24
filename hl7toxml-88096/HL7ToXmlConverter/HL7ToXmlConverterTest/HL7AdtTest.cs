using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL7ToXmlConverterTest
{

    using System.Resources;
    using System.IO;
    using Janoman.Healthcare.HL7;
    using System.Reflection;

    [TestClass]
    public class HL7AdtTest
    {

        public void TestProperty()
        {
        }

        [TestMethod]
        public void CrLfAdtTest()
        {
// Get all input from resources
            Assembly assembly = Assembly.GetExecutingAssembly();
            string hl7File = "AdtA08.hl7";
            string hl7FilePath = string.Format("{0}.{1}", assembly.GetName().Name, hl7File);
            Stream message = assembly.GetManifestResourceStream(hl7FilePath);
            string dictFile = "hl7v2xml.dat";
            string dictFilePath = string.Format("{0}.{1}", assembly.GetName().Name, dictFile);
// read grammar
            var dictionary = assembly.GetManifestResourceStream(dictFilePath); ;
            var eventsMessages = new Dictionary<string, string>();
// map events
// you have to any expeced event there, not only ADT. If you want to transfom ORU you have
// to map as well
// mapping means that you can decide that e.g. A04 sould be handled like A01
            eventsMessages.Add("ADT_A01", "ADT_A01");
            eventsMessages.Add("ADT_A02", "ADT_A02");
            eventsMessages.Add("ADT_A03", "ADT_A03");
            eventsMessages.Add("ADT_A04", "ADT_A01");
            eventsMessages.Add("ADT_A05", "ADT_A01");
            eventsMessages.Add("ADT_A08", "ADT_A01");
            eventsMessages.Add("ADT_A12", "ADT_A12");
// create converter by using factory
            var converter = HL7ToXmlConverterFactory.Create();
// strict mode = false means, that the parser will work in lazy mode
            converter.Properties.SetBoolProperty("strict-mode", false);
// hl7-namespace = true means, that the HL7Xml will have the default namespace for hl72xml
            converter.Properties.SetBoolProperty("hl7-namespace", true);
// set encoding directive
            converter.Properties.SetStringProperty("enconding", "ISO-8859-1");
// init converter
            converter.Init(dictionary, eventsMessages);
// convert
            string hl7XML2 = converter.Convert(message);
            Assert.IsFalse(string.IsNullOrEmpty(hl7XML2));
            File.WriteAllText(string.Format(@"{0}.{1}", assembly.Location, "test.xml"), hl7XML2, Encoding.Default);
        }
    }
}
