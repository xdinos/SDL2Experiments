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
using System.Diagnostics;

namespace SharpCEGui.Base
{
    /// <summary>
    ///  Class that encapsulates a typeface.
    /// 
    /// A Font object is created for each unique typeface required.
    /// The Font class provides methods for loading typefaces from various sources,
    /// and then for outputting text via the Renderer object.
    /// 
    /// This class is not specific to any font renderer, it just provides the
    /// basic interfaces needed to manage fonts.
    /// </summary>
    public abstract class Font : PropertySet, IDisposable
    {
        /// <summary>
        /// Colour value used whenever a colour is not specified.
        /// </summary>
        public static readonly Colour DefaultColour = new Colour(1f, 1f, 1f, 1f);

        /// <summary>
        /// Event fired when the font internal state has changed such that the
        /// rendered size of they glyphs is different.
        /// Handlers are passed a const FontEventArgs reference with
        /// FontEventArgs::font set to the Font whose rendered size has changed.
        /// </summary>
        public event EventHandler<FontEventArgs> RenderSizeChanged;

        // TODO: Destructor.
        //virtual ~Font();

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        #endregion

        /// <summary>
        /// Return the string holding the font name.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return d_name;
        }

        /// <summary>
        /// Return the type of the font.
        /// </summary>
        /// <returns></returns>
        public string GetTypeName()
        {
            return d_type;
        }

        /// <summary>
        /// Return the filename of the used font.
        /// </summary>
        /// <returns></returns>
        public string GetFileName()
        {
            return d_filename;
        }

        /// <summary>
        /// Return whether this Font can draw the specified code-point
        /// </summary>
        /// <param name="cp">
        /// utf32 code point that is the subject of the query.
        /// </param>
        /// <returns>
        /// true if the font contains a mapping for code point \a cp,
        /// false if it does not contain a mapping for \a cp.
        /// </returns>
        public bool IsCodepointAvailable(char cp)
        {
            return (d_cp_map.ContainsKey(cp));
        }

        /// <summary>
        /// Create render geometry for the text that should be rendered into a 
        /// specified area of the display.
        /// </summary>
        /// <param name="buffer">
        /// GeometryBuffer object where the geometry for the text be queued.
        /// </param>
        /// <param name="text">
        /// String object containing the text to be drawn.
        /// </param>
        /// <param name="position">
        /// Reference to a Vector2 object describing the location at which the text
        /// is to be drawn.
        /// </param>
        /// <param name="clipRect">
        /// Rect object describing the clipping area for the drawing.
        /// No drawing will occur outside this Rect.
        /// </param>
        /// <param name="clippingEnabled"></param>
        /// <param name="colours">
        /// ColourRect object describing the colours to be applied when drawing the
        /// text.  NB: The colours specified in here are applied to each glyph,
        /// rather than the text as a whole.
        /// </param>
        /// <param name="spaceExtra">
        /// Number of additional pixels of spacing to be added to space characters.
        /// </param>
        /// <param name="xScale">
        /// Scaling factor to be applied to each glyph's x axis, where 1.0f is
        /// considered to be 'normal'.
        /// </param>
        /// <param name="yScale">
        /// Scaling factor to be applied to each glyph's y axis, where 1.0f is
        /// considered to be 'normal'.
        /// </param>
        /// <returns>
        /// Returns a list of GeometryBuffers representing the render geometry of the text.
        /// </returns>
        public List<GeometryBuffer> CreateRenderGeometryForText(string text,
                                                                Lunatics.Mathematics.Vector2 position,
                                                                Rectf? clipRect,
                                                                bool clippingEnabled,
                                                                ColourRect colours,
                                                                float spaceExtra = 0.0f,
                                                                float xScale = 1.0f,
                                                                float yScale = 1.0f)
        {
            float nextGlyphPos;
            return CreateRenderGeometryForText(text, out nextGlyphPos, position, clipRect, clippingEnabled, colours, spaceExtra, xScale, yScale);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="nextGlyphPosX">
        /// The x-coordinate where subsequent text should be rendered to ensure correct
        /// positioning (which is not possible to determine accurately by using the
        /// extent measurement functions).</param>
        /// <param name="position"></param>
        /// <param name="clipRect"></param>
        /// <param name="clippingEnabled"></param>
        /// <param name="colours"></param>
        /// <param name="spaceExtra"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <returns>
        /// Returns a list of GeometryBuffers representing the render geometry of the text.
        /// </returns>
        public List<GeometryBuffer> CreateRenderGeometryForText(string text,
                                                                out float nextGlyphPosX,
                                                                Lunatics.Mathematics.Vector2 position,
                                                                Rectf? clipRect,
                                                                bool clippingEnabled,
                                                                ColourRect colours,
                                                                float spaceExtra = 0.0f,
                                                                float xScale = 1.0f,
                                                                float yScale = 1.0f)
        {
            var baseY = position.Y + GetBaseline(yScale);
            var glyphPos = position;
            var lastChar = (char)0;
            var geomBuffers = new List<GeometryBuffer>();
            GeometryBuffer textGeometryBuffer = null;

            var imgRenderSettings = new ImageRenderSettings(Rectf.Zero, clipRect, clippingEnabled, colours);

            foreach (var c in text)
            {
                var glyph = GetGlyphData(c);
                if (glyph != null)
                {
                    var img = glyph.GetImage();
                    glyphPos.X += GetKerningAmount(lastChar, c);
                    glyphPos.Y = baseY - (img.GetRenderedOffset().Y - img.GetRenderedOffset().Y * yScale);

                    imgRenderSettings.DestArea = new Rectf(glyphPos, glyph.GetSize(xScale, yScale));

                    if (textGeometryBuffer == null)
                    {
                        var currentGeombuffs = img.CreateRenderGeometry(imgRenderSettings);
                        Debug.Assert(currentGeombuffs.Count <= 1, "Glyphs are expected to be built from a single GeometryBuffer (or none)");
                        if (currentGeombuffs.Count == 1)
                            textGeometryBuffer = currentGeombuffs[0];
                    }
                    else
                    {
                        // Else we add geometry to the rendering batch of the existing geometry
                        img.AddToRenderGeometry(textGeometryBuffer, ref imgRenderSettings.DestArea, ref clipRect, colours);
                    }

                    glyphPos.X += glyph.GetAdvance(xScale);
                    // apply extra spacing to space chars
                    if (c == ' ')
                        glyphPos.X += spaceExtra;
                }

                lastChar = c;
            }

            if (textGeometryBuffer != null)
                geomBuffers.Add(textGeometryBuffer);

            nextGlyphPosX = glyphPos.X;

            // Adding a single geometry buffer containing the batched glyphs
            return geomBuffers;
        }

        /// <summary>
        /// Set the native resolution for this Font
        /// </summary>
        /// <param name="size">
        /// Size object describing the new native screen resolution for this Font.
        /// </param>
        public void SetNativeResolution(Sizef size)
        {
            d_nativeResolution = size;
            
            // re-calculate scaling factors & notify images as required
            NotifyDisplaySizeChanged(System.GetSingleton().GetRenderer().GetDisplaySize());
        }

        /// <summary>
        /// Return the native display size for this Font.  This is only relevant if
        /// the Font is being auto-scaled.
        /// </summary>
        /// <returns>
        /// Size object describing the native display size for this Font.
        /// </returns>
        public Sizef GetNativeResolution()
        {
            return d_nativeResolution;
        }

        /// <summary>
        /// Enable or disable auto-scaling for this Font.
        /// </summary>
        /// <param name="autoScaled">
        /// AutoScaledMode describing how this font should be auto scaled
        /// </param>
        /// <seealso cref="AutoScaledMode"/>
        public void SetAutoScaled(AutoScaledMode autoScaled)
        {
            if (autoScaled == d_autoScaled)
                return;

            d_autoScaled = autoScaled;
            UpdateFont();

            OnRenderSizeChanged(new FontEventArgs(this));
        }

        /// <summary>
        /// Checks whether this font is being auto-scaled and how.
        /// </summary>
        /// <returns>
        /// AutoScaledMode describing how this font should be auto scaled
        /// </returns>
        public AutoScaledMode GetAutoScaled()
        {
            return d_autoScaled;
        }

        /// <summary>
        /// Notify the Font that the display size may have changed.
        /// </summary>
        /// <param name="size">
        /// Size object describing the display resolution
        /// </param>
        public virtual void NotifyDisplaySizeChanged(Sizef size)
        {
            Image.ComputeScalingFactors(d_autoScaled, size, d_nativeResolution, out d_horzScaling, out d_vertScaling);

            if (d_autoScaled != AutoScaledMode.Disabled)
            {
                UpdateFont();

                OnRenderSizeChanged(new FontEventArgs(this));
            }
        }

        /// <summary>
        /// Return the pixel line spacing value for.
        /// </summary>
        /// <param name="yScale">
        /// Scaling factor to be applied to the line spacing, where 1.0f
        /// is considered to be 'normal'.
        /// </param>
        /// <returns>
        /// Number of pixels between vertical base lines, i.e. The minimum
        /// pixel space between two lines of text.
        /// </returns>
        public float GetLineSpacing(float yScale = 1.0f)
        {
            return d_height*yScale;
        }

        /// <summary>
        /// return the exact pixel height of the font.
        /// </summary>
        /// <param name="yScale">
        /// Scaling factor to be applied to the height, where 1.0f
        /// is considered to be 'normal'.
        /// </param>
        /// <returns>
        /// float value describing the pixel height of the font without
        /// any additional padding.
        /// </returns>
        public float GetFontHeight(float yScale = 1.0f)
        {
            return (d_ascender - d_descender)*yScale;
        }

        /// <summary>
        /// Return the number of pixels from the top of the highest glyph to the baseline
        /// </summary>
        /// <param name="yScale">
        /// Scaling factor to be applied to the baseline distance, where 1.0f
        /// is considered to be 'normal'.
        /// </param>
        /// <returns>
        /// pixel spacing from top of front glyphs to baseline
        /// </returns>
        public float GetBaseline(float yScale = 1.0f)
        {
            return d_ascender*yScale;
        }

        /// <summary>
        /// Return the pixel width of the specified text if rendered with
        /// this Font.
        /// </summary>
        /// <param name="text">
        /// String object containing the text to return the rendered pixel
        /// width for.
        /// </param>
        /// <param name="xScale">
        /// Scaling factor to be applied to each glyph's x axis when
        /// measuring the extent, where 1.0f is considered to be 'normal'.
        /// </param>
        /// <returns>
        /// Number of pixels that \a text will occupy when rendered with
        /// this Font.
        /// </returns>
        /// <remarks>
        /// The difference between the advance and the extent of a text string is
        /// important for numerous reasons. Picture some scenario where a glyph
        /// has a swash which extends way beyond the subsequent glyph - the text
        /// extent of those two glyphs is to the end of the swash on the first glyph
        /// whereas the advance of those two glyphs is to the start of a theoretical
        /// third glyph - still beneath the swash of the first glyph.
        /// The difference can basically be summarised as follows:
        ///     - the extent is the total rendered width of all glyphs in the string.
        ///     - the advance is the width to the point where the next character would
        ///       have been drawn.
        /// </remarks>
        /// <seealso cref="GetTextAdvance"/>
        public float GetTextExtent(string text, float xScale = 1.0f)
        {
            var curExtent = 0f;
            var advExtent = 0f;

            for (var c = 0; c < text.Length; ++c)
            {
                var currentCodePoint = text[c];
                var nextIndex = c + 1;
                var nextCodePoint = char.MinValue;
                var isFollowedByAnotherCharacter = false;
                if (nextIndex < text.Length)
                {
                    nextCodePoint = text[nextIndex];
                    isFollowedByAnotherCharacter = true;
                }

                GetGlyphExtents(currentCodePoint, nextCodePoint, isFollowedByAnotherCharacter, ref curExtent, ref advExtent, xScale);
            }

            return Math.Max(advExtent, curExtent);
        }

        /// <summary>
        /// Calculates and returns the size this glyph takes up if it is the last character, or 
        /// the size the glyph takes until the next character begins (considering kerning).
        /// </summary>
        /// <param name="currentCodePoint"></param>
        /// <param name="nextCodePoint"></param>
        /// <param name="isFollowedByAnotherCharacter"></param>
        /// <param name="?"></param>
        /// <param name="curExtent"></param>
        /// <param name="advExtent"></param>
        public void GetGlyphExtents(char currentCodePoint, char nextCodePoint, bool isFollowedByAnotherCharacter, ref float curExtent, ref float advExtent, float x_scale)
        {
            var currentGlyph = GetGlyphData(currentCodePoint);
            if (currentGlyph != null)
            {
                FontGlyph nextGlyph = null;
                
                if(isFollowedByAnotherCharacter)
                    nextGlyph = GetGlyphData(nextCodePoint);
            
                var width = currentGlyph.GetRenderedAdvance(nextGlyph, x_scale);
                
                if (advExtent + width > curExtent)
                    curExtent = advExtent + width;
                
                advExtent += currentGlyph.GetAdvance(x_scale);
            }
        }

        /// <summary>
        /// Return pixel advance of the specified text when rendered with this Font.
        /// </summary>
        /// <param name="text">
        /// String object containing the text to return the pixel advance for.
        /// </param>
        /// <param name="xScale">
        /// Scaling factor to be applied to each glyph's x axis when
        /// measuring the advance, where 1.0f is considered to be 'normal'.
        /// </param>
        /// <returns>
        /// pixel advance of \a text when rendered with this Font.
        /// </returns>
        /// <remarks>
        /// The difference between the advance and the extent of a text string is
        /// important for numerous reasons. Picture some scenario where a glyph
        /// has a swash which extends way beyond the subsequent glyph - the text
        /// extent of those two glyphs is to the end of the swash on the first glyph
        /// whereas the advance of those two glyphs is to the start of a theoretical
        /// third glyph - still beneath the swash of the first glyph.
        /// The difference can basically be summarised as follows:
        /// - the extent is the total rendered width of all glyphs in the string.
        /// - the advance is the width to the point where the next character would
        ///   have been drawn.
        /// </remarks>
        /// <seealso cref="GetTextExtent"/>
        public float GetTextAdvance(string text, float xScale = 1.0f)
        {
            var advance = 0.0f;

            foreach (var c in text)
            {
                var glyph = GetGlyphData(c);
                if (glyph != null)
                    advance += glyph.GetAdvance(xScale);
            }

            return advance;
        }

        /// <summary>
        /// Return the index of the closest text character in String \a text
        /// that corresponds to pixel location \a pixel if the text were rendered.
        /// </summary>
        /// <param name="text">
        /// String object containing the text.
        /// </param>
        /// <param name="pixel">
        /// Specifies the (horizontal) pixel offset to return the character 
        /// index for.
        /// </param>
        /// <param name="xScale">
        /// Scaling factor to be applied to each glyph's x axis when measuring
        /// the text extent, where 1.0f is considered to be 'normal'.
        /// </param>
        /// <returns>
        /// Returns a character index into String \a text for the character that
        /// would be rendered closest to horizontal pixel offset \a pixel if the
        /// text were to be rendered via this Font.  Range of the return is from
        /// 0 to text.length(), so may actually return an index past the end of
        /// the string, which indicates \a pixel was beyond the last character.
        /// </returns>
        public int GetCharAtPixel(string text, float pixel, float xScale = 1.0f)
        {
            return GetCharAtPixel(text, 0, pixel, xScale);
        }

        /// <summary>
        /// Return the index of the closest text character in String \a text,
        /// starting at character index \a start_char, that corresponds
        /// to pixel location \a pixel if the text were to be rendered.
        /// </summary>
        /// <param name="text">
        /// String object containing the text.
        /// </param>
        /// <param name="startChar">
        /// index of the first character to consider.  
        /// This is the lowest value that will be returned from the call.
        /// </param>
        /// <param name="pixel">
        /// Specifies the (horizontal) pixel offset to return the character index for.
        /// </param>
        /// <param name="xScale">
        /// Scaling factor to be applied to each glyph's x axis when measuring
        /// the text extent, where 1.0f is considered to be 'normal'.
        /// </param>
        /// <returns>
        /// Returns a character index into String \a text for the character that
        /// would be rendered closest to horizontal pixel offset \a pixel if the
        /// text were to be rendered via this Font.  Range of the return is from
        /// 0 to text.length(), so may actually return an index past the end of
        /// the string, which indicates \a pixel was beyond the last character.
        /// </returns>
        public int GetCharAtPixel(string text, int startChar, float pixel, float xScale = 1.0f)
        {
            var curExtent = 0f;
            var charCount = text.Length;

            // handle simple cases
            if ((pixel <= 0) || (charCount <= startChar))
                return startChar;

            for (var c = startChar; c < charCount; ++c)
            {
                var glyph = GetGlyphData(text[c]);

                if (glyph != null)
                {
                    curExtent += glyph.GetAdvance(xScale);

                    if (pixel < curExtent)
                        return c;
                }
            }

            return charCount;
        }

        /// <summary>
        /// Sets the default resource group to be used when loading font data
        /// </summary>
        /// <param name="resourceGroup">
        /// String describing the default resource group identifier to be used.
        /// </param>
        public static void SetDefaultResourceGroup(string resourceGroup)
        {
            d_defaultResourceGroup = resourceGroup;
        }

        /// <summary>
        /// Returns the default resource group currently set for Fonts.
        /// </summary>
        /// <returns>
        /// String describing the default resource group identifier that will be
        /// used when loading font data.
        /// </returns>
        public static string GetDefaultResourceGroup()
        {
            return d_defaultResourceGroup;
        }

        /// <summary>
        /// Writes an xml representation of this Font to \a out_stream.
        /// </summary>
        /// <param name="xml_stream">
        /// Stream where xml data should be output.
        /// </param>
        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a pointer to the glyphDat struct for the given codepoint,
        /// or 0 if the codepoint does not have a glyph defined.
        /// </summary>
        /// <param name="codepoint">
        /// utf32 codepoint to return the glyphDat structure for.
        /// </param>
        /// <returns>
        /// Pointer to the glyphDat struct for \a codepoint, or 0 if no glyph
        /// is defined for \a codepoint.
        /// </returns>
        public FontGlyph GetGlyphData(char codepoint)
        {
            if (codepoint > d_maxCodepoint)
                return null;

            var glyph = FindFontGlyph(codepoint);

            if (d_glyphPageLoaded != null)
            {
                // Check if glyph page has been rasterised
                var page = codepoint/GlyphsPerPage;
                var mask = 1 << (page & (BitsPerUint - 1));
                if ((d_glyphPageLoaded[page/BitsPerUint] & mask) != mask)
                {
                    d_glyphPageLoaded[page/BitsPerUint] |= mask;
                    Rasterise((char) (codepoint & ~(GlyphsPerPage - 1)),
                              (char) (codepoint | (GlyphsPerPage - 1)));
                }
            }

            return glyph;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeName"></param>
        /// <param name="filename"></param>
        /// <param name="resourceGroup"></param>
        /// <param name="autoScaled"></param>
        /// <param name="nativeRes"></param>
        protected Font(string name, string typeName, string filename,string resourceGroup, AutoScaledMode autoScaled,Sizef nativeRes)
        {
            d_name = name;
            d_type = typeName;
            d_filename = filename;
            d_resourceGroup = resourceGroup;
            d_ascender = 0;
            d_descender = 0;
            d_height = 0;
            d_autoScaled = autoScaled;
            d_nativeResolution = nativeRes;
            d_maxCodepoint = 0;
            d_glyphPageLoaded = null;

            AddFontProperties();

            var size = System.GetSingleton().GetRenderer().GetDisplaySize();
            Image.ComputeScalingFactors(d_autoScaled, size, d_nativeResolution, out d_horzScaling, out d_vertScaling);
        }

        /// <summary>
        /// This function prepares a certain range of glyphs to be ready for
        /// displaying. This means that after returning from this function
        /// glyphs from d_cp_map[start_codepoint] to d_cp_map[end_codepoint]
        /// should have their d_image member set. If there is an error
        /// during rasterisation of some glyph, it's okay to leave the
        /// d_image field set to NULL, in which case such glyphs will
        /// be skipped from display.
        /// </summary>
        /// <param name="startCodepoint">
        /// The lowest codepoint that should be rasterised
        /// </param>
        /// <param name="endCodepoint">
        /// The highest codepoint that should be rasterised
        /// </param>
        protected virtual void Rasterise(char startCodepoint, char endCodepoint)
        {
            // do nothing by default
        }

        /// <summary>
        /// Update the font as needed, according to the current parameters.
        /// </summary>
        protected abstract void UpdateFont();

        /// <summary>
        /// implementaion version of writeXMLToStream.
        /// </summary>
        /// <param name="xml_stream"></param>
        protected abstract void WriteXMLToStreamImpl(XMLSerializer xml_stream);

        /// <summary>
        /// Register all properties of this class.
        /// </summary>
        protected void AddFontProperties()
        {
            AddProperty(new TplWindowProperty<Font, Sizef>(
                            "NativeRes",
                            "Native screen resolution for this font. Value uses the 'w:# h:#' format.",
                            (x, v) => x.SetNativeResolution(v), x => x.GetNativeResolution(), "Font", Sizef.Zero));

            AddProperty(new TplWindowProperty<Font, string>(
                            "Name", 
                            "This is font name.  Value is a string.",
                            null, x => x.GetName(), "Font", String.Empty));

            AddProperty(new TplWindowProperty<Font, AutoScaledMode>(
                            "AutoScaled",
                            "This indicating whether and how to autoscale font depending on resolution.  Value can be 'false', 'vertical', 'horizontal' or 'true'.",
                            (x, v) => x.SetAutoScaled(v), x => x.GetAutoScaled(), "Font", AutoScaledMode.Disabled));
        }

        /// <summary>
        /// event trigger function for when the font rendering size changes.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnRenderSizeChanged(FontEventArgs args)
        {
            var handler = RenderSizeChanged;
            if (handler != null)
                handler(this, args);
        }

        /// <summary>
        /// Set the maximal glyph index. This reserves the respective
        /// number of bits in the d_glyphPageLoaded array.
        /// </summary>
        /// <param name="codepoint"></param>
        protected void SetMaxCodepoint(char codepoint)
        {
            //if (d_glyphPageLoaded!=null)
            //{
            //    const uint old_size = (((d_maxCodepoint + GLYPHS_PER_PAGE) / GLYPHS_PER_PAGE)
            //        + BITS_PER_UINT - 1) / BITS_PER_UINT;

            //    CEGUI_DELETE_ARRAY_PT(d_glyphPageLoaded, uint, old_size, Font);
            //}
            
            d_maxCodepoint = codepoint;

            d_glyphPageLoaded = null;

            var npages = (codepoint + GlyphsPerPage) / GlyphsPerPage;
            var size = (npages + BitsPerUint - 1) / BitsPerUint;
            d_glyphPageLoaded = new int[size];
        }

        /// <summary>
        /// finds FontGlyph in map and returns it, or null if none.
        /// </summary>
        /// <param name="codepoint"></param>
        /// <returns></returns>
        protected virtual FontGlyph FindFontGlyph(char codepoint)
        {
            FontGlyph fontGlyph = null;
            d_cp_map.TryGetValue(codepoint, out fontGlyph);
            return fontGlyph;
        }

        private float GetKerningAmount(char first, char second)
        {
            if (_kerningPairs.Count == 0)
                return 0f;

            if (_kerningPairs.ContainsKey(first))
            {
                var pairs = _kerningPairs[first];
                if (pairs.ContainsKey(second))
                    return pairs[second];
            }

            return 0f;
        }

        #region Fields

        /// <summary>
        /// Name of this font.
        /// </summary>
        protected string d_name;

        //! Type name string for this font (not used internally)
        protected string d_type;
        
        //! Name of the file used to create this font (font file or imagset)
        protected string d_filename;
        
        //! Name of the font file's resource group.
        protected string d_resourceGroup;
        
        //! Holds default resource group for font loading.
        protected static string d_defaultResourceGroup;

        //! maximal font ascender (pixels above the baseline)
        protected float d_ascender;
        
        //! maximal font descender (negative pixels below the baseline)
        protected float d_descender;
        
        //! (ascender - descender) + linegap
        protected float d_height;

        //! which mode should we use for auto-scaling
        protected AutoScaledMode d_autoScaled;
        
        //! native resolution for this Font.
        protected Sizef d_nativeResolution;
        
        //! current horizontal scaling factor.
        protected float d_horzScaling;
        
        //! current vertical scaling factor.
        protected float d_vertScaling;

        //! Maximal codepoint for font glyphs
        protected int d_maxCodepoint;

        /// <summary>
        /// This bitmap holds information about loaded 'pages' of glyphs.
        /// A glyph page is a set of 256 codepoints, starting at 256-multiples.
        /// For example, the 1st glyph page is 0-255, fourth is 1024-1279 etc.
        /// When a specific glyph is required for painting, the corresponding
        /// bit is checked to see if the respective page has been rasterised.
        /// If not, the rasterise() method is invoked, which prepares the
        /// glyphs from the respective glyph page for being painted.
        /// 
        /// This array is big enough to hold at least max_codepoint bits.
        /// If this member is NULL, all glyphs are considered pre-rasterised.
        /// </summary>
        protected int[] d_glyphPageLoaded;

        /// <summary>
        /// Contains mappings from code points to Image objects
        /// </summary>
        protected Dictionary<char, FontGlyph> d_cp_map = new Dictionary<char, FontGlyph>();

        protected Dictionary<char, Dictionary<char, float>> _kerningPairs = new Dictionary<char, Dictionary<char, float>>();

        /// <summary>
        /// amount of bits in a uint
        /// </summary>
        private const int BitsPerUint = sizeof(int) * 8;

        /// <summary>
        /// must be a power of two 
        /// </summary>
        private const int GlyphsPerPage = 256;

        #endregion
    }
}