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
    /// TabControl class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled
    ///     - Disabled
    /// 
    /// Child Widgets:
    ///     TabPane based widget with name suffix "__auto_TabPane__"
    ///     optional: DefaultWindow to contain tab buttons with name suffix "__auto_TabPane__Buttons"
    /// 
    /// Property initialiser definitions:
    ///     - TabButtonType - specifies a TabButton based widget type to be
    ///     created each time a new tab button is required.
    /// 
    /// \note
    /// The current TabControl base class enforces a strict layout, so while
    /// imagery can be customised as desired, the general layout of the
    /// component widgets is, at least for the time being, fixed.
    /// </summary>
    public class FalagardTabControl : TabControlWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/TabControl";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardTabControl(string type)
            : base(type)
        {
            RegisterProperty(new TplWindowRendererProperty<FalagardTabControl, string>(
                                 "TabButtonType",
                                 "Property to get/set the widget type used when creating tab buttons.  Value should be \"[widgetTypeName]\".",
                                 (x, v) => x.SetTabButtonType(v), x => x.GetTabButtonType(), TypeName, ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetTabButtonType()
        {
            return _tabButtonType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetTabButtonType(string type)
        {
            _tabButtonType = type;
        }

        public override void CreateRenderGeometry()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

            // render basic imagery
            var imagery = wlf.GetStateImagery(Window.IsEffectiveDisabled() ? "Disabled" : "Enabled");
            imagery.Render(Window);
        }

        // overridden from TabControl base class.
        public override TabButton CreateTabButton(string name)
        {
            if (String.IsNullOrEmpty(_tabButtonType))
            {
                throw new InvalidRequestException("d_tabButtonType has not been set!");
            }

            var button = WindowManager.GetSingleton().CreateWindow(_tabButtonType, name);
            button.SetAutoWindow(true);
            return (TabButton) button;
        }

        #region Fields

        private string _tabButtonType;

        #endregion
    }
}