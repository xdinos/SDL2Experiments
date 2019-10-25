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
    /// 
    /// </summary>
    public struct Vector3f : IEquatable<Vector3f>
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public static readonly Vector3f Zero = new Vector3f(0f, 0f, 0f);

        /// <summary>
        /// 
        /// </summary>
        public float d_x;

        /// <summary>
        /// 
        /// </summary>
        public float d_y;

        /// <summary>
        /// 
        /// </summary>
        public float d_z;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3f(float x, float y, float z)
        {
            d_x = x;
            d_y = y;
            d_z = z;
        }

        #endregion

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(Vector3f lhs, Vector3f rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(Vector3f lhs, Vector3f rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector3f operator +(Vector3f lhs, Vector3f rhs)
        {
            return new Vector3f(lhs.d_x + rhs.d_x, lhs.d_y + rhs.d_y, lhs.d_z + rhs.d_z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3f operator *(Vector3f vector, float value)
        {
            return new Vector3f(vector.d_x*value, vector.d_y*value, vector.d_z*value);
        }
        
        #endregion

        public static Vector3f Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static Vector3f Parse(string value, IFormatProvider formatProvider)
        {
            var matches = Parser.Matches(value);
            if (matches.Count == 1)
            {
                return new Vector3f(Single.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
                                    Single.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider),
                                    Single.Parse(matches[0].Groups[3].Captures[0].Value, formatProvider));
            }

            throw new FormatException();
        }

        #region Implementation of IEquatable<Vector3>

        public bool Equals(Vector3f other)
        {
            return Math.Abs(d_x - other.d_x) < float.Epsilon &&
                   Math.Abs(d_y - other.d_y) < float.Epsilon &&
                   Math.Abs(d_z - other.d_z) < float.Epsilon;
        }

        #endregion

        #region Overrides of Object

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "x:{0} y:{1} z:{2}", d_x, d_y, d_z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector3f && Equals((Vector3f) obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = d_x.GetHashCode();
                result = (result*397) ^ d_y.GetHashCode();
                result = (result*397) ^ d_z.GetHashCode();
                return result;
            }
        }

        #endregion

        private static readonly Regex Parser =
            new Regex(String.Format("x:({0}) y:({0}) z:({0})", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
    }
}