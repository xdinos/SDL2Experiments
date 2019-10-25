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
    public struct Rectf : IEquatable<Rectf>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Rectf Zero = new Rectf(Lunatics.Mathematics.Vector2.Zero, Lunatics.Mathematics.Vector2.Zero);

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public Lunatics.Mathematics.Vector2 d_min;

        /// <summary>
        /// 
        /// </summary>
        public Lunatics.Mathematics.Vector2 d_max;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float Left
        {
            get { return d_min.X; }
            set { d_min.X = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Top
        {
            get { return d_min.Y; }
            set { d_min.Y = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Right
        {
            get { return d_max.X; }
            set { d_max.X = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Bottom
        {
            get { return d_max.Y; }
            set { d_max.Y = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Width
        {
            get { return d_max.X - d_min.X; }
            set { d_max.X = d_min.X + value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Height
        {
            get { return d_max.Y - d_min.Y; }
            set { d_max.Y = d_min.Y + value; }
        }

        /// <summary>
        /// Get/Set the top-left position of Rectangle as a Vector2 
        /// (leaves size in tact)
        /// </summary>
        public Lunatics.Mathematics.Vector2 Position
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
        public Sizef Size
        {
            get { return new Sizef(Width, Height); }
            set { d_max = d_min + new Lunatics.Mathematics.Vector2(value.Width, value.Height); }
        }

        #endregion

        #region Constructors

        public Rectf(float left, float top, float right, float bottom)
            : this(new Lunatics.Mathematics.Vector2(left, top), new Lunatics.Mathematics.Vector2(right, bottom))
        {
        }

        public Rectf(Lunatics.Mathematics.Vector2 pos, Sizef size)
            : this(pos, new Lunatics.Mathematics.Vector2(pos.X + size.Width, pos.Y + size.Height))
        {

        }

        public Rectf(Lunatics.Mathematics.Vector2 min, Lunatics.Mathematics.Vector2 max)
        {
            d_min = min;
            d_max = max;
        }

        #endregion

        #region Methods

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
        public Rectf GetIntersection(Rectf other)
        {
            if ((d_max.X <= other.d_min.X) ||
                (d_min.X >= other.d_max.X) ||
                (d_max.Y <= other.d_min.Y) ||
                (d_min.Y >= other.d_max.Y))
            {
                return new Rectf(0.0f, 0.0f, 0.0f, 0.0f);
            }

            return new Rectf(
                (d_min.X > other.d_min.X) ? d_min.X : other.d_min.X,
                (d_min.Y > other.d_min.Y) ? d_min.Y : other.d_min.Y,
                (d_max.X < other.d_max.X) ? d_max.X : other.d_max.X,
                (d_max.Y < other.d_max.Y) ? d_max.Y : other.d_max.Y);
        }

        /// <summary>
        /// Applies an offset the Rectangle object
        /// </summary>
        /// <param name="v">
        /// Vector2 object containing the offsets to be applied to the Rectangle.
        /// </param>
        public void Offset(Lunatics.Mathematics.Vector2 v)
        {
            d_min += v;
            d_max += v;
        }

        /// <summary>
        /// Return true if the given <see cref="Vector2f"/> falls within this <see cref="Rectf"/>
        /// </summary>
        /// <param name="v">
        /// <see cref="Vector2f"/> object describing the position to test.
        /// </param>
        /// <returns>
        /// true if position \a pt is within this <see cref="Rectf"/>'s area, else false
        /// </returns>
        public bool IsPointInRect(Lunatics.Mathematics.Vector2 v)
        {
            if ((d_min.X > v.X) ||
                (d_max.X <= v.X) ||
                (d_min.Y > v.Y) ||
                (d_max.Y <= v.Y))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Operators

        public static bool operator ==(Rectf lhs, Rectf rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Rectf lhs, Rectf rhs)
        {
            return !(lhs == rhs);
        }

        public static Rectf operator +(Rectf lhs, Rectf rhs)
        {
            return new Rectf(lhs.d_min + rhs.d_min, lhs.d_max + rhs.d_max);
        }

        public static Rectf operator *(Rectf rect, float value)
        {
            return new Rectf(rect.d_min*value, rect.d_max*value);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Rectf Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static Rectf Parse(string value, IFormatProvider formatProvider)
        {
            var matches = Parser.Matches(value);
            if (matches.Count == 1)
            {
                return new Rectf(Single.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
                                 Single.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider),
                                 Single.Parse(matches[0].Groups[3].Captures[0].Value, formatProvider),
                                 Single.Parse(matches[0].Groups[4].Captures[0].Value, formatProvider));
            }

            throw new FormatException();
        }

        #region Implementation of IEquatable<Rectangle>

        public bool Equals(Rectf other)
        {
            return d_min == other.d_min &&
                   d_max == other.d_max;
        }

        #endregion

        #region Overrides of Object

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "l:{0} t:{1} r:{2} b:{3}", d_min.X, d_min.Y, d_max.X, d_max.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Rectf && Equals((Rectf) obj);
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
           new Regex(String.Format("l:({0}) t:({0}) r:({0}) b:({0})", RegExHelper.SingleRegEx),
                     RegexOptions.Compiled);
    }
}