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

using System.Collections.Generic;
using Lunatics.Mathematics;

namespace SharpCEGui.Base
{
    /// <summary>
    /// String component that moves a widget to appear as part of the string.
    /// </summary>
    public class RenderedStringWidgetComponent : RenderedStringComponent
    {
        public RenderedStringWidgetComponent()
        {
            d_windowPtrSynched = true;
            d_window = null;
            d_selected = false;
        }

        public RenderedStringWidgetComponent(string widgetName)
        {
            d_windowName = widgetName;
            d_windowPtrSynched = false;
            d_window = null;
            d_selected = false;
        }

        public RenderedStringWidgetComponent(Window widget)
        {
            d_windowPtrSynched = true;
            d_window = widget;
            d_selected = false;
        }

        protected RenderedStringWidgetComponent(RenderedStringWidgetComponent other)
            : base(other)
        {
            d_selected = other.d_selected;
            d_window = other.d_window;
            d_windowName = other.d_windowName;
            d_windowPtrSynched = other.d_windowPtrSynched;
        }

        /// <summary>
        /// Set the window to be controlled by this component.
        /// </summary>
        /// <param name="widgetName"></param>
        public void SetWindow(string widgetName)
        {
            d_windowName = widgetName;
            d_windowPtrSynched = false;
            d_window = null;
        }

        /// <summary>
        /// Set the window to be controlled by this component.
        /// </summary>
        /// <param name="widget"></param>
        public void SetWindow(Window widget)
        {
            d_window = widget;
            d_windowPtrSynched = true;
        }

        /// <summary>
        /// return the window currently controlled by this component
        /// </summary>
        /// <returns></returns>
        public Window GetWindow()
        {
            return GetEffectiveWindow(null); // FIXME: Perhaps?
        }

        #region Overrides of RenderedStringComponent

        public override List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Vector2 position, ColourRect modColours, Rectf? clipRect, float verticalSpace, float spaceExtra)
        {
            var window = GetEffectiveWindow(refWnd);
            if (window == null)
                return new List<GeometryBuffer>();

            var geomBuffers = new List<GeometryBuffer>();

            // HACK: re-adjust for inner-rect of parent
            float xAdj = 0, yAdj = 0;
            var parent = window.GetParent();

            if (parent != null)
            {
                var outer = parent.GetUnclippedOuterRect().Get();
                var inner = parent.GetUnclippedInnerRect().Get();
                xAdj = inner.d_min.X - outer.d_min.X;
                yAdj = inner.d_min.Y - outer.d_min.Y;
            }
            // HACK: re-adjust for inner-rect of parent (Ends)

            var finalPos = position;

            // handle formatting options
            switch (d_verticalFormatting)
            {
                case VerticalFormatting.BottomAligned:
                    finalPos.Y += verticalSpace - GetPixelSize(refWnd).Height;
                    break;

                case VerticalFormatting.Stretched:
                case VerticalFormatting.CentreAligned:
                    if (d_verticalFormatting == VerticalFormatting.Stretched)
                        System.GetSingleton().Logger.LogEvent("RenderedStringWidgetComponent::draw: " +
                                                              "VerticalFormatting.Stretched specified but is unsupported for Widget types; " +
                                                              "defaulting to VerticalFormatting.CentreAligned instead.");

                    finalPos.Y += (verticalSpace - GetPixelSize(refWnd).Height)/2;
                    break;

                case VerticalFormatting.TopAligned:
                    // nothing additional to do for this formatting option.
                    break;

                default:
                    throw new InvalidRequestException("unknown VerticalFormatting option specified.");
            }

            // render the selection if needed
            if (d_selectionImage != null && d_selected)
            {
                var selectArea = new Rectf(position, GetPixelSize(refWnd));
                var imgRenderSettings =new ImageRenderSettings(selectArea, clipRect, true, new ColourRect(0xFF002FFF));
                geomBuffers.AddRange(d_selectionImage.CreateRenderGeometry(imgRenderSettings));
            }

            // we do not actually draw the widget, we just move it into position.
            var wpos = new UVector2(new UDim(0, finalPos.X + d_padding.d_min.X - xAdj),
                                    new UDim(0, finalPos.Y + d_padding.d_min.Y - yAdj));

            window.SetPosition(wpos);

            return geomBuffers;
        }

        public override Sizef GetPixelSize(Window refWnd)
        {
            var sz = Sizef.Zero;
            var window = GetEffectiveWindow(refWnd);
            if (window != null)
            {
                sz = window.GetPixelSize();
                sz.Width += (d_padding.d_min.X + d_padding.d_max.X);
                sz.Height += (d_padding.d_min.Y + d_padding.d_max.Y);
            }

            return sz;
        }

        public override bool CanSplit()
        {
            return false;
        }

        public override RenderedStringComponent Split(Window refWnd, float splitPoint, bool firstComponent)
        {
            throw new InvalidRequestException("this component does not support being split.");
        }

        public override RenderedStringComponent Clone()
        {
            return new RenderedStringWidgetComponent(this);
        }

        public override int GetSpaceCount()
        {
            // widgets do not have spaces
            return 0;
        }

        public override void SetSelection(Window refWnd, float start, float end)
        {
            d_selected = (start != end);
        }

        #endregion

        protected Window GetEffectiveWindow(Window refWnd)
        {
            if (!d_windowPtrSynched)
            {
                if (refWnd == null)
                    return null;

                d_window = refWnd.GetChild(d_windowName);
                d_windowPtrSynched = true;
            }

            return d_window;
        }

        //! Name of window to manipulate
        protected string d_windowName;

        //! whether d_window is synched.
        protected bool d_windowPtrSynched;

        //! pointer to the window controlled by this component.
        protected Window d_window;

        // whether the image is marked as selected.
        protected bool d_selected;
    }
}