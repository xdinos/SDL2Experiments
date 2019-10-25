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
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    // TODO: This is not finished in the slightest!  There will be many changes here...
    /// <summary>
    /// Class that encapsulates information regarding a sub-widget required for a widget.
    /// </summary>
    public class WidgetComponent
    {
        // TODO: do we really need this ?? 
        // public WidgetComponent() {}

        public WidgetComponent(string type, string look, string suffix, string renderer, bool autoWindow)
        {
            _baseType = type;
            _imageryName = look;
            _name = suffix;
            _rendererType = renderer;
            _autoWindow = autoWindow;
            _vertAlign = VerticalAlignment.Top;
            _horzAlign = HorizontalAlignment.Left;
        }

        /// <summary>
        /// Create an instance of this widget component adding it as a child to
        /// the given Window.
        /// </summary>
        /// <param name="parent"></param>
        public void Create(Window parent)
        {
            Window widget = WindowManager.GetSingleton().CreateWindow(_baseType, _name);
            widget.SetAutoWindow(_autoWindow);

            // set the window renderer
            if (!String.IsNullOrEmpty(_rendererType))
                widget.SetWindowRenderer(_rendererType);

            // set the widget look
            if (!String.IsNullOrEmpty(_imageryName))
                widget.SetLookNFeel(_imageryName);

            // add the new widget to its parent
            parent.AddChild(widget);

            // set alignment options
            widget.SetVerticalAlignment(_vertAlign);
            widget.SetHorizontalAlignment(_horzAlign);

            // TODO: We still need code to specify position and size.  Due to the advanced
            // TODO: possibilities, this is better handled via a 'layout' method instead of
            // TODO: setting this once and forgetting about it.

            // initialise properties.  This is done last so that these properties can
            // override properties specified in the look assigned to the created widget.
            foreach (var property in _properties)
            {
                property.Apply(widget);
            }

            // link up the event actions
            foreach (var eventAction in _eventActions)
            {
                eventAction.InitialiseWidget(widget);
            }
        }

        /// <summary>
        /// Cleanup from the given parent widget, the instance of the child
        /// created for this WidgetComponent.
        /// </summary>
        /// <param name="parent"></param>
        public void Cleanup(Window parent)
        {
            if (!parent.IsChild(GetWidgetName()))
                return;

            var widget = parent.GetChild(GetWidgetName());
            // clean up up the event actions
            foreach (var eventAction in _eventActions)
            {
                eventAction.CleanupWidget(widget);
            }

            parent.DestroyChild(widget);
        }

        public ComponentArea GetComponentArea()
        {
            return _area;
        }

        public void SetComponentArea(ComponentArea area)
        {
            _area = area;
        }

        public String GetBaseWidgetType()
        {
            return _baseType;
        }

        public void SetBaseWidgetType(string type)
        {
            _baseType = type;
        }

        public string GetWidgetLookName()
        {
            return _imageryName;
        }

        public void SetWidgetLookName(string look)
        {
            _imageryName = look;
        }

        public string GetWidgetName()
        {
            return _name;
        }

        public void SetWidgetName(string name)
        {
            _name = name;
        }

        public string GetWindowRendererType()
        {
            return _rendererType;
        }

        public void SetWindowRendererType(string type)
        {
            _rendererType = type;
        }

        public VerticalAlignment GetVerticalWidgetAlignment()
        {
            return _vertAlign;
        }

        public void SetVerticalWidgetAlignment(VerticalAlignment alignment)
        {
            _vertAlign = alignment;
        }

        public HorizontalAlignment GetHorizontalWidgetAlignment()
        {
            return _horzAlign;
        }

        public void SetHorizontalWidgetAlignment(HorizontalAlignment alignment)
        {
            _horzAlign = alignment;
        }

        public void AddPropertyInitialiser(PropertyInitialiser initialiser)
        {
            _properties.Add(initialiser);
        }
        
        public void RemovePropertyInitialiser(string name)
        {
            _properties.RemoveAll(x => x.GetTargetPropertyName() == name);
        }

        public void ClearPropertyInitialisers()
        {
            _properties.Clear();
        }

        public void SetAutoWindow(bool value)
        {
            _autoWindow = value;
        }

        public bool IsAutoWindow()
        {
            return _autoWindow;
        }

        public void AddEventAction(EventAction eventAction)
        {
            _eventActions.Add(eventAction);
        }
        public void ClearEventActions()
        {
            _eventActions.Clear();
        }

        public void Layout(Window owner)
        {
            try
            {
                var pixelArea = _area.GetPixelRect(owner);
                var windowArea = new URect(UDim.Absolute(pixelArea.Left),
                                           UDim.Absolute(pixelArea.Top),
                                           UDim.Absolute(pixelArea.Right),
                                           UDim.Absolute(pixelArea.Bottom));

                var wnd = owner.GetChild(_name);
                wnd.SetArea(windowArea);
                wnd.NotifyScreenAreaChanged();
            }
            catch (UnknownObjectException)
            {
            }
        }

        /// <summary>
        /// Writes an xml representation of this WidgetComponent to \a out_stream.
        /// </summary>
        /// <param name="xmlStream">
        /// Stream where xml data should be output.
        /// </param>
        public void WriteXMLToStream(XMLSerializer xmlStream)
        {
            // output opening tag
            xmlStream.OpenTag("Child")
                .Attribute("type", _baseType)
                .Attribute("name", _name);

            if (!String.IsNullOrEmpty(_imageryName))
                xmlStream.Attribute("look", _imageryName);

            if (!String.IsNullOrEmpty(_rendererType))
                xmlStream.Attribute("renderer", _rendererType);

            if (!_autoWindow)
                xmlStream.Attribute("autoWindow", "false");

            // Output <EventAction> elements
            _eventActions.ForEach(x=>x.WriteXMLToStream(xmlStream));

            // output target area
            _area.WriteXMLToStream(xmlStream);

            // output vertical alignment
            xmlStream.OpenTag("VertAlignment")
                .Attribute("type", /*FalagardXMLHelper*/PropertyHelper.ToString<VerticalAlignment>(_vertAlign))
                .CloseTag();

            // output horizontal alignment
            xmlStream.OpenTag("HorzAlignment")
                .Attribute("type", /*FalagardXMLHelper*/PropertyHelper.ToString<HorizontalAlignment>(_horzAlign))
                .CloseTag();

            //output property initialisers
            _properties.ForEach(x=>x.WriteXMLToStream(xmlStream));

            // output closing tag
            xmlStream.CloseTag();
        }

        /// <summary>
        /// Takes the name of a property and returns a pointer to the last
        /// PropertyInitialiser for this property or 0 if the is no
        /// PropertyInitialiser for this property in the WidgetLookFeel
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to look for.
        /// </param>
        /// <returns></returns>
        public PropertyInitialiser FindPropertyInitialiser(string propertyName)
        {
            //PropertiesList::const_reverse_iterator i = d_properties.rbegin();
            //while (i != d_properties.rend())
            //{
            //    if ((*i).getTargetPropertyName() == propertyName)
            //        return &(*i);
            //    ++i;
            //}
            //return 0;

            return _properties.FindLast(x => x.GetTargetPropertyName() == propertyName);
        }

        /// <summary>
        /// perform any processing required due to the given font having changed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public bool HandleFontRenderSizeChange(Window window, Font font)
        {
            if (_area.HandleFontRenderSizeChange(window, font))
            {
                window.PerformChildWindowLayout();
                return true;
            }

            return false;
        }

    //private:
    //    typedef std::vector<PropertyInitialiser
    //        CEGUI_VECTOR_ALLOC(PropertyInitialiser)> PropertiesList;
    //    typedef std::vector<EventAction
    //        CEGUI_VECTOR_ALLOC(EventAction)> EventActionList;

        /*************************************************************************
            Iterator stuff
        *************************************************************************/
        //typedef ConstVectorIterator<PropertiesList> PropertyIterator;
        //typedef ConstVectorIterator<EventActionList> EventActionIterator;

        /*!
         * Return a WidgetComponent::PropertyIterator that iterates over the
         * PropertyInitialiser inside this WidgetComponent.
         */
        public IEnumerator<PropertyInitialiser> GetPropertyIterator()
        {
            throw new NotImplementedException();
        }

        /*!
         * Return a WidgetComponent::EventActionIterator that iterates over the
         * EventAction definitions for this WidgetComponent.
         */
        IEnumerator<EventAction> GetEventActionIterator()
        {
            throw new NotImplementedException();
        }

        #region Fields

        /// <summary>
        /// Destination area for the widget (relative to it's parent).
        /// </summary>
        private ComponentArea _area;

        /// <summary>
        /// Type of widget to be created.
        /// </summary>
        private string _baseType;
        
        /// <summary>
        /// Name of a WidgetLookFeel to be used for the widget.
        /// </summary>
        private string _imageryName;
        
        /// <summary>
        /// name to create this widget with.
        /// </summary>
        private string _name;
        
        /// <summary>
        /// Name of the window renderer type to assign to the widget.
        /// </summary>
        private string _rendererType;

        /// <summary>
        /// specifies whether to mark component as an auto-window.
        /// </summary>
        private bool _autoWindow;

        /// <summary>
        /// Vertical alignment to be used for this widget.
        /// </summary>
        private VerticalAlignment _vertAlign;
        
        /// <summary>
        /// Horizontal alignment to be used for this widget.
        /// </summary>
        private HorizontalAlignment _horzAlign;
        
        /// <summary>
        /// Collection of PropertyInitialisers to be applied the the widget upon creation.
        /// </summary>
        private readonly List<PropertyInitialiser> _properties=new List<PropertyInitialiser>();
        
        /// <summary>
        /// EventActions added to the WidgetComponent
        /// </summary>
        private readonly List<EventAction> _eventActions=new List<EventAction>();

        #endregion
    }
}