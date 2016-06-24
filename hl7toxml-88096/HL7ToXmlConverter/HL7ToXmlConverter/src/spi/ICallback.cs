/**********************************************************************

janoman: HL7 to XML-Parser

ICallback.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief callback interface.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    interface ICallback
    {
        void SetCallback(HL7ConverterMessageDelegate callback);
    }
}
