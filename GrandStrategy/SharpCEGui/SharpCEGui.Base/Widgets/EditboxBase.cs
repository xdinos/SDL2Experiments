#region Copyright
// Copyright (C) 2004 - 2015 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2015
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

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for an Editbox widget.
    /// </summary>
    public abstract class EditboxBase : Window
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/EditboxBase";

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "EditboxBase";

        /// <summary>
        ///  Event fired when the read-only mode for the edit box is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose read only setting
        /// has been changed.
        /// </summary>
        public const string EventReadOnlyModeChanged = "ReadOnlyModeChanged";

        /// <summary>
        /// Event fired when the masked rendering mode (password mode) is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox that has been put into or
        /// taken out of masked text (password) mode.
        /// </summary>
        public const string EventTextMaskingEnabledChanged = "TextMaskingEnabledChanged";

        /// <summary>
        /// Event fired when the code point (character) used for masked text is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose text masking codepoint
        /// has been changed.
        /// </summary>
        public const string EventTextMaskingCodepointChanged = "TextMaskingCodepointChanged";

        /// <summary>
        /// Event fired when the validation string is changed.
        /// Add a comment to this line
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose validation string has
        /// been changed.
        /// </summary>
        public const string EventValidationStringChanged = "ValidationStringChanged";

        /// <summary>
        /// Event fired when the maximum allowable string length is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose maximum string length
        /// has been changed.
        /// </summary>
        public const string EventMaximumTextLengthChanged = "MaximumTextLengthChanged";

        /// <summary>
        /// Event fired when the validity of the Exitbox text (as determined by a
        /// RegexMatcher object) has changed.
        /// Handlers are passed a const RegexMatchStateEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose text validity has
        /// changed and RegexMatchStateEventArgs::matchState set to the new match
        /// validity. Handler return is significant, as follows:
        /// true indicates the new state - and therfore text - is to be accepted.
        /// false indicates the new state is not acceptable, and the previous text
        /// should remain in place. NB: This is only possible when the validity
        /// change is due to a change in the text, if the validity change is due to
        /// a change in the validation regular expression string, then returning
        /// false will have no effect.
        /// </summary>
        public const string EventTextValidityChanged = "TextValidityChanged";

        /// <summary>
        /// Event fired when the text caret position / insertion point is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose current insertion point
        /// has changed.
        /// </summary>
        public const string EventCaretMoved = "CaretMoved";

        /// <summary>
        /// Event fired when the current text selection is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose current text selection
        /// was changed.
        /// </summary>
        public const string EventTextSelectionChanged = "TextSelectionChanged";

        /// <summary>
        /// Event fired when the number of characters in the edit box reaches the
        /// currently set maximum.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox that has become full.
        /// </summary>
        public const string EventEditboxFull = "EditboxFull";

        /// <summary>
        /// Event fired when the user accepts the current text by pressing Return,
        /// Enter, or Tab.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox in which the user has accepted
        /// the current text.
        /// </summary>
        public const string EventTextAccepted = "TextAccepted";

        /// <summary>
        /// Event fired when the read-only mode for the edit box is changed.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose read only setting
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ReadOnlyModeChanged
        {
            add { SubscribeEvent(EventReadOnlyModeChanged, value); }
            remove { UnsubscribeEvent(EventReadOnlyModeChanged, value); }
        }

        /// <summary>
        /// Event fired when the masked rendering mode (password mode) is changed.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox that has been put into or
        /// taken out of masked text (password) mode.
        /// </summary>
        public event GuiEventHandler<EventArgs> TextMaskingEnabledChanged
        {
            add { SubscribeEvent(EventTextMaskingEnabledChanged, value); }
            remove { UnsubscribeEvent(EventTextMaskingEnabledChanged, value); }
        }

        /// <summary>
        /// Event fired whrn the code point (character) used for masked text is
        /// changed.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose text masking codepoint
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> TextMaskingCodepointChanged
        {
            add { SubscribeEvent(EventTextMaskingCodepointChanged, value); }
            remove { UnsubscribeEvent(EventTextMaskingCodepointChanged, value); }
        }

        /// <summary>
        /// Event fired when the validation string is changed.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose validation string has
        /// been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ValidationStringChanged
        {
            add { SubscribeEvent(EventValidationStringChanged, value); }
            remove { UnsubscribeEvent(EventValidationStringChanged, value); }
        }

        /// <summary>
        /// Event fired when the maximum allowable string length is changed.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose maximum string length
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> MaximumTextLengthChanged
        {
            add { SubscribeEvent(EventMaximumTextLengthChanged, value); }
            remove { UnsubscribeEvent(EventMaximumTextLengthChanged, value); }
        }

        /// <summary>
        /// Event fired when the validity of the Exitbox text (as determined by a
        /// RegexMatcher object) has changed.
        /// Handlers are passed a  RegexMatchStateEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose text validity has
        /// changed and RegexMatchStateEventArgs::matchState set to the new match
        /// validity. Handler return is significant, as follows:
        ///     - true indicates the new state - and therfore text - is to be accepted.
        ///     - false indicates the new state is not acceptable, and the previous text
        ///       should remain in place. NB: This is only possible when the validity
        ///       change is due to a change in the text, if the validity change is due to
        ///       a change in the validation regular expression string, then returning
        ///       false will have no effect.
        /// </summary>
        public event GuiEventHandler<EventArgs> TextValidityChanged
        {
            add { SubscribeEvent(EventTextValidityChanged, value); }
            remove { UnsubscribeEvent(EventTextValidityChanged, value); }
        }

        /// <summary>
        /// Event fired when the text caret position / insertion point is changed.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose current insertion point
        /// has changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> CaretMoved
        {
            add { SubscribeEvent(EventCaretMoved, value); }
            remove { UnsubscribeEvent(EventCaretMoved, value); }
        }

        /// <summary>
        /// Event fired when the current text selection is changed.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose current text selection
        /// was changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> TextSelectionChanged
        {
            add { SubscribeEvent(EventTextSelectionChanged, value); }
            remove { UnsubscribeEvent(EventTextSelectionChanged, value); }
        }

        /// <summary>
        /// Event fired when the number of characters in the edit box reaches the
        /// currently set maximum.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox that has become full.
        /// </summary>
        public event GuiEventHandler<EventArgs> EditboxFull
        {
            add { SubscribeEvent(EventEditboxFull, value); }
            remove { UnsubscribeEvent(EventEditboxFull, value); }
        }

        /// <summary>
        /// Event fired when the user accepts the current text by pressing Return,
        /// Enter, or Tab.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox in which the user has accepted
        /// the current text.
        /// </summary>
        public event GuiEventHandler<EventArgs> TextAccepted
        {
            add { SubscribeEvent(EventTextAccepted, value); }
            remove { UnsubscribeEvent(EventTextAccepted, value); }
        }

        #endregion

        /// <summary>
        /// Mouse cursor image property name to use when the edit box is
        /// in read-only mode.
        /// </summary>
        public const string ReadOnlyCursorImagePropertyName = "ReadOnlyCursorImage";

        protected EditboxBase(string type, string name)
                : base(type, name)
        {
            _readOnly = false;
            _textMaskingEnabled = false;
            _maskCodePoint = '*';
            _maxTextLen = Int32.MaxValue;
            _caretPos = 0;
            _selectionStart = 0;
            _selectionEnd = 0;
            _dragging = false;

            AddEditboxBaseProperties();

            // override default and disable text parsing
            d_textParsingEnabled = false;
            // ban the property too, since this being off is not optional.
            BanPropertyFromXML("TextParsingEnabled");

            _undoHandler = new UndoHandler(this);
        }

        /// <summary>
        /// return true if the Editbox has input focus.
        /// </summary>
        /// <returns>
        /// - true if the Editbox has keyboard input focus.
        /// - false if the Editbox does not have keyboard input focus.
        /// </returns>
        public virtual bool HasInputFocus()
        {
            return IsActive();
        }

        /// <summary>
        /// return true if the Editbox is read-only.
        /// </summary>
        /// <returns>
        /// true if the Editbox is read only and can't be edited by the user, false
        /// if the Editbox is not read only and may be edited by the user.
        /// </returns>
        public bool IsReadOnly()
        {
            return _readOnly;
        }

        /// <summary>
        /// return true if the text for the Editbox will be rendered masked.
        /// </summary>
        /// <returns>
        /// true if the Editbox text will be rendered masked using the currently set
        /// mask code point, false if the Editbox text will be rendered as ordinary
        /// text.
        /// </returns>
        public bool IsTextMaskingEnabled()
        {
            return _textMaskingEnabled;
        }

        /// <summary>
        /// return the current position of the caret.
        /// </summary>
        /// <returns>
        /// Index of the insert caret relative to the start of the text.
        /// </returns>
        public int GetCaretIndex()
        {
#if CEGUI_BIDI_SUPPORT
            var caretPos = _caretPos;
            if (d_bidiVisualMapping.GetV2lMapping().Count > caretPos)
                _caretPos = d_bidiVisualMapping.GetV2lMapping()[caretPos];
#endif
            return _caretPos;
        }

        /// <summary>
        /// return the current selection start point.
        /// </summary>
        /// <returns>
        /// Index of the selection start point relative to the start of the text.
        /// If no selection is defined this function returns the position of the
        /// caret.
        /// </returns>
        public int GetSelectionStart()
        {
            return (_selectionStart != _selectionEnd) ? _selectionStart : _caretPos;
        }

        /// <summary>
        /// return the current selection end point.
        /// </summary>
        /// <returns>
        /// Index of the selection end point relative to the start of the text.  If
        /// no selection is defined this function returns the position of the caret.
        /// </returns>
        public int GetSelectionEnd()
        {
            return (_selectionStart != _selectionEnd) ? _selectionEnd : _caretPos;
        }

        /// <summary>
        /// return the length of the current selection (in code points /
        /// characters).
        /// </summary>
        /// <returns>
        /// Number of code points (or characters) contained within the currently
        /// defined selection.
        /// </returns>
        public int GetSelectionLength()
        {
            return _selectionEnd - _selectionStart;
        }

        /// <summary>
        /// return the code point used when rendering masked text.
        /// </summary>
        /// <returns>
        /// utf32 or char (depends on used String class) code point value representing
        /// the Unicode code point that will be rendered instead of the Editbox text
        /// when rendering in masked mode.
        /// </returns>
        public char GetTextMaskCodePoint()
        {
            return _maskCodePoint;
        }

        /// <summary>
        /// return the maximum text length set for this Editbox.
        /// </summary>
        /// <returns>
        /// The maximum number of code points (characters) that can be entered into
        /// this Editbox.
        /// </returns>
        /// <remarks>
        /// Depending on the validation string set, the actual length of text that
        /// can be entered may be less than the value returned here
        /// (it will never be more).
        /// </remarks>
        public int GetMaxTextLength()
        {
            return _maxTextLen;
        }

        /// <summary>
        /// Specify whether the Editbox is read-only.
        /// </summary>
        /// <param name="setting">
        /// true if the Editbox is read only and can't be edited by the user, false
        /// if the Editbox is not read only and may be edited by the user.
        /// </param>
        public void SetReadOnly(bool setting)
        {
            // if setting is changed
            if (_readOnly != setting)
            {
                _readOnly = setting;
                OnReadOnlyChanged(new WindowEventArgs(this));

                // Update the cursor according to the read only state.
                if (_readOnly)
                    SetCursor(d_readOnlyCursorImage);
                else
                    SetCursor(GetProperty<Image>(CursorImagePropertyName));
            }
        }

        /// <summary>
        /// Specify whether the text for the Editbox will be rendered masked.
        /// </summary>
        /// <param name="setting">
        /// - true if the Editbox text should be rendered masked using the currently
        ///   set mask code point.
        /// - false if the Editbox text should be rendered as ordinary text.
        /// </param>
        public void SetTextMaskingEnabled(bool setting)
        {
            // if setting is changed
            if (_textMaskingEnabled != setting)
            {
                _textMaskingEnabled = setting;
                OnTextMaskingEnabledChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set the current position of the caret.
        /// </summary>
        /// <param name="caretPos">
        /// New index for the insert caret relative to the start of the text.  If
        /// the value specified is greater than the number of characters in the
        /// Editbox, the caret is positioned at the end of the text.
        /// </param>
        public void SetCaretIndex(int caretPos)
        {
            // make sure new position is valid
            if (caretPos > GetText().Length)
                caretPos = GetText().Length;

            // if new position is different
            if (_caretPos != caretPos)
            {
                _caretPos = caretPos;

                // Trigger "caret moved" event
                OnCaretMoved(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Define the current selection for the Editbox
        /// </summary>
        /// <param name="startPos">
        /// Index of the starting point for the selection.  If this value is greater
        /// than the number of characters in the Editbox, the selection start will
        /// be set to the end of the text.
        /// </param>
        /// <param name="endPos">
        /// Index of the ending point for the selection.  If this value is greater
        /// than the number of characters in the Editbox, the selection end will be
        /// set to the end of the text.
        /// </param>
        public void SetSelection(int startPos, int endPos)
        {
            // ensure selection start point is within the valid range
            if (startPos > GetText().Length)
                startPos = GetText().Length;

            // ensure selection end point is within the valid range
            if (endPos > GetText().Length)
                endPos = GetText().Length;

            // ensure start is before end
            if (startPos > endPos)
            {
                var tmp = endPos;
                endPos = startPos;
                startPos = tmp;
            }

            // only change state if values are different.
            if ((startPos != _selectionStart) || (endPos != _selectionEnd))
            {
                // setup selection
                _selectionStart = startPos;
                _selectionEnd = endPos;

                // Trigger "selection changed" event
                OnTextSelectionChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Define the current selection start for the Editbox
        /// </summary>
        /// <param name="startPos">
        /// Index of the starting point for the selection.  
        /// If this value is greater than the number of characters in the Editbox, 
        /// the selection start will be set to the end of the text.
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
            SetSelection(GetSelectionStart(), GetSelectionStart() + length);
        }

        /// <summary>
        /// set the code point used when rendering masked text.
        /// </summary>
        /// <param name="codePoint">
        /// utf32 or char (depends on used String class) code point value representing
        /// the vode point that should be rendered instead of the Editbox text when
        /// rendering in masked mode.
        /// </param>
        public void SetTextMaskCodePoint(char codePoint)
        {
            if (codePoint != _maskCodePoint)
            {
                _maskCodePoint = codePoint;

                // Trigger "mask code point changed" event
                OnTextMaskingCodepointChanged(new WindowEventArgs(this));
            }
        }
        
        /// <summary>
        /// set the maximum text length for this Editbox.
        /// </summary>
        /// <param name="maxLenght">
        /// The maximum number of code points (characters) that can be entered into
        /// this Editbox.
        /// </param>
        /// <remarks>
        /// Depending on the validation string set, the actual length of text that
        /// can be entered may be less than the value set here
        /// (it will never be more).
        /// </remarks>
        public abstract void SetMaxTextLength(int maxLenght);

        public override void SetEnabled(bool value)
        {
            base.SetEnabled(value);

            // Update the mouse cursor according to the read only state.
            if (value)
                SetCursor(GetProperty<Image>(CursorImagePropertyName));
            else
                SetCursor(d_readOnlyCursorImage);
        }
        
        public override bool PerformCopy(Clipboard clipboard)
        {
            if (GetSelectionLength() == 0)
                return false;

            var selectedText = GetText().CEGuiSubstring(GetSelectionStart(), GetSelectionLength());

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

        public override bool PerformUndo()
        {
            var result = false;
            if (!IsReadOnly())
            {
                ClearSelection();
                result = _undoHandler.Undo(ref _caretPos);
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
                result = _undoHandler.Redo(ref _caretPos);
                OnTextChanged(new WindowEventArgs(this));
            }

            return result;
        }

        public bool HandleBasicSemanticValue(SemanticEventArgs e)
        {
            switch (e.d_semanticValue)
            {
                case SemanticValue.SV_DeletePreviousCharacter:
                    HandleBackspace();
                    break;

                case SemanticValue.SV_DeleteNextCharacter:
                    HandleDelete();
                    break;
                case SemanticValue.SV_GoToPreviousCharacter:
                    HandleCharLeft(false);
                    break;
                case SemanticValue.SV_GoToNextCharacter:
                    HandleCharRight(false);
                    break;
                case SemanticValue.SV_SelectPreviousCharacter:
                    HandleCharLeft(true);
                    break;

                case SemanticValue.SV_SelectNextCharacter:
                    HandleCharRight(true);
                    break;

                case SemanticValue.SV_GoToPreviousWord:
                    HandleWordLeft(false);
                    break;

                case SemanticValue.SV_GoToNextWord:
                    HandleWordRight(false);
                    break;

                case SemanticValue.SV_SelectPreviousWord:
                    HandleWordLeft(true);
                    break;

                case SemanticValue.SV_SelectNextWord:
                    HandleWordRight(true);
                    break;

                case SemanticValue.SV_GoToStartOfDocument:
                    HandleHome(false);
                    break;

                case SemanticValue.SV_GoToEndOfDocument:
                    HandleEnd(false);
                    break;

                case SemanticValue.SV_SelectToStartOfDocument:
                    HandleHome(true);
                    break;

                case SemanticValue.SV_SelectToEndOfDocument:
                    HandleEnd(true);
                    break;

                case SemanticValue.SV_SelectAll:
                    HandleSelectAll();
                    break;

                default:
                    return false;
            }

            return true;
        }
        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // base class handling
            base.OnCursorPressHold(e);

            if (e.Source == CursorInputSource.Left)
            {
                // grab inputs
                if (CaptureInput())
                {
                    // handle cursor press
                    ClearSelection();
                    _dragging = true;
                    _dragAnchorIdx = GetTextIndexFromPosition(e.Position);
#if CEGUI_BIDI_SUPPORT
                    if (d_bidiVisualMapping.GetV2lMapping().Count > _dragAnchorIdx)
                        _dragAnchorIdx = d_bidiVisualMapping.GetV2lMapping()[_dragAnchorIdx];
#endif
                    SetCaretIndex(_dragAnchorIdx);
                }

                ++e.handled;
            }
        }
        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorActivate(e);

            if (e.Source == CursorInputSource.Left)
            {
                ReleaseInput();
                ++e.handled;
            }
        }
        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorMove(e);

            if (_dragging)
            {
                var anchorIdx = GetTextIndexFromPosition(e.Position);
#if CEGUI_BIDI_SUPPORT
                if (d_bidiVisualMapping.GetV2lMapping().Count > _dragAnchorIdx)
                    _dragAnchorIdx = d_bidiVisualMapping.GetV2lMapping()[_dragAnchorIdx];
#endif
                SetCaretIndex(anchorIdx);

                SetSelection(_caretPos, _dragAnchorIdx);
            }

            ++e.handled;
        }

        protected override void OnCaptureLost(WindowEventArgs e)
        {
            _dragging = false;

            // base class processing
            base.OnCaptureLost(e);

            ++e.handled;
        }

        /// <summary>
        /// Processing to move caret one character left
        /// </summary>
        /// <param name="select"></param>
        protected void HandleCharLeft(bool select)
        {
            if (_caretPos > 0)
                SetCaretIndex(_caretPos - 1);

            if (select)
                SetSelection(_caretPos, _dragAnchorIdx);
            else
                ClearSelection();
        }

        /// <summary>
        /// Processing to move caret one word left
        /// </summary>
        /// <param name="select"></param>
        protected void HandleWordLeft(bool select)
        {
            if (_caretPos > 0)
                SetCaretIndex(TextUtils.GetWordStartIdx(GetText(), GetCaretIndex()));

            if (select)
                SetSelection(_caretPos, _dragAnchorIdx);
            else
                ClearSelection();
        }

        /// <summary>
        /// Processing to move caret one character right
        /// </summary>
        /// <param name="select"></param>
        protected void HandleCharRight(bool select)
        {
            if (_caretPos < GetText().Length)
                SetCaretIndex(_caretPos + 1);

            if (select)
                SetSelection(_caretPos, _dragAnchorIdx);
            else
                ClearSelection();
        }

        /// <summary>
        /// Processing to move caret one word right
        /// </summary>
        /// <param name="select"></param>
        protected void HandleWordRight(bool select)
        {
            if (_caretPos < GetText().Length)
                SetCaretIndex(TextUtils.GetNextWordStartIdx(GetText(), _caretPos));

            if (select)
                SetSelection(_caretPos, _dragAnchorIdx);
            else
                ClearSelection();
        }

        /// <summary>
        /// Processing to move caret to the start of the text.
        /// </summary>
        /// <param name="select"></param>
        protected void HandleHome(bool select)
        {
            if (_caretPos > 0)
                SetCaretIndex(0);

            if (select)
                SetSelection(_caretPos, _dragAnchorIdx);
            else
                ClearSelection();
        }

        /// <summary>
        /// Processing to move caret to the end of the text
        /// </summary>
        /// <param name="select"></param>
        protected void HandleEnd(bool select)
        {
            if (_caretPos < GetText().Length)
                SetCaretIndex(GetText().Length);

            if (select)
                SetSelection(_caretPos, _dragAnchorIdx);
            else
                ClearSelection();
        }

        /// <summary>
        /// Selects the entire text.
        /// </summary>
        protected void HandleSelectAll()
        {
            SetSelection(0, GetText().Length);
            SetCaretIndex(GetText().Length);
        }

        /// <summary>
        /// validate window renderer
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as EditboxWindowRenderer) != null;
        }

        /// <summary>
        ///  Return the text code point index that is rendered closest to screen
        /// position \a pt.
        /// </summary>
        /// <param name="pt">
        /// Point object describing a position on the screen in pixels.
        /// </param>
        /// <returns>
        /// Code point index into the text that is rendered closest to screen
        /// position \a pt.
        /// </returns>
        protected abstract int GetTextIndexFromPosition(Lunatics.Mathematics.Vector2 pt);

        /// <summary>
        /// Erase the currently selected text.
        /// </summary>
        /// <param name="modifyText">
        /// when true, the actual text will be modified.  When false, everything is
        /// done except erasing the characters.
        /// </param>
        protected abstract void EraseSelectedText(bool modifyText = true);

        protected abstract void HandleBackspace();

        protected abstract void HandleDelete();
        
        /// <summary>
        /// Clear the currently defined selection (just the region, not the text).
        /// </summary>
        protected void ClearSelection()
        {
            // perform action only if required.
            if (GetSelectionLength() != 0)
                SetSelection(0, 0);
        }

        /// <summary>
        /// Handler called when the read only state of the Editbox has been changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnReadOnlyChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(EventReadOnlyModeChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when the masked rendering mode (password mode) has been
        /// changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextMaskingEnabledChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(EventTextMaskingEnabledChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when the code point to use for masked rendering has been
        /// changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextMaskingCodepointChanged(WindowEventArgs e)
        {
            // if we are in masked mode, trigger a GUI redraw.
            if (IsTextMaskingEnabled())
                Invalidate(false);

            FireEvent(EventTextMaskingCodepointChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when the maximum text length for the edit box is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMaximumTextLengthChanged(WindowEventArgs e)
        {
            FireEvent(EventMaximumTextLengthChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when the caret (insert point) position changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCaretMoved(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(EventCaretMoved, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when the current text selection changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextSelectionChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(EventTextSelectionChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when the edit box text has reached the set maximum
        /// length.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEditboxFullEvent(WindowEventArgs e)
        {
            FireEvent(EventEditboxFull, e, EventNamespace);
        }

        /// <summary>
        /// return the the read-only mouse cursor image.
        /// </summary>
        /// <returns>
        /// The read-only mouse cursor image.
        /// </returns>
        protected Image GetReadOnlyCursorImage()
        {
            return d_readOnlyCursorImage;
        }

        /// <summary>
        /// Set the read only mouse cursor image.
        /// </summary>
        /// <param name="image">
        /// The Image to be used.
        /// </param>
        protected void SetReadOnlyCursorImage(Image image)
        {
            d_readOnlyCursorImage = image;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool IsSelectionSemanticValue(SemanticValue value)
        {
            return (value >= SemanticValue.SV_SelectRange && value <= SemanticValue.SV_SelectToEndOfLine) ||
                   (value >= SemanticValue.SV_SelectToStartOfDocument && value <= SemanticValue.SV_SelectToPreviousPage);
        }

        private void AddEditboxBaseProperties()
        {
            DefineProperty(
                    "ReadOnly",
                    "Property to get/set the read-only setting for the Editbox.  Value is either \"True\" or \"False\".",
                    (w, v) => w.SetReadOnly(v), w => w.IsReadOnly(), false);

            // TODO: Inconsistency
            DefineProperty(
                    "MaskText",
                    "Property to get/set the mask text setting for the Editbox.  Value is either \"True\" or \"False\".",
                    (w, v) => w.SetTextMaskingEnabled(v), w => w.IsTextMaskingEnabled(), false);

            DefineProperty(
                    "MaskCodepoint",
                    "Property to get/set the utf32 codepoint value used for masking text. Value is either \"[uint]\" (number = codepoint) if CEGUI is compiled with utf32 string or \"[char]\" (just the symbol) if CEGUI is compiled with std::string.",
                    (w, v) => w.SetTextMaskCodePoint(v), w => w.GetTextMaskCodePoint(), (char) 42);

            DefineProperty(
                    "CaretIndex", "Property to get/set the current caret index.  Value is \"[uint]\".",
                    (w, v) => w.SetCaretIndex(v), w => w.GetCaretIndex(), 0);

            DefineProperty(
                    "SelectionStart",
                    "Property to get/set the zero based index of the selection start position within the text.  Value is \"[uint]\".",
                    (w, v) => w.SetSelectionStart(v), w => w.GetSelectionStart(), 0);

            DefineProperty(
                    "SelectionLength",
                    "Property to get/set the length of the selection (as a count of the number of code points selected). Value is \"[uint]\".",
                    (w, v) => w.SetSelectionLength(v), w => w.GetSelectionLength(), 0);

            DefineProperty(
                    "MaxTextLength",
                    "Property to get/set the the maximum allowed text length (as a count of code points). Value is \"[uint]\".",
                    (w, v) => w.SetMaxTextLength(v), w => w.GetMaxTextLength(), Int32.MaxValue);

            DefineProperty<Image>(
                    "ReadOnlyCursorImage",
                    "Property to get/set the mouse cursor image for the EditBox when in Read-only mode.  Value should be \"imageset/image_name\".  Value is the image to use.",
                    (w, v) => w.SetReadOnlyCursorImage(v), w => w.GetReadOnlyCursorImage(), null);
        }

        private void DefineProperty<T>(string name, string help, Action<EditboxBase, T> setter,
                                       Func<EditboxBase, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<EditboxBase, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #region Fields

        /// <summary>
        /// True if the editbox is in read-only mode
        /// </summary>
        protected bool _readOnly;

        /// <summary>
        /// The read only mouse cursor image.
        /// </summary>
        protected Image d_readOnlyCursorImage;

        /// <summary>
        /// True if the editbox text should be rendered masked.
        /// </summary>
        protected bool _textMaskingEnabled;

        /// <summary>
        /// Code point to use when rendering masked text.
        /// </summary>
        protected char _maskCodePoint;

        /// <summary>
        /// Maximum number of characters for this Editbox.
        /// </summary>
        protected int _maxTextLen;

        /// <summary>
        /// Position of the caret / insert-point.
        /// </summary>
        protected int _caretPos;

        /// <summary>
        /// Start of selection area.
        /// </summary>
        protected int _selectionStart;

        /// <summary>
        /// End of selection area.
        /// </summary>
        protected int _selectionEnd;

        /// <summary>
        /// true when a selection is being dragged.
        /// </summary>
        protected bool _dragging;

        /// <summary>
        /// Selection index for drag selection anchor point.
        /// </summary>
        protected int _dragAnchorIdx;

        /// <summary>
        /// 
        /// </summary>
        protected readonly UndoHandler _undoHandler;

        #endregion
    }
}