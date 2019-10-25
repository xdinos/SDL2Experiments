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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class providing a shared library of Font objects to the system.
    /// 
    /// The FontManager is used to create, access, and destroy Font objects.  The
    /// idea is that the FontManager will function as a central repository for Font
    /// objects used within the GUI system, and that those Font objects can be
    /// accessed, via a unique name, by any interested party within the system.
    /// </summary>
    public class FontManager : NamedXMLResourceManager<Font, Font_xmlHandler>
    {
        #region Implementation of Singleton

        private static readonly Lazy<FontManager> Instance = new Lazy<FontManager>(() => new FontManager());
        public static FontManager GetSingleton()
        {
            return Instance.Value;
        }
 
        #endregion

        public IEnumerable<Font> Iterator
        {
            get { return ObjectRegistry.Values; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private FontManager()
            : base("Font")
        {
            // TODO: ...
            //char addr_buff[32];
            //sprintf(addr_buff, "(%p)", static_cast<void*>(this));
            //Logger::getSingleton().logEvent("CEGUI::FontManager singleton created. " + String(addr_buff));
        }

        // TODO: Destructor.
        //~FontManager()
        //{
        //    Logger::getSingleton().logEvent("---- Begining cleanup of Font system ----");

        //    destroyAll();

        //    char addr_buff[32];
        //    sprintf(addr_buff, "(%p)", static_cast<void*>(this));
        //    Logger::getSingleton().logEvent("CEGUI::FontManager singleton destroyed. " + String(addr_buff));
        //}
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                System.GetSingleton().Logger
                      .LogEvent("---- Begining cleanup of Font system ----");

                DestroyAll();

                //    char addr_buff[32];
                //    sprintf(addr_buff, "(%p)", static_cast<void*>(this));
                System.GetSingleton().Logger
                    .LogEvent("CEGUI::FontManager singleton destroyed. " + GetHashCode().ToString("X8"));
            }
        }

        /*!
        \brief
            Creates a FreeType type font.

        \param font_name
            The name that the font will use within the CEGUI system.

        \param point_size
            Specifies the point size that the font is to be rendered at.

        \param anti_aliased
            Specifies whether the font should be rendered using anti aliasing.

        \param font_filename
            The filename of an font file that will be used as the source for
            glyph images for this font.

        \param resource_group
            The resource group identifier to use when loading the font file
            specified by \a font_filename.

        \param auto_scaled
            Specifies whether the font imagery should be automatically scaled to
            maintain the same physical size (which is calculated by using the
            native resolution setting).

        \param native_horz_res
            The horizontal native resolution value.  This is only significant when
            auto scaling is enabled.

        \param native_vert_res
            The vertical native resolution value.  This is only significant when
            auto scaling is enabled.

        \param action
            One of the XMLResourceExistsAction enumerated values indicating what
            action should be taken when a Font with the specified name
            already exists.

        \return
            Reference to the newly create Font object.
        */
        public Font CreateFreeTypeFont(string font_name,
                                       float point_size,
                                       bool anti_aliased,
                                       string font_filename,
                                       string resource_group,
                                       AutoScaledMode auto_scaled,
                                       Sizef native_res,
                                       XMLResourceExistsAction action = XMLResourceExistsAction.XREA_RETURN)
        {
#if CEGUI_HAS_FREETYPE
	        Logger.LogInsane("Attempting to create FreeType font '" + font_name + "' using font file '" +
	                         font_filename + "'.");

			
			// create new object ahead of time
			 var @object = new FreeTypeFont(font_name, point_size, anti_aliased, font_filename, resource_group, auto_scaled, native_res, 0f);

			// return appropriate object instance (deleting any not required)
			return DoExistingObjectAction(font_name, @object, action);

#else
			throw new InvalidRequestException("CEGUI was compiled without freetype support.");
#endif
        }

        public Font CreateFreeTypeFont(string font_name,
                                       float point_size,
                                       bool anti_aliased,
                                       string font_filename)
        {
	        return CreateFreeTypeFont(font_name, point_size, anti_aliased, font_filename,
	                                  "",
	                                  AutoScaledMode.Disabled,
	                                  new Sizef(640f, 480f),
	                                  XMLResourceExistsAction.XREA_RETURN);
        }

        /*!
        \brief
            Creates a Pixmap type font.

        \param font_name
            The name that the font will use within the CEGUI system.

        \param imageset_filename
            The filename of an imageset to load that will be used as the source for
            glyph images for this font.  If \a resource_group is the special value
            of "*", this parameter may instead refer to the name of an already
            loaded Imagset.

        \param resource_group
            The resource group identifier to use when loading the imageset file
            specified by \a imageset_filename.  If this group is set to the special
            value of "*", then \a imageset_filename instead will refer to the name
            of an existing Imageset.

        \param auto_scaled
            Specifies whether the font imagery should be automatically scaled to
            maintain the same physical size (which is calculated by using the
            native resolution setting).

        \param native_horz_res
            The horizontal native resolution value.  This is only significant when
            auto scaling is enabled.

        \param native_vert_res
            The vertical native resolution value.  This is only significant when
            auto scaling is enabled.

        \param action
            One of the XMLResourceExistsAction enumerated values indicating what
            action should be taken when a Font with the specified name
            already exists.

        \return
            Reference to the newly create Font object.
        */
        public Font CreatePixmapFont(string font_name,
                               string imageset_filename,
                               string resource_group,
                               AutoScaledMode auto_scaled,
                               Sizef native_res,
                               XMLResourceExistsAction action)
        {
            Logger.LogInsane("Attempting to create Pixmap font '" + font_name + "' using imageset file '" + imageset_filename + "'.");

            // create new object ahead of time
            Font @object = new PixmapFont(font_name, imageset_filename, resource_group, auto_scaled, native_res);
            
            // return appropriate object instance (deleting any not required)
            return DoExistingObjectAction(font_name, @object, action);
        }

        public Font CreatePixmapFont(string font_name, string imageset_filename)
        {
            return CreatePixmapFont(font_name, imageset_filename,
                                    "",
                                    AutoScaledMode.Disabled,
                                    new Sizef(640f, 480f),
                                    XMLResourceExistsAction.XREA_RETURN);
        }

        /// <summary>
        /// Notify the FontManager that display size may have changed.
        /// </summary>
        /// <param name="size">
        /// Size object describing the display resolution
        /// </param>
        public void NotifyDisplaySizeChanged(Sizef size)
        {
            // notify all attached Font objects of the change in resolution
            foreach (var value in ObjectRegistry.Values)
                value.NotifyDisplaySizeChanged(size);
        }

        /*!
        \brief
            Writes a full XML font file for the specified Font to the given
            OutStream.

        \param name
            String holding the name of the Font to be written to the stream.

        \param out_stream
            OutStream (std::ostream based) object where data is to be sent.
        */
        public void WriteFontToStream(string name, StreamWriter outStream)
        {
            var xml = new XMLSerializer(outStream);
            // output font data
            Get(name).WriteXMLToStream(xml);
        }

        // TODO: ...
        ////! ConstBaseIterator type definition.
        //typedef ConstMapIterator<ObjectRegistry> FontIterator;

        ///*!
        //\brief
        //    Return a FontManager::FontIterator object to iterate over the available
        //    Font objects.
        //*/
        //FontIterator getIterator() const;

        //// ensure we see overloads from template base class
        //using NamedXMLResourceManager<Font, Font_xmlHandler>::createFromContainer;
        //using NamedXMLResourceManager<Font, Font_xmlHandler>::createFromFile;
        //using NamedXMLResourceManager<Font, Font_xmlHandler>::createFromString;
    }
}