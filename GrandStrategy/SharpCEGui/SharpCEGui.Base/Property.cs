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

namespace SharpCEGui.Base
{
    /// <summary>
    /// An abstract class that defines the interface to access object properties by name.
    /// <para>
    /// Property objects allow (via a PropertySet) access to certain properties of objects
    /// by using simple get/set functions and the name of the property to be accessed.
    /// </para>
    /// </summary>
    public abstract class Property
    {
        public const string XMLElementName = "Property";
        public const string NameXMLAttributeName = "name";
        public const string ValueXMLAttributeName = "value";

        /*!
        \brief
            Creates a new Property object.

        \param name
            String containing the name of the new Property.

        \param help
            String containing a description of the Property and it's usage.
		
        \param defaultValue
            String holding the textual representation of the default value for this Property

        \param writesXML
            Specifies whether the writeXMLToStream method should do anything for this Property.  This
            enables selectivity in what properties within a PropertySet will get output as XML.

        \param dataType
            String representation of the data type this property is held in ("int", "UVector2", ...)

        \param origin
            String describing the origin class of this Property (Window, FrameWindow, ...)
        */
        protected Property(string name, string help,
                        string defaultValue = "",
                        bool writesXml = true,
                        string dataType = "Unknown",
                        string origin = "Unknown")
        {
            d_name = name;
            d_help = help;
            d_default = defaultValue;
            d_writeXML = writesXml;
            d_dataType = dataType;
            d_origin = origin;
        }

        /// <summary>
        /// Return a String that describes the purpose and usage of this Property.
        /// </summary>
        /// <returns>
        /// String that contains the help text
        /// </returns>
        public string GetHelp()
        {
            return d_help;
        }

        /// <summary>
        /// Return a the name of this Property
        /// </summary>
        /// <returns>
        /// String containing the name of the Property
        /// </returns>
        public string GetName()
        {
            return d_name;
        }

        /// <summary>
        /// Return string data type of this Property
        /// </summary>
        /// <returns>
        /// String containing the data type of the Property
        /// </returns>
        public string GetDataType()
        {
            return d_dataType;
        }

        /// <summary>
        /// Return string origin of this Property.
        /// </summary>
        /// <returns>
        /// String containing the origin of the Property.
        /// </returns>
        public string GetOrigin()
        {
            return d_origin;
        }

        /// <summary>
        /// Return the current value of the Property as a String
        /// </summary>
        /// <param name="receiver">
        /// Pointer to the target object.
        /// </param>
        /// <returns>
        /// String object containing a textual representation of the current value of the Property
        /// </returns>
        public abstract string Get(PropertyReceiver receiver);
        
        /*!
        \brief
            Sets the value of the property

        \param receiver
            Pointer to the target object.

        \param value
            A String object that contains a textual representation of the new value to assign to the Property.

        \return
            Nothing.

        \exception InvalidRequestException	Thrown when the Property was unable to interpret the content of \a value.
        */
        public abstract void Set(PropertyReceiver receiver, string value);
        
        /*!
        \brief
            Returns whether the property is at it's default value.

        \param receiver
            Pointer to the target object.

        \return
            - true if the property has it's default value.
            - false if the property has been modified from it's default value.
        */
        public virtual bool IsDefault(PropertyReceiver receiver)
        {
            return (Get(receiver) == GetDefault(receiver));
        }
        
        /// <summary>
        /// Returns the default value of the Property as a String.
        /// </summary>
        /// <param name="receiver">
        /// Pointer to the target object.
        /// </param>
        /// <returns>
        /// String object containing a textual representation of the default value for this property.
        /// </returns>
        public virtual string GetDefault(PropertyReceiver receiver)
        {
            return d_default;
        }
        
        /*!
        \brief
            Writes out an XML representation of this class to the given stream.

        \note
            This would normally have been implemented via XMLGenerator base class, but in this
            case we require the target PropertyReceiver in order to obtain the property value.
        */
        public virtual void WriteXMLToStream(PropertyReceiver receiver, XMLSerializer xml_stream)
        {
            if (d_writeXML)
            {
                xml_stream.OpenTag(XMLElementName)
                          .Attribute(NameXMLAttributeName, d_name);
                // Detect wether it is a long property or not
                // Long property are needed if
                var value = Get(receiver);
                if (value.Contains("\n"))
                {
                    xml_stream.Text(value);
                }
                else
                {
                    xml_stream.Attribute(ValueXMLAttributeName, Get(receiver));
                }
                xml_stream.CloseTag();
            }
        }

        /*!
        \brief
            Returns whether the property is readable.

        \return
            - true if the property is readable.
            - false if the property isn't readable.
        */
        public virtual bool IsReadable()
        {
            return true;
        }

        /*!
        \brief
            Returns whether the property is writable.

        \return
            - true if the property is writable.
            - false if the property isn't writable.
        */
        public virtual bool IsWritable()
        {
            return true;
        }

        /*!
        \brief
            Returns whether the property writes to XML streams.
        */
        public virtual bool DoesWriteXML()
        {
            return d_writeXML;
        }

        /// <summary>
        /// function to allow initialisation of a PropertyReceiver.
        /// </summary>
        /// <param name="receiver"></param>
        public virtual void InitialisePropertyReceiver(PropertyReceiver receiver)
        {

        }

        public abstract Property Clone();

        #region Fields

        protected string d_name; //!< String that stores the Property name.
        protected string d_help; //!< String that stores the Property help text.
        protected string d_default; //!< String that stores the Property default value string.
        protected bool d_writeXML; //!< Specifies whether writeXMLToStream should do anything for this property.

        // TODO: This is really ugly but PropertyDefinition forced me to do this to support operator=
        protected string d_dataType; //!< Holds data type of this property

        // TODO: This is really ugly but PropertyDefinition forced me to do this to support operator=
        protected string d_origin; //!< Holds origin of this property

        #endregion
    }

    /// <summary>
    /// base class for properties able to do native set/get
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TypedProperty<T> : Property
    {
        // TODO: do we want less bug prone code but a bit slower (string conversion for default values at construction) or faster
        //       but more typo prone (passing string default value)?

        protected TypedProperty(string name, string help, string origin = "Unknown", T defaultValue = default(T), bool writesXml = true) :
            base(name, help, PropertyHelper.ToString(defaultValue), writesXml, typeof(T).Name/*PropertyHelper.GetDataTypeName<T>()*/, origin)
        {

        }

        public override string Get(PropertyReceiver receiver)
        {
            return PropertyHelper.ToString(GetNative(receiver));
        }

        public override void Set(PropertyReceiver receiver, string value)
        {
            SetNative(receiver, PropertyHelper.FromString<T>(value));
        }

        //public override Property Clone()
        //{
        //    return new TypedProperty<T>(d_name, d_help,
        //                                _getter, _stringGetter,
        //                                _setter, _stringSetter);
        //}

        /// <summary>
        /// native set method, sets the property given a native type
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="value"></param>
        /// <seealso cref="Property.Set"/>
        public virtual void SetNative(PropertyReceiver receiver, T value)
        {
            if (IsWritable())
                SetNativeImpl(receiver, value);
            else
                throw new InvalidRequestException("Property " + d_origin + ":" + d_name + " is not writable!");
        }

        /// <summary>
        /// native get method, returns the native type by copy
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns></returns>
        /// <seealso cref="Property.Get"/>
        public virtual T GetNative(PropertyReceiver receiver)
        {
            if (IsReadable())
                return GetNativeImpl(receiver);
            else
                throw new InvalidRequestException("Property " + d_origin + ":" + d_name + " is not readable!");
        }

        protected abstract void SetNativeImpl(PropertyReceiver receiver, T value);
        protected abstract T GetNativeImpl(PropertyReceiver receiver);
    }

    public abstract class TplProperty<TInstance, T> : TypedProperty<T>
    {
        protected TplProperty(string name, string help, Action<TInstance, T> setter, Func<TInstance,T> getter, string origin = "Unknown", T defaultValue = default(T), bool writesXML = true) 
            : base(name, help, origin, defaultValue, writesXML)
        {
            _setter = setter;
            _getter = getter;
        }

        public override bool IsReadable()
        {
            return _getter != null;
        }

        public override bool IsWritable()
        {
            return _setter != null;
        }

        protected Action<TInstance, T> _setter;
        protected Func<TInstance, T> _getter;
    }

    public class TplWindowProperty<TWindow, T> : TplProperty<TWindow, T> where TWindow : PropertyReceiver
    {
        public TplWindowProperty(string name, string help, Action<TWindow,T> setter, Func<TWindow, T> getter, string origin = "Unknown", T defaultValue = default(T), bool writesXML = true) 
            : base(name, help, setter, getter, origin, defaultValue, writesXML)
        {
        }

        public override Property Clone()
        {
            return new TplWindowProperty<TWindow, T>(
                d_name, d_help, _setter, _getter, d_origin, PropertyHelper.FromString<T>(d_default), d_writeXML);
        }

        protected override void SetNativeImpl(PropertyReceiver receiver, T value)
        {
            _setter((TWindow) receiver, value);
        }

        protected override T GetNativeImpl(PropertyReceiver receiver)
        {
            return _getter((TWindow) receiver);
        }
    }

    public class TplWindowRendererProperty<TWindowRenderer, T> : TplProperty<TWindowRenderer, T> where TWindowRenderer : WindowRenderer
    {
        public TplWindowRendererProperty(string name, string help, Action<TWindowRenderer, T> setter, Func<TWindowRenderer, T> getter, string origin = "Unknown", T defaultValue = default(T), bool writesXML = true)
            : base(name, help, setter, getter, origin, defaultValue, writesXML)
        {
        }

        public override Property Clone()
        {
            return new TplWindowRendererProperty<TWindowRenderer, T>(
                d_name, d_help, _setter, _getter, d_origin, PropertyHelper.FromString<T>(d_default), d_writeXML);
        }

        protected override void SetNativeImpl(PropertyReceiver receiver, T value)
        {
            _setter((TWindowRenderer)((Window)receiver).GetWindowRenderer(), value);
        }

        protected override T GetNativeImpl(PropertyReceiver receiver)
        {
            return _getter((TWindowRenderer)((Window)receiver).GetWindowRenderer());
        }
    }
}