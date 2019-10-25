using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class FactoryModule
    {
        // TODO: Destructor.
        // TODO: virtual ~FactoryModule();

        /// <summary>
        /// Register the factory for objects of the specified type.
        /// </summary>
        /// <param name="typeName"></param>
        public void RegisterFactory(string typeName)
        {
            foreach (var i in Registry)
            {
                if (i.d_type == typeName)
                {
                    i.RegisterFactory();
                    return;
                }
            }

            throw new UnknownObjectException("No factory for type '" + typeName + "' in this module.");
        }

        /// <summary>
        /// Register factories for all object types in the module.
        /// </summary>
        /// <returns></returns>
        public uint RegisterAllFactories()
        {
            foreach (var i in Registry)
                i.RegisterFactory();

            return (uint)Registry.Count;
        }

        /// <summary>
        /// Unregister the factory for objects of the specified type.
        /// </summary>
        /// <param name="typeName"></param>
        public void UnregisterFactory(string typeName)
        {
            foreach (var i in Registry)
            {
                if (i.d_type == typeName)
                {
                    i.UnregisterFactory();
                    return;
                }
            }
        }

        /// <summary>
        /// Unregister factories for all object types in the module.
        /// </summary>
        /// <returns></returns>
        public uint UnregisterAllFactories()
        {
            foreach (var i in Registry)
                i.UnregisterFactory();

            return (uint) Registry.Count;
        }

        //! Collection type that holds pointers to the factory registerer objects.
        //typedef std::vector<FactoryRegisterer*CEGUI_VECTOR_ALLOC(FactoryRegisterer*)> FactoryRegistry;

        /// <summary>
        /// The collection of factorty registerer object pointers.
        /// </summary>
        protected readonly List<FactoryRegisterer> Registry = new List<FactoryRegisterer>();
    }
}