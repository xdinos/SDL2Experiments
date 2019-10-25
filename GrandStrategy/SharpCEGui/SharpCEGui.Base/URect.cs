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
    public struct URect : IEquatable<URect>
    {
        public static readonly URect Zero = new URect(UVector2.Zero, UVector2.Zero);

        #region Fields

        public UVector2 d_min;
        public UVector2 d_max;

        #endregion

        #region Properties

        public UDim Left
        {
            get { return d_min.d_x; }
            set { d_min.d_x = value; }
        }

        public UDim Top
        {
            get { return d_min.d_y; }
            set { d_min.d_y = value; }
        }

        public UDim Right
        {
            get { return d_max.d_x; }
            set { d_max.d_x = value; }
        }

        public UDim Bottom
        {
            get { return d_max.d_y; }
            set { d_max.d_y = value; }
        }

        public UDim Width
        {
            get { return d_max.d_x - d_min.d_x; }
            set { d_max.d_x = d_min.d_x + value; }
        }

        public UDim Height
        {
            get { return d_max.d_y - d_min.d_y; }
            set { d_max.d_y = d_min.d_y + value; }
        }

        /// <summary>
        /// Get/Set the top-left position of Rectangle as a Vector2 
        /// (leaves size in tact)
        /// </summary>
        public UVector2 Position
        {
            get { return d_min; }
            set
            {
                var size = Size;
                d_min = value;
                Size = size;
            }
        }

        /// <summary>
        /// Get/Set the size of the Rectangle area.
        /// </summary>
        public USize Size
        {
            get { return new USize(Width, Height); }
            set { d_max = d_min + new UVector2(value.d_width, value.d_height); }
        }

        #endregion

        #region Constructors

        public URect(UDim left, UDim top, UDim right, UDim bottom)
            : this(new UVector2(left, top), new UVector2(right, bottom))
        {
        }

        public URect(UVector2 pos, USize size)
            : this(pos, new UVector2(pos.d_x + size.d_width, pos.d_y + size.d_height))
        {

        }

        public URect(UVector2 min, UVector2 max)
        {
            d_min = min;
            d_max = max;
        }

        #endregion

        #region Methods

        // TODO: URect GetIntersection(URect other)
        /// <summary>
        /// Return a <see cref="Rectf"/> that is the intersection 
        /// of 'this' with the 'other'
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <remarks>
        /// It can be assumed that 
        /// if <see cref="Left"/> == <see cref="Right"/>, 
        /// or <see cref="Top"/> == <see cref="Bottom"/>,
        /// or <see cref="Width"/> == 0,
        /// or or <see cref="Height"/> == 0, 
        /// then 'this' <see cref="Rectf"/> was totally 
        /// outside 'other' <see cref="Rectf"/>.
        /// </remarks>
        //public URect GetIntersection(URect other)
        //{
        //    if ((d_max.d_x <= other.d_min.d_x) ||
        //        (d_min.d_x >= other.d_max.d_x) ||
        //        (d_max.d_y <= other.d_min.d_y) ||
        //        (d_min.d_y >= other.d_max.d_y))
        //    {
        //        return Zero;
        //    }

        //    return new URect(
        //        (d_min.d_x > other.d_min.d_x) ? d_min.d_x : other.d_min.d_x,
        //        (d_max.d_x < other.d_max.d_x) ? d_max.d_x : other.d_max.d_x,
        //        (d_min.d_y > other.d_min.d_y) ? d_min.d_y : other.d_min.d_y,
        //        (d_max.d_y < other.d_max.d_y) ? d_max.d_y : other.d_max.d_y);
        //}

        #endregion

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(URect lhs, URect rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(URect lhs, URect rhs)
        {
            return !(lhs == rhs);
        }

        public static URect operator +(URect lhs, URect rhs)
        {
            return new URect(lhs.d_min + rhs.d_min, lhs.d_max + rhs.d_max);
        }

        public static URect operator -(URect lhs, URect rhs)
        {
            return new URect(lhs.d_min - rhs.d_min, lhs.d_max - rhs.d_max);
        }

        public static URect operator *(URect rect, float val)
        {
            return new URect(rect.d_min*val, rect.d_max*val);
        }

        #endregion

        public bool IsAbsolute()
        {
            return d_min.IsAbsolute() && d_max.IsAbsolute();
        }

        public static URect Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static URect Parse(string value, IFormatProvider formatProvider)
        {
            var text = value;
            text = text.Replace(" ", "")
                       .Replace("{", "")
                       .Replace("}", "");
            var values = text.Split(',');

            return new URect(
                    new UDim(Single.Parse(values[0], formatProvider), Single.Parse(values[1], formatProvider)),
                    new UDim(Single.Parse(values[2], formatProvider), Single.Parse(values[3], formatProvider)),
                    new UDim(Single.Parse(values[4], formatProvider), Single.Parse(values[5], formatProvider)),
                    new UDim(Single.Parse(values[6], formatProvider), Single.Parse(values[7], formatProvider)));

            //var matches = Parser.Matches(value);
            //if (matches.Count == 1)
            //{
            //    return new URect(
            //        UDim.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
            //        UDim.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider),
            //        UDim.Parse(matches[0].Groups[3].Captures[0].Value, formatProvider),
            //        UDim.Parse(matches[0].Groups[4].Captures[0].Value, formatProvider));
            //}

            //throw new FormatException();
        }

        #region Implementation of IEquatable<Rectangle>

        public bool Equals(URect other)
        {
            return d_min == other.d_min &&
                   d_max == other.d_max;
        }

        #endregion

        #region Overrides of Object

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture,
                                "{{{0},{1},{2},{3}}}",
                                d_min.d_x, d_min.d_y, d_max.d_x, d_max.d_y );
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is URect && Equals((URect)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (d_min.GetHashCode() * 397) ^ d_max.GetHashCode();
            }
        }

        #endregion

        private static readonly Regex Parser =
            //new Regex(String.Format("{{{{{0},{0}}},{{{0},{0}}},{{{0},{0}}},{{{0},{0}}}}}", RegExHelper.SingleRegEx),
            //          RegexOptions.Compiled);
            new Regex(@"{\s*({.*?})\s*,\s*({.*?})\s*,\s*({.*?})\s*,\s*({.*?})\s*}",
                  RegexOptions.Compiled);
    }
}