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

namespace SharpCEGui.Base
{
    /// <summary>
    /// A class that manages the creation of, access to, and destruction of GUI
    /// Scheme objects
    /// </summary>
    public class SchemeManager : NamedXMLResourceManager<Scheme, Scheme_xmlHandler>
    {
        private static readonly Lazy<SchemeManager> Intance = new Lazy<SchemeManager>(() => new SchemeManager());
        public static SchemeManager GetSingleton()
        {
            return Intance.Value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private SchemeManager()
            : base("Scheme")
        {
            d_autoLoadResources = true;
            
            System.GetSingleton().Logger
                .LogEvent("SharpCEGui.SchemeManager singleton created. " + GetHashCode().ToString("X8"));
        }

        //! Destructor.
        // TODO: ~SchemeManager()
        //{
        //    Logger::getSingleton().logEvent("---- Begining cleanup of GUI Scheme system ----");

        //    destroyAll();

        //    char addr_buff[32];
        //    sprintf(addr_buff, "(%p)", static_cast<void*>(this));
        //    Logger::getSingleton().logEvent("CEGUI::SchemeManager singleton destroyed. " + String(addr_buff));
        //}

        // TODO: ...
        ////! Definition of SchemeIterator type.
        //typedef ConstMapIterator<ObjectRegistry> SchemeIterator;

        ///*!
        //\brief
        //    Return a SchemeManager::SchemeIterator object to iterate over the
        //    available schemes.
        //*/
        //SchemeIterator  getIterator() const;
    
        /*!
        \brief
            If this is enabled, Schemas will immediately load their resources after they are created
        
        It's sometimes useful to turn this off when you want to load things more selectively.
        This is enabled by default.
    
        \param enabled
            If true, you will have to load resources from the Scheme yourself!
    
        \note
            Calling Scheme::loadResources after you create the Scheme is equivalent to this being enabled
            and creating the scheme.
        */
        public void SetAutoLoadResources(bool enabled)
        {
            d_autoLoadResources = enabled;
        }
    
        /// <summary>
        /// Checks whether resources are loaded immediately after schemes are created
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="SchemeManager.SetAutoLoadResources"/>
        public bool GetAutoLoadResources()
        {
            return d_autoLoadResources;
        }
        
        // override from base
        protected override void DoPostObjectAdditionAction(Scheme @object)
        {
            if (d_autoLoadResources)
            {
                @object.LoadResources();
            }
        }
    
        /// <summary>
        /// If true, Scheme::loadResources is called after "create" is called for it
        /// </summary>
        protected bool d_autoLoadResources;
    }
}