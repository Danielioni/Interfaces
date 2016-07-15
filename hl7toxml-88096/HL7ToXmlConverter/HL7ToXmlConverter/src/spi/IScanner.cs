/**********************************************************************

janoman: HL7 to XML-Parser

IScanner.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Scanner interface.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    internal interface IScanner : ICallback
    {
        string GetMessageType();
        HL7TokenPair GetNextToken();
        HL7TokenPair GetCurrentToken();
        void PutBackToken();
        void Dump();
        int GetPosition();
        string GetMessage();
    }
}