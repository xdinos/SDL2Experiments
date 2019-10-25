namespace SharpCEGui.Base
{
    /// <summary>
    /// Enumerated type used when specifying horizontal alignments for Element
    /// </summary>
    /// <seealso cref="VerticalAlignment"/>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// Element's position specifies an offset of it's left edge from the left
        /// edge of it's parent.
        /// </summary>
        Left,

        /// <summary>
        /// Element's position specifies an offset of it's horizontal centre from the
        /// horizontal centre of it's parent.
        /// </summary>
        Centre,

        /// <summary>
        /// Element's position specifies an offset of it's right edge from the right
        /// edge of it's parent.
        /// </summary>
        Right
    }
}