using OpenTK;
#if !__MACOS__
using OpenTK.Graphics.OpenGL;
#endif

namespace SharpCEGui.OpenGLRenderer
{
    public static class MathematicsExtensions
    {
        public static Matrix4 ToOpenTK(this Icehole.Mathematics.Matrix @this)
        {
            return new Matrix4(@this.M11, @this.M12, @this.M13, @this.M14,
                               @this.M21, @this.M22, @this.M23, @this.M24,
                               @this.M31, @this.M32, @this.M33, @this.M34,
                               @this.M41, @this.M42, @this.M43, @this.M44);
        }
    }
}