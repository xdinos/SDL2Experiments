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
using System.Globalization;
using System.Text.RegularExpressions;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that holds details of colours for the four corners of a rectangle.
    /// </summary>
    public class ColourRect
    {
        #region Contructors

        public ColourRect()
            : this(new Colour(1f, 1f, 1f, 1f))
        {
        }

        public ColourRect(int argb)
            : this(new Colour(argb))
        {
        }

        public ColourRect(uint argb)
            : this(new Colour(argb))
        {
        }

        public ColourRect(Colour colour)
            : this(colour, colour, colour, colour)
        {
        }

        public ColourRect(int topLeft, int topRight, int bottomLeft, int bottomRight)
            : this(new Colour(topLeft), new Colour(topRight), new Colour(bottomLeft), new Colour(bottomRight))
        {
            
        }

        public ColourRect(uint topLeft, uint topRight, uint bottomLeft, uint bottomRight)
            : this(new Colour(topLeft), new Colour(topRight), new Colour(bottomLeft), new Colour(bottomRight))
        {

        }

        public ColourRect(Colour dTopLeft, Colour dTopRight, Colour dBottomLeft, Colour dBottomRight)
        {
            d_top_left = dTopLeft;
            d_top_right = dTopRight;
            d_bottom_left = dBottomLeft;
            d_bottom_right = dBottomRight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colours"></param>
        public ColourRect(ColourRect colours)
        {
            d_top_left = colours.d_top_left;
            d_top_right = colours.d_top_right;
            d_bottom_left = colours.d_bottom_left;
            d_bottom_right = colours.d_bottom_right;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colour"></param>
        public void SetColours(Colour colour)
        {
            d_top_left = colour;
            d_top_right = colour;
            d_bottom_left = colour;
            d_bottom_right = colour;
        }

        public bool IsMonochromatic()
        {
            return (d_top_left == d_top_right) &&
                   (d_top_right == d_bottom_left) &&
                   (d_bottom_left == d_bottom_right);
        }

        public Colour GetColourAtPoint(float x, float y)
        {
            var h1 = (d_top_right - d_top_left) * x + d_top_left;
            var h2 = (d_bottom_right - d_bottom_left) * x + d_bottom_left;

            return (h2 - h1) * y + h1;
        }

        public ColourRect GetSubRectangle(float left, float right, float top, float bottom)
        {
            return new ColourRect(GetColourAtPoint(left, top),
                                  GetColourAtPoint(right, top),
                                  GetColourAtPoint(left, bottom),
                                  GetColourAtPoint(right, bottom));
        }

        public void ModulateAlpha(float alpha)
        {
            d_top_left.Alpha = d_top_left.Alpha * alpha;
            d_top_right.Alpha = d_top_right.Alpha * alpha;
            d_bottom_left.Alpha = d_bottom_left.Alpha * alpha;
            d_bottom_right.Alpha = d_bottom_right.Alpha * alpha;
        }

        public static ColourRect Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static ColourRect Parse(string value, IFormatProvider formatProvider)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (value.Length == 8)
            {
                int all = 0;
                if (Int32.TryParse(value, NumberStyles.HexNumber, formatProvider, out all))
                {
                    return new ColourRect(new Colour(all));
                }
            }

            var matches = Parser.Matches(value);
            if (matches.Count == 1)
            {
                return new ColourRect(Colour.Parse(matches[0].Groups[1].Captures[0].Value),
                                      Colour.Parse(matches[0].Groups[2].Captures[0].Value),
                                      Colour.Parse(matches[0].Groups[3].Captures[0].Value),
                                      Colour.Parse(matches[0].Groups[4].Captures[0].Value));
            }

            throw new FormatException();
        }

        public override string ToString()
        {
            return String.Format("tl:{0:X8} tr:{1:X8} bl:{2:X8} br:{3:X8}",
                                 d_top_left.ToAlphaRGB(),
                                 d_top_right.ToAlphaRGB(),
                                 d_bottom_left.ToAlphaRGB(),
                                 d_bottom_right.ToAlphaRGB());
        }

        public static ColourRect operator +(ColourRect left, ColourRect right)
        {
            return new ColourRect(left.d_top_left + right.d_top_left,
                                  left.d_top_right + right.d_top_right,
                                  left.d_bottom_left + right.d_bottom_left,
                                  left.d_bottom_right + right.d_bottom_right);
        }

        public static ColourRect operator *(ColourRect left, float value)
        {
            return new ColourRect(left.d_top_left*value,
                                  left.d_top_right*value,
                                  left.d_bottom_left*value,
                                  left.d_bottom_right*value);
        }

        public static ColourRect operator *(ColourRect left, ColourRect right)
        {
            return new ColourRect(left.d_top_left*right.d_top_left,
                                  left.d_top_right*right.d_top_right,
                                  left.d_bottom_left*right.d_bottom_left,
                                  left.d_bottom_right*right.d_bottom_right);
        }

        #region Fields

        public Colour d_top_left;
        public Colour d_top_right;
        public Colour d_bottom_left;
        public Colour d_bottom_right;

        #endregion

        private static readonly Regex Parser =
            //new Regex(@"tl:(0x[0-9A-F]*)\s*tr:(0x[0-9A-F]*)\s*bl:(0x[0-9A-F]*)\s*br:(0x[0-9A-F]*)",
            new Regex(@"tl:([0-9A-F]*)\s+tr:([0-9A-F]*)\s+bl:([0-9A-F]*)\s+br:([0-9A-F]*)",
                      RegexOptions.Compiled);
    }
}