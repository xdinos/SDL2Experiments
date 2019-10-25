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

namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WindowRendererManager
    {
        #region Singleton Impementation

        private static WindowRendererManager _instance;

        /// <summary>
        /// 
        /// </summary>
        public static WindowRendererManager Instance
        {
            get { return _instance ?? (_instance = new WindowRendererManager()); }
        }

        #endregion

        private WindowRendererManager()
        {
            System.GetSingleton().Logger
                  .LogEvent("CEGUI::WindowRendererManager singleton created " + GetHashCode().ToString("X8"));

            // complete addition of any pre-added WindowRendererFactory objects
            if (OwnedFactories.Count != 0)
            {
                System.GetSingleton().Logger
                      .LogEvent("---- Adding pre-registered WindowRendererFactory objects ----");

                foreach (var i in OwnedFactories)
                    AddFactory(i);
            }
        }

        // TODO: ~WindowRendererManager()
        //{
        //    char addr_buff[32];
        //    sprintf(addr_buff, "(%p)", static_cast<void*>(this));
        //    Logger::getSingleton().logEvent(
        //        "CEGUI::WindowRendererManager singleton destroyed " + String(addr_buff));
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsFactoryPresent(string name)
        {
            return _registry.ContainsKey(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WindowRendererFactory GetFactory(string name)
        {
            if (_registry.ContainsKey(name))
                return _registry[name];

            throw new UnknownObjectException("There is no WindowRendererFactory named '" + name + "' available");
        }

        /// <summary>
        /// Creates a WindowRendererFactory of the type \a T and adds it to the
        /// system for use. 
        /// The created WindowRendererFactory will automatically be deleted when the
        /// factory is removed from the system (either directly or at system
        /// deletion time).
        /// </summary>
        /// <typeparam name="T">
        /// Specifies the type of WindowRendererFactory subclass to add a factory for.
        /// </typeparam>
        public static void AddFactory<T>() where T : WindowRendererFactory, new()
        {
            // create the factory object
            WindowRendererFactory factory = new T();

            // only do the actual add now if our singleton has already been created
            if (_instance != null)
            {
                System.GetSingleton().Logger
                      .LogEvent("Created WindowRendererFactory for '" + factory.GetName() + "' WindowRenderers.");

                // add the factory we just created
                try
                {
                    Instance.AddFactory(factory);
                }
                catch (Exception)
                {
                    System.GetSingleton().Logger
                          .LogEvent("Deleted WindowRendererFactory for '" + factory.GetName() + "' WindowRenderers.");

                    // delete the factory object
                    //CEGUI_DELETE_AO factory;

                    throw;
                }
            }

            OwnedFactories.Add(factory);
        }

        /// <summary>
        /// Internally creates a factory suitable for creating WindowRenderer
        /// objects of the given type and adds it to the system.
        /// </summary>
        /// <typeparam name="T">
        /// Specifies the type of WindowRenderer to add a factory for.
        /// </typeparam>
        /// <remarks>
        /// The internally created factory is owned and managed by CEGUI,
        /// and will be automatically deleted when the WindowRenderer type is
        /// removed from the system - either directly by calling
        /// WindowRendererManager::removeFactory or at system shut down.
        /// </remarks>
        public static void AddWindowRendererType<T>() where T:WindowRenderer
        {
            AddFactory<TplWindowRendererFactory<T>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wr"></param>
        /// <exception cref="AlreadyExistsException"></exception>
        public void AddFactory(WindowRendererFactory wr)
        {
            if (wr == null)
                return;
            if (_registry.ContainsKey(wr.GetName()))
            {
                throw new AlreadyExistsException("A WindowRendererFactory named '" + wr.GetName() + "' already exist");
            }

            _registry.Add(wr.GetName(), wr);

            System.GetSingleton().Logger
                  .LogEvent("WindowRendererFactory '" + wr.GetName() + "' added. " + wr.GetHashCode().ToString("X8"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveFactory(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a WindowRenderer instance by factory name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WindowRenderer CreateWindowRenderer(string name)
        {
            var factory = GetFactory(name);
            return factory.Create();
        }

        /// <summary>
        /// Destroy a WindowRenderer using its factory
        /// </summary>
        /// <param name="wr"></param>
        public void DestroyWindowRenderer(WindowRenderer wr)
        {
            GetFactory(wr.GetName()).Destroy(wr);
        }

        private readonly Dictionary<string, WindowRendererFactory> _registry =
            new Dictionary<string, WindowRendererFactory>();

        /// <summary>
        /// Container that tracks WindowFactory objects we created ourselves.
        /// </summary>
        private static readonly List<WindowRendererFactory> OwnedFactories = new List<WindowRendererFactory>();

    }
}