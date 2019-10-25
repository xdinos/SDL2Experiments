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
using SharpCEGui.Base;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// MultiLineEditbox class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled    - Rendering for when the editbox is in enabled and is in read-write mode.
    ///     - ReadOnly  - Rendering for when the editbox is in enabled and is in read-only mode.
    ///     - Disabled  - Rendering for when the editbox is disabled.
    /// 
    /// NamedAreas:
    ///     TextArea         - area where text, selection, and caret imagery will appear.
    ///     TextAreaHScroll  - TextArea when only horizontal scrollbar is visible.
    ///     TextAreaVScroll  - TextArea when only vertical scrollbar is visible.
    ///     TextAreaHVScroll - TextArea when both horizontal and vertical scrollbar is visible.
    /// 
    /// PropertyDefinitions (optional, defaults will be black):
    ///     - NormalTextColour        - property that accesses a colour value to be used to render normal unselected text.
    ///     - SelectedTextColour      - property that accesses a colour value to be used to render selected text.
    ///     - ActiveSelectionColour   - property that accesses a colour value to be used to render active selection highlight.
    ///     - InactiveSelectionColour - property that accesses a colour value to be used to render inactive selection highlight.
    /// 
    /// Imagery Sections:
    ///     - Caret
    /// 
    /// Child Widgets:
    ///     Scrollbar based widget with name suffix "__auto_vscrollbar__"
    ///     Scrollbar based widget with name suffix "__auto_hscrollbar__"
    /// </summary>
    public class FalagardMultiLineEditbox : MultiLineEditboxWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/MultiLineEditbox";

        /// <summary>
        /// Name of property to use to obtain unselected text rendering colour.
        /// </summary>
        public const string UnselectedTextColourPropertyName = "NormalTextColour";

        /// <summary>
        /// Name of property to use to obtain selected text rendering colour.
        /// </summary>
        public const string SelectedTextColourPropertyName = "SelectedTextColour";

        /// <summary>
        /// Name of property to use to obtain active selection rendering colour.
        /// </summary>
        public const string ActiveSelectionColourPropertyName = "ActiveSelectionColour";

        /// <summary>
        /// Name of property to use to obtain inactive selection rendering colour.
        /// </summary>
        public const string InactiveSelectionColourPropertyName = "InactiveSelectionColour";

        /// <summary>
        /// The default timeout (in seconds) used when blinking the caret.
        /// </summary>
        public const float DefaultCaretBlinkTimeout = 0.66f;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardMultiLineEditbox(string type):base(type)
        {
            _blinkCaret = false;
            _caretBlinkTimeout=DefaultCaretBlinkTimeout;
            _caretBlinkElapsed=0.0f;
            _showCaret = true;
        }

        // overridden from base classes.
        public override Rectf GetTextRenderArea()
        {
            var w = (MultiLineEditbox)Window;
            var wlf = GetLookNFeel();
            var vertVisible = w.GetVertScrollbar().IsVisible();
            var horzVisible = w.GetHorzScrollbar().IsVisible();

            // if either of the scrollbars are visible, we might want to use another text rendering area
            if (vertVisible || horzVisible)
            {
                var areaName = "TextArea";

                if (horzVisible)
                {
                    areaName += "H";
                }
                if (vertVisible)
                {
                    areaName += "V";
                }
                areaName += "Scroll";

                if (wlf.IsNamedAreaPresent(areaName))
                {
                    return wlf.GetNamedArea(areaName).GetArea().GetPixelRect(w);
                }
            }

            // default to plain TextArea
            return wlf.GetNamedArea("TextArea").GetArea().GetPixelRect(w);
        }

        public override void CreateRenderGeometry()
        {
            var w = (MultiLineEditbox) Window;

            // Create the render geometry for the general frame and stuff before we handle the text itself
            CacheEditboxBaseImagery();

            // Create the render geometry for the edit box text
            var textarea = GetTextRenderArea();
            CacheTextLines(textarea);

            // Create the render geometry for the caret
            if ((w.HasInputFocus() && !w.IsReadOnly()) && (!_blinkCaret || _showCaret))
                CacheCaretImagery(textarea);
        }

        public override void Update(float elapsed)
        {
            // do base class stuff
            base.Update(elapsed);

            // only do the update if we absolutely have to
            if (_blinkCaret &&
                !((MultiLineEditbox)Window).IsReadOnly() &&
                ((MultiLineEditbox)Window).HasInputFocus())
            {
                _caretBlinkElapsed += elapsed;

                if (_caretBlinkElapsed > _caretBlinkTimeout)
                {
                    _caretBlinkElapsed = 0.0f;
                    _showCaret ^= true;
                    // state changed, so need a redraw
                    Window.Invalidate(false);
                }
            }
        }

        /// <summary>
        /// return whether the blinking caret is enabled.
        /// </summary>
        /// <returns></returns>
        public bool IsCaretBlinkEnabled()
        {
            return _blinkCaret;
        }

        /// <summary>
        /// return the caret blink timeout period (only used if blink is enabled).
        /// </summary>
        /// <returns></returns>
        public float GetCaretBlinkTimeout()
        {
            return _caretBlinkTimeout;
        }
        
        /// <summary>
        /// set whether the blinking caret is enabled.
        /// </summary>
        /// <param name="enable"></param>
        public void SetCaretBlinkEnabled(bool enable)
        {
            _blinkCaret = enable;
        }

        /// <summary>
        /// set the caret blink timeout period (only used if blink is enabled).
        /// </summary>
        /// <param name="seconds"></param>
        public void SetCaretBlinkTimeout(float seconds)
        {
            _caretBlinkTimeout = seconds;
        }

        // overridden from base class
        public override bool HandleFontRenderSizeChange(Font font)
        {
            var res = base.HandleFontRenderSizeChange(font);

            if (Window.GetFont() == font)
            {
                Window.Invalidate(false);
                ((MultiLineEditbox) Window).FormatText(true);
                return true;
            }

            return res;
        }

        /// <summary>
        /// Perform rendering of the widget control frame and other 'static' areas.  This
        /// method should not render the actual text.  Note that the text will be rendered
        /// to layer 4 and the selection brush to layer 3, other layers can be used for
        /// rendering imagery behind and infront of the text  selection..
        /// </summary>
        protected void CacheEditboxBaseImagery()
        {
            var w = (MultiLineEditbox)Window;

            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

            // try and get imagery for our current state
            StateImagery imagery = wlf.GetStateImagery(w.IsEffectiveDisabled()
                                                           ? "Disabled"
                                                           : (w.IsReadOnly() ? "ReadOnly" : "Enabled"));

            // Create the render geometry for the imagery
            imagery.Render(w);
        }

        /// <summary>
        /// Render the caret.
        /// </summary>
        /// <param name="textArea"></param>
        protected void CacheCaretImagery(Rectf textArea)
        {
            var w = (MultiLineEditbox)Window;
            var fnt = w.GetFont();

            // require a font so that we can calculate caret position.
            if (fnt!=null)
            {
                // get line that caret is in
                var caretLine = w.GetLineNumberFromIndex(w.GetCaretIndex());

                var lines = w.GetFormattedLines();

                // if caret line is valid.
                if (caretLine < lines.Count)
                {
                    // calculate pixel offsets to where caret should be drawn
                    var caretLineIdx = w.GetCaretIndex() - lines[caretLine].d_startIdx;
                    var ypos = caretLine * fnt.GetLineSpacing();
                    var xpos = fnt.GetTextAdvance(w.GetText().CEGuiSubstring(lines[caretLine].d_startIdx, caretLineIdx));

                     //// get base offset to target layer for cursor.
                     //Renderer* renderer = System::getSingleton().getRenderer();
                     //float baseZ = renderer->getZLayer(7) - renderer->getCurrentZ();

                    // get WidgetLookFeel for the assigned look.
                    var wlf = GetLookNFeel();
                    // get caret imagery
                    var caretImagery = wlf.GetImagerySection("Caret");

                    // calculate finat destination area for caret
                    var caretArea=new Rectf
                                      {
                                          Left = textArea.Left + xpos,
                                          Top = textArea.Top + ypos,
                                          Width = caretImagery.GetBoundingRect(w).Size.Width,
                                          Height = fnt.GetLineSpacing()
                                      };
                    caretArea.Offset(new Lunatics.Mathematics.Vector2(-w.GetHorzScrollbar().GetScrollPosition(),
                                                                     -w.GetVertScrollbar().GetScrollPosition()));

                    // Create the render geometry for the caret image
                    caretImagery.Render(w, caretArea, null, textArea);
                }
            }
        }

        /// <summary>
        /// Render text lines.
        /// </summary>
        /// <param name="destArea"></param>
        protected void CacheTextLines(Rectf destArea)
        {
            var w = (MultiLineEditbox) Window;

            // text is already formatted, we just grab the lines and
            // create the render geometry for them with the required alignment.
            var drawArea = destArea;
            var vertScrollPos = w.GetVertScrollbar().GetScrollPosition();
            drawArea.Offset(new Lunatics.Mathematics.Vector2(-w.GetHorzScrollbar().GetScrollPosition(), -vertScrollPos));

            var fnt = w.GetFont();

            if (fnt != null)
            {
                // calculate final colours to use.
                var alpha = w.GetEffectiveAlpha();
                var normalTextCol = new ColourRect();
                SetColourRectToUnselectedTextColour(ref normalTextCol);
                normalTextCol.ModulateAlpha(alpha);
                var selectTextCol = new ColourRect();
                SetColourRectToSelectedTextColour(ref selectTextCol);
                selectTextCol.ModulateAlpha(alpha);
                var selectBrushCol = new ColourRect();
                if (w.HasInputFocus())
                    SetColourRectToActiveSelectionColour(ref selectBrushCol);
                else
                    SetColourRectToInactiveSelectionColour(ref selectBrushCol);

                selectBrushCol.ModulateAlpha(alpha);

                var lines = w.GetFormattedLines();
                var numLines = lines.Count;

                // calculate the range of visible lines
                var sidx = (int) (vertScrollPos/fnt.GetLineSpacing());
                var eidx = 1 + sidx + (int) (destArea.Height/fnt.GetLineSpacing());
                eidx = Math.Min(eidx, numLines);
                drawArea.d_min.Y += fnt.GetLineSpacing()*sidx;

                // for each formatted line.
                for (var i = sidx; i < eidx; ++i)
                {
                    var lineRect = drawArea;
                    var currLine = lines[i];
                    var lineText = w.GetTextVisual().CEGuiSubstring(currLine.d_startIdx, currLine.d_length);

                    // offset the font little down so that it's centered within its own spacing
                    var oldTop = lineRect.Top;
                    lineRect.d_min.Y += (fnt.GetLineSpacing() - fnt.GetFontHeight())*0.5f;

                    // if it is a simple 'no selection area' case
                    ColourRect colours;
                    if ((currLine.d_startIdx >= w.GetSelectionEndIndex()) ||
                        ((currLine.d_startIdx + currLine.d_length) <= w.GetSelectionStartIndex()) ||
                        (w.GetSelectionBrushImage() == null))
                    {
                        colours = normalTextCol;
                        
                        // Create Geometry buffers for the text and add to the Window
                        var nextGlyphPos = 0.0f;
                        var textGeomBuffers = fnt.CreateRenderGeometryForText(lineText, out nextGlyphPos, lineRect.Position, destArea, true, colours);
                        w.AppendGeometryBuffers(textGeomBuffers);
                    }
                    else // we have at least some selection highlighting to do
                    {
                        // Start of actual rendering section.
                        String sect;
                        int sectIdx = 0, sectLen;
                        float selStartOffset = 0.0f;

                        // Create the render geometry for any text prior to selected region of line.
                        if (currLine.d_startIdx < w.GetSelectionStartIndex())
                        {
                            // calculate length of text section
                            sectLen = w.GetSelectionStartIndex() - currLine.d_startIdx;

                            // get text for this section
                            sect = lineText.CEGuiSubstring(sectIdx, sectLen);
                            sectIdx += sectLen;

                            // get the pixel offset to the beginning of the selection area highlight.
                            selStartOffset = fnt.GetTextAdvance(sect);

                            // Create the render geometry for this portion of the text
                            colours = normalTextCol;
                            var geomBuffers = fnt.CreateRenderGeometryForText(sect, lineRect.Position, destArea, true, colours);

                            // set position ready for next portion of text
                            lineRect.d_min.X += selStartOffset;
                        }

                        // calculate the length of the selected section
                        sectLen = Math.Min(w.GetSelectionEndIndex() - currLine.d_startIdx, currLine.d_length) - sectIdx;

                        // get the text for this section
                        sect = lineText.CEGuiSubstring(sectIdx, sectLen);
                        sectIdx += sectLen;

                        // get the extent to use as the width of the selection area highlight
                        var selAreaWidth = fnt.GetTextAdvance(sect);

                        var textTop = lineRect.Top;
                        lineRect.Top = oldTop;

                        // calculate area for the selection brush on this line
                        lineRect.Left = drawArea.Left + selStartOffset;
                        lineRect.Right = lineRect.Left + selAreaWidth;
                        lineRect.Bottom = lineRect.Top + fnt.GetLineSpacing();

                        // Create the render geometry for the selection area brush for this line
                        colours = selectBrushCol;
                        var renderSettings = new ImageRenderSettings(lineRect, destArea, true, colours);
                        var selectionGeomBuffers = w.GetSelectionBrushImage().CreateRenderGeometry(renderSettings);
                        w.AppendGeometryBuffers(selectionGeomBuffers);

                        // Create the render geometry for the text for this section
                        colours = selectTextCol;
                        var textGeomBuffers = fnt.CreateRenderGeometryForText(sect, lineRect.Position, destArea, true, colours);
                        w.AppendGeometryBuffers(textGeomBuffers);

                        lineRect.Top = textTop;

                        // Create the render geometry for any text beyond selected region of line
                        if (sectIdx < currLine.d_length)
                        {
                            // update render position to the end of the selected area.
                            lineRect.d_min.X += selAreaWidth;

                            // calculate length of this section
                            sectLen = currLine.d_length - sectIdx;

                            // get the text for this section
                            sect = lineText.CEGuiSubstring(sectIdx, sectLen);

                            // render the text for this section.
                            colours = normalTextCol;

                            var textAfterSelectionGeomBuffers = fnt.CreateRenderGeometryForText(sect, lineRect.Position, destArea, true, colours);
                            w.AppendGeometryBuffers(textAfterSelectionGeomBuffers);
                        }
                    }

                    // update master position for next line in paragraph.
                    drawArea.d_min.Y += fnt.GetLineSpacing();
                }
            }
        }

        /// <summary>
        /// Set the given ColourRect to the colour to be used for rendering Editbox
        /// text oustside of the selected region.
        /// </summary>
        /// <param name="colourRect"></param>
        protected void SetColourRectToUnselectedTextColour(ref ColourRect colourRect)
        {
            SetColourRectToOptionalPropertyColour(UnselectedTextColourPropertyName, ref colourRect);
        }

        /// <summary>
        /// Set the given ColourRect to the colour to be used for rendering Editbox
        /// text falling within the selected region.
        /// </summary>
        /// <param name="colourRect"></param>
        protected void SetColourRectToSelectedTextColour(ref ColourRect colourRect)
        {
            SetColourRectToOptionalPropertyColour(SelectedTextColourPropertyName, ref colourRect);
        }

        /// <summary>
        /// Set the given ColouRect to the colours to be used for rendering the
        /// selection highlight when the editbox is active.
        /// </summary>
        /// <param name="colourRect"></param>
        protected void SetColourRectToActiveSelectionColour(ref ColourRect colourRect)
        {
            SetColourRectToOptionalPropertyColour(ActiveSelectionColourPropertyName, ref colourRect);
        }

        /// <summary>
        /// set the given ColourRect to the colours to be used for rendering the
        /// selection highlight when the editbox is inactive.
        /// </summary>
        /// <param name="colourRect"></param>
        protected void SetColourRectToInactiveSelectionColour(ref ColourRect colourRect)
        {
            SetColourRectToOptionalPropertyColour(InactiveSelectionColourPropertyName, ref colourRect);
        }

        /// <summary>
        /// Set the given ColourRect to the colour(s) fetched from the named property if it exists, 
        /// else the default colour of black.
        /// </summary>
        /// <param name="propertyName">
        /// string object holding the name of the property to be accessed if it exists.
        /// </param>
        /// <param name="colourRect">
        /// Reference to a ColourRect that will be set.
        /// </param>
        protected void SetColourRectToOptionalPropertyColour(string propertyName, ref ColourRect colourRect)
        {
            if (Window.IsPropertyPresent(propertyName))
                colourRect =
                    Window.GetProperty<ColourRect>(propertyName);
            else
                colourRect.SetColours(new Colour(0));
        }

        #region Fields

        /// <summary>
        /// true if the caret imagery should blink.
        /// </summary>
        private bool _blinkCaret;
        
        /// <summary>
        /// time-out in seconds used for blinking the caret.
        /// </summary>
        private float _caretBlinkTimeout;
        
        /// <summary>
        /// current time elapsed since last caret blink state change.
        /// </summary>
        private float _caretBlinkElapsed;
        
        /// <summary>
        /// true if caret should be shown.
        /// </summary>
        private bool _showCaret;

        #endregion
    }
}