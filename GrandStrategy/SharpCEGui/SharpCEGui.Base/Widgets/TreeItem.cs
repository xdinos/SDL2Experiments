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
    /// Base class for tree items
    /// <remarks>
    /// The CEGUI::Tree, CEGUI::TreeItem and any other associated classes are
    /// deprecated and thier use should be minimised - preferably eliminated -
    /// where possible.  It is extremely unfortunate that this widget was ever added
    /// to CEGUI since its design and implementation are poor and do not meet
    /// established standards for the CEGUI project.
    /// <para>
    /// While no alternative currently exists, a superior, replacement tree widget
    /// will be provided prior to the final removal of the current implementation.
    /// </para>
    /// </remarks>
    /// </summary>
    public class TreeItem
    {
        /// <summary>
        /// Default text colour.
        /// </summary>
        public static readonly Colour DefaultTextColour = new Colour(0xFF4444AA);

        /// <summary>
        /// Default selection brush colour.
        /// </summary>
        public static readonly Colour DefaultSelectionColour = new Colour(0xFFFFFFFF);

        /// <summary>
        /// base class constructor
        /// </summary>
        /// <param name="text"></param>
        /// <param name="itemId"></param>
        /// <param name="itemData"></param>
        /// <param name="disabled"></param>
        /// <param name="autoDelete"></param>
        public TreeItem(string text, int itemId = 0, object itemData = null, bool disabled = false,
                        bool autoDelete = true)
        {
            d_bidiVisualMapping=new NBidiVisualMapping();
            d_bidiDataValid = false;
            d_itemId = itemId;
            d_itemData = itemData;
            d_selected = false;
            d_disabled = disabled;
            d_autoDelete = autoDelete;
            d_buttonLocation = Rectf.Zero;
            d_owner = null;
            d_selectCols = new ColourRect(DefaultSelectionColour);
            d_selectBrush = null;
            d_textCols = new ColourRect(DefaultTextColour);
            d_font = null;
            d_iconImage = null;
            d_isOpen = false;
            d_renderedStringValid = false;

            SetText(text);
        }


        /*!
         \brief
            base class destructor
         */
        // TODO: virtual ~TreeItem();

        /// <summary>
        /// Return a pointer to the font being used by this TreeItem
        /// <para>
        /// This method will try a number of places to find a font to be used.  If
        /// no font can be found, NULL is returned.
        /// </para>
        /// </summary>
        /// <returns>
        /// Font to be used for rendering this item
        /// </returns>
        public Font GetFont()
        {
            // prefer out own font
            if (d_font != null)
                return d_font;

            // try our owner window's font setting
            // (may be null if owner uses non existant default font)
            if (d_owner != null)
                return d_owner.GetFont();
            
            // no owner, just use the default (which may be NULL anyway)
            return System.GetSingleton().GetDefaultGUIContext().GetDefaultFont();   
        }

        /// <summary>
        /// Return the current colours used for text rendering.
        /// </summary>
        /// <returns>
        /// ColourRect object describing the currently set colours
        /// </returns>
        public ColourRect GetTextColours()
        {
            return d_textCols;
        }

        /*************************************************************************
            Manipulator methods
         *************************************************************************/
        /*!
         \brief
            Set the font to be used by this TreeItem
     
         \param font
            Font to be used for rendering this item
     
         \return
            Nothing
         */
        void setFont(Font font)
        {
            throw new NotImplementedException();
        }

        /*!
         \brief
            Set the font to be used by this TreeItem
     
         \param font_name
            String object containing the name of the Font to be used for rendering
            this item
     
         \return
            Nothing
         */
        void setFont(string font_name)
        {
            throw new NotImplementedException();
        }

        /*!
         \brief
            Set the colours used for text rendering.
     
         \param cols
            ColourRect object describing the colours to be used.
     
         \return
            Nothing.
         */
        void setTextColours(ColourRect cols)
        { d_textCols = cols; d_renderedStringValid = false; }

        /*!
         \brief
            Set the colours used for text rendering.
     
         \param top_left_colour
            Colour (as ARGB value) to be applied to the top-left corner of each text
            glyph rendered.
     
         \param top_right_colour
            Colour (as ARGB value) to be applied to the top-right corner of each
            text glyph rendered.
     
         \param bottom_left_colour
            Colour (as ARGB value) to be applied to the bottom-left corner of each
            text glyph rendered.
     
         \param bottom_right_colour
            Colour (as ARGB value) to be applied to the bottom-right corner of each
            text glyph rendered.
     
         \return
            Nothing.
         */
        void setTextColours(Colour top_left_colour, Colour top_right_colour,
                            Colour bottom_left_colour, Colour bottom_right_colour)
        {
            throw new NotImplementedException();
        }

        /*!
         \brief
            Set the colours used for text rendering.
     
         \param col
            colour value to be used when rendering.
     
         \return
            Nothing.
         */
        void setTextColours(Colour col)
        { setTextColours(col, col, col, col); }

        /// <summary>
        /// return the text string set for this tree item.
        /// <para>
        /// Note that even if the item does not render text, the text string can
        /// still be useful, since it is used for sorting tree items.
        /// </para>
        /// </summary>
        /// <returns>
        /// String object containing the current text for the tree item.
        /// </returns>
        public string GetText()
        {
            return d_textLogical;
        }

        /// <summary>
        /// return text string with \e visual ordering of glyphs.
        /// </summary>
        /// <returns></returns>
        public string GetTextVisual()
        {
            // no bidi support
            if (d_bidiVisualMapping == null)
                return d_textLogical;

            if (!d_bidiDataValid)
            {
                d_bidiVisualMapping.UpdateVisual(d_textLogical);
                d_bidiDataValid = true;
            }

            return d_bidiVisualMapping.GetTextVisual();
        }

        /*!
        \brief
            Return the text string currently set to be used as the tooltip text for
            this item.

        \return
            String object containing the current tooltip text as sued by this item.
        */
        public string GetTooltipText()
        {
            return d_tooltipText;
        }

        /// <summary>
        /// Return the current ID assigned to this tree item.
        /// <para>
        /// Note that the system does not make use of this value, client code can assign any meaning it wishes to the ID.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            return d_itemId;
        }

        /// <summary>
        /// Return the pointer to any client assigned user data attached to this tree item.
        /// <para>
        /// Note that the system does not make use of this data, client code can assign any meaning it wishes to the attached data.
        /// </para>
        /// </summary>
        /// <returns>
        /// Pointer to the currently assigned user data.
        /// </returns>
        public object GetUserData()
        {
            return d_itemData;
        }

        /// <summary>
        /// return whether this item is selected.
        /// </summary>
        /// <returns>
        /// - true if the item is selected.
        /// - false if the item is not selected.
        /// </returns>
        public bool IsSelected()
        {
            return d_selected;
        }

        /// <summary>
        /// return whether this item is disabled.
        /// </summary>
        /// <returns>
        /// - true if the item is disabled.
        /// - false if the item is enabled.
        /// </returns>
        public bool IsDisabled()
        {
            return d_disabled;
        }

        /// <summary>
        /// return whether this item will be automatically deleted when it is
        /// removed from the tree or when the the tree it is attached to is
        /// destroyed.
        /// </summary>
        /// <returns>
        /// - true if the item object will be deleted by the system when it is
        ///   removed from the tree, or when the tree it is attached to is destroyed.
        /// - false if client code must destroy the item after it is removed from
        ///   the tree.
        /// </returns>
        public bool IsAutoDeleted()
        {
            return d_autoDelete;
        }

        /// <summary>
        /// Get the owner window for this TreeItem.
        /// <para>
        /// The owner of a TreeItem is typically set by the tree widget when an
        /// item is added or inserted.
        /// </para>
        /// </summary>
        /// <returns>
        /// Ponter to the window that is considered the owner of this TreeItem.
        /// </returns>
        public Window GetOwnerWindow()
        {
            return d_owner;
        }

        /// <summary>
        /// Return the current colours used for selection highlighting.
        /// </summary>
        /// <returns>
        /// ColourRect object describing the currently set colours.
        /// </returns>
        public ColourRect GetSelectionColours()
        {
            return d_selectCols;
        }

        /// <summary>
        /// Return the current selection highlighting brush.
        /// </summary>
        /// <returns>
        /// Pointer to the Image object currently used for selection highlighting.
        /// </returns>
        public Image GetSelectionBrushImage()
        {
            return d_selectBrush;
        }

        /// <summary>
        /// set the text string for this tree item.
        /// <para>
        /// Note that even if the item does not render text, the text string can
        /// still be useful, since it is used for sorting tree items.
        /// </para>
        /// </summary>
        /// <param name="text">
        /// String object containing the text to set for the tree item.
        /// </param>
        public void SetText(string text)
        {
            d_textLogical = text;
            d_bidiDataValid = false;
            d_renderedStringValid = false;
        }

        /*!
        \brief
            Set the tooltip text to be used for this item.

        \param text
            String object holding the text to be used in the tooltip displayed for
            this item.

        \return
            Nothing.
        */
        void setTooltipText(string text)
        { d_tooltipText = text; }

        /*!
         \brief
            Set the ID assigned to this tree item.
     
            Note that the system does not make use of this value, client code can
            assign any meaning it wishes to the ID.
     
         \param item_id
            ID code to be assigned to this tree item
     
         \return
            Nothing.
         */
        public void SetId(int itemId)
        {
            d_itemId = itemId;
        }

        /*!
         \brief
             Set the client assigned user data attached to this lis box item.
        
             Note that the system does not make use of this data, client code can
             assign any meaning it wishes to the attached data.
     
         \param item_data
            Pointer to the user data to attach to this tree item.
     
         \return
            Nothing.
         */
        void setUserData(object item_data)
        { d_itemData = item_data; }

        /*!
         \brief
            Set the selected state for the item.
     
         \param setting
            - true if the item is selected.
            - false if the item is not selected.

         \return
            Nothing.
         */
        public void SetSelected(bool setting)
        {
            d_selected = setting;
        }

        /*!
         \brief
            Set the disabled state for the item.
     
         \param setting
            - true if the item should be disabled.
            - false if the item should be enabled.
     
         \return
            Nothing.
         */
        void setDisabled(bool setting)
        { d_disabled = setting; }

        /*!
         \brief
             Set whether this item will be automatically deleted when it is removed
             from the tree, or when the tree it is attached to is destroyed.
     
         \param setting
             - true if the item object should be deleted by the system when the it
               is removed from the tree, or when the tree it is attached to is
               destroyed.
            - false if client code will destroy the item after it is removed from
              the tree.
     
         \return
            Nothing.
         */
        void setAutoDeleted(bool setting)
        { d_autoDelete = setting; }

        /// <summary>
        /// Set the owner window for this TreeItem. This is called by the tree widget when an item is added or inserted.
        /// </summary>
        /// <param name="owner">
        /// Ponter to the window that should be considered the owner of this TreeItem.
        /// </param>
        public void SetOwnerWindow(Window owner)
        {
            d_owner = owner;
        }

        /// <summary>
        /// Set the colours used for selection highlighting.
        /// </summary>
        /// <param name="cols">
        /// ColourRect object describing the colours to be used.
        /// </param>
        public void SetSelectionColours(ColourRect cols)
        {
            d_selectCols = cols;
        }

        /// <summary>
        /// Set the colours used for selection highlighting.
        /// </summary>
        /// <param name="topLeft">
        /// Colour (as ARGB value) to be applied to the top-left corner of the selection area.
        /// </param>
        /// <param name="topRight">
        /// Colour (as ARGB value) to be applied to the top-right corner of the selection area.
        /// </param>
        /// <param name="bottomLeft">
        /// Colour (as ARGB value) to be applied to the bottom-left corner of the selection area.
        /// </param>
        /// <param name="bottomRight">
        /// Colour (as ARGB value) to be applied to the bottom-right corner of the selection area.
        /// </param>
        public void SetSelectionColours(Colour topLeft, Colour topRight, Colour bottomLeft, Colour bottomRight)
        {
            SetSelectionColours(new ColourRect(topLeft, topRight, bottomLeft, bottomRight));
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
            d_selectBrush = image;
        }

        /// <summary>
        /// Set the selection highlighting brush image.
        /// </summary>
        /// <param name="name">
        /// Name of the image to be used.
        /// </param>
        public void SetSelectionBrushImage(string name)
        {
            SetSelectionBrushImage(ImageManager.GetSingleton().Get(name));
        }

        /*!
         \brief
            Tell the treeItem where its button is located.
            Calculated and set in Tree.cpp.
     
         \param buttonOffset
            Location of the button in screenspace.
         */
        public void SetButtonLocation(Rectf buttonOffset)
        {
            d_buttonLocation = buttonOffset;
        }

        public Rectf GetButtonLocation()
        {
            return d_buttonLocation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetIsOpen()
        {
            return d_isOpen;
        }

        public void ToggleIsOpen()
        {
            d_isOpen = !d_isOpen;
        }

        public TreeItem GetTreeItemFromIndex(int itemIndex)
        {
            if (itemIndex > d_listItems.Count)
                return null;

            return d_listItems[itemIndex];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetItemCount()
        {
            return d_listItems.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TreeItem> GetItemList()
        {
            return d_listItems;
        }

        public void AddItem(TreeItem item)
        {
            if (item != null)
            {
                var parentWindow = (Tree)GetOwnerWindow();
        
                // establish ownership
                item.SetOwnerWindow(parentWindow);
        
                // if sorting is enabled, re-sort the tree
                if (parentWindow.IsSortEnabled())
                {
                    // TODO: d_listItems.insert(std::upper_bound(d_listItems.begin(),d_listItems.end(), item, &lbi_less), item);
                    d_listItems.Add(item);
                }
                // not sorted, just stick it on the end.
                else
                {
                    d_listItems.Add(item);
                }
        
                parentWindow.OnListContentsChanged(new WindowEventArgs(parentWindow));
            }
        }

        void removeItem(TreeItem item)
        {
            throw new NotImplementedException();
        }

        public void SetIcon(Image theIcon)
        {
            d_iconImage = theIcon;
        }

        /*!
         \brief
            Return the rendered pixel size of this tree item.
     
         \return
            Size object describing the size of the tree item in pixels.
         */
        public virtual Sizef GetPixelSize()
        {
            var fnt = GetFont();

            if (fnt == null)
                return Sizef.Zero;

            if (!d_renderedStringValid)
                ParseTextString();

            var sz =Sizef.Zero;

            for (int i = 0; i < d_renderedString.GetLineCount(); ++i)
            {
                var line_sz = d_renderedString.GetPixelSize(d_owner, i);
                sz.Height += line_sz.Height;

                if (line_sz.Width > sz.Width)
                    sz.Width = line_sz.Width;
            }

            return sz;
        }

        /// <summary>
        /// Draw the tree item in its current state
        /// </summary>
        /// <param name="geometryBuffers"></param>
        /// <param name="targetRect"></param>
        /// <param name="alpha">
        /// Alpha value to be used when rendering the item (between 0.0f and 1.0f).
        /// </param>
        /// <param name="clipper">
        /// Rect object describing the clipping rectangle for the draw operation.
        /// </param>
        public virtual IEnumerable<GeometryBuffer> Draw(Rectf targetRect, float alpha, Rectf? clipper)
        {
            var geometryBuffers=new List<GeometryBuffer>();
            var finalRect = targetRect;

            var imgRenderSettings = new ImageRenderSettings(finalRect, clipper, true, new ColourRect(0xffffffff), alpha);

            if (d_iconImage != null)
            {
                var finalPos = finalRect;
                finalPos.Width = targetRect.Height; // xdinos ???
                finalPos.Height = targetRect.Height;
                imgRenderSettings.DestArea = finalPos;
                geometryBuffers.AddRange(d_iconImage.CreateRenderGeometry(imgRenderSettings));
                finalRect.d_min.X += targetRect.Height;
            }

            imgRenderSettings.DestArea = finalRect;

            if (d_selected && d_selectBrush != null)
            {
                imgRenderSettings.MultiplyColours = d_selectCols;
                geometryBuffers.AddRange(d_selectBrush.CreateRenderGeometry(imgRenderSettings));
            }

            var font = GetFont();

            if (font == null)
                return geometryBuffers;

            var draw_pos = finalRect.Position;
            draw_pos.Y -= (font.GetLineSpacing() - font.GetBaseline()) * 0.5f;

            if (!d_renderedStringValid)
                ParseTextString();

            var final_colours = new ColourRect(0xFFFFFFFF);

            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
            {
                //d_renderedString.Draw(d_owner, i, geometryBuffers, draw_pos, final_colours, clipper, 0.0f);
                geometryBuffers.AddRange(d_renderedString.CreateRenderGeometry(d_owner, i, draw_pos, final_colours, clipper, 0.0f));
                draw_pos.Y += d_renderedString.GetPixelSize(d_owner, i).Height;
            }

            return geometryBuffers;
        }

        /// <summary>
        /// Perform any updates needed because the given font's render size has changed.
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
            if (GetFont() == font)
                return true;

            for (var i = 0; i < GetItemCount(); ++i)
            {
                if (d_listItems[i].HandleFontRenderSizeChange(font))
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// Less-than operator, compares item texts.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator <(TreeItem lhs, TreeItem rhs)
        {
            return lhs.GetText().CompareTo(rhs.GetText()) == 1;
        }

        /// <summary>
        /// Greater-than operator, compares item texts.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator >(TreeItem lhs, TreeItem rhs)
        {
            return lhs.GetText().CompareTo(rhs.GetText()) == -1;
        }

        /// <summary>
        /// Return a ColourRect object describing the colours in \a cols after
        /// having their alpha component modulated by the value \a alpha.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        protected ColourRect GetModulateAlphaColourRect(ColourRect cols, float alpha)
        {
            return new ColourRect
                (
                    CalculateModulatedAlphaColour(cols.d_top_left, alpha),
                    CalculateModulatedAlphaColour(cols.d_top_right, alpha),
                    CalculateModulatedAlphaColour(cols.d_bottom_left, alpha),
                    CalculateModulatedAlphaColour(cols.d_bottom_right, alpha)
                );
        }

        /// <summary>
        /// Return a colour value describing the colour specified by \a col after
        /// having its alpha component modulated by the value \a alpha.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        protected Colour CalculateModulatedAlphaColour(Colour col, float alpha)
        {
            Colour temp = col;
            temp.Alpha = temp.Alpha*alpha;
            return temp;
        }

        /// <summary>
        /// parse the text visual string into a RenderString representation.
        /// </summary>
        protected void ParseTextString()
        {
            d_renderedString = d_stringParser.Parse(GetTextVisual(), GetFont(), d_textCols);
            d_renderedStringValid = true;
        }

        
        //! Text for this tree item.  If not rendered, still used for sorting.
        String               d_textLogical;            //!< text rendered by this component.
        //! pointer to bidirection support object
        BidiVisualMapping d_bidiVisualMapping;
        //! whether bidi visual mapping has been updated since last text change.
        /*mutable*/ bool d_bidiDataValid;
        //! Text for the individual tooltip of this item.
        String d_tooltipText;
        //! ID code assigned by client code.
        int d_itemId;
        //! Pointer to some client code data.
        object d_itemData;
        //! true if item is selected.  false if item is not selected.
        bool d_selected;
        //! true if item is disabled.  false if item is not disabled.
        bool d_disabled;
        //! true if the system will destroy this item, false if client code will.
        bool d_autoDelete;
        //! Location of the 'expand' button for the item.
        Rectf d_buttonLocation;
        //! Pointer to the window that owns this item.
        Window d_owner;
        //! Colours used for selection highlighting.
        ColourRect d_selectCols;
        //! Image used for rendering selection.
        Image d_selectBrush;
        //! Colours used for rendering the text.
        ColourRect d_textCols;
        //! Font used for rendering text.
        Font d_font;
        //! Image for the icon to be displayed with this TreeItem.
        Image d_iconImage;
        //! list of items in this item's tree branch.
        List<TreeItem> d_listItems = new List<TreeItem>();
        //! true if the this item's tree branch is opened.
        bool d_isOpen;
        //! Parser used to produce a final RenderedString from the standard String.
        private static BasicRenderedStringParser d_stringParser = new BasicRenderedStringParser();
        //! RenderedString drawn by this item.
        /*mutable*/ RenderedString  d_renderedString;
        //! boolean used to track when item state changes (and needs re-parse)
        /*mutable*/ bool d_renderedStringValid;
    }
}