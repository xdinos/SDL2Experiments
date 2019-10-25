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
using SharpCEGui.Base.Views;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for the Combobox widget
    /// </summary>
    public class Combobox : Window
    {
        #region Constants

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/Combobox";

        /// <summary>
        /// Widget name for the editbox component.
        /// </summary>
        public const string EditboxName = "__auto_editbox__";

        /// <summary>
        /// Widget name for the drop list component.
        /// </summary>
        public const string DropListName = "__auto_droplist__";

        /// <summary>
        /// Widget suffix for the button component.
        /// </summary>
        public const string ButtonName = "__auto_button__";

        #endregion

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Combobox";

        // event names from edit box
        public const string EventReadOnlyModeChanged = "ReadOnlyModeChanged";
        public const string EventValidationStringChanged = "ValidationStringChanged";
        public const string EventMaximumTextLengthChanged = "MaximumTextLengthChanged";
        public const string EventTextValidityChanged = "TextValidityChanged";
        public const string EventCaretMoved = "CaretMoved";
        public const string EventTextSelectionChanged = "TextSelectionChanged";
        public const string EventEditboxFull = "EditboxFull";
        public const string EventTextAccepted = "TextAccepted";

        // event names from list box
        public const string EventListContentsChanged = "ListContentsChanged";
        public const string EventListSelectionChanged = "ListSelectionChanged";
        public const string EventSortModeChanged = "SortModeChanged";
        public const string EventVertScrollbarModeChanged = "VertScrollbarModeChanged";
        public const string EventHorzScrollbarModeChanged = "HorzScrollbarModeChanged";

        // events we produce / generate ourselves
        public const string EventDropListDisplayed = "DropListDisplayed";
        public const string EventDropListRemoved = "DropListRemoved";
        public const string EventListSelectionAccepted = "ListSelectionAccepted";

        /// <summary>
        /// Event fired when the read-only mode for the edit box is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose read only mode* 
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ReadOnlyModeChanged
        {
            add { SubscribeEvent(EventReadOnlyModeChanged, value); }
            remove { UnsubscribeEvent(EventReadOnlyModeChanged, value); }
        }

        /// <summary>
        /// Event fired when the edix box validation string is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose validation
        /// string was changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ValidationStringChanged
        {
            add { SubscribeEvent(EventValidationStringChanged, value); }
            remove { UnsubscribeEvent(EventValidationStringChanged, value); }
        }


        /// <summary>
        /// Event fired when the maximum string length is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose maximum edit box
        /// string length has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> MaximumTextLengthChanged
        {
            add { SubscribeEvent(EventMaximumTextLengthChanged, value); }
            remove { UnsubscribeEvent(EventMaximumTextLengthChanged, value); }
        }

        /// <summary>
        /// Event fired when the validity of the Combobox text (as determined by a
        /// RegexMatcher object) has changed.
        /// Handlers are passed a const RegexMatchStateEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose text validity has
        /// changed and RegexMatchStateEventArgs::matchState set to the new match
        /// validity. Handler return is significant, as follows:
        /// - true indicates the new state - and therfore text - is to be accepted.
        /// - false indicates the new state is not acceptable, and the previous text
        ///   should remain in place. NB: This is only possible when the validity
        ///   change is due to a change in the text, if the validity change is due to
        ///   a change in the validation regular expression string, then returning
        ///   false will have no effect.
        /// </summary>
        public event GuiEventHandler<EventArgs> TextValidityChanged
        {
            add { SubscribeEvent(EventTextValidityChanged, value); }
            remove { UnsubscribeEvent(EventTextValidityChanged, value); }
        }

        /// <summary>
        /// Event fired when the edit box text insertion position is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose caret position has
        /// been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> CaretMoved
        {
            add { SubscribeEvent(EventCaretMoved, value); }
            remove { UnsubscribeEvent(EventCaretMoved, value); }
        }

        /// <summary>
        /// Event fired when the current edit box text selection is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose edit box text selection
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> TextSelectionChanged
        {
            add { SubscribeEvent(EventTextSelectionChanged, value); }
            remove { UnsubscribeEvent(EventTextSelectionChanged, value); }
        }

        /// <summary>
        /// Event fired when the number of characters in the edit box has reached
        /// the currently set maximum.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose edit box has become
        /// full.
        /// </summary>
        public event GuiEventHandler<EventArgs> EditboxFull
        {
            add { SubscribeEvent(EventEditboxFull, value); }
            remove { UnsubscribeEvent(EventEditboxFull, value); }
        }

        /// <summary>
        /// Event fired when the user accepts the current edit box text by pressing
        /// Return, Enter, or Tab.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose edit box text has been
        /// accepted / confirmed by the user.
        /// </summary>
        public event GuiEventHandler<EventArgs> TextAccepted
        {
            add { SubscribeEvent(EventTextAccepted, value); }
            remove { UnsubscribeEvent(EventTextAccepted, value); }
        }

        /// <summary>
        /// Event fired when the contents of the list is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose list content has
        /// changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ListContentsChanged
        {
            add { SubscribeEvent(EventListContentsChanged, value); }
            remove { UnsubscribeEvent(EventListContentsChanged, value); }
        }

        /// <summary>
        /// Event fired when there is a change to the currently selected item in the
        /// list.
        /// @note This change in selection may be temporary (for example, when
        /// hovering over an item in the combobox). See also the event
        /// CEGUI::Combobox::EventListSelectionAccepted that is fired for a selection
        /// that the user has 'confirmed' in some way.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose currently selected list
        /// item has changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ListSelectionChanged
        {
            add { SubscribeEvent(EventListSelectionChanged, value); }
            remove { UnsubscribeEvent(EventListSelectionChanged, value); }
        }

        /// <summary>
        /// Event fired when the sort mode setting of the list is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose list sorting mode has
        /// been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> SortModeChanged
        {
            add { SubscribeEvent(EventSortModeChanged, value); }
            remove { UnsubscribeEvent(EventSortModeChanged, value); }
        }

        /// <summary>
        /// Event fired when the vertical scroll bar 'force' setting for the list is
        /// changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose vertical scroll bar
        /// setting is changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> VertScrollbarModeChanged
        {
            add { SubscribeEvent(EventVertScrollbarModeChanged, value); }
            remove { UnsubscribeEvent(EventVertScrollbarModeChanged, value); }
        }

        /// <summary>
        /// Event fired when the horizontal scroll bar 'force' setting for the list
        /// is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose horizontal scroll bar
        /// setting has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> HorzScrollbarModeChanged
        {
            add { SubscribeEvent(EventHorzScrollbarModeChanged, value); }
            remove { UnsubscribeEvent(EventHorzScrollbarModeChanged, value); }
        }

        /// <summary>
        /// Event fired when the drop-down list is displayed
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose drop down list has
        /// been displayed.
        /// </summary>
        public event GuiEventHandler<EventArgs> DropListDisplayed
        {
            add { SubscribeEvent(EventDropListDisplayed, value); }
            remove { UnsubscribeEvent(EventDropListDisplayed, value); }
        }

        /// <summary>
        /// Event fired when the drop-down list is removed / hidden.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox whose drop down list has
        /// been hidden.
        /// </summary>
        public event GuiEventHandler<EventArgs> DropListRemoved
        {
            add { SubscribeEvent(EventDropListRemoved, value); }
            remove { UnsubscribeEvent(EventDropListRemoved, value); }
        }

        /// <summary>
        /// Event fired when the user accepts a selection from the drop-down list
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Combobox in which the user has
        /// confirmed a selection from the drop down list.
        /// </summary>
        public event GuiEventHandler<EventArgs> ListSelectionAccepted
        {
            add { SubscribeEvent(EventListSelectionAccepted, value); }
            remove { UnsubscribeEvent(EventListSelectionAccepted, value); }
        }

        #endregion

        public override bool IsHit(Lunatics.Mathematics.Vector2 position, bool allowDisabled = false)
        {
            return false;
        }

        /// <summary>
        /// returns the mode of operation for the combo box.
        /// </summary>
        /// <returns>
        /// - true if the user can show the list and select an item with a single mouse click.
        /// - false if the user must click to show the list and then click again to select an item.
        /// </returns>
        public bool GetSingleCursorActivationEnabled()
        {
            return d_singleClickOperation;
        }

        /// <summary>
        /// returns true if the drop down list is visible.
        /// </summary>
        /// <returns>
        /// true if the drop down list is visible, false otherwise.
        /// </returns>
        public bool IsDropDownListVisible()
        {
            return GetDropList().IsEffectiveVisible();
        }

        /// <summary>
        /// Return a pointer to the Editbox component widget for this Combobox.
        /// </summary>
        /// <returns>
        /// Pointer to an Editbox object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the Editbox component does not exist.
        /// </exception>
        public Editbox GetEditbox()
        {
            return (Editbox) GetChild(EditboxName);
        }

        /// <summary>
        /// Return a popublic inter to the PushButton component widget for this Combobox.
        /// </summary>
        /// <returns>
        /// Popublic inter to a PushButton object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the PushButton component does not exist.
        /// </exception>
        public PushButton GetPushButton()
        {
            return (PushButton) GetChild(ButtonName);
        }

        /// <summary>
        /// Return a popublic inter to the ComboDropList component widget for this Combobox.
        /// </summary>
        /// <returns>
        /// Popublic inter to an ComboDropList object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the ComboDropList component does not exist.
        /// </exception>
        public ComboDropList GetDropList()
        {
            return (ComboDropList) GetChild(DropListName);
        }

        /// <summary>
        /// return whether the drop-list will vertically auto size to content.
        /// </summary>
        /// <returns></returns>
        public bool GetAutoSizeListHeightToContent()
        {
            return d_autoSizeHeight;
        }

        /// <summary>
        /// return whether the drop-list will horizontally auto size to content.
        /// </summary>
        /// <returns></returns>
        public bool GetAutoSizeListWidthToContent()
        {
            return d_autoSizeWidth;
        }

        /// <summary>
        /// return true if the Editbox has input focus.
        /// </summary>
        /// <returns>
        /// true if the Editbox has keyboard input focus, 
        /// false if the Editbox does not have keyboard input focus.
        /// </returns>
        public bool HasInputFocus()
        {
            return GetEditbox().HasInputFocus();
        }

        /// <summary>
        /// return true if the Editbox is read-only.
        /// </summary>
        /// <returns>
        /// true if the Editbox is read only and can't be edited by the user, 
        /// false if the Editbox is not read only and may be edited by the user.
        /// </returns>
        public bool IsReadOnly()
        {
            return GetEditbox().IsReadOnly();
        }

        /// <summary>
        /// return the validation MatchState for the current Combobox text, given
        /// the currently set validation string.
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
            return GetEditbox().GetTextMatchState();
        }

        /// <summary>
        /// return the currently set validation string
        /// </summary>
        /// <returns>
        /// String object containing the current validation regex data
        /// </returns>
        /// <remarks>
        /// Validation is performed by means of a regular expression.  
        /// If the text matches the regex, the text is said to have passed validation.  
        /// If the text does not match with the regex then the text fails validation.
        /// </remarks>
        public string GetValidationString()
        {
            return GetEditbox().GetValidationString();
        }

        /// <summary>
        /// return the current position of the caret.
        /// </summary>
        /// <returns>
        /// Index of the insert caret relative to the start of the text.
        /// </returns>
        public int GetCaretIndex()
        {
            return GetEditbox().GetCaretIndex();
        }

        /// <summary>
        /// return the current selection start point.
        /// </summary>
        /// <returns>
        /// Index of the selection start popublic int relative to the start of the text.  
        /// If no selection is defined this function returns the position of the caret.</returns>
        public int GetTextSelectionStart()
        {
            return GetEditbox().GetSelectionStart();
        }

        /// <summary>
        /// return the current selection end point.
        /// </summary>
        /// <returns>
        /// Index of the selection end popublic int relative to the start of the text.
        /// If no selection is defined this function returns the position of the caret.
        /// </returns>
        public int GetTextSelectionEnd()
        {
            return GetEditbox().GetSelectionEnd();
        }

        /// <summary>
        /// return the length of the current selection (in code popublic ints / characters).
        /// </summary>
        /// <returns>
        /// Number of code popublic ints (or characters) contained within the currently defined selection.
        /// </returns>
        public int GetTextSelectionLength()
        {
            return GetEditbox().GetSelectionLength();
        }

        /// <summary>
        /// return the maximum text length set for this Editbox.
        /// </summary>
        /// <returns>
        /// The maximum number of code popublic ints (characters) that can be entered public into this Editbox.
        /// </returns>
        /// <remarks>
        /// Depending on the validation string set, the actual length of text that can be entered may be less than the value
        /// returned here (it will never be more).
        /// </remarks>
        public int GetMaxTextLength()
        {
            return GetEditbox().GetMaxTextLength();
        }

        /// <summary>
        /// Return number of items attached to the list box.
        /// </summary>
        /// <returns>
        /// the number of items currently attached to this list box.
        /// </returns>
        public int GetItemCount()
        {
            return GetDropList().GetItemCount();
        }

        /// <summary>
        /// Pointer to a StandardItem based object that is the selected item in the list.  
        /// Will return NULL if no item is selected.
        /// </summary>
        /// <returns>
        /// Return a pointer to the currently selected item.
        /// </returns>
        public StandardItem GetSelectedItem()
        {
            return GetDropList().GetFirstSelectedItem();
        }

        /// <summary>
        /// Return the item at index position \a index.
        /// </summary>
        /// <param name="index">
        /// Zero based index of the item to be returned.
        /// </param>
        /// <returns>
        /// Pointer to the ListboxItem at index position \a index in the list box.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// thrown if \a index is out of range.
        /// </exception>
        public StandardItem GetItemFromIndex(int index)
        {
            return GetDropList().GetItemAtIndex(index);
        }

        /// <summary>
        /// Return the index of ListboxItem \a item
        /// </summary>
        /// <param name="item">
        /// Popublic inter to a ListboxItem whos zero based index is to be returned.
        /// </param>
        /// <returns>
        /// Zero based index indicating the position of ListboxItem \a item in the list box.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this list box.
        /// </exception>
        public int GetItemIndex(StandardItem item)
        {
            return GetDropList().GetModel().GetChildId(item);
        }

        /// <summary>
        /// return whether list sorting is enabled
        /// </summary>
        /// <returns>
        /// true if the list is sorted, false if the list is not sorted
        /// </returns>
        public bool IsSortEnabled()
        {
            return GetDropList().GetSortMode() != ViewSortMode.None;
        }

        /// <summary>
        /// return whether the string at index position \a index is selected
        /// </summary>
        /// <param name="index">
        /// Zero based index of the item to be examined.
        /// </param>
        /// <returns>
        /// true if the item at \a index is selected, false if the item at \a index is not selected.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a index is out of range.
        /// </exception>
        public bool IsItemSelected(int index)
        {
            return GetDropList().IsIndexSelected(index);
        }

        /// <summary>
        /// Search the list for an item with the specified text
        /// </summary>
        /// <param name="text">String object containing the text to be searched for.</param>
        /// <param name="startItem">
        /// ListboxItem where the search is to begin, the search will not include \a item.  If \a item is
        /// NULL, the search will begin from the first item in the list.</param>
        /// <returns>
        /// Pointer to the first ListboxItem in the list after \a item that has text matching \a text.  If
        /// no item matches the criteria NULL is returned.</returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this list box.
        /// </exception>
        public StandardItem FindItemWithText(string text, StandardItem startItem)
        {
            return GetDropList().FindItemWithText(text, startItem);
        }

        /// <summary>
        /// Return whether the specified StandardItem is in the List
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// true if ListboxItem \a item is in the list, false if ListboxItem \a item is not in the list.
        /// </returns>
        public bool IsItemInList(StandardItem item)
        {
            return GetDropList().IsItemInList(item);
        }

        /// <summary>
        /// Return whether the vertical scroll bar is always shown.
        /// </summary>
        /// <returns>
        /// - true if the scroll bar will always be shown even if it is not required.
        /// - false if the scroll bar will only be shown when it is required.
        /// </returns>
        public bool IsVertScrollbarAlwaysShown()
        {
            //TODO: migrate the combobox's sorting option to the new one
            return GetDropList().GetVertScrollbarDisplayMode() == ScrollbarDisplayMode.Shown;
        }

        /// <summary>
        /// Return whether the horizontal scroll bar is always shown.
        /// </summary>
        /// <returns>
        /// - true if the scroll bar will always be shown even if it is not required.
        /// - false if the scroll bar will only be shown when it is required.
        /// </returns>
        public bool IsHorzScrollbarAlwaysShown()
        {
            //TODO: migrate the combobox's sorting option to the new one
            return GetDropList().GetHorzScrollbarDisplayMode() == ScrollbarDisplayMode.Shown;
        }

        /// <summary>
        /// Initialise the Window based object ready for use.
        /// </summary>
        /// <remarks>
        /// This must be called for every window created.  
        /// Normally this is handled automatically by the WindowFactory for each Window type.
        /// </remarks>
        protected override void InitialiseComponents()
        {
            var editbox = GetEditbox();
            var droplist = GetDropList();
            var button = GetPushButton();
            droplist.SetFont(d_font);
            editbox.SetFont(d_font);

            // ban properties forwarded from here
            droplist.BanPropertyFromXML(Window.VisiblePropertyName);
            editbox.BanPropertyFromXML("MaxTextLength");
            editbox.BanPropertyFromXML("ReadOnly");

            // internal event wiring
            button.CursorPressHold += ButtonPressHandler;
            droplist.ListSelectionAccepted += DroplistSelectionAcceptedHandler;
            droplist.Hidden += DroplistHiddenHandler;
            editbox.CursorPressHold += EditboxPointerPressHoldHandler;

            // event forwarding setup
            editbox.ReadOnlyModeChanged += EditboxReadOnlyChangedHandler;
            editbox.ValidationStringChanged += EditboxValidationStringChangedHandler;
            editbox.MaximumTextLengthChanged += EditboxMaximumTextLengthChangedHandler;
            editbox.TextValidityChanged += EditboxTextValidityChangedHandler;
            editbox.CaretMoved += EditboxCaretMovedHandler;
            editbox.TextSelectionChanged += EditboxTextSelectionChangedHandler;
            editbox.EditboxFull += EditboxEditboxFullEventHandler;
            editbox.TextAccepted += EditboxTextAcceptedEventHandler;
            editbox.TextChanged += EditboxTextChangedEventHandler;
            droplist.ViewContentsChanged += ListWidgetListContentsChangedHandler;
            droplist.SelectionChanged += ListWidgetListSelectionChangedHandler;
            droplist.SortModeChanged += ListWidgetSortModeChangedHandler;
            droplist.VertScrollbarDisplayModeChanged += ListWidgetVertScrollModeChangedHandler;
            droplist.HorzScrollbarDisplayModeChanged += ListWidgetHorzScrollModeChangedHandler;

            // put components in their initial positions
            PerformChildWindowLayout();
        }

        /// <summary>
        /// Show the drop-down list
        /// </summary>
        public void ShowDropList()
        {
            UpdateAutoSizedDropList();

            // Display the box
            var droplist = GetDropList();
            droplist.Show();
            droplist.Activate();
            droplist.CaptureInput();

            // Fire off event
            OnDropListDisplayed(new WindowEventArgs(this));
        }

        /// <summary>
        /// Hide the drop-down list
        /// </summary>
        public void HideDropList()
        {
            // the natural order of things when this happens will ensure the list is
            // hidden and events are fired.
            GetDropList().ReleaseInput();
        }

        /// <summary>
        /// Set the mode of operation for the combo box.
        /// </summary>
        /// <param name="setting">
        /// - true if the user should be able to show the list and select an item with a single mouse click.
        /// - false if the user must click to show the list and then click again to select an item.
        /// </param>
        public void SetSingleCursorActivationEnabled(bool setting)
        {
            d_singleClickOperation = setting;
            GetDropList().SetAutoArmEnabled(setting);
        }

        /// <summary>
        /// Select item in list matching editbox text, clear selection if none match
        /// </summary>
        public void SelectListItemWithEditboxText()
        {
            var droplist = GetDropList();

            var item = droplist.FindItemWithText(GetEditbox().GetText(), null);
            if (item != null)
            {
                droplist.SetIndexSelectionState(item, true);
                droplist.EnsureIndexIsVisible(item);

                // Fire off a selection event to inform subscribers
                OnListSelectionAccepted(new WindowEventArgs(this));
            }
            else
            {
                droplist.ClearSelections();
            }
        }

        /// <summary>
        /// Sets whether the Combobox drop-down list will automatically resize
        /// it's height according to the total height of items added to the list.
        /// </summary>
        /// <param name="autoSize"></param>
        public void SetAutoSizeListHeightToContent(bool autoSize)
        {
            d_autoSizeHeight = autoSize;

            if (d_autoSizeHeight && IsDropDownListVisible())
                UpdateAutoSizedDropList();
        }

        /// <summary>
        /// Sets whether the Combobox drop-down list will automatically resize
        /// it's width according to the width of the items added to the list.
        /// </summary>
        /// <param name="autoSize"></param>
        public void SetAutoSizeListWidthToContent(bool autoSize)
        {
            d_autoSizeWidth = autoSize;

            if (d_autoSizeWidth && IsDropDownListVisible())
                UpdateAutoSizedDropList();
        }

        /// <summary>
        /// update drop list size according to auto-size options.
        /// </summary>
        public void UpdateAutoSizedDropList()
        {
            GetDropList().ResizeToContent(d_autoSizeWidth, d_autoSizeHeight);
        }

        /// <summary>
        /// Specify whether the Editbox is read-only.
        /// </summary>
        /// <param name="setting">
        /// true if the Editbox is read only and can't be edited by the user, 
        /// false if the Editbox is not
        /// read only and may be edited by the user.
        /// </param>
        public void SetReadOnly(bool setting)
        {
            GetEditbox().SetReadOnly(setting);
        }

        /// <summary>
        /// Set the text validation string.
        /// <para>
        /// Validation is performed by means of a regular expression.  If the text matches the regex, the text is said to have passed
        /// validation.  If the text does not match with the regex then the text fails validation.
        /// </para>
        /// </summary>
        /// <param name="validationString">
        /// String object containing the validation regex data to be used.
        /// </param>
        public void SetValidationString(string validationString)
        {
            GetEditbox().SetValidationString(validationString);
        }

        /// <summary>
        /// Set the current position of the caret.
        /// </summary>
        /// <param name="caretPos">
        /// New index for the insert caret relative to the start of the text.  If the value specified is greater than the
        /// number of characters in the Editbox, the caret is positioned at the end of the text.
        /// </param>
        public void SetCaretIndex(int caretPos)
        {
            GetEditbox().SetCaretIndex(caretPos);
        }

        /// <summary>
        /// Define the current selection for the Editbox
        /// </summary>
        /// <param name="startPos">
        /// Index of the starting popublic int for the selection.  If this value is greater than the number of characters in the Editbox, the
        /// selection start will be set to the end of the text.
        /// </param>
        /// <param name="endPos">
        /// Index of the ending popublic int for the selection.  If this value is greater than the number of characters in the Editbox, the
        /// selection start will be set to the end of the text.</param>
        public void SetTextSelection(int startPos, int endPos)
        {
            GetEditbox().SetSelection(startPos, endPos);
        }

        /// <summary>
        /// Define the current selection start for the Editbox
        /// </summary>
        /// <param name="startPos">
        /// Index of the starting popublic int for the selection.  
        /// If this value is greater than the number of characters in the Editbox, the
        /// selection start will be set to the end of the text.
        /// </param>
        public void SetTextSelectionStart(int startPos)
        {
            GetEditbox().SetSelectionStart(startPos);
        }

        /// <summary>
        /// Define the current selection for the Editbox
        /// </summary>
        /// <param name="length">
        /// Length of the selection.
        /// </param>
        public void SetTextSelectionLength(int length)
        {
            GetEditbox().SetSelectionLength(length);
        }

        /// <summary>
        /// set the maximum text length for this Editbox.
        /// </summary>
        /// <param name="maxLen">
        /// The maximum number of code popublic ints (characters) that can be entered public into this Editbox.
        /// </param>
        /// <remarks>
        /// Depending on the validation string set, the actual length of text that can be entered may be less than the value
        /// set here (it will never be more).
        /// </remarks>
        public void SetMaxTextLength(int maxLen)
        {
            GetEditbox().SetMaxTextLength(maxLen);
        }

        /// <summary>
        /// Activate the edit box component of the Combobox.
        /// </summary>
        public void ActivateEditbox()
        {
            var editbox = GetEditbox();

            if (!editbox.IsActive())
            {
                editbox.Activate();
            }
        }

        /// <summary>
        /// Remove all items from the list.
        /// <remarks>
        /// Note that this will cause 'AutoDelete' items to be deleted.
        /// </remarks>
        /// </summary>
        public void ResetList()
        {
            GetDropList().ClearList();
        }

        /// <summary>
        /// Add the given ListboxItem to the list.
        /// </summary>
        /// <param name="item">
        /// Popublic inter to the ListboxItem to be added to the list.  
        /// Note that it is the passed object that is added to the
        /// list, a copy is not made.  
        /// If this parameter is NULL, nothing happens.
        /// </param>
        public void AddItem(StandardItem item)
        {
            GetDropList().AddItem(item);
        }

        /// <summary>
        /// Insert an item public into the list box after a specified item already in the list.
        /// 
        /// Note that if the list is sorted, the item may not end up in the requested position.
        /// </summary>
        /// <param name="item">
        /// Pointer to the StandardItem to be inserted. Note that it is the passed
        /// object that is added to the list, a copy is not made.
        /// If this parameter is NULL, nothing happens.
        /// </param>
        /// <param name="position">
        /// Pointer to a StandardItem that \a item is to be inserted after. If this
        /// parameter is NULL, the item is inserted at the start of the list.
        /// </param>
        public void InsertItem(StandardItem item, StandardItem position)
        {
            GetDropList().InsertItem(item, position);
        }

        /// <summary>
        /// Removes the given item from the list box.
        /// </summary>
        /// <param name="item">
        /// Pointer to the StandardItem that is to be removed.
        /// If \a item is not attached to this list widget then nothing will happen.
        /// </param>
        public void RemoveItem(StandardItem item)
        {
            GetDropList().RemoveItem(item);
        }

        /// <summary>
        /// Clear the selected state for all items.
        /// </summary>
        public void ClearAllSelections()
        {
            GetDropList().ClearSelections();
        }

        /// <summary>
        /// Set whether the list should be sorted.
        /// </summary>
        /// <param name="setting">
        /// true if the list should be sorted, false if the list should not be sorted.
        /// </param>
        public void SetSortingEnabled(bool setting)
        {
            GetDropList().SetSortMode(setting ? ViewSortMode.Ascending : ViewSortMode.None);
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
            GetDropList()
                    .SetVertScrollbarDisplayMode(setting ? ScrollbarDisplayMode.Shown : ScrollbarDisplayMode.WhenNeeded);
        }

        /// <summary>
        /// Set whether the horizontal scroll bar should always be shown.
        /// </summary>
        /// <param name="setting">
        /// true if the horizontal scroll bar should be shown even when it is not required.  false if the horizontal
        /// scroll bar should only be shown when it is required.
        /// </param>
        public void SetShowHorzScrollbar(bool setting)
        {
            GetDropList()
                    .SetHorzScrollbarDisplayMode(setting ? ScrollbarDisplayMode.Shown : ScrollbarDisplayMode.WhenNeeded);
        }

        /// <summary>
        /// Set the select state of an attached ListboxItem.
        /// <para>
        /// This is the recommended way of selecting and deselecting items attached to a list box as it respects the
        /// multi-select mode setting.  It is possible to modify the setting on ListboxItems directly, but that approach
        /// does not respect the settings of the list box.
        /// </para>
        /// </summary>
        /// <param name="item">
        /// The ListboxItem to be affected.  This item must be attached to the list box.
        /// </param>
        /// <param name="state">
        /// true to select the item, false to de-select the item.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this list box.
        /// </exception>
        public void SetItemSelectState(StandardItem item, bool state)
        {
            var wasSelected = (item != null && GetDropList().IsItemSelected(item));

            GetDropList().SetIndexSelectionState(item, state);

            ItemSelectChangeTextUpdate(item, state, wasSelected);
        }

        /// <summary>
        /// Set the select state of an attached ListboxItem.
        /// <para>
        /// This is the recommended way of selecting and deselecting items attached to a list box as it respects the
        /// multi-select mode setting.  It is possible to modify the setting on ListboxItems directly, but that approach
        /// does not respect the settings of the list box.
        /// </para>
        /// </summary>
        /// <param name="itemIndex">
        /// The zero based index of the ListboxItem to be affected.  This must be a valid index (0 &lt;= index @Lt; getItemCount())
        /// </param> 
        /// <param name="state">
        /// true to select the item, false to de-select the item.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item_index is out of range for the list box
        /// </exception>
        public void SetItemSelectState(int itemIndex, bool state)
        {
            var droplist = GetDropList();

            var item = (droplist.GetItemCount() > itemIndex)
                           ? droplist.GetItemAtIndex(itemIndex)
                           : null;

            SetItemSelectState(item, state);
        }

        /// <summary>
        /// Causes the list widget to update it's internal state after changes have
        /// been made to one or more attached StandardItem objects.
        /// 
        /// Client code must call this whenever it has made any changes to
        /// StandardItem objects already attached to the list widget. If you are
        /// just adding items, or removed items to update them prior to re-adding
        /// them, there is no need to call this method.
        /// </summary>
        public void HandleUpdatedListItemData()
        {
            GetDropList().InvalidateView(false);
        }


        /// <summary>
        /// Constructor for Combobox base class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Combobox(string type, string name)
            : base(type, name)
        {
            d_singleClickOperation = false;
            d_autoSizeHeight = false;
            d_autoSizeWidth = false;

            AddComboboxProperties();
        }


        // TODO: Destructor for Combobox base class
        // TODO: virtual ~Combobox() { throw new NotImplementedException();}

        /// <summary>
        /// Handler function for button clicks.
        /// </summary>
        /// <param name="e"></param>
        protected bool ButtonPressHandler(EventArgs e)
        {
            SelectListItemWithEditboxText();
            ShowDropList();

            return true;
        }

        /// <summary>
        /// Handler for selections made in the drop-list
        /// </summary>
        /// <param name="e"></param>
        protected bool DroplistSelectionAcceptedHandler(EventArgs e)
        {
            // copy the text from the selected item into the edit box
            var item = ((ComboDropList) ((WindowEventArgs) e).Window).GetFirstSelectedItem();

            if (item != null)
            {
                var editbox = GetEditbox();
                // Put the text from the list item into the edit box
                editbox.SetText(item.GetText());

                // select text if it's editable, and move caret to end
                if (!IsReadOnly())
                {
                    editbox.SetSelection(0, item.GetText().Length);
                    editbox.SetCaretIndex(item.GetText().Length);
                }

                editbox.SetCaretIndex(0);
                editbox.Activate();

                // fire off an event of our own
                OnListSelectionAccepted(new WindowEventArgs(this));
            }

            return true;
        }

        /// <summary>
        /// Handler for when drop-list hides itself
        /// </summary>
        /// <param name="e"></param>
        protected bool DroplistHiddenHandler(EventArgs e)
        {
            OnDroplistRemoved(new WindowEventArgs(this));
            return true;
        }

        /// <summary>
        /// Mouse button down handler attached to edit box
        /// </summary>
        /// <param name="e"></param>
        protected bool EditboxPointerPressHoldHandler(EventArgs e)
        {
            // only interested in left button
            if (((CursorInputEventArgs)e).Source == CursorInputSource.Left)
            {
                var editbox = GetEditbox();

                // if edit box is read-only, show list
                if (editbox.IsReadOnly())
                {
                    var droplist = GetDropList();

                    // if there is an item with the same text as the edit box, pre-select it
                    var item = droplist.FindItemWithText(editbox.GetText(), null);

                    if (item != null)
                    {
                        droplist.SetIndexSelectionState(item, true);
                        droplist.EnsureIndexIsVisible(item);
                    }
                    else
                    {
                        // no matching item, so select nothing
                        droplist.ClearSelections();
                    }

                    ShowDropList();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Update the Combobox text to reflect programmatically made changes to selected list item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newState"></param>
        /// <param name="oldState"></param>
        protected void ItemSelectChangeTextUpdate(StandardItem item, bool newState, bool oldState)
        {
            if (!newState)
            {
                if (GetText() == item.GetText())
                    SetText("");
            }
            else
            {
                if (!oldState)
                    SetText(item.GetText());
            }
        }

        protected bool EditboxReadOnlyChangedHandler(EventArgs e)
        {
            OnReadOnlyChanged(new WindowEventArgs(this));
            return true;
        }

        protected bool EditboxValidationStringChangedHandler(EventArgs e)
        {
            OnValidationStringChanged(new WindowEventArgs(this));
            return true;
        }

        protected bool EditboxMaximumTextLengthChangedHandler(EventArgs e)
        {
            OnMaximumTextLengthChanged(new WindowEventArgs(this));
            return true;
        }

        protected bool EditboxTextValidityChangedHandler(EventArgs e)
        {
            var args = new RegexMatchStateEventArgs(this, ((RegexMatchStateEventArgs) e).matchState);
            OnTextValidityChanged(args);

            return args.handled > 0;
        }

        protected bool EditboxCaretMovedHandler(EventArgs e)
        {
            OnCaretMoved(new WindowEventArgs(this));
            return true;
        }

        protected bool EditboxTextSelectionChangedHandler(EventArgs e)
        {
            OnTextSelectionChanged(new WindowEventArgs(this));
            return true;
        }

        protected bool EditboxEditboxFullEventHandler(EventArgs e)
        {
            OnEditboxFullEvent(new WindowEventArgs(this));
            return true;
        }

        protected bool EditboxTextAcceptedEventHandler(EventArgs e)
        {
            OnTextAcceptedEvent(new WindowEventArgs(this));

            return true;
        }

        protected bool EditboxTextChangedEventHandler(EventArgs e)
        {
            // set this windows text to match
            SetText(((WindowEventArgs) e).Window.GetText());
            return true;
        }

        protected bool ListWidgetListContentsChangedHandler(EventArgs e)
        {
            if (IsDropDownListVisible())
                UpdateAutoSizedDropList();

            OnListContentsChanged(new WindowEventArgs(this));

            return true;
        }

        protected bool ListWidgetListSelectionChangedHandler(EventArgs e)
        {
            OnListSelectionChanged(new WindowEventArgs(this));
            return true;
        }

        protected bool ListWidgetSortModeChangedHandler(EventArgs e)
        {
            OnSortModeChanged(new WindowEventArgs(this));
            return true;
        }

        protected bool ListWidgetVertScrollModeChangedHandler(EventArgs e)
        {
            OnVertScrollbarModeChanged(new WindowEventArgs(this));
            return true;
        }

        protected bool ListWidgetHorzScrollModeChangedHandler(EventArgs e)
        {
            OnHorzScrollbarModeChanged(new WindowEventArgs(this));
            return true;
        }

        /// <summary>
        /// Handler called internally when the read only state of the Combobox's Editbox has been changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnReadOnlyChanged(WindowEventArgs e)
        {
            FireEvent(EventReadOnlyModeChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the Combobox's Editbox validation string has been changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValidationStringChanged(WindowEventArgs e)
        {
            FireEvent(EventValidationStringChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the Combobox's Editbox maximum text length is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMaximumTextLengthChanged(WindowEventArgs e)
        {
            FireEvent(EventMaximumTextLengthChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when something has caused the validity state of the current text to change. 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextValidityChanged(RegexMatchStateEventArgs e)
        {
            FireEvent(EventTextValidityChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the caret in the Comboxbox's Editbox moves.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCaretMoved(WindowEventArgs e)
        {
            FireEvent(EventCaretMoved, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the selection within the Combobox's Editbox changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextSelectionChanged(WindowEventArgs e)
        {
            FireEvent(EventTextSelectionChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the maximum length is reached for text in the Combobox's Editbox.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEditboxFullEvent(WindowEventArgs e)
        {
            FireEvent(EventEditboxFull, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the text in the Combobox's Editbox is accepted (by various means).
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextAcceptedEvent(WindowEventArgs e)
        {
            SelectListItemWithEditboxText();
            FireEvent(EventTextAccepted, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the Combobox's Drop-down list contents are changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnListContentsChanged(WindowEventArgs e)
        {
            FireEvent(EventListContentsChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the selection within the Combobox's drop-down list changes
        /// (this is not the 'final' accepted selection, just the currently highlighted item).
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnListSelectionChanged(WindowEventArgs e)
        {
            FireEvent(EventListSelectionChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called  fired internally when the sort mode for the Combobox's drop-down list is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortModeChanged(WindowEventArgs e)
        {
            FireEvent(EventSortModeChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the 'force' setting for the vertical scrollbar within the Combobox's
        /// drop-down list is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnVertScrollbarModeChanged(WindowEventArgs e)
        {
            FireEvent(EventVertScrollbarModeChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the 'force' setting for the horizontal scrollbar within the Combobox's
        /// drop-down list is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnHorzScrollbarModeChanged(WindowEventArgs e)
        {
            FireEvent(EventHorzScrollbarModeChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the Combobox's drop-down list has been displayed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDropListDisplayed(WindowEventArgs e)
        {
            GetGUIContext().UpdateWindowContainingCursor();
            GetPushButton().SetPushedState(true);
            FireEvent(EventDropListDisplayed, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the Combobox's drop-down list has been hidden.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDroplistRemoved(WindowEventArgs e)
        {
            GetGUIContext().UpdateWindowContainingCursor();
            GetPushButton().SetPushedState(false);
            FireEvent(EventDropListRemoved, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the user has confirmed a selection within the Combobox's drop-down list.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnListSelectionAccepted(WindowEventArgs e)
        {
            FireEvent(EventListSelectionAccepted, e, EventNamespace);
        }


        protected internal override void OnFontChanged(WindowEventArgs e)
        {
            // Propagate to children
            GetEditbox().SetFont(d_font);
            GetDropList().SetFont(d_font);

            // Call base class handler
            base.OnFontChanged(e);
        }

        protected override void OnTextChanged(WindowEventArgs e)
        {
            var editbox = GetEditbox();

            // update ourselves only if needed (prevents perpetual event loop & stack overflow)
            if (editbox.GetText() != GetText())
            {
                // done before doing base class processing so event subscribers see 'updated' version of this.
                editbox.SetText(GetText());
                ++e.handled;

                SelectListItemWithEditboxText();

                base.OnTextChanged(e);
            }
        }

        protected override void OnActivated(ActivationEventArgs e)
        {
            if (!IsActive())
            {
                base.OnActivated(e);
                ActivateEditbox();
            }
        }

        protected override void OnSized(ElementEventArgs e)
        {
            if (IsDropDownListVisible())
                UpdateAutoSizedDropList();

            base.OnSized(e);
        }

        /// <summary>
        /// true if user can show and select from list in a single click.
        /// </summary>
        protected bool d_singleClickOperation;

        protected bool d_autoSizeHeight;
        protected bool d_autoSizeWidth;


        /*************************************************************************
            Private methods
        *************************************************************************/

        private void AddComboboxProperties()
        {
            DefineProperty(
                "ReadOnly",
                "Property to get/set the read-only setting for the Editbox.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetReadOnly(v), x => x.IsReadOnly(), false);

            DefineProperty(
                "ValidationString",
                "Property to get/set the validation string Editbox.  Value is a text string.",
                (x, v) => x.SetValidationString(v), x => x.GetValidationString(), ".*");

            DefineProperty(
                "CaretIndex",
                "Property to get/set the current caret index.  Value is \"[uint]\".",
                (x, v) => x.SetCaretIndex(v), x => x.GetCaretIndex(), 0);

            DefineProperty(
                "SelectionStart",
                "Property to get/set the zero based index of the selection start position within the text.  Value is \"[uint]\".",
                (x, v) => x.SetTextSelectionStart(v), x => x.GetTextSelectionStart(), 0);

            DefineProperty(
                "SelectionLength",
                "Property to get/set the length of the selection (as a count of the number of code points selected).  Value is \"[uint]\".",
                (x, v) => x.SetTextSelectionLength(v), x => x.GetTextSelectionLength(), 0);

            DefineProperty(
                "MaxTextLength",
                "Property to get/set the the maximum allowed text length (as a count of code points).  Value is \"[uint]\".",
                (x, v) => x.SetMaxTextLength(v), x => x.GetMaxTextLength(), Int32.MaxValue);

            // TODO: Inconsistency between setter, getter and property name
            DefineProperty(
                "SortList",
                "Property to get/set the sort setting of the list box.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetSortingEnabled(v), x => x.IsSortEnabled(), false);

            // TODO: Inconsistency between setter, getter and property name
            DefineProperty(
                "ForceVertScrollbar",
                "Property to get/set the 'always show' setting for the vertical scroll bar of the list box.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetShowVertScrollbar(v), x => x.IsVertScrollbarAlwaysShown(), false);

            // TODO: Inconsistency between setter, getter and property name
            DefineProperty(
                "ForceHorzScrollbar",
                "Property to get/set the 'always show' setting for the horizontal scroll bar of the list box.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetShowHorzScrollbar(v), x => x.IsHorzScrollbarAlwaysShown(), false);

            // TODO: Inconsistency between setter, getter and property name
            DefineProperty(
                "SingleClickMode",
                "Property to get/set the 'single click mode' setting for the combo box.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetSingleCursorActivationEnabled(v), x => x.GetSingleCursorActivationEnabled(), false);

            DefineProperty(
                "AutoSizeListHeight",
                "Property to get/set whether the drop down list will vertically auto-size itself to fit it's content. Value is either \"True\" or \"False\".",
                (x, v) => x.SetAutoSizeListHeightToContent(v), x => x.GetAutoSizeListHeightToContent(), false);

            DefineProperty(
                "AutoSizeListWidth",
                "Property to get/set whether the drop down list will horizontally auto-size itself to fit it's content. Value is either \"True\" or \"False\".",
                (x, v) => x.SetAutoSizeListWidthToContent(v), x => x.GetAutoSizeListWidthToContent(), false);
        }

        private void DefineProperty<T>(string name, string help, Action<Combobox, T> setter, Func<Combobox, T> getter,
                                       T defaultValue)
        {
            AddProperty(new TplWindowProperty<Combobox, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }
    }
}