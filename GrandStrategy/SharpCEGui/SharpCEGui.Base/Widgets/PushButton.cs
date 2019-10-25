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

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class to provide logic for push button type widgets.
    /// </summary>
    public class PushButton : ButtonBase
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/PushButton";

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "PushButton";

        public const string EventClicked = "Clicked";

        /// <summary>
        /// Event fired when the button is clicked.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the PushButton that was clicked.
        /// </summary>
        public event GuiEventHandler<EventArgs> Clicked
        {
            add { SubscribeEvent(EventClicked, value); }
            remove { UnsubscribeEvent(EventClicked, value); }
        }

        #endregion

        /// <summary>
        /// Constructor for base PushButton class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public PushButton(string type, string name)
            : base(type, name)
        {

        }

        /// <summary>
        /// handler invoked internally when the button is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClicked(WindowEventArgs e)
        {
            FireEvent(EventClicked, e, EventNamespace);
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            if ((e.Source == CursorInputSource.Left) && IsPushed())
            {
                var sheet = GetGUIContext().GetRootWindow();

                if (sheet != null)
                {
                    // if mouse was released over this widget
                    // (use position from mouse, as e.position has been unprojected)
                    if (this == sheet.GetTargetChildAtPosition(GetGUIContext().GetCursor().GetPosition()))
                    {
                        // fire event
                        OnClicked(new WindowEventArgs(this));
                    }

                }

                ++e.handled;
            }

            // default handling
            base.OnCursorActivate(e);
        }

        protected internal override void OnSemanticInputEvent(SemanticEventArgs e)
        {
            if (IsDisabled())
                return;

            if (e.d_semanticValue == SemanticValue.SV_Confirm)
            {
                OnClicked(e);

                ++e.handled;
            }
        }
    }
}