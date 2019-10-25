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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that provides cursor support.
    /// </summary>
    public class Cursor : IDisposable
    {
        /// <summary>
        /// Name of Event fired when the mouse cursor image is changed.
        /// Handlers are passed a const MouseCursorEventArgs reference with
        /// MouseCursorEventArgs::Cursor set to the Cursor that has
        /// had it's image changed, and MouseCursorEventArgs::image set to the
        /// Image that is now set for the Cursor (may be 0).
        /// </summary>
        public event EventHandler<CursorEventArgs> ImageChanged;

        /// <summary>
        /// Name of Event fired when the Image to be used as a default mouse cursor
        /// image is changed.
        /// Handlers are passed a const MouseCursorEventArgs reference with
        /// MouseCursorEventArgs::Cursor set to the Cursor that has
        /// had it's default image changed, and MouseCursorEventArgs::image set to
        /// the Image that is now set as the default (may be 0).
        /// </summary>
        public event EventHandler<CursorEventArgs> DefaultImageChanged;

        /// <summary>
        /// Constructor for Cursor objects
        /// </summary>
        public Cursor()
        {
            d_indicatorImage = null;
            d_defaultIndicatorImage = null;
            d_position = Lunatics.Mathematics.Vector2.Zero;
            d_visible = true;
            d_customSize = new Sizef(0.0f, 0.0f);
            d_customOffset = Lunatics.Mathematics.Vector2.Zero;
            d_cachedGeometryValid = false;

            var screenArea = new Rectf(Lunatics.Mathematics.Vector2.Zero,
                                       System.GetSingleton().GetRenderer().GetDisplaySize());

            // default constraint is to whole screen
            SetConstraintArea(screenArea);

            SetPosition(s_initialPositionSet
                            ? s_initialPosition
                            : new Lunatics.Mathematics.Vector2(screenArea.Width / 2f, screenArea.Height / 2f));
        }

        // TODO: Destructor for Cursor objects
        // TODO: ~Cursor() { DestroyGeometryBuffers(); }

        public void Dispose()
        {
            DestroyGeometryBuffers();
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
            if (image == d_indicatorImage)
                return;

            d_indicatorImage = image;
            d_cachedGeometryValid = false;

            OnImageChanged(new CursorEventArgs(this));
        }

        /// <summary>
        /// Get the current mouse cursor image.
        /// </summary>
        /// <returns>
        /// The current image used to draw mouse cursor.
        /// </returns>
        public Image GetImage()
        {
            return d_indicatorImage;
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
            if (image == d_defaultIndicatorImage)
                return;

            d_defaultIndicatorImage = image;
            d_cachedGeometryValid = d_indicatorImage != null;

            OnDefaultImageChanged(new CursorEventArgs(this) {image = image});
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
            return d_defaultIndicatorImage;
        }

        /// <summary>
        /// Makes the cursor draw itself
        /// </summary>
        public void Draw()
        {
            if (!d_visible || d_indicatorImage == null)
                return;

            if (!d_cachedGeometryValid)
                CacheGeometry();

            var geom_buffer_count = d_geometryBuffers.Count;
            for (var i = 0; i < geom_buffer_count; ++i)
                d_geometryBuffers[i].Draw();
        }

        /// <summary>
        /// Set the current mouse cursor position
        /// </summary>
        /// <param name="position">
        /// Point object describing the new location for the mouse.  This will be clipped to within the renderer screen area.
        /// </param>
        public void SetPosition(Lunatics.Mathematics.Vector2 position)
        {
            d_position = position;
            ConstrainPosition();

            UpdateGeometryBuffersTranslation();
        }

        /// <summary>
        /// Offset the mouse cursor position by the deltas specified in \a offset.
        /// </summary>
        /// <param name="offset">
        /// Point object which describes the amount to move the cursor in each axis.
        /// </param>
        public void OffsetPosition(Lunatics.Mathematics.Vector2 offset)
        {
            d_position.X += offset.X;
            d_position.Y += offset.Y;
            ConstrainPosition();

            UpdateGeometryBuffersTranslation();
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
            var rendererArea = new Rectf(Lunatics.Mathematics.Vector2.Zero,
                                         System.GetSingleton().GetRenderer().GetDisplaySize());

            if (!area.HasValue)
            {
                d_constraints.d_min.d_x = UDim.Relative(rendererArea.d_min.X/rendererArea.Width);
                d_constraints.d_min.d_y = UDim.Relative(rendererArea.d_min.Y/rendererArea.Height);
                d_constraints.d_max.d_x = UDim.Relative(rendererArea.d_max.X/rendererArea.Width);
                d_constraints.d_max.d_y = UDim.Relative(rendererArea.d_max.Y/rendererArea.Height);
            }
            else
            {
                var finalArea = area.Value.GetIntersection(rendererArea);

                d_constraints.d_min.d_x = UDim.Relative(finalArea.d_min.X/rendererArea.Width);
                d_constraints.d_min.d_y = UDim.Relative(finalArea.d_min.Y/rendererArea.Height);
                d_constraints.d_max.d_x = UDim.Relative(finalArea.d_max.X/rendererArea.Width);
                d_constraints.d_max.d_y = UDim.Relative(finalArea.d_max.Y/rendererArea.Height);
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
            var rendererArea = new Rectf(Lunatics.Mathematics.Vector2.Zero,
                                          System.GetSingleton().GetRenderer().GetDisplaySize());

            if (area.HasValue)
            {
                d_constraints = area.Value;
            }
            else
            {
                d_constraints.d_min.d_x = UDim.Relative(rendererArea.d_min.X/rendererArea.Width);
                d_constraints.d_min.d_y = UDim.Relative(rendererArea.d_min.Y/rendererArea.Height);
                d_constraints.d_max.d_x = UDim.Relative(rendererArea.d_max.X/rendererArea.Width);
                d_constraints.d_max.d_y = UDim.Relative(rendererArea.d_max.Y/rendererArea.Height);
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
        public Lunatics.Mathematics.Vector2 GetPosition()
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
        public Lunatics.Mathematics.Vector2 GetDisplayIndependantPosition()
        {
            var dsz = System.GetSingleton().GetRenderer().GetDisplaySize();

            return new Lunatics.Mathematics.Vector2(d_position.X/(dsz.Width - 1.0f),
                                                   d_position.Y/(dsz.Height - 1.0f));
        }

        /// <summary>
        /// Function used to notify the Cursor of changes in the display size.
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
            UpdateGeometryBuffersClipping(new Rectf(Lunatics.Mathematics.Vector2.Zero, newSize));

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
        /// Cursor instantiation).
        /// 
        /// Calling this function prior to instantiating Cursor will prevent
        /// the mouse having it's position set to the middle of the initial view.
        /// Calling this function after the Cursor is instantiated will have
        /// no effect.
        /// </summary>
        /// <param name="position">
        /// Reference to a point object describing the initial pixel position to
        /// be used for the mouse cursor.
        /// </param>
        public static void SetInitialMousePosition(Lunatics.Mathematics.Vector2 position)
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
        protected virtual void OnImageChanged(CursorEventArgs e)
        {
            var handler = ImageChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Event triggered internally when mouse cursor default image is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDefaultImageChanged(CursorEventArgs e)
        {
            var handler = DefaultImageChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Destroys the geometry buffers of this Cursor.
        /// </summary>
        private void DestroyGeometryBuffers()
        {
            var renderer = System.GetSingleton().GetRenderer();
            foreach (var geometryBuffer in d_geometryBuffers)
                renderer.DestroyGeometryBuffer(geometryBuffer);

            d_geometryBuffers.Clear();
        }

        /// <summary>
        /// Updates the translation of the geometry buffers of this Cursor.
        /// </summary>
        private void UpdateGeometryBuffersTranslation()
        {
            foreach (var geometryBuffer in d_geometryBuffers)
                geometryBuffer.SetTranslation(new Lunatics.Mathematics.Vector3(d_position.X, d_position.Y, 0));
        }

        /// <summary>
        /// Updates the clipping area of the geometry buffers of this Cursor.
        /// </summary>
        /// <param name="clipping_area">
        /// The clipping area that will be applied to the geometry buffers of this Cursor.
        /// </param>
        private void UpdateGeometryBuffersClipping(Rectf clipping_area)
        {
            foreach (var geometryBuffer in d_geometryBuffers)
                geometryBuffer.SetClippingRegion(clipping_area);
        }

        /// <summary>
        /// Checks the mouse cursor position is within the current 'constrain' Rect and adjusts as required.
        /// </summary>
        private void ConstrainPosition()
        {
            var absarea = GetConstraintArea();

            if (d_position.X >= absarea.d_max.X)
                d_position.X = absarea.d_max.X - 1;

            if (d_position.Y >= absarea.d_max.Y)
                d_position.Y = absarea.d_max.Y - 1;

            if (d_position.Y < absarea.d_min.Y)
                d_position.Y = absarea.d_min.Y;

            if (d_position.X < absarea.d_min.X)
                d_position.X = absarea.d_min.X;
        }

        /// <summary>
        /// updates the cached geometry.
        /// </summary>
        private void CacheGeometry()
        {
            d_cachedGeometryValid = true;
            DestroyGeometryBuffers();

            // if no image, nothing more to do.
            if (d_indicatorImage == null)
                return;

            if (Math.Abs(d_customSize.Width - 0.0f) > float.Epsilon ||
                Math.Abs(d_customSize.Height - 0.0f) > float.Epsilon)
            {
                CalculateCustomOffset();
                var imgRenderSettings=new ImageRenderSettings(new Rectf(d_customOffset, d_customSize));
                d_geometryBuffers.AddRange(d_indicatorImage.CreateRenderGeometry(imgRenderSettings));
            }
            else
            {
                var imgRenderSettings = new ImageRenderSettings (new Rectf(Lunatics.Mathematics.Vector2.Zero, d_indicatorImage.GetRenderedSize()));
                d_geometryBuffers.AddRange(d_indicatorImage.CreateRenderGeometry(imgRenderSettings));
            }

            // TODO: const Rectf clipping_area(glm::vec2(0, 0), System::getSingleton().getRenderer()->getDisplaySize());
            var clipping_area = new Rectf(Lunatics.Mathematics.Vector2.Zero,
                                          System.GetSingleton().GetRenderer().GetDisplaySize());
            UpdateGeometryBuffersClipping(clipping_area);
            UpdateGeometryBuffersTranslation();
        }

        /// <summary>
        /// calculate offset for custom image size so 'hot spot' is maintained.
        /// </summary>
        private void CalculateCustomOffset()
        {
            var sz = d_indicatorImage.GetRenderedSize();
            var offset = d_indicatorImage.GetRenderedOffset();

            d_customOffset.X = d_customSize.Width/sz.Width*offset.X - offset.X;
            d_customOffset.Y = d_customSize.Height/sz.Height*offset.Y - offset.Y;
        }

        #region Fields

        ///Image that is currently set as the cursor.
        private Image d_indicatorImage;
        
        //! Image that will be used as the default image for this mouse cursor.
        private Image d_defaultIndicatorImage;

        private Lunatics.Mathematics.Vector2 d_position; //!< Current location of the cursor
        
        private bool d_visible; //!< true if the cursor will be drawn, else false.
        
        private URect d_constraints; //!< Specifies the area (in screen pixels) that the mouse can move around in.

        /// <summary>
        /// buffer to hold geometry for mouse cursor imagery.
        /// </summary>
        private readonly List<GeometryBuffer> d_geometryBuffers = new List<GeometryBuffer>();

        //! custom explicit size to render the cursor image at
        private Sizef d_customSize;
        
        //! correctly scaled offset used when using custom image size.
        private Lunatics.Mathematics.Vector2 d_customOffset;
        
        //! true if the mouse initial position has been pre-set
        private static bool s_initialPositionSet;
        
        //! value set as initial position (if any)
        private static Lunatics.Mathematics.Vector2 s_initialPosition;
        
        //! boolean indicating whether cached pointer geometry is valid.
        private bool d_cachedGeometryValid;

        #endregion
    }
}