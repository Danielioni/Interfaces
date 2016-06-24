/**********************************************************************

janoman: HL7 to XML-Parser

HL7AnalyzerException.cs

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
    /// <summary>
    /// Exception to handle any Analyzer error
    /// </summary>
    [Serializable]
    public class HL7AnalyzerException : HL7ToXmlConverterException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public HL7AnalyzerException()
            : base()
        {
        }

        /// <summary>
        /// Constructor to communicate message
        /// </summary>
        /// <param name="message">error message</param>
        public HL7AnalyzerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Exception to communicate message and nested exception
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="e">nested exception</param>
        public HL7AnalyzerException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
