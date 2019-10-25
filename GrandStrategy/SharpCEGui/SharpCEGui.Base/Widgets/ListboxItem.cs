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
    /// Base class for list box items
    /// </summary>
    public abstract class ListboxItem : IComparable<ListboxItem>
    {
        /// <summary>
        /// Default selection brush colour.
        /// </summary>
        public static readonly Colour DefaultSelectionColour = new Colour(0xFF4444AA);

        /// <summary>
        /// base class constructor
        /// </summary>
        /// <param name="text"></param>
        /// <param name="itemId"></param>
        /// <param name="itemData"></param>
        /// <param name="disabled"></param>
        /// <param name="autoDelete"></param>
        protected ListboxItem(string text,
                              int itemId = 0,
                              object itemData = null,
                              bool disabled = false,
                              bool autoDelete = true)
        {
#if CEGUI_BIDI_SUPPORT
            _bidiVisualMapping = new NBidiVisualMapping();
#else
            _bidiVisualMapping = null;
#endif
            _itemId = itemId;
            _itemData = itemData;
            Selected = false;
            _disabled = disabled;
            _autoDelete = autoDelete;
            Owner = null;
            SelectCols = new ColourRect(DefaultSelectionColour);
            SelectBrush = null;

            SetText(text);
        }

        /// <summary>
        /// return the text string set for this list box item.
        /// <para>
        /// Note that even if the item does not render text, the text string can still be useful, since it
        /// is used for sorting list box items.
        /// </para>
        /// </summary>
        /// <returns>
        /// String object containing the current text for the list box item.
        /// </returns>
        public string GetTooltipText()
        {
            return _tooltipText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return _textLogical;
        }

        /// <summary>
        /// return text string with \e visual ordering of glyphs.
        /// </summary>
        /// <returns></returns>
        public string GetTextVisual()
        {
            // no bidi support
            if (_bidiVisualMapping == null)
                return _textLogical;

            if (!_bidiDataValid)
            {
                _bidiVisualMapping.UpdateVisual(_textLogical);
                _bidiDataValid = true;
            }

            return _bidiVisualMapping.GetTextVisual();
        }

        /// <summary>
        /// Return the current ID assigned to this list box item.
        /// <para>
        /// Note that the system does not make use of this value, client code can assign any meaning it
        /// wishes to the ID.
        /// </para>
        /// </summary>
        /// <returns>
        /// ID code currently assigned to this list box item
        /// </returns>
        public int GetId()
        {
            return _itemId;
        }

        /// <summary>
        /// Return the pointer to any client assigned user data attached to this lis box item.
        /// <para>
        /// Note that the system does not make use of this data, client code can assign any meaning it
        /// wishes to the attached data.
        /// </para>
        /// </summary>
        /// <returns>
        /// Pointer to the currently assigned user data.
        /// </returns>
        public object GetUserData()
        {
            return _itemData;
        }

        /// <summary>
        /// return whether this item is selected.
        /// </summary>
        /// <returns>
        /// true if the item is selected, false if the item is not selected.
        /// </returns>
        public bool IsSelected()
        {
            return Selected;
        }

        /// <summary>
        /// return whether this item is disabled.
        /// </summary>
        /// <returns>
        /// true if the item is disabled, false if the item is enabled.
        /// </returns>
        public bool IsDisabled()
        {
            return _disabled;
        }

        /// <summary>
        /// return whether this item will be automatically deleted when the list box it is attached to
        /// is destroyed, or when the item is removed from the list box.
        /// </summary>
        /// <returns>
        /// true if the item object will be deleted by the system when the list box it is attached to is
        /// destroyed, or when the item is removed from the list.  
        /// false if client code must destroy the item after it is removed from the list.
        /// </returns>
        public bool IsAutoDeleted()
        {
            return _autoDelete;
        }

        /// <summary>
        /// Get the owner window for this ListboxItem.
        /// <para>
        /// The owner of a ListboxItem is typically set by the list box widgets when an item is added or inserted.
        /// </para>
        /// </summary>
        /// <returns>
        /// Pointer to the window that is considered the owner of this ListboxItem.
        /// </returns>
        public Window GetOwnerWindow()
        {
            return Owner;
        }

        /// <summary>
        /// Return the current colours used for selection highlighting.
        /// </summary>
        /// <returns>
        /// ColourRect object describing the currently set colours
        /// </returns>
        public ColourRect GetSelectionColours()
        {
            return SelectCols;
        }

        /// <summary>
        /// Return the current selection highlighting brush.
        /// </summary>
        /// <returns>
        /// Pointer to the Image object currently used for selection highlighting.
        /// </returns>
        public Image GetSelectionBrushImage()
        {
            return SelectBrush;
        }

        /// <summary>
        /// set the text string for this list box item.
        /// <para>
        /// Note that even if the item does not render text, the text string can still be useful, 
        /// since it is used for sorting list box items.
        /// </para>
        /// </summary>
        /// <param name="text">
        /// String object containing the text to set for the list box item.
        /// </param>
        public virtual void SetText(string text)
        {
            _textLogical = text;
            _bidiDataValid = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void SetTooltipText(string text)
        {
            _tooltipText = text;
        }

        /// <summary>
        /// Set the ID assigned to this list box item.
        /// <para>
        /// Note that the system does not make use of this value, client code can assign any meaning it
        /// wishes to the ID.
        /// </para>
        /// </summary>
        /// <param name="itemId">
        /// ID code to be assigned to this list box item
        /// </param>
        public void SetId(int itemId)
        {
            _itemId = itemId;
        }

        /// <summary>
        /// Set the client assigned user data attached to this lis box item.
        /// <para>
        /// Note that the system does not make use of this data, client code can assign any meaning it
        /// wishes to the attached data.
        /// </para>
        /// </summary>
        /// <param name="itemData">
        /// Pointer to the user data to attach to this list item.
        /// </param>
        public void SetUserData(object itemData)
        {
            _itemData = itemData;
        }

        /// <summary>
        /// set whether this item is selected.
        /// </summary>
        /// <param name="setting">
        /// true if the item is selected, 
        /// false if the item is not selected.
        /// </param>
        public void SetSelected(bool setting)
        {
            Selected = setting;
        }

        /// <summary>
        /// set whether this item is disabled.
        /// </summary>
        /// <param name="setting">
        /// true if the item is disabled, false if the item is enabled.
        /// </param>
        public void SetDisabled(bool setting)
        {
            _disabled = setting;
        }

        /// <summary>
        /// Set whether this item will be automatically deleted when the list box it is attached to
        /// is destroyed, or when the item is removed from the list box.
        /// </summary>
        /// <param name="setting">
        /// true if the item object should be deleted by the system when the list box it is attached to is
        /// destroyed, or when the item is removed from the list.  false if client code will destroy the
        /// item after it is removed from the list.
        /// </param>
        public void SetAutoDeleted(bool setting)
        {
            _autoDelete = setting;
        }

        /// <summary>
        /// Set the owner window for this ListboxItem.  This is called by all the list box widgets when
        /// an item is added or inserted.
        /// </summary>
        /// <param name="owner">
        /// Pointer to the window that should be considered the owner of this ListboxItem.
        /// </param>
        public void SetOwnerWindow(Window owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Set the colours used for selection highlighting.
        /// </summary>
        /// <param name="cols">
        /// ColourRect object describing the colours to be used.
        /// </param>
        public void SetSelectionColours(ColourRect cols)
        {
            SelectCols = cols;
        }

        /// <summary>
        /// Set the colours used for selection highlighting.
        /// </summary>
        /// <param name="topLeftColour">
        /// Colour (as ARGB value) to be applied to the top-left corner of the selection area.
        /// </param>
        /// <param name="topRightColour">
        /// Colour (as ARGB value) to be applied to the top-right corner of the selection area.
        /// </param>
        /// <param name="bottomLeftColour">
        /// Colour (as ARGB value) to be applied to the bottom-left corner of the selection area.
        /// </param>
        /// <param name="bottomRightColour">
        /// Colour (as ARGB value) to be applied to the bottom-right corner of the selection area.
        /// </param>
        public void SetSelectionColours(Colour topLeftColour,
                                        Colour topRightColour,
                                        Colour bottomLeftColour,
                                        Colour bottomRightColour)
        {
            SelectCols.d_top_left = topLeftColour;
            SelectCols.d_top_right = topRightColour;
            SelectCols.d_bottom_left = bottomLeftColour;
            SelectCols.d_bottom_right = bottomRightColour;
        }

        /// <summary>
        /// Set the colours used for selection highlighting.
        /// </summary>
        /// <param name="col">
        /// colour value to be used when rendering.
        /// </param>
        public void SetSelectionColours(Colour col)
        {
            SetSelectionColours(col, col, col, col);
        }

        /// <summary>
        /// Set the selection highlighting brush image.
        /// </summary>
        /// <param name="image">
        /// Pointer to the Image object to be used for selection highlighting.
        /// </param>
        public void SetSelectionBrushImage(Image image)
        {
            SelectBrush = image;
        }

        /// <summary>
        /// Set the selection highlighting brush image.
        /// </summary>
        /// <param name="name">
        /// Name of the image to be used
        /// </param>
        public void SetSelectionBrushImage(string name)
        {
            SetSelectionBrushImage(ImageManager.GetSingleton().Get(name));
        }

        /// <summary>
        /// Perform any updates needed because the given font's render size has changed.
        /// <para>The base implementation just returns false.</para>
        /// </summary>
        /// <param name="font">
        /// Pointer to the Font whose render size has changed.
        /// </param>
        /// <returns>
        /// - true if some action was taken.
        /// - false if no action was taken (i.e font is not used here).
        /// </returns>
        public virtual bool HandleFontRenderSizeChange(Font font)
        {
            return false;
        }

        /// <summary>
        /// Return the rendered pixel size of this list box item.
        /// </summary>
        /// <returns>
        /// Size object describing the size of the list box item in pixels.
        /// </returns>
        public abstract Sizef GetPixelSize();

        /// <summary>
        /// Draw the list box item in its current state
        /// </summary>
        /// <param name="targetRect"></param>
        /// <param name="alpha">
        ///     Alpha value to be used when rendering the item (between 0.0f and 1.0f).
        /// </param>
        /// <param name="clipper">
        ///     Rect object describing the clipping rectangle for the draw operation.
        /// </param>
        public abstract List<GeometryBuffer> CreateRenderGeometry(Rectf targetRect, float alpha, Rectf? clipper);

        
        /*!
        \brief
            Less-than operator, compares item texts.
        */
        // TODO: virtual bool    operator<(const ListboxItem& rhs)     {return getText() < rhs.getText(); }


        /*!
        \brief
            Greater-than operator, compares item texts.
        */
        // TODO: virtual bool    operator>(const ListboxItem& rhs) {return getText() > rhs.getText(); }

        public int CompareTo(ListboxItem other)
        {
            if (GetText() == null)
                return -1;
            return GetText().CompareTo(other.GetText());
        }

        /// <summary>
        /// Return a ColourRect object describing the colours in \a cols after having their alpha
        /// component modulated by the value \a alpha.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        protected ColourRect GetModulateAlphaColourRect(ColourRect cols, float alpha)
        {
            return new ColourRect(CalculateModulatedAlphaColour(cols.d_top_left, alpha),
                                  CalculateModulatedAlphaColour(cols.d_top_right, alpha),
                                  CalculateModulatedAlphaColour(cols.d_bottom_left, alpha),
                                  CalculateModulatedAlphaColour(cols.d_bottom_right, alpha));
        }

        /// <summary>
        /// Return a colour value describing the colour specified by \a col after having its alpha
        /// component modulated by the value \a alpha.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        protected Colour CalculateModulatedAlphaColour(Colour col, float alpha)
        {
            var temp = col;
            temp.Alpha = temp.Alpha*alpha;
            return temp;
        }

        #region Fields

        /// <summary>
        /// Pointer to the window that owns this item.
        /// </summary>
        protected Window Owner;

        /// <summary>
        /// true if this item is selected.  false if the item is not selected.
        /// </summary>
        protected bool Selected;

        /// <summary>
        /// Image used for rendering selection.
        /// </summary>
        protected Image SelectBrush;

        /// <summary>
        /// Colours used for selection highlighting.
        /// </summary>
        protected ColourRect SelectCols;
        
        /// <summary>
        /// pointer to bidirection support object
        /// </summary>
        private readonly BidiVisualMapping _bidiVisualMapping;

        /// <summary>
        /// whether bidi visual mapping has been updated since last text change.
        /// </summary>
        private bool _bidiDataValid;

        /// <summary>
        /// Text for the individual tooltip of this item
        /// </summary>
        private string _tooltipText;

        private string _textLogical;

        /// <summary>
        /// ID code assigned by client code.  This has no meaning within the GUI system.
        /// </summary>
        private int _itemId;

        /// <summary>
        /// Pointer to some client code data.  This has no meaning within the GUI system.
        /// </summary>
        private object _itemData;
        
        /// <summary>
        /// true if this item is disabled.  false if the item is not disabled.
        /// </summary>
        private bool _disabled;

        /// <summary>
        /// true if the system should destroy this item, false if client code will destroy the item.
        /// </summary>
        private bool _autoDelete;
        
        #endregion
    }
}