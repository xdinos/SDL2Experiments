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

using SharpCEGui.Base;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// Menubar class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled
    ///     - Disabled
    /// 
    /// Named Areas:
    ///     ItemRenderArea
    /// </summary>
    public class FalagardMenubar : ItemListBaseWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/Menubar";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardMenubar(string type)
            : base(type)
        {

        }

        public override void CreateRenderGeometry()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

            // try and get imagery for our current state
            var imagery = wlf.GetStateImagery(Window.IsEffectiveDisabled() ? "Disabled" : "Enabled");

            // peform the rendering operation.
            imagery.Render(Window);
        }

        public override Rectf GetItemRenderArea()
        {
            return GetLookNFeel().GetNamedArea("ItemRenderArea").GetArea().GetPixelRect(Window);
        }
    }
}