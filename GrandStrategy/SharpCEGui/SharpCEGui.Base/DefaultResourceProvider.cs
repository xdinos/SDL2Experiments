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
using System.IO;
using System.Linq;

namespace SharpCEGui.Base
{
    public class DefaultResourceProvider : ResourceProvider
    {
        /*************************************************************************
		    Construction and Destruction
	    *************************************************************************/

        public DefaultResourceProvider()
        {
        }

        // TODO: ~DefaultResourceProvider(void) {}

        /// <summary>
        /// Set the directory associated with a given resource group identifier.
        /// </summary>
        /// <param name="resourceGroup">
        /// The resource group identifier whose directory is to be set.
        /// </param>
        /// <param name="directory">
        /// The directory to be associated with resource group identifier
        /// \a resourceGroup
        /// </param>
        public void SetResourceGroupDirectory(string resourceGroup, string directory)
        {
            if (directory.Length == 0)
                return;

            _resourceGroups[resourceGroup] = directory.Replace("\\", "/");

            //#if defined(_WIN32) || defined(__WIN32__)
            //    // while we rarely use the unportable '\', the user may have
            //    const String separators("\\/");
            //#else
            //    const String separators("/");
            //#endif

            //    if (String::npos == separators.find(directory[directory.length() - 1]))
            //        d_resourceGroups[resourceGroup] = directory + '/';
            //    else
            //        d_resourceGroups[resourceGroup] = directory;
        }

        /// <summary>
        /// Return the directory associated with the specified resource group
        /// identifier.
        /// </summary>
        /// <param name="resourceGroup">
        /// The resource group identifier for which the associated directory is to
        /// be returned.
        /// </param>
        /// <returns>
        /// String object describing the directory currently associated with resource
        /// group identifier \a resourceGroup.
        /// </returns>
        /// <remarks>
        /// This member is not defined as being const because it may cause
        /// creation of an 'empty' directory specification for the resourceGroup
        /// if the resourceGroup has not previously been accessed.
        /// </remarks>
        public string GetResourceGroupDirectory(string resourceGroup)
        {
            return _resourceGroups[resourceGroup];
        }

        /// <summary>
        /// clears any currently set directory for the specified resource group
        /// identifier.
        /// </summary>
        /// <param name="resourceGroup">
        /// The resource group identifier for which the associated directory is to
        /// be cleared.
        /// </param>
        public void ClearResourceGroupDirectory(string resourceGroup)
        {
            _resourceGroups.Remove(resourceGroup);
        }

        public override void LoadRawDataContainer(string filename, RawDataContainer output, string resourceGroup)
        {
            if (String.IsNullOrEmpty(filename))
                throw new InvalidRequestException("Filename supplied for data loading must be valid.");
            
            var finalFilename = GetFinalFilename(filename, resourceGroup);

            var buffer = File.ReadAllBytes(finalFilename);
            output.SetData(new MemoryStream(buffer, 0, buffer.Length, false, true));
        }

        public override void UnloadRawDataContainer(RawDataContainer data)
        {
            data.Release();
        }

	    public override bool FileExists(string filename, string resourceGroup)
	    {
		    return File.Exists(GetFinalFilename(filename, resourceGroup));
	    }
        
        public override IEnumerable<string> GetResourceGroupFileNames(string filePattern, string resourceGroup)
        {
            var path = _resourceGroups.ContainsKey(resourceGroup)
                           ? _resourceGroups[resourceGroup]
                           : "./";

	        return Directory.GetFiles(path, filePattern).Select(x => x.Replace('\\', '/'));
        }

        /// <summary>
        /// Return the final path and filename, taking into account the given
        /// resource group identifier that should be used when attempting to
        /// load the data.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="resourceGroup"></param>
        /// <returns></returns>
        protected string GetFinalFilename(string filename, string resourceGroup)
        {
            var finalFilename = string.Empty;

            // look up resource group directory
            // if there was an entry for this group, use it's directory as the
            // first part of the filename
            if (_resourceGroups.ContainsKey(resourceGroup))
            {
                finalFilename = _resourceGroups[resourceGroup];
            }

            // append the filename part that we were passed
            finalFilename = Path.Combine(finalFilename, filename);

            // return result
            return finalFilename;
        }

        #region Fields

        private readonly Dictionary<string, string> _resourceGroups = new Dictionary<string, string>();

        #endregion
    }
}