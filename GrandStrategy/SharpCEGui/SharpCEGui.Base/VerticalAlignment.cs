namespace SharpCEGui.Base
{
    /// <summary>
    /// Enumerated type used when specifying vertical alignments for Element
    /// </summary>
    /// <seealso cref="HorizontalAlignment"/>
    public enum VerticalAlignment
    {
        /// <summary>
        /// Element's position specifies an offset of it's top edge from the top edge
        /// of it's parent.
        /// </summary>
        Top,

        /// <summary>
        /// Element's position specifies an offset of it's vertical centre from the
        /// vertical centre of it's parent.
        /// </summary>
        Centre,

        /// <summary>
        /// Element's position specifies an offset of it's bottom edge from the
        /// bottom edge of it's parent.
        /// </summary>
        Bottom
    }
}