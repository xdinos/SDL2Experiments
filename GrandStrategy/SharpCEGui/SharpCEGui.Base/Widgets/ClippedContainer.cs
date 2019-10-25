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

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Helper container window that has configurable clipping.
    /// Used by the ItemListbox widget.
    /// </summary>
    /// <remarks>
    /// This class is deprecated and is scheduled for removal.  The function this
    /// class used to provide was broken when the inner-rect (aka client area)
    /// support got fixed.  The good news is that fixing inner-rect support
    /// effectively negated the need for this class anyway - clipping areas can
    /// now be established in the looknfeel and extracted via the WindowRenderer.
    /// </remarks>
    [Obsolete]
    public class ClippedContainer : Window
    {
        /// <summary>
        /// Type name for ClippedContainer.
        /// </summary>
        public const string WidgetTypeName = "ClippedContainer";

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ClippedContainer";

        /// <summary>
        /// Constructor for ClippedContainer objects.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public ClippedContainer(string type, string name)
            : base(type, name)
        {
            _clipArea = Rectf.Zero;
            _clipperWindow = null;
        }

        /// <summary>
        /// Return the current clipping rectangle.
        /// </summary>
        /// <returns>
        /// Rect object describing the clipping area in pixel that will be applied during rendering.
        /// </returns>
        public Rectf GetClipArea()
        {
            return _clipArea;
        }

        /// <summary>
        /// Returns the reference window used for converting the clipper rect to screen space.
        /// </summary>
        /// <returns></returns>
        public Window GetClipperWindow()
        {
            return _clipperWindow;
        }

        /// <summary>
        /// Set the custom clipper area in pixels.
        /// </summary>
        /// <param name="r"></param>
        public void SetClipArea(Rectf r)
        {
            if (_clipArea != r)
            {
                _clipArea = r;
                Invalidate(false);
                NotifyClippingChanged();
            }
        }

        /// <summary>
        /// Set the clipper reference window.
        /// </summary>
        /// <param name="w">
        /// The window to be used a base for converting the custom clipper rect to
        /// screen space. NULL if the clipper rect is relative to the screen.
        /// </param>
        public void SetClipperWindow(Window w)
        {
            if (_clipperWindow != w)
            {
                _clipperWindow = w;
                Invalidate();
                NotifyClippingChanged();
            }
        }

        protected override Rectf GetUnclippedInnerRectImpl(bool skipAllPixelAlignment)
        {
            // This is obviously doing nothing.  The reason being that whas this
            // used to to is now handled correctly via the fixed 'inner rect' usage,
            // meaning that the looknfeel named areas can be employed to do the correct
            // clipping.  Fixing the inner rect support actually broke this anyhow,
            // since it only worked because the inner rect support was broken.  As
            // such, ClippedContainer serves no useful purpose and will be removed.
            return base.GetUnclippedInnerRectImpl(skipAllPixelAlignment);
        }

        protected override void DrawSelf(RenderingContext ctx)
        {
            base.DrawSelf(ctx);
        }

        #region Fields

        /// <summary>
        /// the pixel rect to be used for clipping relative to either a window or the screen.
        /// </summary>
        private Rectf _clipArea;

        /// <summary>
        /// the base window which the clipping rect is relative to.
        /// </summary>
        private Window _clipperWindow;

        #endregion
    }
}