namespace SharpCEGui.Base
{
    /// <summary>
    /// The ShaderWrapper is an abstract interface that is used in RenderMaterials
    /// and has to be implemented by the specific Renderer. It updates all the
    /// ShaderParameters set for a RenderMaterial before rendering.
    /// </summary>
    public abstract class ShaderWrapper
    {
        // TODO: Destructor.
        //virtual ~ShaderWrapper();

        /// <summary>
        /// This function applies the shader parameters depending on their type,
        /// so that they will be used during rendering
        /// </summary>
        /// <param name="shaderParameterBindings">
        /// The ShaderParameterBindings that will be applied
        /// </param>
        public abstract void PrepareForRendering(ShaderParameterBindings shaderParameterBindings);
    }
}