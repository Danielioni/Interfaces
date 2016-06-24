/**********************************************************************

janoman: HL7 to XML-Parser

Evaluater.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Evaluation helper class.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    /// <summary>
    /// Helper class
    /// </summary>
    internal class Evaluater
    {
        #region public static methods
        public static bool ThrowAndCallback(string message, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            Callback(ref callback, ref message, LogLevel.Error);
            ThrowException(ref message, typedException);
            return true;
        }
        
        public static void ThrowAndCallbackIfNull(object evaluate, string name, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            if (null == evaluate)
            {
                string message = string.Format("Object with name {0} is null.", name);
                Callback(ref callback, ref message, LogLevel.Error);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowIfNull(object evaluate, string name, bool typedException)
        {
            if (null == evaluate)
            {
                string message = string.Format("Object with name {0} is null.", name);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowAndCallbackIfNullOrIsEmpty(string value, string name, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            if (string.IsNullOrEmpty(value))
            {
                string message = string.Format("Value with name {0} is null.", name);
                Callback(ref callback, ref message, LogLevel.Error);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowIfNullOrIsEmpty(string value, string name, bool typedException)
        {
            if (string.IsNullOrEmpty(value))
            {
                string message = string.Format("Value with name {0} is null.", name);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowAndCallbackIfStringNotEqual(string toCompare, string compareWith, string message, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            if (null != toCompare && !toCompare.Equals(compareWith))
            {
                Callback(ref callback, ref message, LogLevel.Error);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowIfStringNotEqual(string toCompare, string compareWith, string message, bool typedException)
        {
            if (null != toCompare && !toCompare.Equals(compareWith))
                ThrowException(ref message, typedException);
        }

        public static void ThrowAndCallbackIfStringEqual(string toCompare, string compareWith, string message, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            if (null != toCompare && toCompare.Equals(compareWith))
            {
                Callback(ref callback, ref message, LogLevel.Error);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowIfStringEqual(string toCompare, string compareWith, string message, bool typedException)
        {
            if (null != toCompare && toCompare.Equals(compareWith))
                ThrowException(ref message, typedException);
        }

        public static void ThrowAndCallbackIfFalse(bool argument, string message, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            if (!argument)
            {
                Callback(ref callback, ref message, LogLevel.Error);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowIfFalse(bool argument, string message, bool typedException)
        {
            if (!argument)
                ThrowException(ref message, typedException);
        }

        public static void ThrowAndCallbackIfTrue(bool argument, string message, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            if (argument)
            {
                Callback(ref callback, ref message, LogLevel.Error);
                ThrowException(ref message, typedException);
            }
        }
        
        public static void ThrowIfTrue(bool argument, string message, bool typedException)
        {
            if (argument)
                ThrowException(ref message, typedException);
        }

        public static void ThrowAndCallbackIfLessThanParserNoValue(ParserState parserState, string message, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            if (parserState < ParserState.NoValue)
            {
                Callback(ref callback, ref message, LogLevel.Error);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowIfLessThanParserNoValue(ParserState parserState, string message, bool typedException)
        {
            if (parserState < ParserState.NoValue)
                ThrowException(ref message, typedException);
        }

        public static void ThrowAndCallbackIfParserError(ParserState parserState, string message, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            if (parserState <= ParserState.Error)
            {
                Callback(ref callback, ref message, LogLevel.Error);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowIfParserError(ParserState parserState, string message, bool typedException)
        {
            if (parserState <= ParserState.Error)
                ThrowException(ref message, typedException);
        }

        public static void ThrowAndCallbackIfCharLessThan(char toCompare, char compareWith, string message, bool typedException, ref HL7ConverterMessageDelegate callback)
        {
            if (toCompare < compareWith)
            {
                Callback(ref callback, ref message, LogLevel.Error);
                ThrowException(ref message, typedException);
            }
        }

        public static void ThrowIfCharLessThan(char toCompare,char compareWith,string message,bool typedException)
        {
            if (toCompare < compareWith)
                ThrowException(ref message, typedException);
        }
        #endregion public static methods

        #region private static methods
        private static void Callback(ref HL7ConverterMessageDelegate callback, ref string message, LogLevel level)
        {
            if (null != callback)
                callback(message, level);
        }

        private static void ThrowException(ref string message, bool typedException)
        {
            if (typedException)
                throw new HL7ToXmlConverterException(message);
            else
                throw new ApplicationException(message);
        }
        #endregion private static methods

    }
}
