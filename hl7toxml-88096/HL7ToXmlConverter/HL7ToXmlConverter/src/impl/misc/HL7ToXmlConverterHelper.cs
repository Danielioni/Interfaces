/**********************************************************************

janoman: HL7 to XML-Parser

HL7ToXmlconverterHelper.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Helper class.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    using System.IO;

    internal class HL7ToXmlConverterHelper
    {
        public static string GetStringFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.Default))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
