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
    public struct Quaternion : IEquatable<Quaternion>
    {
        public static readonly Quaternion Identity = new Quaternion(1f, 0f, 0f, 0f);
        public static readonly Quaternion Zero = new Quaternion(0f, 0f, 0f, 0f);

        private static readonly float DegreesToRadians;
        static Quaternion()
        {
            DegreesToRadians = (float) ((4.0f*Math.Atan2(1.0f, 1.0f))/180.0f);
        }

        #region Fields

        /// <summary>
        /// Imaginary part
        /// </summary>
        public float d_w;

        /// <summary>
        /// X component of the vector part
        /// </summary>
        public float d_x;

        /// <summary>
        /// Y component of the vector part
        /// </summary>
        public float d_y;

        /// <summary>
        /// Z component of the vector part
        /// </summary>
        public float d_z;

        #endregion

        #region Constructors

        public Quaternion(float w, float x, float y, float z)
        {
            d_w = w;
            d_x = x;
            d_y = y;
            d_z = z;
        }

        #endregion

        /// <summary>
        /// quaternion dot product
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public float Dot(Quaternion v)
        {
            return d_w * v.d_w + d_x * v.d_x + d_y * v.d_y + d_z * v.d_z;
        }

        /// <summary>
        /// computers and returns the length of this quaternion
        /// </summary>
        /// <returns></returns>
        public float Length()
        {
            return (float)Math.Sqrt((d_w * d_w) + (d_x * d_x) + (d_y * d_y) + (d_z * d_z));
        }

        /// <summary>
        /// normalises this quaternion and returns it's length (since it has to be computed anyways)
        /// </summary>
        /// <returns></returns>
        public float Normalise()
        {
            var len = Length();
            var factor = 1.0f / len;

            this = this * factor;
            
            return len;
        }
        
        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            return !(lhs == rhs);
        }

        public static Quaternion operator -(Quaternion q)
        {
            return new Quaternion(-q.d_w, -q.d_x, -q.d_y, -q.d_z);
        }

        //! scalar multiplication operator
        public static Quaternion operator *(float v, Quaternion q)
        {
            return new Quaternion(v*q.d_w, v*q.d_x, v*q.d_y, v*q.d_z);
        }

        public static Quaternion operator *(Quaternion q, float v)
        {
            return new Quaternion(v*q.d_w, v*q.d_x, v*q.d_y, v*q.d_z);
        }

        //! addition operator
        public static Quaternion operator +(Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(lhs.d_w + rhs.d_w,
                                  lhs.d_x + rhs.d_x,
                                  lhs.d_y + rhs.d_y,
                                  lhs.d_z + rhs.d_z);
        }

        public static Quaternion operator -(Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(lhs.d_w - rhs.d_w,
                                  lhs.d_x - rhs.d_x,
                                  lhs.d_y - rhs.d_y,
                                  lhs.d_z - rhs.d_z);
        }

        /*!
        \brief quaternion multiplication (not commutative!)

        Lets say we have quaternion A describing a rotation and another quaternion B.
        If we write C = A * B, C is actually describing a rotation we would get if we
        rotated Identity by A and then rotated the result by B
        */
        public static Quaternion operator * (Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(
                lhs.d_w * rhs.d_w - lhs.d_x * rhs.d_x - lhs.d_y * rhs.d_y - lhs.d_z * rhs.d_z,
                lhs.d_w * rhs.d_x + lhs.d_x * rhs.d_w + lhs.d_y * rhs.d_z - lhs.d_z * rhs.d_y,
                lhs.d_w * rhs.d_y + lhs.d_y * rhs.d_w + lhs.d_z * rhs.d_x - lhs.d_x * rhs.d_z,
                lhs.d_w * rhs.d_z + lhs.d_z * rhs.d_w + lhs.d_x * rhs.d_y - lhs.d_y * rhs.d_x
            );
        }
        
        #endregion

        #region Parse Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Quaternion Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static Quaternion Parse(string value, IFormatProvider formatProvider)
        {
            value = value.ToLowerInvariant().Trim();
            if (value.StartsWith("w"))
            {
                var matches = Parser.Matches(value);
                if (matches.Count == 1)
                {
                    return new Quaternion(
                        Single.Parse(matches[0].Groups[1].Captures[0].Value, NumberStyles.Any, formatProvider),
                        Single.Parse(matches[0].Groups[2].Captures[0].Value, NumberStyles.Any, formatProvider),
                        Single.Parse(matches[0].Groups[3].Captures[0].Value, NumberStyles.Any, formatProvider),
                        Single.Parse(matches[0].Groups[4].Captures[0].Value, NumberStyles.Any, formatProvider));
                }
            }
            else
            {
                var matches = ParserAngles.Matches(value);
                if (matches.Count == 1)
                {
                    return FromEulerAnglesDegrees(
                        Single.Parse(matches[0].Groups[1].Captures[0].Value, NumberStyles.Any, formatProvider),
                        Single.Parse(matches[0].Groups[2].Captures[0].Value, NumberStyles.Any, formatProvider),
                        Single.Parse(matches[0].Groups[3].Captures[0].Value, NumberStyles.Any, formatProvider));
                }
            }

            throw new FormatException();
        }
        
        private static readonly Regex Parser =
            new Regex(String.Format(@"\s*w:({0})\s*x:\s*({0})\s*y:\s*({0})\s*z:\s*({0})\s*", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
        private static readonly Regex ParserAngles =
            new Regex(String.Format(@"\s*x:\s*({0})\s*y:\s*({0})\s*z:\s*({0})\s*", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
        #endregion

        public static Quaternion FromEulerAnglesRadians(float x, float y, float z)
        {
            // the order of rotation:
            // 1) around Z
            // 2) around Y
            // 3) around X
            // even though it's passed in reverse...

            var sin_z_2 = (float)Math.Sin(0.5f * z);
	        var sin_y_2 = (float)Math.Sin(0.5f * y);
	        var sin_x_2 = (float)Math.Sin(0.5f * x);

	        var cos_z_2 = (float)Math.Cos(0.5f * z);
	        var cos_y_2 = (float)Math.Cos(0.5f * y);
	        var cos_x_2 = (float)Math.Cos(0.5f * x);

            return new Quaternion(cos_z_2*cos_y_2*cos_x_2 + sin_z_2*sin_y_2*sin_x_2,
                                  cos_z_2*cos_y_2*sin_x_2 - sin_z_2*sin_y_2*cos_x_2,
                                  cos_z_2*sin_y_2*cos_x_2 + sin_z_2*cos_y_2*sin_x_2,
                                  sin_z_2*cos_y_2*cos_x_2 - cos_z_2*sin_y_2*sin_x_2);
        }
        
        public static Quaternion FromEulerAnglesDegrees(float x, float y, float z)
        {
            return FromEulerAnglesRadians(x*DegreesToRadians, y*DegreesToRadians, z*DegreesToRadians);
        }

        public static Quaternion FromAxisAngleRadians(Vector3f axis, float rotation)
        {
            var halfRotation = 0.5f * rotation;
            var halfSin = (float)Math.Sin(halfRotation);

            return new Quaternion((float) Math.Cos(halfRotation),
                                  halfSin*axis.d_x,
                                  halfSin*axis.d_y,
                                  halfSin*axis.d_z);
        }

        public static Quaternion FromaxisAngleDegrees(Vector3f axis, float rotation)
        {
            return FromAxisAngleRadians(axis, rotation * DegreesToRadians);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="position"></param>
        /// <param name="shortestPath"></param>
        /// <returns></returns>
        public static Quaternion Slerp(Quaternion left, Quaternion right, float position, bool shortestPath = false)
        {
            // Geometric Tools, LLC
            // Copyright (c) 1998-2010
            // Distributed under the Boost Software License, Version 1.0.
            // http://www.boost.org/LICENSE_1_0.txt
            // http://www.geometrictools.com/License/Boost/LICENSE_1_0.txt

            var vcos = left.Dot(right);
            Quaternion _right;

            // Do we need to invert rotation?
            if (vcos < 0.0f && shortestPath)
            {
                vcos = -vcos;
                _right = -right;
            }
            else
            {
                _right = right;
            }

            if (Math.Abs(vcos) < 1 - float.Epsilon)
            {
                // Standard case (slerp)
                var vsin = (float) Math.Sqrt(1.0f - vcos*vcos);
                var angle = (float) Math.Atan2(vsin, vcos);
                var invSin = (1.0f/vsin);
                var coeff0 = (float) Math.Sin((1.0f - position)*angle)*invSin;
                var coeff1 = (float) Math.Sin((position)*angle)*invSin;

                return coeff0*left + coeff1*_right;
            }
            else
            {
                // There are two situations:
                // 1. "left" and "right" are very close (cos ~= +1), so we can do a linear
                //    interpolation safely.
                // 2. "left" and "right" are almost inverse of each other (cos ~= -1), there
                //    are an infinite number of possibilities interpolation. but we haven't
                //    have method to fix this case, so just use linear interpolation here.

                var ret = (1.0f - position)*left + position*_right;
                // taking the complement requires renormalisation
                ret.Normalise();
                return ret;
            }
        }

        #region Implementation of IEquatable<Quaternion>

        public bool Equals(Quaternion other)
        {
            return Math.Abs(d_w - other.d_w) < float.Epsilon &&
                   Math.Abs(d_x - other.d_x) < float.Epsilon &&
                   Math.Abs(d_y - other.d_y) < float.Epsilon &&
                   Math.Abs(d_z - other.d_z) < float.Epsilon;
        }

        #endregion

        #region Overrides of Object

        public override string ToString()
        {
            return String.Format("w:{0} x:{1} y:{2} z:{3}", d_w, d_x, d_y, d_z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Quaternion && Equals((Quaternion) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = d_w.GetHashCode();
                result = (result*397) ^ d_x.GetHashCode();
                result = (result*397) ^ d_y.GetHashCode();
                result = (result*397) ^ d_z.GetHashCode();
                return result;
            }
        }

        #endregion
    }

    /// <summary>
    /// Special interpolator class for Quaternion
    /// 
    /// Quaternions can't be interpolated as floats and/or vectors, we have to use
    /// "Spherical linear interpolator" instead.
    /// </summary>
    public class QuaternionSlerpInterpolator : Interpolator
    {
        //! \copydoc Interpolator::getType
        public string GetInterpolatorType()
        {
            return "QuaternionSlerp";
        }
    
        //! \copydoc Interpolator::interpolateAbsolute
        public string InterpolateAbsolute(string value1, string value2, float position)
        {
            var val1 = Quaternion.Parse(value1);
            var val2 = Quaternion.Parse(value2);
            
            return PropertyHelper.ToString(Quaternion.Slerp(val1, val2, position));
        }

        //! \copydoc Interpolator::interpolateRelative
        public string InterpolateRelative(string @base, string value1, string value2, float position)
        {
            throw new NotImplementedException();
        }
    
        //! \copydoc Interpolator::interpolateRelativeMultiply
        public string InterpolateRelativeMultiply(string @base, string value1, string value2, float position)
        {
            throw new NotImplementedException();
        }
    }
}