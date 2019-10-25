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
using System.Diagnostics;

namespace SharpCEGui.Base
{
    /// <summary>
    /// A positioned and sized rectangular node in a tree graph
    /// 
    /// This class implements positioning, alignment, sizing including minimum and
    /// maximum size constraining. In its bare essense it's an unnamed rectangular node
    /// that may contain other unnamed rectangular nodes.
    /// 
    /// Unless you are implementing new CEGUI functionality you do NOT want to use this
    /// class directly. You most likely want to use <see cref="Window"/>.
    /// </summary>
    public abstract class Element : PropertySet, IEventSet
    {
        #region Implementation of IEventSet

        public bool IsMuted()
        {
            return _muted;
        }

        public void SetMutedState(bool value)
        {
            _muted = value;
        }

        //public BoundSlot SubscribeEvent<TEventArgs>(string name, GuiEventHandler<TEventArgs> function)
        //    where TEventArgs : EventArgs
        //{
        //    return SubscribeEvent(name, new SubscriberSlot(args => function((TEventArgs) args)));
        //}

        public BoundSlot SubscribeEvent(string name, GuiEventHandler<EventArgs> function)
        {
            //return SubscribeEvent(name, new SubscriberSlot(function));
            return GetEventObject(name, true).Subscribe(function);
        }

        //public BoundSlot SubscribeEvent(string name, SubscriberSlot subscriber)
        //{
        //    return GetEventObject(name, true).Subscribe(subscriber);
        //}

        public void UnsubscribeEvent(string name, GuiEventHandler<EventArgs> function)
        {
            var @event = GetEventObject(name);
            if (@event != null)
                @event.Unsubscribe(function);
        }

        public void FireEvent(string name, EventArgs args, string eventNamespace = "")
        {
            GlobalEventSet ges = GlobalEventSet.GetSingleton();
            if (ges != null)
                ges.FireEvent(name, args, eventNamespace);

            var ev = GetEventObject(name);
            if (ev != null && !_muted)
                ev.Invoke(args);
        }

        internal void AddEvent(string name)
        {
            AddEvent(new Event(name));
        }

        internal void AddEvent(Event @event)
        {
            if (IsEventPresent(@event.Name))
                throw new AlreadyExistsException("An event named '" + @event.Name + "' already exists in the EventSet.");

            _events.Add(@event.Name, @event);
        }

        internal bool IsEventPresent(string name)
        {
            return _events.ContainsKey(name);
        }
        
        private Event GetEventObject(string name, bool autoAdd = false)
        {
            if (!_events.ContainsKey(name))
            {
                if (!autoAdd)
                    return null;

                AddEvent(name);
            }

            return _events[name];
        }

        /// <summary>
        /// true if events for this EventSet have been muted.
        /// </summary>
        private bool _muted;

        private readonly Dictionary<string, Event> _events = new Dictionary<string, Event>();

        #endregion

        #region Events

        #region Event Strings

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public const string EventNamespace = "Element";
        public const string EventSized="Sized";
        public const string EventParentSized="ParentSized";
        public const string EventMoved="Moved";
        public const string EventHorizontalAlignmentChanged="HorizontalAlignmentChanged";
        public const string EventVerticalAlignmentChanged="VerticalAlignmentChanged";
        public const string EventRotated="Rotated";
        public const string EventChildAdded="ChildAdded";
        public const string EventChildRemoved="ChildRemoved";
        public const string EventZOrderChanged="ZOrderChanged";
        public const string EventNonClientChanged="NonClientChanged";

        #endregion

        /// <summary>
        /// Event fired when the Element size has changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> Sized
        {
            add { SubscribeEvent(EventSized, value); }
            remove { UnsubscribeEvent(EventSized, value); }
        }

        /// <summary>
        /// Event fired when the parent of this Element has been re-sized.
        /// </summary>
        /// <remarks>
        /// <see cref="ElementEventArgs.element"/> pointing to the <em>parent element</em> that
        /// was resized, not the element whose parent was resized.
        /// </remarks>
        public event GuiEventHandler<EventArgs> ParentSized
        {
            add { SubscribeEvent(EventParentSized, value); }
            remove { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Event fired when the Element position has changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> Moved
        {
            add { SubscribeEvent(EventMoved, value); }
            remove { UnsubscribeEvent(EventMoved, value); }
        }

        /// <summary>
        /// Event fired when the horizontal alignment for the element is changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> HorizontalAlignmentChanged
        {
            add { SubscribeEvent(EventHorizontalAlignmentChanged, value); }
            remove { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Event fired when the vertical alignment for the element is changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> VerticalAlignmentChanged
        {
            add { SubscribeEvent(EventVerticalAlignmentChanged, value); }
            remove { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Event fired when the rotation factor(s) for the element are changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> Rotated
        {
            add { SubscribeEvent(EventRotated, value); }
            remove { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Event fired when a child Element has been added.
        /// </summary>
        public event GuiEventHandler<EventArgs> ChildAdded
        {
            add { SubscribeEvent(EventChildAdded, value); }
            remove { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Event fired when a child element has been removed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ChildRemoved
        {
            add { SubscribeEvent(EventChildRemoved, value); }
            remove { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Event fired when the z-order of the element has changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ZOrderChanged
        {
            add { SubscribeEvent(EventZOrderChanged, value); }
            remove { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Event fired when the non-client setting for the Element is changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> NonClientChanged
        {
            add { SubscribeEvent(EventNonClientChanged, value); }
            remove { throw new NotImplementedException(); }
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// A tiny wrapper to hide some of the dirty work of rect caching.
        /// This is used internally by <see cref="Element"/> and other classes, it is passed
        /// to the user in several methods. In those circumstances you most likely
        /// want the result of either the <see cref="Get"/> or <see cref="GetFresh(bool)"/> methods.
        /// </summary>
        public class CachedRectf
        {
            /// <summary>
            /// Function to generate fresh data that might later be cached
            /// <para>
            /// If the bool is true all PixelAlignment settings will be overridden
            /// and no pixel alignment will take place.
            /// </para>
            /// </summary>
            /// <param name="element"></param>
            /// <param name="generator"></param>
            public CachedRectf(Element element, Func<Element, bool, Rectf> generator)
            {
                _element = element;
                _generator = generator;
                // we don't have to initialise _cachedData here, it will get
                // regenerated and reset anyways
                _cacheValid = false;
            }

            /// <summary>
            /// Retrieves cached Rectf or generated a fresh one and caches it 
            /// </summary>
            /// <returns></returns>
            public Rectf Get()
            {
                if (!_cacheValid)
                {
                    RegenerateCache();
                }

                return _cachedData;
            }

            /// <summary>
            /// Skips all caching and calls the generator.
            /// This method will cache the result if cache is invalid and
            /// alignment is not being skipped.
            /// </summary>
            /// <param name="skipAllPixelAlignment"></param>
            /// <returns></returns>
            public Rectf GetFresh(bool skipAllPixelAlignment = false)
            {
                // if the cache is not valid we will use this chance to regenerate it
                // of course this is only applicable if we are allowed to use 
                // pixel alignment where applicable
                if (!_cacheValid && !skipAllPixelAlignment)
                {
                    return Get();
                }

                return _generator(_element, skipAllPixelAlignment);
            }

            /// <summary>
            /// Invalidates the cached Rectf causing it to be regenerated 
            /// </summary>
            /// <remarks>
            /// The regeneration will not happen immediately, it will happen when user
            /// requests the data.
            /// </remarks>
            public void InvalidateCache()
            {
                _cacheValid = false;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool IsCacheValid()
            {
                return _cacheValid;
            }

            /// <summary>
            /// 
            /// </summary>
            public void RegenerateCache()
            {
                // false, since when we are caching we don't want to skip anything, 
                // we want everything to act exactly as it was setup
                _cachedData = _generator(_element, false);

                _cacheValid = true;
            }

            #region Fields

            private Rectf _cachedData;
            private bool _cacheValid;

            private readonly Element _element;
            private readonly Func<Element, bool, Rectf> _generator;

            #endregion
        }

        #endregion

        protected Element()
        {
            d_parent = null;
            d_nonClient = false;
            d_area = new URect(UDim.Zero, UDim.Zero, UDim.Zero, UDim.Zero);
            d_horizontalAlignment = HorizontalAlignment.Left;
            d_verticalAlignment = VerticalAlignment.Top;
            d_minSize = USize.Zero;
            d_maxSize = USize.Zero;
            d_aspectMode = AspectMode.Ignore;
            d_aspectRatio = 1f/1f;
            d_pixelAligned = true;
            d_pixelSize = Sizef.Zero;
            d_rotation = Lunatics.Mathematics.Quaternion.Identity;

            d_unclippedOuterRect = new CachedRectf(this, (element, b) => element.GetUnclippedOuterRectImpl(b));
            d_unclippedInnerRect = new CachedRectf(this, (element, b) => element.GetUnclippedInnerRectImpl(b));

            AddElementProperties();
        }
        
        #region Implementation of IDisposable

        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        [Conditional("DEBUG")]
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                var objectName = GetType().Name;
                throw new ObjectDisposedException(objectName);
            }
        }

        #endregion

        /// <summary>
        /// Retrieves parent of this element
        /// </summary>
        /// <returns>
        /// Parent or null, null means that this Element is a root of
        /// the subtree it represents
        /// </returns>
        public Element GetParentElement()
        {
            ThrowIfDisposed();
            return d_parent;
        }

        /// <summary>
        /// Set the Element area.
        /// <para>
        /// Sets the area occupied by this Element. The defined area is offset from
        /// one of the corners and edges of this Element's parent element (depending on alignments)
        /// or from the top-left corner of the display if this element has no parent
        /// (i.e. if it is the root element).
        /// </para>
        /// </summary>
        /// <param name="pos">
        /// UVector2 describing the new position of the element area. Meaning of
        /// position depends on currently set alignments. By default it is the
        /// offset from the top-left corner of widget's parent.
        /// </param>
        /// <param name="size">
        /// USize describing the new size of the element area.
        /// </param>
        /// <seealso cref="UDim"/>
        public virtual void SetArea(UVector2 pos, USize size)
        {
            ThrowIfDisposed();
            SetAreaImpl(pos, size);
        }
        
        /// <see cref="SetArea(UVector2, USize)"/> 
        public void SetArea(UDim x, UDim y, UDim width, UDim height)
        {
            ThrowIfDisposed();
            SetArea(new UVector2(x, y), new USize(width, height));
        }

        /// <see cref="SetArea(UVector2, USize)"/> 
        public void SetArea(URect value)
        {
            ThrowIfDisposed();
            SetArea(value.d_min, value.Size);
        }

        /// <summary>
        /// Return the element's area.
        /// <para>
        /// Sets the area occupied by this Element. The defined area is offset from
        /// one of the corners and edges of this Element's parent element (depending on alignments)
        /// or from the top-left corner of the display if this element has no parent
        /// (i.e. it is the root element).
        /// </para>
        /// </summary>
        /// <returns>
        /// URect describing the rectangle of the element area.
        /// </returns>
        /// <seealso cref="UDim"/>
        public URect GetArea()
        {
            ThrowIfDisposed();
            return d_area;
        }

        /// <summary>
        /// Set the element's position.
        /// <para>
        /// Sets the position of the area occupied by this element. The position is offset from
        /// one of the corners and edges of this Element's parent element (depending on alignments)
        /// or from the top-left corner of the display if this element has no parent
        /// (i.e. it is the root element).
        /// </para>
        /// </summary>
        /// <param name="value">
        /// UVector2 describing the new position of the element area.
        /// </param>
        /// <seealso cref="UDim"/>
        /// <seealso cref="SetArea(SharpCEGui.Base.UVector2,SharpCEGui.Base.USize)"/>
        public void SetPosition(UVector2 value)
        {
            ThrowIfDisposed();
            SetAreaImpl(value, d_area.Size);
        }

        /// <summary>
        /// Get the element's position.
        /// <para>
        /// Sets the position of the area occupied by this element. The position is offset from
        /// one of the corners of this Element's parent element (depending on alignments)
        /// or from the top-left corner of the display if this element has no parent
        /// (i.e. it is the root element).
        /// </para>
        /// </summary>
        /// <returns>
        /// UVector2 describing the position of the element area.
        /// </returns>
        /// <seealso cref="UDim"/>
        public UVector2 GetPosition()
        {
            ThrowIfDisposed();
            return d_area.Position;
        }

        /// <see cref="SetPosition"/>
        public void SetXPosition(UDim value)
        {
            ThrowIfDisposed();
            SetPosition(new UVector2(value, GetYPosition()));
        }

        /// <see cref="GetPosition"/>
        public UDim GetXPosition()
        {
            ThrowIfDisposed();
            return GetPosition().d_x;
        }

        /// <see cref="SetPosition"/>
        public void SetYPosition(UDim value)
        {
            ThrowIfDisposed();
            SetPosition(new UVector2(GetXPosition(), value));
        }

        /// <see cref="GetPosition"/>
        public UDim GetYPosition()
        {
            ThrowIfDisposed();
            return GetPosition().d_y;
        }

        /// <summary>
        /// Set the horizontal alignment.
        /// <para>
        /// Modifies the horizontal alignment for the element. This setting affects
        /// how the element's position is interpreted relative to its parent.
        /// </para>
        /// </summary>
        /// <param name="value">
        /// One of the HorizontalAlignment enumerated values.
        /// </param>
        public virtual void SetHorizontalAlignment(HorizontalAlignment value)
        {
            ThrowIfDisposed();

            if (d_horizontalAlignment == value)
                return;

            d_horizontalAlignment = value;

            var args = new ElementEventArgs(this);
            OnHorizontalAlignmentChanged(args);
        }

        /// <summary>
        /// Get the horizontal alignment.
        /// <para>
        /// Returns the horizontal alignment for the element. This setting affects
        /// how the element's position is interpreted relative to its parent.
        /// </para>
        /// </summary>
        /// <returns>
        /// One of the HorizontalAlignment enumerated values.
        /// </returns>
        public HorizontalAlignment GetHorizontalAlignment()
        {
            ThrowIfDisposed();

            return d_horizontalAlignment;
        }

        /// <summary>
        /// Set the vertical alignment.
        /// <para>
        /// Modifies the vertical alignment for the element. This setting affects
        /// how the element's position is interpreted relative to its parent.
        /// </para>
        /// </summary>
        /// <param name="value">
        /// One of the VerticalAlignment enumerated values.
        /// </param>
        public virtual void SetVerticalAlignment(VerticalAlignment value)
        {
            ThrowIfDisposed();

            if (d_verticalAlignment == value)
                return;

            d_verticalAlignment = value;

            var args = new ElementEventArgs(this);
            OnVerticalAlignmentChanged(args);
        }

        /// <summary>
        /// Get the vertical alignment.
        /// <para>
        /// Returns the vertical alignment for the element.  This setting affects how
        /// the element's position is interpreted relative to its parent.
        /// </para>
        /// </summary>
        /// <returns>One of the VerticalAlignment enumerated values.</returns>
        public VerticalAlignment GetVerticalAlignment()
        {
            ThrowIfDisposed();

            return d_verticalAlignment;
        }

        /// <summary>
        /// Set the element's size.
        /// <para>
        /// Sets the size of the area occupied by this element.
        /// </para>
        /// </summary>
        /// <param name="value">USize describing the new size of the element's area.</param>
        public void SetSize(USize value)
        {
            ThrowIfDisposed();

            SetArea(d_area.Position, value);
        }

        /// <summary>
        /// Get the element's size.
        /// <para>
        /// Gets the size of the area occupied by this element.
        /// </para> 
        /// </summary>
        /// <returns>USize describing the size of the element's area.</returns>
        public USize GetSize()
        {
            ThrowIfDisposed();

            return d_area.Size;
        }

        /// <see cref="SetSize"/>
        public void SetWidth(UDim value)
        {
            ThrowIfDisposed();

            SetSize(new USize(value, GetSize().d_height));
        }

        /// <see cref="GetSize"/>
        public UDim GetWidth()
        {
            ThrowIfDisposed();

            return GetSize().d_width;
        }

        /// <see cref="SetSize"/>
        public void SetHeight(UDim value)
        {
            ThrowIfDisposed();

            SetSize(new USize(GetSize().d_width, value));
        }

        /// <see cref="GetSize"/>
        public UDim GetHeight()
        {
            ThrowIfDisposed();

            return GetSize().d_height;
        }

        /// <summary>
        /// Set the element's minimum size.
        /// <para>
        /// Sets the minimum size that this element's area may occupy (whether size
        /// changes occur by user interaction, general system operation, or by
        /// direct setting by client code).
        /// </para>
        /// <remarks>
        /// The scale component of UDim takes display size as the base.
        /// It is not dependent on parent element's size!
        /// </remarks>
        /// </summary>
        /// <param name="value">
        /// USize describing the new minimum size of the element's area.
        /// </param>
        /// <seealso cref="SetSize"/>
        public void SetMinSize(USize value)
        {
            ThrowIfDisposed();

            d_minSize = value;

            // TODO: Perhaps we could be more selective and skip this if min size won't affect the size
            SetSize(GetSize());
        }

        /// <summary>
        ///  Get the element's minimum size.
        /// <para>
        /// Gets the minimum size that this element's area may occupy (whether size
        /// changes occur by user interaction, general system operation, or by
        /// direct setting by client code).
        /// </para>
        /// </summary>
        /// <returns>
        /// UVector2 describing the minimum size of the element's area.
        /// </returns>
        /// <seealso cref="SetMinSize"/>
        public USize GetMinSize()
        {
            ThrowIfDisposed();

            return d_minSize;
        }

        /// <summary>
        /// Set the element's maximum size.
        /// <para>
        /// Sets the maximum size that this element area may occupy (whether size
        /// changes occur by user interaction, general system operation, or by
        /// direct setting by client code).
        /// </para>
        /// </summary>
        /// <remarks>
        /// The scale component of UDim takes display size as the base.
        /// It is not dependent on parent element's size!
        /// </remarks>
        /// <param name="value">
        /// USize describing the new maximum size of the element's area.  Note that
        /// zero is used to indicate that the Element's maximum area size will be
        /// unbounded.
        /// </param>
        /// <seealso cref="SetSize"/>
        public void SetMaxSize(USize value)
        {
            ThrowIfDisposed();

            d_maxSize = value;

            // TODO: Perhaps we could be more selective and skip this if max size won't affect the size
            SetSize(GetSize());
        }

        /// <summary>
        /// Get the element's maximum size.
        /// <para>
        /// Gets the maximum size that this element area may occupy (whether size
        /// changes occur by user interaction, general system operation, or by
        /// direct setting by client code).
        /// </para>
        /// </summary>
        /// <returns>
        /// UVector2 describing the maximum size of the element's area.
        /// </returns>
        /// <seealso cref="SetMaxSize"/>
        public USize GetMaxSize()
        {
            ThrowIfDisposed();

            return d_maxSize;
        }

        /// <summary>
        /// Sets current aspect mode and recalculates the area rect
        /// </summary>
        /// <param name="value">
        /// the new aspect mode to set
        /// </param>
        /// <seealso cref="AspectMode"/>
        /// <seealso cref="SetAspectRatio"/>
        public void SetAspectMode(AspectMode value)
        {
            ThrowIfDisposed();

            if (d_aspectMode == value)
                return;

            d_aspectMode = value;

            // TODO: We want an Event and more smart rect update handling

            // Ensure the area is calculated with the new aspect mode
            // TODO: This potentially wastes effort, we should just mark it as dirty
            //       and postpone the calculation for as long as possible
            SetArea(GetArea());
        }

        /// <summary>
        /// Retrieves currently used aspect mode
        /// </summary>
        /// <returns></returns>
        public AspectMode GetAspectMode()
        {
            ThrowIfDisposed();

            return d_aspectMode;
        }

        /// <summary>
        /// Sets target aspect ratio
        /// </summary>
        /// <param name="value">
        /// The desired ratio as width / height. For example 4.0f / 3.0f, 16.0f / 9.0.f, ...
        /// </param>
        /// <remarks>
        /// This is ignored if AspectMode is AM_IGNORE.
        /// </remarks>
        /// <seealso cref="SetAspectRatio"/>
        public void SetAspectRatio(float value)
        {
            ThrowIfDisposed();

            if (d_aspectRatio == value)
                return;

            d_aspectRatio = value;

            // TODO: We want an Event and more smart rect update handling

            // Ensure the area is calculated with the new aspect ratio
            // TODO: This potentially wastes effort, we should just mark it as dirty
            //       and postpone the calculation for as long as possible
            SetArea(GetArea());
        }

        /// <summary>
        /// Retrieves target aspect ratio
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="SetAspectRatio"/>
        public float GetAspectRatio()
        {
            ThrowIfDisposed();

            return d_aspectRatio;
        }

        /// <summary>
        /// Sets whether this Element is pixel aligned (both position and size, basically the 4 "corners").
        /// </summary>
        /// <para>
        /// Impact on the element tree
        /// Lets say we have Element A with child Element B, A is pixel aligned
        /// and it's position is 99.5, 99.5 px in screenspace. This gives us
        /// 100, 100 px pixel aligned position.
        /// 
        /// B's position is always relative to the pixel-aligned position of its
        /// parent. Say B isn't pixel-aligned and it's position is 0.5, 0.5 px.
        /// Its final position will be 100.5, 100.5 px in screenspace, not 100, 100 px!
        /// 
        /// If it were pixel-aligned the final position would be 101, 101 px.
        /// </para>
        /// <para>
        /// Why you should pixel-align widgets
        /// Pixel aligning is enabled by default and for most widgets it makes
        /// a lot of sense and just looks better. Especially with text. However for
        /// HUD or decorative elements pixel aligning might make transitions less
        /// fluid. Feel free to experiment with the setting.
        /// </para>
        /// <param name="value"></param>
        public void SetPixelAligned(bool value)
        {
            ThrowIfDisposed();

            if (d_pixelAligned == value)
                return;

            d_pixelAligned = value;

            // TODO: We want an Event and more smart rect update handling

            // Ensure the area is calculated with the new pixel aligned setting
            // TODO: This potentially wastes effort, we should just mark it as dirty
            //       and postpone the calculation for as long as possible
            SetArea(GetArea());
        }

        /// <summary>
        /// Checks whether this Element is pixel aligned
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="SetPixelAligned"/>
        public bool IsPixelAligned()
        {
            ThrowIfDisposed();

            return d_pixelAligned;
        }

        /// <summary>
        /// Return the element's absolute (or screen, depending on the type of the element) position in pixels.
        /// </summary>
        /// <returns>
        /// Vector2f object describing this element's absolute position in pixels.
        /// </returns>
        public Lunatics.Mathematics.Vector2 GetPixelPosition()
        {
            ThrowIfDisposed();

            return GetUnclippedOuterRect().Get().d_min;
        }

        /// <summary>
        /// Return the element's size in pixels.
        /// </summary>
        /// <returns>
        /// Size object describing this element's size in pixels.
        /// </returns>
        public Sizef GetPixelSize()
        {
            ThrowIfDisposed();

            return d_pixelSize;
        }

        /// <summary>
        /// Calculates this element's pixel size
        /// </summary>
        /// <param name="skipAllPixelAlignment">
        /// Should all pixel-alignment be skipped when calculating the pixel size?
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// If you want to get the pixel size you most probably want to use the
        /// Element::getPixelSize method. This method skips caching and might
        /// impact performance!
        /// </remarks>
        public Sizef CalculatePixelSize(bool skipAllPixelAlignment = false)
        {
            ThrowIfDisposed();

            // calculate pixel sizes for everything, so we have a common format for comparisons.
            var absMin = CoordConverter.AsAbsolute(d_minSize, GetRootContainerSize(), false);
            var absMax = CoordConverter.AsAbsolute(d_maxSize, GetRootContainerSize(), false);

            Sizef baseSize;
            if (skipAllPixelAlignment)
            {
                baseSize = (d_parent != null && !d_nonClient)
                               ? d_parent.GetUnclippedInnerRect().GetFresh(true).Size
                               : GetParentPixelSize(true);
            }
            else
            {
                baseSize = (d_parent != null && !d_nonClient)
                               ? d_parent.GetUnclippedInnerRect().Get().Size
                               : GetParentPixelSize();
            }

            var ret = CoordConverter.AsAbsolute(d_area.Size, baseSize, false);

            // in case absMin components are larger than absMax ones,
            // max size takes precedence
            if (absMax.Width != 0.0f && absMin.Width> absMax.Width)
            {
                absMin.Width = absMax.Width;
                Logger.LogInsane(
                    "MinSize resulted in an absolute pixel size with width larger than what MaxSize resulted in");
            }

            if (absMax.Height != 0.0f && absMin.Height > absMax.Height)
            {
                absMin.Height = absMax.Height;
                Logger.LogInsane("MinSize resulted in an absolute pixel size with height larger than what MaxSize resulted in");
            }

            // limit new pixel size to: minSize <= newSize <= maxSize
            if (ret.Width < absMin.Width)
                ret.Width = absMin.Width;
            else if (absMax.Width != 0.0f && ret.Width > absMax.Width)
                ret.Width = absMax.Width;

            if (ret.Height < absMin.Height)
                ret.Height = absMin.Height;
            else if (absMax.Height != 0.0f && ret.Height > absMax.Height)
                ret.Height = absMax.Height;

            if (d_aspectMode != AspectMode.Ignore)
            {
                // make sure we respect current aspect mode and ratio
                ret.ScaleToAspect(d_aspectMode, d_aspectRatio);

                // make sure we haven't blown any of the hard limits
                // still maintain the aspect when we do this
                if (d_aspectMode == AspectMode.Shrink)
                {
                    float ratio = 1.0f;
                    // check that we haven't blown the min size
                    if (ret.Width < absMin.Width)
                    {
                        ratio = absMin.Width/ret.Width;
                    }
                    if (ret.Height < absMin.Height)
                    {
                        float newRatio = absMin.Height / ret.Height;
                        if (newRatio > ratio)
                            ratio = newRatio;
                    }

                    ret.Width *= ratio;
                    ret.Height *= ratio;
                }
                else if (d_aspectMode == AspectMode.Expand)
                {
                    float ratio = 1.0f;
                    // check that we haven't blown the min size
                    if (absMax.Width != 0.0f && ret.Width > absMax.Width)
                    {
                        ratio = absMax.Width / ret.Width;
                    }
                    if (absMax.Height != 0.0f && ret.Height > absMax.Height)
                    {
                        float newRatio = absMax.Height / ret.Height;
                        if (newRatio > ratio)
                            ratio = newRatio;
                    }

                    ret.Width *= ratio;
                    ret.Height *= ratio;
                }
                // NOTE: When the hard min max limits are unsatisfiable with the aspect lock mode,
                //       the result won't be limited by both limits!
            }

            if (d_pixelAligned)
            {
                ret.Width = CoordConverter.AlignToPixels(ret.Width);
                ret.Height = CoordConverter.AlignToPixels(ret.Height);
            }

            return ret;
        }

        /// <summary>
        /// Return the pixel size of the parent element.
        /// <para>
        /// If this element doesn't have any parent, the display size will be returned.
        /// This method returns a valid Sizef object in all cases.
        /// </para>
        /// </summary>
        /// <param name="skipAllPixelAlignment"></param>
        /// <returns>Size object that describes the pixel dimensions of this Element's parent</returns>
        public Sizef GetParentPixelSize(bool skipAllPixelAlignment = false)
        {
            ThrowIfDisposed();

            if (d_parent != null)
            {
                return skipAllPixelAlignment
                           ? d_parent.CalculatePixelSize(true)
                           : d_parent.GetPixelSize();
            }

            return GetRootContainerSize();
        }

        /// <summary>
        /// sets rotation of this widget
        /// </summary>
        /// <param name="value">A Quaternion describing the rotation</param>
        /// <para>
        /// Euler angles
        /// CEGUI used Euler angles previously. While these are easy to use and seem
        /// intuitive they cause Gimbal locks when animating and are overall the worse
        /// solution than using Quaternions. You can still use Euler angles, see
        /// the CEGUI::Quaternion class for more info about that.
        /// </para>
        public void SetRotation(Lunatics.Mathematics.Quaternion value)
        {
            ThrowIfDisposed();

            d_rotation = value;

            var args = new ElementEventArgs(this);
            OnRotated(args);
        }

        /// <summary>
        /// retrieves rotation of this widget
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="SetRotation"/>
        public Lunatics.Mathematics.Quaternion GetRotation()
        {
            ThrowIfDisposed();

            return d_rotation;
        }

        public void AddChild(Element element)
        {
            ThrowIfDisposed();

            if (element == null)
                throw new InvalidRequestException("Can't add NULL to Element as a child!");

            if (element == this)
                throw new InvalidRequestException("Can't make element its own child - " +
                                                  "this->addChild(this); is forbidden.");

            AddChildImpl(element);

            OnChildAdded(new ElementEventArgs(element));
        }

        public void RemoveChild(Element element)
        {
            ThrowIfDisposed();

            if (element == null)
                throw new InvalidRequestException("NULL can't be a child of any Element, " +
                                                  "it makes little sense to ask for its " +
                                                  "removal");
            RemoveChildImpl(element);
            OnChildRemoved(new ElementEventArgs(element));
        }

        public Element GetChildElementAtIdx(int index)
        {
            return d_children[index];
        }

        public int GetChildCount()
        {
            return d_children.Count;
        }

        /// <summary>
        /// Checks whether given element is attached to this Element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsChild(Element element)
        {
            return d_children.Contains(element);
        }

        public bool IsAncestor(Element element)
        {
            // no parent, no ancestor, nothing can be our ancestor
            if (d_parent == null)
                return false;

            return d_parent == element || d_parent.IsAncestor(element);
        }

        public void SetNonClient(bool value)
        {
            if (value == d_nonClient)
                return;

            d_nonClient = value;

            OnNonClientChanged(new ElementEventArgs(this));
        }

        public bool IsNonClient()
        {
            return d_nonClient;
        }

        public CachedRectf GetUnclippedOuterRect()
        {
            return d_unclippedOuterRect;
        }

        public CachedRectf GetUnclippedInnerRect()
        {
            return d_unclippedInnerRect;
        }

        public CachedRectf GetUnclippedRect(bool inner)
        {
            return inner ? GetUnclippedInnerRect() : GetUnclippedOuterRect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual CachedRectf GetClientChildContentArea()
        {
            return GetUnclippedInnerRect();
        }

        public virtual CachedRectf GetNonClientChildContentArea()
        {
            return GetUnclippedOuterRect();
        }

        public CachedRectf GetChildContentArea(bool nonClient = false)
        {
            return nonClient ? GetNonClientChildContentArea() : GetClientChildContentArea();
        }

        public virtual void NotifyScreenAreaChanged(bool recursive = true)
        {
            d_unclippedOuterRect.InvalidateCache();
            d_unclippedInnerRect.InvalidateCache();

            if (!recursive)
                return;

            // inform children that their screen area must be updated
            var childCount = GetChildCount();
            for (var i = 0; i < childCount; ++i)
                d_children[i].NotifyScreenAreaChanged();
        }

        public virtual Sizef GetRootContainerSize()
        {
            return System.GetSingleton().GetRenderer().GetDisplaySize();
        }

        /// <summary>
        /// Add standard CEGUI::Element properties.
        /// </summary>
        protected void AddElementProperties()
        {
            DefineProperty(
                "Area",
                "Property to get/set the unified area rectangle. Value is a \"URect\".",
                (e, v) => e.SetArea(v), e => e.GetArea(), URect.Zero);

            DefinePropertyNoXml(
                "Position",
                "Property to get/set the unified position. Value is a \"UVector2\".",
                (e, v) => e.SetPosition(v), e => e.GetPosition(), UVector2.Zero);

            DefinePropertyNoXml(
                "Width",
                "Property to get/set the unified width. Value is a \"UDim\".",
                (e, v) => e.SetWidth(v), e => e.GetWidth(), UDim.Zero);

            DefinePropertyNoXml(
                "Height",
                "Property to get/set the unified height. Value is a \"UDim\".",
                (e, v) => e.SetHeight(v), e => e.GetHeight(), UDim.Zero);

            DefineProperty(
                "VerticalAlignment",
                "Property to get/set the vertical alignment.  Value is one of \"Top\", \"Centre\" or \"Bottom\".",
                (x, v) => x.SetVerticalAlignment(v), x => x.GetVerticalAlignment(), VerticalAlignment.Top);

            DefineProperty(
                "HorizontalAlignment",
                "Property to get/set the horizontal alignment.  Value is one of \"Left\", \"Centre\" or \"Right\".",
                (x, v) => x.SetHorizontalAlignment(v), x => x.GetHorizontalAlignment(), HorizontalAlignment.Left);

            DefinePropertyNoXml(
                "Size", "Property to get/set the unified size. Value is a \"USize\".",
                (x, v) => x.SetSize(v), x => x.GetSize(), USize.Zero);

            DefineProperty(
                "MinSize", "Property to get/set the unified minimum size. Value is a \"USize\".",
                (x, v) => x.SetMinSize(v), x => x.GetMinSize(), USize.Zero);

            DefineProperty(
                "MaxSize",
                "Property to get/set the unified maximum size. Value is a \"USize\". Note that zero means no maximum size.",
                (x, v) => x.SetMaxSize(v), x => GetMaxSize(), USize.Zero);

            DefineProperty(
                "AspectMode",
                "Property to get/set the 'aspect mode' setting. Value is either \"Ignore\", \"Shrink\" or \"Expand\".",
                (x, v) => x.SetAspectMode(v), x => x.GetAspectMode(), AspectMode.Ignore);

            DefineProperty(
                "AspectRatio", "Property to get/set the aspect ratio. Only applies when aspect mode is not \"Ignore\".",
                (x, v) => x.SetAspectRatio(v), x => x.GetAspectRatio(), 1.0f);

            DefineProperty(
                "PixelAligned",
                "Property to get/set whether the Element's size and position should be pixel aligned. Value is either \"True\" or \"False\".",
                (x, v) => x.SetPixelAligned(v), x => x.IsPixelAligned(), true);

            DefineProperty(
                "Rotation",
                "Property to get/set the Element's rotation. Value is a quaternion: \"w:[w_float] x:[x_float] y:[y_float] z:[z_float]\"or \"x:[x_float] y:[y_float] z:[z_float]\" to convert from Euler angles (in degrees).",
                (x, v) => x.SetRotation(v), x => x.GetRotation(), Lunatics.Mathematics.Quaternion.Identity);

            DefineProperty(
                "NonClient",
                "Property to get/set whether the Element is 'non-client'. Value is either \"True\" or \"False\".",
                (x, v) => x.SetNonClient(v), x => x.IsNonClient(), false);
        }

        private void DefineProperty<T>(string name, string help, Action<Element,T> setter,Func<Element,T> getter,T defaultValue)
        {
            const string propertyOrigin = "Element";
            AddProperty(new TplWindowProperty<Element, T>(name, help, setter, getter, propertyOrigin, defaultValue));
        }

        private void DefinePropertyNoXml<T>(string name, string help, Action<Element, T> setter, Func<Element, T> getter, T defaultValue)
        {
            const string propertyOrigin = "Element";
            AddProperty(new TplWindowProperty<Element, T>(name, help, setter, getter, propertyOrigin, defaultValue,
                                                          false));
        }

        protected virtual void SetAreaImpl(UVector2 pos, USize size, bool topLeftSizing = false, bool fireEvents = true)
        {
            // we make sure the screen areas are recached when this is called as we need
            // it in most cases
            d_unclippedOuterRect.InvalidateCache();
            d_unclippedInnerRect.InvalidateCache();

            // save original size so we can work out how to behave later on
            var oldSize = d_pixelSize;

            d_area.Size = size;
            d_pixelSize = CalculatePixelSize();

            // have we resized the element?
            var sized = (d_pixelSize != oldSize);

            // If this is a top/left edge sizing op, only modify position if the size
            // actually changed.  If it is not a sizing op, then position may always
            // change.
            var moved = (!topLeftSizing || sized) && pos != d_area.d_min;

            if (moved)
                d_area.Position = pos;

            if (fireEvents)
                FireAreaChangeEvents(moved, sized);
        }

        /// <summary>
        /// helper to return whether the inner rect size has changed
        /// </summary>
        /// <returns></returns>
        protected bool IsInnerRectSizeChanged()
        {
            var oldSize = d_unclippedInnerRect.Get().Size;
            d_unclippedInnerRect.InvalidateCache();
            return oldSize != d_unclippedInnerRect.Get().Size;
        }

        /// <summary>
        /// Set the parent element for this element object.
        /// </summary>
        /// <param name="parent">
        /// Pointer to a Element object that is to be assigned as the parent to this
        /// Element.
        /// </param>
        protected virtual void SetParent(Element parent)
        {
            d_parent = parent;
        }

        /// <summary>
        /// Add given element to child list at an appropriate position
        /// </summary>
        /// <param name="element"></param>
        protected virtual void AddChildImpl(Element element)
        {
            // if element is attached elsewhere, detach it first (will fire normal events)
            var oldParent = element.GetParentElement();
            if (oldParent != null)
                oldParent.RemoveChild(element);

            // add element to child list
            d_children.Add(element);

            // set the parent element
            element.SetParent(this);

            // update area rects and content for the added element
            element.NotifyScreenAreaChanged();

            // correctly call parent sized notification if needed.
            if (oldParent == null || oldParent.GetPixelSize() != GetPixelSize())
            {
                element.OnParentSized(new ElementEventArgs(this));
            }
        }

        /// <summary>
        /// Remove given element from child list
        /// </summary>
        /// <param name="element"></param>
        protected virtual void RemoveChildImpl(Element element)
        {
            // if the element was found in the child list
            if (d_children.Contains(element))
            {
                // remove element from child list
                d_children.Remove(element);

                // reset element's parent so it's no longer this element.
                element.SetParent(null);
            }
        }

        /// <summary>
        /// Default implementation of function to return Element's outer rect area.
        /// </summary>
        /// <param name="skipAllPixelAlignment"></param>
        /// <returns></returns>
        protected virtual Rectf GetUnclippedOuterRectImpl(bool skipAllPixelAlignment)
        {
            var pixelSize = skipAllPixelAlignment
                                ? CalculatePixelSize(true)
                                : GetPixelSize();
            var ret = new Rectf(Lunatics.Mathematics.Vector2.Zero, pixelSize);

            var parent = GetParentElement();

            Rectf parentRectangle;
            if (parent != null)
            {
                var @base = parent.GetChildContentArea(IsNonClient());
                parentRectangle = skipAllPixelAlignment ? @base.GetFresh(true) : @base.Get();
            }
            else
            {
                parentRectangle = new Rectf(Lunatics.Mathematics.Vector2.Zero, GetRootContainerSize());
            }

            var parentSize = parentRectangle.Size;

            var offset = parentRectangle.d_min + CoordConverter.AsAbsolute(GetArea().d_min, parentSize, false);

            switch (GetHorizontalAlignment())
            {
                case HorizontalAlignment.Centre:
                    offset.X += (parentSize.Width - pixelSize.Width) * 0.5f;
                    break;
                case HorizontalAlignment.Right:
                    offset.X += parentSize.Width - pixelSize.Width;
                    break;
            }

            switch (GetVerticalAlignment())
            {
                case VerticalAlignment.Centre:
                    offset.Y += (parentSize.Height - pixelSize.Height)*0.5f;
                    break;
                case VerticalAlignment.Bottom:
                    offset.Y += parentSize.Height - pixelSize.Height;
                    break;
            }

            if (d_pixelAligned && !skipAllPixelAlignment)
            {
                offset = new Lunatics.Mathematics.Vector2(CoordConverter.AlignToPixels(offset.X),
                                                         CoordConverter.AlignToPixels(offset.Y));
            }

            ret.Offset(offset);

            return ret;
        }

        /// <summary>
        /// Default implementation of function to return Element's inner rect area.
        /// </summary>
        /// <param name="skipAllPixelAlignment"></param>
        /// <returns></returns>
        protected virtual Rectf GetUnclippedInnerRectImpl(bool skipAllPixelAlignment)
        {
            return skipAllPixelAlignment
                       ? GetUnclippedOuterRect().GetFresh(true)
                       : GetUnclippedOuterRect().Get();
        }

        /// <summary>
        /// helper to fire events based on changes to area rect
        /// </summary>
        /// <param name="moved"></param>
        /// <param name="sized"></param>
        protected void FireAreaChangeEvents(bool moved, bool sized)
        {
            if (moved)
            {
                OnMoved(new ElementEventArgs(this));
            }

            if (sized)
            {
                OnSized(new ElementEventArgs(this));
            }
        }

        protected void NotifyChildrenOfSizeChange(bool nonClient, bool client)
        {
            var childCount = GetChildCount();
            for (var i = 0; i < childCount; ++i)
            {
                var child = d_children[i];

                if ((nonClient && child.IsNonClient()) || (client && !child.IsNonClient()))
                {
                    d_children[i].OnParentSized(new ElementEventArgs(this));
                }
            }
        }

        #region Event Invocators

        /// <summary>
        /// Handler called when the element's size changes.
        /// </summary>
        /// <param name="e">
        /// ElementEventArgs object whose <see cref="ElementEventArgs.element"/> 
        /// field is set to the element that triggered the event.
        /// </param>
        protected virtual void OnSized(ElementEventArgs e)
        {
            NotifyScreenAreaChanged(false);
            NotifyChildrenOfSizeChange(true, true);

            FireEvent(EventSized, e, EventNamespace);
        }


        /// <summary>
        /// Handler called when this element's parent element has been resized. If
        /// his element is the root / GUI Sheet element, this call will be made when
        /// the display size changes.
        /// </summary>
        /// <param name="e">
        /// ElementEventArgs object whose 'element' pointer field is set the the
        /// element that caused the event; this is typically either this element's
        /// parent element, or NULL to indicate the screen size has changed.
        /// </param>
        protected internal virtual void OnParentSized(ElementEventArgs e)
        {
            d_unclippedOuterRect.InvalidateCache();
            d_unclippedInnerRect.InvalidateCache();

            var oldSize = d_pixelSize;
            d_pixelSize = CalculatePixelSize();
            var sized = (d_pixelSize != oldSize) || IsInnerRectSizeChanged();

            var moved = ((d_area.d_min.d_x.d_scale != 0) ||
                         (d_area.d_min.d_y.d_scale != 0) ||
                         (d_horizontalAlignment != HorizontalAlignment.Left) ||
                         (d_verticalAlignment != VerticalAlignment.Top));

            FireAreaChangeEvents(moved, sized);

            FireEvent(EventParentSized, e, EventNamespace);
        }

        protected virtual void OnMoved(ElementEventArgs e)
        {
            NotifyScreenAreaChanged();
            FireEvent(EventMoved, e, EventNamespace);
        }

        public void OnHorizontalAlignmentChanged(ElementEventArgs e)
        {
            NotifyScreenAreaChanged();
            FireEvent(EventHorizontalAlignmentChanged, e);
        }

        /// <summary>
        /// Handler called when the vertical alignment setting for the element is changed.
        /// </summary>
        /// <param name="e">
        /// ElementEventArgs object initialised as follows:
        ///     - element field is set to point to the element object who's alignment has
        ///       changed (typically 'this').
        /// </param>
        public void OnVerticalAlignmentChanged(ElementEventArgs e)
        {
            NotifyScreenAreaChanged();
            FireEvent(EventVerticalAlignmentChanged, e, EventNamespace);
        }

        protected virtual void OnRotated(ElementEventArgs e)
        {
            FireEvent(EventRotated, e, EventNamespace);
        }

        protected virtual void OnChildAdded(ElementEventArgs e)
        {
            FireEvent(EventChildAdded, e, EventNamespace);
        }

        protected virtual void OnChildRemoved(ElementEventArgs e)
        {
            FireEvent(EventChildRemoved, e, EventNamespace);
        }
        
        protected void OnZOrderChanged(ElementEventArgs e)
        {
            FireEvent(EventZOrderChanged, e, EventNamespace);
        }
        
        protected void OnNonClientChanged(ElementEventArgs e)
        {
            // TODO: Be less wasteful with this update
            SetArea(GetArea());

            FireEvent(EventNonClientChanged, e, EventNamespace);
        }
        
        #endregion

        #region Fields

        /// <summary>
        /// The list of child element objects attached to this.
        /// </summary>
        protected List<Element> d_children = new List<Element>();

        /// <summary>
        /// Holds pointer to the parent element.
        /// </summary>
        protected Element d_parent;

        /// <summary>
        /// true if element is in non-client (outside InnerRect) area of parent.
        /// </summary>
        protected bool d_nonClient;

        /// <summary>
        /// This element objects area as defined by a URectangle
        /// </summary>
        protected URect d_area;

        //! Specifies the base for horizontal alignment.
        protected HorizontalAlignment d_horizontalAlignment;

        //! Specifies the base for vertical alignment.
        protected VerticalAlignment d_verticalAlignment;

        //! current minimum size for the element.
        protected USize d_minSize;

        //! current maximum size for the element.
        protected USize d_maxSize;

        //! How to satisfy current aspect ratio
        protected AspectMode d_aspectMode;

        //! The target aspect ratio
        protected float d_aspectRatio;

        //! If true, the position and size are pixel aligned
        protected bool d_pixelAligned;
        
        //! Current constrained pixel size of the element.
        protected Sizef d_pixelSize;
        
        //! Rotation of this element (relative to the parent)
        protected Lunatics.Mathematics.Quaternion d_rotation;

        /// <summary>
        /// outer area rect in screen pixels
        /// </summary>
        protected CachedRectf d_unclippedOuterRect;

        //! inner area rect in screen pixels
        protected CachedRectf d_unclippedInnerRect;

        #endregion
    }
}
