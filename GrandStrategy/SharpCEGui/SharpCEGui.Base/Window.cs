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
using System.IO;
using System.Linq;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.Base
{
	/// <summary>
	/// An abstract base class providing common functionality and specifying the
	/// required interface for derived classes.
	/// 
	/// The Window base class is core UI object class that the the system knows
	/// about; for this reason, every other window, widget, or similar item within
	/// the system must be derived from Window.
	/// 
	/// The base class provides the common functionality required by all UI objects,
	/// and specifies the minimal interface required to be implemented by derived
	/// classes.
	/// </summary>
	public abstract class Window : NamedElement
	{
		#region Property Names

		public const string CursorImagePropertyName = "CursorImage";
		public const string VisiblePropertyName = "Visible";
		public const string RestoreOldCapturePropertyName = "RestoreOldCapture";
		public const string CursorPassThroughEnabledPropertyName = "CursorPassThroughEnabled";

		#endregion

		#region Events

		public new const string EventNamespace = "Window";

		#region Event Names

		public const string EventUpdated = "Updated";
		public const string EventTextChanged = "TextChanged";
		public const string EventFontChanged = "FontChanged";
		public const string EventAlphaChanged = "AlphaChanged";
		public const string EventIdChanged = "IDChanged";
		public const string EventActivated = "Activated";
		public const string EventDeactivated = "Deactivated";
		public const string EventShown = "Shown";
		public const string EventHidden = "Hidden";
		public const string EventEnabled = "Enabled";
		public const string EventDisabled = "Disabled";
		public const string EventClippedByParentChanged = "ClippedByParentChanged";
		public const string EventDestroyedByParentChanged = "DestroyedByParentChanged";
		public const string EventInheritsAlphaChanged = "InheritsAlphaChanged";
		public const string EventAlwaysOnTopChanged = "AlwaysOnTopChanged";
		public const string EventInputCaptureGained = "InputCaptureGained";
		public const string EventInputCaptureLost = "InputCaptureLost";
		public const string EventInvalidated = "Invalidated";
		public const string EventRenderingStarted = "RenderingStarted";
		public const string EventRenderingEnded = "RenderingEnded";
		public const string EventDestructionStarted = "DestructionStarted";
		public const string EventDragDropItemEnters = "DragDropItemEnters";
		public const string EventDragDropItemLeaves = "DragDropItemLeaves";
		public const string EventDragDropItemDropped = "DragDropItemDropped";
		public const string EventWindowRendererAttached = "WindowRendererAttached";
		public const string EventWindowRendererDetached = "WindowRendererDetached";
		public const string EventTextParsingChanged = "TextParsingChanged";
		public const string EventMarginChanged = "MarginChanged";
		public const string EventCursorEntersArea = "CursorEntersArea";
		public const string EventCursorLeavesArea = "CursorLeavesArea";
		public const string EventCursorEntersSurface = "CursorEntersSurface";
		public const string EventCursorLeavesSurface = "CursorLeavesSurface";
		public const string EventCursorMove = "CursorMove";

		public const string EventCursorPressHold = "CursorPressHold";
		public const string EventCursorActivate = "CursorActivate";
		public const string EventCharacterKey = "CharacterKey";
		public const string EventScroll = "Scroll";

		public const string EventSemanticEvent = "SemanticEvent";

		#endregion

		/// <summary>
		/// Event fired as part of the time based update of the window.
		///  Handlers are passed a const UpdateEventArgs reference.
		/// </summary>
		public event GuiEventHandler<EventArgs> Updated
		{
			add { SubscribeEvent(EventUpdated, value); }
			remove { UnsubscribeEvent(EventUpdated, value); }
		}

		/// <summary>
		/// Event fired when the text string for the Window has changed.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose text was changed.
		/// </summary>
		public event GuiEventHandler<EventArgs> TextChanged
		{
			add { SubscribeEvent(EventTextChanged, value); }
			remove { UnsubscribeEvent(EventTextChanged, value); }
		}

		/// <summary>
		/// Event fired when the Font object for the Window has been changed.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose font was changed.
		/// </summary>
		public event EventHandler<WindowEventArgs> FontChanged;

		/// <summary>
		/// Event fired when the Alpha blend value for the Window has changed.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose alpha value was changed.
		/// </summary>
		public event GuiEventHandler<EventArgs> AlphaChanged
		{
			add { SubscribeEvent(EventAlphaChanged, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the client assigned ID for the Window has changed.
		/// Handlers are passed a const WindowEventArgs reference with
		///  WindowEventArgs::window set to the Window whose Id was changed.
		/// </summary>
		public event GuiEventHandler<EventArgs> IdChanged
		{
			add { SubscribeEvent(EventIdChanged, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window has been activated and has input focus.
		/// Handlers are passed a const ActivationEventArgs reference with
		/// WindowEventArgs::window set to the Window that is gaining activation and
		/// ActivationEventArgs::otherWindow set to the Window that is losing
		/// activation (may be 0).
		/// </summary>
		public event GuiEventHandler<EventArgs> Activated
		{
			add { SubscribeEvent(EventActivated, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window has been deactivated, losing input focus.
		/// Handlers are passed a const ActivationEventArgs reference with
		/// WindowEventArgs::window set to the Window that is losing activation and
		/// ActivationEventArgs::otherWindow set to the Window that is gaining
		/// activation (may be 0).
		/// </summary>
		public event GuiEventHandler<EventArgs> Deactivated
		{
			add { SubscribeEvent(EventDeactivated, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window is shown (made visible).
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window that was shown.
		/// </summary>
		public event GuiEventHandler<EventArgs> Shown
		{
			add { SubscribeEvent(EventShown, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window is made hidden.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window that was hidden.
		/// </summary>
		public event GuiEventHandler<EventArgs> Hidden
		{
			add { SubscribeEvent(EventHidden, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window is enabled so interaction is possible.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window that was enabled.
		/// </summary>
		public event GuiEventHandler<EventArgs> Enabled
		{
			add { SubscribeEvent(EventEnabled, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window is disabled and interaction is no longer
		/// possible. Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window that was disabled.
		/// </summary>
		public event GuiEventHandler<EventArgs> Disabled
		{
			add { SubscribeEvent(EventDisabled, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window clipping mode is modified.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose clipping mode was
		/// changed.
		/// </summary>
		public event GuiEventHandler<EventArgs> ClippedByParentChanged
		{
			add { SubscribeEvent(EventClippedByParentChanged, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window destruction mode is modified.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose destruction mode was
		/// changed.
		/// </summary>
		public event GuiEventHandler<EventArgs> DestroyedByParentChanged
		{
			add { SubscribeEvent(EventDestroyedByParentChanged, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window mode controlling inherited alpha is changed.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose alpha inheritence mode
		/// was changed.
		/// </summary>
		public event GuiEventHandler<EventArgs> InheritsAlphaChanged
		{
			add { SubscribeEvent(EventInheritsAlphaChanged, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the always on top setting for the Window is changed.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose always on top setting
		/// was changed.
		/// </summary>
		public event GuiEventHandler<EventArgs> AlwaysOnTopChanged
		{
			add { SubscribeEvent(EventAlwaysOnTopChanged, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window gains capture of mouse inputs.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window that has captured mouse inputs.
		/// </summary>
		public event GuiEventHandler<EventArgs> InputCaptureGained
		{
			add { SubscribeEvent(EventInputCaptureGained, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window loses capture of mouse inputs.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to either:
		/// - the Window that has lost capture of mouse inputs if that event was
		///   caused by the window itself releasing the capture.
		/// - the Window that is @gaining capture of mouse inputs if that is the
		///   cause of the previous window with capture losing that capture.
		/// </summary>
		public event GuiEventHandler<EventArgs> InputCaptureLost
		{
			add { SubscribeEvent(EventInputCaptureGained, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window has been invalidated.
		/// When a window is invalidated its cached rendering geometry is cleared,
		/// the rendering surface that recieves the window's output is invalidated
		/// and the window's target GUIContext is marked as dirty; this causes all
		/// objects involved in the display of the window to be refreshed the next
		/// time that the GUIContext::draw function is called.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window that has been invalidated.
		/// </summary>
		public event GuiEventHandler<EventArgs> Invalidated
		{
			add { SubscribeEvent(EventInvalidated, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when rendering of the Window has started.  In this context
		/// 'rendering' is the population of the GeometryBuffer with geometry for the
		/// window, not the actual rendering of that GeometryBuffer content to the 
		/// display. Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose rendering has started.
		/// </summary>
		public event GuiEventHandler<EventArgs> RenderingStarted
		{
			add { SubscribeEvent(EventRenderingStarted, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when rendering of the Window has ended.  In this context
		/// 'rendering' is the population of the GeometryBuffer with geometry for the
		/// window, not the actual rendering of that GeometryBuffer content to the 
		/// display. Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose rendering has ended.
		/// </summary>
		public event GuiEventHandler<EventArgs> RenderingEnded
		{
			add { SubscribeEvent(EventRenderingEnded, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when destruction of the Window is about to begin.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window that is about to be destroyed.
		/// </summary>
		public event GuiEventHandler<EventArgs> DestructionStarted
		{
			add { SubscribeEvent(EventRenderingEnded, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when a DragContainer is dragged in to the window's area.
		/// Handlers are passed a const DragDropEventArgs reference with
		/// WindowEventArgs::window set to the window over which a DragContainer has
		/// been dragged (the receiving window) and DragDropEventArgs::dragDropItem
		/// set to the DragContainer that was dragged in to the receiving window's
		/// area.
		/// </summary>
		public event GuiEventHandler<EventArgs> DragDropItemEnters
		{
			add { SubscribeEvent(EventDragDropItemEnters, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when a DragContainer is dragged out of the window's area.
		/// Handlers are passed a const DragDropEventArgs reference with
		/// WindowEventArgs::window set to the window over which a DragContainer has
		/// been dragged out of (the receiving window) and
		/// DragDropEventArgs::dragDropItem set to the DragContainer that was dragged
		/// out of the receiving window's area.
		/// </summary>
		public event GuiEventHandler<EventArgs> DragDropItemLeaves
		{
			add { SubscribeEvent(EventDragDropItemLeaves, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when a DragContainer is dropped within the window's area.
		/// Handlers are passed a const DragDropEventArgs reference with
		/// WindowEventArgs::window set to the window over which a DragContainer was
		/// dropped (the receiving window) and DragDropEventArgs::dragDropItem set to
		/// the DragContainer that was dropped within the receiving window's area.
		/// </summary>
		public event GuiEventHandler<EventArgs> DragDropItemDropped
		{
			add { SubscribeEvent(EventDragDropItemDropped, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when a WindowRenderer object is attached to the window.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the window that had the WindowRenderer
		/// attached to it.
		/// </summary>
		public event GuiEventHandler<EventArgs> WindowRendererAttached
		{
			add { SubscribeEvent(EventWindowRendererAttached, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when a WindowRenderer object is detached from the window.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the window that had the WindowRenderer
		/// detached from it.
		/// </summary>
		public event GuiEventHandler<EventArgs> WindowRendererDetached
		{
			add { SubscribeEvent(EventWindowRendererDetached, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window's setting controlling parsing of it's text
		/// string is changed.
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose text parsing setting was
		/// changed.
		/// </summary>
		public event GuiEventHandler<EventArgs> TextParsingChanged
		{
			add { SubscribeEvent(EventTextParsingChanged, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the Window's margin has changed (any of the four margins)
		/// Handlers are passed a const WindowEventArgs reference with
		/// WindowEventArgs::window set to the Window whose margin was
		/// changed.
		/// </summary>
		public event GuiEventHandler<EventArgs> MarginChanged
		{
			add { SubscribeEvent(EventMarginChanged, value); }
			remove { UnsubscribeEvent(EventMarginChanged, value); }
		}

		/// <summary>
		/// Event fired when the mouse cursor has entered the Window's area.
		/// Handlers are passed a const MouseEventArgs reference with all fields
		/// valid.
		/// </summary>
		public event GuiEventHandler<EventArgs> CursorEntersArea
		{
			add { SubscribeEvent(EventCursorEntersArea, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the mouse cursor has left the Window's area.
		/// Handlers are passed a const MouseEventArgs reference with all fields
		/// valid.
		/// </summary>
		public event GuiEventHandler<EventArgs> CursorLeavesArea
		{
			add { SubscribeEvent(EventCursorLeavesArea, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the mouse cursor enters the Window's area.
		/// Handlers are passed a const MouseEventArgs reference with all fields
		/// valid.
		/// <remarks>
		/// This event is fired if - and only if - the mouse cursor is actually
		/// over some part of this Window's surface area, and will not fire for
		/// example if the location of the mouse is over some child window (even
		/// though the mouse is technically also within the area of this Window).
		/// For an alternative version of this event see the 
		/// <seealso cref="CursorEntersArea"/> event.
		/// </remarks>
		/// </summary>
		public event GuiEventHandler<EventArgs> CursorEntersSurface
		{
			add { SubscribeEvent(EventCursorEntersSurface, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the mouse cursor is no longer over the Window's surface
		/// area. Handlers are passed a const MouseEventArgs reference with all fields
		/// valid.
		/// <remarks>
		/// This event will fire whenever the mouse is no longer actually over
		/// some part of this Window's surface area, for example if the mouse is
		/// moved over some child window (even though technically the mouse has not
		/// actually 'left' this Window's area).  For an alternative version of this
		/// event see the <see cref="CursorLeavesArea"/> event.
		/// </remarks>
		/// </summary>
		public event GuiEventHandler<EventArgs> CursorLeavesSurface
		{
			add { SubscribeEvent(EventCursorLeavesSurface, value); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Event fired when the mouse cursor moves within the area of the Window.
		/// Handlers are passed a const CursorInputEventArgs reference with all fields
		/// valid.
		/// </summary>
		public event GuiEventHandler<EventArgs> CursorMove
		{
			add { SubscribeEvent(EventCursorMove, value); }
			remove { throw new NotImplementedException(); }
		}


		/// <summary>
		/// Event fired when there is a scroll event within the Window's area.
		/// Handlers are passed a const CursorInputEventArgs reference with all fields valid.
		/// </summary>
		public event GuiEventHandler<EventArgs> Scroll
		{
			add { SubscribeEvent(EventScroll, value); }
			remove { UnsubscribeEvent(EventScroll, value); }
		}

		/// <summary>
		/// Event fired when a cursor is pressed and held down within the Window.
		/// Handlers are passed a const CursorInputEventArgs reference with all fields valid.
		/// </summary>
		public event GuiEventHandler<EventArgs> CursorPressHold
		{
			add { SubscribeEvent(EventCursorPressHold, value); }
			remove { UnsubscribeEvent(EventCursorPressHold, value); }
		}

		/// <summary>
		/// Event fired when the cursor is activated within the Window.
		/// Handlers are passed a const CursorInputEventArgs reference with all fields
		/// valid.
		/// </summary>
		public event GuiEventHandler<EventArgs> CursorActivate
		{
			add { SubscribeEvent(EventCursorActivate, value); }
			remove { UnsubscribeEvent(EventCursorActivate, value); }
		}

		/// <summary>
		/// Event fired when the Window receives a character key input event.
		/// Handlers are passed a const KeyEventArgs reference with
		/// WindowEventArgs::window set to the Window receiving the character input,
		/// KeyEventArgs::codepoint set to the Unicode UTF32 / UCS-4 value for the
		/// input, and KeyEventArgs::sysKeys set to the combination of ::SystemKey
		/// values active when the character input was received.
		/// </summary>
		public event GuiEventHandler<EventArgs> CharacterKey
		{
			add { SubscribeEvent(EventCharacterKey, value); }
			remove { UnsubscribeEvent(EventCharacterKey, value); }
		}

		/// <summary>
		/// Event fired when the Window receives a semantic input event.
		/// Handler are passed a const SemanticEventArgs reference with the details
		///  of what semantic event was received
		/// </summary>
		public event GuiEventHandler<EventArgs> SemanticEvent
		{
			add { SubscribeEvent(EventSemanticEvent, value); }
			remove { UnsubscribeEvent(EventSemanticEvent, value); }
		}

		#endregion

		#region Child Widget name suffix constants

		/// <summary>
		/// Widget name suffix for automatically created tooltip widgets.
		/// </summary>
		public const string TooltipNameSuffix = "__auto_tooltip__";

		// XML element and attribute names that relate to Window.
		public const string WindowXMLElementName = "Window";

		public const string AutoWindowXMLElementName = "AutoWindow";
		public const string UserStringXMLElementName = "UserString";
		public const string WindowTypeXMLAttributeName = "type";
		public const string WindowNameXMLAttributeName = "name";
		public const string AutoWindowNamePathXMLAttributeName = "namePath";
		public const string UserStringNameXMLAttributeName = "name";
		public const string UserStringValueXMLAttributeName = "value";

		#endregion

		/// <summary>
		/// Constructor for Window base class 
		/// </summary>
		/// <param name="type">
		/// String object holding Window type (usually provided by WindowFactory).
		/// </param>
		/// <param name="name">
		/// String object holding unique name for the Window.
		/// </param>
		protected Window(string type, string name)
			: base(name)
		{
			// basic types and initial window name
			d_type = type;
			d_autoWindow = false;

			// basic state
			d_initialising = false;
			d_destructionStarted = false;
			d_enabled = true;
			d_visible = true;
			d_active = false;

			// parent related fields
			d_destroyedByParent = true;

			// clipping options
			d_clippedByParent = true;

			// rendering components and options
			d_windowRenderer = null;
			d_surface = null;
			d_needsRedraw = true;
			d_autoRenderingWindow = false;
			d_mouseCursor = null;

			// alpha transparency set up
			d_alpha = 1.0f;
			d_inheritsAlpha = true;

			// mouse input capture set up
			d_oldCapture = null;
			d_restoreOldCapture = false;
			d_distCapturedInputs = false;

			// text system set up
			d_font = null;


#if CEGUI_BIDI_SUPPORT
			d_bidiVisualMapping = new NBidiVisualMapping();
#else
            d_bidiVisualMapping = null;
#endif

			d_bidiDataValid = false;
			d_renderedStringValid = false;
			d_customStringParser = null;
			d_textParsingEnabled = true;

			// margin
			d_margin = new UBox(new UDim(0f, 0f));

			// user specific data
			d_Id = 0;
			d_userData = null;

			// z-order related options
			d_alwaysOnTop = false;
			d_riseOnClick = true;
			d_zOrderingEnabled = true;

			// mouse input options
			d_wantsMultiClicks = true;
			_cursorPassThroughEnabled = false;
			d_autoRepeat = false;
			d_repeatDelay = 0.3f;
			d_repeatRate = 0.06f;
			d_repeatPointerSource = CursorInputSource.None;
			d_repeating = false;
			d_repeatElapsed = 0.0f;

			// drag and drop
			d_dragDropTarget = true;

			// tool tip related
			d_customTip = null;
			d_weOwnTip = false;
			d_inheritsTipText = true;

			// XML writing options
			d_allowWriteXML = true;

			// initialise area cache rects
			d_outerRectClipper = Rectf.Zero;
			d_innerRectClipper = Rectf.Zero;
			d_hitTestRect = Rectf.Zero;

			// cached pixel rect validity flags
			d_outerRectClipperValid = false;
			d_innerRectClipperValid = false;
			d_hitTestRectValid = false;

			// Initial update mode
			d_updateMode = WindowUpdateMode.WUM_VISIBLE;

			// Don't propagate mouse inputs by default.
			d_propagatePointerInputs = false;

			d_guiContext = null;

			d_containsMouse = false;

			// TODO: HandleFontRenderSizeChange
			//d_fontRenderSizeChangeConnection(
			//    GlobalEventSet::getSingleton().subscribeEvent(
			//        "Font/RenderSizeChanged",
			//        Event::Subscriber(&Window::handleFontRenderSizeChange, this)))

			if (GetFont() != null)
				GetFont().RenderSizeChanged += HandleFontRenderSizeChange;

			AddWindowProperties();
		}

		protected override void Dispose(bool disposing)
		{
			if (GetFont() != null)
				GetFont().RenderSizeChanged -= HandleFontRenderSizeChange;

			base.Dispose(disposing);

			// most cleanup actually happened earlier in Window::destroy.
			DestroyGeometryBuffers();
			d_bidiVisualMapping = null;
		}

		/// <summary>
		/// return a String object holding the type name for this Window.
		/// </summary>
		/// <returns>
		/// String object holding the Window type.
		/// </returns>
		public string GetWidgetType()
		{
			return String.IsNullOrEmpty(d_falagardType) ? d_type : d_falagardType;
		}

		/// <summary>
		/// returns whether or not this Window is set to be destroyed when its
		/// parent window is destroyed.
		/// </summary>
		/// <returns>
		/// - true if the Window will be destroyed when its parent is destroyed.
		/// - false if the Window will remain when its parent is destroyed.
		/// </returns>
		public bool IsDestroyedByParent()
		{
			return d_destroyedByParent;
		}

		/// <summary>
		/// returns whether or not this Window is an always on top Window.
		/// Also known as a top-most window.
		/// </summary>
		/// <returns>
		/// - true if this Window is always drawn on top of other normal windows.
		/// - false if the Window has normal z-order behaviour.
		/// </returns>
		public bool IsAlwaysOnTop()
		{
			return d_alwaysOnTop;
		}

		/// <summary>
		/// return whether the Window is currently disabled
		/// </summary>
		/// <remarks>
		/// Only checks the state set for this window, and does not
		/// factor in inherited state from ancestor windows.
		/// </remarks>
		/// <returns>
		/// - true if the window is disabled.
		/// - false if the window is enabled.
		/// </returns>
		public bool IsDisabled()
		{
			ThrowIfDisposed();

			return !d_enabled;
		}

		/// <summary>
		/// return whether the Window is currently disabled
		/// </summary>
		/// - true if the window is disabled.
		/// - false if the window is enabled.
		/// <returns>
		/// </returns>
		/// <remarks>
		/// Not only checks the state set for this window, but also
		/// factors in inherited state from ancestor windows.
		/// </remarks>
		public bool IsEffectiveDisabled()
		{
			var parentDisabled = d_parent != null && GetParent().IsEffectiveDisabled();

			return !d_enabled || parentDisabled;
		}

		/*!
		\brief
		    return true if the Window is currently visible.

		    When true is returned from this function does not mean that the window
		    is not completely obscured by other windows, just that the window will
		    be processed when rendering, and is not explicitly marked as hidden.

		\note
		    Only checks the state set for this window, and does not
		    factor in inherited state from ancestor windows.

		\return
		    - true if the window is set as visible.
		    - false if the window is set as hidden.
		*/

		public bool IsVisible()
		{
			return d_visible;
		}

		/*!
		\brief
		    return true if the Window is currently visible.

		    When true is returned from this function does not mean that the window
		    is not completely obscured by other windows, just that the window will
		    be processed when rendering, and is not explicitly marked as hidden.

		\note
		    Does check the state set for this window, but also
		    factors in inherited state from ancestor windows.

		\return
		    - true if the window will be drawn.
		    - false if the window is hidden and therefore ignored when rendering.
		*/

		public bool IsEffectiveVisible()
		{
			var parentVisible = d_parent == null || GetParent().IsEffectiveVisible();

			return d_visible && parentVisible;
		}

		/// <summary>
		/// return true if this is the active Window.  An active window is a window
		/// that may receive user inputs.
		/// 
		/// Mouse events are always sent to the window containing the mouse cursor
		/// regardless of what this function reports (unless a window has captured
		/// inputs).  The active state mainly determines where send other, for
		/// example keyboard, inputs.
		/// </summary>
		/// <returns>
		/// - true if the window is active and may be sent inputs by the system.
		/// - false if the window is inactive and will not be sent inputs.
		/// </returns>
		public bool IsActive()
		{
			var parentActive = d_parent == null || GetParent().IsActive();

			return d_active && parentActive;
		}

		/// <summary>
		/// Set whether the Window is active or inactive.
		/// </summary>
		/// <param name="setting">
		/// - true to make the Window active.
		/// - false to make the Window inactive (deactivate).
		/// </param>
		/// <remarks>
		/// Activating the window will call to move the window to the front if the 
		/// window is not already active.  The window must be already visible in 
		/// order to activate it otherwise it will have no effect.  
		/// When deactivating, all active children will also be deactivated.
		/// </remarks>
		public void SetActive(bool setting)
		{
			if (IsActive() == setting)
				return;

			if (setting)
				Activate();
			else
				Deactivate();
		}

		/// <summary>
		/// return true if this Window is clipped so that its rendering will not
		/// pass outside of its parent Window area.
		/// </summary>
		/// <returns>
		/// - true if the window will be clipped by its parent Window.
		/// - false if the windows rendering may pass outside its parents area
		/// </returns>
		public bool IsClippedByParent()
		{
			return d_clippedByParent;
		}

		/*!
		\brief
		    

		\return
		    
		*/

		/// <summary>
		/// return the ID code currently assigned to this Window by client code.
		/// </summary>
		/// <returns>
		/// uint value equal to the currently assigned ID code for this Window.
		/// </returns>
		public int GetId()
		{
			return d_Id;
		}

		/// <summary>
		/// returns whether at least one window with the given ID code is attached
		/// to this Window as a child.
		/// </summary>
		/// <param name="id">
		/// uint ID code to look for.
		/// </param>
		/// <returns>
		/// - true if at least one child window was found with the ID code \a ID
		/// - false if no child window was found with the ID code \a ID.
		/// </returns>
		/// <remarks>
		/// ID codes are client assigned and may or may not be unique, and as such,
		/// the return from this function will only have meaning to the client code.
		/// </remarks>
		public bool IsChild(int id)
		{
			var childCount = GetChildCount();

			for (var i = 0; i < childCount; ++i)
				if (GetChildAtIdx(i).GetId() == id)
					return true;

			return false;
		}

		/*!
		\brief
		    returns whether at least one window with the given ID code is attached
		    to this Window or any of it's children as a child.

		\note
		    ID codes are client assigned and may or may not be unique, and as such,
		    the return from this function will only have meaning to the client code.

		    WARNING! This function can be very expensive and should only be used
		    when you have no other option available. If you decide to use it anyway,
		    make sure the window hierarchy from the entry point is small.

		\param ID
		    uint ID code to look for.

		\return
		    - true if at least one child window was found with the ID code \a ID
		    - false if no child window was found with the ID code \a ID.
		*/

		public bool IsChildRecursive(uint id)
		{
			var childCount = GetChildCount();

			for (var i = 0; i < childCount; ++i)
				if (GetChildAtIdx(i).GetId() == id || GetChildAtIdx(i).IsChildRecursive(id))
					return true;

			return false;
		}

		/// <summary>
		/// returns a pointer to the child window at the specified index. Idx is the
		/// index of the window in the child window list. It is based on the order
		/// in which the children were added and is stable.
		/// </summary>
		/// <param name="idx">
		/// Index of the child window list position of the window that should be
		/// returned.This value is not bounds checked, client code should ensure that
		/// this is less than the value returned by getChildCount().
		/// </param>
		/// <returns>
		/// Pointer to the child window currently attached at index position \a idx
		/// </returns>
		public Window GetChildAtIdx(int idx)
		{
			return (Window) (GetChildElementAtIdx(idx));
		}

		/// <summary>
		/// return the attached child window that the given name path references.
		/// 
		/// A name path is a string that describes a path down the window
		/// hierarchy using window names and the forward slash '/' as a separator.
		/// <para>
		/// For example, if this window has a child attached to it named "Panel"
		/// which has its own children attached named "Okay" and "Cancel",
		/// you can access the window "Okay" from this window by using the
		/// name path "Panel/Okay".  To access "Panel", you would simply pass the
		/// name "Panel". 
		/// </para>
		/// </summary>
		/// <param name="namePath">
		/// String object holding the name path of the child window to return.
		/// </param>
		/// <returns>
		/// the Window object referenced by \a name_path.
		/// </returns>
		/// <exception cref="UnknownObjectException">
		/// thrown if \a name_path does not reference a Window attached to this Window.
		/// </exception>
		public Window GetChild(string namePath)
		{
			return (Window) GetChildElement(namePath);
		}

		/*!
		\brief
		    return a pointer to the first attached child window with the specified
		    name. Children are traversed recursively.

		    Contrary to the non recursive version of this function, this one will
		    not throw an exception, but return 0 in case no child was found.

		\note
		    WARNING! This function can be very expensive and should only be used
		    when you have no other option available. If you decide to use it anyway,
		    make sure the window hierarchy from the entry point is small.

		\param name
		    String object holding the name of the window to return a pointer to.

		\return
		    Pointer to the (first) Window object attached to this window that has
		    the name \a name.
		    If no child is found with the name \a name, 0 is returned.
		*/

		public Window GetChildRecursive(string name)
		{
			return (Window) GetChildElementRecursive(name);
		}

		/*!
		\brief
		    return a pointer to the first attached child window with the specified
		    ID value.

		    This function will throw an exception if no child object with the given
		    ID is attached.  This decision was made (over returning NULL if no
		    window was found) so that client code can assume that if the call
		    returns it has a valid window pointer.  We provide the isChild()
		    functions for checking if a given window is attached.

		\param ID
		    uint value specifying the ID code of the window to return a pointer to.

		\return
		    Pointer to the (first) Window object attached to this window that has
		    the ID code \a ID.

		\exception UnknownObjectException
		    thrown if no window with the ID code \a ID is attached to this Window.
		*/

		public Window GetChild(int id)
		{
			ThrowIfDisposed();

			var childCount = GetChildCount();

			for (var i = 0; i < childCount; ++i)
				if (GetChildAtIdx(i).GetId() == id)
					return GetChildAtIdx(i);

			throw new UnknownObjectException("A Window with ID: '" + id.ToString("X") + "' is not attached to Window '" +
			                                 d_name + "'.");
		}

		/*!
		\brief
		    return a pointer to the first attached child window with the specified
		    ID value. Children are traversed recursively.

		    Contrary to the non recursive version of this function, this one will
		    not throw an exception, but return 0 in case no child was found.

		\note
		    WARNING! This function can be very expensive and should only be used
		    when you have no other option available. If you decide to use it anyway,
		    make sure the window hierarchy from the entry point is small.

		\param ID
		    uint value specifying the ID code of the window to return a pointer to.

		\return
		    Pointer to the (first) Window object attached to this window that has
		    the ID code \a ID.
		    If no child is found with the ID code \a ID, 0 is returned.
		*/
		public Window GetChildRecursive(uint id)
		{
			var childCount = GetChildCount();
			var elementsToSearch = new Queue<Element>();

			for (var i = 0; i < childCount; ++i) // load all children into the queue
			{
				var child = GetChildElementAtIdx(i);
				elementsToSearch.Enqueue(child);
			}

			while (elementsToSearch.Count != 0) // breadth-first search for the child to find
			{
				var child = elementsToSearch.Dequeue();

				var window = child as Window;
				if (window != null)
				{
					if (window.GetId() == id)
					{
						return window;
					}
				}

				var elementChildCount = child.GetChildCount();
				for (var i = 0; i < elementChildCount; ++i)
				{
					elementsToSearch.Enqueue(child.GetChildElementAtIdx(i));
				}
			}

			return null;
		}

		/*!
		\brief
		    return a pointer to the Window that currently has input focus starting
		    with this Window.

		\return
		    Pointer to the window that is active (has input focus) starting at this
		    window.  The function will return 'this' if this Window is active
		    and either no children are attached or if none of the attached children
		    are active.  Returns NULL if this Window (and therefore all children)
		    are not active.
		*/

		public Window GetActiveChild()
		{
			// are children can't be active if we are not
			if (!IsActive())
				return null;

			foreach (var wnd in d_drawList)
			{
				// don't need full backward scan for activeness as we already know
				// 'this' is active.  NB: This uses the draw-ordered child list, as that
				// should be quicker in most cases.

				if (wnd.d_active)
					return wnd.GetActiveChild();
			}

			// no child was active, therefore we are the topmost active window
			return this;
		}

		/*!
		\brief
		    return true if any Window with the given ID is some ancestor of this
		    Window.

		\param ID
		    uint value specifying the ID to look for.

		\return
		    - true if an ancestor (parent, or parent of parent, etc) was found with
		      the ID code \a ID.
		    - false if no ancestor window has the ID code \a ID.
		*/

		public bool IsAncestor(uint id)
		{
			// return false if we have no ancestor
			if (d_parent == null)
				return false;

			// check our immediate parent
			if (GetParent().GetId() == id)
				return true;

			// not our parent, check back up the family line
			return GetParent().IsAncestor(id);
		}

		/*!
		\brief
		    return the active Font object for the Window.

		\param useDefault
		    Specifies whether to return the default font if this Window has no
		    preferred font set.

		\return
		    Pointer to the Font being used by this Window.  If the window has no
		    assigned font, and \a useDefault is true, then the default system font
		    is returned.
		*/

		public Font GetFont(bool useDefault = true)
		{
			if (d_font == null)
				return useDefault ? GetGUIContext().GetDefaultFont() : null;

			return d_font;
		}

		/// <summary>
		/// return the current text for the Window
		/// </summary>
		/// <returns>
		/// The String object that holds the current text for this Window.
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

		/// <summary>
		/// return true if the Window inherits alpha from its parent(s).
		/// </summary>
		/// <returns>
		/// - true if the Window inherits alpha from its parent(s)
		/// - false if the alpha for this Window is independant from its parents.
		/// </returns>
		public bool InheritsAlpha()
		{
			return d_inheritsAlpha;
		}

		/// <summary>
		/// return the current alpha value set for this Window
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// The alpha value set for any given window may or may not be the final
		/// alpha value that is used when rendering.  All window objects, by
		/// default, inherit alpha from thier parent window(s) - this will blend
		/// child windows, relatively, down the line of inheritance.  This behaviour
		/// can be overridden via the setInheritsAlpha() method.  To return the true
		/// alpha value that will be applied when rendering, use the
		/// getEffectiveAlpha() method.
		/// <returns>
		/// the currently set alpha value for this Window.  The value returned Will
		/// be between 0.0f and 1.0f.
		/// </returns>
		public float GetAlpha()
		{
			return d_alpha;
		}

		/// <summary>
		/// return the effective alpha value that will be used when rendering this
		/// window, taking into account inheritance of parent window(s) alpha.
		/// </summary>
		/// <returns>
		/// the effective alpha that will be applied to this Window when rendering.
		/// The value returned Will be between 0.0f and 1.0f.
		/// </returns>
		public float GetEffectiveAlpha()
		{
			if (d_parent == null || !InheritsAlpha())
				return d_alpha;

			return d_alpha * GetParent().GetEffectiveAlpha();
		}

		/// <summary>
		/// Return a Rect that describes the rendering clipping rect based upon the
		/// outer rect area of the window.
		/// </summary>
		/// <returns>
		/// The area returned by this function gives you the correct clipping rect
		/// for rendering within the Window's outer rect area.  The area described
		/// may or may not correspond to the final visual clipping actually seen on
		/// the display; this is intentional and neccessary due to the way that
		/// imagery is cached under some configurations.
		/// </returns>
		public Rectf GetOuterRectClipper()
		{
			if (!d_outerRectClipperValid)
			{
				d_outerRectClipper = GetOuterRectClipperImpl();
				d_outerRectClipperValid = true;
			}

			return d_outerRectClipper;
		}

		/// <summary>
		/// Return a Rect that describes the rendering clipping rect based upon the
		/// inner rect area of the window.
		/// </summary>
		/// <remarks>
		/// The area returned by this function gives you the correct clipping rect
		/// for rendering within the Window's inner rect area.  The area described
		/// may or may not correspond to the final visual clipping actually seen on
		/// the display; this is intentional and neccessary due to the way that
		/// imagery is cached under some configurations.
		/// </remarks>
		/// <returns></returns>
		public Rectf GetInnerRectClipper()
		{
			if (!d_innerRectClipperValid)
			{
				d_innerRectClipper = GetInnerRectClipperImpl();
				d_innerRectClipperValid = true;
			}

			return d_innerRectClipper;
		}

		/// <summary>
		/// Return a Rect that describes the rendering clipping rect for the Window.
		/// 
		/// This function can return the clipping rect for either the inner or outer
		/// area dependant upon the boolean values passed in.
		/// </summary>
		/// <remarks>
		/// The areas returned by this function gives you the correct clipping rects
		/// for rendering within the Window's areas.  The area described may or may
		/// not correspond to the final visual clipping actually seen on the
		/// display; this is intentional and neccessary due to the way that imagery
		/// is cached under some configurations.
		/// </remarks>
		/// <param name="nonClient">
		/// - true to return the non-client clipping area (based on outer rect).
		/// - false to return the client clipping area (based on inner rect).
		/// </param>
		/// <returns></returns>
		public Rectf GetClipRect(bool nonClient = false)
		{
			return nonClient ? GetOuterRectClipper() : GetInnerRectClipper();
		}

		/// <summary>
		/// Return the Rect that descibes the clipped screen area that is used for
		/// determining whether this window has been hit by a certain point.
		/// 
		/// The area returned by this function may also be useful for certain
		/// calculations that require the clipped Window area as seen on the display
		/// as opposed to what is used for rendering (since the actual rendering
		/// clipper rects should not to be used if reliable results are desired).
		/// </summary>
		/// <returns></returns>
		public Rectf GetHitTestRect()
		{
			if (!d_hitTestRectValid)
			{
				d_hitTestRect = GetHitTestRectImpl();
				d_hitTestRectValid = true;
			}

			return d_hitTestRect;
		}

		/// <summary>
		/// return the Window that currently has inputs captured.
		/// </summary>
		/// <returns>
		/// Pointer to the Window object that currently has inputs captured, or NULL
		/// if no Window has captured input.
		/// </returns>
		public Window GetCaptureWindow()
		{
			return GetGUIContext().GetInputCaptureWindow();
		}

		/// <summary>
		/// return true if this Window has input captured.
		/// </summary>
		/// <returns>
		/// - true if this Window has captured inputs.
		/// - false if some other Window, or no Window, has captured inputs.
		/// </returns>
		public bool IsCapturedByThis()
		{
			return GetCaptureWindow() == this;
		}

		/// <summary>
		/// return true if an ancestor window has captured inputs.
		/// </summary>
		/// <returns>
		/// - true if input is captured by a Window that is some ancestor (parent,
		///   parent of parent, etc) of this Window.
		/// - false if no ancestor of this window has captured input.
		/// </returns>
		public bool IsCapturedByAncestor()
		{
			return IsAncestor(GetCaptureWindow());
		}

		/// <summary>
		/// return true if a child window has captured inputs.
		/// </summary>
		/// <returns>
		/// - true if input is captured by a Window that is a child of this Window.
		/// - false if no child of this window has not captured input.
		/// </returns>
		public bool IsCapturedByChild()
		{
			return IsChild(GetCaptureWindow());
		}

		/// <summary>
		/// check if the given pixel position would hit this window.
		/// </summary>
		/// <param name="position">
		/// Vector2 object describing the position to check.  The position
		/// describes a pixel offset from the top-left corner of the display.
		/// </param>
		/// <param name="allowDisabled">
		/// - true specifies that the window may be 'hit' if it is disabled.
		/// - false specifies that the window may only be hit if it is enabled.
		/// </param>
		/// <returns>
		/// - true if \a position hits this Window.
		/// - false if \a position does not hit this window.
		/// </returns>
		public virtual bool IsHit(Lunatics.Mathematics.Vector2 position, bool allowDisabled = false)
		{
			// cannot be hit if we are disabled.
			if (!allowDisabled && IsEffectiveDisabled())
				return false;

			var testArea = GetHitTestRect();

			if ((Math.Abs(testArea.Width - 0.0f) < float.Epsilon) || (Math.Abs(testArea.Height - 0.0f) < float.Epsilon))
				return false;

			return testArea.IsPointInRect(position);
		}

		/// <summary>
		/// return the child Window that is hit by the given pixel position
		/// </summary>
		/// <param name="position">
		/// Vector2 object describing the position to check. The position
		/// describes a pixel offset from the top-left corner of the display.</param>
		/// <returns>
		/// Pointer to the child Window that was hit according to the location
		/// \a position, or 0 if no child of this window was hit.
		/// </returns>
		public Window GetChildAtPosition(Lunatics.Mathematics.Vector2 position)
		{
			return GetChildAtPosition(position, (w, f, arg3) => w.IsHit(f, arg3));
		}

		/// <summary>
		/// return the child Window that is 'hit' by the given position, and is
		/// allowed to handle mouse events.
		/// </summary>
		/// <param name="position">
		/// Vector2 object describing the position to check. The position
		/// describes a pixel offset from the top-left corner of the display.
		/// </param>
		/// <param name="allowDisabled">
		/// - true specifies that a disabled window may be returned as the target.
		/// - false specifies that only enabled windows may be returned.
		/// </param>
		/// <returns>
		/// Pointer to the child Window that was hit according to the location
		/// \a position, or 0 if no child of this window was hit.
		/// </returns>
		public Window GetTargetChildAtPosition(Lunatics.Mathematics.Vector2 position, bool allowDisabled = false)
		{
			return GetChildAtPosition(position, (w, p, a) => w.IsHitTargetWindow(p, a), allowDisabled);
		}

		/// <summary>
		/// return the parent of this Window.
		/// </summary>
		/// <returns>
		/// Pointer to the Window object that is the parent of this Window.
		/// This value can be NULL, in which case the Window is a GUI sheet / root.
		/// </returns>
		public Window GetParent()
		{
			return (Window) GetParentElement();
		}

		/// <summary>
		/// Return a pointer to the mouse cursor image to use when the mouse cursor
		/// is within this window's area.
		/// </summary>
		/// <param name="useDefault">
		/// Sepcifies whether to return the default mouse cursor image if this
		/// window specifies no preferred mouse cursor image.
		/// </param>
		/// <returns>
		/// Pointer to the mouse cursor image that will be used when the mouse
		/// enters this window's area.  May return NULL indicating no cursor will
		/// be drawn for this window.
		/// </returns>
		public Image GetCursor(bool useDefault = true)
		{
			if (d_mouseCursor != null)
				return d_mouseCursor;

			return useDefault ? GetGUIContext().GetCursor().GetDefaultImage() : null;
		}

		/*!
		\brief
		    Return the user data set for this Window.

		    Each Window can have some client assigned data attached to it, this data
		    is not used by the GUI system in any way.  Interpretation of the data is
		    entirely application specific.

		\return
		    pointer to the user data that is currently set for this window.
		*/

		public object GetUserData()
		{
			return d_userData;
		}

		/// <summary>
		/// Return whether this window is set to restore old input capture when it
		/// loses input capture.
		/// 
		/// This is only really useful for certain sub-components for widget
		/// writers.
		/// </summary>
		/// <returns>
		/// - true if the window will restore the previous capture window when it
		///   loses input capture.
		/// - false if the window will set the capture window to NULL when it loses
		///   input capture (this is the default behaviour).
		/// </returns>
		public bool RestoresOldCapture()
		{
			return d_restoreOldCapture;
		}

		/// <summary>
		/// Return whether z-order changes are enabled or disabled for this Window.
		/// </summary>
		/// <remarks>
		/// This is distinguished from the is/setRiseOnClickEnabled setting in that
		/// if rise on click is disabled it only affects the users ability to affect
		/// the z order of the Window by clicking the mouse; is still possible to
		/// programatically alter the Window z-order by calling the moveToFront,
		/// moveToBack, moveInFront and moveBehind member functions.  Whereas if z
		/// ordering is disabled those functions are also precluded from affecting
		/// the Window z position.
		/// </remarks>
		/// <returns>
		/// - true if z-order changes are enabled for this window.
		///   moveToFront, moveToBack, moveInFront and moveBehind work normally.
		/// - false: z-order changes are disabled for this window.
		/// moveToFront, moveToBack, moveInFront and moveBehind are ignored.
		/// </returns>
		public bool IsZOrderingEnabled()
		{
			return d_zOrderingEnabled;
		}

		/// <summary>
		/// Return whether this window will receive multi-click events or multiple
		/// 'down' events instead.
		/// </summary>
		/// <returns>
		/// - true if the Window will receive double-click and triple-click events.
		/// - false if the Window will receive multiple mouse button down events
		///   instead of double/triple click events.
		/// </returns>
		public bool WantsMultiClickEvents()
		{
			return d_wantsMultiClicks;
		}

		/// <summary>
		/// Return whether mouse button down event autorepeat is enabled for this
		/// window.
		/// </summary>
		/// <returns>
		/// - true if autorepeat of mouse button down events is enabled for this
		///   window.
		/// - false if autorepeat of mouse button down events is not enabled for
		///   this window.
		/// </returns>
		public bool IsMouseAutoRepeatEnabled()
		{
			return d_autoRepeat;
		}

		/// <summary>
		/// Return the current auto-repeat delay setting for this window.
		/// </summary>
		/// <returns>
		/// float value indicating the delay, in seconds, defore the first repeat
		/// mouse button down event will be triggered when autorepeat is enabled.
		/// </returns>
		public float GetAutoRepeatDelay()
		{
			return d_repeatDelay;
		}

		/// <summary>
		/// Return the current auto-repeat rate setting for this window.
		/// </summary>
		/// <returns>
		/// float value indicating the rate, in seconds, at which repeat mouse
		/// button down events will be generated after the initial delay has
		/// expired.
		/// </returns>
		public float GetAutoRepeatRate()
		{
			return d_repeatRate;
		}

		/// <summary>
		/// Return whether the window wants inputs passed to its attached
		/// child windows when the window has inputs captured.
		/// </summary>
		/// <returns>
		/// - true if System should pass captured input events to child windows.
		/// - false if System should pass captured input events to this window only.
		/// </returns>
		public bool DistributesCapturedInputs()
		{
			return d_distCapturedInputs;
		}

		/// <summary>
		/// Return whether this Window is using the system default Tooltip for its
		/// Tooltip window.
		/// </summary>
		/// <returns>
		/// - true if the Window will use the system default tooltip.
		/// - false if the window has a custom Tooltip object.
		/// </returns>
		public bool IsUsingDefaultTooltip()
		{
			return d_customTip == null;
		}

		/// <summary>
		/// Return a pointer to the Tooltip object used by this Window.  The value
		/// returned may point to the system default Tooltip, a custom Window
		/// specific Tooltip, or be NULL.
		/// </summary>
		/// <returns>
		/// Pointer to a Tooltip based object, or NULL.
		/// </returns>
		public Tooltip GetTooltip()
		{
			return IsUsingDefaultTooltip() ? GetGUIContext().GetDefaultTooltipObject() : d_customTip;
		}

		/// <summary>
		/// Return the custom tooltip type.
		/// </summary>
		/// <returns>
		/// String object holding the current custom tooltip window type, or an
		/// empty string if no custom tooltip is set.
		/// </returns>
		public string GetTooltipType()
		{
			return IsUsingDefaultTooltip() ? String.Empty : d_customTip.GetWidgetType();
		}

		/// <summary>
		/// Return the current tooltip text set for this Window.
		/// </summary>
		/// <returns>
		/// String object holding the current tooltip text set for this window.
		/// </returns>
		public string GetTooltipText()
		{
			if (d_inheritsTipText && d_parent != null && String.IsNullOrEmpty(d_tooltipText))
				return GetParent().GetTooltipText();

			return d_tooltipText;
		}

		/// <summary>
		/// Return whether this window inherits Tooltip text from its parent when
		/// its own tooltip text is not set.
		/// </summary>
		/// <returns>
		/// - true if the window inherits tooltip text from its parent when its own
		///   text is not set.
		/// - false if the window does not inherit tooltip text from its parent
		///   (and shows no tooltip when no text is set).
		/// </returns>
		public bool InheritsTooltipText()
		{
			return d_inheritsTipText;
		}

		/// <summary>
		/// Return whether this window will rise to the top of the z-order when
		/// clicked with the left mouse button.
		/// </summary>
		/// <remarks>
		/// This is distinguished from the is/setZOrderingEnabled setting in that
		/// if rise on click is disabled it only affects the users ability to affect
		/// the z order of the Window by clicking the mouse; is still possible to
		/// programatically alter the Window z-order by calling the moveToFront,
		/// moveToBack, moveInFront and moveBehind member functions.  Whereas if z
		/// ordering is disabled those functions are also precluded from affecting
		/// the Window z position.
		/// </remarks>
		/// <returns>
		/// - true if the window will come to the top of other windows when the left
		///   mouse button is pushed within its area.
		/// - false if the window does not change z-order position when the left
		///   mouse button is pushed within its area.
		/// </returns>
		public bool IsRiseOnClickEnabled()
		{
			return d_riseOnClick;
		}

		/// <summary>
		/// Return the list of GeometryBuffer objects for this Window.
		/// </summary>
		/// <returns>
		/// Reference to the list of GeometryBuffer objects for this Window.
		/// </returns>
		public List<GeometryBuffer> GetGeometryBuffers()
		{
			return d_geometryBuffers;
		}

		/// <summary>
		/// Adds GeometryBuffers to the end of the list of GeometryBuffers of this Window.
		/// </summary>
		/// <param name="geomBuffers">
		/// The GeometryBuffers that will be appended to the window's GeometryBuffers
		/// </param>
		public void AppendGeometryBuffers(IEnumerable<GeometryBuffer> geomBuffers)
		{
			d_geometryBuffers.AddRange(geomBuffers);
		}

		/// <summary>
		/// Get the name of the LookNFeel assigned to this window.
		/// </summary>
		/// <returns>
		/// String object holding the name of the look assigned to this window.
		/// Returns the empty string if no look is assigned.
		/// </returns>
		public string GetLookNFeel()
		{
			return d_lookName;
		}

		/// <summary>
		/// Get whether or not this Window is the modal target.
		/// </summary>
		/// <returns>
		/// Returns true if this Window is the modal target, otherwise false.
		/// </returns>
		public bool GetModalState()
		{
			return (GetGUIContext().GetModalWindow() == this);
		}

		/// <summary>
		/// Returns a named user string.
		/// </summary>
		/// <param name="name">
		/// String object holding the name of the string to be returned.
		/// </param>
		/// <returns>
		/// String object holding the data stored for the requested user string.
		/// </returns>
		/// <exception cref="UnknownObjectException">
		/// thrown if a user string named \a name does not exist.
		/// </exception>
		public string GetUserString(string name)
		{
			string userString;
			if (d_userStrings.TryGetValue(name, out userString))
				return userString;

			throw new UnknownObjectException(
				"a user string named '" + name + "' is not defined for Window '" + d_name + "'.");
		}

		/// <summary>
		/// Return whether a user string with the specified name exists.
		/// </summary>
		/// <param name="name">
		/// String object holding the name of the string to be checked.
		/// </param>
		/// <returns>
		/// - true if a user string named \a name exists.
		/// - false if no such user string exists.
		/// </returns>
		public bool IsUserStringDefined(string name)
		{
			return d_userStrings.ContainsKey(name);
		}

		public IEnumerable<KeyValuePair<string, string>> UserStrings
		{
			get { return d_userStrings; }
		}

		/// <summary>
		/// Returns the active sibling window.
		/// 
		/// This searches the immediate children of this window's parent, and
		/// returns a pointer to the active window.  The method will return this if
		/// we are the immediate child of our parent that is active.  If our parent
		/// is not active, or if no immediate child of our parent is active then 0
		/// is returned.  If this window has no parent, and this window is not
		/// active then 0 is returned, else this is returned.
		/// </summary>
		/// <returns>
		/// A pointer to the immediate child window attached to our parent that is
		/// currently active, or 0 if no immediate child of our parent is active.
		/// </returns>
		public Window GetActiveSibling()
		{
			// initialise with this if we are active, else 0
			Window activeWnd = IsActive() ? this : null;

			// if active window not already known, and we have a parent window
			if (activeWnd == null && d_parent != null)
			{
				// scan backwards through the draw list, as this will
				// usually result in the fastest result.
				var idx = d_parent.GetChildCount();
				while (idx-- > 0)
				{
					var wnd = GetParent().d_drawList[idx];
					// if this child is active
					if (wnd.IsActive())
					{
						// set the return value
						activeWnd = wnd;

						// exit loop early, as we have found what we needed
						break;
					}
				}
			}

			// return whatever we discovered
			return activeWnd;
		}

		/// <summary>
		/// Returns whether this window should ignore mouse event and pass them
		/// through to and other windows behind it. In effect making the window
		/// transparent to the mouse.
		/// </summary>
		/// <returns>
		/// true if mouse pass through is enabled.
		/// false if mouse pass through is not enabled.
		/// </returns>
		public bool IsCursorPassThroughEnabled()
		{
			return _cursorPassThroughEnabled;
		}

		/// <summary>
		/// Returns whether this window is an auto window.
		/// 
		/// An auto window is typically a Window object created automatically by
		/// CEGUI - for example to form part of a multi-element 'compound' widget.
		/// </summary>
		/// <returns></returns>
		public bool IsAutoWindow()
		{
			return d_autoWindow;
		}

		/// <summary>
		/// Returns whether this window is allowed to write XML.
		/// </summary>
		/// <returns></returns>
		public bool IsWritingXMLAllowed()
		{
			return d_allowWriteXML;
		}

		/// <summary>
		/// Returns whether this Window object will receive events generated by
		/// the drag and drop support in the system.
		/// </summary>
		/// <returns>
		/// - true if the Window is enabled as a drag and drop target.
		/// - false if the window is not enabled as a drag and drop target.
		/// </returns>
		public bool IsDragDropTarget()
		{
			return d_dragDropTarget;
		}

		/// <summary>
		/// Fill in the RenderingContext \a ctx with details of the RenderingSurface
		/// where this Window object should normally do it's rendering.
		/// </summary>
		/// <param name="ctx"></param>
		public void GetRenderingContext(out RenderingContext ctx)
		{
			if (d_windowRenderer != null)
				d_windowRenderer.GetRenderingContext(out ctx);
			else
				GetRenderingContextImpl(out ctx);
		}

		/// <summary>
		/// implementation of the default getRenderingContext logic.
		/// </summary>
		/// <param name="ctx"></param>
		public virtual void GetRenderingContextImpl(out RenderingContext ctx)
		{
			if (d_surface != null)
			{
				ctx.surface = d_surface;
				ctx.owner = this;
				ctx.offset = GetUnclippedOuterRect().Get().Position;
				ctx.queue = RenderQueueId.RQ_BASE;
			}
			else if (d_parent != null)
			{
				GetParent().GetRenderingContext(out ctx);
			}
			else
			{
				ctx.surface = GetGUIContext();
				ctx.owner = null;
				ctx.offset = Lunatics.Mathematics.Vector2.Zero;
				ctx.queue = RenderQueueId.RQ_BASE;
			}
		}

		/// <summary>
		/// return the RenderingSurface currently set for this window. May return null.
		/// </summary>
		/// <returns></returns>
		public RenderingSurface GetRenderingSurface()
		{
			return d_surface;
		}

		/// <summary>
		/// return the RenderingSurface that will be used by this window as the
		/// target for rendering.
		/// </summary>
		/// <returns></returns>
		public RenderingSurface GetTargetRenderingSurface()
		{
			if (d_surface != null)
				return d_surface;

			return d_parent != null
				       ? GetParent().GetTargetRenderingSurface()
				       : GetGUIContext();
		}

		/// <summary>
		/// Returns whether \e automatic use of an imagery caching RenderingSurface
		/// (i.e. a RenderingWindow) is enabled for this window.  The reason we
		/// emphasise 'automatic' is because the client may manually set a
		/// RenderingSurface that does exactly the same job.
		/// </summary>
		/// <returns>
		/// - true if automatic use of a caching RenderingSurface is enabled.
		/// - false if automatic use of a caching RenderTarget is not enabled.
		/// </returns>
		public bool IsUsingAutoRenderingSurface()
		{
			return d_autoRenderingWindow;
		}

		public bool IsAutoRenderingSurfaceStencilEnabled()
		{
			return d_autoRenderingSurfaceStencilEnabled;
		}
		public void SetAutoRenderingSurfaceStencilEnabled(bool setting)
		{
			if (d_autoRenderingSurfaceStencilEnabled == setting)
				return;

			d_autoRenderingSurfaceStencilEnabled = setting;

			if (!d_autoRenderingWindow)
				return;

			// We need to recreate the auto rendering window since we just changed a crucial setting for it
			ReleaseRenderingWindow();
			AllocateRenderingWindow(setting);

			// while the actual area on screen may not have changed, the arrangement of
			// surfaces and geometry did...
			NotifyScreenAreaChanged();
		}

		/// <summary>
		/// Returns the window at the root of the hierarchy starting at this
		/// Window.  The root window is defined as the first window back up the
		/// hierarchy that has no parent window.
		/// </summary>
		/// <returns>
		/// A pointer to the root window of the hierarchy that this window is
		/// attached to.
		/// </returns>
		public Window GetRootWindow()
		{
			return d_parent != null ? GetParent().GetRootWindow() : this;
		}

		/// <summary>
		/// Initialises the Window based object ready for use.
		/// </summary>
		/// <remarks>
		/// This must be called for every window created.  Normally this is handled
		/// automatically by the WindowManager.
		/// </remarks>
		protected virtual void InitialiseComponents()
		{

		}

		/// <summary>
		/// Set whether or not this Window will automatically be destroyed when its
		/// parent Window is destroyed.
		/// </summary>
		/// <param name="value">
		/// - true to have the Window auto-destroyed when its parent is destroyed
		///   (default behaviour)
		/// - false to have the Window remain after its parent is destroyed.
		/// </param>
		public void SetDestroyedByParent(bool value)
		{
			if (d_destroyedByParent == value)
				return;

			d_destroyedByParent = value;

			OnParentDestroyChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// Set whether this window is always on top, or not.
		/// </summary>
		/// <param name="value">
		/// - true to have the Window appear on top of all other non always on top windows
		/// - false to allow the window to be covered by other normal windows.
		/// </param>
		public void SetAlwaysOnTop(bool value)
		{
			// only react to an actual change
			if (IsAlwaysOnTop() == value)
				return;

			d_alwaysOnTop = value;

			// move us in front of sibling windows with the same 'always-on-top'
			// setting as we have.
			if (d_parent != null)
			{
				var orgParent = GetParent();

				orgParent.RemoveChildImpl(this);
				orgParent.AddChildImpl(this);

				OnZChangeImpl();
			}

			OnAlwaysOnTopChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// Set whether this window is enabled or disabled.  A disabled window
		/// normally can not be interacted with, and may have different rendering.
		/// </summary>
		/// <param name="value">
		/// - true to enable the Window.
		/// - false to disable the Window.
		/// </param>
		public virtual void SetEnabled(bool value)
		{
			// only react if setting has changed
			if (d_enabled == value)
				return;

			d_enabled = value;
			var args = new WindowEventArgs(this);

			if (d_enabled)
			{
				// check to see if the window is actually enabled (which depends
				// upon all ancestor windows being enabled) we do this so that
				// events we fire give an accurate indication of the state of a
				// window.
				if ((d_parent != null && !GetParent().IsDisabled()) || d_parent == null)
					OnEnabled(args);
			}
			else
			{
				OnDisabled(args);
			}

			GetGUIContext().UpdateWindowContainingCursor();
		}

		/// <summary>
		/// Set whether this window is enabled or disabled.  A disabled window
		/// normally can not be interacted with, and may have different rendering.
		/// </summary>
		/// <param name="value">
		/// - true to disable the Window.
		/// - false to enable the Window.
		/// </param>
		public void SetDisabled(bool value)
		{
			SetEnabled(!value);
		}

		/// <summary>
		/// enable the Window to allow interaction.
		/// </summary>
		public void Enable()
		{
			SetEnabled(true);
		}

		/// <summary>
		/// disable the Window to prevent interaction.
		/// </summary>
		public void Disable()
		{
			SetEnabled(false);
		}

		/// <summary>
		/// Set whether the Window is visible or hidden.
		/// </summary>
		/// <param name="value">
		/// - true to make the Window visible.
		/// - false to make the Window hidden.
		/// </param>
		/// <remarks>
		/// Hiding the active window will cause that window to become deactivated.
		/// Showing a window does not, however, automatically cause that window to
		/// become the active window (call Window::activate after making the window
		/// visible to activate it).
		/// </remarks>
		public void SetVisible(bool value)
		{
			// only react if setting has changed
			if (d_visible == value)
				return;

			d_visible = value;
			var args = new WindowEventArgs(this);
			if (d_visible)
				OnShown(args);
			else
				OnHidden(args);

			GetGUIContext().UpdateWindowContainingCursor();
		}

		/// <summary>
		/// show the Window.
		/// </summary>
		/// <remarks>
		/// Showing a window does not automatically activate the window.  If you
		/// want the window to also become active you will need to call the
		/// Window::activate member also.
		/// </remarks>
		public void Show()
		{
			SetVisible(true);
		}

		/// <summary>
		/// hide the Window.
		/// </summary>
		/// <remarks>
		///  If the window is the active window, it will become deactivated as a
		/// result of being hidden.
		/// </remarks>
		public void Hide()
		{
			SetVisible(false);
		}

		/// <summary>
		/// Activate the Window giving it input focus and bringing it to the top of
		/// all windows with the same always-on-top settig as this Window.
		/// </summary>
		public void Activate()
		{
			// exit if the window is not visible, since a hidden window may not be the
			// active window.
			if (!IsEffectiveVisible())
				return;

			// force complete release of input capture.
			// NB: This is not done via releaseCapture() because that has
			// different behaviour depending on the restoreOldCapture setting.
			if (GetCaptureWindow() != null && GetCaptureWindow() != this)
			{
				Window tmpCapture = GetCaptureWindow();
				GetGUIContext().SetInputCaptureWindow(null);

				tmpCapture.OnCaptureLost(new WindowEventArgs(null));
			}

			MoveToFront();
		}

		/// <summary>
		/// Deactivate the window.  No further inputs will be received by the window
		/// until it is re-activated either programmatically or by the user
		/// interacting with the gui.
		/// </summary>
		public void Deactivate()
		{
			if (IsActive())
				OnDeactivated(new ActivationEventArgs(this) {otherWindow = null});
		}

		/// <summary>
		/// Set whether this Window will be clipped by its parent window(s).
		/// </summary>
		/// <param name="value">
		/// - true to have the Window clipped so that rendering is constrained to
		///   within the area of its parent(s).
		/// - false to have rendering constrained to the screen only.
		/// </param>
		public void SetClippedByParent(bool value)
		{
			// only react if setting has changed
			if (d_clippedByParent == value)
				return;

			d_clippedByParent = value;
			OnClippingChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// Set the current ID for the Window.
		/// </summary>
		/// <param name="value">
		/// Client assigned ID code for this Window.  The GUI system assigns no
		/// meaning to any IDs, they are a device purely for client code usage.
		/// </param>
		public void SetId(int value)
		{
			if (d_Id == value)
				return;

			d_Id = value;

			OnIdChanged(new WindowEventArgs(this));
		}


		/// <summary>
		/// Set the current text string for the Window.
		/// </summary>
		/// <param name="value">
		/// String object containing the text that is to be set as the Window text.
		/// </param>
		public void SetText(string value)
		{
			d_textLogical = value;
			d_renderedStringValid = false;
			d_bidiDataValid = false;

			OnTextChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// Insert the text string \a text into the current text string for the
		/// Window object at the position specified by \a position.
		/// </summary>
		/// <param name="text">
		/// String object holding the text that is to be inserted into the Window
		/// object's current text string.
		/// </param>
		/// <param name="position">
		/// The characted index position where the string \a text should be
		/// inserted.
		/// </param>
		public void InsertText(string text, int position)
		{
			d_textLogical = d_textLogical.Insert(position, text);
			d_renderedStringValid = false;
			d_bidiDataValid = false;

			OnTextChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// Append the string \a text to the currect text string for the Window
		/// object.
		/// </summary>
		/// <param name="text">
		/// String object holding the text that is to be appended to the Window
		/// object's current text string.
		/// </param>
		public void AppendText(string text)
		{
			d_textLogical += text;
			d_renderedStringValid = false;
			d_bidiDataValid = false;

			OnTextChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// Set the font used by this Window.
		/// </summary>
		/// <param name="value">
		/// Pointer to the Font object to be used by this Window.
		/// If \a font is NULL, the default font will be used.
		/// </param>
		public void SetFont(Font value)
		{
			if (d_font == value)
				return;

			if (GetFont() != null)
				GetFont().RenderSizeChanged -= HandleFontRenderSizeChange;

			d_font = value;

			if (GetFont() != null)
				GetFont().RenderSizeChanged += HandleFontRenderSizeChange;

			d_renderedStringValid = false;
			OnFontChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// Set the font used by this Window.
		/// </summary>
		/// <param name="name">
		/// String object holding the name of the Font object to be used by this
		/// Window.  If \a name == "", the default font will be used.
		/// </param>
		/// <exception cref="UnknownObjectException">
		/// thrown if the specified Font is unknown within the system.
		/// </exception>
		public void SetFont(string name)
		{
			SetFont(!String.IsNullOrEmpty(name) ? FontManager.GetSingleton().Get(name) : null);
		}

		/// <summary>
		/// Remove the first child Window with the specified ID.  If there is more
		/// than one attached Window objects with the specified ID, only the fist
		/// one encountered will be removed.
		/// </summary>
		/// <param name="id">
		/// ID number assigned to the Window to be removed.  If no Window with ID
		/// code \a ID is attached, nothing happens.
		/// </param>
		public void RemoveChild(uint id)
		{
			var childCount = GetChildCount();

			for (var i = 0; i < childCount; ++i)
			{
				if (GetChildAtIdx(i).GetId() == id)
				{
					RemoveChild(d_children[i]);
					return;
				}

			}
		}

		/// <summary>
		/// Creates a child window attached to this window.
		/// </summary>
		/// <param name="type">
		/// String that describes the type of Window to be created.  A valid
		/// WindowFactory for the specified type must be registered.
		/// </param>
		/// <param name="name">
		/// String that holds the name that is to be given to the new window.  If
		/// this string is empty, a name will be generated for the window.
		/// </param>
		/// <returns>
		/// Pointer to the newly created child Window object.
		/// </returns>
		public Window CreateChild(string type, string name = "")
		{
			var ret = WindowManager.GetSingleton().CreateWindow(type, name);
			AddChild(ret);

			return ret;
		}

		/// <summary>
		/// Destroys a child window of this window
		/// </summary>
		/// <param name="wnd">
		/// The child window to destroy
		/// </param>
		public void DestroyChild(Window wnd)
		{
			Debug.Assert(IsChild(wnd), "Window you are attempting to destroy is not a child!");

			WindowManager.GetSingleton().DestroyWindow(wnd);
		}

		/// <summary>
		/// Destroys a child window of this window
		/// </summary>
		/// <param name="namePath">
		/// Name path that references the window to destroy
		/// </param>
		public void DestroyChild(string namePath)
		{
			DestroyChild(GetChild(namePath));
		}

		/// <summary>
		/// Move the Window to the top of the z order.
		/// 
		/// - If the Window is a non always-on-top window it is moved the the top of
		///   all other non always-on-top sibling windows, and the process repeated
		///   for all ancestors.
		/// - If the Window is an always-on-top window it is moved to the of of all
		///   sibling Windows, and the process repeated for all ancestors.
		/// </summary>
		public void MoveToFront()
		{
			MoveToFrontImpl(false);
		}

		/// <summary>
		/// Move the Window to the bottom of the Z order.
		/// 
		/// - If the window is non always-on-top the Window is sent to the very
		///   bottom of its sibling windows and the process repeated for all
		///   ancestors.
		/// - If the window is always-on-top, the Window is sent to the bottom of
		///   all sibling always-on-top windows and the process repeated for all
		///   ancestors.
		/// </summary>
		public void MoveToBack()
		{
			// if the window is active, de-activate it.
			if (IsActive())
			{
				OnDeactivated(new ActivationEventArgs(this) {otherWindow = null});
			}

			// we only proceed if we have a parent (otherwise we can have no siblings)
			if (d_parent != null)
			{
				if (d_zOrderingEnabled)
				{
					// remove us from our parent's draw list
					GetParent().RemoveWindowFromDrawList(this);

					// re-attach ourselves to our parent's draw list which will move us
					// in behind sibling windows with the same 'always-on-top' setting
					// as we have.
					GetParent().AddWindowToDrawList(this, true);

					// notify relevant windows about the z-order change.
					OnZChangeImpl();
				}

				GetParent().MoveToBack();
			}
		}

		/// <summary>
		/// Move this window immediately above it's sibling \a window in the z order.
		/// 
		/// No action will be taken under the following conditions:
		/// - \a window is 0.
		/// - \a window is not a sibling of this window.
		/// - \a window and this window have different AlwaysOnTop settings.
		/// - z ordering is disabled for this window.
		/// </summary>
		/// <param name="window">
		/// The sibling window that this window will be moved in front of.
		/// </param>
		public void MoveInFront(Window window)
		{
			if (window == null || window.d_parent == null || window.d_parent != d_parent ||
			    window == this || window.d_alwaysOnTop != d_alwaysOnTop ||
			    !d_zOrderingEnabled)
				return;

			// find our position in the parent child draw list
			var p = GetParent().d_drawList.IndexOf(this);
			//const ChildDrawList::iterator p(std::find(getParent()->d_drawList.begin(),
			//                                          getParent()->d_drawList.end(),
			//                                          this));
			// sanity checK that we were attached to our parent.
			Debug.Assert(p != -1);
			//assert(p != getParent()->d_drawList.end());

			// erase us from our current position
			GetParent().d_drawList.RemoveAt(p);

			// find window we're to be moved in front of in parent's draw list
			var i = GetParent().d_drawList.IndexOf(window);
			//ChildDrawList::iterator i(std::find(getParent()->d_drawList.begin(),
			//                                    getParent()->d_drawList.end(),
			//                                    window));
			// sanity check that target window was also attached to correct parent.
			Debug.Assert(i != -1);
			//assert(i != getParent()->d_drawList.end());

			// reinsert ourselves at the right location
			GetParent().d_drawList.Insert(++i, this);

			// handle event notifications for affected windows.
			OnZChangeImpl();
		}

		/// <summary>
		/// Move this window immediately behind it's sibling \a window in the z
		/// order.
		/// 
		/// No action will be taken under the following conditions:
		/// - \a window is 0.
		/// - \a window is not a sibling of this window.
		/// - \a window and this window have different AlwaysOnTop settings.
		/// - z ordering is disabled for this window.
		/// </summary>
		/// <param name="window">
		/// The sibling window that this window will be moved behind.
		/// </param>
		public void MoveBehind(Window window)
		{
			if (window == null || window.d_parent == null || window.d_parent != d_parent ||
			    window == this || window.d_alwaysOnTop != d_alwaysOnTop ||
			    !d_zOrderingEnabled)
				return;

			// find our position in the parent child draw list
			var p = GetParent().d_drawList.IndexOf(this);
			//const ChildDrawList::iterator p(std::find(getParent()->d_drawList.begin(),
			//                                          getParent()->d_drawList.end(),
			//                                          this));
			// sanity checK that we were attached to our parent.
			Debug.Assert(p != -1);
			//assert(p != getParent()->d_drawList.end());

			// erase us from our current position
			GetParent().d_drawList.RemoveAt(p);

			// find window we're to be moved in front of in parent's draw list
			var i = GetParent().d_drawList.IndexOf(window);
			//const ChildDrawList::iterator i(std::find(getParent()->d_drawList.begin(),
			//                                          getParent()->d_drawList.end(),
			//                                          window));
			// sanity check that target window was also attached to correct parent.
			Debug.Assert(i != -1);
			//assert(i != getParent()->d_drawList.end());

			// reinsert ourselves at the right location
			GetParent().d_drawList.Insert(i, this);

			// handle event notifications for affected windows.
			OnZChangeImpl();
		}

		/// <summary>
		/// Return the (visual) z index of the window on it's parent.
		/// 
		/// The z index is a number that indicates the order that windows will be
		/// drawn (but is not a 'z co-ordinate', as such).  Higher numbers are in
		/// front of lower numbers.
		/// 
		/// The number returned will not be stable, and generally should be used to
		/// compare with the z index of sibling windows (and only sibling windows)
		/// to discover the current z ordering of those windows.
		/// </summary>
		/// <returns></returns>
		public int GetZIndex()
		{
			if (d_parent == null)
				return 0;

			if (!GetParent().d_drawList.Contains(this))
				throw new InvalidRequestException("Window is not in its parent's draw list.");

			//return std::distance(getParent()->d_drawList.begin(), i);
			return GetParent().d_drawList.IndexOf(this);
		}

		/// <summary>
		/// Return whether /a this Window is in front of the given window.
		/// </summary>
		/// <remarks>
		/// Here 'in front' just means that one window is drawn after the other, it
		/// is not meant to imply that the windows are overlapping nor that one
		/// window is obscured by the other.
		/// </remarks>
		/// <param name="wnd"></param>
		/// <returns></returns>
		public bool IsInFront(Window wnd)
		{
			// children are always in front of their ancestors
			if (IsAncestor(wnd))
				return true;

			// conversely, ancestors are always behind their children
			if (wnd.IsAncestor(this))
				return false;

			var w1 = GetWindowAttachedToCommonAncestor(wnd);

			// seems not to be in same window hierarchy
			if (w1 == null)
				return false;

			var w2 = wnd.GetWindowAttachedToCommonAncestor(this);

			// at this point, w1 and w2 share the same parent.
			return w2.GetZIndex() > w1.GetZIndex();
		}

		/// <summary>
		/// Return whether /a this Window is behind the given window.
		/// </summary>
		/// <remarks>
		/// Here 'behind' just means that one window is drawn before the other, it
		/// is not meant to imply that the windows are overlapping nor that one
		/// window is obscured by the other.
		/// </remarks>
		/// <param name="wnd"></param>
		/// <returns></returns>
		public bool IsBehind(Window wnd)
		{
			return !IsInFront(wnd);
		}

		/// <summary>
		/// Captures input to this window
		/// </summary>
		/// <returns>
		/// - true if input was successfully captured to this window.
		/// - false if input could not be captured to this window
		///   (maybe because the window is not active).
		/// </returns>
		public bool CaptureInput()
		{
			// we can only capture if we are the active window (LEAVE THIS ALONE!)
			if (!IsActive())
				return false;

			if (!IsCapturedByThis())
			{
				var currentCapture = GetCaptureWindow();
				GetGUIContext().SetInputCaptureWindow(this);
				var args = new WindowEventArgs(this);

				// inform window which previously had capture that it doesn't anymore.
				if (currentCapture != null && currentCapture != this && !d_restoreOldCapture)
					currentCapture.OnCaptureLost(args);

				if (d_restoreOldCapture)
					d_oldCapture = currentCapture;

				OnCaptureGained(args);
			}

			return true;
		}

		/// <summary>
		/// Releases input capture from this Window.  If this Window does not have
		/// inputs captured, nothing happens.
		/// </summary>
		public void ReleaseInput()
		{
			// if we are not the window that has capture, do nothing
			if (!IsCapturedByThis())
				return;

			// restore old captured window if that mode is set
			if (d_restoreOldCapture)
			{
				GetGUIContext().SetInputCaptureWindow(d_oldCapture);

				// check for case when there was no previously captured window
				if (d_oldCapture != null)
				{
					d_oldCapture = null;
					GetCaptureWindow().MoveToFront();
				}

			}
			else
				GetGUIContext().SetInputCaptureWindow(null);

			OnCaptureLost(new WindowEventArgs(this));
		}

		/// <summary>
		/// Set whether this window will remember and restore the previous window
		/// that had inputs captured.
		/// </summary>
		/// <param name="value">
		/// - true: The window will remember and restore the previous capture
		///   window.  The CaptureLost event is not fired on the previous window
		///   when this window steals input capture.  When this window releases
		///   capture, the old capture window is silently restored.
		/// 
		/// - false: Input capture works as normal, each window losing capture is
		///   signalled via CaptureLost, and upon the final release of capture, no
		///   previous setting is restored (this is the default behaviour).
		/// </param>
		public void SetRestoreOldCapture(bool value)
		{
			d_restoreOldCapture = value;

			var childCount = GetChildCount();

			for (var i = 0; i < childCount; ++i)
				GetChildAtIdx(i).SetRestoreOldCapture(value);
		}

		/// <summary>
		/// Set the current alpha value for this window.
		/// </summary>
		/// <remarks>
		/// The alpha value set for any given window may or may not be the final
		/// alpha value that is used when rendering.  All window objects, by
		/// default, inherit alpha from thier parent window(s) - this will blend
		/// child windows, relatively, down the line of inheritance.  This behaviour
		/// can be overridden via the setInheritsAlpha() method.  To return the true
		/// alpha value that will be applied when rendering, use the
		/// getEffectiveAlpha() method.
		/// </remarks>
		/// <param name="value">
		/// The new alpha value for the window.
		/// Value should be between 0.0f and 1.0f.
		/// </param>
		public void SetAlpha(float value)
		{
			// clamp this to the valid range [0.0, 1.0]
			d_alpha = Math.Max(Math.Min(value, 1.0f), 0.0f);
			OnAlphaChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// Sets whether this Window will inherit alpha from its parent windows.
		/// </summary>
		/// <param name="value">
		/// - true if the Window should use inherited alpha.
		/// - false if the Window should have an independant alpha value.
		/// </param>
		public void SetInheritsAlpha(bool value)
		{
			if (d_inheritsAlpha != value)
			{
				// store old effective alpha so we can test if alpha value changes due
				// to new setting.
				var oldAlpha = GetEffectiveAlpha();

				// notify about the setting change.
				d_inheritsAlpha = value;

				var args = new WindowEventArgs(this);
				OnInheritsAlphaChanged(args);

				// if effective alpha has changed fire notification about that too
				if (Math.Abs(oldAlpha - GetEffectiveAlpha()) > float.Epsilon)
				{
					args.handled = 0;
					OnAlphaChanged(args);
				}
			}
		}

		/// <summary>
		/// Invalidate this window causing at least this window to be redrawn during
		/// the next rendering pass.
		/// </summary>
		[Obsolete("This function is deprecated in favour of the version taking a boolean.")]
		public void Invalidate()
		{
			Invalidate(false);
		}

		/// <summary>
		/// Invalidate this window and - dependant upon \a recursive - all child
		/// content, causing affected windows to be redrawn during the next
		/// rendering pass.
		/// </summary>
		/// <param name="recursive">
		/// Boolean value indicating whether attached child content should also be
		/// invalidated.
		/// - true will cause all child content to be invalidated also.
		/// - false will just invalidate this single window.
		/// </param>
		public void Invalidate(bool recursive)
		{
			InvalidateImpl(recursive);
			GetGUIContext().MarkAsDirty();
		}

		/// <summary>
		/// Set the mouse cursor image to be used when the mouse enters this window.
		/// </summary>
		/// <param name="image">
		/// Pointer to the Image object to use as the mouse cursor image when the
		/// mouse enters the area for this Window.
		/// </param>
		public void SetCursor(Image image)
		{
			d_mouseCursor = image;

			if (GetGUIContext().GetWindowContainingCursor() == this)
				GetGUIContext().GetCursor().SetImage(image);
		}

		/// <summary>
		/// Set the mouse cursor image to be used when the mouse enters this window.
		/// </summary>
		/// <param name="name">
		/// String object that contains the name of the Image to use.
		/// </param>
		/// <exception cref="UnknownObjectException">
		/// thrown if no Image named \a name exists.
		/// </exception>
		public void SetCursor(string name)
		{
			SetCursor(ImageManager.GetSingleton().Get(name));
		}

		/// <summary>
		///  Set the user data set for this Window.
		/// 
		/// Each Window can have some client assigned data attached to it, this data
		/// is not used by the GUI system in any way.  Interpretation of the data is
		/// entirely application specific.
		/// </summary>
		/// <param name="value">
		/// pointer to the user data that is to be set for this window.
		/// </param>
		public void SetUserData(object value)
		{
			d_userData = value;
		}

		/// <summary>
		/// Set whether z-order changes are enabled or disabled for this Window.
		/// </summary>
		/// <remarks>
		/// This is distinguished from the is/setRiseOnClickEnabled setting in that
		/// if rise on click is disabled it only affects the users ability to affect
		/// the z order of the Window by clicking the mouse; is still possible to
		/// programatically alter the Window z-order by calling the moveToFront,
		/// moveToBack, moveInFront and moveBehind member functions.  Whereas if z
		/// ordering is disabled those functions are also precluded from affecting
		/// the Window z position.
		/// </remarks>
		/// <param name="value">
		/// - true if z-order changes are enabled for this window.
		///   moveToFront, moveToBack, moveInFront and moveBehind work normally.
		/// - false: z-order changes are disabled for this window.
		///   moveToFront, moveToBack, moveInFront and moveBehind are ignored.
		/// </param>
		public void SetZOrderingEnabled(bool value)
		{
			d_zOrderingEnabled = value;
		}

		/// <summary>
		/// Set whether this window will receive multi-click events or multiple
		/// 'down' events instead.
		/// </summary>
		/// <param name="value">
		/// - true if the Window will receive double-click and triple-click events.
		/// - false if the Window will receive multiple mouse button down events
		///   instead of double/triple click events.
		/// </param>
		public void SetWantsMultiClickEvents(bool value)
		{
			d_wantsMultiClicks = value;
		}

		/// <summary>
		/// Set whether mouse button down event autorepeat is enabled for this
		/// window.
		/// </summary>
		/// <param name="value">
		/// - true to enable autorepeat of mouse button down events.
		/// - false to disable autorepeat of mouse button down events.
		/// </param>
		public void SetMouseAutoRepeatEnabled(bool value)
		{
			if (d_autoRepeat == value)
				return;

			d_autoRepeat = value;
			d_repeatPointerSource = CursorInputSource.None;

			// FIXME: There is a potential issue here if this setting is
			// FIXME: changed _while_ the mouse is auto-repeating, and
			// FIXME: the 'captured' state of input could get messed up.
			// FIXME: The alternative is to always release here, but that
			// FIXME: has a load of side effects too - so for now nothing
			// FIXME: is done.  This whole aspect of the system needs a
			// FIXME: review an reworking - though such a change was
			// FIXME: beyond the scope of the bug-fix that originated this
			// FIXME: comment block.  PDT - 30/10/06
		}

		/// <summary>
		/// Set the current auto-repeat delay setting for this window.
		/// </summary>
		/// <param name="delay">
		/// float value indicating the delay, in seconds, defore the first repeat
		/// mouse button down event should be triggered when autorepeat is enabled.
		/// </param>
		public void SetAutoRepeatDelay(float delay)
		{
			d_repeatDelay = delay;
		}

		/// <summary>
		/// Set the current auto-repeat rate setting for this window.
		/// </summary>
		/// <param name="rate">
		/// float value indicating the rate, in seconds, at which repeat mouse
		/// button down events should be generated after the initial delay has
		/// expired.
		/// </param>
		public void SetAutoRepeatRate(float rate)
		{
			d_repeatRate = rate;
		}

		/// <summary>
		/// Set whether the window wants inputs passed to its attached
		/// child windows when the window has inputs captured.
		/// </summary>
		/// <param name="value">
		/// - true if System should pass captured input events to child windows.
		/// - false if System should pass captured input events to this window only.
		/// </param>
		public void SetDistributesCapturedInputs(bool value)
		{
			d_distCapturedInputs = value;
		}

		/// <summary>
		/// Internal support method for drag &amp; drop.  You do not normally call
		/// this directly from client code.  See the DragContainer class.
		/// </summary>
		/// <param name="item"></param>
		public void NotifyDragDropItemEnters(DragContainer item)
		{
			if (item == null)
				return;

			OnDragDropItemEnters(new DragDropEventArgs(this) {dragDropItem = item});
		}

		/// <summary>
		/// Internal support method for drag &amp; drop.  You do not normally call
		/// this directly from client code. See the DragContainer class.
		/// </summary>
		/// <param name="item"></param>
		public void NotifyDragDropItemLeaves(DragContainer item)
		{
			if (item == null)
				return;

			OnDragDropItemLeaves(new DragDropEventArgs(this) {dragDropItem = item});
		}

		/// <summary>
		/// Internal support method for drag &amp; drop. You do not normally call
		/// this directly from client code. See the DragContainer class.
		/// </summary>
		/// <param name="item"></param>
		public void NotifyDragDropItemDropped(DragContainer item)
		{
			if (item == null)
				return;

			OnDragDropItemDropped(new DragDropEventArgs(this) {dragDropItem = item});
		}

		/// <summary>
		/// Internal destroy method which actually just adds the window and any
		/// parent destructed child windows to the dead pool.
		/// 
		/// This is virtual to allow for specialised cleanup which may be required
		/// in some advanced cases.  If you override this for the above reason, you
		/// MUST call this base class version.
		/// </summary>
		/// <remarks>
		/// You never have to call this method yourself, use WindowManager to
		/// destroy your Window objects (which will call this for you).
		/// </remarks>
		public virtual void Destroy()
		{
			// because we know that people do not read the API ref properly,
			// here is some protection to ensure that WindowManager does the
			// destruction and not anyone else.
			var wmgr = WindowManager.GetSingleton();

			if (wmgr.IsAlive(this))
			{
				wmgr.DestroyWindow(this);

				// now return, the rest of what we need to do will happen
				// once WindowManager re-calls this method.
				return;
			}

			// signal our imminent destruction
			OnDestructionStarted(new WindowEventArgs(this));

			// Check we are detached from parent
			if (d_parent != null)
				d_parent.RemoveChild(this);

			ReleaseInput();

			// let go of the tooltip if we have it
			Tooltip tip = GetTooltip();
			if (tip != null && tip.GetTargetWindow() == this)
				tip.SetTargetWindow(null);

			// ensure custom tooltip is cleaned up
			SetTooltip(null);

			// clean up looknfeel related things
			if (!String.IsNullOrEmpty(d_lookName))
			{
				d_windowRenderer.OnLookNFeelUnassigned();
				WidgetLookManager.GetSingleton().GetWidgetLook(d_lookName).CleanUpWidget(this);
			}

			// free any assigned WindowRenderer
			if (d_windowRenderer != null)
			{
				d_windowRenderer.OnDetach();
				WindowRendererManager.Instance.DestroyWindowRenderer(d_windowRenderer);
				d_windowRenderer = null;
			}

			// double check we are detached from parent
			if (d_parent != null)
				d_parent.RemoveChild(this);

			CleanupChildren();

			ReleaseRenderingWindow();
			Invalidate(false);
		}

		/// <summary>
		/// Set the custom Tooltip object for this Window.  This value may be 0 to
		/// indicate that the Window should use the system default Tooltip object.
		/// </summary>
		/// <param name="tooltip">
		/// Pointer to a valid Tooltip based object which should be used as the
		/// tooltip for this Window, or 0 to indicate that the Window should use the
		/// system default Tooltip object.  Note that when passing a pointer to a
		/// Tooltip object, ownership of the Tooltip does not pass to this Window
		/// object.
		/// </param>
		public void SetTooltip(Tooltip tooltip)
		{
			// destroy current custom tooltip if one exists and we created it
			if (d_customTip != null && d_weOwnTip)
				WindowManager.GetSingleton().DestroyWindow(d_customTip);

			// set new custom tooltip
			d_weOwnTip = false;
			d_customTip = tooltip;
		}

		/// <summary>
		/// Set the custom Tooltip to be used by this Window by specifying a Window
		/// type.
		/// 
		/// The Window will internally attempt to create an instance of the
		/// specified window type (which must be derived from the base Tooltip
		/// class).  If the Tooltip creation fails, the error is logged and the
		/// Window will revert to using either the existing custom Tooltip or the
		/// system default Tooltip.
		/// </summary>
		/// <param name="tooltipType">
		/// String object holding the name of the Tooltip based Window type which
		/// should be used as the Tooltip for this Window.
		/// </param>
		public void SetTooltipType(string tooltipType)
		{
			// destroy current custom tooltip if one exists and we created it
			if (d_customTip != null && d_weOwnTip)
				WindowManager.GetSingleton().DestroyWindow(d_customTip);

			if (String.IsNullOrEmpty(tooltipType))
			{
				d_customTip = null;
				d_weOwnTip = false;
			}
			else
			{
				try
				{
					d_customTip = (Tooltip) WindowManager.GetSingleton()
					                                     .CreateWindow(tooltipType, GetName() + TooltipNameSuffix);
					d_customTip.SetAutoWindow(true);
					d_weOwnTip = true;
				}
				catch (UnknownObjectException)
				{
					d_customTip = null;
					d_weOwnTip = false;
				}
			}
		}

		/// <summary>
		/// Set the tooltip text for this window.
		/// </summary>
		/// <param name="value">
		/// String object holding the text to be displayed in the tooltip for this
		/// Window.
		/// </param>
		public void SetTooltipText(string value)
		{
			d_tooltipText = value;

			var tooltip = GetTooltip();

			if (tooltip != null && tooltip.GetTargetWindow() == this)
				tooltip.SetText(value);
		}

		/// <summary>
		/// Set whether this window inherits Tooltip text from its parent when its
		/// own tooltip text is not set.
		/// </summary>
		/// <param name="value">
		/// - true if the window should inherit tooltip text from its parent when
		///   its own text is not set.
		/// - false if the window should not inherit tooltip text from its parent
		///   (and so show no tooltip when no text is set).
		/// </param>
		public void SetInheritsTooltipText(bool value)
		{
			d_inheritsTipText = value;
		}

		/// <summary>
		/// Set whether this window will rise to the top of the z-order when clicked
		/// with the left mouse button.
		/// </summary>
		/// <remarks>
		/// This is distinguished from the is/setZOrderingEnabled setting in that
		/// if rise on click is disabled it only affects the users ability to affect
		/// the z order of the Window by clicking the mouse; is still possible to
		/// programatically alter the Window z-order by calling the moveToFront,
		/// moveToBack, moveInFront and moveBehind member functions.  Whereas if z
		/// ordering is disabled those functions are also precluded from affecting
		/// the Window z position.
		/// </remarks>
		/// <param name="value">
		/// - true if the window should come to the top of other windows when the
		///   left mouse button is pushed within its area.
		/// - false if the window should not change z-order position when the left
		///   mouse button is pushed within its area.
		/// </param>
		public void SetRiseOnClickEnabled(bool value)
		{
			d_riseOnClick = value;
		}

		/// <summary>
		/// Set the LookNFeel that shoule be used for this window.
		/// </summary>
		/// <param name="look">
		/// String object holding the name of the look to be assigned to the window.
		/// </param>
		/// <exception cref="UnknownObjectException">
		/// thrown if the look'n'feel specified by \a look does not exist.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// thrown if the Window does not have a WindowRenderer assigned to it.
		/// <seealso cref="Window.SetWindowRenderer"/>.
		/// </exception>
		/// <remarks>
		/// This is really intended as an internal function. The way that client
		/// code is supposed to use skins is by defining a Falagard mapping (either
		/// in a scheme xml file or in code) and then create instances of that
		/// mapped type via WindowManager.  See
		/// WindowFactoryManager::addFalagardWindowMapping and \ref xml_scheme. 
		/// With that being said, it is possible for client code to use this
		/// function so long as you are aware of the implications of doing so:
		/// - Automatically created child windows (AutoWindows) will be deleted, and
		///   references or pointers you hold to these will become invalid.
		/// - Aside from those absolutely required, there is not guarantee that the
		///   newly assigned look will create the same set of child windows, nor
		///   that any created windows will be of any given type.
		/// - Any properties set on automatically created child windows after their
		///   creation will be lost - even if the new look creates a child of the
		///   same type with the same name.
		/// </remarks>
		public virtual void SetLookNFeel(string look)
		{
			if (d_lookName == look)
				return;

			if (d_windowRenderer == null)
				throw new InvalidOperationException("There must be a window renderer assigned to the window '" + d_name +
				                                    "' to set its look'n'feel");

			var wlMgr = WidgetLookManager.GetSingleton();
			if (!String.IsNullOrEmpty(d_lookName))
			{
				d_windowRenderer.OnLookNFeelUnassigned();
				var oldWlf = wlMgr.GetWidgetLook(d_lookName);
				oldWlf.CleanUpWidget(this);
			}

			d_lookName = look;
			System.GetSingleton().Logger.LogEvent("Assigning LookNFeel '" + look +
			                                      "' to window '" + d_name + "'.", LoggingLevel.Informative);

			// Work to initialise the look and feel...
			WidgetLookFeel wlf = wlMgr.GetWidgetLook(look);
			// Get look and feel to initialise the widget as it needs.
			wlf.InitialiseWidget(this);
			// do the necessary binding to the stuff added by the look and feel
			InitialiseComponents();
			// let the window renderer know about this
			d_windowRenderer.OnLookNFeelAssigned();

			Invalidate(false);
			PerformChildWindowLayout();
		}

		/// <summary>
		/// Set the modal state for this Window.
		/// </summary>
		/// <param name="value">
		/// Boolean value defining if this Window should be the modal target.
		/// - true if this Window should be activated and set as the modal target.
		/// - false if the modal target should be cleared if this Window is
		///   currently the modal target.
		/// </param>
		public void SetModalState(bool value)
		{
			// do nothing if state isn't changing
			if (GetModalState() == value)
				return;

			// if going modal
			if (value)
			{
				Activate();
				GetGUIContext().SetModalWindow(this);
			}
			// clear the modal target
			else
				GetGUIContext().SetModalWindow(null);
		}


		/// <summary>
		/// Layout child window content.
		/// Laying out of child content includes:
		///     - ensuring content specified in any assigned WidgetLook has its area
		///       rectangles sychronised.
		///     - assigned WindowRenderer given the opportunity to update child
		///       content areas as needed.
		///     - All content is then potentially updated via the onParentSized
		///       notification as required by changes in non-client and client area
		///       rectangles.
		/// The system may call this at various times (like when a window is resized
		/// for example), and it may be invoked directly where required.
		/// </summary>
		/// <param name="nonClientSizedHint">
		/// Hint that the non-client area rectangle has changed size.
		/// </param>
		/// <param name="clientSizedHint">
		/// Hint that the client area rectangle has changed size.
		/// </param>
		/// <remarks>
		/// The hint parameters are essentially a way to force onParentSized
		/// notifications for a given type (client / nonclient) of child window.
		/// Setting a hint to false does not mean a notification will not happen,
		/// instead it means that the function is to do its best to determine
		/// whether a given notification is required to be sent.
		/// </remarks>
		public virtual void PerformChildWindowLayout(bool nonClientSizedHint = false, bool clientSizedHint = false)
		{
			var oldSize = d_pixelSize;
			d_pixelSize = CalculatePixelSize();

			LayoutLookNFeelChildWidgets();

			var outerChanged = nonClientSizedHint || d_pixelSize != oldSize;
			var innerChanged = clientSizedHint || IsInnerRectSizeChanged();

			d_outerRectClipperValid &= !outerChanged;
			d_innerRectClipperValid &= !innerChanged;

			if (d_windowRenderer != null)
				d_windowRenderer.PerformChildWindowLayout();

			NotifyChildrenOfSizeChange(outerChanged, innerChanged);
		}

		/// <summary>
		/// Sets the value a named user string, creating it as required.
		/// </summary>
		/// <param name="name">
		/// String object holding the name of the string to be returned.
		/// </param>
		/// <param name="value">
		/// String object holding the value to be assigned to the user string.
		/// </param>
		public void SetUserString(string name, string value)
		{
			d_userStrings[name] = value;
		}

		/// <summary>
		/// Draws the Window object and all of it's attached children to the display.
		/// </summary>
		public void Draw()
		{
			// don't do anything if window is not visible
			if (!IsEffectiveVisible())
				return;

			// get rendering context
			RenderingContext ctx;
			GetRenderingContext(out ctx);

			// clear geometry from surface if it's ours
			if (ctx.owner == this)
				ctx.surface.ClearGeometry();

			// redraw if no surface set, or if surface is invalidated
			if (d_surface == null || d_surface.IsInvalidated())
			{
				// perform drawing for 'this' Window
				DrawSelf(ctx);

				// render any child windows
				foreach (var it in d_drawList)
					it.Draw();
			}

			// do final rendering for surface if it's ours
			if (ctx.owner == this)
				ctx.surface.Draw();
		}

		/// <summary>
		/// Cause window to update itself and any attached children.  Client code
		/// does not need to call this method; to ensure full, and proper updates,
		/// call the injectTimePulse methodname method provided by the System class.
		/// </summary>
		/// <remarks>
		/// The update order is such that 'this' window is updated prior to any
		/// child windows, this is so that child windows that access the parent in
		/// their update code get the correct updated state.
		/// </remarks>
		/// <param name="elapsed">
		/// float value indicating the number of seconds passed since the last
		/// update.
		/// </param>
		public virtual void Update(float elapsed)
		{
			// perform update for 'this' Window
			UpdateSelf(elapsed);

			// update underlying RenderingWinodw if needed
			if (d_surface != null && d_surface.IsRenderingWindow())
				((RenderingWindow) d_surface).Update(elapsed);

			var e = new UpdateEventArgs(this, elapsed);
			FireEvent(EventUpdated, e);

			// update child windows
			for (var i = 0; i < GetChildCount(); ++i)
			{
				var child = GetChildAtIdx(i);

				// update children based on their WindowUpdateMode setting.
				if (child.d_updateMode == WindowUpdateMode.WUM_ALWAYS ||
				    (child.d_updateMode == WindowUpdateMode.WUM_VISIBLE && child.IsVisible()))
				{
					child.Update(elapsed);
				}
			}
		}

		/// <summary>
		/// Asks the widget to perform a clipboard copy to the provided clipboard
		/// </summary>
		/// <param name="clipboard">
		/// Target clipboard class
		/// </param>
		/// <returns>
		/// true if the copy was successful and allowed, false otherwise
		/// </returns>
		public virtual bool PerformCopy(Clipboard clipboard)
		{
			// deny copying by default
			return false;
		}

		/// <summary>
		/// Asks the widget to perform a clipboard cut to the provided clipboard
		/// </summary>
		/// <param name="clipboard">
		/// Target clipboard class
		/// </param>
		/// <returns>
		/// true if the cut was successful and allowed, false otherwise
		/// </returns>
		public virtual bool PerformCut(Clipboard clipboard)
		{
			// deny cutting by default
			return false;
		}

		/// <summary>
		/// Asks the widget to perform a clipboard paste from the provided clipboard
		/// </summary>
		/// <param name="clipboard">
		/// Source clipboard class
		/// </param>
		/// <returns>
		/// true if the paste was successful and allowed, false otherwise
		/// </returns>
		public virtual bool PerformPaste(Clipboard clipboard)
		{
			// deny pasting by default
			return false;
		}

		/// <summary>
		/// Asks the widget to perform a undo operation
		/// </summary>
		/// <returns>
		/// true if the undo was successful and allowed, false otherwise
		/// </returns>
		public virtual bool PerformUndo()
		{
			// deny undo by default
			return false;
		}

		/// <summary>
		/// Asks the widget to perform a redo operation
		/// </summary>
		/// <returns>
		/// true if the redo was successful and allowed, false otherwise
		/// </returns>
		public virtual bool PerformRedo()
		{
			// deny redo by default
			return false;
		}

		/// <summary>
		/// Writes an xml representation of this window object to \a out_stream.
		/// </summary>
		/// <param name="xml_stream">
		/// Stream where xml data should be output.
		/// </param>
		public virtual void WriteXMLToStream(XMLSerializer xml_stream)
		{
			// just stop now if we are'nt allowed to write XML
			if (!d_allowWriteXML)
				return;

			// output opening Window tag
			xml_stream.OpenTag(WindowXMLElementName)
			          .Attribute(WindowTypeXMLAttributeName, GetWidgetType());
			// write name if not auto-generated
			if (!GetName().StartsWith(WindowManager.GeneratedWindowNameBase))
			{
				xml_stream.Attribute(WindowNameXMLAttributeName, GetName());
			}
			// write out properties.
			WritePropertiesXML(xml_stream);
			// write out attached child windows.
			WriteChildWindowsXML(xml_stream);
			// now ouput closing Window tag
			xml_stream.CloseTag();

		}

		/// <summary>
		/// Sets the internal 'initialising' flag to true.
		/// This can be use to optimize initialisation of some widgets, and is called
		/// automatically by the layout XML handler when it has created a window.
		/// That is just after the window has been created, but before any children or
		/// properties are read.
		/// </summary>
		public virtual void BeginInitialisation()
		{
			d_initialising = true;
		}

		/// <summary>
		/// Sets the internal 'initialising' flag to false.
		/// This is called automatically by the layout XML handler when it is done
		/// creating a window. That is after all properties and children have been
		/// loaded and just before the next sibling gets created.
		/// </summary>
		public virtual void EndInitialisation()
		{
			d_initialising = false;
		}

		/*!
		\brief
		    Sets whether this window should ignore mouse events and pass them
		    through to any windows behind it. In effect making the window
		    transparent to the mouse.

		\param setting
		    true if mouse pass through is enabled.
		    false if mouse pass through is not enabled.
		*/

		public void SetCursorPassThroughEnabled(bool setting)
		{
			_cursorPassThroughEnabled = setting;
		}

		/// <summary>
		/// Assign the WindowRenderer type to be used when rendering this window.
		/// </summary>
		/// <param name="name">
		/// The factory name of the WindowRenderer to use.
		/// </param>
		/// <remarks>
		/// This is really intended as an internal function. The way that client
		/// code is supposed to use skins is by defining a Falagard mapping (either
		/// in a scheme xml file or in code) and then create instances of that
		/// mapped type via WindowManager.  See
		/// WindowFactoryManager::addFalagardWindowMapping and \ref xml_scheme. 
		/// </remarks>
		public void SetWindowRenderer(string name)
		{
			if (d_windowRenderer != null && d_windowRenderer.GetName() == name)
				return;

			var wrm = WindowRendererManager.Instance;
			if (d_windowRenderer != null)
			{
				// Allow reset of renderer
				if (d_windowRenderer.GetName() == name)
					return;

				OnWindowRendererDetached(new WindowEventArgs(this));
				wrm.DestroyWindowRenderer(d_windowRenderer);
			}

			if (!String.IsNullOrEmpty(name))
			{
				System.GetSingleton()
				      .Logger.LogEvent("Assigning the window renderer '" +
				                       name + "' to the window '" + d_name + "'", LoggingLevel.Informative);
				d_windowRenderer = wrm.CreateWindowRenderer(name);
				OnWindowRendererAttached(new WindowEventArgs(this));
			}
			else
			{
				throw new InvalidRequestException(
					"Attempt to assign a 'null' window renderer to window '" + d_name + "'.");
			}
		}

		/// <summary>
		/// Get the currently assigned WindowRenderer. (Look'N'Feel specification).
		/// </summary>
		/// <returns>
		/// A pointer to the assigned window renderer object.
		/// null if no window renderer is assigned.
		/// </returns>
		public WindowRenderer GetWindowRenderer()
		{
			return d_windowRenderer;
		}

		/// <summary>
		/// Get the factory name of the currently assigned WindowRenderer.
		/// (Look'N'Feel specification).
		/// </summary>
		/// <returns>
		/// The factory name of the currently assigned WindowRenderer.
		/// If no WindowRenderer is assigned an empty string is returned.
		/// </returns>
		public string GetWindowRendererName()
		{
			if (d_windowRenderer != null)
				return d_windowRenderer.GetName();

			return String.Empty;
		}

		/// <summary>
		/// Sets whether this window is allowed to write XML
		/// </summary>
		/// <param name="allow"></param>
		public void SetWritingXmlAllowed(bool allow)
		{
			d_allowWriteXML = allow;
		}

		/// <summary>
		/// Inform the window, and optionally all children, that screen area
		/// rectangles have changed.
		/// </summary>
		/// <param name="recursive">
		/// - true to recursively call notifyScreenAreaChanged on attached child
		///   Window objects.
		/// - false to just process \e this Window.
		/// </param>
		public override void NotifyScreenAreaChanged(bool recursive = true)
		{
			MarkCachedWindowRectsInvalid();
			base.NotifyScreenAreaChanged(recursive);

			UpdateGeometryRenderSettings();
		}

		/// <summary>
		/// Changes the widget's falagard type, thus changing its look'n'feel and optionally its
		/// renderer in the process.
		/// </summary>
		/// <param name="type">New look'n'feel of the widget</param>
		/// <param name="rendererType">New renderer of the widget</param>
		public void SetFalagardType(string type, string rendererType = "")
		{
			// Retrieve the new widget look
			const string separator = "/";
			var pos = type.IndexOf(separator);
			string newLook = type.Substring(0, pos);

			// Check if old one is the same. If so, ignore since we don't need to do
			// anything (type is already assigned)
			pos = d_falagardType.IndexOf(separator);
			var oldLook = d_falagardType.Substring(0, pos);
			if (oldLook == newLook)
				return;

			// Obtain widget kind
			var widget = d_falagardType.Substring(pos + 1);

			// Build new type (look/widget)
			d_falagardType = newLook + separator + widget;

			// Set new renderer
			if (rendererType.Length > 0)
				SetWindowRenderer(rendererType);

			// Apply the new look to the widget
			SetLookNFeel(type);
		}

		/// <summary>
		/// Specifies whether this Window object will receive events generated by
		/// the drag and drop support in the system.
		/// </summary>
		/// <param name="setting">
		/// - true to enable the Window as a drag and drop target.
		/// - false to disable the Window as a drag and drop target.
		/// </param>
		public void SetDragDropTarget(bool setting)
		{
			d_dragDropTarget = setting;
		}

		/// <summary>
		/// Set the RenderingSurface to be associated with this Window, or 0 if
		/// none is required.
		/// <para>
		/// If this function is called, and the option for automatic use of an
		/// imagery caching RenderingSurface is enabled, any automatically created
		/// RenderingSurface will be released, and the affore mentioned option will
		/// be disabled.
		/// </para>
		/// <para>
		/// If after having set a custom RenderingSurface you then subsequently
		/// enable the automatic use of an imagery caching RenderingSurface by
		/// calling setUsingAutoRenderingSurface, the previously set
		/// RenderingSurface will be disassociated from the Window.  Note that the
		/// previous RenderingSurface is not destroyed or cleaned up at all - this
		/// is the job of whoever created that object initially.
		/// </para>
		/// </summary>
		/// <param name="surface">
		/// Pointer to the RenderingSurface object to be associated with the window.
		/// </param>
		public void SetRenderingSurface(RenderingSurface surface)
		{
			if (d_surface == surface)
				return;

			if (d_autoRenderingWindow)
				SetUsingAutoRenderingSurface(false);

			d_surface = surface;

			// transfer child surfaces to this new surface
			if (d_surface != null)
			{
				TransferChildSurfaces();
				NotifyScreenAreaChanged();
			}
		}

		/// <summary>
		/// Invalidate the chain of rendering surfaces from this window backwards to
		/// ensure they get properly redrawn - but doing the minimum amount of work
		/// possibe - next render.
		/// </summary>
		public void InvalidateRenderingSurface()
		{
			// invalidate our surface chain if we have one
			if (d_surface != null)
				d_surface.Invalidate();
			// else look through the hierarchy for a surface chain to invalidate.
			else if (d_parent != null)
				GetParent().InvalidateRenderingSurface();
		}

		/*!
		\brief
		    Sets whether \e automatic use of an imagery caching RenderingSurface
		    (i.e. a RenderingWindow) is enabled for this window.  The reason we
		    emphasise 'atutomatic' is because the client may manually set a
		    RenderingSurface that does exactlythe same job.
		\par
		    Note that this setting really only controls whether the Window
		    automatically creates and manages the RenderingSurface, as opposed to
		    the \e use of the RenderingSurface.  If a RenderingSurfaceis set for the
		    Window it will be used regardless of this setting.
		\par
		    Enabling this option will cause the Window to attempt to create a
		    suitable RenderingSurface (which will actually be a RenderingWindow).
		    If there is an existing RenderingSurface assocated with this Window, it
		    will be removed as the Window's RenderingSurface
		    <em>but not destroyed</em>; whoever created the RenderingSurface in the
		    first place should take care of its destruction.
		\par
		    Disabling this option will cause any automatically created
		    RenderingSurface to be released.
		\par
		    It is possible that the renderer in use may not support facilities for
		    RenderingSurfaces that are suitable for full imagery caching.  If this
		    is the case, then calling getRenderingSurface after enabling this option
		    will return 0.  In these cases this option will still show as being
		    'enabled', this is because Window \e settings should not be influenced
		    by capabilities the renderer in use; for example, this enables correct
		    XML layouts to be written from a Window on a system that does not
		    support such RenderingSurfaces, so that the layout will function as
		    preferred on systems that do.
		\par
		    If this option is enabled, and the client subsequently assigns a
		    different RenderingSurface to the Window, the existing automatically
		    created RenderingSurface will be released and this setting will be
		    disabled.

		\param setting
		    - true to enable automatic use of an imagery caching RenderingSurface.
		    - false to disable automatic use of an imagery caching RenderingSurface.
		*/

		/// <summary>
		/// Sets whether \e automatic use of an imagery caching RenderingSurface
		/// (i.e. a RenderingWindow) is enabled for this window.  The reason we
		/// emphasise 'atutomatic' is because the client may manually set a
		/// RenderingSurface that does exactlythe same job.
		/// <para>
		/// Note that this setting really only controls whether the Window
		/// automatically creates and manages the RenderingSurface, as opposed to
		/// the \e use of the RenderingSurface.  If a RenderingSurfaceis set for the
		/// Window it will be used regardless of this setting.
		/// </para>
		/// <para>
		/// Enabling this option will cause the Window to attempt to create a
		/// suitable RenderingSurface (which will actually be a RenderingWindow).
		/// If there is an existing RenderingSurface assocated with this Window, it
		/// will be removed as the Window's RenderingSurface
		/// <em>but not destroyed</em>; whoever created the RenderingSurface in the
		/// first place should take care of its destruction.
		/// </para>
		/// <para>
		/// Disabling this option will cause any automatically created
		/// RenderingSurface to be released.
		/// </para>
		/// <para>
		/// It is possible that the renderer in use may not support facilities for
		/// RenderingSurfaces that are suitable for full imagery caching.  If this
		/// is the case, then calling getRenderingSurface after enabling this option
		/// will return 0.  In these cases this option will still show as being
		/// 'enabled', this is because Window \e settings should not be influenced
		/// by capabilities the renderer in use; for example, this enables correct
		/// XML layouts to be written from a Window on a system that does not
		/// support such RenderingSurfaces, so that the layout will function as
		/// preferred on systems that do.
		/// </para>
		/// <para>
		/// If this option is enabled, and the client subsequently assigns a
		/// different RenderingSurface to the Window, the existing automatically
		/// created RenderingSurface will be released and this setting will be
		/// disabled.
		/// </para>
		/// </summary>
		/// <param name="value">
		/// - true to enable automatic use of an imagery caching RenderingSurface.
		/// - false to disable automatic use of an imagery caching RenderingSurface.
		/// </param>
		public void SetUsingAutoRenderingSurface(bool value)
		{
			if (value)
			{
				AllocateRenderingWindow(d_autoRenderingSurfaceStencilEnabled);
			}
			else
			{
				ReleaseRenderingWindow();

				// make sure we set this because releaseRenderingWindow won't do it
				// unless the surface was already initialised
				d_autoRenderingWindow = value;
			}

			// while the actual area on screen may not have changed, the arrangement of
			// surfaces and geometry did...
			NotifyScreenAreaChanged();
		}

		/// <summary>
		/// Return the parsed RenderedString object for this window.
		/// </summary>
		/// <returns></returns>
		public RenderedString GetRenderedString()
		{
			if (!d_renderedStringValid)
			{
				d_renderedString = GetRenderedStringParser().Parse(GetTextVisual(), GetFont(), null);
				d_renderedStringValid = true;
			}

			return d_renderedString;
		}

		/// <summary>
		/// Return a pointer to any custom RenderedStringParser set, or 0 if none.
		/// </summary>
		/// <returns></returns>
		public RenderedStringParser GetCustomRenderedStringParser()
		{
			return d_customStringParser;
		}

		/// <summary>
		/// Set a custom RenderedStringParser, or 0 to remove an existing one.
		/// </summary>
		/// <param name="parser"></param>
		public void SetCustomRenderedStringParser(RenderedStringParser parser)
		{
			d_customStringParser = parser;
			d_renderedStringValid = false;
		}

		//! return the active RenderedStringParser to be used
		public virtual RenderedStringParser GetRenderedStringParser()
		{
			// if parsing is disabled, we use a DefaultRenderedStringParser that creates
			// a RenderedString to renderi the input text verbatim (i.e. no parsing).
			if (!d_textParsingEnabled)
				return d_defaultStringParser;

			// Next prefer a custom RenderedStringParser assigned to this Window.
			if (d_customStringParser != null)
				return d_customStringParser;

			// Next prefer any globally set RenderedStringParser.
			var globalParser = System.GetSingleton().GetDefaultCustomRenderedStringParser();
			if (globalParser != null)
				return globalParser;

			// if parsing is enabled and no custom RenderedStringParser is set anywhere,
			// use the system's BasicRenderedStringParser to do the parsing.
			return d_basicStringParser;
		}

		/// <summary>
		/// return whether text parsing is enabled for this window.
		/// </summary>
		/// <returns></returns>
		public bool IsTextParsingEnabled()
		{
			return d_textParsingEnabled;
		}

		/// <summary>
		/// set whether text parsing is enabled for this window.
		/// </summary>
		/// <param name="value"></param>
		public void SetTextParsingEnabled(bool value)
		{
			d_textParsingEnabled = value;
			d_renderedStringValid = false;

			OnTextParsingChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// set margin
		/// </summary>
		/// <param name="value"></param>
		public virtual void SetMargin(UBox value)
		{
			d_margin = value;

			OnMarginChanged(new WindowEventArgs(this));
		}

		/// <summary>
		/// retrieves currently set margin
		/// </summary>
		/// <returns></returns>
		public UBox GetMargin()
		{
			return d_margin;
		}

		/// <summary>
		/// return Vector2 \a pos after being fully unprojected for this Window.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Lunatics.Mathematics.Vector2 GetUnprojectedPosition(Lunatics.Mathematics.Vector2 pos)
		{
			var rs = GetTargetRenderingSurface();

			// if window is not backed by RenderingWindow, return same pos.
			if (!rs.IsRenderingWindow())
				return pos;

			// get first target RenderingWindow
			var rw = (RenderingWindow) rs;

			// setup for loop
			var outPos = pos;

			// while there are rendering windows
			while (rw != null)
			{
				// unproject the point for the current rw
				var inPos = outPos;
				rw.UnprojectPoint(inPos, out outPos);

				// get next rendering window, if any
				rw = (rs = rw.GetOwner()).IsRenderingWindow() ? (RenderingWindow) rs : null;
			}

			return outPos;
		}

		//! return the pointer to the BidiVisualMapping for this window, if any.
		public BidiVisualMapping GetBidiVisualMapping()
		{
			return d_bidiVisualMapping;
		}

		/// <summary>
		/// Add the named property to the XML ban list for this window.
		/// </summary>
		/// <param name="property_name">
		/// Name of the property you want to ban.
		/// </param>
		/// <remarks>
		/// Essentially a property that is banned from XML will never end up being saved to it.
		/// This is very useful if 2 properties overlap (XPosition and Position for example).
		/// 
		/// Please note that properties that are not writable (read-only properties) are
		/// implicitly/automatically banned from XML, no need to ban them manually.
		/// </remarks>
		public void BanPropertyFromXML(string property_name)
		{
			var instance = GetPropertyInstance(property_name);
			if (!instance.IsWritable())
			{
				System.GetSingleton()
				      .Logger.LogEvent("Property '" + property_name + "' " +
				                       "is not writable so it's implicitly banned from XML. No need " +
				                       "to ban it manually", LoggingLevel.Warnings);

				return;
			}

			// check if the insertion failed
			if (!d_bannedXMLProperties.Contains(property_name))
			{
				d_bannedXMLProperties.Add(property_name);
			}
			else
			{
				// just log the incidence
				Logger.LogInsane("Window.BanPropertyFromXML: The property '" + property_name + "' is already banned in window '" +
				                 d_name + "'");
			}
		}

		/// <summary>
		/// Remove the named property from the XML ban list for this window.
		/// </summary>
		/// <param name="property_name"></param>
		public void UnbanPropertyFromXML(string property_name)
		{
			d_bannedXMLProperties.Remove(property_name);
		}

		/// <summary>
		/// Return whether the named property is banned from XML
		/// </summary>
		/// <param name="property_name">
		/// Read-only properties and properties that can't write to XML streams
		/// are implicitly banned. This method will return true for them.
		/// </param>
		/// <returns></returns>
		public bool IsPropertyBannedFromXML(string property_name)
		{
			// generally, there will always less banned properties than all properties,
			// so it makes sense to check that first before querying the property instance
			if (d_bannedXMLProperties.Contains(property_name))
			{
				return true;
			}

			// properties that don't write any XML code are implicitly banned

			// read-only properties are implicitly banned
			// (such stored information wouldn't be of any value in the XML anyways,
			// no way to apply it to the widget)
			var instance = GetPropertyInstance(property_name);
			return (!instance.DoesWriteXML() || !instance.IsWritable());
		}

		/// <summary>
		/// Add the given property to the XML ban list for this window.
		/// </summary>
		/// <param name="property"></param>
		public void BanPropertyFromXML(Property property)
		{
			if (property != null)
				BanPropertyFromXML(property.GetName());
		}

		/// <summary>
		/// Remove the given property from the XML ban list for this window.
		/// </summary>
		/// <param name="property"></param>
		public void UnbanPropertyFromXML(Property property)
		{
			if (property != null)
				UnbanPropertyFromXML(property.GetName());
		}

		/// <summary>
		/// Return whether given property is banned from XML
		/// </summary>
		/// <param name="property">
		/// Read-only properties and properties that can't write to XML streams
		/// are implicitly banned. This method will return true for them.
		/// </param>
		/// <returns></returns>
		public bool IsPropertyBannedFromXML(Property property)
		{
			if (property != null)
				return IsPropertyBannedFromXML(property.GetName());
			else
				return false;
		}

		/// <summary>
		/// Set the window update mode.  This mode controls the behaviour of the
		/// Window::update member function such that updates are processed for
		/// this window (and therefore it's child content) according to the set
		/// mode.
		/// </summary>
		/// <remarks>
		/// Disabling updates can have negative effects on the behaviour of CEGUI
		/// windows and widgets; updates should be disabled selectively and
		/// cautiously - if you are unsure of what you are doing, leave the mode
		/// set to WUM_ALWAYS.
		/// </remarks>
		/// <param name="value">
		/// One of the WindowUpdateMode enumerated values indicating the mode to
		/// set for this Window.
		/// </param>
		public void SetUpdateMode(WindowUpdateMode value)
		{
			d_updateMode = value;
		}

		/// <summary>
		/// Return the current window update mode that is set for this Window.
		/// This mode controls the behaviour of the Window::update member function
		/// such that updates are processed for this window (and therefore it's
		/// child content) according to the set mode.
		/// </summary>
		/// <remarks>
		/// Disabling updates can have negative effects on the behaviour of CEGUI
		/// windows and widgets; updates should be disabled selectively and
		/// cautiously - if you are unsure of what you are doing, leave the mode
		/// set to WUM_ALWAYS.
		/// </remarks>
		/// <returns>
		/// One of the WindowUpdateMode enumerated values indicating the current
		/// mode set for this Window.
		/// </returns>
		public WindowUpdateMode GetUpdateMode()
		{
			return d_updateMode;
		}

		/// <summary>
		/// Set whether mouse input that is not directly handled by this Window
		/// (including it's event subscribers) should be propagated back to the
		/// Window's parent.
		/// </summary>
		/// <param name="value">
		/// - true if unhandled mouse input should be propagated to the parent.
		/// - false if unhandled mouse input should not be propagated.
		/// </param>
		public void SetCursorInputPropagationEnabled(bool value)
		{
			d_propagatePointerInputs = value;
		}

		/// <summary>
		/// Return whether mouse input that is not directly handled by this Window
		/// (including it's event subscribers) should be propagated back to the
		/// Window's parent.
		/// </summary>
		/// <returns>
		/// - true if unhandled mouse input will be propagated to the parent.
		/// - false if unhandled mouse input will not be propagated.
		/// </returns>
		public bool IsMouseInputPropagationEnabled()
		{
			return d_propagatePointerInputs;
		}

		/// <summary>
		/// Clones this Window and returns the result
		/// </summary>
		/// <param name="deepCopy">deepCopy if true, even children are copied</param>
		/// <returns>the cloned Window</returns>
		public Window Clone(bool deepCopy = true)
		{
			var ret = WindowManager.GetSingleton().CreateWindow(GetWidgetType(), GetName());

			// always copy properties
			ClonePropertiesTo(ret);

			// if user requested deep copy, we should copy children as well
			if (deepCopy)
				CloneChildWidgetsTo(ret);

			return ret;
		}

		/// <summary>
		/// copies this widget's properties to given target widget
		/// </summary>
		/// <param name="target"></param>
		public virtual void ClonePropertiesTo(Window target)
		{
			foreach (var propertyIt in ((PropertySet) this))
			{
				var propertyName = propertyIt.GetName();
				var propertyValue = GetProperty(propertyName);

				// we never copy stuff that doesn't get written into XML
				if (IsPropertyBannedFromXML(propertyName))
					continue;

				// some cases when propertyValue is "" could lead to exception throws
				if (String.IsNullOrEmpty(propertyValue))
				{
					// special case, this causes exception throw when no window renderer
					// is assigned to the window
					if (propertyName == "LookNFeel")
						continue;

					// special case, this causes exception throw because we are setting
					// 'null' window renderer
					if (propertyName == "WindowRenderer")
						continue;
				}

				target.SetProperty(propertyName, propertyValue);
			}
		}

		/// <summary>
		/// copies this widget's child widgets to given target widget
		/// </summary>
		/// <param name="target"></param>
		public virtual void CloneChildWidgetsTo(Window target)
		{
			// todo: ChildWindowIterator?
			for (var childI = 0; childI < GetChildCount(); ++childI)
			{
				var child = GetChildAtIdx(childI);
				if (child.IsAutoWindow())
				{
					// we skip auto windows, they are already created
					// automatically

					// note: some windows store non auto windows inside auto windows,
					//       standard solution is to copy these non-auto windows to
					//       the parent window
					//
					//       If you need alternative behaviour, you have to override
					//       this method!

					// so just copy it's child widgets
					child.CloneChildWidgetsTo(target);
					// and skip the auto widget
					continue;
				}

				var newChild = child.Clone(true);
				target.AddChild(newChild);
			}
		}

		/*!
		 \brief
		 Recursively updates all rendering surfaces and windows to work with a new host surface.
		 */
		public virtual void OnTargetSurfaceChanged(RenderingSurface newSurface)
		{
			// Surface was set manually, we don't control it
			//???if (d_surface && !d_surface->isRenderingWindow())?
			//???any window must be processed, even the one that was set externally?
			if (d_surface != null && !d_autoRenderingWindow)
				return;

			if (d_autoRenderingWindow)
			{
				// We use our own auto-window and must update its state
				if (d_surface == null)
				{
					if (newSurface != null)
					{
						AllocateRenderingWindow(d_autoRenderingSurfaceStencilEnabled);

						// Propagate our auto-window as a new host surface for our children
						foreach (var child in d_children)
						{
							if (child is Window childWnd)
								childWnd.OnTargetSurfaceChanged(d_surface);
						}
					}
				}
				else if (newSurface == null)
				{
					if (d_surface != null)
					{
						// We are about to destroy our auto-window, so enforce children that use it
						// as a host surface to destroy their windows first.
						foreach (var child in d_children)
						{
							if (child is Window childWnd)
								childWnd.OnTargetSurfaceChanged(null);
						}

						ReleaseRenderingWindow();
					}
				}
				else if (newSurface != d_surface)
				{
					// Since we have a surface, child surfaces stay with us.  Though we
					// must now ensure /our/ surface is transferred.
					newSurface.TransferRenderingWindow((RenderingWindow) d_surface);
				}
			}
			else
			{
				// If we do not have a surface, transfer any surfaces from our children to
				// whatever our target surface now is.
				foreach (var child in d_children)
				{
					if (child is Window childWnd)
						childWnd.OnTargetSurfaceChanged(newSurface);
				}
			}
		}

		/// <summary>
		/// return the GUIContext this window is associated with.
		/// </summary>
		/// <returns></returns>
		public GUIContext GetGUIContext()
		{
			// GUIContext is always the one on the root window, we do not allow parts
			// of a hierarchy to be drawn to seperate contexts (which is not much of
			// a limitation).
			//
			// ISSUE: if root has no GUIContext set for it, should we return 0 or
			//        System::getDefaultGUIContext?  Come to IRC and argue about it!

			if (GetParent() != null)
				return GetParent().GetGUIContext();

			return d_guiContext ?? System.GetSingleton().GetDefaultGUIContext();
		}

		/// <summary>
		/// function used internally.  Do not call this from client code.
		/// </summary>
		/// <param name="context"></param>
		public void SetGUIContext(GUIContext context)
		{
			if (d_guiContext == context)
				return;

			d_guiContext = context;
			//SyncTargetSurface();
			OnTargetSurfaceChanged(GetTargetRenderingSurface());
		}

		///// <summary>
		///// ensure that the window will be rendered to the correct target surface.
		///// </summary>
		//public void SyncTargetSurface()
		//{
		//	// if we do not have a surface, xfer any surfaces from our children to
		//	// whatever our target surface now is.
		//	if (d_surface == null)
		//		TransferChildSurfaces();
		//	// else, since we have a surface, child surfaces stay with us.  Though we
		//	// must now ensure /our/ surface is xferred if it is a RenderingWindow.
		//	else if (d_surface.IsRenderingWindow())
		//	{
		//		// target surface is eihter the parent's target, or the gui context.
		//		var tgt = d_parent != null ? GetParent().GetTargetRenderingSurface() : GetGUIContext();

		//		tgt.TransferRenderingWindow((RenderingWindow)d_surface);
		//	}
		//}

		/// <summary>
		/// Set whether this window is marked as an auto window.
		/// 
		/// An auto window is typically a Window object created automatically by
		/// CEGUI - for example to form part of a multi-element 'compound' widget.
		/// </summary>
		/// <param name="value"></param>
		public void SetAutoWindow(bool value)
		{
			d_autoWindow = value;

			if (d_autoWindow)
				BanPropertiesForAutoWindow();
		}

		/// <summary>
		/// Return whether Window thinks mouse is currently within its area.
		/// </summary>
		/// <remarks>
		/// If the mouse cursor has moved or Window's area has changed since the
		/// last time the GUIContext updated the window hit information, the value
		/// returned here may be inaccurate - this is not a bug, but is required
		/// to ensure correct handling of certain events.
		/// </remarks>
		/// <returns></returns>
		public bool IsMouseContainedInArea()
		{
			return d_containsMouse;
		}

		// overridden from Element
		public override Sizef GetRootContainerSize()
		{
			return GetGUIContext().GetSurfaceSize();
		}

		/// <summary>
		/// Return whether this Window is focused or not.
		/// 
		/// A window is focused when it is the active Window inside the current
		/// GUIContext.
		/// </summary>
		/// <returns></returns>
		public bool IsFocused()
		{
			return d_isFocused;
		}

		/// <summary>
		/// Makes this Window be focused.
		/// 
		/// Focusing a Window means activating it and setting the focused flag.
		/// This will also trigger the activated event. Focusing works only on
		/// non-disabled widgets.
		/// </summary>
		public void Focus()
		{
			if (IsDisabled())
				return;
			d_isFocused = true;
			OnActivated(new ActivationEventArgs(this));
		}

		/// <summary>
		/// Unfocus this Window.
		/// 
		/// This will trigger the deactivated event if this was an active window.
		/// </summary>
		public void Unfocus()
		{
			d_isFocused = false;
			if (d_active)
				OnDeactivated(new ActivationEventArgs(this));
		}

		/// <summary>
		/// Return whether Window can be focused or not.
		/// 
		/// A Window cannot be usually focused when it's disabled. Other widgets
		/// can override this method based on their own behaviour.
		/// </summary>
		/// <returns></returns>
		public virtual bool CanFocus()
		{
			// by default all widgets can be focused if they are not disabled
			return !IsDisabled();
		}

		// TODO: friend classes for construction / initialisation purposes (for now)
		//friend class System;
		//friend class WindowManager;
		//friend class GUIContext;

		#region Event trigger methods

		/// <summary>
		/// Handler called when the window's size changes.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected override void OnSized(ElementEventArgs e)
		{
			/*
			 * Why are we not calling Element::onSized?  It's because that function
			 * always calls the onParentSized notification for all children - we really
			 * want that to be done via performChildWindowLayout instead and we
			 * definitely don't want it done twice.
			 *
			 * (The other option was to add an Element::performChildLayout function -
			 * maybe we should consider that).
			*/

			// resize the underlying RenderingWindow if we're using such a thing
			if (d_surface != null && d_surface.IsRenderingWindow())
				((RenderingWindow) d_surface).SetSize(GetPixelSize());

			// screen area changes when we're resized.
			// NB: Called non-recursive since the performChildWindowLayout call should
			// have dealt more selectively with child Window cases.
			NotifyScreenAreaChanged(false);
			PerformChildWindowLayout(true, true);

			Invalidate(false);
			FireEvent(Element.EventSized, e);
		}

		/// <summary>
		/// Handler called when the window's position changes.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected override void OnMoved(ElementEventArgs e)
		{
			base.OnMoved(e);

			// handle invalidation of surfaces and trigger needed redraws
			if (d_parent != null)
			{
				GetParent().InvalidateRenderingSurface();
				// need to redraw some geometry if parent uses a caching surface
				if (GetParent().GetTargetRenderingSurface().IsRenderingWindow())
					GetGUIContext().MarkAsDirty();
			}
		}

		protected override void OnRotated(ElementEventArgs e)
		{
			base.OnRotated(e);

			// if we have no surface set, enable the auto surface
			if (d_surface == null)
			{
				System.GetSingleton().Logger
				      .LogEvent("Window::setRotation - Activating AutoRenderingSurface on Window '" + d_name +
				                "' to enable rotation support.");

				SetUsingAutoRenderingSurface(true);

				// still no surface?  Renderer or HW must not support what we need :(
				if (d_surface == null)
				{
					System.GetSingleton().Logger
					      .LogEvent(
						      "Window::setRotation - Failed to obtain a suitable ReneringWindow surface for Window '" +
						      d_name + "'.  Rotation will not be available.",
						      LoggingLevel.Errors);

					return;
				}
			}

			// ensure surface we have is the right type
			if (!d_surface.IsRenderingWindow())
			{
				System.GetSingleton().Logger
				      .LogEvent(
					      "Window::setRotation - Window '" + d_name +
					      "' has a manual RenderingSurface that is not a RenderingWindow.  Rotation will not be available.",
					      LoggingLevel.Errors);

				return;
			}

			// Checks / setup complete!  Now we can finally set the rotation.
			((RenderingWindow) d_surface).SetRotation(d_rotation);
			((RenderingWindow) d_surface).SetPivot(new Lunatics.Mathematics.Vector3(d_pixelSize.Width / 2.0f,
			                                                                       d_pixelSize.Height / 2.0f,
			                                                                       0.0f));
		}

		/// <summary>
		/// Handler called when the window's text is changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnTextChanged(WindowEventArgs e)
		{
			Invalidate(false);
			FireEvent("TextChanged", e);

			//var handler = TextChanged;
			//if (handler != null)
			//    handler(this, e);

			//FireEvent(TextChanged, e);
		}

		/// <summary>
		/// Handler called when the window's font is changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected internal virtual void OnFontChanged(WindowEventArgs e)
		{
			// This was added to enable the Falagard FontDim to work
			// properly.  A better, more selective, solution would
			// probably be to do something funky with events ;)
			PerformChildWindowLayout();

			Invalidate(false);
			FireEvent(FontChanged, e);
		}

		/// <summary>
		/// Handler called when the window's alpha blend value is changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnAlphaChanged(WindowEventArgs e)
		{
			// scan child list and call this method for all children that inherit alpha
			var childCount = GetChildCount();

			for (var i = 0; i < childCount; ++i)
			{
				if (GetChildAtIdx(i).InheritsAlpha())
					GetChildAtIdx(i).OnAlphaChanged(new WindowEventArgs(GetChildAtIdx(i)));
			}

			UpdateGeometryBuffersAlpha();
			InvalidateRenderingSurface();
			GetGUIContext().MarkAsDirty();

			FireEvent(EventAlphaChanged, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the window's client assigned Id is changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnIdChanged(WindowEventArgs e)
		{
			FireEvent(EventIdChanged, e);
		}

		/// <summary>
		/// Handler called when the window is shown (made visible).
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnShown(WindowEventArgs e)
		{
			Invalidate(false);
			FireEvent(EventShown, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the window is hidden.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnHidden(WindowEventArgs e)
		{
			// first deactivate window if it is the active window.
			if (IsActive())
				Deactivate();

			Invalidate(false);
			FireEvent(EventHidden, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the window is enabled.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnEnabled(WindowEventArgs e)
		{
			// signal all non-disabled children that they are now enabled
			// (via inherited state)
			var childCount = GetChildCount();
			for (var i = 0; i < childCount; ++i)
			{
				if (GetChildAtIdx(i).d_enabled)
				{
					GetChildAtIdx(i).OnEnabled(new WindowEventArgs(GetChildAtIdx(i)));
				}
			}

			Invalidate(false);
			FireEvent(EventEnabled, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the window is disabled.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnDisabled(WindowEventArgs e)
		{
			// signal all non-disabled children that they are now disabled
			// (via inherited state)
			var childCount = GetChildCount();
			for (var i = 0; i < childCount; ++i)
			{
				if (GetChildAtIdx(i).d_enabled)
				{
					GetChildAtIdx(i).OnDisabled(new WindowEventArgs(GetChildAtIdx(i)));
				}
			}

			Invalidate(false);
			FireEvent(EventDisabled, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the window's setting for being clipped by it's
		/// parent is changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnClippingChanged(WindowEventArgs e)
		{
			Invalidate(false);
			NotifyClippingChanged();
			FireEvent(EventClippedByParentChanged, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the window's setting for being destroyed
		/// automatically be it's parent is changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnParentDestroyChanged(WindowEventArgs e)
		{
			FireEvent(EventDestroyedByParentChanged, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the window's setting for inheriting alpha-blending
		/// is changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnInheritsAlphaChanged(WindowEventArgs e)
		{
			Invalidate(false);
			FireEvent(EventInheritsAlphaChanged, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the window's always-on-top setting is changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnAlwaysOnTopChanged(WindowEventArgs e)
		{
			// we no longer want a total redraw here, instead we just get each window
			// to resubmit it's imagery to the Renderer.
			GetGUIContext().MarkAsDirty();
			FireEvent(EventAlwaysOnTopChanged, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when this window gains capture of mouse inputs.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnCaptureGained(WindowEventArgs e)
		{
			FireEvent(EventInputCaptureGained, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when this window loses capture of mouse inputs.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnCaptureLost(WindowEventArgs e)
		{
			// reset auto-repeat state
			d_repeatPointerSource = CursorInputSource.None;

			// handle restore of previous capture window as required.
			if (d_restoreOldCapture && (d_oldCapture != null))
			{
				d_oldCapture.OnCaptureLost(e);
				d_oldCapture = null;
			}

			// handle case where mouse is now in a different window
			// (this is a bit of a hack that uses the mouse input injector to handle
			// this for us).
			var moveEvent = new SemanticInputEvent((int) SemanticValue.SV_CursorMove);
			var cursorPosition = GetGUIContext().GetCursor().GetPosition();
			moveEvent.d_payload.array[0] = cursorPosition.X;
			moveEvent.d_payload.array[1] = cursorPosition.Y;
			GetGUIContext().InjectInputEvent(moveEvent);

			FireEvent(EventInputCaptureLost, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when this window gets invalidated.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnInvalidated(WindowEventArgs e)
		{
			FireEvent(EventInvalidated, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when rendering for this window has started.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnRenderingStarted(WindowEventArgs e)
		{
			FireEvent(EventRenderingStarted, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when rendering for this window has ended.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnRenderingEnded(WindowEventArgs e)
		{
			FireEvent(EventRenderingEnded, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the z-order position of this window has changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnZChanged(WindowEventArgs e)
		{
			// we no longer want a total redraw here, instead we just get each window
			// to resubmit it's imagery to the Renderer.
			GetGUIContext().MarkAsDirty();
			FireEvent(EventZOrderChanged, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when this window's destruction sequence has begun.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnDestructionStarted(WindowEventArgs e)
		{
			d_destructionStarted = true;
			FireEvent(EventDestructionStarted, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when this window has become the active window.
		/// </summary>
		/// <param name="e">
		/// ActivationEventArgs class whose 'otherWindow' field is set to the window
		/// that previously was active, or NULL for none.
		/// </param>
		protected virtual void OnActivated(ActivationEventArgs e)
		{
			d_active = true;
			Invalidate(false);
			FireEvent(EventActivated, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when this window has lost input focus and has been 
		/// deactivated.
		/// </summary>
		/// <param name="e">
		/// ActivationEventArgs object whose 'otherWindow' field is set to the
		/// window that has now become active, or NULL for none.
		/// </param>
		protected virtual void OnDeactivated(ActivationEventArgs e)
		{
			// first de-activate all children
			var childCount = GetChildCount();
			for (var i = 0; i < childCount; ++i)
			{
				if (GetChildAtIdx(i).IsActive())
				{
					// make sure the child gets itself as the .window member
					GetChildAtIdx(i)
							.OnDeactivated(new ActivationEventArgs(GetChildAtIdx(i)) {otherWindow = e.otherWindow});
				}

			}

			d_active = false;
			Invalidate(false);
			FireEvent(EventDeactivated, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when this window's parent window has been resized.  If
		/// this window is the root / GUI Sheet window, this call will be made when
		/// the display size changes.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set the the
		/// window that caused the event; this is typically either this window's
		/// parent window, or NULL to indicate the screen size has changed.
		/// </param>
		protected internal override void OnParentSized(ElementEventArgs e)
		{
			base.OnParentSized(e);

			// if we were not moved or sized, do child layout anyway!
			// URGENT FIXME
			//if (!(moved || sized))
			PerformChildWindowLayout();
		}

		protected virtual void OnParentSizedInner(ElementEventArgs e)
		{
			base.OnParentSized(e);
		}

		/// <summary>
		/// Handler called when a child window is added to this window.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that has been added.
		/// </param>
		protected override void OnChildAdded(ElementEventArgs e)
		{
			// we no longer want a total redraw here, instead we just get each window
			// to resubmit it's imagery to the Renderer.
			GetGUIContext().MarkAsDirty();

			base.OnChildAdded(e);
		}

		/// <summary>
		/// Handler called when a child window is removed from this window.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set the window
		/// that has been removed.
		/// </param>
		protected override void OnChildRemoved(ElementEventArgs e)
		{
			// we no longer want a total redraw here, instead we just get each window
			// to resubmit it's imagery to the Renderer.
			GetGUIContext().MarkAsDirty();
			// Though we do need to invalidate the rendering surface!
			GetTargetRenderingSurface().Invalidate();

			base.OnChildRemoved(e);
		}

		/// <summary>
		/// Handler called when the mouse cursor has entered this window's area.
		/// </summary>
		/// <param name="e">
		/// MouseEventArgs object.  All fields are valid.
		/// </param>
		protected internal virtual void OnCursorEntersArea(CursorInputEventArgs e)
		{
			d_containsMouse = true;
			FireEvent(EventCursorEntersArea, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the mouse cursor has left this window's area.
		/// </summary>
		/// <param name="e">
		/// MouseEventArgs object.  All fields are valid.
		/// </param>
		protected internal virtual void OnCursorLeavesArea(CursorInputEventArgs e)
		{
			d_containsMouse = false;
			FireEvent(EventCursorLeavesArea, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the mouse cursor has entered this window's area and
		/// is actually over some part of this windows surface and not, for
		/// instance over a child window - even though technically in those cases
		/// the mouse is also within this Window's area, the handler will not be
		/// called.
		/// </summary>
		/// <param name="e">
		/// MouseEventArgs object.  All fields are valid.
		/// </param>
		/// <seealso cref="OnCursorEntersArea"/>
		protected internal virtual void OnCursorEnters(CursorInputEventArgs e)
		{
			// set the mouse cursor
			GetGUIContext().GetCursor().SetImage(GetCursor());

			// perform tooltip control
			var tip = GetTooltip();
			if (tip != null && !IsAncestor(tip))
				tip.SetTargetWindow(this);

			FireEvent(EventCursorEntersSurface, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the mouse cursor is no longer over this window's
		/// surface area.  This will be called when the mouse is not over a part
		/// of this Window's actual surface - even though technically the mouse is
		/// still within the Window's area, for example if the mouse moves over a
		/// child window.
		/// </summary>
		/// <param name="e">
		/// MouseEventArgs object.  All fields are valid.
		/// </param>
		/// <seealso cref="OnCursorLeavesArea"/>
		protected internal virtual void OnCursorLeaves(CursorInputEventArgs e)
		{
			// perform tooltip control
			var mw = GetGUIContext().GetWindowContainingCursor();
			var tip = GetTooltip();
			if (tip != null && mw != tip && !(mw != null && mw.IsAncestor(tip)))
				tip.SetTargetWindow(null);

			FireEvent(EventCursorLeavesSurface, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the mouse cursor has been moved within this window's
		/// area.
		/// </summary>
		/// <param name="e">
		/// MouseEventArgs object.  All fields are valid.
		/// </param>
		protected internal virtual void OnCursorMove(CursorInputEventArgs e)
		{
			// perform tooltip control
			var tip = GetTooltip();
			if (tip != null)
				tip.ResetTimer();

			FireEvent(EventCursorMove, e, EventNamespace);

			// optionally propagate to parent
			if (e.handled == 0 && d_propagatePointerInputs &&
			    d_parent != null && this != GetGUIContext().GetModalWindow())
			{
				e.Window = GetParent();
				GetParent().OnCursorMove(e);

				return;
			}

			// by default we now mark mouse events as handled
			// (derived classes may override, of course!)
			++e.handled;
		}


		/// <summary>
		/// Handler called when the mouse wheel (z-axis) position changes within
		/// this window's area.
		/// </summary>
		/// <param name="e">
		/// MouseEventArgs object.  All fields are valid.
		/// </param>
		protected internal virtual void OnScroll(CursorInputEventArgs e)
		{
			FireEvent(EventScroll, e, EventNamespace);

			// optionally propagate to parent
			if (e.handled == 0 && d_propagatePointerInputs &&
			    d_parent != null && this != GetGUIContext().GetModalWindow())
			{
				e.Window = GetParent();
				GetParent().OnScroll(e);

				return;
			}

			// by default we now mark mouse events as handled
			// (derived classes may override, of course!)
			++e.handled;
		}

		/// <summary>
		/// Handler called when a cursor is held pressed within this window's area.
		/// </summary>
		/// <param name="e">
		/// CursorInputEventArgs object.  All fields are valid.
		/// </param>
		protected internal virtual void OnCursorPressHold(CursorInputEventArgs e)
		{
			// perform tooltip control
			var tip = GetTooltip();
			if (tip != null)
				tip.SetTargetWindow(null);

			if ((e.Source == CursorInputSource.Left) && MoveToFrontImpl(true))
				++e.handled;

			// if auto repeat is enabled and we are not currently tracking
			// the source that was just pushed (need this source check because
			// it could be us that generated this event via auto-repeat).
			if (d_autoRepeat)
			{
				if (d_repeatPointerSource == CursorInputSource.None)
					CaptureInput();

				if ((d_repeatPointerSource != e.Source) && IsCapturedByThis())
				{
					d_repeatPointerSource = e.Source;
					d_repeatElapsed = 0;
					d_repeating = false;
				}
			}

			FireEvent(EventCursorPressHold, e, EventNamespace);

			// optionally propagate to parent
			if (e.handled == 0 && d_propagatePointerInputs &&
			    d_parent != null && this != GetGUIContext().GetModalWindow())
			{
				e.Window = GetParent();
				GetParent().OnCursorPressHold(e);

				return;
			}

			// by default we now mark cursor events as handled
			// (derived classes may override, of course!)
			++e.handled;
		}

		/// <summary>
		/// Handler called when a cursor is activated within this window's area.
		/// </summary>
		/// <param name="e">
		/// CursorInputEventArgs object.  All fields are valid.
		/// </param>
		protected internal virtual void OnCursorActivate(CursorInputEventArgs e)
		{
			// reset auto-repeat state
			if (d_autoRepeat && d_repeatPointerSource != CursorInputSource.None)
			{
				ReleaseInput();
				d_repeatPointerSource = CursorInputSource.None;
			}

			FireEvent(EventCursorActivate, e, EventNamespace);

			// optionally propagate to parent
			if (e.handled == 0 && d_propagatePointerInputs &&
			    d_parent != null && this != GetGUIContext().GetModalWindow())
			{
				e.Window = GetParent();
				GetParent().OnCursorActivate(e);
			}
		}

		/// <summary>
		/// Handler called when a character-key has been pressed while this window
		/// has input focus.
		/// </summary>
		/// <param name="e">
		/// KeyEventArgs object whose 'codepoint' field is set to the Unicode code
		/// point (encoded as utf32) for the character typed, and whose 'sysKeys'
		/// field represents the combination of SystemKey that were active when the
		/// event was generated.  All other fields should be considered as 'junk'.
		/// </param>
		protected internal virtual void OnCharacter(TextEventArgs e)
		{
			FireEvent(EventCharacterKey, e, EventNamespace);

			// As of 0.7.0 CEGUI::System no longer does input event propogation, so by
			// default we now do that here.  Generally speaking key handling widgets
			// may need to override this behaviour to halt further propogation.
			if (e.handled == 0 && d_parent != null &&
			    this != GetGUIContext().GetModalWindow())
			{
				e.Window = GetParent();
				GetParent().OnCharacter(e);
			}
		}

		/// <summary>
		/// Handler called when a semantic input event occurred
		/// </summary>
		/// <param name="e">
		/// The semantic input event
		/// </param>
		protected internal virtual void OnSemanticInputEvent(SemanticEventArgs e)
		{
			FireEvent(EventSemanticEvent, e, EventNamespace);

			// optionally propagate to parent
			if (e.handled == 0 && d_parent != null && this != GetGUIContext().GetModalWindow())
			{
				e.Window = GetParent();
				GetParent().OnSemanticInputEvent(e);
			}
		}

		/// <summary>
		/// Handler called when a DragContainer is dragged over this window.
		/// </summary>
		/// <param name="e">
		/// DragDropEventArgs object initialised as follows:
		/// - window field is normaly set to point to 'this' window.
		/// - dragDropItem is a pointer to a DragContainer window that triggered
		///   the event.
		/// </param>
		protected virtual void OnDragDropItemEnters(DragDropEventArgs e)
		{
			FireEvent(EventDragDropItemEnters, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when a DragContainer is dragged over this window.
		/// </summary>
		/// <param name="e">
		/// DragDropEventArgs object initialised as follows:
		/// - window field is normaly set to point to 'this' window.
		/// - dragDropItem is a pointer to a DragContainer window that triggered
		///   the event.
		/// </param>
		protected virtual void OnDragDropItemLeaves(DragDropEventArgs e)
		{
			FireEvent(EventDragDropItemLeaves, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when a DragContainer is dragged over this window.
		/// </summary>
		/// <param name="e">
		/// DragDropEventArgs object initialised as follows:
		/// - window field is normaly set to point to 'this' window.
		/// - dragDropItem is a pointer to a DragContainer window that triggered
		///   the event.
		/// </param>
		protected virtual void OnDragDropItemDropped(DragDropEventArgs e)
		{
			FireEvent(EventDragDropItemDropped, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when a new window renderer object is attached.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object initialised as follows:
		/// - window field is set to point to the Window object that just got a new
		///   window renderer attached. (typically 'this').
		/// </param>
		protected virtual void OnWindowRendererAttached(WindowEventArgs e)
		{
			if (!ValidateWindowRenderer(d_windowRenderer))
			{
				throw new InvalidRequestException(
					"The window renderer '" + d_windowRenderer.GetName() + "' is not " +
					"compatible with this widget type (" + GetType() + ")");
			}

			d_windowRenderer.Window = this;
			d_windowRenderer.OnAttach();
			FireEvent(EventWindowRendererAttached, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the currently attached window renderer object is detached.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object initialised as follows:
		/// - window field is set to point to the Window object that just got lost its
		///   window renderer. (typically 'this').
		/// </param>
		protected virtual void OnWindowRendererDetached(WindowEventArgs e)
		{
			d_windowRenderer.OnDetach();
			d_windowRenderer.Window = null;
			FireEvent(EventWindowRendererDetached, e, EventNamespace);
		}

		/// <summary>
		/// Handler called when the window's setting for whether text parsing is
		/// enabled is changed.
		/// </summary>
		/// <param name="e">
		/// WindowEventArgs object whose 'window' pointer field is set to the window
		/// that triggered the event.  For this event the trigger window is always
		/// 'this'.
		/// </param>
		protected virtual void OnTextParsingChanged(WindowEventArgs e)
		{
			FireEvent(EventTextParsingChanged, e, EventNamespace);
		}

		protected virtual void OnMarginChanged(WindowEventArgs e)
		{
			FireEvent(EventMarginChanged, e, EventNamespace);
		}

		#endregion

		#region Implementation Functions

		/// <summary>
		/// Perform actual update processing for this Window.
		/// </summary>
		/// <param name="elapsed">
		/// float value indicating the number of seconds elapsed since the last
		/// update call.
		/// </param>
		protected virtual void UpdateSelf(float elapsed)
		{
			// Mouse button autorepeat processing.
			if (d_autoRepeat && d_repeatPointerSource != CursorInputSource.None)
			{
				d_repeatElapsed += elapsed;

				if (d_repeating)
				{
					if (d_repeatElapsed > d_repeatRate)
					{
						d_repeatElapsed -= d_repeatRate;
						// trigger the repeated event
						GenerateAutoRepeatEvent(d_repeatPointerSource);
					}
				}
				else
				{
					if (d_repeatElapsed > d_repeatDelay)
					{
						d_repeatElapsed = 0;
						d_repeating = true;
						// trigger the repeated event
						GenerateAutoRepeatEvent(d_repeatPointerSource);
					}
				}
			}

			// allow for updates within an assigned WindowRenderer
			if (d_windowRenderer != null)
				d_windowRenderer.Update(elapsed);
		}

		/// <summary>
		/// Perform the actual rendering for this Window.
		/// </summary>
		/// <param name="ctx">
		/// RenderingContext holding the details of the RenderingSurface to be
		/// used for the Window rendering operations.
		/// </param>
		protected virtual void DrawSelf(RenderingContext ctx)
		{
			BufferGeometry(ctx);
			QueueGeometry(ctx);
		}

		/// <summary>
		/// Perform drawing operations concerned with generating and buffering
		/// window geometry.
		/// </summary>
		/// <remarks>
		/// This function is a sub-function of drawSelf; it is provided to make it
		/// easier to override drawSelf without needing to duplicate large sections
		/// of the code from the default implementation.
		/// </remarks>
		/// <param name="ctx"></param>
		protected void BufferGeometry(RenderingContext ctx)
		{
			if (d_needsRedraw)
			{
				// dispose of already cached geometry.
				DestroyGeometryBuffers();

				// signal rendering started
				var args = new WindowEventArgs(this);
				OnRenderingStarted(args);

				// HACK: ensure our rendered string content is up to date
				GetRenderedString();

				// get derived class or WindowRenderer to re-populate geometry buffer.
				if (d_windowRenderer != null)
					d_windowRenderer.CreateRenderGeometry();
				else
					PopulateGeometryBuffer();

				UpdateGeometryBuffersTranslationAndClipping();

				UpdateGeometryBuffersAlpha();

				// signal rendering ended
				args.handled = 0;
				OnRenderingEnded(args);

				// mark ourselves as no longer needed a redraw.
				d_needsRedraw = false;
			}
		}

		/// <summary>
		/// Perform drawing operations concerned with positioning, clipping and
		/// queueing of window geometry to RenderingSurfaces.
		/// </summary>
		/// <remarks>
		/// This function is a sub-function of drawSelf and is provided to make it
		/// easier to override drawSelf without needing to duplicate large sections
		/// of the code from the default implementation.
		/// </remarks>
		/// <param name="ctx"></param>
		protected void QueueGeometry(RenderingContext ctx)
		{
			// add geometry so that it gets drawn to the target surface.
			ctx.surface.AddGeometryBuffers(ctx.queue, d_geometryBuffers);
		}

		/// <summary>
		/// Update the rendering cache.
		/// 
		/// Populates the Window's GeometryBuffer ready for rendering.
		/// </summary>
		protected virtual void PopulateGeometryBuffer()
		{
		}

		/// <summary>
		/// Set the parent window for this window object.
		/// </summary>
		/// <param name="parent">
		/// Pointer to a Window object that is to be assigned as the parent to this
		/// Window.
		/// </param>
		protected override void SetParent(Element parent)
		{
			base.SetParent(parent);
			//SyncTargetSurface();
			OnTargetSurfaceChanged(GetTargetRenderingSurface());
		}

		/// <summary>
		/// Fires off a repeated mouse button down event for this window.
		/// </summary>
		/// <param name="source"></param>
		protected void GenerateAutoRepeatEvent(CursorInputSource source)
		{
			var @event = new CursorInputEventArgs(this)
			             {
				             Position = GetUnprojectedPosition(GetGUIContext().GetCursor().GetPosition()),
				             moveDelta = Lunatics.Mathematics.Vector2.Zero,
				             Source = source,
				             scroll = 0
			             };
			OnCursorPressHold(@event);
		}

		/// <summary>
		/// Function used in checking if a WindowRenderer is valid for this window.
		/// </summary>
		/// <param name="renderer">
		/// Window renderer that will be checked (it can be null!)
		/// </param>
		/// <returns>
		/// Returns true if the given WindowRenderer class name is valid for this window.
		/// False if not.
		/// </returns>
		protected virtual bool ValidateWindowRenderer(WindowRenderer renderer)
		{
			return true;
		}

		/// <summary>
		/// Returns whether a property is at it's default value.
		/// This function is different from Property::isDefatult as it takes the assigned look'n'feel
		/// (if the is one) into account.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected bool IsPropertyAtDefault(Property property)
		{
			// if we have a looknfeel we examine it for defaults
			if (!String.IsNullOrEmpty(d_lookName))
			{
				WidgetLookFeel wlf;
				PropertyInitialiser propinit;
				if (d_parent != null && !String.IsNullOrEmpty(GetParent().GetLookNFeel()))
				{
					wlf = WidgetLookManager.GetSingleton().GetWidgetLook(GetParent().GetLookNFeel());

					// if this property is a target of a PropertyLink, we always report
					// as being at default.  NB: This check is only performed on the
					// immediate parent.
					var pldl = wlf.GetPropertyLinkDefinitions();
					foreach (var i in pldl)
					{
						if (i.GetPropertyName() == property.GetName())
							return true;
					}

					// for an auto-window see if the property is is set via a Property
					// tag within the WidgetComponent that defines it.
					if (d_autoWindow)
					{
						// find the widget component if possible
						var wc = wlf.FindWidgetComponent(GetName());
						if (wc != null)
						{
							propinit = wc.FindPropertyInitialiser(property.GetName());

							if (propinit != null)
								return (GetProperty(property.GetName()) == propinit.GetInitialiserValue());
						}
					}
				}

				// if the looknfeel has a new default for this property we compare
				// against that
				wlf = WidgetLookManager.GetSingleton().GetWidgetLook(d_lookName);
				propinit = wlf.FindPropertyInitialiser(property.GetName());
				if (propinit != null)
					return (GetProperty(property.GetName()) == propinit.GetInitialiserValue());
			}

			// we don't have a looknfeel with a new value for this property so we rely
			// on the hardcoded default
			return property.IsDefault(this);
		}

		/// <summary>
		/// Recursively inform all children that the clipping has changed and screen rects
		/// needs to be recached.
		/// </summary>
		protected void NotifyClippingChanged()
		{
			MarkCachedWindowRectsInvalid();

			// inform children that their clipped screen areas must be updated
			var num = d_children.Count;
			for (var i = 0; i < num; ++i)
				if (GetChildAtIdx(i).IsClippedByParent())
					GetChildAtIdx(i).NotifyClippingChanged();
		}

		/// <summary>
		/// helper to create and setup the auto RenderingWindow surface
		/// </summary>
		/// <param name="addStencilBuffer"></param>
		protected void AllocateRenderingWindow(bool addStencilBuffer)
		{
			if (!d_autoRenderingWindow)
			{
				d_autoRenderingWindow = true;

				var t = System.GetSingleton().GetRenderer().CreateTextureTarget(addStencilBuffer);

				// TextureTargets may not be available, so check that first.
				if (t == null)
				{
					System.GetSingleton().Logger
					      .LogEvent(
						      "Window::allocateRenderingWindow - Failed to create a suitable TextureTarget for use by Window '" +
						      d_name + "'",
						      LoggingLevel.Errors);

					d_surface = null;
					return;
				}

				d_surface = GetTargetRenderingSurface().CreateRenderingWindow(t);
				TransferChildSurfaces();

				// set size and position of RenderingWindow
				((RenderingWindow) d_surface).SetSize(GetPixelSize());
				((RenderingWindow) d_surface).SetPosition(GetUnclippedOuterRect().Get().Position);

				GetGUIContext().MarkAsDirty();
			}
		}

		/// <summary>
		/// helper to clean up the auto RenderingWindow surface
		/// </summary>
		protected void ReleaseRenderingWindow()
		{
			if (d_autoRenderingWindow && d_surface != null)
			{
				var oldSurface = (RenderingWindow) d_surface;
				d_autoRenderingWindow = false;
				d_surface = null;

				// detach child surfaces prior to destroying the owning surface
				TransferChildSurfaces();

				// destroy surface and texture target it used
				var tt = oldSurface.GetTextureTarget();
				oldSurface.GetOwner().DestroyRenderingWindow(oldSurface);
				System.GetSingleton().GetRenderer().DestroyTextureTarget(tt);

				GetGUIContext().MarkAsDirty();
			}
		}

		/// <summary>
		/// Helper to intialise the needed clipping for geometry and render surface.
		/// </summary>
		/// <param name="ctx"></param>
		protected void InitialiseClippers(RenderingContext ctx)
		{
			if (ctx.surface.IsRenderingWindow() && ctx.owner == this)
			{
				var renderingWindow = (RenderingWindow) ctx.surface;

				if (d_clippedByParent && d_parent != null)
					renderingWindow.SetClippingRegion(GetParent().GetClipRect(d_nonClient));
				else
					renderingWindow.SetClippingRegion(new Rectf(Lunatics.Mathematics.Vector2.Zero, GetRootContainerSize()));

				d_clippingRegion = new Rectf(Lunatics.Mathematics.Vector2.Zero, d_pixelSize);
			}
			else
			{
				var geoClip = GetOuterRectClipper();

				if (geoClip.Width != 0.0f && geoClip.Height != 0.0f)
					geoClip.Offset(new Lunatics.Mathematics.Vector2(-ctx.offset.X, -ctx.offset.Y));

				d_clippingRegion = /*Rectf(geo_clip);*/ geoClip;
			}
		}

		/// <summary>
		/// \copydoc Element.SetAreaImpl
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		/// <param name="topLeftSizing"></param>
		/// <param name="fireEvents"></param>
		protected override void SetAreaImpl(UVector2 pos, USize size, bool topLeftSizing = false, bool fireEvents = true)
		{
			MarkCachedWindowRectsInvalid();
			base.SetAreaImpl(pos, size, topLeftSizing, fireEvents);

			//if (moved || sized)
			// FIXME: This is potentially wasteful
			GetGUIContext().UpdateWindowContainingCursor();

			// update geometry position and clipping if nothing from above appears to
			// have done so already (NB: may be occasionally wasteful, but fixes bugs!)
			if (!d_unclippedOuterRect.IsCacheValid())
				UpdateGeometryRenderSettings();
		}

		/// <summary>
		/// Cleanup child windows
		/// </summary>
		protected virtual void CleanupChildren()
		{
			while (GetChildCount() != 0)
			{
				var wnd = (Window) d_children[0];

				// always remove child
				RemoveChild(wnd);

				// destroy child if that is required
				if (wnd.IsDestroyedByParent())
					WindowManager.GetSingleton().DestroyWindow(wnd);
			}
		}

		/// <summary>
		/// \copydoc Element::addChild_impl
		/// </summary>
		/// <param name="element"></param>
		protected override void AddChildImpl(Element element)
		{
			var wnd = element as Window;

			if (wnd == null)
				throw new InvalidRequestException("Window can only have Elements of type Window added as children (Window path: " +
				                                  GetNamePath() + ").");

			// if the element is already a child of this Window, this is a NOOP
			if (IsChild(element))
				return;

			base.AddChildImpl(wnd);

			AddWindowToDrawList(wnd);

			wnd.Invalidate(true);

			wnd.OnZChangeImpl();
		}

		/// <summary>
		/// \copydoc Element::removeChild_impl
		/// </summary>
		/// <param name="element"></param>
		protected override void RemoveChildImpl(Element element)
		{
			var wnd = (Window) element;

			var captureWnd = GetCaptureWindow();

			if ((captureWnd != null && wnd != null) &&
			    (captureWnd == wnd || captureWnd.IsAncestor(wnd)))
				GetCaptureWindow().ReleaseInput();

			// remove from draw list
			RemoveWindowFromDrawList(wnd);

			base.RemoveChildImpl(wnd);

			// find this window in the child list
			if (d_children.Contains(wnd))
			{
				// if the window was found in the child list
				// unban properties window could write as a root window
				wnd.UnbanPropertyFromXML("RestoreOldCapture");
			}

			wnd.OnZChangeImpl();

			// Removed windows should not be active anymore (they are not attached
			// to anything so this would not make sense)
			if (wnd.IsActive())
				wnd.Deactivate();
		}

		/// <summary>
		/// Notify 'this' and all siblings of a ZOrder change event
		/// </summary>
		protected virtual void OnZChangeImpl()
		{
			if (d_parent == null)
			{
				OnZChanged(new WindowEventArgs(this));
			}
			else
			{
				var childCount = d_parent.GetChildCount();

				for (var i = 0; i < childCount; ++i)
				{
					var args = new WindowEventArgs(GetParent().GetChildAtIdx(i));
					GetParent().GetChildAtIdx(i).OnZChanged(args);
				}
			}

			GetGUIContext().UpdateWindowContainingCursor();
		}

		/// <summary>
		/// Add standard CEGUI::Window properties.
		/// </summary>
		protected void AddWindowProperties()
		{
			DefineProperty(
				"Alpha", "Property to get/set the alpha value of the Window. Value is floating point number.",
				(x, v) => x.SetAlpha(v), x => x.GetAlpha(), 1.0f);

			DefineProperty(
				"AlwaysOnTop",
				"Property to get/set the 'always on top' setting for the Window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetAlwaysOnTop(v), x => x.IsAlwaysOnTop(), false);

			DefineProperty(
				"ClippedByParent",
				"Property to get/set the 'clipped by parent' setting for the Window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetClippedByParent(v), x => x.IsClippedByParent(), true);

			DefineProperty(
				"DestroyedByParent",
				"Property to get/set the 'destroyed by parent' setting for the Window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetDestroyedByParent(v), x => x.IsDestroyedByParent(), true);

			DefineProperty(
				"Disabled",
				"Property to get/set the 'disabled state' setting for the Window.  Value is either \"True\" or \"False\".",
				(x, v) => x.SetDisabled(v), x => x.IsDisabled(), false);

			DefineProperty(
				"Font",
				"Property to get/set the font for the Window.  Value is the name of the font to use (must be loaded already).",
				(x, v) => x.SetFont(v), x => x.GetFont(false), null);

			DefineProperty(
				"ID", "Property to get/set the ID value of the Window. Value is an unsigned integer number.",
				(x, v) => x.SetId(v), x => x.GetId(), 0);

			DefineProperty(
				"InheritsAlpha",
				"Property to get/set the 'inherits alpha' setting for the Window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetInheritsAlpha(v), x => x.InheritsAlpha(), true);

			DefineProperty(
				CursorImagePropertyName,
				"Property to get/set the mouse cursor image for the Window.  Value should be \"<image name>\".",
				(x, v) => x.SetCursor(v), x => x.GetCursor(false), null);

			DefineProperty(
				"Visible",
				"Property to get/set the 'visible state' setting for the Window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetVisible(v), x => x.IsVisible(), true);

			DefineProperty(
				"Active",
				"Property to get/set the 'active' setting for the Window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetActive(v), x => x.IsActive(), true);

			DefineProperty(
				"RestoreOldCapture",
				"Property to get/set the 'restore old capture' setting for the Window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetRestoreOldCapture(v), x => x.RestoresOldCapture(), false);

			DefineProperty(
				"Text",
				"Property to get/set the text / caption for the Window. Value is the text string to use. Meaning of this property heavily depends on the type of the Window.",
				(x, v) => x.SetText(v), x => x.GetText(), String.Empty);

			DefineProperty(
				"ZOrderingEnabled",
				"Property to get/set the 'z-order changing enabled' setting for the Window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetZOrderingEnabled(v), x => x.IsZOrderingEnabled(), true);

			DefineProperty(
				"WantsMultiClickEvents",
				"Property to get/set whether the window will receive double-click and triple-click events. Value is either \"True\" or \"False\".",
				(x, v) => x.SetWantsMultiClickEvents(v), x => x.WantsMultiClickEvents(), true);

			DefineProperty(
				"MouseAutoRepeatEnabled",
				"Property to get/set whether the window will receive autorepeat mouse button down events. Value is either \"True\" or \"False\".",
				(x, v) => x.SetMouseAutoRepeatEnabled(v), x => x.IsMouseAutoRepeatEnabled(), false);

			DefineProperty(
				"AutoRepeatDelay",
				"Property to get/set the autorepeat delay. Value is a floating point number indicating the delay required in seconds.",
				(x, v) => x.SetAutoRepeatDelay(v), x => x.GetAutoRepeatDelay(), 0.3f);

			DefineProperty(
				"AutoRepeatRate",
				"Property to get/set the autorepeat rate. Value is a floating point number indicating the rate required in seconds.",
				(x, v) => x.SetAutoRepeatRate(v), x => x.GetAutoRepeatRate(), 0.06f);

			DefineProperty(
				"DistributeCapturedInputs",
				"Property to get/set whether captured inputs are passed to child windows. Value is either \"True\" or \"False\".",
				(x, v) => x.SetDistributesCapturedInputs(v), x => x.DistributesCapturedInputs(), false);

			DefineProperty(
				"TooltipType",
				"Property to get/set the custom tooltip for the window. Value is the type name of the custom tooltip. If \"\", the default System tooltip is used.",
				(x, v) => x.SetTooltipType(v), x => x.GetTooltipType(), String.Empty);

			DefineProperty(
				"TooltipText",
				"Property to get/set the tooltip text for the window. Value is the tooltip text for the window.",
				(x, v) => x.SetTooltipText(v), x => x.GetTooltipText(), String.Empty);

			DefineProperty(
				"InheritsTooltipText",
				"Property to get/set whether the window inherits its parents tooltip text when it has none of its own. Value is either \"True\" or \"False\".",
				(x, v) => x.SetInheritsTooltipText(v), x => x.InheritsTooltipText(), true);

			DefineProperty(
				"RiseOnClickEnabled",
				"Property to get/set whether the window will come to the top of the Z-order when clicked. Value is either \"True\" or \"False\".",
				(x, v) => x.SetRiseOnClickEnabled(v), x => x.IsRiseOnClickEnabled(), true);

			DefineProperty(
				CursorPassThroughEnabledPropertyName,
				"Property to get/set whether the window ignores mouse events and pass them through to any windows behind it. Value is either \"True\" or \"False\".",
				(x, v) => x.SetCursorPassThroughEnabled(v), x => x.IsCursorPassThroughEnabled(), false);

			// TODO: addProperty(&d_windowRendererProperty);
			// TODO: addProperty(&d_lookNFeelProperty);

			DefineProperty(
				"DragDropTarget",
				"Property to get/set whether the Window will receive drag and drop related notifications.  Value is either \"True\" or \"False\".",
				(x, v) => x.SetDragDropTarget(v), x => x.IsDragDropTarget(), true);

			// TODO: Inconsistency
			DefineProperty(
				"AutoRenderingSurface",
				"Property to get/set whether the Window will automatically attempt to use a full imagery caching RenderingSurface (if supported by the renderer).  Here, full imagery caching usually will mean caching a window's representation onto a texture (although no such implementation requirement is specified.) Value is either \"True\" or \"False\".",
				(x, v) => x.SetUsingAutoRenderingSurface(v), x => x.IsUsingAutoRenderingSurface(), false);

			DefineProperty(
				"TextParsingEnabled",
				"Property to get/set the text parsing setting for the Window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetTextParsingEnabled(v), x => x.IsTextParsingEnabled(), true);

			DefineProperty(
				"Margin",
				"Property to get/set margin for the Window. Value format: {top:{[tops],[topo]},left:{[lefts],[lefto]},bottom:{[bottoms],[bottomo]},right:{[rights],[righto]}}.",
				(x, v) => x.SetMargin(v), x => x.GetMargin(), UBox.Zero);

			DefineProperty(
				"UpdateMode",
				"Property to get/set the window update mode setting. Value is one of \"Always\", \"Never\" or \"Visible\".",
				(x, v) => x.SetUpdateMode(v), x => x.GetUpdateMode(), WindowUpdateMode.WUM_VISIBLE);

			DefineProperty(
				"MouseInputPropagationEnabled",
				"Property to get/set whether unhandled mouse inputs should be propagated back to the Window's parent. Value is either \"True\" or \"False\".",
				(x, v) => x.SetCursorInputPropagationEnabled(v), x => x.IsMouseInputPropagationEnabled(), false);

			DefineProperty(
				"AutoWindow",
				"Property to access whether the system considers this window to be an automatically created sub-component window. Value is either \"True\" or \"False\".",
				(x, v) => x.SetAutoWindow(v), x => x.IsAutoWindow(), false);
		}

		private void DefineProperty<T>(string name, string help, Action<Window, T> setter, Func<Window, T> getter,
		                               T defaultValue)
		{
			const string propertyOrigin = "Window";
			AddProperty(new TplWindowProperty<Window, T>(name, help, setter, getter, propertyOrigin, defaultValue));
		}

		/// <summary>
		/// Implements move to front behavior.
		/// </summary>
		/// <param name="wasClicked"></param>
		/// <returns>
		/// Should return true if some action was taken, or false if there was
		/// nothing to be done.
		/// </returns>
		protected virtual bool MoveToFrontImpl(bool wasClicked)
		{
			var tookAction = false;

			// if the window has no parent then we can have no siblings
			if (d_parent == null)
			{
				// perform initial activation if required.
				if (!IsActive())
				{
					tookAction = true;
					OnActivated(new ActivationEventArgs(this) {otherWindow = null});
				}

				return tookAction;
			}

			// bring parent window to front of it's siblings
			tookAction = GetParent().MoveToFrontImpl(wasClicked);

			// get immediate child of parent that is currently active (if any)
			var activeWnd = GetActiveSibling();

			// if a change in active window has occurred
			if (activeWnd != this)
			{
				tookAction = true;

				// notify ourselves that we have become active
				var args = new ActivationEventArgs(this) {otherWindow = activeWnd};
				OnActivated(args);

				// notify any previously active window that it is no longer active
				if (activeWnd != null)
				{
					args.Window = activeWnd;
					args.otherWindow = this;
					args.handled = 0;
					activeWnd.OnDeactivated(args);
				}
			}

			// bring us to the front of our siblings
			if (d_zOrderingEnabled &&
			    (!wasClicked || d_riseOnClick) &&
			    !IsTopOfZOrder())
			{
				tookAction = true;

				// remove us from our parent's draw list
				GetParent().RemoveWindowFromDrawList(this);
				// re-attach ourselves to our parent's draw list which will move us in
				// front of sibling windows with the same 'always-on-top' setting as we
				// have.
				GetParent().AddWindowToDrawList(this);
				// notify relevant windows about the z-order change.
				OnZChangeImpl();
			}

			return tookAction;
		}

		/// <summary>
		/// Add the given window to the drawing list at an appropriate position for
		/// it's settings and the required direction.  Basically, when \a at_back
		/// is false, the window will appear in front of all other windows with the
		/// same 'always on top' setting.  When \a at_back is true, the window will
		/// appear behind all other windows wih the same 'always on top' setting.
		/// </summary>
		/// <param name="wnd">
		/// Window object to be added to the drawing list.
		/// </param>
		/// <param name="at_back">
		/// Indicates whether the window should be placed at the back of other
		/// windows in the same group. If this is false, the window is placed in
		/// front of other windows in the group.
		/// </param>
		protected void AddWindowToDrawList(Window wnd, bool at_back = false)
		{
			// add behind other windows in same group
			if (at_back)
			{
				// calculate position where window should be added for drawing
				var pos = 0;
				if (wnd.IsAlwaysOnTop())
				{
					// find first topmost window
					while ((pos != d_drawList.Count()) && (!d_drawList[pos].IsAlwaysOnTop()))
						++pos;
				}
				// add window to draw list
				d_drawList.Insert(pos, wnd);
			}
			// add in front of other windows in group
			else
			{
				// calculate position where window should be added for drawing
				var pos = d_drawList.Count;
				if (!wnd.IsAlwaysOnTop())
				{
					// find last non-topmost window
					while ((pos > 0) && (d_drawList[pos - 1].IsAlwaysOnTop()))
						--pos;
				}
				// add window to draw list
				d_drawList.Insert(pos, wnd);
			}
		}

		/// <summary>
		/// Removes the window from the drawing list. If the window is not attached
		/// to the drawing list then nothing happens.
		/// </summary>
		/// <param name="wnd">
		/// Window object to be removed from the drawing list.
		/// </param>
		protected void RemoveWindowFromDrawList(Window wnd)
		{
			// if draw list is not empty
			if (d_drawList.Count != 0)
			{
				// attempt to find the window in the draw list
				if (d_drawList.Contains(wnd))
				{
					// remove the window if it was found in the draw list
					d_drawList.Remove(wnd);
				}
			}
		}

		/// <summary>
		/// Return whether the window is at the top of the Z-Order.  This will
		/// correctly take into account 'Always on top' windows as needed.
		/// </summary>
		/// <returns>
		/// - true if the Window is at the top of the z-order in relation to sibling
		///   windows with the same 'always on top' setting.
		/// - false if the Window is not at the top of the z-order in relation to
		///   sibling windows with the same 'always on top' setting.
		/// </returns>
		protected bool IsTopOfZOrder()
		{
			// if not attached, then always on top!
			if (d_parent == null)
				return true;

			// get position of window at top of z-order in same group as this window
			var pos = GetParent().d_drawList.Count - 1;
			if (!d_alwaysOnTop)
			{
				// find last non-topmost window
				while ((pos > 0) && GetParent().d_drawList[pos].IsAlwaysOnTop())
					--pos;
			}

			// return whether the window at the top of the z order is us
			return GetParent().d_drawList[pos] == this;
		}

		/// <summary>
		/// Update position and clip region on this Windows geometry / rendering
		/// surface.
		/// </summary>
		protected void UpdateGeometryRenderSettings()
		{
			RenderingContext ctx;
			GetRenderingContext(out ctx);

			// move the underlying RenderingWindow if we're using such a thing
			if (ctx.owner == this && ctx.surface.IsRenderingWindow())
			{
				((RenderingWindow) ctx.surface).SetPosition(GetUnclippedOuterRect().Get().Position);
				((RenderingWindow) d_surface).SetPivot(new Lunatics.Mathematics.Vector3(d_pixelSize.Width / 2.0f,
				                                                                       d_pixelSize.Height / 2.0f,
				                                                                       0.0f));
				d_translation = Lunatics.Mathematics.Vector3.Zero;
			}
			// if we're not texture backed, update geometry position.
			else
			{
				// position is the offset of the window on the dest surface.
				var ucrect = GetUnclippedOuterRect().Get();
				d_translation = new Lunatics.Mathematics.Vector3(ucrect.d_min.X - ctx.offset.X,
				                                                ucrect.d_min.Y - ctx.offset.Y,
				                                                0.0f);
			}
			InitialiseClippers(ctx);
			UpdateGeometryBuffersTranslationAndClipping();
		}

		/// <summary>
		/// transfer RenderingSurfaces to be owned by our target RenderingSurface.
		/// </summary>
		protected void TransferChildSurfaces()
		{
			var s = GetTargetRenderingSurface();

			var childCount = GetChildCount();
			for (var i = 0; i < childCount; ++i)
			{
				var c = GetChildAtIdx(i);

				if (c.d_surface != null && c.d_surface.IsRenderingWindow())
					s.TransferRenderingWindow((RenderingWindow) c.d_surface);
				else
					c.TransferChildSurfaces();
			}
		}

		/// <summary>
		/// helper function for calculating clipping rectangles.
		/// </summary>
		/// <param name="unclippedArea"></param>
		/// <returns></returns>
		protected Rectf GetParentElementClipIntersection(Rectf unclippedArea)
		{
			return unclippedArea.GetIntersection(
				(d_parent != null && d_clippedByParent)
					? GetParent().GetClipRect(IsNonClient())
					: new Rectf(Lunatics.Mathematics.Vector2.Zero, GetRootContainerSize()));
		}

		/// <summary>
		/// helper function to invalidate window and optionally child windows.
		/// </summary>
		/// <param name="recursive"></param>
		protected void InvalidateImpl(bool recursive)
		{
			d_needsRedraw = true;
			InvalidateRenderingSurface();

			OnInvalidated(new WindowEventArgs(this));

			if (recursive)
			{
				var childCount = GetChildCount();
				for (var i = 0; i < childCount; ++i)
					GetChildAtIdx(i).InvalidateImpl(true);
			}
		}

		/// <summary>
		/// Helper function to return the ancestor Window of /a wnd that is attached
		/// as a child to a window that is also an ancestor of /a this.  Returns 0
		/// if /a wnd and /a this are not part of the same hierachy.
		/// </summary>
		/// <param name="wnd"></param>
		/// <returns></returns>
		protected Window GetWindowAttachedToCommonAncestor(Window wnd)
		{
			var w = wnd;
			var tmp = w.GetParent();

			while (tmp != null)
			{
				if (IsAncestor(tmp))
					break;

				w = tmp;
				tmp = tmp.GetParent();
			}

			return tmp != null ? w : null;
		}

		protected override Rectf GetUnclippedInnerRectImpl(bool skipAllPixelAlignment)
		{
			// TODO: skip all pixel alignment!
			return d_windowRenderer != null
				       ? d_windowRenderer.GetUnclippedInnerRect()
				       : ( /*skipAllPixelAlignment*/true
					                                    ? GetUnclippedOuterRect().GetFresh(true)
					                                    : GetUnclippedOuterRect().Get());
		}

		/// <summary>
		/// Default implementation of function to return Window outer clipper area.
		/// </summary>
		/// <returns></returns>
		protected virtual Rectf GetOuterRectClipperImpl()
		{
			return (d_surface != null && d_surface.IsRenderingWindow())
				       ? GetUnclippedOuterRect().Get()
				       : GetParentElementClipIntersection(GetUnclippedOuterRect().Get());
		}

		/// <summary>
		/// Default implementation of function to return Window inner clipper area.
		/// </summary>
		/// <returns></returns>
		protected virtual Rectf GetInnerRectClipperImpl()
		{
			return (d_surface != null && d_surface.IsRenderingWindow())
				       ? GetUnclippedInnerRect().Get()
				       : GetParentElementClipIntersection(GetUnclippedInnerRect().Get());
		}

		/// <summary>
		/// Default implementation of function to return Window hit-test area.
		/// </summary>
		/// <returns></returns>
		protected virtual Rectf GetHitTestRectImpl()
		{
			// if clipped by parent wnd, hit test area is the intersection of our
			// outer rect with the parent's hit test area intersected with the
			// parent's clipper.
			if (d_parent != null && d_clippedByParent)
			{
				return GetUnclippedOuterRect().Get().GetIntersection(
					GetParent().GetHitTestRect().GetIntersection(
						GetParent().GetClipRect(IsNonClient())));
			}

			// not clipped to parent wnd, so get intersection with screen area.
			return GetUnclippedOuterRect().Get().GetIntersection(
				new Rectf(Lunatics.Mathematics.Vector2.Zero, GetRootContainerSize()));
		}

		protected virtual int WritePropertiesXML(XMLSerializer xmlStream)
		{
			var propertiesWritten = 0;

			foreach (var iter in (IEnumerable<Property>) this)
			{
				// first we check to make sure the property is'nt banned from XML
				if (!IsPropertyBannedFromXML(iter))
				{
					try
					{
						// only write property if it's not at the default state
						if (!IsPropertyAtDefault(iter))
						{
							iter.WriteXMLToStream(this, xmlStream);
							++propertiesWritten;
						}
					}
					catch (InvalidRequestException)
					{
						// This catches errors from the MultiLineColumnList for example
						System.GetSingleton().Logger
						      .LogEvent("Window.WritePropertiesXML: property receiving failed.  Continuing...",
						                LoggingLevel.Errors);
					}
				}
			}

			return propertiesWritten;
		}

		protected virtual int WriteChildWindowsXML(XMLSerializer xmlStream)
		{
			var windowsWritten = 0;

			for (var i = 0; i < GetChildCount(); ++i)
			{
				var child = GetChildAtIdx(i);

				// conditional to ensure that auto created windows are handled
				// seperately.
				if (!child.IsAutoWindow())
				{
					child.WriteXMLToStream(xmlStream);
					++windowsWritten;
				}
				// this is one of those auto created windows so we do some special
				// handling
				else if (child.WriteAutoChildWindowXML(xmlStream))
				{
					++windowsWritten;
				}
			}

			return windowsWritten;
		}

		protected virtual bool WriteAutoChildWindowXML(XMLSerializer xml_stream)
		{
			// just stop now if we are'nt allowed to write XML
			if (!d_allowWriteXML)
				return false;

			// we temporarily output to this string stream to see if have to do the tag
			// at all.  Make sure this stream does UTF-8
			var ss = new StreamWriter(new MemoryStream());
			var xml = new XMLSerializer(ss);
			xml.OpenTag(AutoWindowXMLElementName);
			// Create the XML Child Tree
			// write out properties.
			WritePropertiesXML(xml);
			// write out attached child windows.
			WriteChildWindowsXML(xml);
			xml.CloseTag();
			if (xml.GetTagCount() <= 1)
				return false;

			// output opening AutoWindow tag
			xml_stream.OpenTag(AutoWindowXMLElementName);
			// write name suffix attribute
			xml_stream.Attribute(AutoWindowNamePathXMLAttributeName, GetName());
			// Inefficient : do the XML serialization again
			// write out properties.
			WritePropertiesXML(xml_stream);
			// write out attached child windows.
			WriteChildWindowsXML(xml_stream);
			xml_stream.CloseTag();

			return true;
		}

		protected virtual void BanPropertiesForAutoWindow()
		{
			BanPropertyFromXML("AutoWindow"); // :-D
			BanPropertyFromXML("DestroyedByParent");
			BanPropertyFromXML("VerticalAlignment");
			BanPropertyFromXML("HorizontalAlignment");
			BanPropertyFromXML("Area");
			BanPropertyFromXML("Position");
			BanPropertyFromXML("Size");
			BanPropertyFromXML("MinSize");
			BanPropertyFromXML("MaxSize");
			// TODO: BanPropertyFromXML(d_windowRendererProperty);
			// TODO: BanPropertyFromXML(d_lookNFeelProperty);
		}

		/// <summary>
		/// Handler function for when font render size changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		protected virtual /*bool*/ void HandleFontRenderSizeChange(object sender, FontEventArgs args)
		{
			if (d_windowRenderer == null)
				return /*false*/;

			var result = d_windowRenderer.HandleFontRenderSizeChange(args.font);
			// TODO: return result;
		}

		/// <summary>
		/// mark the rect caches defined on Window invalid 
		/// (does not affect Element) 
		/// </summary>
		protected void MarkCachedWindowRectsInvalid()
		{
			d_outerRectClipperValid = false;
			d_innerRectClipperValid = false;
			d_hitTestRectValid = false;
		}

		protected void LayoutLookNFeelChildWidgets()
		{
			if (String.IsNullOrEmpty(d_lookName))
				return;

			try
			{
				WidgetLookManager.GetSingleton()
				                 .GetWidgetLook(d_lookName)
				                 .LayoutChildWidgets(this);
			}
			catch (UnknownObjectException)
			{
				System.GetSingleton().Logger
				      .LogEvent("Window::layoutLookNFeelChildWidgets: WidgetLook '" + d_lookName + "' was not found.",
				                LoggingLevel.Errors);
			}
		}

		#endregion

		protected Window GetChildAtPosition(Lunatics.Mathematics.Vector2 position,
		                                    Func<Window, Lunatics.Mathematics.Vector2, bool, bool> hittestfunc,
		                                    bool allow_disabled = false)
		{
			Lunatics.Mathematics.Vector2 p;
			// if the window has RenderingWindow backing
			if (d_surface != null && d_surface.IsRenderingWindow())
				((RenderingWindow) d_surface).UnprojectPoint(position, out p);
			else
				p = position;

			foreach (var child in ((IEnumerable<Window>) d_drawList).Reverse())
			{
				if (child.IsEffectiveVisible())
				{
					// recursively scan for hit on children of this child window...
					var wnd = child.GetChildAtPosition(p, hittestfunc, allow_disabled);

					// return window pointer if we found a hit down the chain somewhere
					if (wnd != null)
						return wnd;

					// see if this child is hit and return it's pointer if it is
					if (hittestfunc(child, p, allow_disabled))
						return child;
				}
			}

			// nothing hit
			return null;

		}

		protected bool IsHitTargetWindow(Lunatics.Mathematics.Vector2 position, bool allowDisabled)
		{
			return !IsCursorPassThroughEnabled() && IsHit(position, allowDisabled);
		}

		private void DestroyGeometryBuffers()
		{
			var renderer = System.GetSingleton().GetRenderer();
			foreach (var geometryBuffer in d_geometryBuffers)
				renderer.DestroyGeometryBuffer(geometryBuffer);

			d_geometryBuffers.Clear();
		}

		private void UpdateGeometryBuffersTranslationAndClipping()
		{
			foreach (var geometryBuffer in d_geometryBuffers)
			{
				geometryBuffer.SetTranslation(d_translation);
				geometryBuffer.SetClippingRegion(d_clippingRegion);
			}
		}

		private void UpdateGeometryBuffersAlpha()
		{
			var finalAlpha = GetEffectiveAlpha();
			foreach (var geometryBuffer in d_geometryBuffers)
				geometryBuffer.SetAlpha(finalAlpha);
		}


		/*************************************************************************
		    Properties for Window base class
		*************************************************************************/

		///*!
		//\brief
		//    Property to access/change the assigned window renderer object.

		//    \par Usage:
		//        - Name: WindowRenderer
		//        - Format: "[windowRendererName]"

		//    \par Where [windowRendererName] is the factory name of the window
		//         renderer type you wish to assign.
		//*/
		//static class WindowRendererProperty : public TplWindowProperty<Window, String>
		//{
		//public:
		//    WindowRendererProperty();
		//    void writeXMLToStream(const PropertyReceiver* receiver, XMLSerializer& xml_stream) const;
		//} d_windowRendererProperty;

		///*!
		//\brief
		//    Property to access/change the assigned look'n'feel.

		//    \par Usage:
		//        - Name: LookNFeel
		//        - Format: "[LookNFeelName]"

		//    \par Where [LookNFeelName] is the name of the look'n'feel you wish
		//         to assign.
		//*/
		//static class LookNFeelProperty : public TplWindowProperty<Window, String>
		//{
		//public:
		//    LookNFeelProperty();
		//    void writeXMLToStream(const PropertyReceiver* receiver,
		//                          XMLSerializer& xml_stream) const;
		//} d_lookNFeelProperty;

		/*************************************************************************
		    Implementation Data
		*************************************************************************/
		//! definition of type used for the list of child windows to be drawn
		//typedef std::vector<Window*, CEGUI_VECTOR_ALLOC(Window*)> ChildDrawList;
		//! definition of type used for the UserString dictionary.
		//typedef std::map<String, String, StringFastLessCompare,CEGUI_MAP_ALLOC(String, String)> UserStringMap;
		//! definition of type used to track properties banned from writing XML.
		//typedef std::set<String, StringFastLessCompare, CEGUI_SET_ALLOC(String)> BannedXMLPropertySet;

		//! type of Window (also the name of the WindowFactory that created us)
		protected string d_type;

		//! Type name of the window as defined in a Falagard mapping.
		protected internal string d_falagardType;

		//! true when this window is an auto-window
		protected bool d_autoWindow;

		//! true when this window is currently being initialised (creating children etc)
		protected bool d_initialising;

		//! true when this window is being destroyed.
		protected bool d_destructionStarted;

		//! true when Window is enabled
		protected bool d_enabled;

		//! is window visible (i.e. it will be rendered, but may still be obscured)
		protected bool d_visible;

		//! true when Window is the active Window (receiving inputs).
		private bool d_active;

		//! Child window objects arranged in rendering order.
		protected List<Window> d_drawList = new List<Window>();

		//! true when Window will be auto-destroyed by parent.
		protected bool d_destroyedByParent;

		//! true when Window will be clipped by parent Window area Rect.
		protected bool d_clippedByParent;

		//! Name of the Look assigned to this window (if any).
		protected string d_lookName;

		//! The WindowRenderer module that implements the Look'N'Feel specification
		protected WindowRenderer d_windowRenderer;

		//! Object which acts as a cache of geometry drawn by this Window.
		protected List<GeometryBuffer> d_geometryBuffers = new List<GeometryBuffer>();

		//! RenderingSurface owned by this window (may be 0)
		protected RenderingSurface d_surface;

		//! true if window geometry cache needs to be regenerated.
		protected bool d_needsRedraw;

		//! holds setting for automatic creation of of surface (RenderingWindow)
		protected bool d_autoRenderingWindow;

		//! holds setting for stencil buffer usage in texture caching
		protected bool d_autoRenderingSurfaceStencilEnabled;

		//! Holds pointer to the Window objects current mouse cursor image.
		protected Image d_mouseCursor;

		//! Alpha transparency setting for the Window
		protected float d_alpha;

		//! true if the Window inherits alpha from the parent Window
		protected bool d_inheritsAlpha;

		//! The Window that previously had capture (used for restoreOldCapture mode)
		protected Window d_oldCapture;

		//! Restore capture to the previous capture window when releasing capture.
		protected bool d_restoreOldCapture;

		//! Whether to distribute captured inputs to child windows.
		protected bool d_distCapturedInputs;

		//! Holds pointer to the Window objects current Font.
		protected Font d_font;

		//! Holds the text / label / caption for this Window.
		protected string d_textLogical = String.Empty;

		//! pointer to bidirection support object
		protected BidiVisualMapping d_bidiVisualMapping;

		//! whether bidi visual mapping has been updated since last text change.
		protected bool d_bidiDataValid;

		//! RenderedString representation of text string as ouput from a parser.
		protected RenderedString d_renderedString;

		//! true if d_renderedString is valid, false if needs re-parse.
		protected bool d_renderedStringValid;

		//! Shared instance of a parser to be used in most instances.
		protected static BasicRenderedStringParser d_basicStringParser = new BasicRenderedStringParser();

		//! Shared instance of a parser to be used when rendering text verbatim.
		protected static DefaultRenderedStringParser d_defaultStringParser = new DefaultRenderedStringParser();

		//! Pointer to a custom (user assigned) RenderedStringParser object.
		protected RenderedStringParser d_customStringParser;

		//! true if use of parser other than d_defaultStringParser is enabled
		protected bool d_textParsingEnabled;

		//! Margin, only used when the Window is inside LayoutContainer class
		protected UBox d_margin;

		/// <summary>
		/// User ID assigned to this Window
		/// </summary>
		protected int d_Id;

		//! Holds pointer to some user assigned data.
		protected object d_userData;

		/// <summary>
		/// Holds a collection of named user string values.
		/// </summary>
		protected Dictionary<string, string> d_userStrings = new Dictionary<string, string>();

		//! true if Window will be drawn on top of all other Windows
		protected bool d_alwaysOnTop;

		//! whether window should rise in the z order when left clicked.
		protected bool d_riseOnClick;

		//! true if the Window responds to z-order change requests.
		protected bool d_zOrderingEnabled;

		//! true if the Window wishes to hear about multi-click mouse events.
		protected bool d_wantsMultiClicks;

		//! whether (most) mouse events pass through this window
		protected bool _cursorPassThroughEnabled;

		//! whether pressed mouse button will auto-repeat the down event.
		protected bool d_autoRepeat;

		//! seconds before first repeat event is fired
		protected float d_repeatDelay;

		//! seconds between further repeats after delay has expired.
		protected float d_repeatRate;

		//! Cursor source we're tracking for auto-repeat purposes.
		CursorInputSource d_repeatPointerSource;

		//! implements repeating - is true after delay has elapsed,
		protected bool d_repeating;

		//! implements repeating - tracks time elapsed.
		protected float d_repeatElapsed;

		//! true if window will receive drag and drop related notifications
		protected bool d_dragDropTarget;

		//! Text string used as tip for this window.
		protected string d_tooltipText = String.Empty;

		//! Possible custom Tooltip for this window.
		protected Tooltip d_customTip;

		//! true if this Window created the custom Tooltip.
		protected bool d_weOwnTip;

		//! whether tooltip text may be inherited from parent.
		protected bool d_inheritsTipText;

		//! true if this window is allowed to write XML, false if not
		protected bool d_allowWriteXML;

		//! collection of properties not to be written to XML for this window.
		protected List<string> d_bannedXMLProperties = new List<string>();

		//! outer area clipping rect in screen pixels
		protected Rectf d_outerRectClipper;

		//! inner area clipping rect in screen pixels
		protected Rectf d_innerRectClipper;

		//! area rect used for hit-testing against this window
		protected Rectf d_hitTestRect;

		protected bool d_outerRectClipperValid;

		protected bool d_innerRectClipperValid;

		protected bool d_hitTestRectValid;

		//! The mode to use for calling Window::update
		protected WindowUpdateMode d_updateMode;

		//! specifies whether mouse inputs should be propagated to parent(s)
		protected bool d_propagatePointerInputs;

		//! GUIContext.  Set when this window is used as a root window.
		protected GUIContext d_guiContext;

		//! true when mouse is contained within this Window's area.
		protected bool d_containsMouse;

		//! The translation which was set for this window.
		protected Lunatics.Mathematics.Vector3 d_translation;

		//! true when this window is focused.
		protected bool d_isFocused;

		//! The clipping region which was set for this window.
		protected Rectf d_clippingRegion;

		//private:
		///*************************************************************************
		//    May not copy or assign Window objects
		//*************************************************************************/
		//Window(const Window&): NamedElement() {}
		//Window& operator=(const Window&) {return *this;}

		////! Not intended for public use, only used as a "Font" property getter
		//const Font* property_getFont() const;

		////! Not intended for public use, only used as a "Cursor" property getter
		//const Image* property_getMouseCursor() const;

		////! connection for event listener for font render size changes.
		//Event::ScopedConnection d_fontRenderSizeChangeConnection;

		protected void FireEvent<T>(EventHandler<T> eventHandler, T args) where T : EventArgs
		{
			var handler = eventHandler;
			if (handler != null)
			{
				foreach (var @delegate in handler.GetInvocationList())
					@delegate.DynamicInvoke(this, args);
				// handler(this, args);
			}
		}
	}
}