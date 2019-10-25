#region Copyrights
// Copyright (C) 2012 - 2013 The CEGuiSharp Development Team
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
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using SharpCEGui.Base.Views;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.Base
{

    public static class MathematicsEx
    {
        #region Vector 3
        public static string ToString(Lunatics.Mathematics.Vector3 v)
        {
            return String.Format(CultureInfo.InvariantCulture, "x:{0} y:{1} z:{2}", v.X, v.Y, v.Z);
        }
        public static Lunatics.Mathematics.Vector3 Vector3Parse(string value)
        {
            return Vector3Parse(value, CultureInfo.InvariantCulture);
        }

        public static Lunatics.Mathematics.Vector3 Vector3Parse(string value, IFormatProvider formatProvider)
        {
            var matches = Vector3Parser.Matches(value);
            if (matches.Count == 1)
            {
                return new Lunatics.Mathematics.Vector3(Single.Parse(matches[0].Groups[1].Captures[0].Value, formatProvider),
                                    Single.Parse(matches[0].Groups[2].Captures[0].Value, formatProvider),
                                    Single.Parse(matches[0].Groups[3].Captures[0].Value, formatProvider));
            }

            throw new FormatException();
        }

        private static readonly Regex Vector3Parser =
            new Regex(String.Format("x:({0}) y:({0}) z:({0})", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
        #endregion


        public static string ToString(Lunatics.Mathematics.Quaternion q)
        {
            return String.Format("w:{0} x:{1} y:{2} z:{3}",q.W, q.X, q.Y, q.Z);
        }

        public static Lunatics.Mathematics.Quaternion QuaternionParse(string value)
        {
            return QuaternionParse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static Lunatics.Mathematics.Quaternion QuaternionParse(string value, IFormatProvider formatProvider)
        {
            value = value.ToLowerInvariant().Trim();
            if (value.StartsWith("w"))
            {
                var matches = QuaternionParser.Matches(value);
                if (matches.Count == 1)
                {
                    return new Lunatics.Mathematics.Quaternion(
                        Single.Parse(matches[0].Groups[1].Captures[0].Value, NumberStyles.Any, formatProvider),
                        Single.Parse(matches[0].Groups[2].Captures[0].Value, NumberStyles.Any, formatProvider),
                        Single.Parse(matches[0].Groups[3].Captures[0].Value, NumberStyles.Any, formatProvider),
                        Single.Parse(matches[0].Groups[4].Captures[0].Value, NumberStyles.Any, formatProvider));
                }
            }
            else
            {
                var matches = QuaternionParserAngles.Matches(value);
                if (matches.Count == 1)
                {
                    throw new NotImplementedException();
                    //return FromEulerAnglesDegrees(
                    //    Single.Parse(matches[0].Groups[1].Captures[0].Value, NumberStyles.Any, formatProvider),
                    //    Single.Parse(matches[0].Groups[2].Captures[0].Value, NumberStyles.Any, formatProvider),
                    //    Single.Parse(matches[0].Groups[3].Captures[0].Value, NumberStyles.Any, formatProvider));
                }
            }

            throw new FormatException();
        }

        private static readonly Regex QuaternionParser =
            new Regex(String.Format(@"\s*w:({0})\s*x:\s*({0})\s*y:\s*({0})\s*z:\s*({0})\s*", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
        private static readonly Regex QuaternionParserAngles =
            new Regex(String.Format(@"\s*x:\s*({0})\s*y:\s*({0})\s*z:\s*({0})\s*", RegExHelper.SingleRegEx),
                      RegexOptions.Compiled);
    }
    /// <summary>
    /// 
    /// </summary>
    public static class PropertyHelper
    {
        static PropertyHelper()
        {
            RegisterFunction<Boolean>(
                x => Boolean.Parse(x),
                x => ((Boolean)x).ToString(CultureInfo.InvariantCulture));

            RegisterFunction<Byte>(
                x => Byte.Parse(x, CultureInfo.InvariantCulture), 
                x => ((Byte) x).ToString(CultureInfo.InvariantCulture));

            RegisterFunction<Int16>(
                x => Int16.Parse(x, CultureInfo.InvariantCulture), 
                x => ((Int16)x).ToString(CultureInfo.InvariantCulture));

            RegisterFunction<Int32>(
                x => Int32.Parse(x, CultureInfo.InvariantCulture), 
                x => ((Int32)x).ToString(CultureInfo.InvariantCulture));

            RegisterFunction<Int64>(
                x => Int64.Parse(x, CultureInfo.InvariantCulture),
                x => ((Int64) x).ToString(CultureInfo.InvariantCulture));

            RegisterFunction<UInt16>(
                x => UInt16.Parse(x, CultureInfo.InvariantCulture),
                x => ((UInt16) x).ToString(CultureInfo.InvariantCulture));

            RegisterFunction<UInt32>(
                x => UInt32.Parse(x, CultureInfo.InvariantCulture),
                x => ((UInt32) x).ToString(CultureInfo.InvariantCulture));

            RegisterFunction<UInt64>(
                x => UInt64.Parse(x, CultureInfo.InvariantCulture),
                x => ((UInt64) x).ToString(CultureInfo.InvariantCulture));

            RegisterFunction<Char>(
                x => Char.Parse(x),
                x => ((Char) x).ToString(CultureInfo.InvariantCulture));

            RegisterFunction<Single>(
                x => String.IsNullOrEmpty(x) ? 0f : Single.Parse(x, CultureInfo.InvariantCulture),
                x => ((Single) x).ToString(CultureInfo.InvariantCulture));
            
            RegisterFunction<Double>(
                x => Double.Parse(x, CultureInfo.InvariantCulture), 
                x => ((Double)x).ToString(CultureInfo.InvariantCulture));
            
            RegisterFunction<String>(
                x => x, 
                x => (string)x);

            RegisterFunction<Colour>(
                x => Colour.Parse(x), 
                x => ((Colour) x).ToString());

            RegisterFunction<ColourRect>(
                x => ColourRect.Parse(x, CultureInfo.InvariantCulture),
                x => x.ToString());

            RegisterFunction<Sizef>(
                x => Sizef.Parse(x, CultureInfo.InvariantCulture),
                x => ((Sizef)x).ToString());

            RegisterFunction<Lunatics.Mathematics.Vector3>(
                    x => MathematicsEx.Vector3Parse(x, CultureInfo.InvariantCulture),
                    x => MathematicsEx.ToString((Lunatics.Mathematics.Vector3)x));

            RegisterFunction<Rectf>(
                x => Rectf.Parse(x, CultureInfo.InvariantCulture),
                x => ((Rectf)x).ToString());

            RegisterFunction<Lunatics.Mathematics.Quaternion>(
                    x => MathematicsEx.QuaternionParse(x, CultureInfo.InvariantCulture),
                    x => MathematicsEx.ToString((Lunatics.Mathematics.Quaternion) x));

            RegisterFunction<UDim>(
                x => UDim.Parse(x, CultureInfo.InvariantCulture),
                x => ((UDim)x).ToString());

            RegisterFunction<UVector2>(
                x => UVector2.Parse(x, CultureInfo.InvariantCulture),
                x => ((UVector2) x).ToString());

            RegisterFunction<USize>(
                x => USize.Parse(x, CultureInfo.InvariantCulture),
                x => ((USize)x).ToString());

            RegisterFunction<URect>(
                x=> URect.Parse(x,CultureInfo.InvariantCulture),
                x=> ((URect)x).ToString());

            RegisterFunction<UBox>(
                x => UBox.Parse(x, CultureInfo.InvariantCulture),
                x => ((UBox)x).ToString());

            RegisterFunction<Image>(
                x => String.IsNullOrEmpty(x) ? null : ImageManager.GetSingleton().Get(x),
                x => x != null ? ((Image)x).GetName() : String.Empty);

            RegisterFunction<Font>(
                x => String.IsNullOrEmpty(x) ? null : FontManager.GetSingleton().Get(x),
                x => x != null ? ((Font)x).GetName() : String.Empty);

            RegisterFunction<FontMetricType>(
                x => (FontMetricType)Enum.Parse(typeof(FontMetricType), x, true),
                x => ((FontMetricType)x).ToString());

            RegisterFunction<AspectMode>(
                x => (AspectMode)Enum.Parse(typeof(AspectMode), x, true),
                x => ((AspectMode)x).ToString());

            RegisterFunction<HorizontalAlignment>(
                x =>
                    {
                        HorizontalAlignment value;
                        if (!Enum.TryParse(x, true, out value))
                        {
                            switch (x.ToLowerInvariant())
                            {
                                case "leftaligned":
                                    return HorizontalAlignment.Left;
                                case "centrealigned":
                                    return HorizontalAlignment.Centre;
                                case "rightaligned":
                                    return HorizontalAlignment.Right;
                            }
                        }

                        return value;
                    },
                x => ((HorizontalAlignment)x).ToString());

            RegisterFunction<VerticalAlignment>(
                x =>
                    {
                        VerticalAlignment value;
                        if (!Enum.TryParse(x, true, out value))
                        {
                            switch (x.ToLowerInvariant())
                            {
                                case "topaligned":
                                    return VerticalAlignment.Top;
                                case "centrealigned":
                                    return VerticalAlignment.Centre;
                                case "bottomaligned":
                                    return VerticalAlignment.Bottom;
                            }
                        }

                        return value;
                    },
                x => ((VerticalAlignment)x).ToString());
            
            RegisterFunction<VerticalTextFormatting>(
                x => (VerticalTextFormatting) Enum.Parse(typeof (VerticalTextFormatting), x, true),
                x => ((VerticalTextFormatting) x).ToString());

            RegisterFunction<HorizontalTextFormatting>(
                x => (HorizontalTextFormatting)Enum.Parse(typeof(HorizontalTextFormatting), x, true),
                x => ((HorizontalTextFormatting)x).ToString());

            RegisterFunction<AutoScaledMode>(
                x =>
                    {
                        AutoScaledMode value;
                        if (!Enum.TryParse(x, true, out value))
                        {
                            if (x.ToLowerInvariant() == "false")
                                return AutoScaledMode.Disabled;
                        }
                        return value;
                    },
                x => ((AutoScaledMode) x).ToString());

            RegisterFunction<VerticalFormatting>(
                x => (VerticalFormatting)Enum.Parse(typeof(VerticalFormatting), x, true),
                x => ((VerticalFormatting)x).ToString());

            RegisterFunction<HorizontalFormatting>(
                x => (HorizontalFormatting)Enum.Parse(typeof(HorizontalFormatting), x, true),
                x => ((HorizontalFormatting)x).ToString());

            RegisterFunction<FrameImageComponent>(
                x => (FrameImageComponent)Enum.Parse(typeof(FrameImageComponent), x, true),
                x => ((FrameImageComponent)x).ToString());

            RegisterFunction<DimensionType>(
                x => (DimensionType) Enum.Parse(typeof (DimensionType), x, true),
                x => ((DimensionType)x).ToString());

            RegisterFunction<DimensionOperator>(
                x => (DimensionOperator)Enum.Parse(typeof(DimensionOperator), x, true),
                x => ((DimensionOperator)x).ToString());

            RegisterFunction<WindowUpdateMode>(
                x => (WindowUpdateMode)Enum.Parse(typeof(WindowUpdateMode), x, true),
                x => ((WindowUpdateMode)x).ToString());

            RegisterFunction<ScrollbarDisplayMode>(
                x => (ScrollbarDisplayMode)Enum.Parse(typeof(ScrollbarDisplayMode), x, true),
                x => ((ScrollbarDisplayMode)x).ToString());

            RegisterFunction<ViewSortMode>(
                x => (ViewSortMode)Enum.Parse(typeof(ViewSortMode), x, true),
                x => ((ViewSortMode)x).ToString());

            RegisterFunction<Tuple<float, float>>(
                x => { throw new NotImplementedException(); },
                x => String.Format("min:{0} max:{1}", ((Tuple<float, float>) x).Item1, ((Tuple<float, float>) x).Item1));

            #region Types from Widgets

            RegisterFunction<ListHeaderSegment.SortDirection>(
                x => (ListHeaderSegment.SortDirection)Enum.Parse(typeof(ListHeaderSegment.SortDirection), x, true),
                x => ((ListHeaderSegment.SortDirection)x).ToString());

            RegisterFunction<MultiColumnList.SelectionMode>(
                x => (MultiColumnList.SelectionMode)Enum.Parse(typeof(MultiColumnList.SelectionMode), x, true),
                x => ((MultiColumnList.SelectionMode)x).ToString());

            RegisterFunction<ItemListBase.SortMode>(
                x => (ItemListBase.SortMode)Enum.Parse(typeof(ItemListBase.SortMode), x, true),
                x => ((ItemListBase.SortMode)x).ToString());

            RegisterFunction<Spinner.TextInputMode>(
                x => (Spinner.TextInputMode)Enum.Parse(typeof(Spinner.TextInputMode), x, true),
                x => ((Spinner.TextInputMode)x).ToString());

            RegisterFunction<GridLayoutContainer.AutoPositioning>(
                x => (GridLayoutContainer.AutoPositioning)Enum.Parse(typeof(GridLayoutContainer.AutoPositioning), x, true),
                x => ((GridLayoutContainer.AutoPositioning)x).ToString());

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ToString<T>(T value)
        {
            return ParseFuncs[typeof(T)].Item2(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FromString<T>(string value)
        {
            return (T) ParseFuncs[typeof (T)].Item1(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromString"></param>
        /// <param name="toString"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterFunction<T>(Func<string, object> fromString, Func<object, string> toString)
        {
            ParseFuncs.Add(typeof(T), new Tuple<Func<string, object>, Func<object, string>>(fromString, toString));
        }

        private static readonly Dictionary<Type, Tuple<Func<string, object>, Func<object, string>>> ParseFuncs =
            new Dictionary<Type, Tuple<Func<string, object>, Func<object, string>>>();
    }
}