/**********************************************************************

janoman: HL7 to XML-Parser

HL7Sequence.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Sequence implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    /// <summary>
    /// Container to store elements like HL7-components...
    /// </summary>
    internal sealed class HL7Sequence : HL7Element, ICloneable
    {
        #region members
        List<HL7Element> elements;

        public List<HL7Element> Elements
        {
            get { return elements; }
            set { elements = value; }
        }
        #endregion members

        #region ctors
        public HL7Sequence()
        {
            this.elements = new List<HL7Element>();
            Occur = 0;
        }
        #endregion ctors

        #region public methods
        /// <summary>
        /// Convert a sequence to an element
        /// </summary>
        /// <returns></returns>
        public HL7Element ToElement()
        {
            return (HL7Element)this;
        }
        #endregion

        #region ICloneable Member

        object ICloneable.Clone()
        {
            List<HL7Element> newElements = new List<HL7Element>();
            foreach (HL7Element element in this.elements)
            {
                HL7Element newElement = (HL7Element)element.Clone();
                newElements.Add(newElement);
            }
            HL7Sequence newSequence = new HL7Sequence();
            newSequence.Elements = newElements;
            return newSequence;
        }

        #endregion
    }
}
