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
    public struct USize : IEquatable<USize>
    {
        public static readonly USize Zero = new USize(UDim.Zero, UDim.Zero);

        #region Fields

        public UDim d_width;
        public UDim d_height;

        #endregion

        #region Constructors

        public USize(UDim width, UDim height)
        {
            d_width = width;
            d_height = height;
        }

        #endregion

        #region Operators

        public static bool operator ==(USize lhs, USize rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(USize lhs, USize rhs)
        {
            return !(lhs == rhs);
        }

        public static USize operator +(USize lhs, USize rhs)
        {
            return new USize(lhs.d_width + rhs.d_width, lhs.d_height + rhs.d_height);
        }

        public static USize operator *(USize size, float val)
        {
            return new USize(size.d_width*val, size.d_height*val);
        }

        #endregion

        public static USize Parse(string value)
        {
            return Parse(value, CultureInfo.InvariantCulture);
        }

        public static USize Parse(string value, IFormatProvider formatProvider)
        {
            var text = value;
            text = text.Replace(" ", "")
                       .Replace("{", "")
                       .Replace("}", "");
            var values = text.Split(',');

            return new USize(
                    new UDim(Single.Parse(values[0], formatProvider),
                             Single.Parse(values[1], formatProvider)),
                    new UDim(Single.Parse(values[2], formatProvider),
                             Single.Parse(values[3], formatProvider)));

            //var matches = Parser.Matches(value);
            //if (matches.Count == 1)
            //{
            //    return new USize(
            //        UDim.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
            //        UDim.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider));
            //        //new UDim(Single.Parse(),
            //        //         Single.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider)),
            //        //new UDim(Single.Parse(matches[0].Groups[3].Captures[0].Value, formatProvider),
            //        //         Single.Parse(matches[0].Groups[4].Captures[0].Value, formatProvider)));
            //}

            //throw new FormatException();
        }

        #region Implementation of IEquatable<USize>

        public bool Equals(USize other)
        {
            return d_width == other.d_width && d_height == other.d_height;
        }

        #endregion

        #region Overrides of Object

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{{0},{1}}}", d_width, d_height );
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is USize && Equals((USize)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (d_width.GetHashCode() * 397) ^ d_height.GetHashCode();
            }
        }

        #endregion

        private static readonly Regex Parser =
            //new Regex(String.Format("{{{{({0}),({0})}},{{({0}),({0})}}}}", RegExHelper.SingleRegEx),
            //          RegexOptions.Compiled);
            //@"{\s*({[\S|\s]*\s*})\s*,\s*({[\S|\s]*\s*})\s*}"
            new Regex(@"{\s*({.*?})\s*,\s*({.*?})\s*}",
                      RegexOptions.Compiled);

    }
}