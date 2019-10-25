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
    /// RenderingWindow is a RenderingSurface that can be "drawn back" onto another
    /// RenderingSurface and is primarily intended to be used as a kind of cache for
    /// rendered imagery.
    /// </summary>
    public class RenderingWindow : RenderingSurface
    {
        /// <summary>
        /// Constructor for RenderingWindow objects.
        /// </summary>
        /// <param name="target">
        /// The TextureTarget based object that will be used as the target for
        /// content rendering done by the RenderingWindow.
        /// </param>
        /// <param name="owner">
        /// The RenderingSurface object that will be our initial owner.  This
        /// RenderingSurface is also the target where our cached imagery will be
        /// rendered back to.
        /// </param>
        /// <remarks>
        /// The TextureTarget \a target remains under it's original ownership, and
        /// the RenderingSurface \a owner actually owns \e this object.
        /// </remarks>
        public RenderingWindow(ITextureTarget target, RenderingSurface owner)
            : base(target)
        {
            d_renderer = System.GetSingleton().GetRenderer();
            d_textarget = target;
            d_owner = owner;
            d_geometry = d_renderer.CreateGeometryBufferTextured();
            d_geometryValid = false;
            d_position = Lunatics.Mathematics.Vector2.Zero;
            d_size = Sizef.Zero;
            d_rotation = Lunatics.Mathematics.Quaternion.Identity;

            d_geometry.SetBlendMode(BlendMode.RttPremultiplied);
        }

        // TODO: Destructor for RenderingWindow objects.
        //~RenderingWindow();

        /// <summary>
        /// Set the clipping region that will be used when rendering the imagery
        /// for this RenderingWindow back onto the RenderingSurface that owns it.
        /// <para>
        /// This is not the clipping region used when rendering the queued geometry
        /// \e onto the RenderingWindow, that still uses whatever regions are set
        /// on the queued GeometryBuffer objects.
        /// </para>
        /// </summary>
        /// <param name="region">
        /// Rect object describing a rectangular clipping region.
        /// </param>
        /// <remarks>
        /// The region should be described as absolute pixel locations relative to
        /// the screen or other root surface.  The region should \e not be described
        /// relative to the owner of the RenderingWindow.
        /// </remarks>
        public void SetClippingRegion(Rectf region)
        {
            var finalRegion = region;

            // clip region position must be offset according to our owner position, if
            // that is a RenderingWindow.
            if (d_owner.IsRenderingWindow())
            {
                finalRegion.Offset(
                        new Lunatics.Mathematics.Vector2(-((RenderingWindow) d_owner).d_position.X,
                                                        -((RenderingWindow) d_owner).d_position.Y));
            }

            d_geometry.SetClippingRegion(finalRegion);
        }

        /// <summary>
        /// Set the two dimensional position of the RenderingWindow in pixels.  The
        /// origin is at the top-left corner.
        /// </summary>
        /// <param name="position">
        /// Vector2 object describing the desired location of the RenderingWindow,
        /// in pixels.
        /// </param>
        /// <remarks>
        /// This position is an absolute pixel location relative to the screen or
        /// other root surface.  It is \e not relative to the owner of the
        /// RenderingWindow.
        /// </remarks>
        public void SetPosition(Lunatics.Mathematics.Vector2 position)
        {
            d_position = position;

            var trans = new Lunatics.Mathematics.Vector3(d_position.X, d_position.Y, 0.0f);
            // geometry position must be offset according to our owner position, if
            // that is a RenderingWindow.
            if (d_owner.IsRenderingWindow())
            {
                trans.X -= ((RenderingWindow)d_owner).d_position.X;
                trans.Y -= ((RenderingWindow)d_owner).d_position.Y;
            }

            d_geometry.SetTranslation(trans);
        }

        /// <summary>
        /// Set the size of the RenderingWindow in pixels.
        /// </summary>
        /// <param name="size">
        /// Size object that describes the desired size of the RenderingWindow, in
        /// pixels.
        /// </param>
        public void SetSize(Sizef size)
        {
            // URGENT FIXME: Isn't this in the hands of the user?
            /*d_size.d_width = PixelAligned(size.d_width);
            d_size.d_height = PixelAligned(size.d_height);*/
            d_size = size;
            d_geometryValid = false;

            d_textarget.DeclareRenderSize(d_size);
        }

        /// <summary>
        /// Set the rotation quaternion to be used when rendering the RenderingWindow
        /// back onto it's owning RenderingSurface.
        /// </summary>
        /// <param name="rotation">
        /// Quaternion object describing the rotation.
        /// </param>
        public void SetRotation(Lunatics.Mathematics.Quaternion rotation)
        {
            d_rotation = rotation;
            d_geometry.SetRotation(d_rotation);
        }

        /// <summary>
        /// Set the location of the pivot point around which the RenderingWindow
        /// will be rotated.
        /// </summary>
        /// <param name="pivot">
        /// Vector3 describing the three dimensional point around which the
        /// RenderingWindow will be rotated.
        /// </param>
        public void SetPivot(Lunatics.Mathematics.Vector3 pivot)
        {
            d_pivot = pivot;
            d_geometry.SetPivot(d_pivot);
        }

        /// <summary>
        /// Return the current pixel position of the RenderingWindow.  The origin is
        /// at the top-left corner.
        /// </summary>
        /// <returns>
        /// Vector2 object describing the pixel position of the RenderingWindow.
        /// </returns>
        /// <remarks>
        /// This position is an absolute pixel location relative to the screen or
        /// other root surface.  It is \e not relative to the owner of the
        /// RenderingWindow.
        /// </remarks>
        public Lunatics.Mathematics.Vector2 GetPosition()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the current size of the RenderingWindow in pixels.
        /// </summary>
        /// <returns>
        /// Size object describing the current pixel size of the RenderingWindow.
        /// </returns>
        public Sizef GetSize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the current rotation being applied to the RenderingWindow
        /// </summary>
        /// <returns>
        /// Quaternion object describing the rotation for the RenderingWindow.
        /// </returns>
        public Lunatics.Mathematics.Quaternion GetRotation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the rotation pivot point location for the RenderingWindow.
        /// </summary>
        /// <returns>
        /// Vector3 object describing the current location of the pivot point used
        /// when rotating the RenderingWindow.
        /// </returns>
        public Lunatics.Mathematics.Vector3 GetPivot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the TextureTarget object that is the target for content rendered
        /// to this RenderingWindows.  This is the same object passed into the
        /// constructor.
        /// </summary>
        /// <returns>
        /// The TextureTarget object that receives the rendered output resulting
        /// from geometry queued to this RenderingWindow.
        /// </returns>
        public ITextureTarget GetTextureTarget()
        {
            return d_textarget;
        }

        /// <summary>
        /// Peform time based updated for the RenderingWindow.
        /// </summary>
        /// <param name="elapsed">
        /// float value describing the number of seconds that have passed since the
        /// previous call to update.
        /// </param>
        /// <remarks>
        /// Currently this really only has meaning for RenderingWindow objects that
        /// have RenderEffect objects set.  Though this may not always be the case.
        /// </remarks>
        public void Update(float elapsed)
        {
            var effect = d_geometry.GetRenderEffect();

            if (effect != null)
                d_geometryValid &= effect.Update(elapsed, this);
        }

        /// <summary>
        /// Set the RenderEffect that should be used with the RenderingWindow.  This
        /// may be 0 to remove a previously set RenderEffect.
        /// </summary>
        /// <param name="effect"></param>
        /// <remarks>
        /// Ownership of the RenderEffect does not change; the RenderingWindow will
        /// not delete a RenderEffect assigned to it when the RenderingWindow is
        /// destroyed.
        /// </remarks>
        public void SetRenderEffect(RenderEffect effect)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a pointer to the RenderEffect currently being used with the
        /// RenderingWindow.  A return value of 0 indicates that no RenderEffect
        /// is being used.
        /// </summary>
        /// <returns>
        /// Pointer to the RenderEffect used with this RenderingWindow, or 0 for
        /// none.
        /// </returns>
        public RenderEffect GetRenderEffect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// generate geometry to be used when rendering back the RenderingWindow to
        /// it's owning RenderingSurface.
        /// </summary>
        /// <remarks>
        /// In normal usage you should never have to call this directly.  There may
        /// be certain cases where it might be useful to call this when using the
        /// RenderEffect system.
        /// </remarks>
        public void RealiseGeometry()
        {
            if (d_geometryValid)
                return;

            d_geometry.Reset();

            var effect = d_geometry.GetRenderEffect();

            if (effect == null || effect.RealiseGeometry(this, d_geometry))
                RealiseGeometryImpl();

            d_geometryValid = true;
        }

        /// <summary>
        /// Mark the geometry used when rendering the RenderingWindow back to it's
        /// owning RenderingSurface as invalid so that it gets regenerated on the
        /// next rendering pass.
        /// 
        /// This is separate from the main invalidate() function because in most
        /// cases invalidating the cached imagery will not require the potentially
        /// expensive regeneration of the geometry for the RenderingWindow itself.
        /// </summary>
        public void InvalidateGeometry()
        {
            d_geometryValid = false;
        }

        /// <summary>
        /// Return the RenderingSurface that owns the RenderingWindow.  This is
        /// also the RenderingSurface that will be used when the RenderingWindow
        /// renders back it's cached imagery content.
        /// </summary>
        /// <returns>
        /// RenderingSurface object that owns, and is targetted by, the
        /// RenderingWindow.
        /// </returns>
        public RenderingSurface GetOwner()
        {
            return d_owner;
        }

        /// <summary>
        /// Fill in Vector2 object \a p_out with an unprojected version of the
        /// point described by Vector2 \a p_in.
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public void UnprojectPoint(Lunatics.Mathematics.Vector2 pIn, out Lunatics.Mathematics.Vector2 pOut)
        {
            // quick test for rotations to save us a lot of work in the unrotated case
            if ((d_rotation == Lunatics.Mathematics.Quaternion.Identity))
            {
                pOut = pIn;
                return;
            }

            var @in = pIn;

            // localise point for cases where owner is also a RenderingWindow
            if (d_owner.IsRenderingWindow())
                @in -= ((RenderingWindow) d_owner).GetPosition();

            d_owner.GetRenderTarget().UnprojectPoint(d_geometry, @in, out pOut);
            pOut.X += d_position.X;
            pOut.Y += d_position.Y;
        }

        #region Overrides from base

        public override void Draw()
        {
            // update geometry if needed.
            if (!d_geometryValid)
                RealiseGeometry();

            if (d_invalidated)
            {
                // base class will render out queues for us
                base.Draw();
                // mark as no longer invalidated
                d_invalidated = false;
            }

            // add our geometry to our owner for rendering
            d_owner.AddGeometryBuffer(RenderQueueId.RQ_BASE, d_geometry);
        }

        public override void Invalidate()
        {
            // this override is potentially expensive, so only do the main work when we
            // have to.
            if (!d_invalidated)
            {
                base.Invalidate();
                d_textarget.Clear();
            }

            // also invalidate what we render back to.
            d_owner.Invalidate();
        }

        public override bool IsRenderingWindow()
        {
            return true;
        }

        #endregion

        /// <summary>
        /// default generates geometry to draw window as a single quad.
        /// </summary>
        protected virtual void RealiseGeometryImpl()
        {
            var tex = d_textarget.GetTexture();

            var tu = d_size.Width*tex.GetTexelScaling().X;
            var tv = d_size.Height*tex.GetTexelScaling().Y;
            var texRect = d_textarget.IsRenderingInverted()
                               ? new Rectf(0, 1, tu, 1 - tv)
                               : new Rectf(0, 0, tu, tv);

            var area = new Rectf(0, 0, d_size.Width, d_size.Height);
            var c = new Lunatics.Mathematics.Vector4(1f, 1f, 1f, 1f);
            var vbuffer = new TexturedColouredVertex[6];

            // vertex 0
            vbuffer[0].Position = new Lunatics.Mathematics.Vector3(area.d_min.X, area.d_min.Y, 0.0f);
            vbuffer[0].Colour = c;
            vbuffer[0].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.d_min.X, texRect.d_min.Y);

            // vertex 1
            vbuffer[1].Position = new Lunatics.Mathematics.Vector3(area.d_min.X, area.d_max.Y, 0.0f);
            vbuffer[1].Colour = c;
            vbuffer[1].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.d_min.X, texRect.d_max.Y);

            // vertex 2
            vbuffer[2].Position = new Lunatics.Mathematics.Vector3(area.d_max.X, area.d_max.Y, 0.0f);
            vbuffer[2].Colour = c;
            vbuffer[2].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.d_max.X, texRect.d_max.Y);

            // vertex 3
            vbuffer[3].Position = new Lunatics.Mathematics.Vector3(area.d_max.X, area.d_min.Y, 0.0f);
            vbuffer[3].Colour = c;
            vbuffer[3].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.d_max.X, texRect.d_min.Y);

            // vertex 4
            vbuffer[4].Position = new Lunatics.Mathematics.Vector3(area.d_min.X, area.d_min.Y, 0.0f);
            vbuffer[4].Colour = c;
            vbuffer[4].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.d_min.X, texRect.d_min.Y);

            // vertex 5
            vbuffer[5].Position = new Lunatics.Mathematics.Vector3(area.d_max.X, area.d_max.Y, 0.0f);
            vbuffer[5].Colour = c;
            vbuffer[5].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.d_max.X, texRect.d_max.Y);

            d_geometry.SetTexture("texture0", tex);
            d_geometry.AppendGeometry(vbuffer);
        }

        /// <summary>
        /// set a new owner for this RenderingWindow object
        /// </summary>
        /// <param name="owner"></param>
        protected internal void SetOwner(RenderingSurface owner)
        {
            d_owner = owner;
        }

        // TODO: ...
        //// friend is so that RenderingSurface can call setOwner to xfer ownership.
        //friend void RenderingSurface::transferRenderingWindow(RenderingWindow&);

        #region Fields

        /// <summary>
        /// holds ref to renderer
        /// </summary>
        protected Renderer d_renderer;

        /// <summary>
        /// TextureTarget to draw to. Like d_target in base, but avoiding downcasts.
        /// </summary>
        protected ITextureTarget d_textarget;

        /// <summary>
        /// RenderingSurface that owns this object, we render back to this object.
        /// </summary>
        protected RenderingSurface d_owner;

        /// <summary>
        /// GeometryBuffer that holds geometry for drawing this window.
        /// </summary>
        protected GeometryBuffer d_geometry;

        /// <summary>
        /// indicates whether data in GeometryBuffer is up-to-date
        /// </summary>
        protected bool d_geometryValid;

        /// <summary>
        /// Position of this RenderingWindow
        /// </summary>
        protected Lunatics.Mathematics.Vector2 d_position;

        /// <summary>
        /// Size of this RenderingWindow
        /// </summary>
        protected Sizef d_size;

        /// <summary>
        /// Rotation for this RenderingWindow
        /// </summary>
        protected Lunatics.Mathematics.Quaternion d_rotation;

        /// <summary>
        /// Pivot point used for the rotation.
        /// </summary>
        protected Lunatics.Mathematics.Vector3 d_pivot;

        #endregion
    }
}