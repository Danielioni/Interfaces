/**********************************************************************

janoman: HL7 to XML-Parser

HL7XmlOutput.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Output implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Janoman.Healthcare.HL7
{
    
    internal sealed class HL7XmlOutput : IXmlOutput
    {

        #region ctors
        public HL7XmlOutput() : this("ISO-8859-1")
        {
        }
        public HL7XmlOutput(string encoding)
        {
            if (string.IsNullOrEmpty(encoding))
                encoding = "ISO-8859-1";

            this.xmlBuilder = new StringBuilder(string.Format("<?xml version=\"1.0\" encoding=\"{0}\"?>",encoding));
        }
        #endregion ctors

        #region private members
        private StringBuilder xmlBuilder = new StringBuilder();
        #endregion private members

        #region IXmlOutput Member

        public void XmlStartElement(HL7Element element)
        {
            Evaluater.ThrowIfNull(element, "Element", true);

            xmlBuilder.Append("\n");
            xmlBuilder.Append("<");
            xmlBuilder.Append(element.Name);
            foreach (KeyValuePair<string, string> attribute in element.Attributes)
            {
                xmlBuilder.Append(" ");
                xmlBuilder.Append(attribute.Key);
                xmlBuilder.Append("=\"");
                xmlBuilder.Append(EscapeXmlEntities(attribute.Value));
                xmlBuilder.Append("\"");
            }
            xmlBuilder.Append(">");

        }

        public void XmlText(HL7Element element)
        {
            Evaluater.ThrowIfNull(element, "Element", true);

            this.xmlBuilder.Append(EscapeXmlEntities(element.Text));
        }

        public void XmlEndElement(HL7Element element)
        {
            Evaluater.ThrowIfNull(element, "Element", true);

            xmlBuilder.Append("</");
            xmlBuilder.Append(element.Name);
            xmlBuilder.Append(">");
        }

        public string GetXml()
        {
            return this.xmlBuilder.ToString();
        }
        #endregion

        private string EscapeXmlEntities(string xml)
        {
            if (0 <= xml.IndexOf("&"))
                xml = xml.Replace("&", "&amp;");

            if (0 <= xml.IndexOf("<"))
                xml = xml.Replace("<", "&lt;");

            if (0 <= xml.IndexOf(">"))
                xml = xml.Replace(">", "&gt;");
            
            if (0 <= xml.IndexOf("'"))
                xml = xml.Replace("'", "&apos;");
            
            if (0 <= xml.IndexOf("\""))
                xml = xml.Replace("\"", "&quot;");
            
            return xml;
        }

    }
}
