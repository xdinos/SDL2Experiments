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
    /// Interface for objects that hook into RenderingWindow to affect the rendering
    /// process, thus allowing various effects to be achieved.
    /// </summary>
    public interface RenderEffect
    {
        // TODO: virtual ~RenderEffect() {};
        
        /// <summary>
        /// Return the number of passes required by this effect.
        /// </summary>
        /// <returns>
        /// integer value indicating the number of rendering passes required to
        /// fully render this effect.
        /// </returns>
        int GetPassCount();

        /// <summary>
        /// Function called prior to RenderingWindow::draw being called.  This is
        /// intended to be used for any required setup / state initialisation and is
        /// called once for each pass in the effect.
        /// </summary>
        /// <param name="pass">
        /// Indicates the pass number to be initialised (starting at pass 0).
        /// </param>
        /// <remarks>
        /// Note that this function is called \e after any standard state
        /// initialisation that might be peformed by the Renderer module.
        /// </remarks>
        void PerformPreRenderFunctions(int pass);

        /// <summary>
        /// Function called after RenderingWindow::draw is called.  This is intended
        /// to be used for any required cleanup / state restoration.  This function
        /// is called <em>once only</em>, unlike performPreRenderFunctions which may
        /// be called multiple times; once for each pass in the effect.
        /// </summary>
        /// <remarks>
        /// Note that this function is called \e before any standard state
        /// cleanup that might be peformed by the Renderer module.
        /// </remarks>
        void PerformPostRenderFunctions();

        /// <summary>
        /// Function called to generate geometry for the RenderingWindow.
        /// 
        /// The geometry generated should be fully unclipped and window local.  The
        /// origin for the geometry is located at the top-left corner.
        /// </summary>
        /// <param name="window">
        /// The RenderingWindow object that is being processed.
        /// </param>
        /// <param name="geometry">
        /// GeometryBuffer object where the generated geometry should be added.
        /// This object will be cleared before this function is invoked.
        /// </param>
        /// <returns>
        /// boolean value indicating whether the RenderingWindow should generate
        /// it's own geometry.
        /// - true if the RenderingWindow should generate it's own geometry.  You
        ///   will usually only return true if you do not need to use custom geometry.
        /// - false if you have added any required geometry needed to represent the
        ///   RenderingWindow.
        /// </returns>
        bool RealiseGeometry(RenderingWindow window, GeometryBuffer geometry);

        /// <summary>
        /// Function called to perform any time based updates on the RenderEffect
        /// state.
        /// </summary>
        /// <remarks>
        /// This function should only affect the internal state of the RenderEffect
        /// object.  This function should definitely \e not be used to directly
        /// affect any render states of the underlying rendering API or engine.
        /// </remarks>
        /// <param name="elapsed">
        /// The number of seconds that have elapsed since the last time this
        /// function was called.
        /// </param>
        /// <param name="window">
        /// RenderingWindow object that the RenderEffect is being applied to.
        /// </param>
        /// <returns>
        /// boolean that indicates whether the window geometry will still be valid
        /// after the update.
        /// </returns>
        bool Update(float elapsed, RenderingWindow window);
    }
}