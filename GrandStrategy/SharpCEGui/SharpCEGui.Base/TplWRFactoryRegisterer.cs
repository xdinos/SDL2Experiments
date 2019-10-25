namespace SharpCEGui.Base
{
    /// <summary>
    /// Template based implementation of FactoryRegisterer that allows easy
    /// registration of a factory for any WindowRenderer type.
    /// </summary>
    public class TplWRFactoryRegisterer<T> : FactoryRegisterer where T : WindowRenderer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TplWRFactoryRegisterer()
            : base((string) typeof (T).GetField("TypeName").GetValue(null))
        {
        }

        public override void UnregisterFactory()
        {
            WindowRendererManager.Instance.RemoveFactory(d_type);
        }

        protected override void DoFactoryAdd()
        {
            WindowRendererManager.AddWindowRendererType<T>();
        }

        protected override bool IsAlreadyRegistered()
        {
            return WindowRendererManager.Instance.IsFactoryPresent(d_type);
        }
    }
}