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

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// Static class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled                     - basic rendering for enabled state.
    ///     - Disabled                    - basic rendering for disabled state.
    ///     - EnabledFrame                - frame rendering for enabled state
    ///     - DisabledFrame               - frame rendering for disabled state.
    ///     - WithFrameEnabledBackground  - backdrop rendering for enabled state with frame enabled.
    ///     - WithFrameDisabledBackground - backdrop rendering for disabled state with frame enabled.
    ///     - NoFrameEnabledBackground    - backdrop rendering for enabled state with frame disabled.
    ///     - NoFrameDisabledBackground   - backdrop rendering for disabled state with frame disabled.
    /// </summary>
    public class FalagardStatic : WindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/Static";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardStatic(string type)
            : base(type)
        {
            FrameEnabled = false;
            _backgroundEnabled = false;

            RegisterProperty(new TplWindowRendererProperty<FalagardStatic, bool>(
                                 "FrameEnabled",
                                 "Property to get/set the state of the frame enabled setting for the FalagardStatic widget. Value is either \"True\" or \"False\".",
                                 (w, v) => w.SetFrameEnabled(v), w => w.IsFrameEnabled(), TypeName, true));

            RegisterProperty(new TplWindowRendererProperty<FalagardStatic, bool>(
                                 "BackgroundEnabled",
                                 "Property to get/set the state of the frame background setting for the FalagardStatic widget. Value is either \"True\" or \"False\".",
                                 (w, v) => w.SetBackgroundEnabled(v), w => w.IsBackgroundEnabled(), TypeName, true));
        }

        /// <summary>
        /// Return whether the frame for this static widget is enabled or disabled.
        /// </summary>
        /// <returns>
        /// true if the frame is enabled and will be rendered.  
        /// false is the frame is disabled and will not be rendered.
        /// </returns>
        public bool IsFrameEnabled()
        {
            return FrameEnabled;
        }

        /// <summary>
        /// Return whether the background for this static widget is enabled to disabled.
        /// </summary>
        /// <returns>
        /// true if the background is enabled and will be rendered.  
        /// false if the background is disabled and will not be rendered.
        /// </returns>
        public bool IsBackgroundEnabled()
        {
            return _backgroundEnabled;
        }

        /// <summary>
        /// Enable or disable rendering of the frame for this static widget.
        /// </summary>
        /// <param name="setting">
        /// true to enable rendering of a frame.  
        /// false to disable rendering of a frame.
        /// </param>
        public void SetFrameEnabled(bool setting)
        {
            if (FrameEnabled != setting)
            {
                FrameEnabled = setting;
                Window.Invalidate(false);
            }
        }

        /// <summary>
        /// Enable or disable rendering of the background for this static widget.
        /// </summary>
        /// <param name="setting">
        /// true to enable rendering of the background.  
        /// false to disable rendering of the background.
        /// </param>
        public void SetBackgroundEnabled(bool setting)
        {
            if (_backgroundEnabled != setting)
            {
                _backgroundEnabled = setting;
                Window.Invalidate(false);
            }
        }

        public override void CreateRenderGeometry()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

		    bool isEnabled = !Window.IsEffectiveDisabled();

            // render frame section
            if (FrameEnabled)
            {
                wlf.GetStateImagery(isEnabled ? "EnabledFrame" : "DisabledFrame").Render(Window);
            }

            // render background section
            if (_backgroundEnabled)
            {
                var imagery = FrameEnabled
                                  ? wlf.GetStateImagery(isEnabled
                                                            ? "WithFrameEnabledBackground"
                                                            : "WithFrameDisabledBackground")
                                  : wlf.GetStateImagery(isEnabled
                                                            ? "NoFrameEnabledBackground"
                                                            : "NoFrameDisabledBackground");
                // peform the rendering operation.
                imagery.Render(Window);
            }

            // render basic imagery
            wlf.GetStateImagery(isEnabled ? "Enabled" : "Disabled").Render(Window);
        }

        #region Fields

        /// <summary>
        /// True when the frame is enabled.
        /// </summary>
        protected bool FrameEnabled;

        /// <summary>
        /// true when the background is enabled.
        /// </summary>
        private bool _backgroundEnabled;

        #endregion
    }
}