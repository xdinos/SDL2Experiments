#region Copyright
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2013
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that contains a collection of <seealso cref="Property"/> objects.
    /// </summary>
    public abstract class PropertySet : PropertyReceiver, IEnumerable<Property>
    {
        /// <summary>
        /// Adds a new Property to the PropertySet
        /// </summary>
        /// <param name="property">
        /// Pointer to the Property object to be added to the PropertySet.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if \a property is null.
        /// </exception>
        /// <exception cref="AlreadyExistsException">
        /// Thrown if a Property with the same name as \a property already exists in the PropertySet.
        /// </exception>
        public void AddProperty(Property property)
        {
            if (property==null)
                throw  new ArgumentNullException("The given Property object is invalid.");

            if (_properties.ContainsKey(property.GetName()))
                throw new AlreadyExistsException("A Property named '" + property.GetName() +
                                                 "' already exists in the PropertySet.");

            _properties.Add(property.GetName(), property);

            property.InitialisePropertyReceiver(this);
        }

        /// <summary>
        /// Removes a Property from the PropertySet.
        /// </summary>
        /// <param name="name">
        /// String containing the name of the Property to be removed.
        /// If Property \a name is not in the set, nothing happens.
        /// </param>
        public void RemoveProperty(string name)
        {
            if (_properties.ContainsKey(name))
                _properties.Remove(name);
        }

        /// <summary>
        /// Retrieves a property instance (that was previously added).
        /// </summary>
        /// <param name="name">
        /// String containing the name of the Property to be retrieved. 
        /// If Property \a name is not in the set, exception is thrown.
        /// </param>
        /// <returns>
        /// Pointer to the property instance.
        /// </returns>
        public Property GetPropertyInstance(string name)
        {
            Property baseProperty;
            if (!_properties.TryGetValue(name, out baseProperty))
                throw new UnknownObjectException("There is no Property named '" + name + "' available in the set.");

            return baseProperty;
        }
        
        /// <summary>
        /// Removes all Property objects from the PropertySet.
        /// </summary>
        public void ClearProperties()
        {
            _properties.Clear();
        }
        
        /// <summary>
        /// Checks to see if a Property with the given name is in the PropertySet.
        /// </summary>
        /// <param name="name">
        /// String containing the name of the <see cref="Property"/> to check for.
        /// </param>
        /// <returns>
        /// true if a <see cref="Property"/> named \a name is in the PropertySet.
        /// false if no <see cref="Property"/> named \a name is in the PropertySet.
        /// </returns>
        public bool IsPropertyPresent(string name)
        {
            return _properties.ContainsKey(name);
        }
        
        /// <summary>
        /// Return the help text for the specified Property.
        /// </summary>
        /// <param name="name">
        /// String holding the name of the Property who's help text is to be returned.
        /// </param>
        /// <returns>
        /// String object containing the help text for the Property \a name.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if no Property named \a name is in the PropertySet.
        /// </exception>
        public string GetPropertyHelp(string name)
        {
            return GetPropertyInstance(name).GetHelp();
        }

        /// <summary>
        /// Gets the current value of the specified Property.
        /// </summary>
        /// <param name="name">
        /// String containing the name of the Property who's value is to be returned.
        /// </param>
        /// <returns>
        /// String object containing a textual representation of the requested Property.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if no Property named \a name is in the PropertySet.
        /// </exception>
        public string GetProperty(string name)
        {
            return GetPropertyInstance(name).Get(this);
        }
        
        /// <summary>
        /// \copydoc PropertySet::getProperty
        /// 
        /// This method tries to do a native type get without string conversion if possible,
        /// if that is not possible, it gracefully falls back to string conversion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetProperty<T>(string name)
        {
            var baseProperty = GetPropertyInstance(name);
            var typedProperty = baseProperty as TypedProperty<T>;

            return typedProperty != null
                       ? typedProperty.GetNative(this)
                       : PropertyHelper.FromString<T>(baseProperty.Get(this));
        }

        /// <summary>
        /// Sets the current value of a Property.
        /// </summary>
        /// <param name="name">
        /// String containing the name of the Property who's value is to be set.
        /// </param>
        /// <param name="value">
        /// String containing a textual representation of the new value for the Property.
        /// </param>
        /// <exception cref="UnknownObjectException">
        /// Thrown if no Property named \a name is in the PropertySet.
        /// </exception>
        /// <exception cref="InvalidRequestException">
        /// Thrown when the Property was unable to interpret the content of \a value.
        /// </exception>
        public void SetProperty(string name, string value)
        {
            GetPropertyInstance(name).Set(this, value);
        }

        /// <summary>
        /// \copydoc PropertySet::setProperty
        /// 
        /// This method tries to do a native type set without string conversion if possible,
        /// if that is not possible, it gracefully falls back to string conversion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty<T>(string name, T value)
        {
            var baseProperty = GetPropertyInstance(name);
            var typedProperty = baseProperty as TypedProperty<T>;

            if (typedProperty != null)
                typedProperty.SetNative(this, value);
            else
                baseProperty.Set(this, value.ToString());
        }

        /// <summary>
        /// Returns whether a Property is at it's default value.
        /// </summary>
        /// <param name="name">
        /// String containing the name of the Property who's default state is to be tested.
        /// </param>
        /// <returns>
        /// - true if the property has it's default value.
        /// - false if the property has been modified from it's default value.
        /// </returns>
        public bool IsPropertyDefault(string name)
        {
            return GetPropertyInstance(name).IsDefault(this);
        }
        
        /// <summary>
        /// Returns the default value of a Property as a String.
        /// </summary>
        /// <param name="name">
        /// String containing the name of the Property who's default string is to be returned.
        /// </param>
        /// <returns>
        /// String object containing a textual representation of the default value for this property.
        /// </returns>
        public string GetPropertyDefault(string name)
        {
            return GetPropertyInstance(name).GetDefault(this);
        }

        #region Implementation of IEnumerator<Property>

        public IEnumerator<Property> GetEnumerator()
        {
            return ((IEnumerable<Property>) _properties.Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
        
        #region Fields

        private readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();

        #endregion
    }
}