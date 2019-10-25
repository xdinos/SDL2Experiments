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
    /// Simple grid index structure.
    /// </summary>
    public struct MCLGridRef
    {
        public MCLGridRef(int r, int c)
        {
            row = r;
            column = c;
        }

        public int	row;		//!< Zero based row index.
	    public int	column;		//!< Zero based column index.

        // TODO: operators
        //MCLGridRef& operator=(MCLGridRef rhs);
        //bool operator<(MCLGridRef rhs){ throw new NotImplementedException(); }
        //bool operator<=(MCLGridRef rhs){ throw new NotImplementedException(); }
        //bool operator>(MCLGridRef rhs){ throw new NotImplementedException(); }
        //bool operator>=(MCLGridRef rhs){ throw new NotImplementedException(); }
        //bool operator==(MCLGridRef rhs){ throw new NotImplementedException(); }
        //bool operator!=(MCLGridRef rhs){ throw new NotImplementedException(); }
    }

    /// <summary>
    /// Base class for the multi column list window renderer.
    /// </summary>
    public abstract class MultiColumnListWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected MultiColumnListWindowRenderer(string name)
            : base(name, MultiColumnList.EventNamespace)
        {
            
        }

        /// <summary>
        /// Return a Rect object describing, in un-clipped pixels, 
        /// the window relative area that is to be used for rendering list items.
        /// </summary>
        /// <returns>
        /// Rect object describing the area of the Window to be used for rendering list box items.
        /// </returns>
        public abstract Rectf GetListRenderArea();
    }

    /// <summary>
    /// Base class for the multi column list widget.
    /// </summary>
    public class MultiColumnList : Window
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "MultiColumnList";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/MultiColumnList";

        #region Events

        /// <summary>
        /// Event fired when the selection mode for the list box changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList whose selection mode
        /// has been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SelectionModeChanged;

        /// <summary>
        /// Event fired when the nominated select column changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList whose nominated
        /// selection column has been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> NominatedSelectColumnChanged;

        /// <summary>
        /// Event fired when the nominated select row changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList whose nominated
        /// selection row has been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> NominatedSelectRowChanged;

        /// <summary>
        /// Event fired when the vertical scroll bar 'force' setting changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList whose vertical scroll
        /// bar mode has been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> VertScrollbarModeChanged;

        /// <summary>
        /// Event fired when the horizontal scroll bar 'force' setting changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList whose horizontal
        /// scroll bar mode has been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> HorzScrollbarModeChanged;
        
        /// <summary>
        /// Event fired when the current selection(s) within the list box changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList whose current
        /// selection has changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SelectionChanged;

        /// <summary>
        /// Event fired when the contents of the list box changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList whose contents has
        /// changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> ListContentsChanged;

        /// <summary>
        /// Event fired when the sort column changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList whose sort column has
        /// been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SortColumnChanged;

        /// <summary>
        /// Event fired when the sort direction changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList whose sort direction
        /// has been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SortDirectionChanged;

        /// <summary>
        /// Event fired when the width of a column in the list changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList for which a column
        /// width has changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> ListColumnSized;

        /// <summary>
        /// Event fired when the column order changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MultiColumnList for which the order
        /// of columns has been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> ListColumnMoved;

        #endregion

        /// <summary>
        /// Widget name for the vertical scrollbar component.
        /// </summary>
        public const string VertScrollbarName = "__auto_vscrollbar__";

        /// <summary>
        /// Widget name for the horizontal scrollbar component.
        /// </summary>
        public const string HorzScrollbarName = "__auto_hscrollbar__";

        /// <summary>
        /// Widget name for the list header component.
        /// </summary>
        public const string ListHeaderName = "__auto_listheader__";

        /// <summary>
        /// Enumerated values for the selection modes possible with a Multi-column list
        /// </summary>
        public enum SelectionMode
        {
            /// <summary>
            /// Any single row may be selected.  All items in the row are selected.
            /// </summary>
            RowSingle,

            /// <summary>
            /// Multiple rows may be selected.  All items in the row are selected.
            /// </summary>
            RowMultiple,

            /// <summary>
            /// Any single cell may be selected.
            /// </summary>
            CellSingle,

            /// <summary>
            /// Multiple cells bay be selected.
            /// </summary>
            CellMultiple,

            /// <summary>
            /// Any single item in a nominated column may be selected.
            /// </summary>
            NominatedColumnSingle,

            /// <summary>
            /// Multiple items in a nominated column may be selected.
            /// </summary>
            NominatedColumnMultiple,

            /// <summary>
            /// Any single column may be selected.  All items in the column are selected.
            /// </summary>
            ColumnSingle,

            /// <summary>
            /// Multiple columns may be selected.  All items in the column are selected.
            /// </summary>
            ColumnMultiple,

            /// <summary>
            /// Any single item in a nominated row may be selected.
            /// </summary>
            NominatedRowSingle,

            /// <summary>
            /// Multiple items in a nominated row may be selected.
            /// </summary>
            NominatedRowMultiple
        }

        /// <summary>
        /// Return whether user manipulation of the sort column and direction are enabled.
        /// </summary>
        /// <returns>
        /// true if the user may interactively modify the sort column and direction.  
        /// false if the user may not modify the sort column and direction (these can still be set programmatically).
        /// </returns>
	    public bool IsUserSortControlEnabled()
	    {
            return GetListHeader().IsSortingEnabled();
	    }

        /// <summary>
        /// Return whether the user may size column segments.
        /// </summary>
        /// <returns>
        /// true if the user may interactively modify the width of columns, false if they may not.
        /// </returns>
	    public bool IsUserColumnSizingEnabled()
	    {
            return GetListHeader().IsColumnSizingEnabled();
	    }

        /// <summary>
        /// Return whether the user may modify the order of the columns.
        /// </summary>
        /// <returns>
        /// true if the user may interactively modify the order of the columns, false if they may not.
        /// </returns>
	    public bool IsUserColumnDraggingEnabled()
	    {
            return GetListHeader().IsColumnDraggingEnabled();
	    }

        /// <summary>
        /// Return the number of columns in the multi-column list
        /// </summary>
        /// <returns>
        /// int value equal to the number of columns in the list.
        /// </returns>
	    public int GetColumnCount()
	    {
            return d_columnCount;
	    }

        /// <summary>
        /// Return the number of rows in the multi-column list.
        /// </summary>
        /// <returns>
        /// int value equal to the number of rows currently in the list.
        /// </returns>
	    public int GetRowCount()
        {
            return d_grid.Count;
        }

        /// <summary>
        /// Return the zero based index of the current sort column.  
        /// There must be at least one column to successfully call this method.
        /// </summary>
        /// <returns>
        /// Zero based column index that is the current sort column.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if there are no columns in this multi column list.
        /// </exception>
        public int GetSortColumn()
        {
            return GetListHeader().GetSortColumn();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
	    public int GetSortColumnId()
	    {
            if (GetColumnCount() > 0)
                return GetColumnId(GetSortColumn());
            return 0;
	    }

        /// <summary>
        /// Return the zero based column index of the column with the specified ID.
        /// </summary>
        /// <param name="colId">
        /// ID code of the column whos index is to be returned.
        /// </param>
        /// <returns>
        /// Zero based column index of the first column whos ID matches \a col_id.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if no attached column has the requested ID.
        /// </exception>
	    public int GetColumnWithId(int colId)
	    {
            return GetListHeader().GetColumnFromId(colId);
	    }

        /// <summary>
        /// Return the zero based index of the column whos header text matches the specified text.
        /// </summary>
        /// <param name="text">
        /// String object containing the text to be searched for.
        /// </param>
        /// <returns>
        /// Zero based column index of the column whos header has the specified text.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if no columns header has the requested text.
        /// </exception>
	    public int GetColumnWithHeaderText(string text)
	    {
            return GetListHeader().GetColumnWithText(text);
	    }

        /// <summary>
        /// Return the total width of all column headers.
        /// </summary>
        /// <returns>
        /// Sum total of all the column header widths as a UDim.
        /// </returns>
	    public UDim GetTotalColumnHeadersWidth()
	    {
	        var header = GetListHeader();
            var width = UDim.Zero;

            for (var i = 0; i < GetColumnCount(); ++i)
                width += header.GetColumnWidth(i);

            return width;
	    }

        /// <summary>
        /// Return the width of the specified column header (and therefore the column itself).
        /// </summary>
        /// <param name="colIdx">
        /// Zero based column index of the column whos width is to be returned.
        /// </param>
        /// <returns>
        /// Width of the column header at the zero based column index specified by \a col_idx, as a UDim
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a column is out of range.
        /// </exception>
	    public UDim GetColumnHeaderWidth(int colIdx)
	    {
            return GetListHeader().GetColumnWidth(colIdx);
	    }

        /// <summary>
        /// Return the currently set sort direction.
        /// </summary>
        /// <returns>
        /// One of the ListHeaderSegment.SortDirection enumerated values specifying the current sort direction.
        /// </returns>
	    public ListHeaderSegment.SortDirection GetSortDirection()
	    {
            return GetListHeader().GetSortDirection();
	    }

        /// <summary>
        /// Return the ListHeaderSegment object for the specified column
        /// </summary>
        /// <param name="colIdx">
        /// zero based index of the column whos ListHeaderSegment is to be returned.
        /// </param>
        /// <returns>
        /// ListHeaderSegment object for the column at the requested index.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a col_idx is out of range.
        /// </exception>
	    public ListHeaderSegment GetHeaderSegmentForColumn(int colIdx)
	    {
            return GetListHeader().GetSegmentFromColumn(colIdx);
	    }

        /// <summary>
        /// Return the zero based index of the Row that contains \a item.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem that the row index is to returned for.
        /// </param>
        /// <returns>
        /// Zero based index of the row that contains ListboxItem \a item.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to the list box.
        /// </exception>
	    public int GetItemRowIndex(ListboxItem item)
	    {
            for (var i = 0; i < GetRowCount(); ++i)
            {
                if (IsListboxItemInRow(item, i))
                    return i;
            }

            // item is not attached to the list box, throw...
            throw new InvalidRequestException("the given ListboxItem is not attached to this MultiColumnList.");
	    }

        /// <summary>
        /// Return the current zero based index of the column that contains \a item.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem that the column index is to returned for.
        /// </param>
        /// <returns>
        /// Zero based index of the column that contains ListboxItem \a item.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to the list box.
        /// </exception>
	    public int GetItemColumnIndex(ListboxItem item)
	    {
            for (var i = 0; i < GetColumnCount(); ++i)
            {
                if (IsListboxItemInColumn(item, i))
                    return i;
            }

            // item is not attached to the list box, throw...
            throw new InvalidRequestException("the given ListboxItem is not attached to this MultiColumnList.");
	    }

        /// <summary>
        /// Return the grid reference for \a item.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem whos current grid reference is to be returned.
        /// </param>
        /// <returns>
        /// MCLGridRef object describing the current grid reference of ListboxItem \a item.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to the list box.
        /// </exception>
	    public MCLGridRef GetItemGridReference(ListboxItem item)
	    {
            return new MCLGridRef(GetItemRowIndex(item), GetItemColumnIndex(item));
	    }

        /// <summary>
        /// Return a pointer to the ListboxItem at the specified grid reference.
        /// </summary>
        /// <param name="gridRef">
        /// MCLGridRef object that describes the position of the ListboxItem to be returned.
        /// </param>
        /// <returns>
        /// Pointer to the ListboxItem at grid reference \a grid_ref.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a grid_ref is invalid for this list box.
        /// </exception>
        public ListboxItem GetItemAtGridReference(MCLGridRef gridRef)
        {
            // check for invalid grid ref
            if (gridRef.column >= GetColumnCount())
                throw new InvalidRequestException("the column given in the grid reference is out of range.");

            if (gridRef.row >= GetRowCount())
                throw new InvalidRequestException("the row given in the grid reference is out of range.");

            return d_grid[gridRef.row][gridRef.column];
        }

        /// <summary>
        /// return whether ListboxItem \a item is attached to the column at index \a col_idx.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem to look for.
        /// </param>
        /// <param name="colIdx">
        /// Zero based index of the column that is to be searched.
        /// </param>
        /// <returns>
        /// - true if \a item is attached to list box column \a col_idx.
        /// - false if \a item is not attached to list box column \a col_idx.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a col_idx is out of range.
        /// </exception>
	    public bool IsListboxItemInColumn(ListboxItem item, int colIdx)
	    {
            // check for invalid index
            if (colIdx >= GetColumnCount())
                throw new InvalidRequestException("the column index given is out of range.");

            for (var i = 0; i < GetRowCount(); ++i)
            {
                if (d_grid[i][colIdx] == item)
                    return true;
            }

            // Item was not in the column.
            return false;
	    }

        /// <summary>
        /// return whether ListboxItem \a item is attached to the row at index \a row_idx.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem to look for.
        /// </param>
        /// <param name="rowIdx">
        /// Zero based index of the row that is to be searched.
        /// </param>
        /// <returns>
        /// - true if \a item is attached to list box row \a row_idx.
        /// - false if \a item is not attached to list box row \a row_idx.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a row_idx is out of range.
        /// </exception>
	    public bool IsListboxItemInRow(ListboxItem item, int rowIdx)
	    {
            // check for invalid index
            if (rowIdx >= GetRowCount())
                throw new InvalidRequestException("the row index given is out of range.");

            for (var i = 0; i < GetColumnCount(); ++i)
            {
                if (d_grid[rowIdx][i] == item)
                {
                    return true;
                }

            }

            // Item was not in the row.
            return false;
	    }

        /// <summary>
        /// return whether ListboxItem \a item is attached to the list box.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem to look for.
        /// </param>
        /// <returns>
        /// - true if \a item is attached to list box.
        /// - false if \a item is not attached to list box.
        /// </returns>
	    public bool IsListboxItemInList(ListboxItem item)
	    {
            for (var i = 0; i < GetRowCount(); ++i)
            {
                for (var j = 0; j < GetColumnCount(); ++j)
                {
                    if (d_grid[i][j] == item)
                        return true;
                }
            }

            return false;
	    }

        /// <summary>
        /// Return the ListboxItem in column \a col_idx that has the text string \a text.
        /// </summary>
        /// <param name="text">
        /// String object containing the text to be searched for.
        /// </param>
        /// <param name="colIdx">
        /// Zero based index of the column to be searched.
        /// </param>
        /// <param name="startItem">
        /// Pointer to the ListboxItem where the exclusive search is to start, or NULL to search from the top of the column.
        /// </param>
        /// <returns>
        /// Pointer to the first ListboxItem in column \a col_idx, after \a start_item, that has the string \a text.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a start_item is not attached to the list box, or if \a col_idx is out of range.
        /// </exception>
	    public ListboxItem FindColumnItemWithText(string text, int colIdx, ListboxItem startItem)
	    {
            // ensure column is valid
            if (colIdx >= GetColumnCount())
                throw new InvalidRequestException("specified column index is out of range.");
            
            // find start position for search
            var i = (startItem == null) ? 0 : GetItemRowIndex(startItem) + 1;

            for (; i < GetRowCount(); ++i)
            {
                // does this item match?
                if (d_grid[i][colIdx].GetText() == text)
                    return d_grid[i][colIdx];
            }

            // no matching item.
            return null;
	    }

        /// <summary>
        /// Return the ListboxItem in row \a row_idx that has the text string \a text.
        /// </summary>
        /// <param name="text">
        /// String object containing the text to be searched for.
        /// </param>
        /// <param name="rowIdx">
        /// Zero based index of the row to be searched.
        /// </param>
        /// <param name="startItem">
        /// Pointer to the ListboxItem where the exclusive search is to start, or NULL to search from the start of the row.
        /// </param>
        /// <returns>
        /// Pointer to the first ListboxItem in row \a row_idx, after \a start_item, that has the string \a text.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a start_item is not attached to the list box, or if \a row_idx is out of range.
        /// </exception>
	    public ListboxItem FindRowItemWithText(string text, int rowIdx, ListboxItem startItem)
	    {
            // ensure row is valid
            if (rowIdx >= GetRowCount())
                throw new InvalidRequestException("specified row index is out of range.");
            
            // find start position for search
            var i = (startItem == null) ? 0 : GetItemColumnIndex(startItem) + 1;

            for (; i < GetColumnCount(); ++i)
            {
                // does this item match?
                if (d_grid[rowIdx][i].GetText() == text)
                    return d_grid[rowIdx][i];
            }

            // no matching item.
            return null;
	    }

        /// <summary>
        /// Return the ListboxItem that has the text string \a text.
        /// <para>
        /// List box searching progresses across the columns in each row.
        /// </para>
        /// </summary>
        /// <param name="text">
        /// String object containing the text to be searched for.
        /// </param>
        /// <param name="startItem">
        /// Pointer to the ListboxItem where the exclusive search is to start, or NULL to search the whole list box.
        /// </param>
        /// <returns>
        /// Pointer to the first ListboxItem, after \a start_item, that has the string \a text.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a start_item is not attached to the list box.
        /// </exception>
	    public ListboxItem FindListItemWithText(string text, ListboxItem startItem)
	    {
	        var startRef=new MCLGridRef(0, 0);

	        // get position of start_item if it's not NULL
            if (startItem != null)
            {
                startRef = GetItemGridReference(startItem);
                ++startRef.column;
            }

            // perform the search
	        for (var i = startRef.row; i < GetRowCount(); ++i)
	        {
		        for (var j = startRef.column; j < GetColumnCount(); ++j)
		        {
			        // does this item match?
			        if (d_grid[i][j].GetText() == text)
				        return d_grid[i][j];
		        }
	        }

	        // No match
	        return null;
	    }

        /// <summary>
        /// Return a pointer to the first selected ListboxItem attached to this list box.
        /// <para>
        /// List box searching progresses across the columns in each row.
        /// </para>
        /// </summary>
        /// <returns>
        /// Pointer to the first ListboxItem attached to this list box that is selected, or NULL if no item is selected.
        /// </returns>
	    public ListboxItem GetFirstSelectedItem()
	    {
            return GetNextSelected(null);
	    }

        /// <summary>
        /// Return a pointer to the next selected ListboxItem after \a start_item.
        /// <para>
        /// List box searching progresses across the columns in each row.
        /// </para>
        /// </summary>
        /// <param name="startItem">
        /// Pointer to the ListboxItem where the exclusive search is to start, or NULL to search the whole list box.
        /// </param>
        /// <returns>
        /// Pointer to the first selected ListboxItem attached to this list box, after \a start_item, or NULL if no item is selected.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a start_item is not attached to the list box.
        /// </exception>
	    public ListboxItem GetNextSelected(ListboxItem startItem)
	    {
	        var startRef=new MCLGridRef(0, 0);

	        // get position of start_item if it's not NULL
	        if (startItem!=null)
	        {
		        startRef = GetItemGridReference(startItem);
		        if (++startRef.column == GetColumnCount())
		        {
		            startRef.column = 0;
		            ++startRef.row;
		        }
	        }

	        // perform the search
	        for (var i = startRef.row; i < GetRowCount(); ++i)
	        {
		        for (var j = startRef.column; j < GetColumnCount(); ++j)
		        {
			        // does this item match?
			        var item = d_grid[i][j];

			        if ((item != null) && item.IsSelected())
			            return d_grid[i][j];
			    }
	        }

	        // No match
	        return null;
	    }

        /// <summary>
        /// Return the number of selected ListboxItems attached to this list box.
        /// </summary>
        /// <returns>
        /// int value equal to the number of ListboxItems attached to this list box that are currently selected.
        /// </returns>
	    public int GetSelectedCount()
	    {
            var count = 0;

            for (var i = 0; i < GetRowCount(); ++i)
            {
                for (var j = 0; j < GetColumnCount(); ++j)
                {
                    var item = d_grid[i][j];

                    if ((item != null) && item.IsSelected())
                        ++count;
                }
            }

            return count;
	    }

        /// <summary>
        /// Return whether the ListboxItem at \a grid_ref is selected.
        /// </summary>
        /// <param name="gridRef">
        /// MCLGridRef object describing the grid reference that is to be examined.
        /// </param>
        /// <returns>
        /// - true if there is a ListboxItem at \a grid_ref and it is selected.
        /// - false if there is no ListboxItem at \a grid_ref, or if the item is not selected.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a grid_ref contains an invalid grid position.
        /// </exception>
	    public bool IsItemSelected(MCLGridRef gridRef)
	    {
            var item = GetItemAtGridReference(gridRef);

            if (item!=null)
                return item.IsSelected();
            
            // if no item exists here, then it can't be selected.
            return false;
	    }

        /// <summary>
        /// Return the ID of the currently set nominated selection column to be used when in one of the NominatedColumn*
        /// selection modes. There must be at least one column to successfully call this method.
        /// <para>
        /// You should only ever call this when GetColumnCount() returns > 0.
        /// </para>
        /// </summary>
        /// <returns>
        /// Id code of the nominated selection column.
        /// </returns>
	    public int GetNominatedSelectionColumnId()
	    {
            if (GetColumnCount() > 0)
                return GetListHeader().GetSegmentFromColumn(d_nominatedSelectCol).GetId();

            return 0;
	    }

        /// <summary>
        /// Return the index of the currently set nominated selection column to be used when in one of the NominatedColumn*
        /// selection modes.
        /// </summary>
        /// <returns>
        /// Zero based index of the nominated selection column.
        /// </returns>
	    public int GetNominatedSelectionColumn()
	    {
            return d_nominatedSelectCol;
	    }

        /// <summary>
        /// Return the index of the currently set nominated selection row to be used when in one of the NominatedRow*
        /// selection modes.
        /// </summary>
        /// <returns>
        /// Zero based index of the nominated selection column.
        /// </returns>
	    public int GetNominatedSelectionRow()
	    {
            return d_nominatedSelectRow;
	    }

        /// <summary>
        /// Return the currently set selection mode.
        /// </summary>
        /// <returns>
        /// One of the MultiColumnList.SelectionMode enumerated values specifying the current selection mode.
        /// </returns>
	    public SelectionMode GetSelectionMode()
	    {
            return d_selectMode;
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
            return d_forceVertScroll;
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
            return d_forceHorzScroll;
	    }

        /// <summary>
        /// Return the ID code assigned to the requested column.
        /// </summary>
        /// <param name="colIdx">
        /// Zero based index of the column whos ID code is to be returned.
        /// </param>
        /// <returns>
        /// Current ID code assigned to the column at the requested index.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a col_idx is out of range
        /// </exception>
	    public int GetColumnId(int colIdx)
	    {
            return GetListHeader().GetSegmentFromColumn(colIdx).GetId();
	    }

        /// <summary>
        /// Return the ID code assigned to the requested row.
        /// </summary>
        /// <param name="rowIdx">
        /// Zero based index of the row who's ID code is to be returned.
        /// </param>
        /// <returns>
        /// Current Id code assigned to the row at the requested index.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a row_idx is out of range
        /// </exception>
        public int GetRowId(int rowIdx)
        {
            // check for invalid index
            if (rowIdx >= GetRowCount())
                throw new InvalidRequestException("the row index given is out of range.");
            
            return d_grid[rowIdx].d_rowID;
        }

        /// <summary>
        /// Return the zero based row index of the row with the specified ID.
        /// </summary>
        /// <param name="rowId">
        /// ID code of the row who's index is to be returned.
        /// </param>
        /// <returns>
        /// Zero based row index of the first row who's ID matches \a row_id.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if no row has the requested ID.
        /// </exception>
	    public int GetRowWithId(int rowId)
	    {
            for (var i = 0; i < GetRowCount(); ++i)
            {
                if (d_grid[i].d_rowID == rowId)
                    return i;
            }

            // No such row found, throw exception
            throw new InvalidRequestException("no row with the requested ID is present.");
	    }

        /// <summary>
        /// Return a Rect object describing, in un-clipped pixels, the window relative area
        /// that is to be used for rendering list items.
        /// </summary>
        /// <returns>
        /// Rect object describing the area of the Window to be used for rendering list box items.
        /// </returns>
        public Rectf GetListRenderArea()
        {
            if (d_windowRenderer != null)
                return ((MultiColumnListWindowRenderer) d_windowRenderer).GetListRenderArea();

            throw new InvalidRequestException("This function must be implemented by the window renderer module");
        }

        /// <summary>
        /// Return a pointer to the vertical scrollbar component widget for this MultiColumnList.
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
        /// Return a pointer to the horizontal scrollbar component widget for this MultiColumnList.
        /// </summary>
        /// <returns>
        /// Pointer to a Scrollbar object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the horizontal Scrollbar component does not exist.
        /// </exception>
        public Scrollbar GetHorzScrollbar()
        {
            return (Scrollbar)GetChild(HorzScrollbarName);
        }

        /// <summary>
        /// Return a pointer to the list header component widget for this MultiColumnList.
        /// </summary>
        /// <returns>
        /// Pointer to a ListHeader object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the list header component does not exist.
        /// </exception>
        public ListHeader GetListHeader()
        {
            return (ListHeader)GetChild(ListHeaderName);
        }

        /// <summary>
        /// Return the sum of all row heights in pixels.
        /// </summary>
        /// <returns></returns>
        public float GetTotalRowsHeight()
        {
            var height = 0.0f;
            for (var i = 0; i < GetRowCount(); ++i)
                height += GetHighestRowItemHeight(i);

            return height;
        }

        /// <summary>
        /// Return the pixel width of the widest item in the given column
        /// </summary>
        /// <param name="colIdx"></param>
        /// <returns></returns>
        public float GetWidestColumnItemWidth(int colIdx)
        {
            if (colIdx >= GetColumnCount())
                throw new InvalidRequestException("specified column is out of range.");
            
            var width = 0.0f;

		    // check each item in the column
		    for (var i = 0; i < GetRowCount(); ++i)
		    {
			    var item = d_grid[i][colIdx];

			    // if the slot has an item in it
			    if (item!=null)
			    {
			        var sz = item.GetPixelSize();

				    // see if this item is wider than the previous widest
				    if (sz.Width > width)
				    {
					    // update current widest
					    width = sz.Width;
				    }
			    }
		    }

		    // return the widest item.
		    return width;
        }

        /// <summary>
        /// Return, in pixels, the height of the highest item in the given row.
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <returns></returns>
        public float GetHighestRowItemHeight(int rowIdx)
        {
            if (rowIdx >= GetRowCount())
                throw new InvalidRequestException("specified row is out of range.");
            
            var height = 0.0f;

            // check each item in the column
            for (var i = 0; i < GetColumnCount(); ++i)
            {
                var item = d_grid[rowIdx][i];

                // if the slot has an item in it
                if (item!=null)
                {
                    var sz = item.GetPixelSize();

                    // see if this item is higher than the previous highest
                    if (sz.Height > height)
                    {
                        // update current highest
                        height = sz.Height;
                    }

                }

            }

            // return the hightest item.
            return height;
        }

        /// <summary>
        /// Get whether or not column auto-sizing (autoSizeColumnHeader()) will use
        /// the list header segment size.
        /// </summary>
        /// <returns>
        /// Return true if the header segment will be included in the width calculation.
        /// </returns>
        public bool GetAutoSizeColumnUsesHeader()
        {
            return d_autoSizeColumnUsesHeader;
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
            // get the component sub-widgets
	        var vertScrollbar = GetVertScrollbar();
	        var horzScrollbar = GetHorzScrollbar();
	        var header       = GetListHeader();

	        // subscribe some events
            header.SegmentRenderOffsetChanged += HandleHeaderScroll;
            header.SegmentSequenceChanged += HandleHeaderSegMove;
            header.SegmentSized += HandleColumnSizeChange;
            header.SortColumnChanged += HandleSortColumnChange;
            header.SortDirectionChanged += HandleSortDirectionChange;
            header.SplitterDoubleClicked += HandleHeaderSegDblClick;
            horzScrollbar.ScrollPositionChanged += HandleHorzScrollbar;
            vertScrollbar.ScrollPositionChanged += HandleVertScrollbar;


	        // final initialisation now widget is complete
	        SetSortDirection(ListHeaderSegment.SortDirection.None);

	        // Perform initial layout
	        ConfigureScrollbars();
	        PerformChildWindowLayout();
        }

        /// <summary>
        /// Remove all items from the list.
        /// <para>
        /// Note that this will cause 'AutoDelete' items to be deleted.
        /// </para>
        /// </summary>
	    public void ResetList()
	    {
	        if (ResetListImpl())
		        OnListContentsChanged(new WindowEventArgs(this));
	    }

        /// <summary>
        /// Add a column to the list box.
        /// </summary>
        /// <param name="text">
        /// String object containing the text label for the column header.
        /// </param>
        /// <param name="colId">
        /// ID code to be assigned to the column header.
        /// </param>
        /// <param name="width">
        /// UDim describing the initial width to be set for the column.
        /// </param>
	    public void	AddColumn(string text, int colId, UDim width)
	    {
            InsertColumn(text, colId, width, GetColumnCount());
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
	    public void AddColumn(string value)
	    {
	        var idstart = value.LastIndexOf("id:");
            var wstart = value.LastIndexOf("width:");
            var capstart = value.IndexOf("text:");

            // some defaults in case of missing data
            var caption = String.Empty;
            var id = "0";
            var width = "{0.33,0}";

            // extract the caption field
            if (capstart != -1)
            {
                capstart = value.IndexOf(":") + 1;

                if (wstart == -1)
                    if (idstart == -1)
                        caption = value.Substring(capstart);
                    else
                        caption = value.Substring(capstart, idstart - capstart);
                else
                    caption = value.Substring(capstart, wstart - capstart);

                // trim trailing whitespace
                TextUtils.TrimTrailingChars(ref caption, TextUtils.DefaultWhitespace);
            }

            // extract the width field
            if (wstart != -1)
            {
                width = value.Substring(wstart);
                width = width.Substring(width.IndexOf("{"));
                width = width.Substring(0, width.LastIndexOf("}") + 1);
            }

            // extract the id field.
            if (idstart != -1)
            {
                id = value.Substring(idstart);
                id = id.Substring(id.IndexOf(":") + 1);
            }

            // add the column accordingly
            AddColumn(caption, PropertyHelper.FromString<int>(id), PropertyHelper.FromString<UDim>(width));
	    }

        /// <summary>
        /// Insert a new column in the list.
        /// </summary>
        /// <param name="text">
        /// String object containing the text label for the column header.
        /// </param>
        /// <param name="colId">
        /// ID code to be assigned to the column header.
        /// </param>
        /// <param name="width">
        /// UDim describing the initial width to be set for the column.
        /// </param>
        /// <param name="position">
        /// Zero based index where the column is to be inserted.  If this is greater than the current
        /// number of columns, the new column is inserted at the end.
        /// </param>
        public void InsertColumn(string text, int colId, UDim width, int position)
        {
            // if position is out of range, add item to end of current columns.
	        if (position > GetColumnCount())
		        position = GetColumnCount();

	        // set-up the header for the new column.
	        GetListHeader().InsertColumn(text, colId, width, position);
            ++d_columnCount;

            // Set the font equal to that of our list
            var segment = GetHeaderSegmentForColumn(position);
            segment.SetFont(d_font);
            // ban properties from segment that we control from here.
            segment.BanPropertyFromXML("ID");
            segment.BanPropertyFromXML("Text");
            segment.BanPropertyFromXML("Font");

	        // Insert a blank entry at the appropriate position in each row.
	        for (var i = 0; i < GetRowCount(); ++i)
	        {
                d_grid[i].d_items.Insert(position, null);
	        }

	        // update stored nominated selection column if that has changed.
	        if ((d_nominatedSelectCol >= position) && (GetColumnCount() > 1))
	        {
		        d_nominatedSelectCol++;
	        }

	        // signal a change to the list contents
	        OnListContentsChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Removes a column from the list box.  
        /// This will cause any ListboxItem using the autoDelete option in the column to be deleted.
        /// </summary>
        /// <param name="colIdx">
        /// Zero based index of the column to be removed.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a col_idx is invalid.
        /// </exception>
	    public void RemoveColumn(int colIdx)
	    {
	        // ensure index is valid, and throw if not.
            if (colIdx >= GetColumnCount())
                throw new InvalidRequestException("the specified column index is out of range.");

            // update stored column index values
            if (d_nominatedSelectCol == colIdx)
                d_nominatedSelectCol = 0;
            
            // remove the column from each row
            for (var i = 0; i < GetRowCount(); ++i)
            {
                // extract the item pointer.
                var item = d_grid[i][colIdx];

                // remove the column entry from the row
                d_grid[i].d_items.RemoveAt(colIdx);

                // delete the ListboxItem as needed.
                if ((item != null) && item.IsAutoDeleted())
                {
                    // TODO: CEGUI_DELETE_AO item;
                }
            }

            // remove header segment
            GetListHeader().RemoveColumn(colIdx);
            --d_columnCount;

            // signal a change to the list contents
            OnListContentsChanged(new WindowEventArgs(this));
	    }

        /// <summary>
        /// Removes a column from the list box.  
        /// This will cause any ListboxItem using the autoDelete option in the column to be deleted.
        /// </summary>
        /// <param name="colId">
        /// ID code of the column to be deleted.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if no column with \a col_id is available on this list box.
        /// </exception>
	    public void RemoveColumnWithId(int colId)
	    {
            RemoveColumn(GetColumnWithId(colId));
	    }

        /// <summary>
        /// Move the column at index \a col_idx so it is at index \a position.
        /// </summary>
        /// <param name="colIdx">
        /// Zero based index of the column to be moved.
        /// </param>
        /// <param name="position">
        /// Zero based index of the new position for the column.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a col_idx is invalid.
        /// </exception>
	    public void MoveColumn(int colIdx, int position)
	    {
            // move the segment on the header, events will ensure the items get moved also.
            GetListHeader().MoveColumn(colIdx, position);
	    }

        /// <summary>
        /// Move the column with ID \a col_id so it is at index \a position.
        /// </summary>
        /// <param name="colId">
        /// ID code of the column to be moved.
        /// </param>
        /// <param name="position">
        /// Zero based index of the new position for the column.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if no column with \a col_id is available on this list box.
        /// </exception>
	    public void MoveColumnWithId(int colId, int position)
	    {
            MoveColumn(GetColumnWithId(colId), position);
	    }

        /// <summary>
        /// Add an empty row to the list box.
        /// </summary>
        /// <param name="rowId">
        /// ID code to be assigned to the new row.
        /// </param>
        /// <returns>
        /// Initial zero based index of the new row.
        /// </returns>
        /// <remarks>
        /// If the list is being sorted, the new row will appear at an appropriate position according to the sorting being
        /// applied.  If no sorting is being done, the new row will appear at the bottom of the list.
        /// </remarks>
	    public int AddRow(int rowId = 0)
	    {
            return AddRow(null, 0, rowId);
	    }
        
        /// <summary>
        /// Add a row to the list box, and set the item in the column with ID \a col_id to \a item.
        /// </summary>
        /// <param name="item">
        /// Pointer to a ListboxItem to be used as the initial contents for the column with ID \a col_id.
        /// </param>
        /// <param name="colId">
        /// ID code of the column whos initial item is to be set to \a item.
        /// </param>
        /// <param name="rowId">
        /// ID code to be assigned to the new row.
        /// </param>
        /// <returns>
        /// Initial zero based index of the new row.
        /// </returns>
        /// <remarks>
        /// If the list is being sorted, the new row will appear at an appropriate position according to the sorting being
        /// applied.  If no sorting is being done, the new row will appear at the bottom of the list.
        /// </remarks>
        /// <exception cref="InvalidRequestException">
        /// thrown if no column with the specified ID is attached to the list box.
        /// </exception>
	    public int AddRow(ListboxItem item, int colId, int rowId = 0)
	    {
            // Build the new row
            var row = new ListRow
                          {
                              d_rowID = rowId,
                              d_sortColumn = GetSortColumn()
                          };

            // TODO: row.d_items.resize(getColumnCount(), 0);
            for (var i=0;i<GetColumnCount();i++) row.d_items.Add(null);
	        

	        if (item!=null)
	        {
		        // discover which column to initially set
		        var colIdx = GetColumnWithId(colId);

		        // establish item ownership & enter item into column
		        item.SetOwnerWindow(this);
		        row[colIdx] = item;
	        }

	        int pos;

	        // if sorting is enabled, insert at an appropriate position
            var dir = GetSortDirection();
	        if (dir != ListHeaderSegment.SortDirection.None)
	        {
                // TODO: 
                // calculate where the row should be inserted
	            pos = d_grid.BinarySearch(row);
	            pos = pos < 0 ? ~pos : pos;
	            pos = dir == ListHeaderSegment.SortDirection.Descending ? pos - 1 : pos;

	            pos = pos < 0 ? 0 : pos;

	            d_grid.Insert(pos, row);
	        }
	        else
	        {
                // not sorted, just stick it on the end.
		        pos = GetRowCount();
		        d_grid.Add(row);
	        }

	        // signal a change to the list contents
	        OnListContentsChanged(new WindowEventArgs(this));

	        return pos;
	    }

        /// <summary>
        /// Insert an empty row into the list box.
        /// </summary>
        /// <param name="rowIdx">
        /// Zero based index where the row should be inserted.  
        /// If this is greater than the current number of rows, the row is
        /// appended to the list.
        /// </param>
        /// <param name="rowId">
        /// ID code to be assigned to the new row.
        /// </param>
        /// <returns>
        /// Zero based index where the row was actually inserted.
        /// </returns>
        /// <remarks>
        /// If the list is being sorted, the new row will appear at an appropriate position according to the sorting being
        /// applied.  If no sorting is being done, the new row will appear at the specified index.
        /// </remarks>
	    public int InsertRow(int rowIdx, int rowId = 0)
	    {
            return InsertRow(null, 0, rowIdx, rowId);
	    }

        /// <summary>
        /// Insert a row into the list box, and set the item in the column with ID \a col_id to \a item.
        /// </summary>
        /// <param name="item">
        /// Pointer to a ListboxItem to be used as the initial contents for the column with ID \a col_id.
        /// </param>
        /// <param name="colId">
        /// ID code of the column whos initial item is to be set to \a item.
        /// </param>
        /// <param name="rowIdx">
        /// Zero based index where the row should be inserted.  If this is greater than the current number of rows, the row is
        /// appended to the list.
        /// </param>
        /// <param name="rowId">
        /// ID code to be assigned to the new row.
        /// </param>
        /// <returns>
        /// Zero based index where the row was actually inserted.
        /// </returns>
        /// <remarks>
        /// If the list is being sorted, the new row will appear at an appropriate position according to the sorting being
        /// applied.  If no sorting is being done, the new row will appear at the specified index.
        /// </remarks>
        /// <exception cref="InvalidRequestException">
        /// thrown if no column with the specified ID is attached to the list box.
        /// </exception>
        public int InsertRow(ListboxItem item, int colId, int rowIdx, int rowId = 0)
        {
            // if sorting is enabled, use add instead of insert
	        if (GetSortDirection() != ListHeaderSegment.SortDirection.None)
	            return AddRow(item, colId, rowId);
	        
            // Build the new row (empty)
            var row = new ListRow {d_rowID = rowId, d_sortColumn = GetSortColumn()};
            // TODO: row.d_items.resize(getColumnCount(), 0);
            for (var i = 0; i < GetColumnCount(); i++) row.d_items.Add(null);
		    

		    // if row index is too big, just insert at end.
		    if (rowIdx > GetRowCount())
			    rowIdx = GetRowCount();

		    d_grid.Insert(rowIdx, row);

		    // set the initial item in the new row
		    SetItem(item, colId, rowIdx);

		    // signal a change to the list contents
		    OnListContentsChanged(new WindowEventArgs(this));

		    return rowIdx;
        }

        /// <summary>
        /// Remove the list box row with index \a row_idx.  Any ListboxItem in row \a row_idx using autoDelete mode will be deleted.
        /// </summary>
        /// <param name="rowIdx">
        /// Zero based index of the row to be removed.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a row_idx is invalid.
        /// </exception>
	    public void RemoveRow(int rowIdx)
	    {
	        // ensure row exists
            if (rowIdx >= GetRowCount())
                throw new InvalidRequestException("The specified row index is out of range.");

            // delete items we are supposed to
            for (var i = 0; i < GetColumnCount(); ++i)
            {
                var item = d_grid[rowIdx][i];

                if ((item != null) && item.IsAutoDeleted())
                {
                    // TODO: CEGUI_DELETE_AO item;
                }

            }

            // erase the row from the grid.
            d_grid.RemoveAt(rowIdx);

            // if we have erased the selection row, reset that to 0
            if (d_nominatedSelectRow == rowIdx)
            {
                d_nominatedSelectRow = 0;
            }

            // signal a change to the list contents
            OnListContentsChanged(new WindowEventArgs(this));
	    }

        /// <summary>
        /// Set the ListboxItem for grid reference \a position.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem to be set at \a position.
        /// </param>
        /// <param name="position">
        /// MCLGridRef describing the grid reference of the item to be set.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a position contains an invalid grid reference.
        /// </exception>
	    public void SetItem(ListboxItem item, MCLGridRef position)
	    {
	        // validate grid ref
	        if (position.column >= GetColumnCount())
	            throw new InvalidRequestException("the specified column index is invalid.");

            if (position.row >= GetRowCount())
                throw new InvalidRequestException("the specified row index is invalid.");
	        
	        // delete old item as required
	        var oldItem = d_grid[position.row][position.column];

	        if ((oldItem != null) && oldItem.IsAutoDeleted())
	        {
		        // TODO: CEGUI_DELETE_AO oldItem;
	        }

	        // set new item.
	        if (item!=null)
		        item.SetOwnerWindow(this);

	        d_grid[position.row][position.column] = item;

	        // signal a change to the list contents
	        OnListContentsChanged(new WindowEventArgs(this));
	    }

        /// <summary>
        /// Set the ListboxItem for the column with ID \a col_id in row \a row_idx.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem to be set into the list.
        /// </param>
        /// <param name="colId">
        /// ID code of the column to receive \a item.
        /// </param>
        /// <param name="rowIdx">
        /// Zero based index of the row to receive \a item.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if no column with ID \a col_id exists, or of \a row_idx is out of range.
        /// </exception>
	    public void SetItem(ListboxItem item, int colId, int rowIdx)
	    {
            SetItem(item, new MCLGridRef(rowIdx, GetColumnWithId(colId)));
	    }

        /// <summary>
        /// Set the selection mode for the list box.
        /// </summary>
        /// <param name="selectionMode">
        /// One of the MultiColumnList::SelectionMode enumerated values specifying the selection mode to be used.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if the value specified for \a sel_mode is invalid.
        /// </exception>
	    public void SetSelectionMode(SelectionMode selectionMode)
	    {
            if (d_selectMode != selectionMode)
            {
                d_selectMode = selectionMode;

                ClearAllSelections();

                switch (d_selectMode)
                {
                    case SelectionMode.RowSingle:
                        d_multiSelect = false;
                        d_fullRowSelect = true;
                        d_fullColSelect = false;
                        d_useNominatedCol = false;
                        d_useNominatedRow = false;
                        break;

                    case SelectionMode.RowMultiple:
                        d_multiSelect = true;
                        d_fullRowSelect = true;
                        d_fullColSelect = false;
                        d_useNominatedCol = false;
                        d_useNominatedRow = false;
                        break;

                    case SelectionMode.CellSingle:
                        d_multiSelect = false;
                        d_fullRowSelect = false;
                        d_fullColSelect = false;
                        d_useNominatedCol = false;
                        d_useNominatedRow = false;
                        break;

                    case SelectionMode.CellMultiple:
                        d_multiSelect = true;
                        d_fullRowSelect = false;
                        d_fullColSelect = false;
                        d_useNominatedCol = false;
                        d_useNominatedRow = false;
                        break;

                    case SelectionMode.NominatedColumnSingle:
                        d_multiSelect = false;
                        d_fullRowSelect = false;
                        d_fullColSelect = false;
                        d_useNominatedCol = true;
                        d_useNominatedRow = false;
                        break;

                    case SelectionMode.NominatedColumnMultiple:
                        d_multiSelect = true;
                        d_fullRowSelect = false;
                        d_fullColSelect = false;
                        d_useNominatedCol = true;
                        d_useNominatedRow = false;
                        break;

                    case SelectionMode.ColumnSingle:
                        d_multiSelect = false;
                        d_fullRowSelect = false;
                        d_fullColSelect = true;
                        d_useNominatedCol = false;
                        d_useNominatedRow = false;
                        break;

                    case SelectionMode.ColumnMultiple:
                        d_multiSelect = true;
                        d_fullRowSelect = false;
                        d_fullColSelect = true;
                        d_useNominatedCol = false;
                        d_useNominatedRow = false;
                        break;

                    case SelectionMode.NominatedRowSingle:
                        d_multiSelect = false;
                        d_fullRowSelect = false;
                        d_fullColSelect = false;
                        d_useNominatedCol = false;
                        d_useNominatedRow = true;
                        break;

                    case SelectionMode.NominatedRowMultiple:
                        d_multiSelect = true;
                        d_fullRowSelect = false;
                        d_fullColSelect = false;
                        d_useNominatedCol = false;
                        d_useNominatedRow = true;
                        break;

                    default:
                        throw new InvalidRequestException("invalid or unknown SelectionMode value supplied.");
                }

                // Fire event.
                OnSelectionModeChanged(new WindowEventArgs(this));
            }
	    }

        /// <summary>
        /// Set the column to be used for the NominatedColumn* selection modes.
        /// </summary>
        /// <param name="columnId">
        /// ID code of the column to be used in NominatedColumn* selection modes.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if no column has ID code \a col_id.
        /// </exception>
	    public void SetNominatedSelectionColumnId(int columnId)
	    {
            SetNominatedSelectionColumn(GetColumnWithId(columnId));
	    }

        /// <summary>
        /// Set the column to be used for the NominatedColumn* selection modes.
        /// </summary>
        /// <param name="columnIndex">
        /// zero based index of the column to be used in NominatedColumn* selection modes.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a col_idx is out of range.
        /// </exception>
	    public void SetNominatedSelectionColumn(int columnIndex)
	    {
	        if (d_nominatedSelectCol != columnIndex)
	        {
		        ClearAllSelections();

		        d_nominatedSelectCol = columnIndex;

		        // Fire event.
		        OnNominatedSelectColumnChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Set the row to be used for the NominatedRow* selection modes.
        /// </summary>
        /// <param name="rowIndex">
        /// zero based index of the row to be used in NominatedRow* selection modes.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a row_idx is out of range.
        /// </exception>
	    public void SetNominatedSelectionRow(int rowIndex)
	    {
	        if (d_nominatedSelectRow != rowIndex)
	        {
		        ClearAllSelections();

		        d_nominatedSelectRow = rowIndex;

		        // Fire event.
		        OnNominatedSelectRowChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Set the sort direction to be used.
        /// </summary>
        /// <param name="direction">
        /// One of the ListHeaderSegment.SortDirection enumerated values specifying the sort direction to be used.
        /// </param>
	    public void SetSortDirection(ListHeaderSegment.SortDirection direction)
	    {
            if (GetSortDirection() != direction)
            {
                // set the sort direction on the header, events will make sure everything else is updated.
                GetListHeader().SetSortDirection(direction);
            }
	    }

        /// <summary>
        /// Set the column to be used as the sort key.
        /// </summary>
        /// <param name="columnIndex">
        /// Zero based index of the column to use as the key when sorting the list items.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if col_idx is out of range.
        /// </exception>
	    public void SetSortColumn(int columnIndex)
	    {
            if (GetSortColumn() != columnIndex)
            {
                // set the sort column on the header, events will make sure everything else is updated.
                GetListHeader().SetSortColumn(columnIndex);
            }
	    }

        /// <summary>
        /// Set the column to be used as the sort key.
        /// </summary>
        /// <param name="columnId">
        /// ID code of the column to use as the key when sorting the list items.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if col_id is invalid for this list box.
        /// </exception>
	    public void SetSortColumnById(int columnId)
	    {
            var header = GetListHeader();

            if (header.GetSegmentFromColumn(GetSortColumn()).GetId() != columnId)
            {
                // set the sort column on the header, events will make sure everything else is updated.
                header.SetSortColumnFromId(columnId);
            }
	    }

        /// <summary>
        /// Set whether the vertical scroll bar should always be shown, or just when needed.
        /// </summary>
        /// <param name="setting">
        /// - true to have the vertical scroll bar shown at all times.
        /// - false to have the vertical scroll bar appear only when needed.
        /// </param>
	    public void SetShowVertScrollbar(bool setting)
	    {
	        if (d_forceVertScroll != setting)
	        {
		        d_forceVertScroll = setting;

		        ConfigureScrollbars();

		        // Event firing.
		        OnVertScrollbarModeChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Set whether the horizontal scroll bar should always be shown, or just when needed.
        /// </summary>
        /// <param name="setting">
        /// - true to have the horizontal scroll bar shown at all times.
        /// - false to have the horizontal scroll bar appear only when needed.
        /// </param>
	    public void SetShowHorzScrollbar(bool setting)
	    {
	        if (d_forceHorzScroll != setting)
	        {
		        d_forceHorzScroll = setting;

		        ConfigureScrollbars();

		        // Event firing.
		        OnHorzScrollbarModeChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Removed the selected state from any currently selected ListboxItem attached to the list.
        /// </summary>
        public void ClearAllSelections()
        {
            // only fire events and update if we actually made any changes
	        if (ClearAllSelectionsImpl())
	        {
		        // Fire event.
		        OnSelectionChanged(new WindowEventArgs(this));
	        }
        }

        /// <summary>
        /// Sets or clears the selected state of the given ListboxItem which must be attached to the list.
        /// </summary>
        /// <param name="item">
        /// Pointer to the attached ListboxItem to be affected.
        /// </param>
        /// <param name="state">
        /// - true to put the ListboxItem into the selected state.
        /// - false to put the ListboxItem into the de-selected state.
        /// </param>
        /// <remarks>
        /// Depending upon the current selection mode, this may cause other items to be selected, other
        /// items to be deselected, or for nothing to actually happen at all.
        /// </remarks>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to the list box.
        /// </exception>
	    public void SetItemSelectState(ListboxItem item, bool state)
	    {
            SetItemSelectState(GetItemGridReference(item), state);
	    }

        /// <summary>
        /// Sets or clears the selected state of the ListboxItem at the given grid reference.
        /// </summary>
        /// <param name="gridRef">
        /// MCLGridRef object describing the position of the item to be affected.
        /// </param>
        /// <param name="state">
        /// - true to put the ListboxItem into the selected state.
        /// - false to put the ListboxItem into the de-selected state.
        /// </param>
        /// <remarks>
        /// Depending upon the current selection mode, this may cause other items to be selected, other
        /// items to be deselected, or for nothing to actually happen at all.
        /// </remarks>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a grid_ref is invalid for this list box.
        /// </exception>
	    public void SetItemSelectState(MCLGridRef gridRef, bool state)
	    {
	        if (SetItemSelectStateImpl(gridRef, state))
	        {
		        // Fire event.
		        OnSelectionChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Inform the list box that one or more attached ListboxItems have been externally modified, and
        /// the list should re-sync its internal state and refresh the display as needed.
        /// </summary>
	    public void HandleUpdatedItemData()
	    {
            ResortList();
            ConfigureScrollbars();
            Invalidate(false);
	    }

        /// <summary>
        /// Set the width of the specified column header (and therefore the column itself).
        /// </summary>
        /// <param name="columnIndex">
        /// Zero based column index of the column whos width is to be set.
        /// </param>
        /// <param name="width">
        /// UDim value specifying the new width for the column.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a column is out of range.
        /// </exception>
	    public void SetColumnHeaderWidth(int columnIndex, UDim width)
	    {
            GetListHeader().SetColumnWidth(columnIndex, width);
	    }

        /// <summary>
        /// Set whether user manipulation of the sort column and direction are enabled.
        /// </summary>
        /// <param name="setting">
        /// - true if the user may interactively modify the sort column and direction.
        /// - false if the user may not modify the sort column and direction (these can still be set programmatically).
        /// </param>
	    public void SetUserSortControlEnabled(bool setting)
	    {
            GetListHeader().SetSortingEnabled(setting);
	    }

        /// <summary>
        /// Set whether the user may size column segments.
        /// </summary>
        /// <param name="setting">
        /// - true if the user may interactively modify the width of columns.
        /// - false if the user may not change the width of the columns.
        /// </param>
	    public void SetUserColumnSizingEnabled(bool setting)
	    {
            GetListHeader().SetColumnSizingEnabled(setting);
	    }

        /// <summary>
        /// Set whether the user may modify the order of the columns.
        /// </summary>
        /// <param name="setting">
        /// - true if the user may interactively modify the order of the columns.
        /// - false if the user may not modify the order of the columns.
        /// </param>
	    public void SetUserColumnDraggingEnabled(bool setting)
	    {
            GetListHeader().SetColumnDraggingEnabled(setting);
	    }

        /// <summary>
        /// Automatically determines the "best fit" size for the specified column and sets
        /// the column width to the same.
        /// </summary>
        /// <param name="columnIndex">
        /// Zero based index of the column to be sized.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a col_idx is out of range.
        /// </exception>
	    public void AutoSizeColumnHeader(int columnIndex)
	    {
	        // check for invalid index
            if (columnIndex >= GetColumnCount())
                throw new InvalidRequestException("the column index given is out of range.");
	    
	        // get the width of the widest item in the column.
		    var width = Math.Max(GetWidestColumnItemWidth(columnIndex), ListHeader.MinimumSegmentPixelWidth);

		    // set new column width
		    SetColumnHeaderWidth(columnIndex, UDim.Absolute(width));
	    }

        /// <summary>
        /// Set the ID code assigned to a given row.
        /// </summary>
        /// <param name="rowIndex">
        /// Zero based index of the row who's ID code is to be set.
        /// </param>
        /// <param name="rowId">
        /// ID code to be assigned to the row at the requested index.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a row_idx is out of range
        /// </exception>
        public void SetRowId(int rowIndex, int rowId)
        {
            // check for invalid index
            if (rowIndex >= GetRowCount())
            {
                throw new InvalidRequestException("the row index given is out of range.");
            }

            d_grid[rowIndex].d_rowID = rowId;
        }

        /// <summary>
        /// Ensure the specified item is made visible within the multi-column listbox.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem to be made visible in the multi-column listbox.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this multicolumnlist.
        /// </exception>
        public void EnsureItemIsVisible(ListboxItem item)
        {
            // NB: throws InvalidRequestException if non-existant item
            EnsureItemIsVisible(GetItemGridReference(item));
        }

        /// <summary>
        /// Ensure the item at the specified grid coordinate is visible within the multi-column listbox.
        /// </summary>
        /// <param name="gridRef">
        /// MCLGridRef object holding the grid coordinate that is to be made visible.
        /// </param>
        public void EnsureItemIsVisible(MCLGridRef gridRef)
        {
            EnsureRowIsVisible(gridRef.row);
            EnsureColumnIsVisible(gridRef.column);
        }

        /// <summary>
        /// Ensure that the row of the \a item is visible within the multi-column listbox.
        /// <para>
        /// This doesn't necessarily make \a item visible.
        /// </para>
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem whose row is to be made visible in the multi-column listbox.
        /// </param>
        public void EnsureItemRowIsVisible(ListboxItem item)
        {
            // NB: throws InvalidRequestException if non-existant item
            EnsureRowIsVisible(GetItemGridReference(item).row);
        }

        /// <summary>
        /// Ensure that the column of \a item is visible within the multi-column listbox.
        /// <para>
        /// This doesn't necessarily make \a item visible.
        /// </para>
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem whose column is to be made visible in the multi-column listbox.
        /// </param>
        public void EnsureItemColumnIsVisible(ListboxItem item)
        {
            // NB: throws InvalidRequestException if non-existant item
            EnsureColumnIsVisible(GetItemGridReference(item).column);
        }

        /// <summary>
        /// Ensure that the row with index \a row_idx is visible within the multi-column listbox.
        /// </summary>
        /// <param name="rowIndex">
        /// row_idx is the zero-based index of the row to be made visible.
        /// </param>
        public void EnsureRowIsVisible(int rowIndex)
        {
            var rows = GetRowCount();

            var vertScrollbar = GetVertScrollbar();

            // handle horizontal scrolling
            // handle simple "scroll to the bottom" case
            if (rowIndex >= rows)
            {
                vertScrollbar.SetScrollPosition(vertScrollbar.GetDocumentSize() - vertScrollbar.GetPageSize());
            }
            else
            {
                var top = 0.0f;
                var listHeight = GetListRenderArea().Height;

                // get distance to top of item
                int row;
                for (row = 0; row < rowIndex; ++row)
                    top += GetHighestRowItemHeight(row);

                // calculate distance to bottom of item
                var bottom = top + GetHighestRowItemHeight(row);

                // account for current scrollbar value
                var currPos = vertScrollbar.GetScrollPosition();
                top -= currPos;
                bottom -= currPos;

                // if top edge is above the view area, or if the item is too big to fit
                if ((top < 0.0f) || ((bottom - top) > listHeight))
                {
                    // scroll top of item to top of box.
                    vertScrollbar.SetScrollPosition(currPos + top);
                }
                // if the bottom edge is below the view area
                else if (bottom >= listHeight)
                {
                    // position bottom of item at the bottom of the list
                    vertScrollbar.SetScrollPosition(currPos + bottom - listHeight);
                }
            }
        }

        /// <summary>
        /// Ensure that the column with ID \a column_idx is visible within the multi-column listbox.
        /// </summary>
        /// <param name="columnIndex">
        /// column_idx is the zero-based index of the column to be made visible.
        /// </param>
        public void EnsureColumnIsVisible(int columnIndex)
        {
            var cols = GetColumnCount();
            var horzScrollbar = GetHorzScrollbar();

            // handle horizontal scrolling
            // first the simple "scroll to the right edge" case
            if (columnIndex >= cols)
            {
                horzScrollbar.SetScrollPosition(horzScrollbar.GetDocumentSize() - horzScrollbar.GetPageSize());
            }
            else
            {
                var left = 0.0f;
                var listWidth = GetListRenderArea().Width;

                // get distance to left edge of item
                int col;
                for (col = 0; col < columnIndex; ++col)
                    left += CoordConverter.AsAbsolute(GetColumnHeaderWidth(col), GetParentPixelSize().Width);

                // get the distance to the right edge of the item
                var right = left + CoordConverter.AsAbsolute(GetColumnHeaderWidth(col), GetParentPixelSize().Width);

                // account for current scrollbar value
                var currPos = horzScrollbar.GetScrollPosition();
                left    -= currPos;
                right   -= currPos;

                // if the left edge is to the left of the view area, or if the item is
                // too big to fit
                if ((left < 0.0f) || ((right - left) > listWidth))
                {
                    // scroll left edge of item to left edge of box.
                    horzScrollbar.SetScrollPosition(currPos + left);
                }
                // if right edge is to the right of the view area
                else if (right >= listWidth)
                {
                    // position the right edge of the item at the right edge of the list
                    horzScrollbar.SetScrollPosition(currPos + right - listWidth);
                }
            }
        }

        /// <summary>
        /// Instruct column auto-sizing (autoSizeColumnHeader()) to also use the list header segment size.
        /// </summary>
        /// <param name="includeHeader">
        /// Whether method autoSizeColumnHeader() also should use the size of the column header segment.
        /// </param>
        public void SetAutoSizeColumnUsesHeader(bool includeHeader)
        {
            d_autoSizeColumnUsesHeader = includeHeader;
        }

        /// <summary>
        /// Constructor for the Multi-column list base class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public MultiColumnList(string type, string name)
            : base(type, name)
        {
            d_forceVertScroll = false;
            d_forceHorzScroll = false;
            d_nominatedSelectCol = 0;
            d_nominatedSelectRow = 0;
            d_lastSelected = null;
            d_columnCount = 0;
            d_autoSizeColumnUsesHeader = false;

            // add properties
	        AddMultiColumnListProperties();

	        // set default selection mode
	        d_selectMode = SelectionMode.CellSingle;		// hack to ensure call below does what it should.
	        SetSelectionMode(SelectionMode.RowSingle);
        }


        // TODO: Destructor for the multi-column list base class.
	    // TODO: virtual ~MultiColumnList()
        // TODO: {
        // TODO:     // delete any items we are supposed to
        // TODO:     resetList_impl();
        // TODO: }

        /// <summary>
        /// display required integrated scroll bars according to current state of the list box and update their values.
        /// </summary>
	    protected void ConfigureScrollbars()
	    {
	        var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();
	        float totalHeight	= GetTotalRowsHeight();
	        float fullWidth		= GetListHeader().GetTotalSegmentsPixelExtent();

	        //
	        // First show or hide the scroll bars as needed (or requested)
	        //
	        // show or hide vertical scroll bar as required (or as specified by option)
	        if ((totalHeight > GetListRenderArea().Height) || d_forceVertScroll)
	        {
		        vertScrollbar.Show();

		        // show or hide horizontal scroll bar as required (or as specified by option)
		        if ((fullWidth > GetListRenderArea().Width) || d_forceHorzScroll)
		        {
			        horzScrollbar.Show();
		        }
		        else
		        {
			        horzScrollbar.Hide();
		        }

	        }
	        else
	        {
		        // show or hide horizontal scroll bar as required (or as specified by option)
		        if ((fullWidth > GetListRenderArea().Width) || d_forceHorzScroll)
		        {
			        horzScrollbar.Show();

			        // show or hide vertical scroll bar as required (or as specified by option)
			        if ((totalHeight > GetListRenderArea().Height) || d_forceVertScroll)
			        {
				        vertScrollbar.Show();
			        }
			        else
			        {
				        vertScrollbar.Hide();
			        }

		        }
		        else
		        {
			        vertScrollbar.Hide();
			        horzScrollbar.Hide();
		        }

	        }

	        //
	        // Set up scroll bar values
	        //
            var renderArea = GetListRenderArea();

	        vertScrollbar.SetDocumentSize(totalHeight);
	        vertScrollbar.SetPageSize(renderArea.Height);
	        vertScrollbar.SetStepSize(Math.Max(1.0f, renderArea.Height / 10.0f));
	        vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition());

	        horzScrollbar.SetDocumentSize(fullWidth);
	        horzScrollbar.SetPageSize(renderArea.Width);
	        horzScrollbar.SetStepSize(Math.Max(1.0f, renderArea.Width / 10.0f));
	        horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition());
	    }

        /// <summary>
        /// select all strings between positions \a start and \a end.  (inclusive).  
        /// Returns true if something was modified.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        protected bool SelectRange(MCLGridRef start, MCLGridRef end)
        {
            MCLGridRef tmpStart = start;
            MCLGridRef tmpEnd = end;

            // ensure start is before end
            if (tmpStart.column > tmpEnd.column)
            {
                tmpStart.column = tmpEnd.column;
                tmpEnd.column = start.column;
            }

            if (tmpStart.row > tmpEnd.row)
            {
                tmpStart.row = tmpEnd.row;
                tmpEnd.row = start.row;
            }

            var modified = false;

            // loop through all items selecting them.
            for (var i = tmpStart.row; i <= tmpEnd.row; ++i)
            {
                for (var j = tmpStart.column; j <= tmpEnd.column; ++j)
                {
                    var item = d_grid[i][j];

                    if (item != null)
                    {
                        modified |= SetItemSelectStateImpl(GetItemGridReference(item), true);
                    }
                }
            }

            return modified;
        }

        /// <summary>
        /// Clear the selected state for all items (implementation)
        /// </summary>
        /// <returns>
        /// true if some selections were cleared, false nothing was changed.
        /// </returns>
	    protected bool ClearAllSelectionsImpl()
	    {
            // flag used so we can track if we did anything.
            var modified = false;

            for (var i = 0; i < GetRowCount(); ++i)
            {
                for (var j = 0; j < GetColumnCount(); ++j)
                {
                    var item = d_grid[i][j];

                    // if slot has an item, and item is selected
                    if ((item != null) && item.IsSelected())
                    {
                        // clear selection state and set modified flag
                        item.SetSelected(false);
                        modified = true;
                    }

                }

            }

            // signal whether or not we did anything.
            return modified;
	    }

        /// <summary>
        /// Return the ListboxItem under the given window local pixel co-ordinate.
        /// </summary>
        /// <param name="pt">
        /// ListboxItem that is under window pixel co-ordinate \a pt, or NULL if no 
        /// item is under that position.
        /// </param>
        /// <returns></returns>
        protected ListboxItem GetItemAtPoint(Lunatics.Mathematics.Vector2 pt)
	    {
	        var header = GetListHeader();
            var listArea = GetListRenderArea();

            var y = listArea.d_min.Y - GetVertScrollbar().GetScrollPosition();
            var x = listArea.d_min.X - GetHorzScrollbar().GetScrollPosition();

            for (var i = 0; i < GetRowCount(); ++i)
            {
                y += GetHighestRowItemHeight(i);

                // have we located the row?
                if (pt.Y < y)
                {
                    // scan across to find column that was clicked
                    for (var j = 0; j < GetColumnCount(); ++j)
                    {
                        var seg = header.GetSegmentFromColumn(j);
                        x += CoordConverter.AsAbsolute(seg.GetWidth(), header.GetPixelSize().Width);

                        // was this the column?
                        if (pt.X < x)
                        {
                            // return contents of grid element that was clicked.
                            return d_grid[i][j];
                        }
                    }
                }
            }

            return null;
	    }

        /// <summary>
        /// Set select state for the given item.  This appropriately selects other 
        /// items depending upon the select mode.  Returns true if something is
        /// changed, else false.
        /// </summary>
        /// <param name="gridRef"></param>
        /// <param name="state"></param>
        /// <returns></returns>
	    protected bool SetItemSelectStateImpl(MCLGridRef gridRef, bool state)
	    {
            // validate grid ref
            if (gridRef.column >= GetColumnCount())
                throw new InvalidRequestException("the specified column index is invalid.");

            if (gridRef.row >= GetRowCount())
                throw new InvalidRequestException("the specified row index is invalid.");
            
            // only do this if the setting is changing
            if (d_grid[gridRef.row][gridRef.column].IsSelected() != state)
            {
                // if using nominated selection row and/ or column, check that they match.
                if ((!d_useNominatedCol || (d_nominatedSelectCol == gridRef.column)) &&
                    (!d_useNominatedRow || (d_nominatedSelectRow == gridRef.row)))
                {
                    // clear current selection if not multi-select box
                    if (state && (!d_multiSelect))
                    {
                        ClearAllSelectionsImpl();
                    }

                    // full row?
                    if (d_fullRowSelect)
                    {
                        // clear selection on all items in the row
                        SetSelectForItemsInRow(gridRef.row, state);
                    }
                    // full column?
                    else if (d_fullColSelect)
                    {
                        // clear selection on all items in the column
                        SetSelectForItemsInColumn(gridRef.column, state);
                    }
                    // single item to be affected
                    else
                    {
                        d_grid[gridRef.row][gridRef.column].SetSelected(state);
                    }

                    return true;
                }

            }

            return false;
	    }

        /// <summary>
        /// Set select state for all items in the given row
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="state"></param>
        protected void SetSelectForItemsInRow(int rowIndex, bool state)
        {
            for (var i = 0; i < GetColumnCount(); ++i)
            {
                var item = d_grid[rowIndex][i];

                if (item != null)
                {
                    item.SetSelected(state);
                }
            }
        }

        /// <summary>
        /// Set select state for all items in the given column
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="state"></param>
        protected void SetSelectForItemsInColumn(int columnIndex, bool state)
        {
            for (var i = 0; i < GetRowCount(); ++i)
            {
                var item = d_grid[i][columnIndex];

                if (item != null)
                {
                    item.SetSelected(state);
                }
            }
        }

        /// <summary>
        /// Move the column at index \a col_idx so it is at index \a position.  
        /// Implementation version which does not move the header segment (since that may have already happened).
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="position"></param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a col_idx is invalid.
        /// </exception>
	    protected void MoveColumnImpl(int columnIndex, int position)
	    {
            // ensure index is valid, and throw if not.
            if (columnIndex >= GetColumnCount())
                throw new InvalidRequestException("the specified source column index is out of range.");

            // if position is too big, insert at end.
            if (position > GetColumnCount())
                position = GetColumnCount();
            
            // update select column index value if needed
            if (d_nominatedSelectCol == columnIndex)
            {
                d_nominatedSelectCol = position;
            }
            else if ((columnIndex < d_nominatedSelectCol) && (position >= d_nominatedSelectCol))
            {
                d_nominatedSelectCol--;
            }
            else if ((columnIndex > d_nominatedSelectCol) && (position <= d_nominatedSelectCol))
            {
                d_nominatedSelectCol++;
            }

            // move column entry in each row.
            for (var i = 0; i < GetRowCount(); ++i)
            {
                // store entry.
                var item = d_grid[i][columnIndex];

                // remove the original column for this row.
                d_grid[i].d_items.RemoveAt(columnIndex);

                // insert entry at its new position
                d_grid[i].d_items.Insert(position, item);
            }
	    }

        /// <summary>
        /// Remove all items from the list.
        /// <para>
        /// Note that this will cause 'AutoDelete' items to be deleted.
        /// </para>
        /// </summary>
        /// <returns>
        /// - true if the list contents were changed.
        /// - false if the list contents were not changed (list already empty).
        /// </returns>
	    protected bool ResetListImpl()
	    {
            // just return false if the list is already empty (no rows == empty)
            if (GetRowCount() == 0)
                return false;
           
            // we have items to be removed and possible deleted
            for (var i = 0; i < GetRowCount(); ++i)
            {
                for (var j = 0; j < GetColumnCount(); ++j)
                {
                    var item = d_grid[i][j];

                    // delete item as needed.
                    if ((item != null) && item.IsAutoDeleted())
                    {
                        // TODO: CEGUI_DELETE_AO item;
                    }
                }
            }

            // clear all items from the grid.
            d_grid.Clear();

            // reset other affected fields
            d_nominatedSelectRow = 0;
            d_lastSelected = null;

            return true;
	    }

        // overrides function in base class.
        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as MultiColumnListWindowRenderer) != null;
        }
        
        // overrides function in base class.
        protected override int WritePropertiesXML(XMLSerializer xmlStream)
        {
            // basically this is here to translate the columns in the list into
            // instances of the <ColumnHeader> element.  Because the SortColumnID
            // property requires the column to exist, we also write that out manually.

            // Dump all other properties first
            var propCnt = base.WritePropertiesXML(xmlStream);

            // create an dump <ColumnHeader> elements
            for (var i = 0; i < GetColumnCount(); ++i)
            {
                var seg = GetHeaderSegmentForColumn(i);

                // column text
                var propString = "text:";
                propString += seg.GetText();
                // column width
                propString += " width:";
                propString += PropertyHelper.ToString(seg.GetWidth());
                // column id
                propString += " id:";
                propString += PropertyHelper.ToString(seg.GetId());
                // create the tag
                xmlStream.OpenTag(Property.XMLElementName)
                          .Attribute(Property.NameXMLAttributeName, "ColumnHeader")
                          .Attribute(Property.ValueXMLAttributeName, propString)
                          .CloseTag();
                ++propCnt;
            }

            // write out SortColumnID property, if any(!)
            try
            {
                var sortColumnId = GetColumnWithId(GetSortColumn());
                if (sortColumnId != 0)
                {
                    xmlStream.OpenTag(Property.XMLElementName)
                              .Attribute(Property.NameXMLAttributeName, "SortColumnID")
                              .Attribute(Property.ValueXMLAttributeName, PropertyHelper.ToString(sortColumnId))
                              .CloseTag();
                    ++propCnt;
                }
            }
            catch (InvalidRequestException)
            {
                // This catches error(s) from the MultiLineColumnList for example
                System.GetSingleton().Logger
                      .LogEvent("MultiColumnList::writePropertiesXML - invalid sort column requested. Continuing...",
                                LoggingLevel.Errors);
            }

            return propCnt;
        }

        /// <summary>
        /// Causes the internal list to be (re)sorted.
        /// </summary>
        protected void ResortList()
        {
            // re-sort list according to direction
            var dir = GetSortDirection();

            if (dir == ListHeaderSegment.SortDirection.Descending)
            {
                d_grid.Sort((a, b) => b.CompareTo(a));
            }
            else if (dir == ListHeaderSegment.SortDirection.Ascending)
            {
                d_grid.Sort((a, b) => a.CompareTo(b));
            }
            // else no (or invalid) direction, so do not sort.
        }

        /// <summary>
        /// Handler called when the selection mode of the list box changes
        /// </summary>
        /// <param name="e"></param>
	    protected virtual void OnSelectionModeChanged(WindowEventArgs e)
        {
            FireEvent(SelectionModeChanged, e);
        }

        /// <summary>
        /// Handler called when the nominated selection column changes
        /// </summary>
        /// <param name="e"></param>
	    protected virtual void OnNominatedSelectColumnChanged(WindowEventArgs e)
        {
            FireEvent(NominatedSelectColumnChanged, e);
        }

        /// <summary>
        /// Handler called when the nominated selection row changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNominatedSelectRowChanged(WindowEventArgs e)
        {
            FireEvent(NominatedSelectRowChanged, e);
        }

        /// <summary>
        /// Handler called when the vertical scroll bar 'force' mode is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnVertScrollbarModeChanged(WindowEventArgs e)
        {
            FireEvent(VertScrollbarModeChanged, e);
        }

        /// <summary>
        /// Handler called when the horizontal scroll bar 'force' mode is changed.
        /// </summary>
        /// <param name="e"></param>
	    protected virtual	void	OnHorzScrollbarModeChanged(WindowEventArgs e)
        {
            FireEvent(HorzScrollbarModeChanged, e);
        }

        /// <summary>
        /// Handler called when the current selection changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(SelectionChanged, e);
        }

        /// <summary>
        /// Handler called when the list contents is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnListContentsChanged(WindowEventArgs e)
        {
            ConfigureScrollbars();
            Invalidate(false);
            FireEvent(ListContentsChanged, e);
        }

        /// <summary>
        /// Handler called when the sort column changes.
        /// </summary>
        /// <param name="e"></param>
	    protected virtual	void	OnSortColumnChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(SortColumnChanged, e);
        }

        /// <summary>
        /// Handler called when the sort direction changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortDirectionChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(SortDirectionChanged, e);
        }

        /// <summary>
        /// Handler called when a column is sized.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnListColumnSized(WindowEventArgs e)
        {
            ConfigureScrollbars();
            Invalidate(false);
            FireEvent(ListColumnSized, e);
        }

        /// <summary>
        /// Handler called when the column order is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnListColumnMoved(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(ListColumnMoved, e);
        }

        protected internal override void OnFontChanged(WindowEventArgs e)
        {
            // Propagate to children
            // Set the font equal to that of our list
            for (var col = 0; col < GetColumnCount(); col++)
                GetHeaderSegmentForColumn(col).SetFont(d_font);

            // Call base class handler
            base.OnFontChanged(e);
        }

        protected override void OnSized(ElementEventArgs e)
        {
            // base class handling
	        base.OnSized(e);

	        ConfigureScrollbars();

	        ++e.handled;
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorPressHold(e);

	        if (e.Source == CursorInputSource.Left)
	        {
                var localPoint = CoordConverter.ScreenToWindow(this, e.Position);
	            HandleSelection(localPoint, false, false);

		        ++e.handled;
	        }
        }

        protected internal override void OnSemanticInputEvent(SemanticEventArgs e)
        {
            var cumulative = e.d_semanticValue == SemanticValue.SV_SelectCumulative;
            var range = e.d_semanticValue == SemanticValue.SV_SelectRange;

            if (cumulative || range)
            {
                var localPoint = CoordConverter.ScreenToWindow(this, GetGUIContext().GetCursor().GetPosition());
                HandleSelection(localPoint, cumulative, range);

                ++ e.handled;
            }
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            // base class processing.
	        base.OnScroll(e);

            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

	        if (vertScrollbar.IsEffectiveVisible() && (vertScrollbar.GetDocumentSize() > vertScrollbar.GetPageSize()))
	        {
		        vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition() + vertScrollbar.GetStepSize() * -e.scroll);
	        }
	        else if (horzScrollbar.IsEffectiveVisible() && (horzScrollbar.GetDocumentSize() > horzScrollbar.GetPageSize()))
	        {
		        horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition() + horzScrollbar.GetStepSize() * -e.scroll);
	        }

	        ++e.handled;
        }

        private void HandleSelection(Lunatics.Mathematics.Vector2 position, bool cumulative, bool range)
        {
            bool modified = false;

            var item = GetItemAtPoint(position);

            if (item!=null)
            {
                // TODO: wtf this is overwritten right after
                // clear old selections if not a cumulative selection or if multi-select is off
                if (!cumulative || !d_multiSelect)
                {
                    modified = ClearAllSelectionsImpl();
                }

                modified = true;

                // select range or item, depending upon state and last selected item
                if (range && (d_lastSelected != null) && d_multiSelect)
                {
                    modified |= SelectRange(GetItemGridReference(item), GetItemGridReference(d_lastSelected));
                }
                else
                {
                    modified |= SetItemSelectStateImpl(GetItemGridReference(item), item.IsSelected() ^ true);
                }

                // update last selected item
                d_lastSelected = item.IsSelected() ? item : null;
            }

            // fire event if needed
            if (modified)
                OnSelectionChanged(new WindowEventArgs(this));
        }

	    protected void /*bool*/ HandleHeaderScroll(object sender, WindowEventArgs e)
	    {
            // grab the header scroll value, convert to pixels, and set the scroll bar to match.
            GetHorzScrollbar().SetScrollPosition(GetListHeader().GetSegmentOffset());

            // TODO: return true;
	    }

        protected void /*bool*/ HandleHeaderSegMove(object sender, HeaderSequenceEventArgs e)
	    {
	        MoveColumnImpl(e.d_oldIdx, e.d_newIdx);

	        // signal change to our clients
	        OnListColumnMoved(new WindowEventArgs(this));

	        // TODO: return true;
	    }

        protected void /*bool*/ HandleColumnSizeChange(object sender, WindowEventArgs e)
	    {
	        ConfigureScrollbars();

	        // signal change to our clients
	        OnListColumnSized(new WindowEventArgs(this));

	        // TODO: return true;
	    }

        protected bool HandleHorzScrollbar(EventArgs e)
	    {
            // set header offset to match scroll position
            GetListHeader().SetSegmentOffset(GetHorzScrollbar().GetScrollPosition());
            Invalidate(false);
            return true;
	    }

        protected bool HandleVertScrollbar(EventArgs e)
	    {
            Invalidate(false);
            return true;
	    }

        protected void /*bool*/ HandleSortColumnChange(object sender, WindowEventArgs e)
	    {
	        var col = GetSortColumn();

	        // set new sort column on all rows
	        for (var i = 0; i < GetRowCount(); ++i)
	        {
		        d_grid[i].d_sortColumn = col;
	        }

            ResortList();

	        // signal change to our clients
	        OnSortColumnChanged(new WindowEventArgs(this));

	        // TODO: return true;
	    }

        protected void /*bool*/ HandleSortDirectionChange(object sender, WindowEventArgs e)
	    {
	        ResortList();
	        // signal change to our clients
	        OnSortDirectionChanged(new WindowEventArgs(this));

	        // TODO: return true;
	    }

        protected void /*bool*/ HandleHeaderSegDblClick(object sender, WindowEventArgs e)
	    {
	        // get the column index for the segment that was double-clicked
	        var col = GetListHeader().GetColumnFromSegment((ListHeaderSegment)e.Window);

	        AutoSizeColumnHeader(col);

	        // TODO: return true;
	    }

        /// <summary>
        /// Struct used internally to represent a row in the list and also to ease
        /// sorting of the rows.
        /// </summary>
        class ListRow : IComparable<ListRow>
        {
            public readonly List<ListboxItem> d_items =new List<ListboxItem>();
		    public int		d_sortColumn;
		    public int		d_rowID;
            
		    
            //ListboxItem const& operator[](uint idx) const	{return d_items[idx];}
            //ListboxItem&	operator[](uint idx) {return d_items[idx];}
            public ListboxItem this[int idx]
            {
                get { return d_items[idx]; }
                set { d_items[idx] = value; }
            }
            // TODO: operators
            //bool	operator<(const ListRow& rhs){ throw new NotImplementedException(); }
            //bool	operator>(const ListRow& rhs){ throw new NotImplementedException(); }
            public int CompareTo(ListRow other)
            {
                var a = d_items[d_sortColumn];

                // TODO:...
                if (other == null)
                    return -1;

                var b = other.d_items[d_sortColumn];

                if (a == null)
                    return 1;
                if (b == null)
                    return -1;
                
                // TODO: review this
                return a.CompareTo(b);
            }
        }

	    // scrollbar settings.
	    bool	d_forceVertScroll;		//!< true if vertical scrollbar should always be displayed
	    bool	d_forceHorzScroll;		//!< true if horizontal scrollbar should always be displayed

	    // selection abilities.
	    SelectionMode	d_selectMode;	//!< Holds selection mode (represented by settings below).
	    int	d_nominatedSelectCol;	//!< Nominated column for single column selection.
	    int	d_nominatedSelectRow;	//!< Nominated row for single row selection.
	    bool	d_multiSelect;			//!< Allow multiple selections.
	    bool	d_fullRowSelect;		//!< All items in a row are selected.
	    bool	d_fullColSelect;		//!< All items in a column are selected.
	    bool	d_useNominatedRow;		//!< true if we use a nominated row to select.
	    bool	d_useNominatedCol;		//!< true if we use a nominated col to select.
	    ListboxItem	d_lastSelected;	//!< holds pointer to the last selected item (used in range selections)

        int    d_columnCount;          //!< keeps track of the number of columns.

	    List<ListRow> d_grid=new List<ListRow>();			//!< Holds the list box data.

        //! whether header size will be considered when auto-sizing columns.
        bool d_autoSizeColumnUsesHeader;
        
	    private void AddMultiColumnListProperties()
	    {
            // TODO: Inconsistency
	        DefineProperty(
	            "ColumnsSizable",
	            "Property to get/set the setting for user sizing of the column headers.  Value is either \"True\" or \"False\".",
	            (x, v) => x.SetUserColumnSizingEnabled(v), x => x.IsUserColumnSizingEnabled(), true);

            // TODO: Inconsistency
	        DefineProperty(
	            "ColumnsMovable",
	            "Property to get/set the setting for user moving of the column headers.  Value is either \"True\" or \"False\".",
	            (x, v) => x.SetUserColumnDraggingEnabled(v), x => x.IsUserColumnDraggingEnabled(), true);

            // TODO: Inconsistency
	        DefineProperty(
	            "SortSettingEnabled",
	            "Property to get/set the setting for for user modification of the sort column & direction. Value is either \"True\" or \"False\".",
	            (x, v) => x.SetUserSortControlEnabled(v), x => x.IsUserSortControlEnabled(), true);
            
	        DefineProperty(
	            "SortDirection",
	            "Property to get/set the sort direction setting of the list. Value is the text of one of the SortDirection enumerated value names.",
	            (x, v) => x.SetSortDirection(v), x => x.GetSortDirection(), ListHeaderSegment.SortDirection.None);

            // TODO: Inconsistency
	        DefineProperty(
	            "ForceVertScrollbar",
	            "Property to get/set the 'always show' setting for the vertical scroll bar of the list box. Value is either \"True\" or \"False\".",
	            (x, v) => x.SetShowVertScrollbar(v), x => x.IsVertScrollbarAlwaysShown(), false);

            // TODO: Inconsistency
	        DefineProperty(
	            "ForceHorzScrollbar",
	            "Property to get/set the 'always show' setting for the horizontal scroll bar of the list box. Value is either \"True\" or \"False\".",
	            (x, v) => x.SetShowHorzScrollbar(v), x => x.IsHorzScrollbarAlwaysShown(), false);
            
            // TODO: Inconsistency
	        DefineProperty(
	            "NominatedSelectionColumnID",
	            "Property to get/set the nominated selection column (via ID).  Value is an unsigned integer number.",
	            (x, v) => x.SetNominatedSelectionColumn(v), x => x.GetNominatedSelectionColumnId(), 0);

	        DefineProperty(
	            "NominatedSelectionRow",
	            "Property to get/set the nominated selection row.  Value is an unsigned integer number.",
	            (x, v) => x.SetNominatedSelectionRow(v), x => x.GetNominatedSelectionRow(), 0);

	        DefineProperty(
	            "RowCount", "Property to access the number of rows in the list (read only)",
	            null, x => x.GetRowCount(), 0);

	        DefineProperty(
	            "SelectionMode",
	            "Property to get/set the selection mode setting of the list. Value is the text of one of the SelectionMode enumerated value names.",
	            (x, v) => x.SetSelectionMode(v), x => x.GetSelectionMode(), SelectionMode.RowSingle);

	        DefineProperty(
	            "AutoSizeColumnUsesHeader",
	            "Property to get/set the 'use header size' flag when auto-sizing a column. Value is either \"True\" or \"False\".",
	            (x, v) => x.SetAutoSizeColumnUsesHeader(v), x => x.GetAutoSizeColumnUsesHeader(), false);

            // TODO: This is quite a hack, isn't it?
	        DefineProperty(
	            "ColumnHeader", "Property to set up a column (there is no getter for this property)",
	            (x, v) => x.AddColumn(v), null, "");
	    }

        private void DefineProperty<T>(string name, string help, Action<MultiColumnList, T> setter, Func<MultiColumnList, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<MultiColumnList, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }
    }

//template<>
//class PropertyHelper<MultiColumnList::SelectionMode>
//{
//public:
//    typedef MultiColumnList::SelectionMode return_type;
//    typedef return_type safe_method_return_type;
//    typedef MultiColumnList::SelectionMode pass_type;
//    typedef String string_return_type;

//    static string getDataTypeName()
//    {
//        static String type("SelectionMode");

//        return type;
//    }

//    static return_type fromString(string str)
//    {
//        MultiColumnList::SelectionMode mode;

//        if (str == "RowMultiple")
//        {
//            mode = MultiColumnList::RowMultiple;
//        }
//        else if (str == "ColumnSingle")
//        {
//            mode = MultiColumnList::ColumnSingle;
//        }
//        else if (str == "ColumnMultiple")
//        {
//            mode = MultiColumnList::ColumnMultiple;
//        }
//        else if (str == "CellSingle")
//        {
//            mode = MultiColumnList::CellSingle;
//        }
//        else if (str == "CellMultiple")
//        {
//            mode = MultiColumnList::CellMultiple;
//        }
//        else if (str == "NominatedColumnSingle")
//        {
//            mode = MultiColumnList::NominatedColumnSingle;
//        }
//        else if (str == "NominatedColumnMultiple")
//        {
//            mode = MultiColumnList::NominatedColumnMultiple;
//        }
//        else if (str == "NominatedRowSingle")
//        {
//            mode = MultiColumnList::NominatedRowSingle;
//        }
//        else if (str == "NominatedRowMultiple")
//        {
//            mode = MultiColumnList::NominatedRowMultiple;
//        }
//        else
//        {
//            mode = MultiColumnList::RowSingle;
//        }
//        return mode;
//    }

//    static string_return_type toString(pass_type val)
//    {
//        switch(val)
//        {
//        case MultiColumnList::RowMultiple:
//            return String("RowMultiple");
//            break;

//        case MultiColumnList::ColumnSingle:
//            return String("ColumnSingle");
//            break;

//        case MultiColumnList::ColumnMultiple:
//            return String("ColumnMultiple");
//            break;

//        case MultiColumnList::CellSingle:
//            return String("CellSingle");
//            break;

//        case MultiColumnList::CellMultiple:
//            return String("CellMultiple");
//            break;

//        case MultiColumnList::NominatedColumnSingle:
//            return String("NominatedColumnSingle");
//            break;

//        case MultiColumnList::NominatedColumnMultiple:
//            return String("NominatedColumnMultiple");
//            break;

//        case MultiColumnList::NominatedRowSingle:
//            return String("NominatedRowSingle");
//            break;

//        case MultiColumnList::NominatedRowMultiple:
//            return String("NominatedRowMultiple");
//            break;

//        default:
//            return String("RowSingle");
//            break;
//        }
//    }
//};
}