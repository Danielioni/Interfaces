/**********************************************************************

janoman: HL7 to XML-Parser

IAnalyzer.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Analyzer interface.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    internal interface IAnalyzer : ICallback
    {
        void Run();
        string GetMessageProtocol();
    }
}
