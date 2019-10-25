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
    /// Class providing logic buttons that can have their selected state toggled.
    /// </summary>
    public class ToggleButton : ButtonBase
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ToggleButton";

        #region Events

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/ToggleButton";

        public const string EventSelectStateChanged = "SelectStateChanged";

        /// <summary>
        /// Event fired when then selected state of the ToggleButton changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ToggleButton whose state has changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> SelectStateChanged
        {
            add { SubscribeEvent(EventSelectStateChanged, value); }
            remove { UnsubscribeEvent(EventSelectStateChanged, value); }
        }

        #endregion

        /// <summary>
        /// returns true if the toggle button is in the selected state. 
        /// </summary>
        /// <returns></returns>
        public bool IsSelected()
        {
            return Selected;
        }

        /// <summary>
        /// sets whether the toggle button is in the selected state.
        /// </summary>
        /// <param name="select"></param>
        public void SetSelected(bool select)
        {
            if (Selected == select)
                return;

            Selected = select;
            Invalidate(false);

            OnSelectStateChange(new WindowEventArgs(this));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public ToggleButton(string type, string name)
            : base(type, name)
        {
            Selected = false;
            AddToggleButtonProperties();
        }

        protected virtual bool GetPostClickSelectState()
        {
            return Selected ^ true;
        }

        protected void AddToggleButtonProperties()
        {
            AddProperty(new TplWindowProperty<ToggleButton, bool>(
                            "Selected",
                            "Property to access the selected state of the ToggleButton. Value is either \"True\" or \"False\".",
                            (w, v) => w.SetSelected(v), w => w.IsSelected(), WidgetTypeName));
        }

        //! event triggered internally when toggle button select state changes.
        protected virtual void OnSelectStateChange(WindowEventArgs e)
        {
            FireEvent(EventSelectStateChanged, e, EventNamespace);
        }

        // base class overriddes
        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            if (e.Source == CursorInputSource.Left && IsPushed())
            {
                var sheet = GetGUIContext().GetRootWindow();
                if (sheet != null)
                {
                    // was mouse released over this widget
                    // (use mouse position, as e.position is already unprojected)
                    if (this == sheet.GetTargetChildAtPosition(GetGUIContext().GetCursor().GetPosition()))
                    {
                        SetSelected(GetPostClickSelectState());
                    }
                }

                ++e.handled;
            }

            base.OnCursorActivate(e);
        }

        #region Fields

        protected bool Selected;

        #endregion
    }
}