/**********************************************************************

janoman: HL7 to XML-Parser

IParser.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Parser interface.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    
    internal interface IParser : ICallback
    {
        /// <summary>
        /// Parse dictionary
        /// </summary>
        /// <param name="grammarEntry">Grammar entry in the dictionary</param>
        /// <param name="grammarRootName">name of Grammar item</param>
        /// <returns>Status des Parser</returns>
        ParserState ParseDictionaryEntry(string grammarEntry, string grammarRootName);
        IContentHandler ContentHandler { set; }

    }
}
