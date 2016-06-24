/**********************************************************************

janoman: HL7 to XML-Parser

Enums.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Enumerations.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    /// <summary>
    /// Log level to map level to Logger frameworks like log4net
    /// simulary to log4net
    /// </summary>
    public enum LogLevel : int
    {
        /// <summary>
        /// DEBUG Level
        /// </summary>
        Debug = 0,
        /// <summary>
        /// INFO level
        /// </summary>
        Info = 1,
        /// <summary>
        /// WARN level
        /// </summary>
        Warn = 2,
        /// <summary>
        /// ERROR Level
        /// </summary>
        Error = 3,
        /// <summary>
        /// FATAL level
        /// </summary>
        Fatal = 4
    }

    internal enum HL7Token : int
    {
        SegmentTerminator = 1,
        FieldSeparator = 2,
        ComponentSeparator = 3,
        SubComponenteSeparator = 4,
        RepeatSeparator = 5,
        EOF = 6,
        Error = 0
    }

    internal enum ParseDictionaryEntryState : int
    {
        Undefined = 0,
        Begin = 1,
        End = 2,
        Error = 3,
        TrimLeft = 4,
        ReadType = 5,
        ReadSequence = 6,
        EvaluateDelimeter = 7
    }

    internal enum TypeOccur : uint
    {
        Required = 0,
        Optional = 1,
        Repeatable = 2
    }
    [Flags]
    internal enum ParserState : int
    {
        Error = -1,
        IllegalChar = -2,
        Parse2Failed = -3,
        UnknownState = -4,
        Error1 = -5,
        ErrorOpenFile = -6,
        /// <summary>
        /// No value found
        /// </summary>
        NoValue = 0,
        /// <summary>
        /// value found
        /// </summary>
        Value = 1,
        /// <summary>
        /// item may repeat
        /// </summary>
        Repeatable = 2,
        /// <summary>
        /// after recursion
        /// </summary>
        Recursion = 4,
        /// <summary>
        /// if segment tag was matched
        /// </summary>
        Segment = 8,

        Begin = 8,
        End = 9,
        //Error = 10,
        TrimLeft = 11,
        ReadElement = 12,
        ReadSequence = 13,
        ReadDelimeter = 14,
        TrimRight = 15,
        SequenceOccur = 16,
        SequenceTrim = 17

    }

}
