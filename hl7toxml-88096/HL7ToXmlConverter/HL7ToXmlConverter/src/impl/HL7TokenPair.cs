/**********************************************************************

janoman: HL7 to XML-Parser

HL7TokenPair.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Token implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    
    internal sealed class HL7TokenPair
    {
        #region ctors
        public HL7TokenPair()
        {
            this.tag = string.Empty;
        }
        public HL7TokenPair(HL7Token token, string tag)
        {
            this.token = token;
            this.tag = tag;
        }
        #endregion ctors
        #region properties
        private HL7Token token;

        public HL7Token Token
        {
            get { return token; }
            set { token = value; }
        }

        private string tag;

        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private bool readed;

        public bool Readed
        {
            get { return readed; }
            set { readed = value; }
        }
        #endregion properties
    }
}
