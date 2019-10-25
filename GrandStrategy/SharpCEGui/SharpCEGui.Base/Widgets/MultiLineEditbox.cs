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

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for multi-line edit box window renderer objects.
    /// </summary>
    public abstract class MultiLineEditboxWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected MultiLineEditboxWindowRenderer(string name)
            : base(name, MultiLineEditbox.EventNamespace)
        {
            
        }

        /// <summary>
        /// Return a Rect object describing, in un-clipped pixels, the window relative area
        /// that the text should be rendered in to.
        /// </summary>
        /// <returns>
        /// Rect object describing the area of the Window to be used for rendering text.
        /// </returns>
        public abstract Rectf GetTextRenderArea();

        // base class overrides
        protected internal override void OnLookNFeelAssigned()
        {
            global::System.Diagnostics.Debug.Assert(Window != null);

            // ensure window's text has a terminating \n
            var text = Window.GetText();
            if (String.IsNullOrEmpty(text) || text[text.Length - 1] != '\n')
            {
                text += '\n';
                Window.SetText(text);
            }
        }
    }

    /// <summary>
    /// Base class for the multi-line edit box widget.
    /// </summary>
    public class MultiLineEditbox : EditboxBase
    {
        #region Constants

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "MultiLineEditbox";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/MultiLineEditbox";

        /// <summary>
        /// Widget name for the vertical scrollbar component.
        /// </summary>
        public const string VertScrollbarName = "__auto_vscrollbar__";

        /// <summary>
        /// Widget name for the horizontal scrollbar component.
        /// </summary>
        public const string HorzScrollbarName = "__auto_hscrollbar__";

        #endregion

        #region Events

        /// <summary>
        /// Event fired when the read-only mode for the edit box has been changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiLineEditbox whose read-only mode
        /// was changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> ReadOnlyModeChanged;

        /// <summary>
        /// Event fired when the word wrap mode of the edit box has been changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiLineEditbox whose word wrap
        /// mode was changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> WordWrapModeChanged;

        /// <summary>
        /// Event fired when the maximum allowable string length for the edit box
        /// has been changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiLineEditbox whose maximum string
        /// length was changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> MaximumTextLengthChanged;

        /// <summary>
        /// Event fired when the text caret / current insertion position is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiLineEditbox whose caret position
        /// has changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> CaretMoved;

        /// <summary>
        /// Event fired when the current text selection for the edit box is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiLineEditbox whose text selection
        /// was changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> TextSelectionChanged;

        /// <summary>
        /// Event fired when the number of characters in the edit box reaches the
        /// current maximum length.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiLineEditbox whose text length
        /// has reached the set maximum allowable length for the edit box.
        /// </summary>
        public event EventHandler<WindowEventArgs> EditboxFull;

        /// <summary>
        /// Event fired when the mode setting that forces the display of the
        /// vertical scroll bar for the edit box is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiLineEditbox whose vertical
        /// scrollbar mode has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> VertScrollbarModeChanged;

        /// <summary>
        /// Event fired when the mode setting that forces the display of the
        /// horizontal scroll bar for the edit box is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiLineEditbox whose horizontal
        /// scrollbar mode has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> HorzScrollbarModeChanged;

        #endregion

        #region Inner Types

        /// <summary>
        /// struct used to store information about a formatted line within the
        /// paragraph.
        /// </summary>
        public struct LineInfo
        {
            /// <summary>
            /// Starting index for this line.
            /// </summary>
            public int d_startIdx;

            /// <summary>
            /// Code point length of this line.
            /// </summary>
            public int d_length;

            /// <summary>
            /// Rendered extent of this line.
            /// </summary>
            public float d_extent;
        }

        #endregion

        /// <summary>
        /// return true if the edit box has input focus.
        /// </summary>
        /// <returns>
        /// - true if the edit box has keyboard input focus.
        /// - false if the edit box does not have keyboard input focus.
        /// </returns>
        public bool HasInputFocus()
        {
            return IsActive();
        }

        /// <summary>
        /// return true if the edit box is read-only.
        /// </summary>
        /// <returns>
        /// - true if the edit box is read only and can't be edited by the user.
        /// - false if the edit box is not read only and may be edited by the user.
        /// </returns>
        public bool IsReadOnly()
        {
            return d_readOnly;
        }

        /// <summary>
        /// return the current position of the caret.
        /// </summary>
        /// <returns>
        /// Index of the insert caret relative to the start of the text.
        /// </returns>
        public int GetCaretIndex()
        {
            return d_caretPos;
        }

        /// <summary>
        /// return the current selection start point.
        /// </summary>
        /// <returns>
        /// Index of the selection start point relative to the start of the text.  
        /// If no selection is defined this function returns the position of the caret.
        /// </returns>
        public int GetSelectionStartIndex()
        {
            return (d_selectionStart != d_selectionEnd) ? d_selectionStart : d_caretPos;
        }

        /// <summary>
        /// return the current selection end point.
        /// </summary>
        /// <returns>
        /// Index of the selection end point relative to the start of the text.
        /// If no selection is defined this function returns the position of 
        /// the caret.
        /// </returns>
        public int GetSelectionEndIndex()
        {
            return (d_selectionStart != d_selectionEnd) ? d_selectionEnd : d_caretPos;
        }

        /// <summary>
        /// return the length of the current selection (in code points / characters).
        /// </summary>
        /// <returns>
        /// Number of code points (or characters) contained within the currently defined selection.
        /// </returns>
        public int GetSelectionLength()
        {
            return d_selectionEnd - d_selectionStart;
        }

        /// <summary>
        /// return the maximum text length set for this edit box.
        /// </summary>
        /// <returns>
        /// The maximum number of code points (characters) that can be entered into this edit box.
        /// </returns>
        public int GetMaxTextLength()
        {
            return d_maxTextLen;
        }

        /// <summary>
        /// Return whether the text in the edit box will be word-wrapped.
        /// </summary>
        /// <returns>
        /// - true if the text will be word-wrapped at the edges of the widget frame.
        /// - false if text will not be word-wrapped (a scroll bar will be used to access long text lines).
        /// </returns>
        public bool IsWordWrapped()
        {
            return d_wordWrap;
        }

        /// <summary>
        /// Return a pointer to the vertical scrollbar component widget for this
        /// MultiLineEditbox.
        /// </summary>
        /// <returns>
        /// Pointer to a Scrollbar object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the vertical Scrollbar component does not exist.
        /// </exception>
        public Scrollbar GetVertScrollbar()
        {
            return (Scrollbar) GetChild(VertScrollbarName);
        }

        /// <summary>
        /// UnknownObjectException
        /// </summary>
        /// <returns>
        /// - true if the scroll bar will always be shown even if it is not required.
        /// - false if the scroll bar will only be shown when it is required.
        /// </returns>
        public bool IsVertScrollbarAlwaysShown()
        {
            return d_forceVertScroll;
        }

        /// <summary>
        /// Return a pointer to the horizontal scrollbar component widget for this
        /// MultiLineEditbox.
        /// </summary>
        /// <returns>
        /// Pointer to a Scrollbar object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the horizontal Scrollbar component does not exist.
        /// </exception>
        public Scrollbar GetHorzScrollbar()
        {
            return (Scrollbar) GetChild(HorzScrollbarName);
        }

        /// <summary>
        /// Return a Rect object describing, in un-clipped pixels, the window relative area
        /// that the text should be rendered in to.
        /// </summary>
        /// <returns>
        /// Rect object describing the area of the Window to be used for rendering text.
        /// </returns>
        public Rectf GetTextRenderArea()
        {
            if (d_windowRenderer != null)
            {
                var wr = (MultiLineEditboxWindowRenderer) d_windowRenderer;
                return wr.GetTextRenderArea();
            }
            
            throw new InvalidRequestException("This function must be implemented by the window renderer module");
        }

        /// <summary>
        /// get d_lines 
        /// </summary>
        /// <returns></returns>
        public IList<LineInfo> GetFormattedLines()
        {
            return d_lines;
        }

        /// <summary>
        /// Return the line number a given index falls on with the current formatting.  Will return last line
        /// if index is out of range.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetLineNumberFromIndex(int index)
        {
            var lineCount = d_lines.Count;

            if (lineCount == 0)
                return 0;
            
            if (index >= GetText().Length - 1)
                return lineCount - 1;

            var indexCount = 0;
            var caretLine = 0;

            for (; caretLine < lineCount; ++caretLine)
            {
                indexCount += d_lines[caretLine].d_length;

                if (index < indexCount)
                {
                    return caretLine;
                }
            }

            throw new InvalidRequestException("Unable to identify a line from the given, invalid, index.");
        }

        /// <summary>
        /// Initialise the Window based object ready for use.
        /// </summary>
        /// <remarks>
        /// This must be called for every window created.  
        /// Normally this is handled automatically by the WindowFactory 
        /// for each Window type.
        /// </remarks>
        protected override void InitialiseComponents()
        {
            // create the component sub-widgets
            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

            vertScrollbar.Shown += HandleVertScrollbarVisibilityChanged;
            vertScrollbar.Hidden += HandleVertScrollbarVisibilityChanged;

            vertScrollbar.ScrollPositionChanged += HandleScrollChange;
            horzScrollbar.ScrollPositionChanged += HandleScrollChange;

            FormatText(true);
            PerformChildWindowLayout();
        }
        
        /// <summary>
        /// Specify whether the edit box is read-only.
        /// </summary>
        /// <param name="setting">
        /// - true if the edit box is read only and can't be edited by the user.
        /// - false if the edit box is not read only and may be edited by the user.
        /// </param>
        public void SetReadOnly(bool setting)
        {
            // if setting is changed
	        if (d_readOnly != setting)
	        {
		        d_readOnly = setting;
 		        OnReadOnlyChanged(new WindowEventArgs(this));
	        }
        }

        /// <summary>
        /// Set the current position of the caret.
        /// </summary>
        /// <param name="caretPos">
        /// New index for the insert caret relative to the start of the text.  
        /// If the value specified is greater than the number of characters in 
        /// the edit box, the caret is positioned at the end of the text.
        /// </param>
        public void SetCaretIndex(int caretPos)
        {
            // make sure new position is valid
            if (caretPos > GetText().Length - 1)
            {
                caretPos = GetText().Length - 1;
            }

            // if new position is different
            if (d_caretPos != caretPos)
            {
                d_caretPos = caretPos;
                EnsureCaretIsVisible();

                // Trigger "caret moved" event
                OnCaretMoved(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Define the current selection for the edit box
        /// </summary>
        /// <param name="startPos">
        /// Index of the starting point for the selection.  
        /// If this value is greater than the number of characters in the edit box, 
        /// the selection start will be set to the end of the text.
        /// </param>
        /// <param name="endPos">
        /// Index of the ending point for the selection.  If this value is 
        /// greater than the number of characters in the edit box, the
        /// selection start will be set to the end of the text.
        /// </param>
        public void SetSelection(int startPos, int endPos)
        {
            // ensure selection start point is within the valid range
            if (startPos > GetText().Length - 1)
            {
                startPos = GetText().Length - 1;
            }

            // ensure selection end point is within the valid range
            if (endPos > GetText().Length - 1)
            {
                endPos = GetText().Length - 1;
            }

            // ensure start is before end
            if (startPos > endPos)
            {
                var tmp = endPos;
                endPos = startPos;
                startPos = tmp;
            }

            // only change state if values are different.
            if ((startPos != d_selectionStart) || (endPos != d_selectionEnd))
            {
                // setup selection
                d_selectionStart = startPos;
                d_selectionEnd = endPos;

                // Trigger "selection changed" event
                OnTextSelectionChanged(new WindowEventArgs(this));
            }
        }


        /// <summary>
        /// Define the current selection start for the Editbox
        /// </summary>
        /// <param name="startPos">
        /// Index of the starting point for the selection.  If this value is greater 
        /// than the number of characters in the Editbox, the selection start will be 
        /// set to the end of the text.
        /// </param>
        public void SetSelectionStart(int startPos)
        {
            SetSelection(startPos, startPos + GetSelectionLength());
        }

        /// <summary>
        /// Define the current selection for the Editbox
        /// </summary>
        /// <param name="length">
        /// Length of the selection.
        /// </param>
        public void SetSelectionLength(int length)
        {
            SetSelection(GetSelectionStartIndex(), GetSelectionStartIndex() + length);
        }

        /// <summary>
        /// set the maximum text length for this edit box.
        /// </summary>
        /// <param name="maxLen">
        /// The maximum number of code points (characters) that can be entered into this Editbox.
        /// </param>
        public override void SetMaxTextLength(int maxLen)
        {
            if (d_maxTextLen != maxLen)
	        {
		        d_maxTextLen = maxLen;

		        // Trigger max length changed event
	            var args = new WindowEventArgs(this);
		        OnMaximumTextLengthChanged(args);

		        // trim string
                if (GetText().Length > d_maxTextLen)
		        {
                    var newText = GetText();
                    // TODO: ... newText = newText.resize(d_maxTextLen);
                    SetText(newText);
                    d_undoHandler.ClearUndoHistory();

			        OnTextChanged(args);
		        }

	        }
        }

        /// <summary>
        /// Scroll the view so that the current caret position is visible.
        /// </summary>
        public void EnsureCaretIsVisible()
        {
            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

            // calculate the location of the caret
            var fnt = GetFont();
            var caretLine = GetLineNumberFromIndex(d_caretPos);

            if (caretLine < d_lines.Count)
            {
                var textArea = GetTextRenderArea();

                var caretLineIdx = d_caretPos - d_lines[caretLine].d_startIdx;

                var ypos = caretLine*fnt.GetLineSpacing();
                var xpos = fnt.GetTextAdvance(GetText().CEGuiSubstring(d_lines[caretLine].d_startIdx, caretLineIdx));

                // adjust position for scroll bars
                xpos -= horzScrollbar.GetScrollPosition();
                ypos -= vertScrollbar.GetScrollPosition();

                // if caret is above window, scroll up
                if (ypos < 0)
                {
                    vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition() + ypos);
                }
                    // if caret is below the window, scroll down
                else if ((ypos += fnt.GetLineSpacing()) > textArea.Height)
                {
                    vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition() + (ypos - textArea.Height) +
                                                    fnt.GetLineSpacing());
                }

                // if caret is left of the window, scroll left
                if (xpos < 0)
                {
                    horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition() + xpos - 50);
                }
                    // if caret is right of the window, scroll right
                else if (xpos > textArea.Width)
                {
                    horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition() + (xpos - textArea.Width) + 50);
                }
            }
        }

        /// <summary>
        /// Set whether the text will be word wrapped or not.
        /// </summary>
        /// <param name="setting">
        /// - true if the text should word-wrap at the edges of the text box.
        /// - false if the text should not wrap, but a scroll bar should be used.
        /// </param>
        public void SetWordWrapping(bool setting)
        {
            if (setting != d_wordWrap)
	        {
		        d_wordWrap = setting;
		        FormatText(true);

		        OnWordWrapModeChanged(new WindowEventArgs(this));
	        }
        }

        /// <summary>
        /// Set whether the vertical scroll bar should always be shown.
        /// </summary>
        /// <param name="setting">
        /// true if the vertical scroll bar should be shown even when it is not required.  
        /// false if the vertical scroll bar should only be shown when it is required.
        /// </param>
        public void SetShowVertScrollbar(bool setting)
        {
            if (d_forceVertScroll != setting)
	        {
		        d_forceVertScroll = setting;

		        ConfigureScrollbars();
		        OnVertScrollbarModeChanged(new WindowEventArgs(this));
	        }
        }

        /// <summary>
        /// selection brush image property support
        /// </summary>
        /// <param name="image"></param>
        public void SetSelectionBrushImage(Image image)
        {
            d_selectionBrush = image;
            Invalidate(false);
        }

        /// <summary>
        /// get the selection brush image for the editbox
        /// </summary>
        /// <returns></returns>
        public Image GetSelectionBrushImage()
        {
            return d_selectionBrush;
        }

        public override bool PerformCopy(Clipboard clipboard)
        {
            if (GetSelectionLength() == 0)
                return false;

            var selectedText = GetText().CEGuiSubstring(GetSelectionStartIndex(), GetSelectionLength());

            clipboard.SetText(selectedText);
            return true;
        }

        public override bool PerformCut(Clipboard clipboard)
        {
            if (IsReadOnly())
                return false;

            if (!PerformCopy(clipboard))
                return false;

            HandleDelete();

            return true;
        }

        public override bool PerformPaste(Clipboard clipboard)
        {
            if (IsReadOnly())
                return false;

            var clipboardText = clipboard.GetText();

            if (String.IsNullOrEmpty(clipboardText))
                return false;

            // backup current text
            var tmp = GetText().Remove(GetSelectionStartIndex(), GetSelectionLength());

            // erase selected text
            EraseSelectedText();

            // if there is room
            if (GetText().Length - clipboardText.Length < d_maxTextLen)
            {
                var newText = GetText();
                var undo = new UndoHandler.UndoAction
                               {
                                   d_type = UndoHandler.UndoActionType.UAT_INSERT,
                                   d_startIdx = GetCaretIndex(),
                                   d_text = clipboardText
                               };
                d_undoHandler.AddUndoHistory(undo);
                newText = newText.Insert(GetCaretIndex(), clipboardText);
                SetText(newText);

                d_caretPos += clipboardText.Length;

                OnTextChanged(new WindowEventArgs(this));

                return true;
            }

            // Trigger text box full event
            OnEditboxFullEvent(new WindowEventArgs(this));

            return false;
        }

        /// <summary>
        /// Format the text into lines as dictated by the formatting options.
        /// </summary>
        /// <param name="updateScrollbars">
        /// - true if scrollbar configuration should be performed.
        /// - false if scrollbar configuration should not be performed.
        /// </param>
        public void FormatText(bool updateScrollbars)
        {
            d_widestExtent = 0.0f;

            var fnt = GetFont();

            if (fnt != null)
            {
                var areaWidth = GetTextRenderArea().Width;
                var currPos = 0;

                // now we will check if our width changed, if not we'll just update text from last cursor
                if (Math.Abs(areaWidth - d_lastRenderWidth) > float.Epsilon || d_undoHandler.CanUndo())
                {
                    d_lines.Clear();
                }
                else
                {
                    // ok there's no need to update whole text
                    var lastuUndoPos = d_undoHandler.GetLastAction().d_startIdx;
                    // ok now delete all formatting data before lastuUndoPos
                    var countToRemove = GetLineNumberFromIndex(lastuUndoPos) - 1;
                    if (countToRemove >= 0)
                    {
                        d_lines.RemoveRange(countToRemove, d_lines.Count - countToRemove);
                        if (d_lines.Count > 0)
                            currPos = d_lines[d_lines.Count - 1].d_startIdx + d_lines[d_lines.Count - 1].d_length;
                    }
                    else
                    {
                        d_lines.Clear();
                    }
                }

                while (currPos < GetText().Length)
                {
                    int paraLen;
                    if ((paraLen = GetText().IndexOf(d_lineBreakChars, currPos)) == -1)
                    {
                        paraLen = GetText().Length - currPos;
                    }
                    else
                    {
                        paraLen -= currPos;
                        paraLen++;
                    }

                    var paraText = GetText().CEGuiSubstring(currPos, paraLen);

                    if (!d_wordWrap || (areaWidth <= 0.0f))
                    {
                        // no word wrapping, so we are just one long line.
                        var line = new LineInfo
                                       {
                                           d_startIdx = currPos,
                                           d_length = paraLen,
                                           d_extent = fnt.GetTextExtent(paraText)
                                       };
                        d_lines.Add(line);

                        // update widest, if needed.
                        if (line.d_extent > d_widestExtent)
                        {
                            d_widestExtent = line.d_extent;
                        }

                    }
                        // must word-wrap the paragraph text
                    else
                    {
                        var lineIndex = 0;

                        // while there is text in the string
                        while (lineIndex < paraLen)
                        {
                            var lineLen = 0;
                            var lineExtent = 0.0f;

                            // loop while we have not reached the end of the paragraph string
                            while (lineLen < (paraLen - lineIndex))
                            {
                                // get cp / char count of next token
                                var nextTokenSize = GetNextTokenLength(paraText, lineIndex + lineLen);

                                // get pixel width of the token
                                float tokenExtent =
                                    fnt.GetTextExtent(paraText.CEGuiSubstring(lineIndex + lineLen, nextTokenSize));

                                // would adding this token would overflow the available width
                                if ((lineExtent + tokenExtent) > areaWidth)
                                {
                                    // Was this the first token?
                                    if (lineLen == 0)
                                    {
                                        // get point at which to break the token
                                        lineLen = fnt.GetCharAtPixel(paraText.CEGuiSubstring(lineIndex, nextTokenSize),
                                                                     areaWidth);
                                    }

                                    // text wraps, exit loop early with line info up until wrap point
                                    break;
                                }

                                // add this token to current line
                                lineLen += nextTokenSize;
                                lineExtent += tokenExtent;
                            }

                            // set up line info and add to collection
                            d_lines.Add(new LineInfo
                                            {
                                                d_startIdx = currPos + lineIndex,
                                                d_length = lineLen,
                                                d_extent = lineExtent
                                            });

                            // update widest, if needed.
                            if (lineExtent > d_widestExtent)
                            {
                                d_widestExtent = lineExtent;
                            }

                            // update position in string
                            lineIndex += lineLen;
                        }

                    }

                    // skip to next 'paragraph' in text
                    currPos += paraLen;
                }

                d_lastRenderWidth = areaWidth;
            }

            if (updateScrollbars)
                ConfigureScrollbars();

            Invalidate(false);
        }

        public override bool PerformUndo()
        {
            var result = false;
            if (!IsReadOnly())
            {
                ClearSelection();
                result = d_undoHandler.Undo(ref d_caretPos);
                OnTextChanged(new WindowEventArgs(this));
            }

            return result;
        }

        public override bool PerformRedo()
        {
            var result = false;
            if (!IsReadOnly())
            {
                ClearSelection();
                result = d_undoHandler.Redo(ref d_caretPos);
                OnTextChanged(new WindowEventArgs(this));
            }

            return result;
        }

        /// <summary>
        /// Constructor for the MultiLineEditbox base class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public MultiLineEditbox(string type, string name)
            : base(type, name)
        {
            d_readOnly = false;
            d_maxTextLen = Int32.MaxValue;
            d_caretPos = 0;
            d_selectionStart = 0;
            d_selectionEnd = 0;
            d_dragging = false;
            d_dragAnchorIdx = 0;
            d_wordWrap = true;
            d_widestExtent = 0.0f;
            d_forceVertScroll = false;
            d_forceHorzScroll = false;
            d_selectionBrush = null;
            d_lastRenderWidth = 0.0f;

            AddMultiLineEditboxProperties();

            // create undo handler
            d_undoHandler = new UndoHandler(this);

            // override default and disable text parsing
            d_textParsingEnabled = false;

            // Since parsing is ever allowed in the editbox, ban the property too.
            BanPropertyFromXML("TextParsingEnabled");
        }


        // TODO: Destructor for the MultiLineEditbox base class.
        // TODO: virtual ~MultiLineEditbox(void);


        /// <summary>
        /// Format the text into lines as needed by the current formatting options.
        /// </summary>
        /// <remarks>This is deprecated in favour of the version taking a boolean.</remarks>
        [Obsolete("This is deprecated in favour of the version taking a boolean.")]
        protected void FormatText()
        {
            FormatText(true);
        }

        /// <summary>
        /// Return the length of the next token in String \a text starting at index \a start_idx.
        /// <para>Any single whitespace character is one token, any group of other characters is a token.</para>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIdx"></param>
        /// <returns>The code point length of the token.</returns>
        protected int GetNextTokenLength(string text, int startIdx)
        {
            var pos = text.IndexOf(TextUtils.DefaultWrapDelimiters, startIdx);

            // handle case where no more whitespace exists (so this is last token)
            if (pos == -1)
                return (text.Length - startIdx);
            
            // handle 'delimiter' token cases
            if ((pos - startIdx) == 0)
                return 1;

            return (pos - startIdx);
        }


        /// <summary>
        /// display required integrated scroll bars according to current state of the edit box and update their values.
        /// </summary>
        protected void ConfigureScrollbars()
        {
            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();
            var lspc = GetFont().GetLineSpacing();

            //
            // First show or hide the scroll bars as needed (or requested)
            //
            // show or hide vertical scroll bar as required (or as specified by option)
            if (d_forceVertScroll ||
                ((d_lines.Count)*lspc > GetTextRenderArea().Height))
            {
                vertScrollbar.Show();

                // show or hide horizontal scroll bar as required (or as specified by option)
                horzScrollbar.SetVisible(d_forceHorzScroll ||
                                         (d_widestExtent > GetTextRenderArea().Width));
            }
                // show or hide horizontal scroll bar as required (or as specified by option)
            else if (d_forceHorzScroll ||
                     (d_widestExtent > GetTextRenderArea().Width))
            {
                horzScrollbar.Show();

                // show or hide vertical scroll bar as required (or as specified by option)
                vertScrollbar.SetVisible(d_forceVertScroll ||
                                         ((d_lines.Count)*lspc > GetTextRenderArea().Height));
            }
            else
            {
                vertScrollbar.Hide();
                horzScrollbar.Hide();
            }

            //
            // Set up scroll bar values
            //
            var renderArea = GetTextRenderArea();

            vertScrollbar.SetDocumentSize(d_lines.Count*lspc);
            vertScrollbar.SetPageSize(renderArea.Height);
            vertScrollbar.SetStepSize(Math.Max(1.0f, renderArea.Height/10.0f));
            vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition());

            horzScrollbar.SetDocumentSize(d_widestExtent);
            horzScrollbar.SetPageSize(renderArea.Width);
            horzScrollbar.SetStepSize(Math.Max(1.0f, renderArea.Width/10.0f));
            horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition());
        }

        /*!
        \brief
            Return the text code point index that is rendered closest to screen position \a pt.

        \param pt
            Point object describing a position on the screen in pixels.

        \return
            Code point index into the text that is rendered closest to screen position \a pt.
        */

        protected override int GetTextIndexFromPosition(Lunatics.Mathematics.Vector2 pt)
        {
            // calculate final window position to be checked
            var wndPt = CoordConverter.ScreenToWindow(this, pt);

            var textArea = GetTextRenderArea();

            wndPt -= textArea.d_min;

            // factor in scroll bar values
            wndPt.X += GetHorzScrollbar().GetScrollPosition();
            wndPt.Y += GetVertScrollbar().GetScrollPosition();

            var lineNumber = (int) (Math.Max(0.0f, wndPt.Y)/GetFont().GetLineSpacing());

            if (lineNumber >= d_lines.Count)
            {
                lineNumber = d_lines.Count - 1;
            }

            var lineText = GetText().CEGuiSubstring(d_lines[lineNumber].d_startIdx, d_lines[lineNumber].d_length);

            var lineIdx = GetFont().GetCharAtPixel(lineText, wndPt.X);

            if (lineIdx >= lineText.Length - 1)
            {
                lineIdx = lineText.Length - 1;
            }

            return d_lines[lineNumber].d_startIdx + lineIdx;
        }

        /// <summary>
        /// Erase the currently selected text.
        /// </summary>
        /// <param name="modifyText">
        /// when true, the actual text will be modified.  When false, everything is done except erasing the characters.
        /// </param>
        protected override void EraseSelectedText(bool modifyText = true)
        {
            if (GetSelectionLength() != 0)
            {
                // setup new caret position and remove selection highlight.
                SetCaretIndex(GetSelectionStartIndex());

                // erase the selected characters (if required)
                if (modifyText)
                {
                    var newText = GetText();
                    var undo = new UndoHandler.UndoAction
                                   {
                                       d_type = UndoHandler.UndoActionType.UAT_DELETE,
                                       d_startIdx = GetSelectionStartIndex(),
                                       d_text =
                                           newText.CEGuiSubstring(GetSelectionStartIndex(), GetSelectionLength())
                                   };
                    d_undoHandler.AddUndoHistory(undo);
                    newText = newText.Remove(GetSelectionStartIndex(), GetSelectionLength());
                    SetText(newText);

                    // trigger notification that text has changed.
                    OnTextChanged(new WindowEventArgs(this));
                }

                ClearSelection();
            }
        }

        /// <summary>
        /// Processing for backspace key
        /// </summary>
        protected override void HandleBackspace()
        {
            if (!IsReadOnly())
            {
                if (GetSelectionLength() != 0)
                {
                    EraseSelectedText();
                }
                else if (d_caretPos > 0)
                {
                    var newText = GetText();
                    var undo = new UndoHandler.UndoAction
                                   {
                                       d_type = UndoHandler.UndoActionType.UAT_DELETE,
                                       d_startIdx = d_caretPos - 1,
                                       d_text = newText.Substring(d_caretPos - 1, 1)
                                   };
                    d_undoHandler.AddUndoHistory(undo);
                    newText = newText.Remove(d_caretPos - 1, 1);
                    SetCaretIndex(d_caretPos - 1);
                    SetText(newText);

                    OnTextChanged(new WindowEventArgs(this));
                }
            }
        }

        /// <summary>
        /// Processing for Delete key
        /// </summary>
        protected override void HandleDelete()
        {
            if (!IsReadOnly())
            {
                if (GetSelectionLength() != 0)
                {
                    EraseSelectedText();
                }
                else if (GetCaretIndex() < GetText().Length - 1)
                {
                    var newText = GetText();
                    var undo = new UndoHandler.UndoAction
                                   {
                                       d_type = UndoHandler.UndoActionType.UAT_DELETE,
                                       d_startIdx = d_caretPos,
                                       d_text = newText.Substring(d_caretPos, 1)
                                   };
                    d_undoHandler.AddUndoHistory(undo);
                    newText = newText.Remove(d_caretPos, 1);
                    SetText(newText);

                    EnsureCaretIsVisible();

                    OnTextChanged(new WindowEventArgs(this));
                }
            }
        }

        /// <summary>
        /// Processing to move caret one character left
        /// </summary>
        /// <param name="sysKeys"></param>
        protected void HandleCharLeft(uint sysKeys)
        {
            if (d_caretPos > 0)
            {
                SetCaretIndex(d_caretPos - 1);
            }

            if ((sysKeys & (uint) SystemKey.Shift) == (uint) SystemKey.Shift)
            {
                SetSelection(d_caretPos, d_dragAnchorIdx);
            }
            else
            {
                ClearSelection();
            }
        }

        /// <summary>
        /// Processing to move caret one word left
        /// </summary>
        /// <param name="sysKeys"></param>
        protected void HandleWordLeft(uint sysKeys)
        {
            if (d_caretPos > 0)
            {
                SetCaretIndex(TextUtils.GetWordStartIdx(GetText(), GetCaretIndex()));
            }

            if ((sysKeys & (uint) SystemKey.Shift) == (uint) SystemKey.Shift)
            {
                SetSelection(d_caretPos, d_dragAnchorIdx);
            }
            else
            {
                ClearSelection();
            }
        }

        /// <summary>
        /// Processing to move caret one character right
        /// </summary>
        /// <param name="sysKeys"></param>
        protected void HandleCharRight(uint sysKeys)
        {
            if (d_caretPos < GetText().Length - 1)
            {
                SetCaretIndex(d_caretPos + 1);
            }

            if ((sysKeys & (uint) SystemKey.Shift) == (uint) SystemKey.Shift)
            {
                SetSelection(d_caretPos, d_dragAnchorIdx);
            }
            else
            {
                ClearSelection();
            }
        }

        /// <summary>
        /// Processing to move caret one word right
        /// </summary>
        /// <param name="sysKeys"></param>
        protected void HandleWordRight(uint sysKeys)
        {
            if (d_caretPos < GetText().Length - 1)
            {
                SetCaretIndex(TextUtils.GetNextWordStartIdx(GetText(), GetCaretIndex()));
            }

            if ((sysKeys & (uint) SystemKey.Shift) == (uint) SystemKey.Shift)
            {
                SetSelection(d_caretPos, d_dragAnchorIdx);
            }
            else
            {
                ClearSelection();
            }
        }

        /// <summary>
        /// Processing to move caret to the start of the text.
        /// </summary>
        /// <param name="sysKeys"></param>
        protected void HandleDocHome(uint sysKeys)
        {
            if (d_caretPos > 0)
            {
                SetCaretIndex(0);
            }

            if ((sysKeys & (uint) SystemKey.Shift) == (uint) SystemKey.Shift)
            {
                SetSelection(d_caretPos, d_dragAnchorIdx);
            }
            else
            {
                ClearSelection();
            }
        }

        /// <summary>
        /// Processing to move caret to the end of the text
        /// </summary>
        /// <param name="sysKeys"></param>
        protected void HandleDocEnd(uint sysKeys)
        {
            if (d_caretPos < GetText().Length - 1)
            {
                SetCaretIndex(GetText().Length - 1);
            }

            if ((sysKeys & (uint) SystemKey.Shift) == (uint) SystemKey.Shift)
            {
                SetSelection(d_caretPos, d_dragAnchorIdx);
            }
            else
            {
                ClearSelection();
            }
        }

        /// <summary>
        /// Processing to move caret to the start of the current line.
        /// </summary>
        /// <param name="select"></param>
        protected void HandleLineHome(bool select)
        {
            var line = GetLineNumberFromIndex(d_caretPos);

            if (line < d_lines.Count)
            {
                var lineStartIdx = d_lines[line].d_startIdx;

                if (d_caretPos > lineStartIdx)
                {
                    SetCaretIndex(lineStartIdx);
                }

                if (select)
                {
                    SetSelection(d_caretPos, d_dragAnchorIdx);
                }
                else
                {
                    ClearSelection();
                }
            }
        }

        /// <summary>
        /// Processing to move caret to the end of the current line
        /// </summary>
        /// <param name="select"></param>
        protected void HandleLineEnd(bool select)
        {
            var line = GetLineNumberFromIndex(d_caretPos);

            if (line < d_lines.Count)
            {
                var lineEndIdx = d_lines[line].d_startIdx + d_lines[line].d_length - 1;

                if (d_caretPos < lineEndIdx)
                {
                    SetCaretIndex(lineEndIdx);
                }

                if (select)
                {
                    SetSelection(d_caretPos, d_dragAnchorIdx);
                }
                else
                {
                    ClearSelection();
                }
            }
        }

        /// <summary>
        /// Processing to move caret up a line.
        /// </summary>
        /// <param name="select"></param>
        protected void HandleLineUp(bool select)
        {
            var caretLine = GetLineNumberFromIndex(d_caretPos);

            if (caretLine > 0)
            {
                var caretPixelOffset =
                    GetFont()
                        .GetTextAdvance(GetText()
                                            .CEGuiSubstring(d_lines[caretLine].d_startIdx,
                                                            d_caretPos - d_lines[caretLine].d_startIdx));

                --caretLine;

                var newLineIndex =
                    GetFont()
                        .GetCharAtPixel(
                            GetText().CEGuiSubstring(d_lines[caretLine].d_startIdx, d_lines[caretLine].d_length),
                            caretPixelOffset);

                SetCaretIndex(d_lines[caretLine].d_startIdx + newLineIndex);
            }

            if (select)
            {
                SetSelection(d_caretPos, d_dragAnchorIdx);
            }
            else
            {
                ClearSelection();
            }
        }

        /// <summary>
        /// Processing to move caret down a line.
        /// </summary>
        /// <param name="select"></param>
        protected void HandleLineDown(bool select)
        {
            var caretLine = GetLineNumberFromIndex(d_caretPos);

            if ((d_lines.Count > 1) && (caretLine < (d_lines.Count - 1)))
            {
                var caretPixelOffset =
                    GetFont()
                        .GetTextAdvance(GetText()
                                            .CEGuiSubstring(d_lines[caretLine].d_startIdx,
                                                            d_caretPos - d_lines[caretLine].d_startIdx));

                ++caretLine;

                var newLineIndex =
                    GetFont()
                        .GetCharAtPixel(
                            GetText().CEGuiSubstring(d_lines[caretLine].d_startIdx, d_lines[caretLine].d_length),
                            caretPixelOffset);

                SetCaretIndex(d_lines[caretLine].d_startIdx + newLineIndex);
            }

            if (select)
            {
                SetSelection(d_caretPos, d_dragAnchorIdx);
            }
            else
            {
                ClearSelection();
            }
        }

        /// <summary>
        /// Processing to insert a new line / paragraph.
        /// </summary>
        protected void HandleNewLine()
        {
            if (!IsReadOnly())
            {
                // erase selected text
                EraseSelectedText();

                // if there is room
                if (GetText().Length - 1 < d_maxTextLen)
                {
                    var newText = GetText();
                    var undo = new UndoHandler.UndoAction
                                   {
                                       d_type = UndoHandler.UndoActionType.UAT_INSERT,
                                       d_startIdx = GetCaretIndex(),
                                       d_text = "\x0a"
                                   };
                    d_undoHandler.AddUndoHistory(undo);
                    newText = newText.Insert(GetCaretIndex(), "\x0a");
                    SetText(newText);

                    d_caretPos++;

                    OnTextChanged(new WindowEventArgs(this));
                }
            }
        }

        /// <summary>
        /// Processing to move caret one page up
        /// </summary>
        /// <param name="select"></param>
        protected void HandlePageUp(bool select)
        {
            var caretLine = GetLineNumberFromIndex(d_caretPos);
            var nbLine = (int) (GetTextRenderArea().Height/GetFont().GetLineSpacing());
            var newline = 0;
            if (nbLine < caretLine)
            {
                newline = caretLine - nbLine;
            }
            SetCaretIndex(d_lines[newline].d_startIdx);
            if (select)
            {
                SetSelection(d_caretPos, d_selectionEnd);
            }
            else
            {
                ClearSelection();
            }
            EnsureCaretIsVisible();
        }

        /// <summary>
        /// Processing to move caret one page down
        /// </summary>
        /// <param name="select"></param>
        protected void HandlePageDown(bool select)
        {
            var caretLine = GetLineNumberFromIndex(d_caretPos);
            var nbLine = (int) (GetTextRenderArea().Height/GetFont().GetLineSpacing());
            var newline = caretLine + nbLine;
            if (d_lines.Count != 0)
            {
                newline = Math.Min(newline, d_lines.Count - 1);
            }
            SetCaretIndex(d_lines[newline].d_startIdx + d_lines[newline].d_length - 1);
            if (select)
            {
                SetSelection(d_selectionStart, d_caretPos);
            }
            else
            {
                ClearSelection();
            }
            EnsureCaretIsVisible();
        }

        /// <summary>
        /// Internal handler that is triggered when the user interacts with the scrollbars.
        /// </summary>
        /// <param name="args"></param>
        protected bool HandleScrollChange(EventArgs args)
        {
            // simply trigger a redraw of the Listbox.
            Invalidate(false);
            return true;
        }

        /// <summary>
        /// handler triggered when vertical scrollbar is shown or hidden
        /// </summary>
        /// <param name="args"></param>
        protected bool HandleVertScrollbarVisibilityChanged(EventArgs args)
        {
            if (d_wordWrap)
                FormatText(false);

            return true;
        }

        /// <summary>
        /// validate window renderer 
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as MultiLineEditboxWindowRenderer) != null;
        }

        /// <summary>
        /// Handler called when the read-only state of the edit box changes
        /// </summary>
        /// <param name="e"></param>
        protected void OnReadOnlyChanged(WindowEventArgs e)
        {
            FireEvent(ReadOnlyModeChanged, e);
        }

        /// <summary>
        /// Handler called when the word wrap mode for the the edit box changes
        /// </summary>
        /// <param name="e"></param>
        protected void OnWordWrapModeChanged(WindowEventArgs e)
        {
            FireEvent(WordWrapModeChanged, e);
        }

        /// <summary>
        /// Handler called when the maximum text length for the edit box changes
        /// </summary>
        /// <param name="e"></param>
        protected void OnMaximumTextLengthChanged(WindowEventArgs e)
        {
            FireEvent(MaximumTextLengthChanged, e);
        }

        /// <summary>
        /// Handler called when the caret moves.
        /// </summary>
        /// <param name="e"></param>
        protected void OnCaretMoved(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(CaretMoved, e);
        }

        /// <summary>
        /// Handler called when the text selection changes
        /// </summary>
        /// <param name="e"></param>
        protected void OnTextSelectionChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(TextSelectionChanged, e);
        }

        /// <summary>
        /// Handler called when the edit box is full
        /// </summary>
        /// <param name="e"></param>
        protected void OnEditboxFullEvent(WindowEventArgs e)
        {
            FireEvent(EditboxFull, e);
        }

        /// <summary>
        /// Handler called when the 'always show' setting for the vertical scroll bar changes.
        /// </summary>
        /// <param name="e"></param>
        protected void OnVertScrollbarModeChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(VertScrollbarModeChanged, e);
        }

        /// <summary>
        /// Handler called when 'always show' setting for the horizontal scroll bar changes.
        /// </summary>
        /// <param name="e"></param>
        protected void OnHorzScrollbarModeChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(HorzScrollbarModeChanged, e);
        }

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorMove(e);

            if (d_dragging)
            {
                SetCaretIndex(GetTextIndexFromPosition(e.Position));
                SetSelection(d_caretPos, d_dragAnchorIdx);
            }

            ++e.handled;
        }

        protected override void OnCaptureLost(WindowEventArgs e)
        {
            d_dragging = false;

            // base class processing
            base.OnCaptureLost(e);

            ++e.handled;
        }

        protected internal override void OnCharacter(TextEventArgs e)
        {
            // NB: We are not calling the base class handler here because it propogates
            // inputs back up the window hierarchy, whereas, as a consumer of key
            // events, we want such propogation to cease with us regardless of whether
            // we actually handle the event.

            // fire event.
            FireEvent(Window.EventCharacterKey, e, EventNamespace);

            // only need to take notice if we have focus
            if (e.handled == 0 && HasInputFocus() && !IsReadOnly() &&
                GetFont().IsCodepointAvailable(e.d_character))
            {
                // erase selected text
                EraseSelectedText();

                // if there is room
                if (GetText().Length - 1 < d_maxTextLen)
                {
                    var newText = GetText();
                    var undo = new UndoHandler.UndoAction
                                   {
                                       d_type = UndoHandler.UndoActionType.UAT_INSERT,
                                       d_startIdx = GetCaretIndex(),
                                       d_text =
                                           e.d_character.ToString(
                                               global::System.Globalization.CultureInfo.InvariantCulture)
                                   };
                    d_undoHandler.AddUndoHistory(undo);
                    newText = newText.Insert(GetCaretIndex(),
                                             e.d_character.ToString(
                                                 global::System.Globalization.CultureInfo.InvariantCulture));
                    SetText(newText);

                    d_caretPos++;

                    OnTextChanged(new WindowEventArgs(this));

                    ++e.handled;
                }
                else
                {
                    // Trigger text box full event
                    OnEditboxFullEvent(new WindowEventArgs(this));
                }
            }
        }

        protected override void OnTextChanged(WindowEventArgs e)
        {
            // ensure last character is a new line
            if ((GetText().Length == 0) || (GetText()[GetText().Length - 1] != '\n'))
            {
                var newText = GetText();
                newText += '\n';
                SetText(newText);
            }


            // base class processing
            base.OnTextChanged(e);

            // clear selection
            ClearSelection();
            // layout new text
            FormatText(true);
            // layout child windows (scrollbars) since text layout may have changed
            PerformChildWindowLayout();
            // ensure caret is still within the text
            SetCaretIndex(GetCaretIndex());
            // ensure caret is visible
            // NB: this will already have been called at least once, but since we
            // may have changed the formatting of the text, it needs to be called again.
            EnsureCaretIsVisible();

            ++e.handled;
        }

        protected override void OnSized(ElementEventArgs e)
        {
            FormatText(true);

            // base class handling
            base.OnSized(e);

            ++e.handled;

        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            // base class processing.
            base.OnScroll(e);

            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

            if (vertScrollbar.IsEffectiveVisible() &&
                (vertScrollbar.GetDocumentSize() > vertScrollbar.GetPageSize()))
            {
                vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition() +
                                                vertScrollbar.GetStepSize()*-e.scroll);
            }
            else if (horzScrollbar.IsEffectiveVisible() &&
                     (horzScrollbar.GetDocumentSize() > horzScrollbar.GetPageSize()))
            {
                horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition() +
                                                horzScrollbar.GetStepSize()*-e.scroll);
            }

            ++e.handled;
        }

        protected internal override void OnFontChanged(WindowEventArgs e)
        {
            base.OnFontChanged(e);
            FormatText(true);
        }

        protected internal override void OnSemanticInputEvent(SemanticEventArgs e)
        {
            if (IsDisabled())
                return;

            if (e.d_semanticValue == SemanticValue.SV_SelectAll && e.d_payload.source == CursorInputSource.Left)
            {
                HandleSelectAllText(e);

                ++e.handled;
            }
            else if (e.d_semanticValue == SemanticValue.SV_SelectWord && e.d_payload.source == CursorInputSource.Left)
            {
                var text = GetText();
                d_dragAnchorIdx = TextUtils.GetWordStartIdx(text,
                                                            d_caretPos == text.Length
                                                                    ? d_caretPos
                                                                    : d_caretPos + 1);
                d_caretPos = TextUtils.GetNextWordStartIdx(text, d_caretPos);

                // perform actual selection operation.
                SetSelection(d_dragAnchorIdx, d_caretPos);

                ++e.handled;
            }

            if (e.handled == 0 && HasInputFocus())
            {
                if (IsReadOnly())
                {
                    base.OnSemanticInputEvent(e);
                    return;
                }

                if (GetSelectionLength() == 0 && IsSelectionSemanticValue(e.d_semanticValue))
                    d_dragAnchorIdx = d_caretPos;

                // Check if the semantic value to be handled is of a general type and can thus be
                // handled via common EditboxBase handlers
                bool isSemanticValueHandled = HandleBasicSemanticValue(e);

                // If the semantic value was not handled, check for specific values
                if (!isSemanticValueHandled)
                {
                    // We assume it will be handled now, if not it will be set to false in default-case
                    isSemanticValueHandled = true;

                    switch (e.d_semanticValue)
                    {

                        case SemanticValue.SV_Confirm:
                            HandleNewLine();
                            break;

                        case SemanticValue.SV_GoUp:
                            HandleLineUp(false);
                            break;

                        case SemanticValue.SV_SelectUp:
                            HandleLineUp(true);
                            break;

                        case SemanticValue.SV_GoDown:
                            HandleLineDown(false);
                            break;

                        case SemanticValue.SV_SelectDown:
                            HandleLineDown(true);
                            break;

                        case SemanticValue.SV_GoToStartOfLine:
                            HandleLineHome(false);
                            break;

                        case SemanticValue.SV_SelectToStartOfLine:
                            HandleLineHome(true);
                            break;

                        case SemanticValue.SV_GoToEndOfLine:
                            HandleLineEnd(false);
                            break;

                        case SemanticValue.SV_SelectToEndOfLine:
                            HandleLineEnd(true);
                            break;

                        case SemanticValue.SV_GoToPreviousPage:
                            HandlePageUp(false);
                            break;

                        case SemanticValue.SV_GoToNextPage:
                            HandlePageDown(false);
                            break;

                        default:
                            base.OnSemanticInputEvent(e);
                            isSemanticValueHandled = false;
                            break;
                    }
                }

                if (isSemanticValueHandled)
                    ++e.handled;
            }
        }

        private void HandleSelectAllText(SemanticEventArgs e)
        {
            var caretLine = GetLineNumberFromIndex(d_caretPos);
            var lineStart = d_lines[caretLine].d_startIdx;

            // find end of last paragraph
            var paraStart = GetText().LastIndexOf(d_lineBreakChars, lineStart);

            // if no previous paragraph, selection will start at the beginning.
            if (paraStart == -1)
                paraStart = 0;
            // find end of this paragraph
            var paraEnd = GetText().IndexOf(d_lineBreakChars, lineStart);

            // if paragraph has no end, which actually should never happen, fix the
            // erroneous situation and select up to end at end of text.
            if (paraEnd == -1)
            {
                var newText = GetText();
                newText.Insert(1, "\n");
                SetText(newText);

                paraEnd = GetText().Length - 1;
            }

            // set up selection using new values.
            d_dragAnchorIdx = paraStart;
            SetCaretIndex(paraEnd);
            SetSelection(d_dragAnchorIdx, d_caretPos);
            ++e.handled;
        }

        #region Fields

        /// <summary>
        /// true if the edit box is in read-only mode
        /// </summary>
        protected bool d_readOnly;

        /// <summary>
        /// Maximum number of characters for this Editbox.
        /// </summary>
        protected int d_maxTextLen;

        /// <summary>
        /// Position of the caret / insert-point.
        /// </summary>
        protected int d_caretPos;

        /// <summary>
        /// Start of selection area.
        /// </summary>
        protected int d_selectionStart;

        /// <summary>
        /// End of selection area.
        /// </summary>
        protected int d_selectionEnd;

        /// <summary>
        /// true when a selection is being dragged.
        /// </summary>
        protected bool d_dragging;

        /// <summary>
        /// Selection index for drag selection anchor point.
        /// </summary>
        protected int d_dragAnchorIdx;

        /// <summary>
        /// Holds what we consider to be line break characters.
        /// </summary>
        protected const string d_lineBreakChars = "\n";

        protected bool		  d_wordWrap;		//!< true when formatting uses word-wrapping.

        // Holds the lines for the current formatting.
        protected List<LineInfo> d_lines=new List<LineInfo>();
        protected float         d_lastRenderWidth;  //!< Holds last render area width
        protected float		  d_widestExtent;	//!< Holds the extent of the widest line as calculated in the last formatting pass.
        protected UndoHandler d_undoHandler;    //!< Undo handler class

        /// <summary>
        /// true if vertical scrollbar should always be displayed
        /// </summary>
        protected bool d_forceVertScroll;

        /// <summary>
        /// true if horizontal scrollbar should always be displayed
        /// </summary>
        protected bool d_forceHorzScroll;

        /// <summary>
        /// Image to use as the selection brush (should be set by derived class).
        /// </summary>
        protected Image d_selectionBrush;

        #endregion

        #region Dynamic Properties

        private void AddMultiLineEditboxProperties()
        {
            DefineProperty(
                "ReadOnly",
                "Property to get/set the read-only setting for the Editbox.  Value is either \"True\" or \"False\".",
                (w, v) => w.SetReadOnly(v), w => w.IsReadOnly(), false);

            DefineProperty(
                "CaretIndex", "Property to get/set the current caret index.  Value is \"[uint]\".",
                (w, v) => w.SetCaretIndex(v), w => w.GetCaretIndex(), 0);

            DefineProperty(
                "SelectionStart",
                "Property to get/set the zero based index of the selection start position within the text.  Value is \"[uint]\".",
                (w, v) => w.SetSelectionStart(v), w => w.GetSelectionStartIndex(), 0);
            DefineProperty(
                "SelectionLength",
                "Property to get/set the length of the selection (as a count of the number of code points selected).  Value is \"[uint]\".",
                (w, v) => w.SetSelectionLength(v), w => w.GetSelectionLength(), 0);

            DefineProperty(
                "MaxTextLength",
                "Property to get/set the the maximum allowed text length (as a count of code points).  Value is \"[uint]\".",
                (w, v) => w.SetMaxTextLength(v), w => w.GetMaxTextLength(), Int32.MaxValue);

            // TODO: Inconsistency
            DefineProperty(
                "WordWrap",
                "Property to get/set the word-wrap setting of the edit box.  Value is either \"True\" or \"False\".",
                (w, v) => w.SetWordWrapping(v), w => w.IsWordWrapped(), true);

            DefineProperty(
                "SelectionBrushImage",
                "Property to get/set the selection brush image for the editbox.  Value should be \"set:[imageset name] image:[image name]\".",
                (w, v) => w.SetSelectionBrushImage(v), w => w.GetSelectionBrushImage(), null);

            // TODO: Inconsistency
            DefineProperty(
                "ForceVertScrollbar",
                "Property to get/set the 'always show' setting for the vertical scroll bar of the list box. Value is either \"True\" or \"False\".",
                (w, v) => w.SetShowVertScrollbar(v), w => w.IsVertScrollbarAlwaysShown(), false);
        }

        private void DefineProperty<T>(string name, string help, Action<MultiLineEditbox, T> setter,
                                       Func<MultiLineEditbox, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<MultiLineEditbox, T>(name, help, setter, getter, WidgetTypeName,
                                                                   defaultValue));
        }

        #endregion
    }
}