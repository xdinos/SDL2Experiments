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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that provides mouse cursor support.
    /// </summary>
    public class MouseCursor : IDisposable
    {
        /// <summary>
        /// Name of Event fired when the mouse cursor image is changed.
        /// Handlers are passed a const MouseCursorEventArgs reference with
        /// MouseCursorEventArgs::mouseCursor set to the MouseCursor that has
        /// had it's image changed, and MouseCursorEventArgs::image set to the
        /// Image that is now set for the MouseCursor (may be 0).
        /// </summary>
        public event EventHandler<MouseCursorEventArgs> ImageChanged;

        /// <summary>
        /// Name of Event fired when the Image to be used as a default mouse cursor
        /// image is changed.
        /// Handlers are passed a const MouseCursorEventArgs reference with
        /// MouseCursorEventArgs::mouseCursor set to the MouseCursor that has
        /// had it's default image changed, and MouseCursorEventArgs::image set to
        /// the Image that is now set as the default (may be 0).
        /// </summary>
        public event EventHandler<MouseCursorEventArgs> DefaultImageChanged;

        /// <summary>
        /// Constructor for MouseCursor objects
        /// </summary>
        public MouseCursor()
        {
            d_cursorImage = null;
            d_defaultCursorImage = null;
            d_position = new Vector2f(0.0f, 0.0f);
            d_visible = true;
            d_geometry = System.GetSingleton().GetRenderer().CreateGeometryBuffer();
            d_customSize = new Sizef(0.0f, 0.0f);
            d_customOffset = new Vector2f(0.0f, 0.0f);
            d_cachedGeometryValid = false;

            var screenArea = new Rectf(Vector2f.Zero, System.GetSingleton().GetRenderer().GetDisplaySize());

            d_geometry.SetClippingRegion(screenArea);

            // default constraint is to whole screen
            SetConstraintArea(screenArea);

            SetPosition(s_initialPositionSet
                            ? s_initialPosition
                            : new Vector2f(screenArea.Width/2, screenArea.Height/2));
        }

        // TODO: Destructor for MouseCursor objects
        // TODO: ~MouseCursor(void);

        public void Dispose()
        {
            System.GetSingleton().GetRenderer().DestroyGeometryBuffer(d_geometry);
        }

        /// <summary>
        /// Set the current mouse cursor image
        /// </summary>
        /// <param name="name">
        /// String object holding the name of the desired Image.
        /// </param>
        /// <exception cref="UnknownObjectException	">
        /// thrown if Image \a name is not known.
        /// </exception>
        public void SetImage(string name)
        {
            SetImage(ImageManager.GetSingleton().Get(name));
        }

        /// <summary>
        /// Set the current mouse cursor image
        /// </summary>
        /// <param name="image"></param>
        public void SetImage(Image image)
        {
            if (image == d_cursorImage)
                return;

            d_cursorImage = image;
            d_cachedGeometryValid = false;

            OnImageChanged(new MouseCursorEventArgs(this));
        }

        /// <summary>
        /// Get the current mouse cursor image.
        /// </summary>
        /// <returns>
        /// The current image used to draw mouse cursor.
        /// </returns>
        public Image GetImage()
        {
            return d_cursorImage;
        }

        /// <summary>
        /// Set the image to be used as the default mouse cursor.
        /// </summary>
        /// <param name="image">
        /// Pointer to an image object that is to be used as the default mouse
        /// cursor.  To have no cursor rendered by default, you can specify 0 here.
        /// </param>
        public void SetDefaultImage(Image image)
        {
            if (image == d_defaultCursorImage)
                return;

            d_defaultCursorImage = image;
            d_cachedGeometryValid = d_cursorImage != null;

            OnDefaultImageChanged(new MouseCursorEventArgs(this) {image = image});
        }

        /// <summary>
        /// Set the image to be used as the default mouse cursor.
        /// </summary>
        /// <param name="name">
        /// String object that contains the name of the Image that is to be used.
        /// </param>
        /// <exception cref="UnknownObjectException">
        /// thrown if no Image named \a name exists.
        /// </exception>
        public void SetDefaultImage(string name)
        {
            SetDefaultImage(ImageManager.GetSingleton().Get(name));
        }

        /// <summary>
        /// Return the currently set default mouse cursor image
        /// </summary>
        /// <returns>
        /// Pointer to the current default image used for the mouse cursor.  May
        /// return 0 if default cursor has not been set, or has intentionally
        /// been set to 0 - which results in a blank default cursor.
        /// </returns>
        public Image GetDefaultImage()
        {
            return d_defaultCursorImage;
        }

        /// <summary>
        /// Makes the cursor draw itself
        /// </summary>
        public void Draw()
        {
            if (!d_visible || d_cursorImage == null)
                return;

            if (!d_cachedGeometryValid)
                CacheGeometry();

            d_geometry.Draw();
        }

        /// <summary>
        /// Set the current mouse cursor position
        /// </summary>
        /// <param name="position">
        /// Point object describing the new location for the mouse.  This will be clipped to within the renderer screen area.
        /// </param>
        public void SetPosition(Vector2f position)
        {
            d_position = position;
            ConstrainPosition();

            d_geometry.SetTranslation(new Vector3f(d_position.d_x, d_position.d_y, 0));
        }

        /// <summary>
        /// Offset the mouse cursor position by the deltas specified in \a offset.
        /// </summary>
        /// <param name="offset">
        /// Point object which describes the amount to move the cursor in each axis.
        /// </param>
        public void OffsetPosition(Vector2f offset)
        {
            d_position.d_x += offset.d_x;
            d_position.d_y += offset.d_y;
            ConstrainPosition();

            d_geometry.SetTranslation(new Vector3f(d_position.d_x, d_position.d_y, 0));
        }

        /// <summary>
        /// Set the area that the mouse cursor is constrained to.
        /// </summary>
        /// <param name="area">
        /// Pointer to a Rect object that describes the area of the display that the mouse is allowed to occupy. 
        /// The given area will be clipped to the current Renderer screen area - it is never possible for the 
        /// mouse to leave this area. If this parameter is NULL, the constraint is set to the size of the current 
        /// Renderer screen area.
        ///  </param>
        public void SetConstraintArea(Rectf? area)
        {
            var rendererArea = new Rectf(Vector2f.Zero, System.GetSingleton().GetRenderer().GetDisplaySize());

            if (!area.HasValue)
            {
                d_constraints.d_min.d_x = UDim.Relative(rendererArea.d_min.d_x/rendererArea.Width);
                d_constraints.d_min.d_y = UDim.Relative(rendererArea.d_min.d_y/rendererArea.Height);
                d_constraints.d_max.d_x = UDim.Relative(rendererArea.d_max.d_x/rendererArea.Width);
                d_constraints.d_max.d_y = UDim.Relative(rendererArea.d_max.d_y/rendererArea.Height);
            }
            else
            {
                var finalArea = area.Value.GetIntersection(rendererArea);

                d_constraints.d_min.d_x = UDim.Relative(finalArea.d_min.d_x/rendererArea.Width);
                d_constraints.d_min.d_y = UDim.Relative(finalArea.d_min.d_y/rendererArea.Height);
                d_constraints.d_max.d_x = UDim.Relative(finalArea.d_max.d_x/rendererArea.Width);
                d_constraints.d_max.d_y = UDim.Relative(finalArea.d_max.d_y/rendererArea.Height);
            }

            ConstrainPosition();
        }

        /// <summary>
        /// Set the area that the mouse cursor is constrained to.
        /// </summary>
        /// <param name="area">
        /// Pointer to a URect object that describes the area of the display that the mouse is allowed to occupy.  
        /// The given area will be clipped to the current Renderer screen area - it is never possible for the mouse
        /// to leave this area.  If this parameter is NULL, the constraint is set to the size of the current Renderer 
        /// screen area.
        /// </param>
        public void SetUnifiedConstraintArea(URect? area)
        {
            var rendererArea = new Rectf(Vector2f.Zero,
                                          System.GetSingleton().GetRenderer().GetDisplaySize());

            if (area.HasValue)
            {
                d_constraints = area.Value;
            }
            else
            {
                d_constraints.d_min.d_x = UDim.Relative(rendererArea.d_min.d_x/rendererArea.Width);
                d_constraints.d_min.d_y = UDim.Relative(rendererArea.d_min.d_y/rendererArea.Height);
                d_constraints.d_max.d_x = UDim.Relative(rendererArea.d_max.d_x/rendererArea.Width);
                d_constraints.d_max.d_y = UDim.Relative(rendererArea.d_max.d_y/rendererArea.Height);
            }

            ConstrainPosition();
        }

        /// <summary>
        /// Hides the mouse cursor.
        /// </summary>
        public void Hide()
        {
            d_visible = false;
        }

        /// <summary>
        /// Shows the mouse cursor.
        /// </summary>
        public void Show()
        {
            d_visible = true;
        }

        /// <summary>
        /// Set the visibility of the mouse cursor.
        /// </summary>
        /// <param name="visible">
        /// 'true' to show the mouse cursor, 
        /// 'false' to hide it.
        /// </param>
        public void SetVisible(bool visible)
        {
            d_visible = visible;
        }

        /// <summary>
        /// return whether the mouse cursor is visible.
        /// </summary>
        /// <returns>
        /// true if the mouse cursor is visible, 
        /// false if the mouse cursor is hidden.
        /// </returns>
        public bool IsVisible()
        {
            return d_visible;
        }

        /// <summary>
        /// Return the current mouse cursor position as a pixel offset from the top-left corner of the display.
        /// </summary>
        /// <returns>
        /// Point object describing the mouse cursor position in screen pixels.
        /// </returns>
        public Vector2f GetPosition()
        {
            return d_position;
        }

        /// <summary>
        /// return the current constraint area of the mouse cursor.
        /// </summary>
        /// <returns>
        /// Rect object describing the active area that the mouse cursor is constrained to.
        /// </returns>
        public Rectf GetConstraintArea()
        {
            return CoordConverter.AsAbsolute(d_constraints, System.GetSingleton().GetRenderer().GetDisplaySize());
        }

        /// <summary>
        /// return the current constraint area of the mouse cursor.
        /// </summary>
        /// <returns>
        /// URect object describing the active area that the mouse cursor is constrained to.
        /// </returns>
        public URect GetUnifiedConstraintArea()
        {
            return d_constraints;
        }

        /// <summary>
        /// Return the current mouse cursor position as display resolution independant values.
        /// </summary>
        /// <returns>
        /// Point object describing the current mouse cursor position as resolution independant values that
        /// range from 0.0f to 1.0f, where 0.0f represents the left-most and top-most positions, and 1.0f
        /// represents the right-most and bottom-most positions.
        /// </returns>
        public Vector2f GetDisplayIndependantPosition()
        {
            var dsz = System.GetSingleton().GetRenderer().GetDisplaySize();

            return new Vector2f(d_position.d_x/(dsz.d_width - 1.0f),
                                d_position.d_y/(dsz.d_height - 1.0f));
        }

        /// <summary>
        /// Function used to notify the MouseCursor of changes in the display size.
        /// 
        /// You normally would not call this directly; rather you would call the
        /// function System::notifyDisplaySizeChanged and that will then call this
        /// function for you.
        /// </summary>
        /// <param name="newSize">
        /// Size object describing the new display size in pixels.
        /// </param>
        public void NotifyDisplaySizeChanged(Sizef newSize)
        {
            var screenArea = new Rectf(Vector2f.Zero, newSize);
            d_geometry.SetClippingRegion(screenArea);

            // invalidate to regenerate geometry at (maybe) new size
            d_cachedGeometryValid = false;
        }

        /// <summary>
        /// Set an explicit size for the mouse cursor image to be drawn at.
        /// 
        /// This will override the size that is usually obtained directly from the
        /// mouse cursor image and will stay in effect across changes to the mouse
        /// cursor image.
        /// 
        /// Setting this size to (0, 0) will revert back to using the size as
        /// obtained from the Image itself.
        /// </summary>
        /// <param name="size">
        /// Reference to a Size object that describes the size at which the cursor
        /// image should be drawn in pixels.
        /// </param>
        public void SetExplicitRenderSize(Sizef size)
        {
            d_customSize = size;
            d_cachedGeometryValid = false;
        }
        
        /// <summary>
        /// Return the explicit render size currently set.  A return size of (0, 0)
        /// indicates that the real image size will be used.
        /// </summary>
        /// <returns></returns>
        public Sizef GetExplicitRenderSize()
        {
            return d_customSize;
        }

        /// <summary>
        /// Static function to pre-initialise the mouse cursor position (prior to
        /// MouseCursor instantiation).
        /// 
        /// Calling this function prior to instantiating MouseCursor will prevent
        /// the mouse having it's position set to the middle of the initial view.
        /// Calling this function after the MouseCursor is instantiated will have
        /// no effect.
        /// </summary>
        /// <param name="position">
        /// Reference to a point object describing the initial pixel position to
        /// be used for the mouse cursor.
        /// </param>
        public static void SetInitialMousePosition(Vector2f position)
        {
            s_initialPosition = position;
            s_initialPositionSet = true;
        }

        /// <summary>
        /// Mark the cached geometry as invalid so it will be recached next time the
        /// mouse cursor is drawn.
        /// </summary>
        public void Invalidate()
        {
            d_cachedGeometryValid = false;
        }

        /// <summary>
        /// Event triggered internally when mouse cursor image is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnImageChanged(MouseCursorEventArgs e)
        {
            var handler = ImageChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Event triggered internally when mouse cursor default image is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDefaultImageChanged(MouseCursorEventArgs e)
        {
            var handler = DefaultImageChanged;
            if (handler != null)
                handler(this, e);
        }


        /// <summary>
        /// Checks the mouse cursor position is within the current 'constrain' Rect and adjusts as required.
        /// </summary>
        private void ConstrainPosition()
        {
            var absarea = GetConstraintArea();

            if (d_position.d_x >= absarea.d_max.d_x)
                d_position.d_x = absarea.d_max.d_x - 1;

            if (d_position.d_y >= absarea.d_max.d_y)
                d_position.d_y = absarea.d_max.d_y - 1;

            if (d_position.d_y < absarea.d_min.d_y)
                d_position.d_y = absarea.d_min.d_y;

            if (d_position.d_x < absarea.d_min.d_x)
                d_position.d_x = absarea.d_min.d_x;
        }

        /// <summary>
        /// updates the cached geometry.
        /// </summary>
        private void CacheGeometry()
        {
            d_cachedGeometryValid = true;
            d_geometry.Reset();

            // if no image, nothing more to do.
            if (d_cursorImage == null)
                return;

            if (Math.Abs(d_customSize.d_width - 0.0f) > float.Epsilon ||
                Math.Abs(d_customSize.d_height - 0.0f) > float.Epsilon)
            {
                CalculateCustomOffset();
                d_cursorImage.Render(d_geometry, d_customOffset, d_customSize);
            }
            else
            {
                d_cursorImage.Render(d_geometry, Vector2f.Zero);
            }
        }

        /// <summary>
        /// calculate offset for custom image size so 'hot spot' is maintained.
        /// </summary>
        private void CalculateCustomOffset()
        {
            var sz = d_cursorImage.GetRenderedSize();
            var offset = d_cursorImage.GetRenderedOffset();

            d_customOffset.d_x = d_customSize.d_width/sz.d_width*offset.d_x - offset.d_x;
            d_customOffset.d_y = d_customSize.d_height/sz.d_height*offset.d_y - offset.d_y;
        }

        #region Fields

        //! Image that is currently set as the mouse cursor.
        private Image d_cursorImage;
        
        //! Image that will be used as the default image for this mouse cursor.
        private Image d_defaultCursorImage;
        
        private Vector2f d_position; //!< Current location of the cursor
        
        private bool d_visible; //!< true if the cursor will be drawn, else false.
        
        private URect d_constraints; //!< Specifies the area (in screen pixels) that the mouse can move around in.
        
        /// <summary>
        /// buffer to hold geometry for mouse cursor imagery.
        /// </summary>
        private readonly GeometryBuffer d_geometry;

        //! custom explicit size to render the cursor image at
        private Sizef d_customSize;
        
        //! correctly scaled offset used when using custom image size.
        private Vector2f d_customOffset;
        
        //! true if the mouse initial position has been pre-set
        private static bool s_initialPositionSet;
        
        //! value set as initial position (if any)
        private static Vector2f s_initialPosition;
        
        //! boolean indicating whether cached pointer geometry is valid.
        private bool d_cachedGeometryValid;

        #endregion
    }
}