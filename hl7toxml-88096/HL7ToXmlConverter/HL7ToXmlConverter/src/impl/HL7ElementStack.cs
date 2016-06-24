/**********************************************************************

janoman: HL7 to XML-Parser

HL7ElementStack.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Element Stack implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Janoman.Healthcare.HL7
{

    internal sealed class HL7ElementStack : List<HL7Element>
    {
        public void Dump(ref StringBuilder builder)
        {
            var elements = (List<HL7Element>)this;
            builder.Append("/");

            foreach (HL7Element e in elements)
                builder.Append(string.Format("{0}/", e.Name));
            
            builder.Append("\n");
        }

        public void Mark()
        {
            var elements = (List<HL7Element>)this;
            
            foreach (HL7Element e in elements)
                e.Occur = 1;
        }

        public void WriteXml(IXmlOutput xmlOut)
        {
            var elements = (List<HL7Element>)this;
            if (null == elements
                || 0 == elements.Count)
            {
                return;
            }

            foreach (HL7Element e in elements)
            {
                if (e.Open) continue;
                xmlOut.XmlStartElement(e);
                e.Open = true;

            }
            
            xmlOut.XmlText(elements[elements.Count - 1]);


            elements[elements.Count - 1].Open = false;
            xmlOut.XmlEndElement(elements[elements.Count - 1]);
        }

    }
}
