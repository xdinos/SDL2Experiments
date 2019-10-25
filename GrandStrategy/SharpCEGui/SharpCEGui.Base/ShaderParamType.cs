namespace SharpCEGui.Base
{
    /// <summary>
    /// Enumerated type that contains the valid types a shader parameter can have
    /// </summary>
    public enum ShaderParamType
    {
        /// <summary>
        /// A regular integer type
        /// </summary>
        SPT_INT,

        /// <summary>
        /// A regular float type
        /// </summary>
        SPT_FLOAT,

        /// <summary>
        /// A pointer to a CEGUI Texture
        /// </summary>
        SPT_TEXTURE,

        /// <summary>
        /// A pointer to a 4x4 Matrix
        /// </summary>
        SPT_MATRIX_4X4,

        /// <summary>
        /// Total number of shader parameter types
        /// </summary>
        SPT_COUNT
    };
}