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
    /// Class encapsulating the 'Unified Box' - this is usually used for margin
    /// \par top, left, right and bottom represent offsets on each edge.
    /// </summary>
    /// <remarks>
    /// Name taken from W3 'box model'
    /// </remarks>
    public struct UBox : IEquatable<UBox>
    {
        public static readonly UBox Zero = new UBox(UDim.Zero);

        #region Fields

        public UDim d_top;
        public UDim d_left;
        public UDim d_bottom;
        public UDim d_right;

        #endregion

        #region Constructors

        public UBox(UDim margin)
            :this(margin,margin,margin,margin)
        {}

        public UBox(UDim dTop, UDim dLeft, UDim dBottom, UDim dRight)
        {
            d_top = dTop;
            d_left = dLeft;
            d_bottom = dBottom;
            d_right = dRight;
        }

        #endregion

        #region Operators

        public static bool operator ==(UBox lhs, UBox rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(UBox lhs, UBox rhs)
        {
            return !(lhs == rhs);
        }

        public static UBox operator +(UBox lhs, UBox rhs)
        {
            return new UBox(lhs.d_top + rhs.d_top,
                            lhs.d_left + rhs.d_left,
                            lhs.d_bottom + rhs.d_bottom,
                            lhs.d_right + rhs.d_right);
        }

        public static UBox operator -(UBox lhs, UBox rhs)
        {
            return new UBox(lhs.d_top - rhs.d_top,
                            lhs.d_left - rhs.d_left,
                            lhs.d_bottom - rhs.d_bottom,
                            lhs.d_right - rhs.d_right);
        }

        public static UBox operator *(UBox lhs, UBox rhs)
        {
            return new UBox(lhs.d_top*rhs.d_top,
                            lhs.d_left*rhs.d_left,
                            lhs.d_bottom*rhs.d_bottom,
                            lhs.d_right*rhs.d_right);
        }

        public static UBox operator *(UBox lhs, float value)
        {
            return new UBox(lhs.d_top*value,
                            lhs.d_left*value,
                            lhs.d_bottom*value,
                            lhs.d_right*value);
        }

        #endregion

        #region Implementation of IEquatable<UBox>

        public bool Equals(UBox other)
        {
            return d_top == other.d_top &&
                   d_left == other.d_left &&
                   d_bottom == other.d_bottom &&
                   d_right == other.d_right;
        }

        #endregion

        public static UBox Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static UBox Parse(string value, IFormatProvider formatProvider)
        {
            var matches = Parser.Matches(value);
            if (matches.Count == 1)
            {
                return new UBox(
                    UDim.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
                    UDim.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider),
                    UDim.Parse(matches[0].Groups[3].Captures[0].Value, formatProvider),
                    UDim.Parse(matches[0].Groups[4].Captures[0].Value, formatProvider));
            }

            throw new FormatException();
        }

        #region Overrides of Object

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                               "{{top:{0},left:{1},bottom:{2},right:{3}}}",
                                d_top, d_left, d_bottom, d_right );
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is UBox && Equals((UBox) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = d_top.GetHashCode();
                result = (result*397) ^ d_left.GetHashCode();
                result = (result*397) ^ d_bottom.GetHashCode();
                result = (result*397) ^ d_right.GetHashCode();
                return result;
            }
        }

        #endregion

        private static readonly Regex Parser =
            new Regex(
                @"{\s*top:\s*({.*?})\s*,\s*left:\s*({.*?})\s*,\s*bottom:\s*({.*?})\s*,\s*right:\s*({.*?})\s*}",
                RegexOptions.Compiled);
    }
}