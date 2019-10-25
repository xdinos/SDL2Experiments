#region Copyright
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2013
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpCEGui.Base
{
    public class WindowFactoryManager
    {
        #region Singleton Impementation
        private readonly static Lazy<WindowFactoryManager> Instance = new Lazy<WindowFactoryManager>(() => new WindowFactoryManager());
        public static WindowFactoryManager GetSingleton()
        {
            return Instance.Value;
        }
        #endregion

        /// <summary>
        /// struct used to hold mapping information required to create a falagard based window.
        /// </summary>
        public struct FalagardWindowMapping
        {
            public string d_windowType;
            public string d_lookName;
            public string d_baseType;
            public string d_rendererType;
            public string d_effectName;
        };

        /// <summary>
        /// Class used to track active alias targets for Window factory types.
        /// </summary>
	    public class AliasTargetStack
	    {
            /// <summary>
            /// Constructor for WindowAliasTargetStack objects
            /// </summary>
		    public AliasTargetStack() {}

            // TODO: Destructor for WindowAliasTargetStack objects
		    // TODO: ~AliasTargetStack(void) {}


		    /*!
		    \brief
			    Return a String holding the current target type for this stack

		    \return
			    reference to a String object holding the currently active target type name for this stack.
		    */
		    public string GetActiveTarget()
		    {
		        return d_targetStack.Peek();
		    }

		    /*!
		    \brief
			    Return the number of stacked target types in the stack

		    \return
			    number of target types stacked for this alias.
		    */
		    public uint GetStackedTargetCount()
		    {
		        return (uint)d_targetStack.Count;
		    }


	        //friend class WindowFactoryManager;

            private Stack<string> d_targetStack; //!< Container holding the target types.
	    };


	    /*************************************************************************
		    Construction and Destruction
	    *************************************************************************/
	    
        /// <summary>
        /// Constructs a new WindowFactoryManager object.
        /// </summary>
	    private WindowFactoryManager()
	    {
	        System.GetSingleton().Logger.LogEvent("CEGUI::WindowFactoryManager singleton created");

            // complete addition of any pre-added WindowFactory objects
            if (d_ownedFactories.Count != 0)
            {
                System.GetSingleton().Logger.LogEvent("---- Adding pre-registered WindowFactory objects ----");
                foreach (var i in d_ownedFactories)
                {
                    AddFactory(i);
                }
            }
	    }


	    /*!
	    \brief
		    Destructor for WindowFactoryManager objects
	    */
        //~WindowFactoryManager(void)
        //{
        //    Logger::getSingleton().logEvent("CEGUI::WindowFactoryManager singleton destroyed");
        //}



        /// <summary>
        /// Adds a new WindowFactory to the list of registered factories.
        /// </summary>
        /// <param name="factory">
        /// Pointer to the WindowFactory to be added to the WindowManager.
        /// </param>
        /// <exception cref="ArgumentNullException">factory was null</exception>
        /// <exception cref="AlreadyExistsException">factory provided a Window type name which is in use by another registered WindowFactory.</exception>
	    public void AddFactory(WindowFactory factory)
	    {
	        // throw exception if passed factory is null.
            if (factory == null)
                throw new ArgumentNullException("The provided WindowFactory pointer was invalid.");

            // throw exception if type name for factory is already in use
            if (d_factoryRegistry.ContainsKey(factory.GetTypeName()))
                throw new AlreadyExistsException("A WindowFactory for type '" + factory.GetTypeName() +
                                                 "' is already registered.");

	        // add the factory to the registry
	        d_factoryRegistry[factory.GetTypeName()] = factory;

            System.GetSingleton().Logger
                  .LogEvent("WindowFactory for '" + factory.GetTypeName() + "' windows added. " +
                            factory.GetHashCode().ToString("X8"));
	    }

        /// <summary>
        /// Creates a WindowFactory of the type \a T and adds it to the system for
        /// use. The created WindowFactory will automatically be deleted when the
        /// factory is removed from the system (either directly or at system 
        /// deletion time).
        /// </summary>
        /// <typeparam name="TFactory">
        /// Specifies the type of WindowFactory subclass to add a factory for.
        /// </typeparam>
        public static void AddFactory<TFactory>() where TFactory : WindowFactory, new()
        {
            // create the factory object
            var factory = new TFactory();

            // only do the actual add now if our singleton has already been created
            if (Instance.IsValueCreated)
            {
                System.GetSingleton().Logger
                      .LogEvent("Created WindowFactory for '" + factory.GetTypeName() + "' windows.");

                // add the factory we just created
                try
                {
                    GetSingleton().AddFactory(factory);
                }
                catch
                {
                    System.GetSingleton().Logger
                          .LogEvent("Deleted WindowFactory for '" + factory.GetTypeName() + "' windows.");
                    // delete the factory object
                    //CEGUI_DELETE_AO factory;

                    throw;
                }
            }

            d_ownedFactories.Add(factory);
        }

        /// <summary>
        /// Internally creates a factory suitable for creating Window objects
        /// of the given type and adds it to the system.
        /// </summary>
        /// <typeparam name="TWindow">
        /// Specifies the type of Window to add a factory for.
        /// </typeparam>
        /// <remarks>
        /// The internally created factory is owned and managed by CEGUI,
        /// and will be automatically deleted when the window type is removed from
        /// the system - either directly by calling
        /// WindowFactoryManager::removeFactory or at system shut down.
        /// </remarks>
        public static void AddWindowType<TWindow>() where TWindow : Window
        {
            AddFactory<TplWindowFactory<TWindow>>();
        }

        /// <summary>
        /// Removes a WindowFactory from the list of registered factories.
        /// <para>
        /// The WindowFactory object is not destroyed (since it was created externally), 
        /// instead it is just removed from the list.
        /// </para>
        /// </summary>
        /// <param name="name">
        /// String which holds the name (technically, Window type name) of the WindowFactory to be removed.  
        /// If \a name is not in the list, no error occurs (nothing happens).
        /// </param>
        public void RemoveFactory(string name)
        {
            // exit if no factory exists for this type
            if (!d_factoryRegistry.ContainsKey(name))
                return;

            // see if we own this factory
            var j = d_ownedFactories.SingleOrDefault(x => x == d_factoryRegistry[name]);

            var addr_buff = d_factoryRegistry[name].GetHashCode().ToString("X8");

            d_factoryRegistry.Remove(name);

            System.GetSingleton().Logger
                  .LogEvent("WindowFactory for '" + name + "' windows removed. " + addr_buff);

            // delete factory object if we created it
            if (j != null)
            {
                System.GetSingleton().Logger
                      .LogEvent("Deleted WindowFactory for '" + j.GetTypeName() + "' windows.");

                //CEGUI_DELETE_AO (*j);
                d_ownedFactories.Remove(j);
            }
        }

        /// <summary>
        /// Removes a WindowFactory from the list of registered factories.
        /// <para>
        /// The WindowFactory object is not destroyed (since it was created externally), 
        /// instead it is just removed from the list.
        /// </para>
        /// </summary>
        /// <param name="factory">
        /// Pointer to the factory object to be removed.  
        /// If \a factory is null, or if no such WindowFactory is in the list, 
        /// no error occurs (nothing happens).
        /// </param>
	    public void RemoveFactory(WindowFactory factory)
	    {
            if (factory!=null)
                RemoveFactory(factory.GetTypeName());
	    }

        /// <summary>
        /// Remove all WindowFactory objects from the list.
        /// </summary>
        public void RemoveAllFactories()
        {
            while (d_factoryRegistry.Count != 0)
                RemoveFactory(d_factoryRegistry.First().Value);
        }

        /// <summary>
        /// Return a pointer to the specified WindowFactory object.
        /// </summary>
        /// <param name="type">
        /// String holding the Window object type to return the WindowFactory for.
        /// </param>
        /// <returns>
        /// Pointer to the WindowFactory object that creates Windows of the type \a type.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// No WindowFactory object for Window objects of type \a type was found.
        /// </exception>
        public WindowFactory GetFactory(string type)
        {
            // first, dereference aliased types, as needed.
            var targetType = GetDereferencedAliasType(type);

            // try for a 'real' type
            // found an actual factory for this type
            if (d_factoryRegistry.ContainsKey(targetType))
                return d_factoryRegistry[targetType];

            // no concrete type, try for a falagard mapped type
            // found falagard mapping for this type
            if (d_falagardRegistry.ContainsKey(targetType))
            {
                // recursively call getFactory on the target base type
                return GetFactory(d_falagardRegistry[targetType].d_baseType);
            }

            // type not found anywhere, give up with an exception.
            throw new UnknownObjectException(
                "A WindowFactory object, an alias, or mapping for '" + type +
                "' Window objects is not registered with the system.");
        }

        /*!
        \brief
            Checks the list of registered WindowFactory objects, aliases, and
            falagard mapped types for one which can create Window objects of the
            specified type.

        \param name
            String containing the Window type name to check for.

        \return
            - true if a WindowFactory, alias, or falagard mapping for Window objects
              of type \a name is registered.
            - false if the system knows nothing about windows of type \a name.
        */
        public bool IsFactoryPresent(string name)
        {
            throw new NotImplementedException();
        }


	    /*!
	    \brief
		    Adds an alias for a current window type.

		    This method allows you to create an alias for a specified window type.  This means that you can then use
		    either name as the type parameter when creating a window.

	    \note
		    You need to be careful using this system.  Creating an alias using a name that already exists will replace the previous
		    mapping for that alias.  Each alias name maintains a stack, which means that it is possible to remove an alias and have the
		    previous alias restored.  The windows created via an alias use the real type, so removing an alias after window creation is always
		    safe (i.e. it is not the same as removing a real factory, which would cause an exception when trying to destroy a window with a missing
		    factory).

	    \param aliasName
		    String object holding the alias name.  That is the name that \a targetType will also be known as from no on.

	    \param targetType
		    String object holding the type window type name that is to be aliased.  This type must already exist.

	    \return
		    Nothing.

	    \exception UnknownObjectException	thrown if \a targetType is not known within the system.
	    */
	    public void AddWindowTypeAlias(string aliasName, string targetType)
	    {
	        throw new NotImplementedException();
	    }


	    /*!
	    \brief
		    Remove the specified alias mapping.  If the alias mapping does not exist, nothing happens.

	    \note
		    You are required to supply both the alias and target names because there may exist more than one entry for a given
		    alias - therefore you are required to be explicit about which alias is to be removed.

	    \param aliasName
		    String object holding the alias name.

	    \param targetType
		    String object holding the type window type name that was aliased.

	    \return
		    Nothing.
	    */
	    public void RemoveWindowTypeAlias(string aliasName, string targetType)
	    {
	        throw new NotImplementedException();
	    }

	    /*!
	    \brief
		    Remove all registered window type alias mappings.
	    */
	    public void RemoveAllWindowTypeAliases()
	    {
	        throw new NotImplementedException();
	    }

        /// <summary>
        /// Add a mapping for a falagard based window.
        /// 
        /// This function creates maps a target window type and target 'look' name onto a registered window type, thus allowing
        /// the ususal window creation interface to be used to create windows that require extra information to full initialise
        /// themselves.
        /// </summary>
        /// <remarks>
        /// These mappings support 'late binding' to the target window type, as such the type indicated by \a targetType need not
        /// exist in the system until attempting to create a Window using the type.
        /// <para>
        /// Also note that creating a mapping for an existing type will replace any previous mapping for that same type.
        /// </para>
        /// </remarks>
        /// <param name="newType">
        /// The type name that will be used to create windows using the target type and look.
        /// </param>
        /// <param name="targetType">
        /// The base window type.
        /// </param>
        /// <param name="lookName">
        /// The name of the 'look' that will be used by windows of this type.
        /// </param>
        /// <param name="renderer">
        /// The type of window renderer to assign for windows of this type.
        /// </param>
        /// <param name="effectName">
        /// The identifier of the RenderEffect to attempt to set up for windows of this type.
        /// </param>
        public void AddFalagardWindowMapping(string newType,
                                             string targetType,
                                             string lookName,
                                             string renderer,
                                             string effectName = "")
        {
            var mapping = new FalagardWindowMapping
                              {
                                  d_windowType = newType,
                                  d_baseType = targetType,
                                  d_lookName = lookName,
                                  d_rendererType = renderer,
                                  d_effectName = effectName
                              };

            // see if the type we're creating already exists
            if (d_falagardRegistry.ContainsKey(newType))
            {
                // type already exists, log the fact that it's going to be replaced.
                System.GetSingleton().Logger
                      .LogEvent("Falagard mapping for type '" + newType +
                                "' already exists - current mapping will be replaced.");
            }

            System.GetSingleton().Logger.
                   LogEvent("Creating falagard mapping for type '" +
                            newType + "' using base type '" + targetType + "', window renderer '" +
                            renderer + "' Look'N'Feel '" + lookName + "' and RenderEffect '" +
                            effectName + "'. " + mapping.GetHashCode().ToString("X8"));

            d_falagardRegistry[newType] = mapping;
        }

        /*!
        \brief
            Remove the specified falagard type mapping if it exists.

        \return
            Nothing.
        */
        public void RemoveFalagardWindowMapping(string type)
        {
            throw new NotImplementedException();
        }

        /*!
	    \brief
		    Remove all registered falagard type mappings
	    */
        public void removeAllFalagardWindowMappings()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return whether the given type is a falagard mapped type.
        /// </summary>
        /// <param name="type">
        /// Name of a window type.
        /// </param>
        /// <returns>
        /// - true if the requested type is a Falagard mapped window type.
        /// - false if the requested type is a normal WindowFactory (or alias), 
        ///   or if the type does not exist.
        /// </returns>
        public bool IsFalagardMappedType(string type)
        {
            return d_falagardRegistry.ContainsKey(type);
        }

        /*!
        \brief
            Return the name of the LookN'Feel assigned to the specified window mapping.

        \param type
            Name of a window type.  The window type referenced should be a falagard mapped type.

        \return
            String object holding the name of the look mapped for the requested type.

        \exception InvalidRequestException thrown if \a type is not a falagard mapping type (or maybe the type didn't exist).
        */
        public string GetMappedLookForType(string type)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Return the name of the WindowRenderer assigned to the specified window mapping.

        \param type
            Name of a window type.  The window type referenced should be a falagard mapped type.

        \return
            String object holding the name of the window renderer mapped for the requested type.

        \exception InvalidRequestException thrown if \a type is not a falagard mapping type (or maybe the type didn't exist).
        */
        public string GetMappedRendererForType(string type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use the alias system, where required, to 'de-reference' the specified
        /// type to an actual window type that can be created directly (that being
        /// either a concrete window type, or a falagard mapped type).
        /// </summary>
        /// <param name="type">
        /// String describing the type to be de-referenced.
        /// </param>
        /// <returns>
        /// String object holding a type for a window that can be created directly;
        /// that is, a type that does not describe an alias to some other type.
        /// </returns>
        /// <remarks>
        /// Even though implied by the above description, this method does not
        /// check that a factory for the final type exists; we simply say that the
        /// returned type is not an alias for some other type.
        /// </remarks>
        public string GetDereferencedAliasType(string type)
        {
            // if this is an aliased type, ensure to fully dereference by recursively
            // calling ourselves on the active target for the given type.
            if (d_aliasRegistry.ContainsKey(type))
                return GetDereferencedAliasType(d_aliasRegistry[type].GetActiveTarget());
            
            // we're not an alias, so return the input type unchanged
            return type;
        }

        /// <summary>
        /// Return the FalagardWindowMapping for the specified window mapping \a type.
        /// </summary>
        /// <param name="type">
        /// Name of a window type.  
        /// The window type referenced should be a falagard mapped type.
        /// </param>
        /// <returns>
        /// FalagardWindowMapping object describing the falagard mapping.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a type is not a falagard mapping type (or maybe the type didn't exist).
        /// </exception>
        public FalagardWindowMapping GetFalagardMappingForType(string type)
        {
            if (d_falagardRegistry.ContainsKey(type))
            {
                return d_falagardRegistry[type];
            }
            
            // type does not exist as a mapped type (or an alias for one)
            throw new InvalidRequestException("Window factory type '" + type +
                                              "' is not a falagard mapped type (or an alias for one).");

        }


        //!< The container that forms the WindowFactory registry
        private Dictionary<string, WindowFactory> d_factoryRegistry =new Dictionary<string, WindowFactory>();

        //!< The container that forms the window type alias registry.
        private Dictionary<string, AliasTargetStack> d_aliasRegistry =new Dictionary<string, AliasTargetStack>();

        //!< Container that hold all the falagard window mappings.
        private Dictionary<string, FalagardWindowMapping> d_falagardRegistry = new Dictionary<string, FalagardWindowMapping>();

        //! Container that tracks WindowFactory objects we created ourselves.
        private static List<WindowFactory> d_ownedFactories = new List<WindowFactory>();

    //public:
    //    /*************************************************************************
    //        Iterator stuff
    //    *************************************************************************/
    //    typedef	ConstMapIterator<WindowFactoryRegistry>	WindowFactoryIterator;
    //    typedef ConstMapIterator<TypeAliasRegistry>		TypeAliasIterator;
    //    typedef ConstMapIterator<FalagardMapRegistry>   FalagardMappingIterator;

    //    /*!
    //    \brief
    //        Return a WindowFactoryManager::WindowFactoryIterator object to iterate over the available WindowFactory types.
    //    */
    //    WindowFactoryIterator	getIterator(void) const;


    //    /*!
    //    \brief
    //        Return a WindowFactoryManager::TypeAliasIterator object to iterate over the defined aliases for window types.
    //    */
        public Dictionary<string, AliasTargetStack>.Enumerator getAliasIterator()
        {
            return d_aliasRegistry.GetEnumerator();
        }


    //    /*!
    //    \brief
    //        Return a WindowFactoryManager::FalagardMappingIterator object to iterate over the defined falagard window mappings.
    //    */
        public IEnumerable<KeyValuePair<string, FalagardWindowMapping>> getFalagardMappingIterator()
        {
            return d_falagardRegistry;
        }
    }
}