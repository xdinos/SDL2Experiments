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

using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Abstract class that defines the required interface for all resource provider sub-classes.
    /// 
    /// A ResourceProvider is used to load both XML and binary data from an external source.  
    /// This could be from a filesystem or the resource manager of a specific renderer.
    /// </summary>
    public abstract class ResourceProvider
    {
        /// <summary>
        /// Constructor for the ResourceProvider class
        /// </summary>
	    protected ResourceProvider() { }

        /*!
        \brief
            Destructor for the ResourceProvider class
        */
	    // TODO: virtual ~ResourceProvider(void) { }

        /*************************************************************************
            Accessor functions
        *************************************************************************/

        //    /*!
        //    \brief
        //        Load XML data using InputSource objects.
        //
        //    \param filename
        //        String containing a filename of the resource to be loaded.
        //
        //	\param output
        //		Reference to a InputSourceContainer object to load the data into.
        //   */
        //    virtual void loadInputSourceContainer(const String& filename, InputSourceContainer& output) = 0;

        /// <summary>
        /// Load raw binary data.
        /// </summary>
        /// <param name="filename">
        /// String containing a filename of the resource to be loaded.
        /// </param>
        /// <param name="output">
        /// Reference to a RawDataContainer object to load the data into.
        /// </param>
        /// <param name="resourceGroup">
        /// Optional String that may be used by implementations to identify the group from
        /// which the resource should be loaded.
        /// </param>
        public abstract void LoadRawDataContainer(string filename, RawDataContainer output, string resourceGroup);

        /// <summary>
        /// Unload raw binary data. This gives the resource provider a change to unload the data
        /// in its own way before the data container object is destroyed.  If it does nothing,
        /// then the object will release its memory.
        /// </summary>
        /// <param name="rawDataContainer">
        /// Reference to a RawDataContainer object that is about to be destroyed.
        /// </param>
        public virtual void UnloadRawDataContainer(RawDataContainer rawDataContainer)
        {
        }

        /// <summary>
        /// Return the current default resource group identifier.
        /// </summary>
        /// <returns>
        /// String object containing the currently set default resource group identifier.
        /// </returns>
        public string GetDefaultResourceGroup()
        {
            return d_defaultResourceGroup;
        }
    
        /// <summary>
        /// Set the default resource group identifier.
        /// </summary>
        /// <param name="resourceGroup">
        /// String object containing the default resource group identifier to be used.
        /// </param>
        public void SetDefaultResourceGroup(string resourceGroup)
        {
            d_defaultResourceGroup = resourceGroup;
        }

	    public abstract bool FileExists(string filename, string resourceGroup);

		/// <summary>
		/// enumerate the files in \a resource_group that match \a file_pattern and
		/// append thier names to \a out_vec
		/// </summary>
		/// <param name="filePattern"></param>
		/// <param name="resourceGroup"></param>
		/// <returns></returns>
		public abstract IEnumerable<string> GetResourceGroupFileNames(string filePattern, string resourceGroup);

        /// <summary>
        /// Default resource group identifier.
        /// </summary>
        protected string d_defaultResourceGroup;
    }
}