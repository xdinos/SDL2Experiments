namespace SharpCEGui.Base
{
    /// <summary>
    /// EventArgs class passed to subscribers of RenderTarget events.
    /// </summary>
    public class RenderTargetEventArgs : EventArgs
    {
        public IRenderTarget RenderTarget { get; set; }

        public RenderTargetEventArgs(IRenderTarget renderTarget)
        {
            RenderTarget = renderTarget;
        }
    }
}