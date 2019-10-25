using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace SharpCEGui.Base
{
    internal class FntGlyph : FontGlyph
    {
        public readonly float OriginalAdvance;

        internal FntGlyph(float advance = 0.0f, Image image = null, bool valid = false)
            :base( advance, image,valid)
        {
            OriginalAdvance = advance;
        }

        
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class FntFont : Font
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filename"></param>
        /// <param name="resourceGroup"></param>
        /// <param name="autoScaled"></param>
        /// <param name="nativeRes"></param>
        public FntFont(string name, string filename, string resourceGroup, AutoScaledMode autoScaled, Sizef nativeRes)
            : base(name, Font_xmlHandler.FontTypeFnt, filename, resourceGroup, autoScaled, nativeRes)
        {
            d_origHorzScaling = 1.0f;

            Reinit();
            UpdateFont();
        }

        public override void NotifyDisplaySizeChanged(Sizef size)
        {
            Image.ComputeScalingFactors(d_autoScaled, size, d_nativeResolution, out d_horzScaling, out d_vertScaling);

            if (d_autoScaled != AutoScaledMode.Disabled)
            {
                UpdateFont(size);

                OnRenderSizeChanged(new FontEventArgs(this));
            }
        }

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

                var fntGlyph = (FntGlyph) i.Value;
                fntGlyph.SetAdvance(fntGlyph.OriginalAdvance * d_horzScaling);

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

        private void UpdateFont(Sizef displaySize)
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

                var fntGlyph = (FntGlyph)i.Value;
                fntGlyph.SetAdvance(fntGlyph.OriginalAdvance * d_horzScaling);

                var img = i.Value.GetImage();

                var bi = img as BitmapImage;
                if (bi != null)
                    bi.NotifyDisplaySizeChanged(displaySize);

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
            throw new global::System.NotImplementedException();
        }

        private void Reinit()
        {
            var fontData = new RawDataContainer();

            System.GetSingleton().GetResourceProvider()
                  .LoadRawDataContainer(d_filename, fontData, String.IsNullOrEmpty(d_resourceGroup)
                                                                  ? GetDefaultResourceGroup()
                                                                  : d_resourceGroup);
            LoadFntFile(fontData.Stream());
        }

        private void LoadFntFile(Stream stream)
        {
            var fnt = XElement.Load(stream);

            var info = fnt.Element("info");
            var common = fnt.Element("common");
            var pages = fnt.Element("pages");

            var face = info.Attribute("face").Value;
            d_ascender = XmlConvert.ToSingle(common.Attribute("base").Value);
            d_height = XmlConvert.ToSingle(common.Attribute("lineHeight").Value);
            d_descender = d_ascender - d_height;

            foreach (var page in pages.Elements("page"))
            {
                var pageId = page.Attribute("id").Value;
                var pageFile = page.Attribute("file").Value;
                System.GetSingleton().GetRenderer()
                      .CreateTexture(d_name + "_glyph_images_page_id_" + pageId, pageFile,
                                     String.IsNullOrEmpty(d_resourceGroup)
                                         ? GetDefaultResourceGroup()
                                         : d_resourceGroup);
            }

            var chars = fnt.Element("chars");
            foreach (var @char in chars.Elements("char"))
            {
                var pageId = d_name + "_glyph_images_page_id_" + @char.Attribute("page").Value;
                var charId = (char)XmlConvert.ToUInt32(@char.Attribute("id").Value);
                var area = new Rectf(new Lunatics.Mathematics.Vector2(XmlConvert.ToSingle(@char.Attribute("x").Value),
                                                  XmlConvert.ToSingle(@char.Attribute("y").Value)),
                                     new Sizef(XmlConvert.ToSingle(@char.Attribute("width").Value),
                                               XmlConvert.ToSingle(@char.Attribute("height").Value)));
                var offset = new Lunatics.Mathematics.Vector2(XmlConvert.ToSingle(@char.Attribute("xoffset").Value),
                                          XmlConvert.ToSingle(@char.Attribute("yoffset").Value) - d_ascender);
                d_cp_map[charId] = new FntGlyph(XmlConvert.ToSingle(@char.Attribute("xadvance").Value),
                                                 new BitmapImage(charId.ToString(global::System.Globalization.CultureInfo.InvariantCulture),
                                                                System.GetSingleton().GetRenderer().GetTexture(pageId),
                                                                area, offset, 
                                                                d_autoScaled,
                                                                d_nativeResolution),
                                                 true);
                if (d_maxCodepoint < charId)
                    d_maxCodepoint = charId;
            }

            //foreach (var kerning in chars.Elements("kerning"))
            //{
            //    var first = XmlConvert.ToChar(kerning.Attribute("first").Value);
            //    var second = XmlConvert.ToChar(kerning.Attribute("second").Value);
            //    var amount = XmlConvert.ToSingle(kerning.Attribute("amount").Value);
            //    if (!_kerningPairs.ContainsKey(first))
            //        _kerningPairs.Add(first, new Dictionary<char, float>());
            //    _kerningPairs[first].Add(second, amount);
            //}
        }

        #region Fields

        //! Current X scaling for glyph images
        private float d_origHorzScaling;

        #endregion
    }
}