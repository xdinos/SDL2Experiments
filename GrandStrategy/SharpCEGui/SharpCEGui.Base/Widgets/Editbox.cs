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

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for the EditboxWindowRenderer class
    /// </summary>
    public abstract class EditboxWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected EditboxWindowRenderer(string name)
                : base(name, Editbox.EventNamespace)
        {
        }

        /// <summary>
        /// Return the text code point index that is rendered closest to screen
        /// position \a pt.
        /// </summary>
        /// <param name="pt">
        /// Point object describing a position on the screen in pixels.
        /// </param>
        /// <returns>
        /// Code point index into the text that is rendered closest to screen
        /// position \a pt.
        /// </returns>
        public abstract int GetTextIndexFromPosition(Lunatics.Mathematics.Vector2 pt);
    }

    /// <summary>
    /// Class for an Editbox widget.
    /// </summary>
    public class Editbox : EditboxBase
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public new const string WidgetTypeName = "CEGUI/Editbox";

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Editbox";

        /// <summary>
        /// Event fired when the validation string is changed.
        /// Add a comment to this line
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose validation string has
        /// been changed.
        /// </summary>
        public new const string EventValidationStringChanged = "ValidationStringChanged";

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
        public new const string EventTextValidityChanged = "TextValidityChanged";

        /// <summary>
        /// Event fired when the validation string is changed.
        /// Handlers are passed a  WindowEventArgs reference with
        /// WindowEventArgs::window set to the Editbox whose validation string has
        /// been changed.
        /// </summary>
        public new event GuiEventHandler<EventArgs> ValidationStringChanged
        {
            add { SubscribeEvent(EventValidationStringChanged, value); }
            remove { UnsubscribeEvent(EventValidationStringChanged, value); }
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
        public new event GuiEventHandler<EventArgs> TextValidityChanged
        {
            add { SubscribeEvent(EventTextValidityChanged, value); }
            remove { UnsubscribeEvent(EventTextValidityChanged, value); }
        }

        #endregion
        
        /// <summary>
        /// return the validation MatchState for the current Editbox text, given the
        /// currently set validation string.
        /// </summary>
        /// <returns>
        /// One of the MatchState enumerated values indicating the current match state.
        /// </returns>
        /// <remarks>
        /// Validation is performed by means of a regular expression.  If the text
        /// matches the regex, the text is said to have passed validation.  If the
        /// text does not match with the regex then the text fails validation.
        /// </remarks>
        public RegexMatcher.MatchState GetTextMatchState()
        {
            return _validatorMatchState;
        }

        /// <summary>
        /// return the currently set validation string
        /// </summary>
        /// <returns>
        /// String object containing the current validation regex data
        /// </returns>
        /// <remarks>
        /// Validation is performed by means of a regular expression.  If the text
        /// matches the regex, the text is said to have passed validation.  If the
        /// text does not match with the regex then the text fails validation.
        /// </remarks>
        public string GetValidationString()
        {
            return _validationString;
        }

        /// <summary>
        /// Set the text validation string.
        /// </summary>
        /// <param name="validationString">
        /// String object containing the validation regex data to be used.
        /// </param>
        /// <remarks>
        /// Validation is performed by means of a regular expression.  If the text
        /// matches the regex, the text is said to have passed validation.  If the
        /// text does not match with the regex then the text fails validation.
        /// </remarks>
        public void SetValidationString(string validationString)
        {
            if (validationString == _validationString)
                return;

            if (_validator == null)
                throw new InvalidRequestException("Unable to set validation string on Editbox '" + GetNamePath() +
                                                  "' because it does not currently have a RegexMatcher validator.");

            _validationString = validationString;
            _validator.SetRegexString(validationString);

            // notification
            OnValidationStringChanged(new WindowEventArgs(this));

            HandleValidityChangeForString(GetText());
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
        public override void SetMaxTextLength(int maxLenght)
        {
            if (_maxTextLen != maxLenght)
            {
                _maxTextLen = maxLenght;

                // Trigger max length changed event
                var args = new WindowEventArgs(this);
                OnMaximumTextLengthChanged(args);

                // trim string
                if (GetText().Length > _maxTextLen)
                {

                    var newText = GetText();
                    // TODO: ... hmmm
                    //newText.resize(d_maxTextLen);
                    SetText(newText);
                    OnTextChanged(args);
                    _undoHandler.ClearUndoHistory();

                    var state = GetStringMatchState(GetText());
                    if (_validatorMatchState != state)
                    {
                        OnTextValidityChanged(new RegexMatchStateEventArgs(this, state));
                        _validatorMatchState = state;
                    }
                }
            }
        }

        /// <summary>
        /// Set the RegexMatcher based validator for this Editbox.
        /// </summary>
        /// <param name="validator">
        /// Pointer to an object that implements the RegexMatcher interface, or 0
        /// to restore a system supplied RegexMatcher (if support is available).
        /// </param>
        /// <remarks>
        /// If the previous RegexMatcher validator is one supplied via the system,
        /// it is deleted and replaced with the given RegexMatcher.  User supplied
        /// RegexMatcher objects will never be deleted by the system and you must
        /// ensure that the object is not deleted while the Editbox holds a pointer
        /// to it.  Once the Editbox is destroyed or the validator is set to
        /// something else it is the responsibility of client code to ensure any
        /// previous custom validator is deleted.
        /// </remarks>
        public void SetValidator(RegexMatcher validator)
        {
            if (_weOwnValidator && _validator != null)
                System.GetSingleton().DestroyRegexMatcher(_validator);

            _validator = validator;

            if (_validator != null)
                _weOwnValidator = false;
            else
            {
                _validator = System.GetSingleton().CreateRegexMatcher();
                _weOwnValidator = true;
            }
        }

        

        public override bool PerformPaste(Clipboard clipboard)
        {
            if (IsReadOnly())
                return false;

            var clipboardText = clipboard.GetText();

            if (String.IsNullOrEmpty(clipboardText))
                return false;

            // backup current text
            var tmp = GetText();

            var undoSelection = new UndoHandler.UndoAction
                                {
                                        d_type = UndoHandler.UndoActionType.UAT_DELETE,
                                        d_startIdx = GetSelectionStart(),
                                        d_text = tmp.CEGuiSubstring(GetSelectionStart(), GetSelectionLength())
                                };

            tmp = tmp.Remove(GetSelectionStart(), GetSelectionLength());


            // if there is room
            if (tmp.Length < _maxTextLen)
            {
                var undo = new UndoHandler.UndoAction
                           {
                                   d_type = UndoHandler.UndoActionType.UAT_INSERT,
                                   d_startIdx = GetCaretIndex(),
                                   d_text = clipboardText
                           };

                tmp = tmp.Insert(GetSelectionStart(), clipboardText);

                if (HandleValidityChangeForString(tmp))
                {
                    // erase selection using mode that does not modify getText()
                    // (we just want to update state)
                    EraseSelectedText(false);

                    // advance caret (done first so we can "do stuff" in event
                    // handlers!)
                    _caretPos += clipboardText.Length;

                    // set text to the newly modified string
                    SetText(tmp);

                    _undoHandler.AddUndoHistory(undo);
                    if (GetSelectionLength() > 0)
                        _undoHandler.AddUndoHistory(undoSelection);

                    return true;
                }
            }

            return false;
        }

	    /// <summary>
	    /// Constructor for Editbox class.
	    /// </summary>
	    /// <param name="type"></param>
	    /// <param name="name"></param>
	    public Editbox(string type, string name)
		    : base(type, name)
	    {
		    _validator = System.GetSingleton().CreateRegexMatcher();
		    _weOwnValidator = true;

		    _validatorMatchState = RegexMatcher.MatchState.MS_VALID;
		    _previousValidityChangeResponse = true;

		    AddEditboxProperties();

		    // default to accepting all characters
		    if (_validator != null)
			    SetValidationString(".*");
		    // set copy of validation string to ".*" so getter returns something valid.
		    else
			    _validationString = ".*";

	    }

	    protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_weOwnValidator && _validator != null)
                    System.GetSingleton().DestroyRegexMatcher(_validator);
            }
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
        protected override int GetTextIndexFromPosition(Lunatics.Mathematics.Vector2 pt)
        {
            if (d_windowRenderer != null)
            {
                var wr = (EditboxWindowRenderer) d_windowRenderer;
                return wr.GetTextIndexFromPosition(pt);
            }

            throw new InvalidRequestException("This function must be implemented by the window renderer");
        }

        
       

        /// <summary>
        /// return the match state of the given string for the validation regular
        /// expression.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected RegexMatcher.MatchState GetStringMatchState(string str)
        {
            return _validator?.GetMatchStateOfString(str) ?? RegexMatcher.MatchState.MS_VALID;
        }

        /// <summary>
        /// Helper to update validator match state as needed for the given string
        /// and event handler return codes.
        /// 
        /// This effectively asks permission from event handlers to proceed with the
        /// change, updates d_validatorMatchState and returns an appropriate bool.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected bool HandleValidityChangeForString(string str)
        {
            var newState = GetStringMatchState(str);

            if (newState == _validatorMatchState)
                return _previousValidityChangeResponse;

            var args = new RegexMatchStateEventArgs(this, newState);
            OnTextValidityChanged(args);

            var response = (args.handled != 0);
            if (response)
            {
                _validatorMatchState = newState;
                _previousValidityChangeResponse = true;
            }

            return response;
        }

        /// <summary>
        /// Processing for backspace key
        /// </summary>
        protected override void HandleBackspace()
        {
            if (!IsReadOnly())
            {
                var tmp = GetText();

                if (GetSelectionLength() != 0)
                {
                    var undoSelection = new UndoHandler.UndoAction
                                        {
                                                d_type = UndoHandler.UndoActionType.UAT_DELETE,
                                                d_startIdx = GetSelectionStart(),
                                                d_text = tmp.CEGuiSubstring(GetSelectionStart(), GetSelectionLength())
                                        };

                    tmp = tmp.Remove(GetSelectionStart(), GetSelectionLength());

                    if (HandleValidityChangeForString(tmp))
                    {
                        // erase selection using mode that does not modify getText()
                        // (we just want to update state)
                        EraseSelectedText(false);

                        // set text to the newly modified string
                        SetText(tmp);
                        _undoHandler.AddUndoHistory(undoSelection);
                    }
                }
                else if (GetCaretIndex() > 0)
                {
                    var undo = new UndoHandler.UndoAction
                               {
                                       d_type = UndoHandler.UndoActionType.UAT_DELETE,
                                       d_startIdx = _caretPos - 1,
                                       d_text = tmp.CEGuiSubstring(_caretPos - 1, 1)
                               };

                    tmp = tmp.Remove(_caretPos - 1, 1);

                    if (HandleValidityChangeForString(tmp))
                    {
                        SetCaretIndex(_caretPos - 1);

                        // set text to the newly modified string
                        SetText(tmp);
                        _undoHandler.AddUndoHistory(undo);
                    }
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
                var tmp = GetText();

                if (GetSelectionLength() != 0)
                {
                    var undoSelection = new UndoHandler.UndoAction
                                        {
                                                d_type = UndoHandler.UndoActionType.UAT_DELETE,
                                                d_startIdx = GetSelectionStart(),
                                                d_text =
                                                        tmp.CEGuiSubstring(GetSelectionStart(), GetSelectionLength())
                                        };

                    tmp = tmp.Remove(GetSelectionStart(), GetSelectionLength());

                    if (HandleValidityChangeForString(tmp))
                    {
                        // erase selection using mode that does not modify getText()
                        // (we just want to update state)
                        EraseSelectedText(false);

                        // set text to the newly modified string
                        SetText(tmp);
                        _undoHandler.AddUndoHistory(undoSelection);
                    }
                }
                else if (GetCaretIndex() < tmp.Length)
                {
                    var undo = new UndoHandler.UndoAction
                               {
                                       d_type = UndoHandler.UndoActionType.UAT_DELETE,
                                       d_startIdx = _caretPos,
                                       d_text = tmp.CEGuiSubstring(_caretPos, 1)
                               };

                    tmp = tmp.Remove(_caretPos, 1);

                    if (HandleValidityChangeForString(tmp))
                    {
                        // set text to the newly modified string
                        SetText(tmp);
                        _undoHandler.AddUndoHistory(undo);
                    }
                }

            }
        }

        protected override void EraseSelectedText(bool modifyText = true)
        {
            if (GetSelectionLength() != 0)
            {
                // setup new caret position and remove selection highlight.
                SetCaretIndex(_selectionStart);
                ClearSelection();

                // erase the selected characters (if required)
                if (modifyText)
                {
                    var newText = GetText();
                    var undo = new UndoHandler.UndoAction
                    {
                        d_type = UndoHandler.UndoActionType.UAT_DELETE,
                        d_startIdx = GetSelectionStart(),
                        d_text =
                                newText.CEGuiSubstring(GetSelectionStart(),
                                                       GetSelectionLength())
                    };
                    _undoHandler.AddUndoHistory(undo);

                    newText = GetText().Remove(GetSelectionStart(), GetSelectionLength());
                    SetText(newText);

                    // trigger notification that text has changed.
                    OnTextChanged(new WindowEventArgs(this));
                }

            }
        }


        /// <summary>
        /// Event fired internally when the validation string is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValidationStringChanged(WindowEventArgs e)
        {
            FireEvent(EventValidationStringChanged, e, EventNamespace);
        }
        
        /// <summary>
        /// Handler called when something has caused the validity state of the
        /// current text to change.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextValidityChanged(RegexMatchStateEventArgs e)
        {
            FireEvent(EventTextValidityChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when the user accepts the edit box text by pressing
        /// Return, Enter, or Tab.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextAcceptedEvent(WindowEventArgs e)
        {
            FireEvent(EventTextAccepted, e, EventNamespace);
        }

        // Overridden event handlers

        protected internal override void OnSemanticInputEvent(SemanticEventArgs e)
        {
            if (IsDisabled())
                return;

            if (e.d_semanticValue == SemanticValue.SV_SelectAll && e.d_payload.source == CursorInputSource.Left)
            {
                _dragAnchorIdx = 0;
                SetCaretIndex(GetText().Length);
                SetSelection(_dragAnchorIdx, _caretPos);
                ++e.handled;
            }
            else if (e.d_semanticValue == SemanticValue.SV_SelectWord && e.d_payload.source == CursorInputSource.Left)
            {
                // if masked, set up to select all
                if (IsTextMaskingEnabled())
                {
                    _dragAnchorIdx = 0;
                    SetCaretIndex(GetText().Length);
                }
                // not masked, so select the word that was double-clicked.
                else
                {
                    _dragAnchorIdx = TextUtils.GetWordStartIdx(GetText(),
                        _caretPos == GetText().Length ? _caretPos : _caretPos + 1);
                    _caretPos = TextUtils.GetNextWordStartIdx(GetText(), _caretPos);
                }

                // perform actual selection operation.
                SetSelection(_dragAnchorIdx, _caretPos);

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
                    _dragAnchorIdx = _caretPos;

                // Check if the semantic value to be handled is of a general type and can thus be
                // handled via common EditboxBase handlers
                var isSemanticValueHandled = HandleBasicSemanticValue(e);

                // If the semantic value was not handled, check for specific values
                if (!isSemanticValueHandled)
                {
                    // We assume it will be handled now, if not it will be set to false in default-case
                    isSemanticValueHandled = true;

                    switch (e.d_semanticValue)
                    {
                    case SemanticValue.SV_Confirm:
                    {
                        // Fire 'input accepted' event
                        OnTextAcceptedEvent(new WindowEventArgs(this));
                        break;
                    }

                    case SemanticValue.SV_GoToStartOfLine:
                        HandleHome(false);
                        break;

                    case SemanticValue.SV_GoToEndOfLine:
                        HandleEnd(false);
                        break;

                    case SemanticValue.SV_SelectToStartOfLine:
                        HandleHome(true);
                        break;

                    case SemanticValue.SV_SelectToEndOfLine:
                        HandleEnd(true);
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

//        protected internal override void OnCursorMove(CursorInputEventArgs e)
//        {
//            // base class processing
//            base.OnCursorMove(e);

//            if (_dragging)
//            {
//                var anchorIdx = GetTextIndexFromPosition(e.Position);
//#if CEGUI_BIDI_SUPPORT
//                if (d_bidiVisualMapping.GetV2lMapping().Count > anchorIdx)
//                    anchorIdx = d_bidiVisualMapping.GetV2lMapping()[anchorIdx];
//#endif
//                SetCaretIndex(anchorIdx);
//                SetSelection(_caretPos, _dragAnchorIdx);
//            }
//            ++e.handled;
//        }

        protected internal override void OnCharacter(TextEventArgs e)
        {
            // NB: We are not calling the base class handler here because it propogates
            // inputs back up the window hierarchy, whereas, as a consumer of key
            // events, we want such propogation to cease with us regardless of whether
            // we actually handle the event.

            // fire event.
            FireEvent(EventCharacterKey, e, EventNamespace);

            // only need to take notice if we have focus
            if (e.handled == 0 && HasInputFocus() && !IsReadOnly() &&
                GetFont().IsCodepointAvailable(e.d_character))
            {
                // backup current text
                var tmp = GetText();

                var undoSelection = new UndoHandler.UndoAction
                                    {
                                            d_type = UndoHandler.UndoActionType.UAT_DELETE,
                                            d_startIdx = GetSelectionStart(),
                                            d_text = tmp.CEGuiSubstring(GetSelectionStart(), GetSelectionLength())
                                    };

                tmp = tmp.Remove(GetSelectionStart(), GetSelectionLength());

                // if there is room
                if (tmp.Length < _maxTextLen)
                {
                    var undo = new UndoHandler.UndoAction
                               {
                                       d_type = UndoHandler.UndoActionType.UAT_INSERT,
                                       d_startIdx = GetSelectionStart(),
                                       d_text = e.d_character.ToString(global::System.Globalization.CultureInfo.InvariantCulture)
                               };

                    tmp = tmp.Insert(GetSelectionStart(),
                                     e.d_character.ToString(global::System.Globalization.CultureInfo.InvariantCulture));

                    if (HandleValidityChangeForString(tmp))
                    {
                        // erase selection using mode that does not modify getText()
                        // (we just want to update state)
                        EraseSelectedText(false);

                        // advance caret (done first so we can "do stuff" in event handlers!)
                        _caretPos++;

                        // set text to the newly modified string
                        SetText(tmp);

                        // char was accepted into the Editbox - mark event as handled.
                        ++e.handled;

                        _undoHandler.AddUndoHistory(undo);
                        if (GetSelectionLength() > 0)
                            _undoHandler.AddUndoHistory(undoSelection);
                    }
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
            // base class processing
            base.OnTextChanged(e);

            // clear selection
            ClearSelection();

            // make sure caret is within the text
            if (_caretPos > GetText().Length)
                SetCaretIndex(GetText().Length);

            ++e.handled;
        }

        private void AddEditboxProperties()
        {
            DefineProperty(
                    "ValidationString",
                    "Property to get/set the validation string Editbox.  Value is a text string.",
                    (w, v) => w.SetValidationString(v), w => w.GetValidationString(), ".*");
        }

        private void DefineProperty<T>(string name, string help, Action<Editbox, T> setter, Func<Editbox, T> getter,
                                       T defaultValue)
        {
            AddProperty(new TplWindowProperty<Editbox, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #region Fields
        
        /// <summary>
        /// Copy of validation reg-ex string.
        /// </summary>
        private string _validationString;

        /// <summary>
        /// Pointer to class used for validation of text.
        /// </summary>
        private RegexMatcher _validator;

        /// <summary>
        /// specifies whether validator was created by us, or supplied by user.
        /// </summary>
        private bool _weOwnValidator;

        /// <summary>
        /// Current match state of EditboxText
        /// </summary>
        private RegexMatcher.MatchState _validatorMatchState;

        /// <summary>
        /// Previous match state change response
        /// </summary>
        private bool _previousValidityChangeResponse;

        #endregion
    }
}