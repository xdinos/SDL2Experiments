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

using System.Runtime.InteropServices;

namespace SharpCEGui.Base
{
    //[StructLayout(LayoutKind.Sequential)]
    //public struct Vertex
    //{
    //    /// <summary>
    //    /// Position of the vertex in 3D space.
    //    /// </summary>
    //    public Vector3f Position;

    //    /// <summary>
    //    /// Texture coordinates to be applied to the vertex.
    //    /// </summary>
    //    public Vector2f TextureCoordinates;

    //    /// <summary>
    //    /// Color to be applied to the vertex.
    //    /// </summary>
    //    public Colour Colour { get; set; }
    //}

    /// <summary>
    /// Structure that is used to hold the attributes of a vertex for coloured and
    /// textured geometry in 3D space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TexturedColouredVertex
    {
        /// <summary>
        /// Position of the vertex in 3D space.
        /// </summary>
        public Lunatics.Mathematics.Vector3 Position;

        /// <summary>
        /// Multiplicative-colour attribute of the vertex.
        /// </summary>
        public Lunatics.Mathematics.Vector4 Colour;

        /// <summary>
        /// Texture coordinates of the vertex.
        /// </summary>
        public Lunatics.Mathematics.Vector2 TextureCoordinates;

        /// <summary>
        /// Sets the colour of the struct
        /// </summary>
        /// <param name="colour"></param>
        public void SetColour(Colour colour)
        {
            Colour = new Lunatics.Mathematics.Vector4(colour.Red, colour.Green, colour.Blue, colour.Alpha);
        }
    }

    /// <summary>
    /// Structure that is used to hold the attributes of coloured geometry
    /// in 3D space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ColouredVertex
    {
        /// <summary>
        /// Position of the vertex in 3D space.
        /// </summary>
        public Lunatics.Mathematics.Vector3 Position;

        /// <summary>
        /// Colour attribute of the vertex.
        /// </summary>
        public Lunatics.Mathematics.Vector4 Colour;

        /// <summary>
        /// Sets the colour of the struct
        /// </summary>
        /// <param name="colour"></param>
        public void SetColour(Colour colour)
        {
            Colour = new Lunatics.Mathematics.Vector4(colour.Red, colour.Green, colour.Blue, colour.Alpha);
        }
    }
}