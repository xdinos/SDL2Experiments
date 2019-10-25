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
using System.Collections.Generic;
using Lunatics.Mathematics;

namespace SharpCEGui.Base
{
    /// <summary>
    /// String component that draws an image.
    /// </summary>
    public class RenderedStringTextComponent : RenderedStringComponent
    {
        public RenderedStringTextComponent()
            : this(null, (Font) null)
        {
        }

        public RenderedStringTextComponent(string text) 
            : this(text, (Font)null)
        {
        }

        public RenderedStringTextComponent(string text, string fontName)
            : this(text, String.IsNullOrEmpty(fontName) ? null : FontManager.GetSingleton().Get(fontName))
        {
        }

        public RenderedStringTextComponent(string text, Font font)
        {
            d_text = text;
            d_font = font;
            d_colours = new ColourRect(0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF);
            d_selectionStart = 0;
            d_selectionLength = 0;
        }

        /// <summary>
        /// Set the text to be rendered by this component.
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            d_text = text;
        }

        /// <summary>
        /// return the text that will be rendered by this component
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return d_text;
        }

        /// <summary>
        /// set the font to use when rendering the text.
        /// </summary>
        /// <param name="font"></param>
        public void SetFont(Font font)
        {
            d_font = font;
        }

        /// <summary>
        /// set the font to use when rendering the text.
        /// </summary>
        /// <param name="fontName"></param>
        public void SetFont(string fontName)
        {
            d_font = String.IsNullOrEmpty(fontName)
                             ? null
                             : FontManager.GetSingleton().Get(fontName);
        }

        /// <summary>
        /// return the font set to be used.  If 0 the default font will be used.
        /// </summary>
        /// <returns></returns>
        public Font GetFont()
        {
            return d_font;
        }

        /// <summary>
        /// Set the colour values used when rendering this component.
        /// </summary>
        /// <param name="cr"></param>
        public void SetColours(ColourRect cr)
        {
            d_colours = cr;
        }

        /// <summary>
        /// Set the colour values used when rendering this component.
        /// </summary>
        /// <param name="c"></param>
        public void SetColours(Colour c)
        {
            d_colours.SetColours(c);
        }

        //! return the ColourRect object used when drawing this component.
        public ColourRect GetColours()
        {
            throw new NotImplementedException();
        }

        #region Overrides of RenderedStringComponent

        public override List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Vector2 position, ColourRect modColours, Rectf? clipRect, float verticalSpace, float spaceExtra)
        {
            var fnt = GetEffectiveFont(refWnd);

            if (fnt == null)
                return new List<GeometryBuffer>();

            var finalPos = position;
            var yScale = 1.0f;

            // handle formatting options
            switch (d_verticalFormatting)
            {
                case VerticalFormatting.BottomAligned:
                    finalPos.Y += verticalSpace - GetPixelSize(refWnd).Height;
                    break;

                case VerticalFormatting.CentreAligned:
                    finalPos.Y += (verticalSpace - GetPixelSize(refWnd).Height)/2;
                    break;

                case VerticalFormatting.Stretched:
                    yScale = verticalSpace/GetPixelSize(refWnd).Height;
                    break;

                case VerticalFormatting.TopAligned:
                    // nothing additional to do for this formatting option.
                    break;

                default:
                    throw new InvalidRequestException("unknown VerticalFormatting option specified.");
            }

            // apply padding to position:
            finalPos += d_padding.Position;

            // apply modulative colours if needed.
            var finalCols = d_colours;
            if (modColours != null)
                finalCols *= modColours;

            // render selection
            if (d_selectionImage != null && d_selectionLength != 0)
            {
                float selStartExtent = 0f, selEndExtent = 0f;

                if (d_selectionStart > 0)
                    selStartExtent = fnt.GetTextExtent(d_text.CEGuiSubstring(0, d_selectionStart));

                selEndExtent = fnt.GetTextExtent(d_text.CEGuiSubstring(0, d_selectionStart + d_selectionLength));

                var selRect = new Rectf(position.X + selStartExtent,
                                         position.Y,
                                         position.X + selEndExtent,
                                         position.Y + verticalSpace);
                var imgRenderSetting = new ImageRenderSettings(selRect, clipRect, true, new ColourRect(0xFF002FFF));
                d_selectionImage.CreateRenderGeometry(imgRenderSetting);
            }

            // draw the text string.
            return fnt.CreateRenderGeometryForText(d_text, finalPos, clipRect, true, finalCols, spaceExtra, 1.0f, yScale);
        }

        public override Sizef GetPixelSize(Window refWnd)
        {
            var fnt = GetEffectiveFont(refWnd);

            var psz = new Sizef(d_padding.d_min.X + d_padding.d_max.X,
                                d_padding.d_min.Y + d_padding.d_max.Y);

            if (fnt != null)
            {
                psz.Width += fnt.GetTextExtent(d_text);
                psz.Height += fnt.GetFontHeight();
            }

            return psz;
        }

        public override bool CanSplit()
        {
            return d_text.Length > 1;
        }

        public override RenderedStringComponent Split(Window refWnd, float splitPoint, bool firstComponent)
        {
            var fnt = GetEffectiveFont(refWnd);

            // This is checked, but should never fail, since if we had no font our
            // extent would be 0 and we would never cause a split to be needed here.
            if (fnt == null)
                throw new InvalidRequestException("unable to split with no font set.");

            // create 'left' side of split and clone our basic configuration
            var lhs = new RenderedStringTextComponent();
            lhs.d_padding = d_padding;
            lhs.d_verticalFormatting = d_verticalFormatting;
            lhs.d_font = d_font;
            lhs.d_colours = d_colours;

            // calculate the 'best' place to split the text
            var leftLen = 0;
            var leftExtent = 0.0f;

            while (leftLen < d_text.Length)
            {
                var tokenLen = GetNextTokenLength(d_text, leftLen);
                // exit loop if no more valid tokens.
                if (tokenLen == 0)
                    break;

                var tokenExtent = fnt.GetTextExtent(d_text.CEGuiSubstring(leftLen, tokenLen));

                // does the next token extend past the split point?
                if (leftExtent + tokenExtent > splitPoint)
                {
                    // if it was the first token, split the token itself
                    if (firstComponent && leftLen == 0)
                        leftLen =
                            Math.Max(1,
                                     fnt.GetCharAtPixel(d_text.CEGuiSubstring(0, tokenLen), splitPoint));
            
                    // left_len is now the character index at which to split the line
                    break;
                }
        
                // add this token to the left side
                leftLen += tokenLen;
                leftExtent += tokenExtent;
            }
    
            // perform the split.
            lhs.d_text = d_text.CEGuiSubstring(0, leftLen);

            // here we're trimming leading delimiters from the substring range 
            var rhsStart = d_text.IndexNotOf(TextUtils.DefaultWrapDelimiters, leftLen);
            if (rhsStart == -1)
                rhsStart = leftLen;

            // split the selection
            if (d_selectionLength!=0)
            {
                var selEnd = d_selectionStart + d_selectionLength - 1;
                lhs.d_selectionStart = d_selectionStart;
                lhs.d_selectionLength = selEnd < leftLen ? d_selectionLength : leftLen - d_selectionStart;

                if (selEnd >= leftLen)
                {
                    d_selectionStart = 0;
                    d_selectionLength -= rhsStart;
                }
                else
                    SetSelection(refWnd, 0, 0);
            }

            d_text = d_text.Substring(rhsStart);

            return lhs;
        }

        public override RenderedStringComponent Clone()
        {
            return new RenderedStringTextComponent
                       {
                           d_aspectLock = d_aspectLock,
                           d_colours = d_colours,
                           d_font = d_font,
                           d_padding = d_padding,
                           d_selectionImage = d_selectionImage,
                           d_selectionLength = d_selectionLength,
                           d_selectionStart = d_selectionStart,
                           d_text = d_text,
                           d_verticalFormatting = d_verticalFormatting
                       };
        }

        public override int GetSpaceCount()
        {
            throw new global::System.NotImplementedException();
        }

        public override void SetSelection(Window refWnd, float start, float end)
        {
            if (start >= end)
            {
                d_selectionStart = d_selectionLength = 0;
                return;
            }

            var fnt = GetEffectiveFont(refWnd);

            d_selectionStart = fnt.GetCharAtPixel(d_text, start);
            d_selectionLength = fnt.GetCharAtPixel(d_text, end) - d_selectionStart;
        }

        #endregion

        protected Font GetEffectiveFont(Window window)
        {
            if (d_font != null)
                return d_font;

            return window != null ? window.GetFont() : null;
        }

        protected static int GetNextTokenLength(string text, int startIdx)
        {
            var wordStart = text.IndexNotOf(TextUtils.DefaultWrapDelimiters, startIdx);

            if (wordStart == -1)
                wordStart = startIdx;

            var wordEnd = text.IndexOfAny(TextUtils.DefaultWrapDelimiters.ToCharArray(), wordStart);

            if (wordEnd == -1)
                wordEnd = text.Length;

            return wordEnd - startIdx;
        }

        /// <summary>
        /// pointer to the image drawn by the component.
        /// </summary>
        protected string d_text;
        
        /// <summary>
        /// Font to use for text rendering, 0 for system default.
        /// </summary>
        protected Font d_font;

        /// <summary>
        /// ColourRect object describing the colours to use when rendering.
        /// </summary>
        protected ColourRect d_colours;
        
        /// <summary>
        /// last set selection
        /// </summary>
        protected int d_selectionStart;
        protected int d_selectionLength;
    }
}