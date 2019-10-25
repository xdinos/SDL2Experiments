namespace SharpCEGui.Base
{
    /// <summary>
    /// A RenderMaterial is used for rendering GeometryBuffers. It contains
    /// a pointer to the used shader (ShaderWrapper) and owns shader parameters.
    /// </summary>
    public class RenderMaterial
    {
        public RenderMaterial(ShaderWrapper shaderWrapper)
        {
            d_shaderWrapper = shaderWrapper;
            d_shaderParamBindings = new ShaderParameterBindings();
        }

        // TODO: Destructor.
        //virtual ~RenderMaterial();

        /// <summary>
        /// Return the ShaderParameterBindings of this Renderer.
        /// </summary>
        /// <returns>
        /// The pointer to the ShaderParameterBindings.
        /// </returns>
        public ShaderParameterBindings GetShaderParamBindings()
        {
            return d_shaderParamBindings;
        }

        /// <summary>
        /// Returns a pointer to the ShaderWrapper used for this Renderer.
        /// </summary>
        /// <returns>
        /// The pointer to the ShaderWrapper used for this Renderer.
        /// </returns>
        public ShaderWrapper GetShaderWrapper()
        {
            return d_shaderWrapper;
        }

        /// <summary>
        /// Applies the shader parameter bindings to the shader of this material.
        /// </summary>
        public void PrepareForRendering()
        {
            d_shaderWrapper.PrepareForRendering(d_shaderParamBindings);
        }

        /// <summary>
        /// pointer to the Shader that is used in this material.
        /// </summary>
        protected ShaderWrapper d_shaderWrapper;

        /// <summary>
        /// data structure that contains all the shader parameters that are
        /// to be sent to the shader program using the parameter's name
        /// </summary>
        protected ShaderParameterBindings d_shaderParamBindings;
    }
}