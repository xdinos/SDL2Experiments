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

using SharpCEGui.Base;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// StaticText class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled                     - basic rendering for enabled state.
    ///     - Disabled                    - basic rendering for disabled state.
    ///     - EnabledFrame                - frame rendering for enabled state
    ///     - DisabledFrame               - frame rendering for disabled state.
    ///     - WithFrameEnabledBackground  - backdrop rendering for enabled state with frame enabled.
    ///     - WithFrameDisabledBackground - backdrop rendering for disabled state with frame enabled.
    ///     - NoFrameEnabledBackground    - backdrop rendering for enabled state with frame disabled.
    ///     - NoFrameDisabledBackground   - backdrop rendering for disabled state with frame disabled.
    /// 
    /// Named Areas (missing areas will default to 'WithFrameTextRenderArea'):
    ///     WithFrameTextRenderArea
    ///     WithFrameTextRenderAreaHScroll
    ///     WithFrameTextRenderAreaVScroll
    ///     WithFrameTextRenderAreaHVScroll
    ///     NoFrameTextRenderArea
    ///     NoFrameTextRenderAreaHScroll
    ///     NoFrameTextRenderAreaVScroll
    /// NoFrameTextRenderAreaHVScroll
    /// </summary>
    public class FalagardStaticText : FalagardStatic
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public new const string TypeName = "Core/StaticText";

        #region Constants

        /// <summary>
        /// Widget name for the vertical scrollbar component.
        /// </summary>
        public const string VertScrollbarName = "__auto_vscrollbar__";

        /// <summary>
        /// Widget name for the horizontal scrollbar component.
        /// </summary>
        public const string HorzScrollbarName = "__auto_hscrollbar__";

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardStaticText(string type)
            : base(type)
        {
            _horzFormatting = HorizontalTextFormatting.LeftAligned;
            _vertFormatting = VerticalTextFormatting.CentreAligned;
            _textCols = new ColourRect(0xFFFFFFFF);
            _enableVertScrollbar = false;
            _enableHorzScrollbar = false;
            _formattedRenderedString = null;
            _formatValid = false;
            
            DefineProperty(
                "TextColours",
                "Property to get/set the text colours for the FalagardStaticText widget. Value is \"tl:[aarrggbb] tr:[aarrggbb] bl:[aarrggbb] br:[aarrggbb]\".",
                (w, v) => w.SetTextColours(v), w => w.GetTextColours(), new ColourRect(0xFFFFFFFF));

            DefineProperty(
                "HorzFormatting",
                "Property to get/set the horizontal formatting mode. Value is one of the HorzFormatting strings.",
                (w, v) => w.SetHorizontalFormatting(v), w => w.GetHorizontalFormatting(),
                HorizontalTextFormatting.LeftAligned);

            DefineProperty(
                "VertFormatting",
                "Property to get/set the vertical formatting mode. Value is one of the VertFormatting strings.",
                (w, v) => w.SetVerticalFormatting(v), w => w.GetVerticalFormatting(),
                VerticalTextFormatting.CentreAligned);

            DefineProperty(
                "VertScrollbar",
                "Property to get/set the setting for the vertical scroll bar. Value is either \"True\" or \"False\".",
                (w, v) => w.SetVerticalScrollbarEnabled(v), w => w.IsVerticalScrollbarEnabled(), false);

            DefineProperty(
                "HorzScrollbar",
                "Property to get/set the setting for the horizontal scroll bar. Value is either \"True\" or \"False\".",
                (w, v) => w.SetHorizontalScrollbarEnabled(v), w => w.IsHorizontalScrollbarEnabled(), false);

            DefineProperty(
                "HorzExtent",
                "Property to get the current horizontal extent of the formatted text string. Value is a float indicating the pixel extent.",
                null, w => w.GetHorizontalTextExtent(), 0);

            DefineProperty(
                "VertExtent",
                "Property to get the current vertical extent of the formatted text string. Value is a float indicating the pixel extent.",
                null, w => w.GetVerticalTextExtent(), 0);
        }

        private void DefineProperty<T>(string name, string help, System.Action<FalagardStaticText, T> setter,
                                       System.Func<FalagardStaticText, T> getter, T defaultValue)
        {
            RegisterProperty(new TplWindowRendererProperty<FalagardStaticText, T>(
                                 name, help, setter, getter, TypeName, defaultValue));
        }

        #endregion

        // TODO: Destructor.
        // TODO: ~FalagardStaticText() { delete d_formattedRenderedString; }

        /// <summary>
        /// Return a ColourRect object containing the colours used when rendering this widget.
        /// </summary>
        /// <returns></returns>
        public ColourRect GetTextColours()
        {
            return _textCols;
        }

        /// <summary>
        /// Return the current horizontal formatting option set for this widget.
        /// </summary>
        /// <returns></returns>
        public HorizontalTextFormatting GetHorizontalFormatting()
        {
            return _horzFormatting;
        }

        /// <summary>
        /// Return the current vertical formatting option set for this widget.
        /// </summary>
        /// <returns></returns>
        public VerticalTextFormatting GetVerticalFormatting()
        {
            return _vertFormatting;
        }

        /// <summary>
        /// Sets the colours to be applied when rendering the text.
        /// </summary>
        /// <param name="colours"></param>
        public void SetTextColours(ColourRect colours)
        {
            _textCols = colours;
            Window.Invalidate(false);
        }

        /// <summary>
        /// Set the vertical formatting required for the text.
        /// </summary>
        /// <param name="value"></param>
        public void SetVerticalFormatting(VerticalTextFormatting value)
        {
            _vertFormatting = value;
            ConfigureScrollbars();
            Window.Invalidate(false);
        }

        /// <summary>
        /// Set the horizontal formatting required for the text.
        /// </summary>
        /// <param name="value"></param>
        public void SetHorizontalFormatting(HorizontalTextFormatting value)
        {
            if (value == _horzFormatting)
                return;

            _horzFormatting = value;
            SetupStringFormatter();
            ConfigureScrollbars();
            Window.Invalidate(false);
        }

        /// <summary>
        /// Return whether the vertical scroll bar is set to be shown if needed.
        /// </summary>
        /// <returns></returns>
        public bool IsVerticalScrollbarEnabled()
        {
            return _enableVertScrollbar;
        }

        /// <summary>
        /// Return whether the horizontal scroll bar is set to be shown if needed.
        /// </summary>
        /// <returns></returns>
        public bool IsHorizontalScrollbarEnabled()
        {
            return _enableHorzScrollbar;
        }

        /// <summary>
        /// Set whether the vertical scroll bar will be shown if needed.
        /// </summary>
        /// <param name="setting"></param>
        public void SetVerticalScrollbarEnabled(bool setting)
        {
            _enableVertScrollbar = setting;
            ConfigureScrollbars();
            Window.PerformChildWindowLayout();
            _formatValid = false;
            Window.Invalidate(false);
        }

        /// <summary>
        /// Set whether the horizontal scroll bar will be shown if needed.
        /// </summary>
        /// <param name="setting"></param>
        public void SetHorizontalScrollbarEnabled(bool setting)
        {
            _enableHorzScrollbar = setting;
            ConfigureScrollbars();
            Window.PerformChildWindowLayout();
            _formatValid = false;
            Window.Invalidate(false);
        }

        /// <summary>
        /// return the current horizontal formatted text extent in pixels.
        /// </summary>
        /// <returns></returns>
        public float GetHorizontalTextExtent()
        {
            if (!_formatValid)
                UpdateFormatting();

            return _formattedRenderedString != null
                       ? _formattedRenderedString.GetHorizontalExtent(Window)
                       : 0.0f;
        }

        /// <summary>
        /// return the current vertical formatted text extent in pixels.
        /// </summary>
        /// <returns></returns>
        public float GetVerticalTextExtent()
        {
            if (!_formatValid)
                UpdateFormatting();

            return _formattedRenderedString != null
                       ? _formattedRenderedString.GetVerticalExtent(Window)
                       : 0.0f;
        }

        // overridden from base class
        public override bool HandleFontRenderSizeChange(Font font)
        {
            var res = base.HandleFontRenderSizeChange(font);

            if (Window.GetFont() == font)
            {
                Window.Invalidate(false);
                _formatValid = false;
                return true;
            }

            return res;
        }

        public override void CreateRenderGeometry()
        {
            // base class rendering
            base.CreateRenderGeometry();

            AddScrolledTextRenderGeometry();
        }

        /// <summary>
        /// update string formatting (gets area size to use from looknfeel)
        /// </summary>
        protected void UpdateFormatting()
        {
            UpdateFormatting(GetTextRenderArea().Size);
        }

        /// <summary>
        /// update string formatting using given area size.
        /// </summary>
        /// <param name="sz"></param>
        protected void UpdateFormatting(Sizef sz)
        {
            if (Window == null)
                return;

            if (_formattedRenderedString == null)
                SetupStringFormatter();

            // 'touch' the window's rendered string to ensure it's re-parsed if needed.
            _formattedRenderedString.SetRenderedString(Window.GetRenderedString());

            _formattedRenderedString.Format(Window, sz);
            _formatValid = true;

        }

        // overridden from FalagardStatic base class
        protected override void OnLookNFeelAssigned()
        {
            // do initial scrollbar setup
            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

            vertScrollbar.Hide();
            horzScrollbar.Hide();

            Window.PerformChildWindowLayout();

            // scrollbar events
            vertScrollbar.ScrollPositionChanged += HandleScrollbarChange;
            horzScrollbar.ScrollPositionChanged += HandleScrollbarChange;

            // events that scrollbars should react to
            Window.TextChanged += OnTextChanged;
            Window.Sized += OnSized;
            Window.FontChanged += OnFontChanged;
            Window.Scroll += OnScroll;
        }

        protected override void OnLookNFeelUnassigned()
        {
            Window.TextChanged -= OnTextChanged;
            Window.Sized -= OnSized;
            Window.FontChanged -= OnFontChanged;
            Window.Scroll -= OnScroll;
        }

        protected void InvalidateFormatting()
        {
            _formatValid = false;
            Window.Invalidate(false);
        }

        /// <summary>
        /// Adds the render geometry for scrolled text to the Window
        /// </summary>
        protected void AddScrolledTextRenderGeometry()
        {
            UpdateFormatting();

            // get destination area for the text.
            var clipper = GetTextRenderArea();
            var absarea = clipper;

            if (!_formatValid)
                UpdateFormatting(clipper.Size);

            // see if we may need to adjust horizontal position
            var horzScrollbar = GetHorzScrollbar();
            if (horzScrollbar.IsEffectiveVisible())
            {
                var range = horzScrollbar.GetDocumentSize() - horzScrollbar.GetPageSize();

                switch (_horzFormatting/*TODO:GetActualHorizontalFormatting()*/)
                {
                    case HorizontalTextFormatting.LeftAligned:
                    case HorizontalTextFormatting.WordWrapLeftAligned:
                    case HorizontalTextFormatting.Justified:
                    case HorizontalTextFormatting.WordWrapJustified:
                        absarea.Offset(new Lunatics.Mathematics.Vector2(-horzScrollbar.GetScrollPosition(), 0));
                        break;

                    case HorizontalTextFormatting.CentreAligned:
                    case HorizontalTextFormatting.WordWrapCentreAligned:
                        absarea.Width = horzScrollbar.GetDocumentSize();
                        absarea.Offset(new Lunatics.Mathematics.Vector2(range / 2 - horzScrollbar.GetScrollPosition(), 0));
                        break;

                    case HorizontalTextFormatting.RightAligned:
                    case HorizontalTextFormatting.WordWrapRightAligned:
                        absarea.Offset(new Lunatics.Mathematics.Vector2(range - horzScrollbar.GetScrollPosition(), 0));
                        break;

                    default:
                        throw new InvalidRequestException("Invalid actual horizontal formatting.");
                }
            }

            // adjust y positioning according to formatting option
            var textHeight = _formattedRenderedString.GetVerticalExtent(Window);
            var vertScrollbar = GetVertScrollbar();
            var vertScrollPosition = vertScrollbar.GetScrollPosition();
            // if scroll bar is in use, position according to that.
            if (vertScrollbar.IsEffectiveVisible())
            {
                absarea.d_min.Y -= vertScrollPosition;
            }
            else
            {
                // no scrollbar, so adjust position according to formatting set.
                switch (_vertFormatting/*TODO:GetActualVerticalFormatting()*/)
                {
                    case VerticalTextFormatting.CentreAligned:
                        absarea.d_min.Y += CoordConverter.AlignToPixels((absarea.Height - textHeight)*0.5f);
                        break;

                    case VerticalTextFormatting.BottomAligned:
                        absarea.d_min.Y = absarea.d_max.Y - textHeight;
                        break;

                    case VerticalTextFormatting.TopAligned:
                        break;

                    default:
                        throw new InvalidRequestException("Invalid actual vertical formatting.");
                }
            }

            // calculate final colours
            var finalCols = _textCols;
            finalCols.ModulateAlpha(Window.GetEffectiveAlpha());
            // cache the text for rendering.
            var geomBuffers = _formattedRenderedString.CreateRenderGeometry(Window, absarea.Position, finalCols, clipper);
            Window.AppendGeometryBuffers(geomBuffers);
        }

        protected void ConfigureScrollbars()
        {
            // get the scrollbars
            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

            // get the sizes we need
            var renderArea = GetTextRenderArea();
            var renderAreaSize = renderArea.Size;
            var documentSize = GetDocumentSize(renderArea);

            // show or hide vertical scroll bar as required (or as specified by option)
            var showVert = ((documentSize.Height > renderAreaSize.Height) && _enableVertScrollbar);
            var showHorz = ((documentSize.Width > renderAreaSize.Width) && _enableHorzScrollbar);

            vertScrollbar.SetVisible(showVert);
            horzScrollbar.SetVisible(showHorz);

            // if scrollbar visibility just changed we have might have a better TextRenderArea
            // if so we go with that instead
            var updatedRenderArea = GetTextRenderArea();
            if (renderArea != updatedRenderArea)
            {
                _formatValid = false;
                renderArea = updatedRenderArea;
                renderAreaSize = renderArea.Size;
                documentSize = GetDocumentSize(renderArea);
            }

            // Set up scroll bar values
            vertScrollbar.SetDocumentSize(documentSize.Height);
            vertScrollbar.SetPageSize(renderAreaSize.Height);
            vertScrollbar.SetStepSize(System.Math.Max(1.0f, renderAreaSize.Height/10.0f));

            horzScrollbar.SetDocumentSize(documentSize.Width);
            horzScrollbar.SetPageSize(renderAreaSize.Width);
            horzScrollbar.SetStepSize(System.Math.Max(1.0f, renderAreaSize.Width/10.0f));
        }

        protected Scrollbar GetVertScrollbar()
        {
            // return component created by look'n'feel assignment.
            return (Scrollbar) Window.GetChild(VertScrollbarName);
        }

        protected Scrollbar GetHorzScrollbar()
        {
            // return component created by look'n'feel assignment.
            return (Scrollbar) Window.GetChild(HorzScrollbarName);
        }

        protected Rectf GetTextRenderArea()
        {
            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();
            var vertVvisible = vertScrollbar.IsVisible();
            var horzVisible = horzScrollbar.IsVisible();

            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

            var areaName = FrameEnabled ? "WithFrameTextRenderArea" : "NoFrameTextRenderArea";

            // if either of the scrollbars are visible, we might want to use a special rendering area
            if (vertVvisible || horzVisible)
            {
                if (horzVisible)
                {
                    areaName += "H";
                }
                if (vertVvisible)
                {
                    areaName += "V";
                }
                areaName += "Scroll";
            }

            if (wlf.IsNamedAreaPresent(areaName))
            {
                return wlf.GetNamedArea(areaName).GetArea().GetPixelRect(Window);
            }

            // default to plain WithFrameTextRenderArea
            return wlf.GetNamedArea("WithFrameTextRenderArea").GetArea().GetPixelRect(Window);
        }

        protected Sizef GetDocumentSize(Rectf renderArea)
        {
            if (!_formatValid)
                UpdateFormatting(renderArea.Size);

            return new Sizef(_formattedRenderedString.GetHorizontalExtent(Window),
                             _formattedRenderedString.GetVerticalExtent(Window));
        }

        protected void SetupStringFormatter()
        {
            // delete any existing formatter
            // TODO: delete d_formattedRenderedString;
            _formattedRenderedString = null;
            _formatValid = false;

            // create new formatter of whichever type...
            switch (_horzFormatting)
            {
                case HorizontalTextFormatting.LeftAligned:
                    _formattedRenderedString =
                        new LeftAlignedRenderedString(Window.GetRenderedString());
                    break;

                case HorizontalTextFormatting.RightAligned:
                    _formattedRenderedString =
                        new RightAlignedRenderedString(Window.GetRenderedString());
                    break;

                case HorizontalTextFormatting.CentreAligned:
                    _formattedRenderedString =
                        new CentredRenderedString(Window.GetRenderedString());
                    break;

                case HorizontalTextFormatting.Justified:
                    _formattedRenderedString =
                        new JustifiedRenderedString(Window.GetRenderedString());
                    break;

                case HorizontalTextFormatting.WordWrapLeftAligned:
                    _formattedRenderedString =
                        new RenderedStringWordWrapper<LeftAlignedRenderedString>(Window.GetRenderedString());
                    //new RenderedStringWordWrapper<LeftAlignedRenderedString>(d_window.GetRenderedString(),rs => new LeftAlignedRenderedString(rs));
                    break;

                case HorizontalTextFormatting.WordWrapRightAligned:
                    _formattedRenderedString =
                        new RenderedStringWordWrapper<RightAlignedRenderedString>(Window.GetRenderedString());
                    //new RenderedStringWordWrapper<RightAlignedRenderedString>(d_window.GetRenderedString(), rs => new RightAlignedRenderedString(rs));
                    break;

                case HorizontalTextFormatting.WordWrapCentreAligned:
                    _formattedRenderedString =
                        new RenderedStringWordWrapper<CentredRenderedString>(Window.GetRenderedString());
                    //new RenderedStringWordWrapper<CentredRenderedString>(d_window.GetRenderedString(), rs => new CentredRenderedString(rs));
                    break;

                case HorizontalTextFormatting.WordWrapJustified:
                    _formattedRenderedString =
                        new RenderedStringWordWrapper<JustifiedRenderedString>(Window.GetRenderedString());
                    //new RenderedStringWordWrapper<JustifiedRenderedString>(d_window.GetRenderedString(), rs => new JustifiedRenderedString(rs));
                    break;
            }
        }

        // overridden event handlers

        protected bool OnTextChanged(EventArgs e)
        {
            _formatValid = false;
            ConfigureScrollbars();
            Window.Invalidate(false);
            return true;
        }

        protected bool OnSized(/*ElementEventArgs*/EventArgs e)
        {
            _formatValid = false;
            ConfigureScrollbars();
            return true;
        }

        protected void OnFontChanged(object sender, WindowEventArgs e)
        {
            _formatValid = false;
            ConfigureScrollbars();
            Window.Invalidate(false);
            // TODO: return true;
        }

        protected bool OnScroll(EventArgs @event)
        {
            var e = (CursorInputEventArgs)@event;
            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

            var vertScrollbarVisible = vertScrollbar.IsEffectiveVisible();
            var horzScrollbarVisible = horzScrollbar.IsEffectiveVisible();

            if (vertScrollbarVisible && (vertScrollbar.GetDocumentSize() > vertScrollbar.GetPageSize()))
            {
                vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition() + vertScrollbar.GetStepSize()*-e.scroll);
            }
            else if (horzScrollbarVisible && (horzScrollbar.GetDocumentSize() > horzScrollbar.GetPageSize()))
            {
                horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition() + horzScrollbar.GetStepSize()*-e.scroll);
            }

            return vertScrollbarVisible || horzScrollbarVisible;
        }

        // event subscribers
        protected bool HandleScrollbarChange(EventArgs e)
        {
            Window.Invalidate(false);
            return true;
        }

        #region Fields

        /// <summary>
        /// Horizontal formatting to be applied to the text.
        /// </summary>
        private HorizontalTextFormatting _horzFormatting;

        /// <summary>
        /// Vertical formatting to be applied to the text.
        /// </summary>
        private VerticalTextFormatting _vertFormatting;

        /// <summary>
        /// Colours used when rendering the text.
        /// </summary>
        private ColourRect _textCols;

        /// <summary>
        /// true if vertical scroll bar is enabled.
        /// </summary>
        private bool _enableVertScrollbar;

        /// <summary>
        /// true if horizontal scroll bar is enabled.
        /// </summary>
        private bool _enableHorzScrollbar;

        /// <summary>
        /// Class that renders RenderedString with some formatting.
        /// </summary>
        private FormattedRenderedString _formattedRenderedString;

        /// <summary>
        /// true when string formatting is up to date.
        /// </summary>
        private bool _formatValid;

        #endregion
    }
}