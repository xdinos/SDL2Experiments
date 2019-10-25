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
    /// Class used as a two dimensional vector (aka a Point)
    /// </summary>
    public struct UVector2 : IEquatable<UVector2>
    {
        public static readonly UVector2 Zero = new UVector2(UDim.Zero, UDim.Zero);

        #region Fields

        public UDim d_x;

        public UDim d_y;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public UVector2(UDim x, UDim y)
        {
            d_x = x;
            d_y = y;
        }

        // TODO: Implicit conversion from/to System.Drawing.PointF ??
        ///// <summary>Implicit conversion of this class to System.Drawing.PointF</summary>
        ///// <param name="vector">Vector to be converted</param>
        ///// <returns>An equivalent vector in the PointF class</returns>
        //public static implicit operator PointF(Vector2 vector)
        //{
        //    return new PointF(vector.X, vector.Y);
        //}

        ///// <summary>Implicit conversion of System.Drawing.PointF to this class</summary>
        ///// <param name="point">Point to be converted</param>
        ///// <returns>An equivalent vector in the Vector2 class</returns>
        //public static implicit operator Vector2(PointF point)
        //{
        //    return new Vector2(point.X, point.Y);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static UVector2 operator +(UVector2 lhs, UVector2 rhs)
        {
            return new UVector2(lhs.d_x + rhs.d_x, lhs.d_y + rhs.d_y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static UVector2 operator -(UVector2 lhs, UVector2 rhs)
        {
            return new UVector2(lhs.d_x - rhs.d_x, lhs.d_y - rhs.d_y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static UVector2 operator /(UVector2 lhs, UVector2 rhs)
        {
            return new UVector2(lhs.d_x / rhs.d_x, lhs.d_y / rhs.d_y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static UVector2 operator *(UVector2 lhs, UVector2 rhs)
        {
            return new UVector2(lhs.d_x * rhs.d_x, lhs.d_y * rhs.d_y);
        }

        public static UVector2 operator *(UVector2 vec, float val)
        {
            return new UVector2(vec.d_x * val, vec.d_y * val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(UVector2 lhs, UVector2 rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(UVector2 lhs, UVector2 rhs)
        {
            return !(lhs == rhs);
        }

        public bool IsAbsolute()
        {
            return d_x.IsAbsolute() && d_y.IsAbsolute();
        }

        public static UVector2 Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static UVector2 Parse(string value, IFormatProvider formatProvider)
        {
            var text = value;
            text = text.Replace(" ", "")
                       .Replace("{", "")
                       .Replace("}", "");
            var values = text.Split(',');

            return new UVector2(
                    new UDim(Single.Parse(values[0], formatProvider),
                             Single.Parse(values[1], formatProvider)),
                    new UDim(Single.Parse(values[2], formatProvider),
                             Single.Parse(values[3], formatProvider)));

            //var matches = Parser.Matches(value);
            //if (matches.Count == 1)
            //{
            //    return new UVector2(
            //        new UDim(Single.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
            //                 Single.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider)),
            //        new UDim(Single.Parse(matches[0].Groups[3].Captures[0].Value, formatProvider),
            //                 Single.Parse(matches[0].Groups[4].Captures[0].Value, formatProvider)));
            //}

            //throw new FormatException();
        }

        public bool Equals(UVector2 other)
        {
            return d_x == other.d_x && d_y == other.d_y;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{{0},{1}}}",  d_x, d_y );
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is UVector2 && Equals((UVector2)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (d_x.GetHashCode() * 397) ^ d_y.GetHashCode();
            }
        }

        // TODO: ...
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public SizeF AsSize()
        //{
        //    return new SizeF(X, Y);
        //}

        private static readonly Regex Parser =
            new Regex(String.Format(@"{{\s*{{\s*({0})\s*,\s*({0})\s*}}\s*,\s*{{\s*({0})\s*,\s*({0})\s*}}\s*}}", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
        //new Regex(@"\{\s*(\{[\S|\s]*\})\s*,\s*(\{[\S|\s]*\})\s*\}",
        //          RegexOptions.Compiled);
    }
}