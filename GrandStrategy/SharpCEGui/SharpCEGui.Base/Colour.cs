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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Struct representing colour values within the system.
    /// </summary>
    public struct Colour : IEquatable<Colour>
    {
        #region Properties

        public float Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                _argbValid = false;
            }
        }

        public float Red
        {
            get { return _red; }
            set
            {
                _red = value;
                _argbValid = false;
            }
        }

        public float Green
        {
            get { return _green; }
            set
            {
                _green = value;
                _argbValid = false;
            }
        }

        public float Blue
        {
            get { return _blue; }
            set
            {
                _blue = value;
                _argbValid = false;
            }
        }

        #endregion

        #region Constructors

        public Colour(int argb)
            : this((uint) argb)
        {
        }

        public Colour(uint argb)
        {
            _argb = argb;

            _blue = (argb & 0xFF) / 255.0f;
            argb >>= 8;
            _green = (argb & 0xFF) / 255.0f;
            argb >>= 8;
            _red = (argb & 0xFF) / 255.0f;
            argb >>= 8;
            _alpha = (argb & 0xFF) / 255.0f;

            _argbValid = true;
        }

        public Colour(float red, float green, float blue)
            : this(red, green, blue, 1f)
        {

        }

        public Colour(float red, float green, float blue, float alpha)
        {
            _red = red;
            _green = green;
            _blue = blue;
            _alpha = alpha;
            _argb = 0;
            _argbValid = false;
        }

        #endregion

        public float GetHue()
        {
            throw new NotImplementedException();
        }

        public float GetSaturation()
        {
            throw new NotImplementedException();
        }

        public float GetLumination()
        {
            throw new NotImplementedException();
        }

        public int ToAlphaRGB()
        {
            if (!_argbValid)
            {
                _argb = CalculateAlphaRGB();
                _argbValid = true;
            }

            return (int) _argb;
        }
        
        #region Operators

        public static bool operator ==(Colour lhs, Colour rhs)
        {
            if(ReferenceEquals(lhs, null))
                return ReferenceEquals(rhs, null);
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Colour lhs, Colour rhs)
        {
            return !(lhs == rhs);
        }

        public static Colour operator +(Colour lhs, Colour rhs)
        {
            return new Colour(lhs._red + rhs._red,
                              lhs._green + rhs._green,
                              lhs._blue + rhs._blue,
                              lhs._alpha + rhs._alpha);
        }

        public static Colour operator -(Colour lhs, Colour rhs)
        {
            return new Colour(lhs._red - rhs._red,
                              lhs._green - rhs._green,
                              lhs._blue - rhs._blue,
                              lhs._alpha - rhs._alpha);
        }

        public static Colour operator *(Colour colour, float factor)
        {
            return new Colour(colour._red*factor,
                              colour._green*factor,
                              colour._blue*factor,
                              colour._alpha*factor);
        }

        public static Colour operator *(Colour left, Colour right)
        {
            return new Colour(left._red * right._red,
                              left._green * right._green,
                              left._blue * right._blue,
                              left._alpha * right._alpha);
        }

        #endregion

        public static Colour Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static Colour Parse(string value, IFormatProvider formatProvider)
        {
            return new Colour((int)UInt32.Parse(value, NumberStyles.HexNumber));
        }
        
        #region Overrides of Object

        public override string ToString()
        {
            return String.Format("{0:X8}", ToAlphaRGB());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Colour && Equals((Colour)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _red.GetHashCode();
                result = (result * 397) ^ _green.GetHashCode();
                result = (result * 397) ^ _blue.GetHashCode();
                result = (result * 397) ^ _alpha.GetHashCode();
                return result;
            }
        }

        #endregion

        #region Implementation of IEquatable<Colour>

        public bool Equals(Colour other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return _red.Equals(other._red) &&
                   _green.Equals(other._green) &&
                   _blue.Equals(other._blue) &&
                   _alpha.Equals(other._alpha);

        }

        #endregion

        #region Private Methods

        private uint CalculateAlphaRGB()
        {
            uint result = 0;

            result += ((uint)(_alpha * 255.0f)) << 24;
            result += ((uint)(_red * 255.0f)) << 16;
            result += ((uint)(_green * 255.0f)) << 8;
            result += ((uint)(_blue * 255.0f));

            return result;
        }
        #endregion

        #region Fields

        private float _red;
        private float _green;
        private float _blue;
        private float _alpha;
        private uint _argb;
        private bool _argbValid;

        #endregion
    }
}