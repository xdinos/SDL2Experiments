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
using System.Globalization;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// EventArgs class used for segment move (sequence changed) events.
    /// </summary>
    public class HeaderSequenceEventArgs : WindowEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="oldIdx"></param>
        /// <param name="newIdx"></param>
        public HeaderSequenceEventArgs(Window wnd, int oldIdx, int newIdx)
            : base(wnd)
        {
            d_oldIdx = oldIdx;
            d_newIdx = newIdx;
        }

        /// <summary>
        /// The original column index of the segment that has moved.
        /// </summary>
        public int d_oldIdx;

        /// <summary>
        /// The new column index of the segment that has moved.
        /// </summary>
        public int d_newIdx;
    }

    /// <summary>
    /// Base class for the multi column list header window renderer.
    /// </summary>
    public abstract class ListHeaderWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected ListHeaderWindowRenderer(string name) : base(name, ListHeader.EventNamespace)
        {

        }

        /// <summary>
        /// Create and return a pointer to a new ListHeaderSegment based object.
        /// </summary>
        /// <param name="name">
        /// String object holding the name that should be given to the new Window.
        /// </param>
        /// <returns>
        /// Pointer to an ListHeaderSegment based object of whatever type is appropriate for this ListHeader.
        /// </returns>
        public abstract ListHeaderSegment CreateNewSegment(string name);

        /// <summary>
        /// Cleanup and destroy the given ListHeaderSegment that was created via the
        /// createNewSegment method.
        /// </summary>
        /// <param name="segment">
        /// Pointer to a ListHeaderSegment based object to be destroyed.
        /// </param>
        public abstract void DestroyListSegment(ListHeaderSegment segment);
    }

    /// <summary>
    /// Base class for the multi column list header widget.
    /// </summary>
    public class ListHeader: Window
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ListHeader";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/ListHeader";
        
        #region Events

        /// <summary>
        /// Event fired when the current sort column of the header is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeader whose sort column has
        /// been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> SortColumnChanged;

        /// <summary>
        /// Event fired when the sort direction of the header is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeader whose sort direction had
        /// been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SortDirectionChanged;
        
        /// <summary>
        /// Event fired when a segment of the header is sized by the user.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment that has been sized.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SegmentSized;
        
        /// <summary>
        /// Event fired when a segment of the header is clicked by the user.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment that was clicked.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SegmentClicked;
        
        /// <summary>
        /// Event fired when a segment splitter of the header is double-clicked.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment whose splitter area
        /// was double-clicked.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SplitterDoubleClicked;
        
        /// <summary>
        /// Event fired when the order of the segments in the header has changed.
        /// Handlers are passed a const HeaderSequenceEventArgs reference with
        /// WindowEventArgs::window set to the ListHeader whose segments have changed
        /// sequence, HeaderSequenceEventArgs::d_oldIdx is the original index of the
        /// segment that has moved, and HeaderSequenceEventArgs::d_newIdx is the new
        /// index of the segment that has moved.
        /// </summary>
        public event EventHandler<HeaderSequenceEventArgs> SegmentSequenceChanged;
        
        /// <summary>
        /// Event fired when a segment is added to the header.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeader that has had a new segment
        /// added.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SegmentAdded;
        
        /// <summary>
        /// Event fired when a segment is removed from the header.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeader that has had a segment
        /// removed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SegmentRemoved;
        
        /// <summary>
        /// Event fired when setting that controls user modification to sort
        /// configuration is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeader whose user sort control
        /// setting has been changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SortSettingChanged;
        
        /// <summary>
        /// Event fired when setting that controls user drag &amp; drop of segments is
        /// changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeader whose drag &amp; drop enabled
        /// setting has changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> DragMoveSettingChanged;
        
        /// <summary>
        /// Event fired when setting that controls user sizing of segments is
        /// changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeader whose user sizing setting
        /// has changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> DragSizeSettingChanged;
        
        /// <summary>
        /// Event fired when the rendering offset for the segments changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeader whose segment rendering
        /// offset has changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> SegmentRenderOffsetChanged;

        #endregion

        /// <summary>
        /// Speed to scroll at when dragging outside header.
        /// </summary>
        public const float ScrollSpeed = 8.0f;

        /// <summary>
        /// Miniumum width of a segment in pixels.
        /// </summary>
        public const float MinimumSegmentPixelWidth = 20.0f;

        /// <summary>
        /// Widget name suffix for header segments.
        /// </summary>
        public const string SegmentNameSuffix = "__auto_seg_";

	    /// <summary>
        /// Return the number of columns or segments attached to the header.
        /// </summary>
        /// <returns>
        /// uint value equal to the number of columns / segments currently in the header.
        /// </returns>
        public int GetColumnCount()
        {
            return _segments.Count;
        }

        /// <summary>
        /// Return the ListHeaderSegment object for the specified column
        /// </summary>
        /// <param name="column">
        /// zero based column index of the ListHeaderSegment to be returned.
        /// </param>
        /// <returns>
        /// ListHeaderSegment object at the requested index.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if column is out of range.
        /// </exception>
        public ListHeaderSegment GetSegmentFromColumn(int column)
        {
            if (column >= GetColumnCount())
                throw new InvalidRequestException("requested column index is out of range for this ListHeader.");
            
            return _segments[column];
        }

        /// <summary>
        /// Return the ListHeaderSegment object with the specified ID.
        /// </summary>
        /// <param name="id">
        /// id code of the ListHeaderSegment to be returned.
        /// </param>
        /// <returns>
        /// ListHeaderSegment object with the ID \a id.  If more than one segment has the same ID, only the first one will
        /// ever be returned.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if no segment with the requested ID is attached.
        /// </exception>
	    public ListHeaderSegment GetSegmentFromId(int id)
	    {
            for (var i = 0; i < GetColumnCount(); ++i)
            {
                if (_segments[i].GetId() == id)
                {
                    return _segments[i];
                }

            }

            // No such segment found, throw exception
	        throw new InvalidRequestException("no segment with the requested ID is attached to this ListHeader.");
	    }

        /// <summary>
        /// Return the ListHeaderSegment that is marked as being the 'sort key' segment.  
        /// There must be at least one segment to successfully call this method.
        /// </summary>
        /// <returns>
        /// ListHeaderSegment object which is the sort-key segment.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if no segments are attached to the ListHeader.
        /// </exception>
	    public ListHeaderSegment GetSortSegment()
        {
            if (_sortSegment == null)
                throw new InvalidRequestException(
                    "Sort segment was invalid!  (No segments are attached to the ListHeader?)");

            return _sortSegment;
	    }

        /// <summary>
        /// Return the ListHeaderSegment ID that is marked as being the 'sort key' segment.  There must be at least one segment
        /// to successfully call this method.
        /// </summary>
        /// <returns>
        /// uint which is the sort-key segment ID.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if no segments are attached to the ListHeader.
        /// </exception>
	    public int GetSortSegmentId()
	    {
            return GetSortSegment().GetId();
	    }

        /// <summary>
        /// Return the zero based column index of the specified segment.
        /// </summary>
        /// <param name="segment">
        /// ListHeaderSegment whos zero based index is to be returned.
        /// </param>
        /// <returns>
        /// Zero based column index of the ListHeaderSegment \a segment.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a segment is not attached to this ListHeader.
        /// </exception>
	    public int GetColumnFromSegment(ListHeaderSegment segment)
	    {
            for (var i = 0; i < GetColumnCount(); ++i)
            {
                if (_segments[i] == segment)
                    return i;
            }

            // No such segment found, throw exception
            throw new InvalidRequestException("the given ListHeaderSegment is not attached to this ListHeader.");
	    }

        /// <summary>
        /// Return the zero based column index of the segment with the specified ID.
        /// </summary>
        /// <param name="id">
        /// ID code of the segment whos column index is to be returned.
        /// </param>
        /// <returns>
        /// Zero based column index of the first ListHeaderSegment whos ID matches \a id. 
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if no attached segment has the requested ID.
        /// </exception>
	    public int GetColumnFromId(int id)
	    {
            for (var i = 0; i < GetColumnCount(); ++i)
            {
                if (_segments[i].GetId() == id)
                {
                    return i;
                }
            }

            // No such segment found, throw exception
            throw new InvalidRequestException("no column with the requested ID is available on this ListHeader.");
	    }

        /// <summary>
        /// Return the zero based index of the current sort column.  
        /// There must be at least one segment/column to successfully call this method.
        /// </summary>
        /// <returns>
        /// Zero based column index that is the current sort column.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if there are no segments / columns in this ListHeader.
        /// </exception>
	    public int GetSortColumn()
	    {
            return GetColumnFromSegment(GetSortSegment());
	    }

        /// <summary>
        /// Return the zero based column index of the segment with the specified text.
        /// </summary>
        /// <param name="text">
        /// String object containing the text to be searched for.
        /// </param>
        /// <returns>
        /// Zero based column index of the segment with the specified text.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if no attached segments have the requested text.
        /// </exception>
	    public int GetColumnWithText(String text)
	    {
            for (var i = 0; i < GetColumnCount(); ++i)
            {
                if (_segments[i].GetText() == text)
                {
                    return i;
                }
            }

            // No such segment found, throw exception
	        throw new InvalidRequestException("no column with the text '" + text + "' is attached to this ListHeader.");
	    }

        /// <summary>
        /// Return the pixel offset to the given ListHeaderSegment.
        /// </summary>
        /// <param name="segment">
        /// ListHeaderSegment object that the offset to is to be returned.
        /// </param>
        /// <returns>
        /// The number of pixels up-to the begining of the ListHeaderSegment described by \a segment.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a segment is not attached to the ListHeader.
        /// </exception>
	    public float GetPixelOffsetToSegment(ListHeaderSegment segment)
	    {
            var offset = 0.0f;

            for (var i = 0; i < GetColumnCount(); ++i)
            {
                if (_segments[i] == segment)
                {
                    return offset;
                }

                offset += _segments[i].GetPixelSize().Width;
            }

            // No such segment found, throw exception
            throw new InvalidRequestException("the given ListHeaderSegment is not attached to this ListHeader.");
	    }

        /// <summary>
        /// Return the pixel offset to the ListHeaderSegment at the given zero based column index.
        /// </summary>
        /// <param name="column">
        /// Zero based column index of the ListHeaderSegment whos pixel offset it to be returned.
        /// </param>
        /// <returns>
        /// The number of pixels up-to the begining of the ListHeaderSegment located at zero based column
        /// index \a column.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a column is out of range.
        /// </exception>
        public float GetPixelOffsetToColumn(int column)
        {
            if (column >= GetColumnCount())
                throw new InvalidRequestException("requested column index is out of range for this ListHeader.");
            
            var offset = 0.0f;

            for (var i = 0; i < column; ++i)
                offset += _segments[i].GetPixelSize().Width;
            
            return offset;
        }

        /// <summary>
        /// Return the total pixel width of all attached segments.
        /// </summary>
        /// <returns>
        /// Sum of the pixel widths of all attached ListHeaderSegment objects.
        /// </returns>
        public float GetTotalSegmentsPixelExtent()
        {
            var extent = 0.0f;
            for (var i = 0; i < GetColumnCount(); ++i)
                extent += _segments[i].GetPixelSize().Width;

            return extent;
        }

        /// <summary>
        /// Return the width of the specified column.
        /// </summary>
        /// <param name="column">
        /// Zero based column index of the segment whose width is to be returned.
        /// </param>
        /// <returns>
        /// UDim describing the width of the ListHeaderSegment at the zero based
        /// column index specified by \a column.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a column is out of range.
        /// </exception>
	    public UDim GetColumnWidth(int column)
	    {
            if (column >= GetColumnCount())
                throw new InvalidRequestException("requested column index is out of range for this ListHeader.");
            
            return _segments[column].GetWidth();
	    }

        /// <summary>
        /// Return the currently set sort direction.
        /// </summary>
        /// <returns>
        /// One of the ListHeaderSegment.SortDirection enumerated values specifying the current sort direction.
        /// </returns>
	    public ListHeaderSegment.SortDirection GetSortDirection()
	    {
            return _sortDirection;
	    }

        /// <summary>
        /// Return whether user manipulation of the sort column &amp; direction are enabled.
        /// </summary>
        /// <returns>
        /// true if the user may interactively modify the sort column and direction.  false if the user may not
        /// modify the sort column and direction (these can still be set programmatically).
        /// </returns>
	    public bool IsSortingEnabled()
	    {
            return _sortingEnabled;
	    }

        /// <summary>
        /// Return whether the user may size column segments.
        /// </summary>
        /// <returns>
        /// true if the user may interactively modify the width of column segments, false if they may not.
        /// </returns>
	    public bool IsColumnSizingEnabled()
	    {
            return _sizingEnabled;
	    }

        /// <summary>
        /// Return whether the user may modify the order of the segments.
        /// </summary>
        /// <returns>
        /// true if the user may interactively modify the order of the column segments, false if they may not.
        /// </returns>
	    public bool IsColumnDraggingEnabled()
	    {
            return _movingEnabled;
	    }

        /// <summary>
        /// Return the current segment offset value.  
        /// This value is used to implement scrolling of the header segments within the ListHeader area.
        /// </summary>
        /// <returns>
        /// float value specifying the current segment offset value in whatever metrics system is active for the ListHeader.
        /// </returns>
	    public float GetSegmentOffset()
	    {
	        return _segmentOffset;
	    }

        /// <summary>
        /// Set whether user manipulation of the sort column and direction is enabled.
        /// </summary>
        /// <param name="setting">
        /// - true to allow interactive user manipulation of the sort column and direction.
        /// - false to disallow interactive user manipulation of the sort column and direction.
        /// </param>
	    public void SetSortingEnabled(bool setting)
	    {
	        if (_sortingEnabled != setting)
	        {
		        _sortingEnabled = setting;

		        // make the setting change for all component segments.
		        for (var i = 0; i <GetColumnCount(); ++i)
		        {
			        _segments[i].SetClickable(_sortingEnabled);
		        }

		        // Fire setting changed event.
		        OnSortSettingChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Set the current sort direction.
        /// </summary>
        /// <param name="direction">
        /// One of the ListHeaderSegment::SortDirection enumerated values indicating the sort direction to be used.
        /// </param>
	    public void	SetSortDirection(ListHeaderSegment.SortDirection direction)
	    {
	        if (_sortDirection != direction)
	        {
		        _sortDirection = direction;

		        // set direction of current sort segment
		        if (_sortSegment!=null)
			        _sortSegment.SetSortDirection(direction);

		        // Fire sort direction changed event.
		        OnSortDirectionChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Set the column segment to be used as the sort column.
        /// </summary>
        /// <param name="segment">
        /// ListHeaderSegment object indicating the column to be sorted.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a segment is not attached to this ListHeader.
        /// </exception>
	    public void	SetSortSegment(ListHeaderSegment segment)
	    {
            SetSortColumn(GetColumnFromSegment(segment));
	    }

        /// <summary>
        /// Set the column to be used as the sort column.
        /// </summary>
        /// <param name="column">
        /// Zero based column index indicating the column to be sorted.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a column is out of range for this ListHeader.
        /// </exception>
        public void SetSortColumn(int column)
        {
            if (column >= GetColumnCount())
                throw new InvalidRequestException("specified column index is out of range for this ListHeader.");

            // if column is different to current sort segment
            if (_sortSegment != _segments[column])
            {
                // set sort direction on 'old' sort segment to none.
                if (_sortSegment!=null)
                    _sortSegment.SetSortDirection(ListHeaderSegment.SortDirection.None);

                // set-up new sort segment
                _sortSegment = _segments[column];
                _sortSegment.SetSortDirection(_sortDirection);

                // Fire sort column changed event
                OnSortColumnChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set the column to to be used for sorting via its ID code.
        /// </summary>
        /// <param name="id">
        /// ID code of the column segment that is to be used as the sort column.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if no segment with ID \a id is attached to the ListHeader.
        /// </exception>
	    public void	SetSortColumnFromId(int id)
	    {
            SetSortSegment(GetSegmentFromId(id));
	    }

        /// <summary>
        /// Set whether columns may be sized by the user.
        /// </summary>
        /// <param name="setting">
        /// - true to indicate that the user may interactively size segments.
        /// - false to indicate that the user may not interactively size segments.
        /// </param>
	    public void	SetColumnSizingEnabled(bool setting)
	    {
	        if (_sizingEnabled != setting)
	        {
		        _sizingEnabled = setting;

		        // make the setting change for all component segments.
		        for (var i = 0; i <GetColumnCount(); ++i)
		        {
			        _segments[i].SetSizingEnabled(_sizingEnabled);
		        }

		        // Fire setting changed event.
		        OnDragSizeSettingChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Set whether columns may be reordered by the user via drag and drop.
        /// </summary>
        /// <param name="setting">
        /// - true to indicate the user may change the order of the column segments via drag and drop.
        /// - false to indicate the user may not change the column segment order.
        /// </param>
	    public void	SetColumnDraggingEnabled(bool setting)
	    {
	        if (_movingEnabled != setting)
	        {
		        _movingEnabled = setting;

		        // make the setting change for all component segments.
		        for (var i = 0; i <GetColumnCount(); ++i)
		        {
			        _segments[i].SetDragMovingEnabled(_movingEnabled);
		        }

		        // Fire setting changed event.
		        OnDragMoveSettingChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Add a new column segment to the end of the header.
        /// </summary>
        /// <param name="text">String object holding the initial text for the new segment</param>
        /// <param name="id">Client specified ID code to be assigned to the new segment.</param>
        /// <param name="width">UDim describing the initial width of the new segment.</param>
	    public void	AddColumn(string text, int id, UDim width)
	    {
            // add just inserts at end.
            InsertColumn(text, id, width, GetColumnCount());
	    }

        /// <summary>
        /// Insert a new column segment at the specified position.
        /// </summary>
        /// <param name="text">String object holding the initial text for the new segment</param>
        /// <param name="id">Client specified ID code to be assigned to the new segment.</param>
        /// <param name="width">UDim describing the initial width of the new segment.</param>
        /// <param name="position">
        /// Zero based column index indicating the desired position for the new column.  If this is greater than
        /// the current number of columns, the new segment is added to the end if the header.
        /// </param>
        public void InsertColumn(string text, int id, UDim width, int position)
        {
            // if position is too big, insert at end.
            if (position > GetColumnCount())
                position = GetColumnCount();

            var seg = CreateInitialisedSegment(text, id, width);
            _segments.Insert(position, seg);

            // add window as a child of this
            AddChild(seg);

            LayoutSegments();

            // Fire segment added event.
            var args = new WindowEventArgs(this);
            OnSegmentAdded(args);

            // if sort segment is invalid, make it valid now we have a segment attached
            if (_sortSegment == null)
                SetSortColumn(position);
        }

        /// <summary>
        /// Insert a new column segment at the specified position.
        /// </summary>
        /// <param name="text">
        /// Insert a new column segment at the specified position.
        /// </param>
        /// <param name="id">
        /// Client specified ID code to be assigned to the new segment.
        /// </param>
        /// <param name="width">
        /// UDim describing the initial width of the new segment.
        /// </param>
        /// <param name="position">ListHeaderSegment object indicating the insert position for the new segment.  
        /// The new segment will be inserted before the segment indicated by \a position.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if ListHeaderSegment \a position is not attached to the ListHeader.
        /// </exception>
	    public void InsertColumn(string text, int id, UDim width, ListHeaderSegment position)
	    {
            InsertColumn(text, id, width, GetColumnFromSegment(position));
	    }

        /// <summary>
        /// Removes a column segment from the ListHeader.
        /// </summary>
        /// <param name="column">
        /// vZero based column index indicating the segment to be removed.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a column is out of range.
        /// </exception>
	    public void RemoveColumn(int column)
	    {
	        if (column >= GetColumnCount())
	            throw new InvalidRequestException("specified column index is out of range for this ListHeader.");
	        
            var seg = _segments[column];

		    // remove from the list of segments
		    _segments.RemoveAt(column);

		    // have we removed the sort column?
		    if (_sortSegment == seg)
		    {
			    // any other columns?
			    if (GetColumnCount() > 0)
			    {
				    // put first column in as sort column
				    _sortDirection = ListHeaderSegment.SortDirection.None;
				    SetSortColumn(0);
			    }
			    else
			    {
                    // no columns, set sort segment to NULL
				    _sortSegment = null;
			    }

		    }

		    // detach segment window from the header (this)
		    RemoveChild(seg);

		    // destroy the segment (done in derived class, since that's where it was created).
		    DestroyListSegment(seg);

		    LayoutSegments();

		    // Fire segment removed event.
		    OnSegmentRemoved(new WindowEventArgs(this));
	    }

        /// <summary>
        /// Remove the specified segment from the ListHeader.
        /// </summary>
        /// <param name="segment">
        /// ListHeaderSegment object that is to be removed from the ListHeader.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a segment is not attached to this ListHeader.
        /// </exception>
	    public void RemoveSegment(ListHeaderSegment segment)
	    {
            RemoveColumn(GetColumnFromSegment(segment));
	    }

        /// <summary>
        /// Moves a column segment into a new position.
        /// </summary>
        /// <param name="column">
        /// Zero based column index indicating the column segment to be moved.
        /// </param>
        /// <param name="position">
        /// Zero based column index indicating the new position for the segment.  
        /// If this is greater than the current number of segments, the segment is moved to the end of the header.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a column is out of range for this ListHeader.
        /// </exception>
	    public void MoveColumn(int column, int position)
	    {
	        if (column >= GetColumnCount())
	            throw new InvalidRequestException("specified column index is out of range for this ListHeader.");
	        
            // if position is too big, move to end.
		    if (position >= GetColumnCount())
			    position = GetColumnCount() - 1;

		    var seg = _segments[column];

		    // remove original copy of segment
		    _segments.RemoveAt(column);

		    // insert the segment at it's new position
		    _segments.Insert(position, seg);

		    // Fire sequence changed event
		    OnSegmentSequenceChanged(new HeaderSequenceEventArgs(this, column, position));

		    LayoutSegments();
	    }

        /// <summary>
        /// Move a column segment to a new position.
        /// </summary>
        /// <param name="column">
        /// Zero based column index indicating the column segment to be moved.
        /// </param>
        /// <param name="position">
        /// ListHeaderSegment object indicating the new position for the segment.  The segment at \a column
        /// will be moved behind segment \a position (that is, segment \a column will appear to the right of
        /// segment \a position).
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a column is out of range for this ListHeader, or if \a position
        /// is not attached to this ListHeader.
        /// </exception>
	    public void MoveColumn(int column, ListHeaderSegment position)
	    {
            MoveColumn(column, GetColumnFromSegment(position));
	    }

        /// <summary>
        /// Moves a segment into a new position.
        /// </summary>
        /// <param name="segment">
        /// ListHeaderSegment object that is to be moved.
        /// </param>
        /// <param name="position">
        /// Zero based column index indicating the new position for the segment.  
        /// If this is greater than the current number of segments, the segment is moved to the end of the header.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a segment is not attached to this ListHeader.
        /// </exception>
	    public void MoveSegment(ListHeaderSegment segment, int position)
	    {
            MoveColumn(GetColumnFromSegment(segment), position);
	    }


	    /*!
	    \brief
		    

	    \param segment
		    

	    \param position
		    

	    \return
		    Nothing.

	    \exception InvalidRequestException 
	    */
        /// <summary>
        /// Move a segment to a new position.
        /// </summary>
        /// <param name="segment">
        /// ListHeaderSegment object that is to be moved.
        /// </param>
        /// <param name="position">
        /// ListHeaderSegment object indicating the new position for the segment.  The segment \a segment
        /// will be moved behind segment \a position (that is, segment \a segment will appear to the right of
        /// segment \a position).
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if either \a segment or \a position are not attached to this ListHeader.
        /// </exception>
	    public void MoveSegment(ListHeaderSegment segment, ListHeaderSegment position)
	    {
            MoveColumn(GetColumnFromSegment(segment), GetColumnFromSegment(position));
	    }

        /// <summary>
        /// Set the current base segment offset.  (This implements scrolling of the header segments within
        /// the header area).
        /// </summary>
        /// <param name="offset">
        /// New base offset for the first segment.  The segments will of offset to the left by the amount specified.
        /// \a offset should be specified using the active metrics system for the ListHeader.
        /// </param>
	    public void SetSegmentOffset(float offset)
	    {
	        if (_segmentOffset != offset)
	        {
		        _segmentOffset = offset;
		        LayoutSegments();
		        Invalidate(false);
	
		        // Fire event.
		        OnSegmentOffsetChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Set the width of the specified column.
        /// </summary>
        /// <param name="column">
        /// Zero based column index of the segment whose width is to be set.
        /// </param>
        /// <param name="width">
        /// UDim value specifying the new width to set for the ListHeaderSegment at the zero based column
        /// index specified by \a column.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a column is out of range.
        /// </exception>
        public void SetColumnWidth(int column, UDim width)
        {
            if (column >= GetColumnCount())
                throw new InvalidRequestException("specified column index is out of range for this ListHeader.");

            _segments[column].SetSize(new USize(width, _segments[column].GetSize().d_height));

            LayoutSegments();

            // Fire segment sized event.
            OnSegmentSized(new WindowEventArgs(_segments[column]));
        }

        /// <summary>
        /// Constructor for the list header base class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public ListHeader(string type, string name)
            : base(type, name)
        {
            _sortSegment = null;
            _sizingEnabled = true;
            _sortingEnabled = true;
            _movingEnabled = true;
            _uniqueIdNumber = 0;
            _segmentOffset = 0.0f;
            _sortDirection = ListHeaderSegment.SortDirection.None;
            AddHeaderProperties();
        }

        /// <summary>
        /// Create initialise and return a ListHeaderSegment object, with all events subscribed and ready to use.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="id"></param>
        /// <param name="width"></param>
        /// <returns></returns>
	    private ListHeaderSegment CreateInitialisedSegment(string text, int id, UDim width)
        {
            // Build unique name
	        var name = SegmentNameSuffix + _uniqueIdNumber.ToString(CultureInfo.InvariantCulture);

	        // create segment.
	        var newseg = CreateNewSegment(name);
	        _uniqueIdNumber++;

	        // setup segment;
	        newseg.SetSize(new USize(width, UDim.Relative(1.0f)));
	        newseg.SetMinSize(new USize(UDim.Absolute(MinimumSegmentPixelWidth), UDim.Absolute(0)));
	        newseg.SetText(text);
	        newseg.SetId(id);
            newseg.SetSizingEnabled(_sizingEnabled);
            newseg.SetDragMovingEnabled(_movingEnabled);
            newseg.SetClickable(_sortingEnabled);

	        // subscribe events we listen to
            newseg.SegmentSized += SegmentSizedHandler;
            newseg.SegmentDragStop += SegmentMovedHandler;
            newseg.SegmentClicked += SegmentClickedHandler;
            newseg.SplitterDoubleClicked += SegmentDoubleClickHandler;
            newseg.SegmentDragPositionChanged += SegmentDragHandler;

	        return newseg;
        }

        /// <summary>
        /// Layout the attached segments
        /// </summary>
        protected void LayoutSegments()
        {
            var pos = new UVector2(UDim.Absolute(-_segmentOffset), UDim.Absolute(0.0f));

            for (var i = 0; i < GetColumnCount(); ++i)
            {
                _segments[i].SetPosition(pos);
                pos.d_x += _segments[i].GetWidth();
            }
        }

        /// <summary>
        /// Create and return a pointer to a new ListHeaderSegment based object.
        /// </summary>
        /// <param name="name">String object holding the name that should be given to the new Window.</param>
        /// <returns>
        /// Pointer to an ListHeaderSegment based object of whatever type is appropriate for this ListHeader.</returns>
        protected ListHeaderSegment CreateNewSegment(string name)
        {
            if (d_windowRenderer != null)
                return ((ListHeaderWindowRenderer) d_windowRenderer).CreateNewSegment(name);

            throw new InvalidRequestException("This function must be implemented by the window renderer module");

        }

        /// <summary>
        /// Cleanup and destroy the given ListHeaderSegment that was created via the
        /// createNewSegment method.
        /// </summary>
        /// <param name="segment">
        /// Pointer to a ListHeaderSegment based object to be destroyed.
        /// </param>
        protected void DestroyListSegment(ListHeaderSegment segment)
        {
            if (d_windowRenderer != null)
            {
                ((ListHeaderWindowRenderer)d_windowRenderer).DestroyListSegment(segment);
            }
            else
            {
                throw new InvalidRequestException("This function must be implemented by the window renderer module");
            }
        }

        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as ListHeaderWindowRenderer) != null;
        }
        
        /// <summary>
        /// Handler called when the sort column is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortColumnChanged(WindowEventArgs e)
        {
            FireEvent(SortColumnChanged, e);
        }

        /// <summary>
        /// Handler called when the sort direction is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortDirectionChanged(WindowEventArgs e)
        {
            FireEvent(SortDirectionChanged, e);
        }

        /// <summary>
        /// Handler called when a segment is sized by the user.  e.window points to the segment.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentSized(WindowEventArgs e)
        {
            FireEvent(SegmentSized, e);
        }

        /// <summary>
        /// Handler called when a segment is clicked by the user.  e.window points to the segment.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentClicked(WindowEventArgs e)
        {
            FireEvent(SegmentClicked, e);
        }

        /// <summary>
        /// Handler called when a segment splitter / sizer is double-clicked.  e.window points to the segment.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSplitterDoubleClicked(WindowEventArgs e)
        {
            FireEvent(SplitterDoubleClicked, e);
        }

        /// <summary>
        /// Handler called when the segment / column order changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentSequenceChanged(HeaderSequenceEventArgs e)
        {
            FireEvent(SegmentSequenceChanged, e);
        }

        /// <summary>
        /// Handler called when a new segment is added to the header.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentAdded(WindowEventArgs e)
        {
            FireEvent(SegmentAdded, e);
        }

        /// <summary>
        /// Handler called when a segment is removed from the header.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentRemoved(WindowEventArgs e)
        {
            FireEvent(SegmentRemoved, e);
        }

        /// <summary>
        /// Handler called then setting that controls the users ability to modify the search column &amp; direction changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortSettingChanged(WindowEventArgs e)
        {
            FireEvent(SortSettingChanged, e);
        }

        /// <summary>
        /// Handler called when the setting that controls the users ability to drag and drop segments changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDragMoveSettingChanged(WindowEventArgs e)
        {
            FireEvent(DragMoveSettingChanged, e);
        }

        /// <summary>
        /// Handler called when the setting that controls the users ability to size segments changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDragSizeSettingChanged(WindowEventArgs e)
        {
            FireEvent(DragSizeSettingChanged, e);
        }

        /// <summary>
        /// Handler called when the base rendering offset for the segments (scroll position) changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentOffsetChanged(WindowEventArgs e)
        {
            FireEvent(SegmentRenderOffsetChanged, e);
        }

	    protected void/*bool*/ SegmentSizedHandler(object sender,WindowEventArgs e)
	    {
	        LayoutSegments();

	        // Fire segment sized event.
	        OnSegmentSized(new WindowEventArgs(e.Window));

	        // TODO: return true;
	    }

        protected void/*bool*/ SegmentMovedHandler(object sender, WindowEventArgs e)
        {
            var mousePos = GetUnprojectedPosition(GetGUIContext().GetCursor().GetPosition());

	        // segment must be dropped within the window
	        if (IsHit(mousePos))
	        {
		        // get mouse position as something local
	            var localMousePos = CoordConverter.ScreenToWindow(this, mousePos);

		        // set up to allow for current offsets
		        var currwidth = -_segmentOffset;

		        // calculate column where dragged segment was dropped
                int col;
		        for (col = 0; col < GetColumnCount(); ++col)
		        {
			        currwidth += _segments[col].GetPixelSize().Width;

			        if (localMousePos.X < currwidth)
			        {
				        // this is the column, exit loop early
				        break;
			        }
		        }

		        // find original column for dragged segment.
		        var seg = (ListHeaderSegment)e.Window;
		        var curcol = GetColumnFromSegment(seg);

		        // move column
		        MoveColumn(curcol, col);
	        }

	        // TODO: return true;
	    }

        protected void/*bool*/ SegmentClickedHandler(object sender, WindowEventArgs e)
	    {
	        // double-check we allow this action
	        if (_sortingEnabled)
	        {
	            var seg = (ListHeaderSegment) e.Window;

		        // is this a new sort column?
		        if (_sortSegment != seg)
		        {
			        _sortDirection = ListHeaderSegment.SortDirection.Descending;
			        SetSortSegment(seg);
		        }
		        // not a new segment, toggle current direction
		        else if (_sortSegment!=null)
		        {
			        var currDir = _sortSegment.GetSortDirection();

			        // set new direction based on the current value.
			        switch (currDir)
			        {
			        case ListHeaderSegment.SortDirection.None:
				        SetSortDirection(ListHeaderSegment.SortDirection.Descending);
				        break;

			        case ListHeaderSegment.SortDirection.Ascending:
				        SetSortDirection(ListHeaderSegment.SortDirection.Descending);
				        break;

			        case ListHeaderSegment.SortDirection.Descending:
				        SetSortDirection(ListHeaderSegment.SortDirection.Ascending);
				        break;
			        }

		        }

		        // Notify that a segment has been clicked
		        OnSegmentClicked(new WindowEventArgs(e.Window));
	        }

	        // TODO: return true;

	    }
        protected void/*bool*/ SegmentDoubleClickHandler(object sender, WindowEventArgs e)
	    {
	        OnSplitterDoubleClicked(new WindowEventArgs(e.Window));
	        // TODO: return true;
	    }

        protected void/*bool*/ SegmentDragHandler(object sender, WindowEventArgs e)
	    {
	        // what we do here is monitor the position and scroll if we can when mouse is outside area.

	        // get mouse position as something local
            var localMousePos = CoordConverter.ScreenToWindow(this,
                                                              GetUnprojectedPosition(
                                                                  GetGUIContext().GetCursor().GetPosition()));

	        // scroll left?
	        if (localMousePos.X < 0.0f)
	        {
		        if (_segmentOffset > 0.0f)
		        {
			        SetSegmentOffset(Math.Max(0.0f, _segmentOffset - ScrollSpeed));
		        }
	        }
	        // scroll right?
	        else if (localMousePos.X >= d_pixelSize.Width)
	        {
		        var maxOffset = Math.Max(0.0f, GetTotalSegmentsPixelExtent() - d_pixelSize.Width);

		        // if we have not scrolled to the limit
		        if (_segmentOffset < maxOffset)
		        {
			        // scroll, but never beyond the limit
			        SetSegmentOffset(Math.Min(maxOffset, _segmentOffset + ScrollSpeed));
		        }

	        }

	        // TODO: return true;
	    }

        private void AddHeaderProperties()
        {
            // TODO: Inconsistency
            DefineProperty(
                "SortSettingEnabled",
                "Property to get/set the setting for for user modification of the sort column & direction.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetSortingEnabled(v), x => x.IsSortingEnabled(), true);

            // TODO: Inconsistency
            DefineProperty(
                "ColumnsSizable",
                "Property to get/set the setting for user sizing of the column headers.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetColumnSizingEnabled(v), x => x.IsColumnSizingEnabled(), true);

            // TODO: Inconsistency
            DefineProperty(
                "ColumnsMovable",
                "Property to get/set the setting for user moving of the column headers.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetColumnDraggingEnabled(v), x => x.IsColumnDraggingEnabled(), true);

            // TODO: Inconsistency
            DefineProperty(
                "SortColumnID",
                "Property to get/set the current sort column (via ID code). Value is an unsigned integer number.",
                (x, v) => x.SetSortColumnFromId(v), x => x.GetSortSegmentId(), 0);

            DefineProperty(
                "SortDirection",
                "Property to get/set the sort direction setting of the header. Value is the text of one of the SortDirection enumerated value names.",
                (x, v) => x.SetSortDirection(v), x => x.GetSortDirection(), ListHeaderSegment.SortDirection.None);
        }

        private void DefineProperty<T>(string name, string help, Action<ListHeader, T> setter, Func<ListHeader, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<ListHeader, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #region Fields

        /// <summary>
        /// Pointer to the segment that is currently set as the sork-key,
        /// </summary>
        private ListHeaderSegment _sortSegment;

        /// <summary>
        /// true if segments can be sized by the user.
        /// </summary>
        private bool _sizingEnabled;

        /// <summary>
        /// true if the sort criteria modifications by user are enabled (no sorting is actuall done)
        /// </summary>
        private bool _sortingEnabled;

        /// <summary>
        /// true if drag &amp; drop moving of columns / segments is enabled.
        /// </summary>
        private bool _movingEnabled;

        /// <summary>
        /// field used to create unique names.
        /// </summary>
        private int _uniqueIdNumber;
        
        /// <summary>
        /// Base offset used to layout the segments (allows scrolling within the window area)
        /// </summary>
        private float _segmentOffset;

        /// <summary>
        /// copy of the current sort direction.
        /// </summary>
        private ListHeaderSegment.SortDirection _sortDirection;

        /// <summary>
        /// Attached segment windows in header order.
        /// </summary>
        private readonly List<ListHeaderSegment> _segments = new List<ListHeaderSegment>();

        #endregion
    }
}