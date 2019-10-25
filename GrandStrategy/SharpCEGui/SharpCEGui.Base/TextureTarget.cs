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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Specialisation of RenderTarget interface that should be used as the base
    /// class for RenderTargets that are implemented using textures.
    /// </summary>
    public interface ITextureTarget : IRenderTarget
    {
        /// <summary>
        /// Clear the surface of the underlying texture.
        /// </summary>
        void Clear();

        /// <summary>
        /// Return a pointer to the CEGUI::Texture that the TextureTarget is using.
        /// </summary>
        /// <returns>
        /// Texture object that the TextureTarget uses when rendering imagery.
        /// </returns>
        Texture GetTexture();

        /// <summary>
        /// Used to declare to the TextureTarget the largest size, in pixels, of the
        /// next set of incoming rendering operations.
        /// </summary>
        /// <remarks>
        /// The main purpose of this is to allow for the implemenatation to resize
        /// the underlying texture so that it can hold the imagery that will be
        /// drawn.
        /// </remarks>
        /// <param name="sz">
        /// Size object describing the largest area that will be rendererd in the
        /// next batch of rendering operations.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// May be thrown if the TextureTarget would not be able to handle the
        /// operations rendering content of the given size.
        /// </exception>
        void DeclareRenderSize(Sizef sz);

        /// <summary>
        /// Return whether rendering done on the target texture is inverted in
        /// relation to regular textures.
        /// 
        /// <para>
        /// This is intended to be used when generating geometry for rendering the
        /// TextureTarget onto another surface.
        /// </para>
        /// </summary>
        /// <returns>
        /// - true if the texture content should be considered as inverted
        ///   vertically in comparison with other regular textures.
        /// - false if the texture content has the same orientation as regular
        ///   textures.
        /// </returns>
        bool IsRenderingInverted();

        /// <summary>
        /// Return whether this TextureTarget has a stencil buffer attached or not.
        /// </summary>
        /// <returns>
        /// - true if a stencil buffer is attached
        /// - false if no stencil buffer is attached
        /// </returns>
        bool GetUsesStencil();
    }

    ///// <summary>
    ///// Specialisation of RenderTarget interface that should be used as the base
    ///// class for RenderTargets that are implemented using textures.
    ///// </summary>
    //public abstract class TextureTarget : RenderTarget, ITextureTarget
    //{
    //    /// <summary>
    //    /// Clear the surface of the underlying texture.
    //    /// </summary>
    //    public abstract void Clear();

    //    /// <summary>
    //    /// Return a pointer to the CEGUI::Texture that the TextureTarget is using.
    //    /// </summary>
    //    /// <returns>
    //    /// Texture object that the TextureTarget uses when rendering imagery.
    //    /// </returns>
    //    public abstract Texture GetTexture();

    //    /// <summary>
    //    /// Used to declare to the TextureTarget the largest size, in pixels, of the
    //    /// next set of incoming rendering operations.
    //    /// </summary>
    //    /// <remarks>
    //    /// The main purpose of this is to allow for the implemenatation to resize
    //    /// the underlying texture so that it can hold the imagery that will be
    //    /// drawn.
    //    /// </remarks>
    //    /// <param name="sz">
    //    /// Size object describing the largest area that will be rendererd in the
    //    /// next batch of rendering operations.
    //    /// </param>
    //    /// <exception cref="InvalidRequestException">
    //    /// May be thrown if the TextureTarget would not be able to handle the
    //    /// operations rendering content of the given size.
    //    /// </exception>
    //    public abstract void DeclareRenderSize(Sizef sz);

    //    /// <summary>
    //    /// Return whether rendering done on the target texture is inverted in
    //    /// relation to regular textures.
    //    /// 
    //    /// <para>
    //    /// This is intended to be used when generating geometry for rendering the
    //    /// TextureTarget onto another surface.
    //    /// </para>
    //    /// </summary>
    //    /// <returns>
    //    /// - true if the texture content should be considered as inverted
    //    ///   vertically in comparison with other regular textures.
    //    /// - false if the texture content has the same orientation as regular
    //    ///   textures.
    //    /// </returns>
    //    public abstract bool IsRenderingInverted();

    //    /// <summary>
    //    /// Return whether this TextureTarget has a stencil buffer attached or not.
    //    /// </summary>
    //    /// <returns>
    //    /// - true if a stencil buffer is attached
    //    /// - false if no stencil buffer is attached
    //    /// </returns>
    //    public bool GetUsesStencil()
    //    {
    //        return d_usesStencil;
    //    }

    //    #region Fields

    //    //! Determines if the instance has a stencil buffer attached or not
    //    protected bool d_usesStencil;

    //    #endregion
    //}
}