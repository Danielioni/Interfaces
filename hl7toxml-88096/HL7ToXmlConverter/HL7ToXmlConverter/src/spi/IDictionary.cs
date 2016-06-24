/**********************************************************************

janoman: HL7 to XML-Parser

IDictionary.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Dictionary interface.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    /// <summary>
    /// dictionary Interface
    /// </summary>
    internal interface IDictionary : ICallback
    {
        string Lookup(string tag);
        KeyValuePair<string,string> LookupAndAlsoAttributes(string tag);
    }
}
