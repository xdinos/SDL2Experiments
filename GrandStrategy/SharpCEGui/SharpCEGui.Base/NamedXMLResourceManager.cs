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
    /*!
\brief
    Templatised manager class that loads and manages named XML based resources.

\tparam T
    Type for the object that is loaded and managed by the class.

\tparam U
    Type for the loader used to create object instances of T from XML files.
    This class does all of the creation / loading work and can be of any form
    or type desired, with the following requirements:
    - a constructor signature compatible with the following call:
      U(const String& xml_filename, const String& resource_group);
    - a function getObjectName that returns the name of the object as specified
      in the XML file given in the constructor to U.
    - a function getObject that returns a reference to the object created as a
      result of processing the XML file given in the constructor to U.
\note
    Once NamedXMLResourceManager calls getObject on the instance of U, it
    assumes that it now owns the object returned, if no call to getObject is
    made, no such transfer of ownership is assumed.
*/

    public class NamedXMLResourceManager<T, U> : IDisposable where U : XmlHandler,IXmlHandler<T>, new() where T: IDisposable
    {
        #region ResourceEventSet

        // implementation class to gather EventSet parts for all template instances.

        /** Name of event fired when a resource is created by this manager.
        * Handlers are passed a const ResourceEventArgs reference with
        * ResourceEventArgs::resourceType String set to the type of resource that
        * the event is related to, and ResourceEventArgs::resourceName String set
        * to the name of the resource that the event is related to.
        */
        public event EventHandler<ResourceEventArgs> ResourceCreated;

        /** Name of event fired when a resource is destroyed by this manager.
        * Handlers are passed a const ResourceEventArgs reference with
        * ResourceEventArgs::resourceType String set to the type of resource that
        * the event is related to, and ResourceEventArgs::resourceName String set
        * to the name of the resource that the event is related to.
        */
        public event EventHandler<ResourceEventArgs> ResourceDestroyed;

        /** Name of event fired when a resource is replaced by this manager.
        * Handlers are passed a const ResourceEventArgs reference with
        * ResourceEventArgs::resourceType String set to the type of resource that
        * the event is related to, and ResourceEventArgs::resourceName String set
        * to the name of the resource that the event is related to.
        */
        public event EventHandler<ResourceEventArgs> ResourceReplaced;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resource_type">
        /// String object holding some textual indication of the type of objects
        /// managed by the collection.  This is used to give more descriptive
        /// log and exception messages.
        /// </param>
        public NamedXMLResourceManager(string resource_type)
        {
            _resourceType = resource_type;
        }

        // TODO: Destructor.
        // virtual ~NamedXMLResourceManager();

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        #endregion

        /*!
        \brief
            Creates a new T object from a RawDataContainer and adds it to the collection.

            Use an instance of the xml resource loading class \a U to process the
            XML source thereby creating an instance of class \a T and add it to the collection under
            the name specified in the XML file.

        \param source
            RawDataContainer holding the XML source to be used when creating the
            new object instance.

        \param action
            One of the XMLResourceExistsAction enumerated values indicating what
            action should be taken when an object with the specified name
            already exists within the collection.
        */

        public T CreateFromContainer(RawDataContainer source,
                                     XMLResourceExistsAction action = XMLResourceExistsAction.XREA_RETURN)
        {
            var xmlLoader = new U();

            xmlLoader.HandleContainer(source);

            return DoExistingObjectAction(xmlLoader.GetObjectName(),
                                          xmlLoader.GetObject(), action);
        }

        /// <summary>
        /// Creates a new T object from an XML file and adds it to the collection.
        /// <para>
        /// Use an instance of the xml resource loading class \a U to process the
        /// XML file \a xml_filename from resource group \a resource_group thereby
        /// creating an instance of class \a T and add it to the collection under
        /// the name specified in the XML file.
        /// </para>
        /// </summary>
        /// <param name="xmlFilename">
        /// String holding the filename of the XML file to be used when creating the
        /// new object instance.
        /// </param>
        /// <param name="resourceGroup">
        /// String holding the name of the resource group identifier to be used
        /// when loading the XML file described by \a xml_filename.
        /// </param>
        /// <param name="action">
        /// One of the XMLResourceExistsAction enumerated values indicating what
        /// action should be taken when an object with the specified name
        /// already exists within the collection.
        /// </param>
        /// <returns></returns>
        public T CreateFromFile(string xmlFilename, string resourceGroup = "",
                                XMLResourceExistsAction action = XMLResourceExistsAction.XREA_RETURN)
        {
            var xmlLoader = new U();

            xmlLoader.HandleFile(xmlFilename, resourceGroup);
            return DoExistingObjectAction(xmlLoader.GetObjectName(),
                                          xmlLoader.GetObject(), action);
        }

        /*!
        \brief
            Creates a new T object from a string and adds it to the collection.

            Use an instance of the xml resource loading class \a U to process the
            XML source thereby creating an instance of class \a T and add it to the collection under
            the name specified in the XML file.

        \param source
            String holding the XML source to be used when creating the
            new object instance.

        \param action
            One of the XMLResourceExistsAction enumerated values indicating what
            action should be taken when an object with the specified name
            already exists within the collection.
        */

        public T CreateFromString(string source, XMLResourceExistsAction action = XMLResourceExistsAction.XREA_RETURN)
        {
            var xmlLoader = new U();

            xmlLoader.HandleString(source);
            return DoExistingObjectAction(xmlLoader.GetObjectName(),
                                          xmlLoader.GetObject(), action);
        }

        /// <summary>
        /// Destroy the object named \a object_name, or do nothing if such an
        /// object does not exist in the collection.
        /// </summary>
        /// <param name="object_name">
        /// String holding the name of the object to be destroyed.
        /// </param>
        public void Destroy(string object_name)
        {
            //typename ObjectRegistry::iterator i(d_objects.find(object_name));

            //// exit if no such object.
            //if (i == d_objects.end())
            //    return;

            //DestroyObject(i);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Destroy the object \a object, or do nothing if such an
        /// object does not exist in the collection.
        /// </summary>
        /// <param name="object">
        /// The object to be destroyed (beware of keeping references to this object
        /// once it's been destroyed!)
        /// </param>
        public void Destroy(T @object)
        {
            // don't want to force a 'getName' function on T here, so we'll look for the
            // object the hard way.
            //typename ObjectRegistry::iterator i(d_objects.begin());
            //for (; i != d_objects.end(); ++i)
            //{
            //    if (i->second == &object)
            //    {
            //        destroyObject(i);
            //        return;
            //    }
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        /// Destroy all objects.
        /// </summary>
        public void DestroyAll()
        {
            while (ObjectRegistry.Count != 0)
            {
                var i = ObjectRegistry.First();
                DestroyObject(ref i);
            }
        }
        
        /// <summary>
        /// Return a reference to the object named \a object_name.
        /// </summary>
        /// <param name="objectName">
        /// String holding the name of the object to be returned.
        /// </param>
        /// <returns></returns>
        /// <exception cref="UnknownObjectException">
        /// thrown if no object named \a object_name exists within the collection.
        /// </exception>
        public T Get(string objectName)
        {
            if (!ObjectRegistry.ContainsKey(objectName))
                throw new UnknownObjectException("No object of type '" + _resourceType + "' named '" + objectName +
                                                 "' is present in the collection.");

            return ObjectRegistry[objectName];
        }

        /// <summary>
        /// Return whether an object named \a object_name exists.
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public bool IsDefined(string objectName)
        {
            return ObjectRegistry.ContainsKey(objectName);
        }

        /// <summary>
        /// Create a new T object from files with names matching \a pattern in \a resource_group
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="resourceGroup"></param>
        public void CreateAll(string pattern, string resourceGroup)
        {
            var names = System.GetSingleton()
                              .GetResourceProvider()
                              .GetResourceGroupFileNames(pattern, resourceGroup);
            foreach (var name in names)
            {
                CreateFromFile(name, resourceGroup);
            }
        }

        /// <summary>
        /// implementation of object destruction.
        /// </summary>
        /// <param name="ob"></param>
        protected void DestroyObject(ref KeyValuePair<string, T> ob)
        {
            System.GetSingleton().Logger
                  .LogEvent("Object of type '" + _resourceType + "' named '" + ob.Key + "' has been destroyed. " +
                            ob.Value.GetHashCode().ToString("X8"), LoggingLevel.Informative);

            // Set up event args for event notification
            var args = new ResourceEventArgs(_resourceType, ob.Key);

            //CEGUI_DELETE_AO ob->second;
            ob.Value.Dispose();
            ObjectRegistry.Remove(ob.Key);

            // fire event signalling an object has been destroyed
            var handler = ResourceDestroyed;
            if (handler != null)
                handler(this, args);
        }

        /// <summary>
        /// function to enforce XMLResourceExistsAction policy.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="object"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected T DoExistingObjectAction(string objectName, T @object, XMLResourceExistsAction action)
        {
            EventHandler<ResourceEventArgs> handler;

            if (IsDefined(objectName))
            {
                switch (action)
                {
                case XMLResourceExistsAction.XREA_RETURN:
                        System.GetSingleton().Logger
                            .LogEvent("---- Returning existing instance of " + _resourceType + " named '" + objectName + "'.");
                        // delete any new object we already had created
                        @object.Dispose();
                        // return existing instance of object.
                        return ObjectRegistry[objectName];

                case XMLResourceExistsAction.XREA_REPLACE:
                        System.GetSingleton().Logger
                            .LogEvent("---- Replacing existing instance of " + _resourceType + " named '" + objectName + "' (DANGER!).");
                        Destroy(objectName);
                        handler = ResourceReplaced;
                        break;

                case XMLResourceExistsAction.XREA_THROW:
                    // TODO: CEGUI_DELETE_AO object;
                        throw new AlreadyExistsException("an object of type '" + _resourceType + "' named '" +
                                                         objectName + "' already exists in the collection.");

                default:
                    // TODO: CEGUI_DELETE_AO object;
                        throw new InvalidRequestException("Invalid CEGUI::XMLResourceExistsAction was specified.");
                }
            }
            else
                handler = ResourceCreated;

            ObjectRegistry[objectName] = @object;
            DoPostObjectAdditionAction(@object);

            // fire event about this resource change
            if (handler != null)
                handler(this, new ResourceEventArgs(_resourceType, objectName));

            return @object;
        }

        /// <summary>
        /// Function called each time a new object is added to the collection.
        /// </summary>
        /// <param name="object"></param>
        protected virtual void DoPostObjectAdditionAction(T @object)
        {
            // do nothing by default.
        }

        /// <summary>
        /// String holding the text for the resource type managed.
        /// </summary>
        private readonly string _resourceType;

        /// <summary>
        /// the collection of objects
        /// </summary>
        protected Dictionary<string, T>  ObjectRegistry = new Dictionary<string, T>();
    }
}