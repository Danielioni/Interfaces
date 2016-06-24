/**********************************************************************

janoman: HL7 to XML-Parser

CallbackHelper.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Helper class.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    internal class CallbackHelper
    {
        #region public static methods
        internal static void Debug(ref HL7ConverterMessageDelegate callback, string message)
        {
            Log(ref callback, ref message, LogLevel.Debug);
        }

        internal static void Info(ref HL7ConverterMessageDelegate callback, string message)
        {
            Log(ref callback, ref message, LogLevel.Info);
        }

        internal static void Error(ref HL7ConverterMessageDelegate callback, string message)
        {
            Log(ref callback, ref message, LogLevel.Error);
        }

        internal static void Warn(ref HL7ConverterMessageDelegate callback, string message)
        {
            Log(ref callback, ref message, LogLevel.Warn);
        }

        internal static void Fatal(ref HL7ConverterMessageDelegate callback, string message)
        {
            Log(ref callback, ref message, LogLevel.Fatal);
        }

        #endregion public static methods

        #region private static methods
        private static void Log(ref HL7ConverterMessageDelegate callback, ref string message, LogLevel level)
        {
            if (null != callback)
                callback(message, level);
        }
        #endregion private static methods
    }
}
