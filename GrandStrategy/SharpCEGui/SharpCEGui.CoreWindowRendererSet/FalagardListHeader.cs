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
using SharpCEGui.Base;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// ListHeader class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// Property Initialisers:
    ///     SegmentWidgetType   - type of widget to create for segments.
    /// 
    /// Imagery States:
    ///     - Enabled           - basic rendering for enabled state.
    ///     - Disabled          - basic rendering for disabled state.
    /// </summary>
    public class FalagardListHeader : ListHeaderWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/ListHeader";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardListHeader(string type)
            : base(type)
        {
            RegisterProperty(
                new TplWindowRendererProperty<FalagardListHeader, string>(
                    "SegmentWidgetType",
                    "Property to get/set the widget type used when creating header segments. Value should be \"[widgetTypeName]\".",
                    (x, v) => x.SetSegmentWidgetType(v), x => x.GetSegmentWidgetType(), TypeName, ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSegmentWidgetType()
        {
            return _segmentWidgetType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetSegmentWidgetType(string type)
        {
            _segmentWidgetType = type;
        }

        // overridden from ListHeaderWindowRenderer base class.
        public override void CreateRenderGeometry()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

            // render basic imagery
            var imagery = wlf.GetStateImagery(Window.IsEffectiveDisabled() ? "Disabled" : "Enabled");
            imagery.Render(Window);
        }

        public override ListHeaderSegment CreateNewSegment(string name)
        {
            // make sure this has been set
            if (String.IsNullOrEmpty(_segmentWidgetType))
                throw new InvalidRequestException("Segment widget type has not been set!");

            var segment = WindowManager.GetSingleton().CreateWindow(_segmentWidgetType, name);
            segment.SetAutoWindow(true);
            return (ListHeaderSegment) segment;
        }

        public override void DestroyListSegment(ListHeaderSegment segment)
        {
            WindowManager.GetSingleton().DestroyWindow(segment);
        }

        // data fields
        private string _segmentWidgetType;
    }
}