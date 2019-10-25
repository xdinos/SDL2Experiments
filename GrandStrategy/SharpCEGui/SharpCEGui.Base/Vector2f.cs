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
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class used as a two dimensional vector (aka a Point)
    /// </summary>
    [DebuggerDisplay("{d_x},{d_y}")]
    public struct Vector2f : IEquatable<Vector2f>
    {
        public static readonly Vector2f Zero = new Vector2f(0f, 0f);

        #region Fields

        public float d_x;

        public float d_y;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2f(float x, float y)
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
        public static Vector2f operator +(Vector2f lhs, Vector2f rhs)
        {
            return new Vector2f(lhs.d_x + rhs.d_x, lhs.d_y + rhs.d_y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector2f operator -(Vector2f lhs, Vector2f rhs)
        {
            return new Vector2f(lhs.d_x - rhs.d_x, lhs.d_y - rhs.d_y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector2f operator /(Vector2f lhs, Vector2f rhs)
        {
            return new Vector2f(lhs.d_x/rhs.d_x, lhs.d_y/rhs.d_y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector2f operator *(Vector2f lhs, Vector2f rhs)
        {
            return new Vector2f(lhs.d_x*rhs.d_x, lhs.d_y*rhs.d_y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2f operator *(Vector2f vector, float value)
        {
            return new Vector2f(vector.d_x * value, vector.d_y * value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(Vector2f lhs, Vector2f rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(Vector2f lhs, Vector2f rhs)
        {
            return !(lhs == rhs);
        }

        public static Vector2f Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static Vector2f Parse(string value, IFormatProvider formatProvider)
        {
            var matches = Parser.Matches(value);
            if (matches.Count == 1)
            {
                return new Vector2f(Single.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
                                    Single.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider));
            }

            throw new FormatException();
        }

        #region Implementation of IEquatable<Vector2>

        public bool Equals(Vector2f other)
        {
            return Math.Abs(d_x - other.d_x) < float.Epsilon &&
                   Math.Abs(d_y - other.d_y) < float.Epsilon;
        }

        #endregion

        #region Overrides of Object

        public override string ToString()
        {
            return String.Format("x:{0} y:{1}", d_x, d_y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2f && Equals((Vector2f) obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (d_x.GetHashCode()*397) ^ d_y.GetHashCode();
            }
        }

        #endregion

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
            new Regex(String.Format("x:({0}) y:({0})", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
    }
}