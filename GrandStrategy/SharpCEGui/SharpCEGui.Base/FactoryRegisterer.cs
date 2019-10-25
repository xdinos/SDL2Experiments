namespace SharpCEGui.Base
{
    /// <summary>
    /// Base class encapsulating a type name and common parts of factory
    /// registration.
    /// </summary>
    public abstract class FactoryRegisterer
    {
        // TODO: Destructor.
        // TODO: virtual ~FactoryRegisterer();

        /// <summary>
        /// Perform registration (addition) of the factory for whichever
        /// type this class registers a factory for.
        /// </summary>
        public void RegisterFactory()
        {
            if (IsAlreadyRegistered())
            {
                System.GetSingleton().Logger
                    .LogEvent("Factory for '" + d_type +"' appears to be  already registered, skipping.", LoggingLevel.Informative);
            }
            else
                DoFactoryAdd();
        }

        /// <summary>
        /// Perform unregistration (removal) of the factory for whichever
        /// type this class registers a factory for.
        /// </summary>
        public abstract void UnregisterFactory();

        //! describes the type this class registers a factory for.
        public readonly string d_type;

        //! Constructor.
        protected FactoryRegisterer(string type)
        {
            d_type = type;
        }

        /// <summary>
        /// Function to do the actual addition of a factory to the CEGUI system.
        /// </summary>
        protected abstract void DoFactoryAdd();

        /// <summary>
        /// Function to check if factory for our type is already registered.
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsAlreadyRegistered();
    }
}