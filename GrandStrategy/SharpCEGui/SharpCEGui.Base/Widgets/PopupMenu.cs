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

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for popup menus
    /// </summary>
    public class PopupMenu : MenuBase
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "PopupMenu";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/PopupMenu";

        /// <summary>
        /// Get the fade in time for this popup menu.
        /// </summary>
        /// <returns>
        /// The time in seconds that it takes for the popup to fade in. 0 if fading is disabled.
        /// </returns>
        public float GetFadeInTime()
        {
            return _fadeInTime;
        }

        /// <summary>
        /// Get the fade out time for this popup menu.
        /// </summary>
        /// <returns>
        /// The time in seconds that it takes for the popup to fade out. 0 if fading is disabled.
        /// </returns>
        public float GetFadeOutTime()
        {
            return _fadeOutTime;
        }

        /// <summary>
        /// Find out if this popup menu is open or closed;
        /// </summary>
        /// <returns></returns>
        public bool IsPopupMenuOpen()
        {
            return _isOpen;
        }

        /// <summary>
        /// Set the fade in time for this popup menu.
        /// </summary>
        /// <param name="fadetime">
        /// The time in seconds that it takes for the popup to fade in.
        /// If this parameter is zero, fading is disabled.
        /// </param>
        public void SetFadeInTime(float fadetime)
        {
            _fadeInTime=fadetime;
        }

        /// <summary>
        /// Set the fade out time for this popup menu.
        /// </summary>
        /// <param name="fadetime">
        /// The time in seconds that it takes for the popup to fade out.
        /// If this parameter is zero, fading is disabled.
        /// </param>
        public void SetFadeOutTime(float fadetime)
        {
            _fadeOutTime=fadetime;
        }

        /// <summary>
        /// Tells the popup menu to open.
        /// </summary>
        /// <param name="notify">
        /// true if the parent menu item (if any) is to handle the opening. false if not.
        /// </param>
        public void OpenPopupMenu(bool notify = true)
        {
            // already open and not fading, or fading in?
            if (_isOpen && (!_fading || !_fadingOut))
            {
                // then don't do anything
                return;
            }

            // should we let the parent menu item initiate the open?
            var parent = GetParent();
            if (notify && parent != null && (parent as MenuItem)!=null)
            {
                ((MenuItem)parent).OpenPopupMenu();
                return; // the rest will be handled when MenuItem calls us itself
            }

            // we'll handle it ourselves then.
            // are we fading, and fading out?
            if (_fading && _fadingOut)
            {
                if (_fadeInTime > 0.0f && _fadeOutTime > 0.0f)
                {
                    // jump to the point of the fade in that has the same alpha as right now - this keeps it smooth
                    _fadeElapsed = ((_fadeOutTime - _fadeElapsed) / _fadeOutTime) * _fadeInTime;
                }
                else
                {
                    // start the fade in from the beginning
                    _fadeElapsed = 0;
                }
                // change to fade in
                _fadingOut = false;
            }
            // otherwise just start normal fade in!
            else if (_fadeInTime > 0.0f)
            {
                _fading = true;
                _fadingOut = false;
                SetAlpha(0.0f);
                _fadeElapsed = 0;
            }
            // should not fade!
            else
            {
                _fading = false;
                SetAlpha(_origAlpha);
            }

            Show();
            MoveToFront();
        }

        /// <summary>
        /// Tells the popup menu to close.
        /// </summary>
        /// <param name="notify">
        /// true if the parent menu item (if any) is to handle the closing. false if not.
        /// </param>
        public void ClosePopupMenu(bool notify = true)
        {
            // already closed?
            if (!_isOpen)
            {
                // then do nothing
                return;
            }

            // should we let the parent menu item close initiate the close?
            var parent = GetParent();
            if (notify && parent != null && (parent as MenuItem)!=null)
            {
                ((MenuItem)parent).ClosePopupMenu();
                return; // the rest will be handled when MenuItem calls us itself
            }

            // we'll do it our selves then.
            // are we fading, and fading in?
            if (_fading && !_fadingOut)
            {
                // make sure the "fade back out" is smooth - if possible !
                if (_fadeOutTime > 0.0f && _fadeInTime > 0.0f)
                {
                    // jump to the point of the fade in that has the same alpha as right now - this keeps it smooth
                    _fadeElapsed = ((_fadeInTime - _fadeElapsed) / _fadeInTime) * _fadeOutTime;
                }
                else
                {
                    // start the fade in from the beginning
                    _fadeElapsed = 0;
                }
                // change to fade out
                _fadingOut = true;
            }
            // otherwise just start normal fade out!
            else if (_fadeOutTime > 0.0f)
            {
                _fading = true;
                _fadingOut = true;
                SetAlpha(_origAlpha);
                _fadeElapsed = 0;
            }
            // should not fade!
            else
            {
                _fading = false;
                Hide();
            }
        }

        /// <summary>
        /// Constructor for PopupMenu objects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public PopupMenu(string type, string name)
            : base(type, name)
        {
            _origAlpha = d_alpha;
            _fadeOutTime = 0;
            _fadeInTime = 0;
            _fading = false;
            _fadingOut = false;
            _isOpen = false;

            ItemSpacing = 2;

            AddPopupMenuProperties();

            // enable auto resizing
            AutoResize = true;

            // disable parent clipping
            SetClippedByParent(false);

            // hide by default
            Hide();
        }

        protected override void UpdateSelf(float elapsed)
        {
            base.UpdateSelf(elapsed);

            // handle fading
            if (_fading)
            {
		        _fadeElapsed+=elapsed;

		        // fading out
		        if (_fadingOut)
		        {
			        if (_fadeElapsed>=_fadeOutTime)
			        {
				        Hide();
				        _fading=false;
				        SetAlpha(_origAlpha); // set real alpha so users can show directly without having to restore it
			        }
			        else
			        {
				        SetAlpha(_origAlpha*(_fadeOutTime-_fadeElapsed)/_fadeOutTime);
			        }

		        }

		        // fading in
		        else
		        {
			        if (_fadeElapsed>=_fadeInTime)
			        {
				        _fading=false;
				        SetAlpha(_origAlpha);
			        }
			        else
			        {
				        SetAlpha(_origAlpha*_fadeElapsed/_fadeInTime);
			        }
		        }
	        }
        }

        protected override void LayoutItemWidgets()
        {
            // get render area
	        var renderRect = GetItemRenderArea();

	        // get starting position
	        var x0 = CoordConverter.AlignToPixels(renderRect.d_min.X);
	        var y0 = CoordConverter.AlignToPixels(renderRect.d_min.Y);

	        
	        var sz=new UVector2(UDim.Absolute(CoordConverter.AlignToPixels(renderRect.Width)), UDim.Absolute(0)); // set item width

	        // iterate through all items attached to this window
            foreach (var item in ListItems)
            {
                // get the "optimal" height of the item and use that!
                sz.d_y.d_offset = CoordConverter.AlignToPixels(item.GetItemPixelSize().Height); // rounding errors ?

                // set destination rect
                var rect = new URect
                               {
                                   Position = new UVector2(UDim.Absolute(x0), UDim.Absolute(y0)),
                                   Size = new USize(sz.d_x, sz.d_y)
                               };
                // todo: vector vs size
                item.SetArea(rect);

                // next position
                y0 += CoordConverter.AlignToPixels(sz.d_y.d_offset + ItemSpacing);
            }
        }

        protected override Sizef GetContentSize()
        {
            // find the content sizes
	        var widest = 0f;
	        var totalHeight = 0f;
	
	        var i = 0;
            var max = ListItems.Count;
	        while (i < max)
	        {
		        var sz = ListItems[i].GetItemPixelSize();
		        if (sz.Width > widest)
			        widest = sz.Width;
		        totalHeight += sz.Height;

		        i++;
	        }
	
	        var count = (float)i;

	        // vert item spacing
	        if (count >= 2)
	        {
	            totalHeight += (count-1)*ItemSpacing;
	        }

	        // return the content size
	        return new Sizef(widest, totalHeight);
        }

        protected override void OnAlphaChanged(WindowEventArgs e)
        {
            base.OnAlphaChanged(e);
	
	        // if we are not fading, this is a real alpha change request and we save a copy of the value
	        if (!_fading)
	        {
		        _origAlpha = d_alpha;
	        }
        }

        protected override void OnDestructionStarted(WindowEventArgs e)
        {
            // if we are attached to a menuitem, we make sure that gets updated
            var p = GetParent();
            if (p != null && (p as MenuItem) != null)
            {
                ((MenuItem) p).SetPopupMenu(null);
            }
            base.OnDestructionStarted(e);
        }

        protected override void OnShown(WindowEventArgs e)
        {
            _isOpen = true;
            base.OnShown(e);
        }

        protected override void OnHidden(WindowEventArgs e)
        {
            _isOpen = false;
            base.OnHidden(e);
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            base.OnCursorPressHold(e);
            // dont reach our parent
            ++e.handled;
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            base.OnCursorActivate(e);
            // dont reach our parent
            ++e.handled;
        }

        private void AddPopupMenuProperties()
        {
            AddProperty(new TplWindowProperty<PopupMenu, float>(
                            "FadeInTime",
                            "Property to get/set the fade in time in seconds of the popup menu.  Value is a float.",
                            (x, v) => x.SetFadeInTime(v), x => x.GetFadeInTime(), WidgetTypeName));

            AddProperty(new TplWindowProperty<PopupMenu, float>(
                            "FadeOutTime",
                            "Property to get/set the fade out time in seconds of the popup menu.  Value is a float.",
                            (x, v) => x.SetFadeOutTime(v), x => x.GetFadeOutTime(), WidgetTypeName));
        }

        #region Fields

        /// <summary>
        /// The original alpha of this window.
        /// </summary>
        private float _origAlpha;

        /// <summary>
        /// The time in seconds this popup menu has been fading.
        /// </summary>
        private float _fadeElapsed;

        /// <summary>
        /// The time in seconds it takes for this popup menu to fade out.
        /// </summary>
        private float _fadeOutTime;

        /// <summary>
        /// The time in seconds it takes for this popup menu to fade in.
        /// </summary>
        private float _fadeInTime;

        /// <summary>
        /// true if this popup menu is fading in/out. false if not
        /// </summary>
        private bool _fading;

        /// <summary>
        /// true if this popup menu is fading out. false if fading in.
        /// </summary>
        private bool _fadingOut;

        /// <summary>
        /// true if this popup menu is open. false if not.
        /// </summary>
        private bool _isOpen;

        #endregion
    }
}