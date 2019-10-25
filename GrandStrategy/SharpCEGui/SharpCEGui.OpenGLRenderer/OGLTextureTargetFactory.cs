using SharpCEGui.Base;

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// This class allows us to implement a factory template for creating and
    /// destroying any type of TextureTarget.  The code that detects
    /// the computer's abilities will generate an appropriate factory for a
    /// TextureTarget based on what the host system can provide - or use the
    /// default 'null' factory if no suitable TextureTargets are available.
    /// </summary>
    internal class OGLTextureTargetFactory
    {
        public OGLTextureTargetFactory()
        {
        }

        // TODO: virtual ~OGLTextureTargetFactory() {}

        public virtual ITextureTarget Create(OpenGLRendererBase renderer)
        {
            return null;
        }
    }
}