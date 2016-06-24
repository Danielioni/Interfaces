/**********************************************************************

janoman: HL7 to XML-Parser

HL7Element.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Element implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    internal class HL7Element : ICloneable
    {
        #region ctors
        public HL7Element()
        {
        }
        public HL7Element(string name)
        {
            this.name = name;
        }
        public HL7Element(string name, Dictionary<string, string> attributes) : this(name)
        {
            this.attributes = attributes;
        }
        #endregion ctors

        #region members
        private string name = string.Empty;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Dictionary<string,string> attributes = null;

        public Dictionary<string, string> Attributes
        {
            get 
            {
                if (attributes == null)
                    attributes = new Dictionary<string, string>();
                return attributes; 
            }
            set { attributes = value; }
        }

        private string text = string.Empty;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private bool repeatable = false;

        public bool Repeatable
        {
            get { return repeatable; }
            set { repeatable = value; }
        }

        private bool optional = false;

        public bool Optional
        {
            get { return optional; }
            set { optional = value; }
        }

        private int depth;

        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        private int level;

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        private int occur;

        public int Occur
        {
            get { return occur; }
            set { occur = value; }
        }

        private bool empty = true;

        public bool Empty
        {
            get { return empty; }
            set { empty = value; }
        }

        private bool open = false;

        public bool Open
        {
            get { return open; }
            set { open = value; }
        }
        #endregion members

        #region public methods
        public void AddAttribute(string key, string value)
        {
            if (Attributes.ContainsKey(key))
            {
                Attributes.Remove(key);
            }
            Attributes.Add(key, value);
        }
        #endregion public methods

        #region ICloneable Member

        public object Clone()
        {
            HL7Element element = new HL7Element();
            element.Attributes = new Dictionary<string, string>(Attributes);
            element.Depth = this.depth;
            element.Empty = this.empty;
            element.Level = this.level;
            element.Name = this.name;
            element.Occur = this.occur;
            element.Open = this.open;
            element.Optional = this.optional;
            element.Repeatable = this.repeatable;
            element.Text = this.text;
            return element;
        }

        #endregion
    }
}
