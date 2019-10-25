using System;
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Singleton class that manages creation and destruction of RenderEffect based
    /// objects.
    /// </summary>
    public class RenderEffectManager
    {
        #region Implementation of Singleton
        private readonly static Lazy<RenderEffectManager> Instance = new Lazy<RenderEffectManager>(()=>new RenderEffectManager());
        public static RenderEffectManager GetSingleton()
        {
            return Instance.Value;
        }
        #endregion

        //! Collection type used for the render effect registry
        //typedef std::map<String, RenderEffectFactory*, StringFastLessCompare CEGUI_MAP_ALLOC(String, RenderEffectFactory*)> RenderEffectRegistry;

        //! Collection type to track which effects we created with which factories
        //typedef std::map<RenderEffect*, RenderEffectFactory*, std::less<RenderEffect*> CEGUI_MAP_ALLOC(RenderEffect*, RenderEffectFactory*)> EffectCreatorMap;

        //! Collection of registered render effects
        private Dictionary<string, RenderEffectFactroy> d_effectRegistry = new Dictionary<string, RenderEffectFactroy>();
        
        //! Collection of effect instances we created (and the factory used)
        private Dictionary<RenderEffect, RenderEffectFactroy> d_effects = new Dictionary<RenderEffect, RenderEffectFactroy>();

        //! Iterator type that iterates over entries in the RenderEffectRegistry
        //typedef ConstMapIterator<RenderEffectRegistry> RenderEffectIterator;

        //! Constructor for RenderEffectManager objects.
        private RenderEffectManager()
        {
            throw new NotImplementedException();
        }

        // TODO: Destructor for RenderEffectManager objects.0
        // TODO: ~RenderEffectManager();

        /*!
        \brief
            Register a RenderEffect type with the system and associate it with the
            identifier \a name.

            This registers a RenderEffect based class, such that instances of that
            class can subsequently be created by requesting an effect using the
            specified identifier.

        \tparam T
            The RenderEffect based class to be instantiated when an effect is
            requested using the identifier \a name.

        \param name
            String object describing the identifier that the RenderEffect based
            class will be registered under.

        \exception AlreadyExistsException
            thrown if a RenderEffect is already registered using \a name.
        */
        public void AddEffect<T>(string name)
        {
            if (IsEffectAvailable(name))
                throw new AlreadyExistsException("A RenderEffect is already registered under the name '" + name + "'");

            // create an instance of a factory to create effects of type T
            d_effectRegistry[name] = new TplRenderEffectFactory<T>();

            System.GetSingleton().Logger.LogEvent("Registered RenderEffect named '" + name + "'");
        }

        /*!
        \brief
            Remove / unregister any RenderEffect using the specified identifier.

        \param name
            String object describing the identifier of the RenderEffect that is to
            be removed / unregistered.  If no such RenderEffect is present, no
            action is taken.

        \note
            You should avoid removing RenderEffect types that are still in use.
            Internally a factory system is employed for the creation and deletion
            of RenderEffect objects; if an effect - and therefore it's factory - is
            removed while instances are still active, it will not be possible to
            safely delete those RenderEffect object instances.
        */
        public void RemoveEffect(string name)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Return whether a RenderEffect has been registered under the specified
            name.

        \param name
            String object describing the identifier of a RenderEffect to test for.

        \return
            - true if a RenderEffect with the specified name is registered.
            - false if no RenderEffect with the specified name is registered.
        */
        public bool IsEffectAvailable(string name)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Create an instance of the RenderEffect based class identified by the
            specified name.

        \param name
            String object describing the identifier of the RenderEffect based
            class that is to be created.

        \param window
            Pointer to a Window object.  Exactly how or if this is used will
            depend upon the specific effect being created.

        \return
            Reference to the newly created RenderEffect.

        \exception UnknownObjectException
            thrown if no RenderEffect class has been registered using the
            identifier \a name.
        */
        public RenderEffect Create(string name, Window window)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Destroy the given RenderEffect object.

        \note
            This function will only destroy objects that were created via the
            RenderEffectManager.  Attempts to destroy objects created by other
            means will result in an InvalidRequestException.  This option was
            chosen over silently ignoring the request in order to aid application
            developers in thier debugging.

        \param effect
            Reference to the RenderEffect object that is to be destroyed.

        \exception InvalidRequestException
            thrown if \a effect was not created by the RenderEffectManager.
        */
        public void Destroy(RenderEffect effect)
        {
            throw new NotImplementedException();
        }
    }
}