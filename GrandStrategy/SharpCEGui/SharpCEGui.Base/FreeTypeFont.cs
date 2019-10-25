#if CEGUI_HAS_FREETYPE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SharpFont;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Implementation of the Font class interface using the FreeType library.
    /// 
    /// This implementation tries to provide maximal support for any kind of
    /// fonts supported by FreeType. It has been tested on outline font formats
    /// like TTF and PS as well as on bitmap font formats like PCF and FON.
    /// 
    /// Glyphs are rendered dynamically on demand, so a large font with lots
    /// of glyphs won't slow application startup time.
    /// </summary>
    public sealed class FreeTypeFont : Font
    {
        /// <summary>
        /// Constructor for FreeTypeFont based fonts.
        /// </summary>
        /// <param name="fontName">The name that the font will use within the CEGUI system.</param>
        /// <param name="pointSize">
        /// Specifies the point size that the font is to be rendered at.
        /// </param>
        /// <param name="antiAliased">
        /// Specifies whether the font should be rendered using anti aliasing.
        /// </param>
        /// <param name="fontFilename">
        /// The filename of an font file that will be used as the source for
        /// glyph images for this font.
        /// </param>
        /// <param name="resourceGroup">
        /// The resource group identifier to use when loading the font file
        /// specified by \a font_filename.
        /// </param>
        /// <param name="autoScaled">
        /// Specifies whether the font imagery should be automatically scaled to
        /// maintain the same physical size (which is calculated by using the
        /// native resolution setting).
        /// </param>
        /// <param name="nativeRes">
        /// The native resolution value. This is only significant when  auto scaling is enabled.
        /// </param>
        /// <param name="specificLineSpacing">
        /// If specified (non-zero), this will be the line spacing that we will
        /// report for this font, regardless of what is mentioned in the font file
        /// itself.
        /// </param>
        public FreeTypeFont(string fontName, float pointSize,
                            bool antiAliased, string fontFilename,
                            string resourceGroup,
                            AutoScaledMode autoScaled,
                            Sizef nativeRes,
                            float specificLineSpacing)
            : base(fontName, Font_xmlHandler.FontTypeFreeType, fontFilename,
                   resourceGroup, autoScaled, nativeRes)
        {
            d_specificLineSpacing = specificLineSpacing;
            d_ptSize = pointSize;
            d_antiAliased = antiAliased;
            d_fontFace = null;
            d_kerningMode = KerningMode.Default;

            if (ft_usage_count == 0)
                _library = new Library();

            AddFreeTypeFontProperties();
            UpdateFont();

            System.GetSingleton().Logger
                  .LogEvent(String.Format("Successfully loaded {0} glyphs", d_cp_map.Count), LoggingLevel.Informative);

            ft_usage_count++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontName"></param>
        /// <param name="pointSize"></param>
        /// <param name="antiAliased"></param>
        /// <param name="fontFilename"></param>
        public FreeTypeFont(string fontName, float pointSize, bool antiAliased, string fontFilename)
            : this(fontName, pointSize, antiAliased, fontFilename, "", AutoScaledMode.Disabled, new Sizef(640f, 480f), 0f)
        {
        }

#region Implementation of IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Free();
                --ft_usage_count;
                if (ft_usage_count == 0)
                    _library.Dispose();
            }
        }

#endregion

        /// <summary>
        /// return the point size of the freetype font.
        /// </summary>
        /// <returns></returns>
        public float GetPointSize()
        {
            return d_ptSize;
        }

        /// <summary>
        /// return whether the freetype font is rendered anti-aliased.
        /// </summary>
        /// <returns></returns>
        public bool IsAntiAliased()
        {
            return d_antiAliased;
        }

        /// <summary>
        /// return the point size of the freetype font.
        /// </summary>
        /// <param name="pointSize"></param>
        public void SetPointSize(float pointSize)
        {
            if (pointSize == d_ptSize)
                return;

            d_ptSize = pointSize;
            UpdateFont();

            OnRenderSizeChanged(new FontEventArgs(this));
        }

        /// <summary>
        /// return whether the freetype font is rendered anti-aliased.
        /// </summary>
        /// <param name="antiAlaised"></param>
        public void SetAntiAliased(bool antiAlaised)
        {
            if (antiAlaised == d_antiAliased)
                return;

            d_antiAliased = antiAlaised;
            UpdateFont();

            OnRenderSizeChanged(new FontEventArgs(this));
        }

        /// <summary>
        /// Gets the font face
        /// </summary>
        /// <returns></returns>
        internal Face GetFontFace()
        {
            return d_fontFace;
        }

        /// <summary>
        /// Gets the kerning mode
        /// </summary>
        /// <returns></returns>
        internal KerningMode GetKerningMode()
        {
            return d_kerningMode;
        }

        /// <summary>
        /// Copy the current glyph data into \a buffer, which has a width of
        /// \a buf_width pixels (not bytes).
        /// </summary>
        /// <param name="buffer">
        /// Memory buffer large enough to receive the imagery for the currently
        /// loaded glyph
        /// .</param>
        /// <param name="index"></param>
        /// <param name="bufWidth">
        /// Width of \a buffer in pixels (where each pixel is a argb_t).
        /// </param>
        private void DrawGlyphToBuffer(byte[] buffer, int index, int bufWidth)
        {
            var glyphBitmap = d_fontFace.Glyph.Bitmap;
            if (glyphBitmap.Buffer == IntPtr.Zero)
                return;

            var bufferData = glyphBitmap.BufferData;
            
            for (var i = 0; i < glyphBitmap.Rows; ++i)
            {
                var srcIdx = i*glyphBitmap.Pitch;
                var dstIdx = index + i*bufWidth;
                switch (glyphBitmap.PixelMode)
                {
                    case PixelMode.Gray:
                        {
                            for (var j = 0; j < glyphBitmap.Width; ++j)
                            {
                                // RGBA
                                buffer[dstIdx++] = 0xff;
                                buffer[dstIdx++] = 0xff;
                                buffer[dstIdx++] = 0xff;
                                buffer[dstIdx++] = bufferData[srcIdx++];
                            }
                        }
                        break;

                    case PixelMode.Mono:
                        for (var j = 0; j < glyphBitmap.Width; ++j)
                        {
                            if ((bufferData[srcIdx+(j / 8)] & (0x80 >> (j & 7))) == (0x80 >> (j & 7)))
                            {
                                buffer[dstIdx++] = 0xff;
                                buffer[dstIdx++] = 0xff;
                                buffer[dstIdx++] = 0xff;
                                buffer[dstIdx++] = 0xff;
                            }
                            else
                            {
                                buffer[dstIdx++] = 0x00;
                                buffer[dstIdx++] = 0x00;
                                buffer[dstIdx++] = 0x00;
                                buffer[dstIdx++] = 0x00;
                            }
                        }
                        break;

                    default:
                        throw new InvalidRequestException(
                            "The glyph could not be drawn because the pixel mode is unsupported.");
                }

                //buffer += buf_width;
            }
        }

        /// <summary>
        /// Return the required texture size required to store imagery for the glyphs from s to e
        /// </summary>
        /// <param name="s">
        /// The first glyph in set
        /// </param>
        /// <param name="e">
        /// The last glyph in set
        /// </param>
        /// <returns></returns>
        private int GetTextureSize(char s, char e)
        {
            var texsize = 32; // start with 32x32
            var maxTexsize = System.GetSingleton().GetRenderer().GetMaxTextureSize();
            var glyphCount = 0;

            // Compute approximatively the optimal texture size for font
            while (texsize < maxTexsize)
            {
                var x = InterGlyphPadSpace;
                var y = InterGlyphPadSpace;
                var yb = InterGlyphPadSpace;
                foreach (var c in d_cp_map.Where(cp=>cp.Key>=s && cp.Key<=e))
                {
                    // skip glyphs that are already rendered
                    if (c.Value.GetImage() != null)
                        continue;

                    // load glyph metrics (don't render)
                    try
                    {
                        var glyphIdx = d_fontFace.GetCharIndex(c.Key);
                        d_fontFace.LoadGlyph(glyphIdx, LoadFlags.Default | LoadFlags.ForceAutohint, LoadTarget.Normal);
                        //d_fontFace.LoadChar(c.Key, LoadFlags.Default | LoadFlags.ForceAutohint, LoadTarget.Normal);
                    }
                    catch
                    {
                        continue;
                    }

                    var glyphWidth = (int)(Math.Ceiling((float)d_fontFace.Glyph.Metrics.Width * FT_POS_COEF)) + InterGlyphPadSpace;
                    var glyphHeight = (int)(Math.Ceiling((float)d_fontFace.Glyph.Metrics.Height * FT_POS_COEF)) + InterGlyphPadSpace;

                    x += glyphWidth;
                    if (x > texsize)
                    {
                        x = InterGlyphPadSpace;
                        y = yb;
                    }
                    var yy = y + glyphHeight;
                    if (yy > texsize)
                        goto too_small;

                    if (yy > yb)
                        yb = yy;

                    ++glyphCount;
                }
                // Okay, the texture size is enough for holding our glyphs
                break;

            too_small:
                texsize *= 2;
            }

            return glyphCount!=0 ? texsize : 0;
        }

        /// <summary>
        /// Register all properties of this class.
        /// </summary>
        private void AddFreeTypeFontProperties()
        {
            AddProperty(new TplWindowProperty<FreeTypeFont, float>(
                            "PointSize", "This is the point size of the font.",
                            (x, v) => x.SetPointSize(v), x => x.GetPointSize(), "FreeTypeFont", 0f));

            AddProperty(new TplWindowProperty<FreeTypeFont, bool>(
                            "Antialiased",
                            "This is a flag indicating whenever to render antialiased font or not. Value is either true or false.",
                            (x, v) => x.SetAntiAliased(v), x => x.IsAntiAliased(), "FreeTypeFont", false));
        }

        /// <summary>
        /// Free all allocated font data.
        /// </summary>
        private void Free()
        {
            if (d_fontFace==null)
                return;
            
            d_cp_map.Clear();

            _glyphImages.Clear();

            foreach (var glyphTexture in _glyphTextures)
                System.GetSingleton().GetRenderer().DestroyTexture(glyphTexture);

            _glyphTextures.Clear();

            if (_fontDataPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_fontDataPtr);
                _fontDataPtr = IntPtr.Zero;
            }

            d_fontFace.Dispose();
            d_fontFace = null;

            System.GetSingleton().GetResourceProvider().UnloadRawDataContainer(d_fontData);
        }

        /// <summary>
        /// initialise FontGlyph for given codepoint.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="fontGlyph"></param>
        private void InitialiseFontGlyph(char cp, FontGlyph fontGlyph)
        {
            // load-up required glyph metrics (don't render)
            try
            {
                var glyphIdx = d_fontFace.GetCharIndex(cp);
                d_fontFace.LoadGlyph(glyphIdx, LoadFlags.Default | LoadFlags.ForceAutohint, LoadTarget.Normal);


                var adv = (float)d_fontFace.Glyph.Metrics.HorizontalAdvance * FT_POS_COEF;

                fontGlyph.SetAdvance(adv);
                fontGlyph.SetValid(true);
            }
            catch
            {

            }
        }

        private void InitialiseGlyphMap()
        {
            uint gindex;
            uint codepoint = d_fontFace.GetFirstChar(out gindex);
            uint maxCodepoint = codepoint;

            while (gindex!=0)
            {
                if (maxCodepoint < codepoint)
                    maxCodepoint = codepoint;

                d_cp_map[(char)codepoint] = new FontGlyph();

                codepoint = d_fontFace.GetNextChar(codepoint, out gindex);
            }

            SetMaxCodepoint((char)maxCodepoint);
        }

        protected override FontGlyph FindFontGlyph(char codepoint)
        {
            FontGlyph fontGlyph;
            if (!d_cp_map.TryGetValue(codepoint, out fontGlyph))
                return null;

            if (!fontGlyph.IsValid())
                InitialiseFontGlyph(codepoint, fontGlyph);

            return fontGlyph;
        }

        protected override void Rasterise(char startCodepoint, char endCodepoint)
        {
            //CodepointMap::iterator s = d_cp_map.lower_bound(start_codepoint);
            //if (s == d_cp_map.end())
            //    return;
            var first = d_cp_map.OrderBy(x => x.Key).First().Key;
            var last = d_cp_map.OrderByDescending(x => x.Key).First().Key;
            var s = d_cp_map.OrderBy(x => x.Key).First(x=>x.Key>=startCodepoint).Key;

            //CodepointMap::iterator orig_s = s;
            var orig_s = startCodepoint;
            //CodepointMap::iterator e = d_cp_map.upper_bound(end_codepoint);
            var e = d_cp_map.Last(x => x.Key <= endCodepoint).Key;
            while (true)
            {
                // Create a new Imageset for glyphs
                var texsize = GetTextureSize(s, e);
                // If all glyphs were already rendered, do nothing
                if (texsize == 0)
                    break;

                var textureName =d_name + "_auto_glyph_images_" + PropertyHelper.ToString(s);

                var texture = System.GetSingleton().GetRenderer()
                                    .CreateTexture(textureName, new Sizef(texsize, texsize));
                _glyphTextures.Add(texture);

                // Create a memory buffer where we will render our glyphs
                var memBuffer = new byte[sizeof (int)*texsize*texsize];

                // Go ahead, line by line, top-left to bottom-right
                var x = InterGlyphPadSpace;
                var y = InterGlyphPadSpace;
                var yb = InterGlyphPadSpace;

                // Set to true when we finish rendering all glyphs we were asked to
                var finished = false;

                // Set to false when we reach d_cp_map.end() and we start going backward
                var forward = true;
                
                // To conserve texture space we will render more glyphs than asked,
                // but never less than asked. First we render all glyphs from s to e
                // and after that we render glyphs until we reach d_cp_map.end(),
                // and if there's still free texture space we will go backward
                // from s until we hit d_cp_map.begin().
                while (s <= last)
                {
                    // Check if we finished rendering all the required glyphs
                    finished |= (s == e);

                    // Check if glyph already rendered
                    if (d_cp_map.ContainsKey(s))
                    {

                        if (d_cp_map[s].GetImage() == null)
                        {
                            // Render the glyph
                            try
                            {
                                var glyphIdx = d_fontFace.GetCharIndex(s);
                                d_fontFace.LoadGlyph(glyphIdx, LoadFlags.Render | LoadFlags.ForceAutohint,
                                                    d_antiAliased ? LoadTarget.Normal : LoadTarget.Mono);

                                var glyphWidth = d_fontFace.Glyph.Bitmap.Width + InterGlyphPadSpace;
                                var glyphHeight = d_fontFace.Glyph.Bitmap.Rows + InterGlyphPadSpace;

                                // Check if glyph right margin does not exceed texture size
                                var x_next = x + glyphWidth;
                                if (x_next > texsize)
                                {
                                    x = InterGlyphPadSpace;
                                    x_next = x + glyphWidth;
                                    y = yb;
                                }

                                // Check if glyph bottom margine does not exceed texture size
                                var y_bot = y + glyphHeight;
                                if (y_bot > texsize)
                                    break;

                                // Copy rendered glyph to memory buffer in RGBA format
                                DrawGlyphToBuffer(memBuffer, 4*((y*texsize) + x), 4*texsize);

                                // Create a new image in the imageset
                                var area = new Rectf(x, y,
                                                     (x + glyphWidth - InterGlyphPadSpace),
                                                     (y + glyphHeight - InterGlyphPadSpace));

                                var offset = new Lunatics.Mathematics.Vector2(
                                        (float) d_fontFace.Glyph.Metrics.HorizontalBearingX*FT_POS_COEF,
                                        -(float) d_fontFace.Glyph.Metrics.HorizontalBearingY*FT_POS_COEF);

                                var name = PropertyHelper.ToString(s);
                                var img = new BitmapImage(name, texture, area, offset, AutoScaledMode.Disabled,
                                                         d_nativeResolution);
                                _glyphImages.Add(img);
                                d_cp_map[s].SetImage(img);

                                // Advance to next position
                                x = x_next;
                                if (y_bot > yb)
                                {
                                    yb = y_bot;
                                }

                            }
                            catch (FreeTypeException)
                            {
                                System.GetSingleton().Logger
                                      .LogEvent(
                                          "Font::loadFreetypeGlyph - Failed to load glyph for codepoint: " + s +
                                          ".  Will use an empty image for this glyph!", LoggingLevel.Errors);

                                // Create a 'null' image for this glyph so we do not seg later
                                var area = Rectf.Zero;
                                var offset = Lunatics.Mathematics.Vector2.Zero;
                                var name = PropertyHelper.ToString(s);
                                var img = new BitmapImage(name, texture, area, offset, AutoScaledMode.Disabled,
                                                         d_nativeResolution);
                                _glyphImages.Add(img);
                                d_cp_map[s].SetImage(img);
                            }
                        }
                    }

                    // Go to next glyph, if we are going forward
                    if (forward)
                        if (++s > last)
                        {
                            finished = true;
                            forward = false;
                            s = orig_s;
                        }
                    // Go to previous glyph, if we are going backward
                    if (!forward)
                        if ((s == first) || (--s == first))
                            break;
                }

                // Copy our memory buffer into the texture and free it
                texture.LoadFromMemory(memBuffer, new Sizef(texsize, texsize), Texture.PixelFormat.RGBA);

                if (finished)
                    break;
            }
        }

        protected override void UpdateFont()
        {
            Free();

            System.GetSingleton().GetResourceProvider()
                  .LoadRawDataContainer(d_filename, d_fontData, String.IsNullOrEmpty(d_resourceGroup)
                                                                    ? GetDefaultResourceGroup()
                                                                    : d_resourceGroup);
            
            // create face using input font
            _fontDataPtr = Marshal.AllocHGlobal((int)d_fontData.GetSize());
            Marshal.Copy(d_fontData.GetBuffer(), 0, _fontDataPtr, (int) d_fontData.GetSize());
            d_fontFace = _library.NewMemoryFace(_fontDataPtr, (int)d_fontData.GetSize(), 0);
            
            // check that default Unicode character map is available
            if (d_fontFace.CharMap==null)
            {
                d_fontFace.Dispose();
                d_fontFace = null;
                throw new GenericException("The font '" + d_name +
                                           "' does not have a Unicode charmap, and cannot be used.");
            }

            var horzdpi = System.GetSingleton().GetRenderer().GetDisplayDotsPerInch().X;
            var vertdpi = System.GetSingleton().GetRenderer().GetDisplayDotsPerInch().Y;

            var hps = d_ptSize * 64;
            var vps = d_ptSize * 64;
            if (d_autoScaled != AutoScaledMode.Disabled)
            {
                hps *= d_horzScaling;
                vps *= d_vertScaling;
            }

            try
            {
                d_fontFace.SetCharSize((int) hps, (int) vps, (uint) horzdpi, (uint) vertdpi);
            }
            catch(FreeTypeException)
            {
                // For bitmap fonts we can render only at specific point sizes.
                // Try to find nearest point size and use it, if that is possible
                float ptSize72 = (d_ptSize * 72.0f) / vertdpi;
                float bestDelta = 99999;
                float bestSize = 0;
                foreach (BitmapSize t in d_fontFace.AvailableSizes)
                {
                    var size = (float)t.Size * FT_POS_COEF;
                    var delta = Math.Abs(size - ptSize72);
                    if (delta < bestDelta)
                    {
                        bestDelta = delta;
                        bestSize = size;
                    }
                }

                if ((bestSize <= 0))
                {
                    throw new GenericException("The font '" + d_name + "' cannot be rasterised at a size of " + d_ptSize + " points, and cannot be used.");
                }

                d_fontFace.SetCharSize(0, (int)(bestSize*64), 0, 0);
            }

            if ((d_fontFace.FaceFlags & FaceFlags.Scalable)==FaceFlags.Scalable)
            {
                //float x_scale = d_fontFace->size->metrics.x_scale * FT_POS_COEF * (1.0/65536.0);
                var y_scale = (float)d_fontFace.Size.Metrics.ScaleY*(FT_POS_COEF)*(1.0f/65536.0f);
                d_ascender = d_fontFace.Ascender*y_scale;
                d_descender = d_fontFace.Descender*y_scale;
                d_height = d_fontFace.Height*y_scale;
            }
            else
            {
                d_ascender = (float)d_fontFace.Size.Metrics.Ascender*FT_POS_COEF;
                d_descender = (float)d_fontFace.Size.Metrics.Descender * FT_POS_COEF;
                d_height = (float)d_fontFace.Size.Metrics.Height * FT_POS_COEF;
            }

            if (d_specificLineSpacing > 0.0f)
            {
                d_height = d_specificLineSpacing;
            }

            InitialiseGlyphMap();
        }

        protected override void WriteXMLToStreamImpl(XMLSerializer xml_stream)
        {
            throw new global::System.NotImplementedException();
        }

        //! If non-zero, the overridden line spacing that we're to report.
        float d_specificLineSpacing;
        
        //! Point size of font.
        float d_ptSize;
        
        //! True if the font should be rendered as anti-alaised by freeType.
        bool d_antiAliased;

        //! FreeType-specific font handle
        Face d_fontFace;
        
        //! Font file data
        RawDataContainer d_fontData = new RawDataContainer();

        private IntPtr _fontDataPtr;
        
        //! Type definition for TextureVector.
        //typedef std::vector<Texture*
        //    CEGUI_VECTOR_ALLOC(Texture*)> TextureVector;
        
        //! Textures that hold the glyph imagery for this font.
        private readonly List<Texture> _glyphTextures = new List<Texture>();
        
        //typedef std::vector<BasicImage*
        //    CEGUI_VECTOR_ALLOC(BasicImage*)> ImageVector;
        
        //! collection of images defined for this font.
        private readonly List<BitmapImage> _glyphImages = new List<BitmapImage>();

        private static Library _library;

        // Font objects usage count
        private static int ft_usage_count;

#region Constants

        // Pixels to put between glyphs
        private const int InterGlyphPadSpace = 2;

        // A multiplication coefficient to convert FT_Pos values into normal floats
        private const float FT_POS_COEF = (1.0f / 64.0f);

        /// <summary>
        /// Kerning mode
        /// </summary>
        private KerningMode d_kerningMode;

#endregion
    }
}
#endif