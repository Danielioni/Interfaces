/**********************************************************************

janoman: HL7 to XML-Parser

IContentHandler.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief content handler interface.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    
    /// <summary>
    /// Interface for a Content handler
    /// </summary>
    internal interface IContentHandler : ICallback
    {
        /// <summary>
        /// start parsing of an element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        ParserState StartElement(HL7Element element);
        /// <summary>
        /// End parsing of an element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        ParserState EndElement(HL7Element element);
        /// <summary>
        /// Start parsing Complex Type
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        ParserState StartComplexType(HL7Element element);
        /// <summary>
        /// End parsing Complex Type
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        ParserState EndComplexType(HL7Element element);
        /// <summary>
        /// Start parsing Sequence
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        ParserState StartSequence(HL7Sequence sequence);
        /// <summary>
        /// End parsing Sequence
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        ParserState EndSequence(HL7Sequence sequence);
    }
}
