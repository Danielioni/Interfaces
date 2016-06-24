/**********************************************************************

janoman: HL7 to XML-Parser

HL7ToXmlConverterFactory.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Factory implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    /// <summary>
    /// Factory for creating Converter
    /// <example>
    /// <code>
    /// string hl7File = ReadFile("AdtA08.hl7");
    /// string dictFile = "hl7v2xml.dat";
    /// //read grammar
    /// Stream dictionary = assembly.GetManifestResourceStream(dictFilePath);
    /// Dictionary&lt;string, string&gt; eventsMessages = new Dictionary&lt;string, string&gt;();
    /// // map events
    /// //mapping means that you can decide that e.g. A04 sould be handled like A01
    /// eventsMessages.Add("ADT_A01", "ADT_A01");
    /// eventsMessages.Add("ADT_A02", "ADT_A02");
    /// eventsMessages.Add("ADT_A03", "ADT_A03");
    /// eventsMessages.Add("ADT_A04", "ADT_A01");
    /// eventsMessages.Add("ADT_A05", "ADT_A01");
    /// eventsMessages.Add("ADT_A08", "ADT_A01");
    /// eventsMessages.Add("ADT_A12", "ADT_A12");
    /// //create converter by using factory
    /// IConverter converter = HL7ToXmlConverterFactory.Create();
    /// //strict mode = false means, that the parser will work in lazy mode
    /// converter.Properties.SetBoolProperty("strict-mode", false);
    /// //hl7-namespace = true means, that the HL7Xml will have the default namespace for hl72xml
    /// converter.Properties.SetBoolProperty("hl7-namespace", true);
    /// //init converter
    /// converter.Init(dictionary, eventsMessages);
    /// //convert
    /// string hl7XML2 = converter.Convert(message);
    /// </code>
    /// </example>
    /// </summary>
    
    public class HL7ToXmlConverterFactory
    {
        #region ctors
        private HL7ToXmlConverterFactory()
        {
        }
        #endregion ctors

        #region factory methods
        /// <summary>
        /// Create converter
        /// </summary>
        /// <returns>Reference to Converter Interface</returns>
        public static IConverter Create()
        {
            try
            {
                return new HL7ToXmlConverter();
            }
            catch (Exception e)
            {
                throw new HL7ToXmlConverterException("Factory can't create instance of IHL7XmlConverter", e);
            }
        }
        #endregion factory methods
    }
}
