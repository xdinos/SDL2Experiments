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
    public struct UDim : IEquatable<UDim>
    {
        public static readonly UDim Zero = new UDim(0f, 0f);
        //public static readonly UDim Relative = new UDim(1f, 0f);
        public static readonly UDim Pixel = new UDim(0f, 1f);

        public float d_scale;
        public float d_offset;

        #region Constructors

        public UDim(float scale, float offset)
        {
            d_scale = scale;
            d_offset = offset;
        }

        public static UDim Absolute(float value)
        {
            return new UDim(0f, value);
        }

        public static UDim Relative(float value)
        {
            return new UDim(value, 0f);
        }

        #endregion

        #region Operators

        public static bool operator ==(UDim lhs, UDim rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(UDim lhs, UDim rhs)
        {
            return !(lhs == rhs);
        }

        public static UDim operator +(UDim lhs,UDim rhs)
        {
            return new UDim(lhs.d_scale + rhs.d_scale, lhs.d_offset + rhs.d_offset);
        }

        public static UDim operator -(UDim lhs, UDim rhs)
        {
            return new UDim(lhs.d_scale - rhs.d_scale, lhs.d_offset - rhs.d_offset);
        }

        public static UDim operator *(UDim lhs, UDim rhs)
        {
            return new UDim(lhs.d_scale * rhs.d_scale, lhs.d_offset * rhs.d_offset);
        }

        public static UDim operator /(UDim lhs, UDim rhs)
        {
            // division by zero sets component to zero.  Not technically correct
            // but probably better than exceptions and/or NaN values.
            return new UDim(
                Math.Abs(rhs.d_scale - 0.0f) < float.Epsilon ? 0.0f : lhs.d_scale/rhs.d_scale,
                Math.Abs(rhs.d_offset - 0.0f) < float.Epsilon ? 0.0f : lhs.d_offset/rhs.d_offset);
        }

        public static UDim operator *(UDim lhs, float value)
        {
            return new UDim(lhs.d_scale * value, lhs.d_offset * value);
        }

        public static UDim operator *(float value, UDim rhs)
        {
            return new UDim(rhs.d_scale * value, rhs.d_offset * value);
        }

        #endregion
        
        public bool IsAbsolute()
        {
            return d_scale == 0f;
        }

        public static UDim Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static UDim Parse(string value, IFormatProvider formatProvider)
        {
            var text = value;
            text = text.Replace(" ", "")
                       .Replace("{", "")
                       .Replace("}", "");
            var values = text.Split(',');

            return new UDim(Single.Parse(values[0], formatProvider),
                             Single.Parse(values[1], formatProvider));

            //var matches = Parser.Matches(value.Trim());
            //if (matches.Count == 1)
            //{
            //    return new UDim(Single.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
            //                    Single.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider));
            //}

            //throw new FormatException();
        }

        #region Implementation of IEquatable<UDim>

        public bool Equals(UDim other)
        {
            return Math.Abs(d_scale - other.d_scale) < float.Epsilon &&
                   Math.Abs(d_offset - other.d_offset) < float.Epsilon;
        }

        #endregion

        #region Overrides of Object

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{{0},{1}}}", d_scale, d_offset );
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is UDim && Equals((UDim) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (d_scale.GetHashCode()*397) ^ d_offset.GetHashCode();
            }
        }

        #endregion

        private static readonly Regex Parser =
            new Regex(String.Format(@"{{\s*({0})\s*,\s*({0})\s*}}", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
    }
}