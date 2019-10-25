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
    /// Class that holds the size (width & height) of something.
    /// </summary>
    public struct Sizef : IEquatable<Sizef>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Sizef Zero = new Sizef(0f, 0f);

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public float Width;
        /// <summary>
        /// 
        /// </summary>
        public float Height;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Sizef(float width,float height)
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Methods

        public void ScaleToAspect(AspectMode mode, float ratio)
        {
            if (mode == AspectMode.Ignore)
                return;

            if (Width <= 0 && Height <= 0)
                return;

            //assert(ratio > 0);

            float expectedWidth = Height*ratio;
            bool keepHeight = (mode == AspectMode.Shrink)
                                  ? expectedWidth <= Width
                                  : expectedWidth >= Width;

            if (keepHeight)
            {
                Width = expectedWidth;
            }
            else
            {
                Height = Width/ratio;
            }
        }

        public void Clamp(Sizef min, Sizef max)
        {
            global::System.Diagnostics.Debug.Assert(min.Width <= max.Width);
            global::System.Diagnostics.Debug.Assert(min.Height <= max.Height);

            if (Width < min.Width)
                Width = min.Width;
            else if (Width > max.Width)
                Width = max.Width;

            if (Height < min.Height)
                Height = min.Height;
            else if (Height > max.Height)
                Height = max.Height;
        }

        #endregion

        #region Operators

        public static bool operator ==(Sizef lhs, Sizef rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Sizef lhs, Sizef rhs)
        {
            return !(lhs == rhs);
        }

        public static Sizef operator *(Sizef s, float c)
        {
            return new Sizef(s.Width*c, s.Height*c);
        }

        public static Sizef operator +(Sizef left, Sizef right)
        {
            return new Sizef(left.Width + right.Width, left.Height + right.Height);
        }

        public static Sizef operator -(Sizef left, Sizef right)
        {
            return new Sizef(left.Width - right.Width, left.Height - right.Height);
        }

        public static Sizef operator*(Sizef left, Sizef right)
        {
            return new Sizef(left.Width * right.Width, left.Height * right.Height);
        }

        public static Sizef operator *(Sizef s, Lunatics.Mathematics.Vector2 vec)
        {
            return new Sizef(s.Width * vec.X, s.Height * vec.Y);
        }

        #endregion

        public static Sizef Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static Sizef Parse(string value, IFormatProvider formatProvider)
        {
            var matches = Parser.Matches(value);
            if (matches.Count == 1)
            {
                return new Sizef(Single.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
                                 Single.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider));
            }

            throw new FormatException();
        }

        #region Implementation of IEquatable<Size>

        public bool Equals(Sizef other)
        {
            return Math.Abs(Width - other.Width) < float.Epsilon &&
                   Math.Abs(Height - other.Height) < float.Epsilon;
        }

        #endregion

        #region Overrides of Object

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Sizef && Equals((Sizef) obj);
        }

        public override string ToString()
        {
            return String.Format("w:{0} h:{1}", Width, Height);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width.GetHashCode()*397) ^ Height.GetHashCode();
            }
        }

        #endregion

        private static readonly Regex Parser =
            new Regex(String.Format("w:({0}) h:({0})", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
    }
}