using System;

namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FalagardPropertyBase<T> : /* TODO: PropertyDefinitionBase*/ TypedProperty<T>, IPropertyDefinition
    {
        protected FalagardPropertyBase(string name, string help,
                                       string initialValue, string origin,
                                       bool redrawOnWrite, bool layoutOnWrite,
                                       string fireEvent, string eventNamespace) :
            /*PropertyDefinitionBase*/
            //base(name, help, initialValue,redrawOnWrite, layoutOnWrite, fireEvent, eventNamespace)
            /*TypedProperty<T>*/
            base(name, help, origin, PropertyHelper.FromString<T>(initialValue))
        {
            d_propertyName=name;
            d_helpString = help;
            d_initialValue = initialValue;
            d_writeCausesRedraw = redrawOnWrite;
            d_writeCausesLayout = layoutOnWrite;
            d_eventFiredOnWrite = fireEvent;
            d_eventNamespace = eventNamespace;
        }

        // TODO: ~FalagardPropertyBase() {}

        public string GetPropertyName()
        {
            return d_propertyName;
        }

        public void SetPropertyName(string name)
        {
            d_propertyName = name;
        }

        public string GetInitialValue()
        {
            return d_initialValue;
        }

        public void SetInitialValue(string value)
        {
            d_initialValue = value;
        }

        public string GetHelpString()
        {
            return d_helpString;
        }

        public void SetHelpString(string help)
        {
            d_helpString = help;
        }

        public bool IsRedrawOnWrite()
        {
            return d_writeCausesRedraw;
        }

        public void SetRedrawOnWrite(bool value)
        {
            d_writeCausesRedraw = value;
        }

        public bool IsLayoutOnWrite()
        {
            return d_writeCausesLayout;
        }

        public void SetLayoutOnWrite(bool value)
        {
            d_writeCausesLayout = value;
        }

        public string GetEventFiredOnWrite()
        {
            return d_eventFiredOnWrite;
        }

        public void SetEventFiredOnWrite(string eventName)
        {
            d_eventFiredOnWrite = eventName;
        }

        public string GetEventNamespace()
        {
            return d_eventNamespace;
        }

        public void SetEventNamespace(string eventNamespace)
        {
            d_eventNamespace = eventNamespace;
        }

        /// <summary>
        /// Writes an xml representation of the PropertyDefinitionBase based
        /// object to \a out_stream.
        /// </summary>
        /// <param name="xml_stream">
        /// XMLSerializer where xml data should be output.
        /// </param>
        public virtual void WriteDefinitionXMLToStream(XMLSerializer xml_stream)
        {
            WriteDefinitionXMLElementType(xml_stream);
            WriteDefinitionXMLAttributes(xml_stream);
            xml_stream.CloseTag();
        }

        /*!
        \brief
            Write out the text of the XML element type.  Note that you should
            not write the opening '<' character, nor any other information such
            as attributes in this function.  The writeExtraAttributes function
            can be used for writing attributes.

        \param xml_stream
            XMLSerializer where xml data should be output.
        */
        public abstract void WriteDefinitionXMLElementType(XMLSerializer xml_stream);

        /*!
        \brief
            Write out any xml attributes added in a sub-class.  Note that you
            should not write the closing '/>' character sequence, nor any other
            information in this function.  You should always call the base class
            implementation of this function when overriding.

        \param xml_stream
            XMLSerializer where xml data should be output.
        */

        public virtual void WriteDefinitionXMLAttributes(XMLSerializer xml_stream)
        {
            xml_stream.Attribute("name", d_propertyName);

            if (!String.IsNullOrEmpty(d_initialValue))
                xml_stream.Attribute("initialValue", d_initialValue);

            if (!String.IsNullOrEmpty(d_helpString))
                xml_stream.Attribute("help", d_helpString);

            if (d_writeCausesRedraw)
                xml_stream.Attribute("redrawOnWrite", "true");

            if (d_writeCausesLayout)
                xml_stream.Attribute("layoutOnWrite", "true");

            if (!String.IsNullOrEmpty(d_eventFiredOnWrite))
                xml_stream.Attribute("fireEvent", d_eventFiredOnWrite);
        }

        protected override void SetNativeImpl(PropertyReceiver receiver, T value)
        {
            if (d_writeCausesLayout)
                ((Window) receiver).PerformChildWindowLayout();

            if (d_writeCausesRedraw)
                ((Window) receiver).Invalidate(false);

            if (!String.IsNullOrEmpty(d_eventFiredOnWrite))
            {
                throw new NotImplementedException();
                //WindowEventArgs args = new WindowEventArgs((Window) receiver);
                //args.window.FireEvent(d_eventFiredOnWrite, args, d_eventNamespace);
            }
        }

        protected string d_propertyName;
        protected string d_initialValue;
        protected string d_helpString;
        protected bool d_writeCausesRedraw;
        protected bool d_writeCausesLayout;
        protected string d_eventFiredOnWrite;
        protected string d_eventNamespace;
    }

    public class PropertyDefinition<T> : FalagardPropertyBase<T>
    {
        // TODO: typedef typename TypedProperty<T>::Helper Helper;

        //------------------------------------------------------------------------//
        public PropertyDefinition(string name, string initialValue,
                           string help, string origin,
                           bool redrawOnWrite, bool layoutOnWrite,
                           string fireEvent, string eventNamespace) :
            base(name, help, initialValue, origin,
                                    redrawOnWrite, layoutOnWrite,
                                    fireEvent, eventNamespace)
        {
            d_userStringName = name + "_fal_auto_prop__";
        }

        // TODO: ~PropertyDefinition() {}

        public override void InitialisePropertyReceiver(PropertyReceiver receiver)
        {
            SetWindowUserString((Window)receiver, this.d_default);
        }

        //------------------------------------------------------------------------//
        public override Property Clone()
        {
            // TODO: return new PropertyDefinition<T>(this);
            throw new NotImplementedException();
        }

        //------------------------------------------------------------------------//
        protected override T GetNativeImpl(PropertyReceiver receiver)
        {
            var wnd = (Window)receiver;

            // the try/catch is used instead of querying the existence of the user
            // string in order that for the 'usual' case - where the user string
            // exists - there is basically no additional overhead, and that any
            // overhead is incurred only for the initial creation of the user
            // string.
            // Maybe the only negative here is that an error gets logged, though
            // this can be treated as a 'soft' error.
            try
            {
                return PropertyHelper.FromString<T>(wnd.GetUserString(d_userStringName));
            }
            catch(UnknownObjectException)
            {
                System.GetSingleton().Logger
                      .LogEvent("PropertyDefiniton::get: Defining new user string: " + d_userStringName);

                // HACK: FIXME: TODO: This const_cast is basically to allow the
                // above mentioned performance measure; the alternative would be
                // to just return d_default, and while technically correct, it
                // would be very slow.
                wnd.SetUserString(d_userStringName, d_default);

                return PropertyHelper.FromString<T>(d_default);
            }
        }

        protected override void SetNativeImpl(PropertyReceiver receiver, T value)
        {
            SetWindowUserString((Window)receiver, PropertyHelper.ToString(value));
            base.SetNativeImpl(receiver, value);
        }

        protected void SetWindowUserString(Window window, string value)
        {
            window.SetUserString(d_userStringName, value);
        }

        // TODO: ...
        public override void WriteDefinitionXMLElementType(XMLSerializer xml_stream)
        {
            xml_stream.OpenTag("PropertyDefinition");
        }

        protected string d_userStringName;
    };
}