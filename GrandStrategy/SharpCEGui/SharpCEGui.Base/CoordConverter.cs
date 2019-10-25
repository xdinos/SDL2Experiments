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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Utility class that helps in converting various types of co-ordinate between
    /// absolute screen positions and positions offset from the top-left corner of
    /// a given Window object.
    /// </summary>
    public static class CoordConverter
    {
        /// <summary>
        /// Static method used to return a float value rounded to the nearest integer.
        /// This method is used throughout the library to ensure that elements are
        /// kept at integer pixel positions on the display if user wishes so.
        /// </summary>
        /// <param name="x">
        /// Expression to be rounded to nearest whole number
        /// </param>
        /// <returns>
        /// x after having been rounded
        /// </returns>
        /// <seealso cref="Element.SetPixelAligned"/>
        public static float AlignToPixels(float x)
        {
            return (int) ((x) + ((x) > 0.0f ? 0.5f : -0.5f));
        }

        /// <summary>
        /// converts given UDim to absolute value
        /// </summary>
        /// <param name="u"></param>
        /// <param name="base"></param>
        /// <param name="pixelAlign"></param>
        /// <returns></returns>
        public static float AsAbsolute(UDim u, float @base, bool pixelAlign = true)
        {
            return pixelAlign ? AlignToPixels(@base*u.d_scale + u.d_offset) : @base*u.d_scale + u.d_offset;
        }
        
        /// <summary>
        /// converts given USize to absolute Size
        /// </summary>
        /// <param name="v"></param>
        /// <param name="base"></param>
        /// <param name="pixelAlign"></param>
        /// <returns></returns>
        public static Sizef AsAbsolute(USize v, Sizef @base, bool pixelAlign = true)
        {
            return new Sizef(AsAbsolute(v.d_width, @base.Width, pixelAlign),
                             AsAbsolute(v.d_height, @base.Height, pixelAlign));
        }

        /// <summary>
        /// converts given <see cref="UVector2"/> to absolute <see cref="Vector2f"/>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="base"></param>
        /// <param name="pixelAlign"></param>
        /// <returns></returns>
        public static Lunatics.Mathematics.Vector2 AsAbsolute(UVector2 v, Sizef @base, bool pixelAlign = true)
        {
            return new Lunatics.Mathematics.Vector2(AsAbsolute(v.d_x, @base.Width, pixelAlign),
                                                   AsAbsolute(v.d_y, @base.Height, pixelAlign));
        }

        public static Rectf AsAbsolute(URect r, Sizef @base, bool pixelAlign = true)
        {
            return new Rectf(AsAbsolute(r.d_min.d_x, @base.Width, pixelAlign),
                             AsAbsolute(r.d_min.d_y, @base.Height, pixelAlign),
                             AsAbsolute(r.d_max.d_x, @base.Width, pixelAlign),
                             AsAbsolute(r.d_max.d_y, @base.Height, pixelAlign));
        }

        public static float AsRelative(UDim u, float @base)
        {
            return (Math.Abs(@base - 0.0f) > float.Epsilon)
                       ? u.d_offset/@base + u.d_scale
                       : 0.0f;
        }


        /// <summary>
        /// Convert a screen Vector2 pixel point to a window co-ordinate point,
        /// specified in pixels.
        /// </summary>
        /// <param name="window">
        /// Window object to use as a target for the conversion.
        /// </param>
        /// <param name="vec">
        /// Vector2 object describing the point to be converted.
        /// </param>
        /// <returns>
        /// Vector2 object describing a window co-ordinate point that is equivalent
        /// to screen based Vector2 point \a vec.
        /// </returns>
        public static Lunatics.Mathematics.Vector2 ScreenToWindow(Window window, Lunatics.Mathematics.Vector2 vec)
        {
            return vec - GetBaseValue(window);
        }

        public static float ScreenToWindowX(Window window, float x)
        {
            return x - GetBaseXValue(window);
        }

        /// <summary>
        /// Return the base X co-ordinate of the given Window object.
        /// </summary>
        /// <param name="window">
        /// Window object to return base position for.
        /// </param>
        /// <returns>
        /// float value indicating the base on-screen pixel location of the window
        /// on the x axis (i.e. The screen co-ord of the window's left edge).
        /// </returns>
        public static float GetBaseXValue(Window window)
        {
            var parent = window.GetParent();

            var parent_rect = parent != null
                                  ? parent.GetChildContentArea(window.IsNonClient()).Get()
                                  : new Rectf(Lunatics.Mathematics.Vector2.Zero, window.GetRootContainerSize());

            var parent_width = parent_rect.Width;
            var baseX = parent_rect.d_min.X;

            baseX += AsAbsolute(window.GetArea().d_min.d_x, parent_width);

            switch (window.GetHorizontalAlignment())
            {
                case HorizontalAlignment.Centre:
                    baseX += (parent_width - window.GetPixelSize().Width)*0.5f;
                    break;
                case HorizontalAlignment.Right:
                    baseX += parent_width - window.GetPixelSize().Width;
                    break;
                default:
                    break;
            }

            return AlignToPixels(baseX);
        }

        /// <summary>
        /// Return the base Y co-ordinate of the given Window object.
        /// </summary>
        /// <param name="window">
        /// Window object to return base position for.
        /// </param>
        /// <returns>
        /// float value indicating the base on-screen pixel location of the window
        /// on the y axis (i.e. The screen co-ord of the window's top edge).
        /// </returns>
        public static float GetBaseYValue(Window window)
        {
            var parent = window.GetParent();

            var parent_rect = parent != null
                                  ? parent.GetChildContentArea(window.IsNonClient()).Get()
                                  : new Rectf(Lunatics.Mathematics.Vector2.Zero, window.GetRootContainerSize());

            var parent_height = parent_rect.Height;
            var baseY = parent_rect.d_min.Y;

            baseY += AsAbsolute(window.GetArea().d_min.d_y, parent_height);

            switch (window.GetVerticalAlignment())
            {
                case VerticalAlignment.Centre:
                    baseY += (parent_height - window.GetPixelSize().Height)*0.5f;
                    break;
                case VerticalAlignment.Bottom:
                    baseY += parent_height - window.GetPixelSize().Height;
                    break;
                default:
                    break;
            }

            return AlignToPixels(baseY);
        }

        /// <summary>
        /// Return the base position of the given Window object.
        /// </summary>
        /// <param name="window">
        /// Window object to return base position for.
        /// </param>
        /// <returns>
        /// Vector2 value indicating the base on-screen pixel location of the window
        /// object. (i.e. The screen co-ord of the window's top-left corner).
        /// </returns>
        private static Lunatics.Mathematics.Vector2 GetBaseValue(Window window)
        {
            return new Lunatics.Mathematics.Vector2(GetBaseXValue(window), GetBaseYValue(window));
        }
    }
}