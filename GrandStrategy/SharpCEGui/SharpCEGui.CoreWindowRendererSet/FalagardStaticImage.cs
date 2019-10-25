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
    /// StaticImage class for the FalagardBase module.
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
    ///     - WithFrameImage              - image rendering when frame is enabled
    ///     - NoFrameImage                - image rendering when frame is disabled (defaults to WithFrameImage if not present)
    /// </summary>
    public class FalagardStaticImage : FalagardStatic
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public new const string TypeName = "Core/StaticImage";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardStaticImage(string type)
            : base(type)
        {
            _image = null;
            RegisterProperty(new TplWindowRendererProperty<FalagardStaticImage, Image>(
                                 "Image",
                                 "Property to get/set the image for the FalagardStaticImage widget. Value should be \"set:[imageset name] image:[image name]\".",
                                 (w, v) => w.SetImage(v), w => w.GetImage(), TypeName));
        }

        /// <summary>
        /// Set the image for this FalagardStaticImage widget
        /// </summary>
        /// <param name="img"></param>
        public void SetImage(Image img)
        {
            _image = img;
            Window.Invalidate(false);
        }

        /// <summary>
        /// Get the image for this FalagardStaticImage widget
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            return _image;
        }

        public override void CreateRenderGeometry()
        {
            // base class rendering
            base.CreateRenderGeometry();

            // render image if there is one
            if (_image != null)
            {
                // get WidgetLookFeel for the assigned look.
                var wlf = GetLookNFeel();
                var imageryName = (!FrameEnabled && wlf.IsStateImageryPresent("NoFrameImage"))
                                      ? "NoFrameImage"
                                      : "WithFrameImage";
                wlf.GetStateImagery(imageryName).Render(Window);
            }
        }

        #region Fields

        private Image _image;

        #endregion
    }
}