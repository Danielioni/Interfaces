/**********************************************************************

janoman: HL7 to XML-Parser

HL7DictionaryException.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Exception.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    [Serializable]
    class HL7DictionaryException : HL7ToXmlConverterException
    {
        public HL7DictionaryException() : base()
        {
        }

        public HL7DictionaryException(string message)
            : base(message)
        {
        }

        public HL7DictionaryException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
