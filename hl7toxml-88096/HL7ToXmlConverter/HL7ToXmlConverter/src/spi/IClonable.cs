/**********************************************************************

janoman: HL7 to XML-Parser

ICloneabel.cs

Copyright (c) 2011 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Abstract Typed Cloneable implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    internal interface ICloneable<T> : ICloneable
    {
        new T Clone();
    }

    /// <summary>
    /// Abstract Clonable class<br/>Not used at this time
    /// </summary>
    /// <typeparam name="T">Object which should be cloneable</typeparam>
    public abstract class CloneableBase<T> : ICloneable<T> where T : CloneableBase<T>
    {
        /// <summary>
        /// Abstract method to clone objects
        /// </summary>
        /// <returns></returns>
        public abstract T Clone();
        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }

    /// <summary>
    /// Abstract Cloneable classe extention<br/>Not used at this time
    /// </summary>
    /// <typeparam name="T">Object which should be cloneable</typeparam>
    public abstract class CloneableExBase<T> : CloneableBase<T> where T : CloneableExBase<T>
    {
        /// <summary>
        /// Abstract method to create cloneable object
        /// </summary>
        /// <returns>Clone of object which should be cloned</returns>
        protected abstract T CreateClone();
        /// <summary>
        /// Fille clone
        /// </summary>
        /// <param name="clone">Object which should be filled</param>
        protected abstract void FillClone(T clone);
        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Object to clone</returns>
        public override T Clone()
        {
            T clone = this.CreateClone();
            if (object.ReferenceEquals(clone, null)) { throw new NullReferenceException("Clone was not created."); }
            return clone;
        }
    }

}
