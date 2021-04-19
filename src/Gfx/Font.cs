using System;
using System.Collections.Generic;
using SDL2;
using Flora.Util;

namespace Flora.Gfx {
    public class Font {
        internal const int TextureSize = 1024;
        internal static SDL.SDL_Color white = new SDL.SDL_Color();

        internal IntPtr font;
        internal List<IntPtr> textures;
        internal Dictionary<ushort, GlyphInfo> glyphInfos;
        internal float scale = 1f;

        public Font(string path, int size) {
            if (!Gfx.isGfxInitialized) throw new InvalidOperationException("Gfx is not initialized");

            if (size < 2) throw new ArgumentException("Font size must be bigger than 1");
            if (size > 256) throw new ArgumentException("Font size must be smaller than 256");

            white.r = 0xFF;
            white.g = 0xFF;
            white.b = 0xFF;
            white.a = 0xFF;

            font = SDL_ttf.TTF_OpenFont(path, size);
        }

        ~Font() {
            SDL_ttf.TTF_CloseFont(font);
        }

        internal GlyphInfo GetGlyphInfo(ushort glyph) {
            if (glyphInfos.ContainsKey(glyph)) return glyphInfos[glyph];
            return PlaceGlyph(glyph);
        }

        // glyph must not be in glyphInfos - call GetGlyphInfo to implicitly check it
        internal GlyphInfo PlaceGlyph(ushort glyph) {
            // this glyph is not supported by font - don't draw it
            if (SDL_ttf.TTF_GlyphIsProvided(font, glyph) == 0) {
                var emptyGi = new GlyphInfo();
                glyphInfos[glyph] = emptyGi;
                return emptyGi;
            }

            var gi = new GlyphInfo();

            // get glyph texture
            var glyphSurface = SDL_ttf.TTF_RenderGlyph_Blended(font, glyph, white);
            var glyphTexture = SDL.SDL_CreateTextureFromSurface(Gfx.sdlRenderer, glyphSurface);
            SDL.SDL_FreeSurface(glyphSurface);

            uint udummy; int dummy; int w; int h;
            SDL.SDL_QueryTexture(glyphTexture, out udummy, out dummy, out w, out h);

            // create first texture if none exists
            if (textures.Count == 0) {
                var newTexture = SDL.SDL_CreateTexture(Gfx.sdlRenderer, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, TextureSize, TextureSize);
                textures.Add(newTexture);
            }

            // I know this part is burning pile of garbage, will consider optimizing sometime later
            bool isGlyphPlaced = false;
            // for each page...
            for (int i = 0; i < textures.Count; i++) {
                // get all glyph rects in this page
                List<Rect> rectsOfThisPage = new List<Rect>();
                foreach (var j in glyphInfos) if (j.Value.page == i) rectsOfThisPage.Add(j.Value.rect);
                
                // if this page is empty, draw here on (0, 0)
                if (rectsOfThisPage.Count == 0) {
                    SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, textures[i]);
                    Rect targetRect = new Rect(0, 0, w, h);
                    var targetRectS = targetRect.ToSDLRect();
                    SDL.SDL_RenderCopy(Gfx.sdlRenderer, glyphTexture, IntPtr.Zero, ref targetRectS);
                    glyphInfos.Add(glyph, new GlyphInfo(glyph, targetRect));
                    SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, IntPtr.Zero);
                    isGlyphPlaced = true;
                    break;
                }
                
                bool isGlyphPlacedOnThisPage = false;
                // for each rects of this page...
                foreach (var r in rectsOfThisPage) {
                    Rect testRect = new Rect(0, 0, w, h);

                    // check right of this rect
                    testRect.x = r.x + r.w;
                    testRect.y = r.y;
                    // if rectangle fit within texture...
                    if (testRect.x + testRect.w <= TextureSize && testRect.y + testRect.h <= TextureSize) {
                        // and if it does not intersect with rects of this page...
                        bool isVacant = true;
                        foreach (var rr in rectsOfThisPage) if (Rect.Intersect(testRect, rr)) isVacant = false;
                        if (isVacant) {
                            // then place it here
                            SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, textures[i]);
                            var targetRect = testRect.ToSDLRect();
                            SDL.SDL_RenderCopy(Gfx.sdlRenderer, glyphTexture, IntPtr.Zero, ref targetRect);
                            glyphInfos.Add(glyph, new GlyphInfo(glyph, testRect));
                            SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, IntPtr.Zero);
                            isGlyphPlacedOnThisPage = true;
                            break;
                        }
                    }

                    // check bottom of this rect
                    testRect.x = r.x;
                    testRect.y = r.y + r.h;
                    // if rectangle fit within texture...
                    if (testRect.x + testRect.w <= TextureSize && testRect.y + testRect.h <= TextureSize) {
                        // and if it does not intersect with rects of this page...
                        bool isVacant = true;
                        foreach (var rr in rectsOfThisPage) if (Rect.Intersect(testRect, rr)) isVacant = false;
                        if (isVacant) {
                            // then place it here
                            SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, textures[i]);
                            var targetRect = testRect.ToSDLRect();
                            SDL.SDL_RenderCopy(Gfx.sdlRenderer, glyphTexture, IntPtr.Zero, ref targetRect);
                            glyphInfos.Add(glyph, new GlyphInfo(glyph, testRect));
                            SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, IntPtr.Zero);
                            isGlyphPlacedOnThisPage = true;
                            break;
                        }
                    }
                }

                // if it's drawn in this page, skip checking pages after this
                if (isGlyphPlacedOnThisPage) {
                    isGlyphPlaced = true;
                    break;
                }
            }
            
            // if glyph is not placed...
            if (!isGlyphPlaced) {
                // we need a new page
                var newTexture = SDL.SDL_CreateTexture(Gfx.sdlRenderer, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, TextureSize, TextureSize);
                textures.Add(newTexture);

                // and draw on new texture at (0, 0)
                SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, textures[textures.Count - 1]);
                Rect targetRect = new Rect(0, 0, w, h);
                var targetRectS = targetRect.ToSDLRect();
                SDL.SDL_RenderCopy(Gfx.sdlRenderer, glyphTexture, IntPtr.Zero, ref targetRectS);
                glyphInfos.Add(glyph, new GlyphInfo(glyph, targetRect));
                SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, IntPtr.Zero);
            }

            SDL.SDL_DestroyTexture(glyphTexture);
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