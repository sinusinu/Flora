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

            textures = new List<IntPtr>();
            glyphInfos = new Dictionary<ushort, GlyphInfo>();

            font = SDL_ttf.TTF_OpenFont(path, size);
            if (font == IntPtr.Zero) throw new InvalidOperationException("Cannot open font: " + SDL.SDL_GetError());
        }

        ~Font() {
            ClearCache();
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

            // get glyph texture
            var glyphSurface = SDL_ttf.TTF_RenderGlyph_Blended(font, glyph, white);
            var glyphTexture = SDL.SDL_CreateTextureFromSurface(Gfx.sdlRenderer, glyphSurface);
            SDL.SDL_FreeSurface(glyphSurface);

            uint udummy; int dummy; int w; int h;
            SDL.SDL_QueryTexture(glyphTexture, out udummy, out dummy, out w, out h);

            // create first texture if none exists
            if (textures.Count == 0) {
                var newTexture = SDL.SDL_CreateTexture(Gfx.sdlRenderer, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, TextureSize, TextureSize);
                SDL.SDL_SetTextureBlendMode(newTexture, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
                textures.Add(newTexture);
            }

            // will return this
            GlyphInfo gi = null;

            // I know this part is burning pile of garbage, will consider optimizing sometime later
            bool isGlyphPlaced = false;
            // for each page...
            for (int i = 0; i < textures.Count; i++) {
                // get all glyph rects in this page
                List<Rect> rectsOfThisPage = new List<Rect>();
                foreach (var j in glyphInfos) if (j.Value.page == i) {
                    rectsOfThisPage.Add(j.Value.rect);
                } 
                
                // if this page is empty, draw here on (0, 0)
                if (rectsOfThisPage.Count == 0) {
                    SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, textures[i]);
                    Rect targetRect = new Rect(0, 0, w, h);
                    var targetRectS = targetRect.ToSDLRect();
                    gi = new GlyphInfo(i, targetRect);
                    SDL.SDL_RenderCopy(Gfx.sdlRenderer, glyphTexture, IntPtr.Zero, ref targetRectS);
                    glyphInfos.Add(glyph, gi);
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
                        foreach (var rr in rectsOfThisPage) {
                            if (Rect.Intersect(testRect, rr)) isVacant = false;
                        }
                        if (isVacant) {
                            // then place it here
                            SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, textures[i]);
                            var targetRect = testRect.ToSDLRect();
                            gi = new GlyphInfo(i, testRect);
                            SDL.SDL_RenderCopy(Gfx.sdlRenderer, glyphTexture, IntPtr.Zero, ref targetRect);
                            glyphInfos.Add(glyph, gi);
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
                            gi = new GlyphInfo(i, testRect);
                            SDL.SDL_RenderCopy(Gfx.sdlRenderer, glyphTexture, IntPtr.Zero, ref targetRect);
                            glyphInfos.Add(glyph, gi);
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
                SDL.SDL_SetTextureBlendMode(newTexture, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
                textures.Add(newTexture);

                // and draw on new texture at (0, 0)
                SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, textures[textures.Count - 1]);
                Rect targetRect = new Rect(0, 0, w, h);
                var targetRectS = targetRect.ToSDLRect();
                gi = new GlyphInfo(textures.Count - 1, targetRect);
                SDL.SDL_RenderCopy(Gfx.sdlRenderer, glyphTexture, IntPtr.Zero, ref targetRectS);
                glyphInfos.Add(glyph, gi);
                SDL.SDL_SetRenderTarget(Gfx.sdlRenderer, IntPtr.Zero);
            }

            SDL.SDL_DestroyTexture(glyphTexture);

            return gi;
        }

        internal void ClearCache() {
            foreach (var tx in textures) SDL.SDL_DestroyTexture(tx);
            textures.Clear();
            glyphInfos.Clear();
        }

        /// <summary>
        /// Sets hinting of this font. Font cache gets cleared on call.
        /// </summary>
        /// <param name="newHinting">New hinting for this font (Default is Normal)</param>
        public void SetHinting(FontHinting hinting) {
            ClearCache();
            SDL_ttf.TTF_SetFontHinting(font, (int)hinting);
        }
        
        /// <summary>
        /// Sets scaling of this font.
        /// </summary>
        /// <param name="scale">New scale for this font</param>
        public void SetScale(float scale) {
            this.scale = scale;
        }

        /// <summary>
        /// Draw given text with this font.
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="x">X position of the text (top-left)</param>
        /// <param name="y">Y position of the text (top-left)</param>
        public void Draw(string text, int x, int y) {
            var charArray = text.ToCharArray();
            var stringGlyphs = new ushort[charArray.Length];
            for (int i = 0; i < charArray.Length; i++) stringGlyphs[i] = charArray[i];

            int currentX = x;

            foreach (var glyph in stringGlyphs) {
                var glyphInfo = GetGlyphInfo(glyph);
                if (glyphInfo.page == -1) continue;

                var dstRect = new SDL.SDL_Rect();
                dstRect.x = x + currentX;
                dstRect.y = y;
                dstRect.w = (int)(glyphInfo.rect.w * scale);
                dstRect.h = (int)(glyphInfo.rect.h * scale);
                Gfx.DrawGlyph(textures[glyphInfo.page], glyphInfo.rect.ToSDLRect(), ref dstRect, 0, dstRect.w / 2, dstRect.h / 2, Gfx.FlipMode.None);

                currentX += dstRect.w;
            }
        }
    }
}