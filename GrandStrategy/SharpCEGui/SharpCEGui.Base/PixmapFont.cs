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
    /// Implementation of the Font class interface using static Imageset's.
    /// 
    /// To create such a font you must create a Imageset with all the glyphs,
    /// and then define individual glyphs via defineMapping.
    /// </summary>
    public sealed class PixmapFont : Font
    {
        /*!
        \brief
            Constructor for Pixmap type fonts.

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
        */

        public PixmapFont(string font_name, string imageset_filename,
                          string resource_group,
                          AutoScaledMode auto_scaled,
                          Sizef native_res)
            : base(font_name, Font_xmlHandler.FontTypePixmap, imageset_filename, resource_group, auto_scaled, native_res
                )
        {
            d_origHorzScaling = 1.0f;
            d_imagesetOwner = false;
            
            AddPixmapFontProperties();

            Reinit();
            UpdateFont();
        }

        public PixmapFont(string font_name, string imageset_filename)
            : this(font_name, imageset_filename, "", AutoScaledMode.Disabled, new Sizef(640f, 480f))
        {
        }

        //! Destructor.
        // TODO: ~PixmapFont();

        public void DefineMapping(char codepoint, string image_name, float horz_advance)
        {
            var image = ImageManager.GetSingleton().Get(d_imageNamePrefix + '/' + image_name);

            var adv = (horz_advance == -1.0f)
                          ? (float) (int) (image.GetRenderedSize().Width + image.GetRenderedOffset().X)
                          : horz_advance;

            if (d_autoScaled != AutoScaledMode.Disabled)
                adv *= d_origHorzScaling;

            if (codepoint > d_maxCodepoint)
                d_maxCodepoint = codepoint;

            // create a new FontGlyph with given character code
            var glyph = new FontGlyph(adv, image, true);

            if (image.GetRenderedOffset().Y < -d_ascender)
                d_ascender = -image.GetRenderedOffset().Y;
            if (image.GetRenderedSize().Height + image.GetRenderedOffset().Y > -d_descender)
                d_descender = -(image.GetRenderedSize().Height + image.GetRenderedOffset().Y);

            d_height = d_ascender - d_descender;

            // add glyph to the map
            d_cp_map[codepoint] = glyph;
        }

        public void DefineMapping(string value)
        {
            throw new NotImplementedException();
        }

        //! Return the image name prefix that the font is using for it's glyphs.
        public string GetImageNamePrefix()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Set image name prefix font should use for it's glyphs.

            This will potentially cause existing images to be destroyed (if they
            were created specifically by, and for, this Font).  Images using the new
            name prefix must already exist within the system.

        \param name_prefix
            Name prefix used by an existing set of images to be used as the glyph
            source for this Font.
        */

        public void SetImageNamePrefix(string name_prefix)
        {
            throw new NotImplementedException();
        }

        //! Initialize the imageset.
        private void Reinit()
        {
            if (d_imagesetOwner)
                ImageManager.GetSingleton().DestroyImageCollection(d_imageNamePrefix);

            if (d_resourceGroup == "*")
            {
                d_imageNamePrefix = d_filename;
                d_imagesetOwner = false;
            }
            else
            {
                ImageManager.GetSingleton().LoadImageset(d_filename, d_resourceGroup);
                // here we assume the imageset name will match the font name
                d_imageNamePrefix = d_name; 
                d_imagesetOwner = true;
            }
        }

        /// <summary>
        /// No auto scaling takes place
        /// </summary>
        private void AddPixmapFontProperties()
        {
            const string propertyOrigin = "PixmapFont";

            AddProperty(new TplWindowProperty<PixmapFont, string>(
                            "ImageNamePrefix",
                            "This is the name prefix used by the images that contain the glyph imagery for this font.",
                            (x, v) => x.SetImageNamePrefix(v), x => x.GetImageNamePrefix(), propertyOrigin, ""));

            AddProperty(new TplWindowProperty<PixmapFont, string>(
                            "Mapping",
                            "This is the glyph-to-image mapping font property. It cannot be read. Format is: codepoint,advance,imagename",
                            (x, v) => x.DefineMapping(v), null, propertyOrigin, ""));
        }

        // override of functions in Font base class.
        protected override void UpdateFont()
        {
            var factor = (d_autoScaled != AutoScaledMode.Disabled ? d_horzScaling : 1.0f) / d_origHorzScaling;

            d_ascender = 0;
            d_descender = 0;
            d_height = 0;
            d_maxCodepoint = 0;

            foreach (var i in d_cp_map)
            {
                if (i.Key > d_maxCodepoint)
                    d_maxCodepoint = i.Key;

                i.Value.SetAdvance(i.Value.GetAdvance() * factor);

                var img = i.Value.GetImage();

                var bi = img as BitmapImage;
                if (bi!=null)
                {
                    bi.SetAutoScaled(d_autoScaled);
                    bi.SetNativeResolution(d_nativeResolution);
                }

                if (img.GetRenderedOffset().Y < d_ascender)
                    d_ascender = img.GetRenderedOffset().Y;
                if (img.GetRenderedSize().Height + img.GetRenderedOffset().Y > d_descender)
                    d_descender = img.GetRenderedSize().Height + img.GetRenderedOffset().Y;
            }

            d_ascender = -d_ascender;
            d_descender = -d_descender;
            d_height = d_ascender - d_descender;

            d_origHorzScaling = d_autoScaled != AutoScaledMode.Disabled ? d_horzScaling : 1.0f;
        }

        protected override void WriteXMLToStreamImpl(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        //! The Image name prefix used for the glyphs
        private string d_imageNamePrefix;

        //! Current X scaling for glyph images
        private float d_origHorzScaling;

        //! true if we own the imageset
        private bool d_imagesetOwner;
    }
}