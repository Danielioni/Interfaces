/**********************************************************************

janoman: HL7 to XML-Parser

IXmlOutput.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Xml output interface.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    internal interface IXmlOutput
    {
        void XmlStartElement(HL7Element element);
        void XmlText(HL7Element element);
        void XmlEndElement(HL7Element element);
        string GetXml();
    }
}
