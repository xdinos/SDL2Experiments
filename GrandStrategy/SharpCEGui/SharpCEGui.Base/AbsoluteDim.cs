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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Dimension type that represents an absolute pixel value.
    /// Implements BaseDim interface.
    /// </summary>
    public class AbsoluteDim : BaseDim
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public AbsoluteDim()
            : this(0f)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public AbsoluteDim(float value)
        {
            _value = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the current value of the AbsoluteDim.
        /// </summary>
        /// <returns></returns>
        public float GetBaseValue()
        {
            return _value;
        }

        /// <summary>
        /// Set the current value of the AbsoluteDim.
        /// </summary>
        /// <param name="value"></param>
        public void SetBaseValue(float value)
        {
            _value = value;
        }

        #endregion

        #region Overrides of BaseDim

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        public override float GetValue(Window wnd)
        {
            return _value;
        }

        public override float GetValue(Window wnd, Rectf container)
        {
            return _value;
        }

        public override BaseDim Clone()
        {
            return new AbsoluteDim(_value);
        }

        protected override void WriteXmlElementNameImpl(XMLSerializer xmlStream)
        {
            xmlStream.OpenTag("AbsoluteDim");
        }

        protected override void WriteXmlElementAttributesImpl(XMLSerializer xmlStream)
        {
            xmlStream.Attribute("value", _value.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
        }

        #endregion

        #region Fields

        /// <summary>
        /// holds pixel value for the AbsoluteDim.
        /// </summary>
        private float _value;

        #endregion
    }
}