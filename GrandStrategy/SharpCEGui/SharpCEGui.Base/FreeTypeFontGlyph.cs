#if CEGUI_HAS_FREETYPE

namespace SharpCEGui.Base
{
    /// <summary>
    /// Internal class representing a single FreeType font glyph.
    /// 
    /// For TrueType fonts initially all FontGlyphs are empty
    /// (getImage() will return a nullptr), but they are filled by demand.
    /// </summary>
    public class FreeTypeFontGlyph : FontGlyph
    {
        public FreeTypeFontGlyph(FreeTypeFont freeTypeFont, uint glyphIndex, float advance = 0.0f, Image image = null,
                                 bool valid = false)
                : base(advance, image, valid)
        {
            d_freeTypeFont = freeTypeFont;
            d_glyphIndex = glyphIndex;
        }

        // TODO: ~FreeTypeFontGlyph() {}

        public override float GetRenderedAdvance(FontGlyph nextGlyph, float xScale)
        {
            bool isFollowedByAnotherCharacter = (nextGlyph != null);
            var nextGlyphFT = nextGlyph as FreeTypeFontGlyph;

            if (isFollowedByAnotherCharacter && nextGlyphFT == null)
            {
                throw new InvalidRequestException(
                        "FreeTypeFontGlyph::getRenderedAdvance - Attempted to cast following Font Glyph to a FreeTypeFontGlyph has failed. " +
                        "This should not occur because FreeTypeFontGlyphs shall be followed by FreeTypeFontGlyphs only.");
            }

            var sizeX = GetImage().GetRenderedSize().Width + GetImage().GetRenderedOffset().X;
            sizeX *= xScale;

            // Last character, no kerning is done
            if (!isFollowedByAnotherCharacter)
            {
                return sizeX;
            }

            //// Determine kerning
            //FT_Vector kerning;

            var face = d_freeTypeFont.GetFontFace();
            var kerningMode = d_freeTypeFont.GetKerningMode();

            if (face == null)
            {
                throw new InvalidRequestException("FreeTypeFontGlyph::getRenderedAdvance - Attempted to access Font Face of a FreeType font, but it is not set to a valid Face.");
            }

            var leftGlyphIndex = d_glyphIndex;
            var rightGlyphIndex = nextGlyphFT.GetGlyphIndex();
            
            var kerning = face.GetKerning(leftGlyphIndex, rightGlyphIndex, kerningMode);

            //if (error != 0)
            //{
            //    throw new InvalidRequestException("FreeTypeFontGlyph::getRenderedAdvance - Kerning returned with error code " + error);
            //}

            sizeX += (float)kerning.X*xScale;

            return sizeX;
        }

        /// <summary>
        /// Returns the FreeType glyph index for this glyph.
        /// </summary>
        /// <returns></returns>
        public uint GetGlyphIndex()
        {
            return d_glyphIndex;
        }

        /// <summary>
        /// Sets the FreeType glyph index for this glyph.
        /// </summary>
        /// <param name="value"></param>
        public void SetGlyphIndex(uint value)
        {
            d_glyphIndex = value;
        }

        private FreeTypeFont d_freeTypeFont;
        private uint d_glyphIndex;
    }
}

#endif