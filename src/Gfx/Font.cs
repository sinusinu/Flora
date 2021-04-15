using System;
using System.Collections.Generic;
using SDL2;
using Flora.Util;

namespace Flora.Gfx {
    public class Font {
        internal const int TextureSize = 1024;

        internal IntPtr font;
        internal List<IntPtr> textures;
        internal Dictionary<ushort, GlyphInfo> glyphInfos;
        internal float scale = 1f;

        public Font(string path, int size) {
            if (!Gfx.isGfxInitialized) throw new InvalidOperationException("Flora.Gfx is not initialized.");

            if (size < 2) throw new ArgumentException("Font size must be bigger than 1");
            if (size > 256) throw new ArgumentException("Font size must be smaller than 256");

            font = SDL_ttf.TTF_OpenFont(path, size);
        }

        ~Font() {
            SDL_ttf.TTF_CloseFont(font);
        }

        internal GlyphInfo GetGlyphInfo(ushort glyph) {
            if (glyphInfos.ContainsKey(glyph)) return glyphInfos[glyph];
            return PlaceGlyph(glyph);
        }

        internal GlyphInfo PlaceGlyph(ushort glyph) {
            if (SDL_ttf.TTF_GlyphIsProvided(font, glyph) == 0) {
                var emptyGi = new GlyphInfo();
                glyphInfos[glyph] = emptyGi;
                return emptyGi;
            }

            var gi = new GlyphInfo();

            int minx, maxx, miny, maxy, advance;
            SDL_ttf.TTF_GlyphMetrics(font, glyph, out minx, out maxx, out miny, out maxy, out advance);
            
            Rect rect = new Rect(0, 0, maxx - minx, maxy - miny);   // x, y will be filled after packing

            // TODO: pack glyph into pages

            // TODO: set available texture as render target and draw glyph on rect

            return gi;
        }

        public void SetScale(float scale) {
            this.scale = scale;
        }

        public void Draw(string text, int x, int y) {
            var charArray = text.ToCharArray();
            var stringGlyphs = new ushort[charArray.Length];
            for (int i = 0; i < charArray.Length; i++) stringGlyphs[i] = charArray[i];

            // TODO: foreach glyph GetGlyphInfo and draw
            // check doc of TTF_GlyphMetrics
        }
    }
}