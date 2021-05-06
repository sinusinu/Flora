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
        internal Color color = new Color(0xFF, 0xFF, 0xFF, 0xFF);
        internal float _lineHeight;

        public float lineHeight { get { return _lineHeight * scale; }}

        public Font(string path, int size) {
            if (!Gfx.isGfxInitialized) throw new InvalidOperationException("Gfx is not initialized");

            if (size < 2) throw new ArgumentException("Font size must be bigger than 1");
            if (size > 255) throw new ArgumentException("Font size must be smaller than 256");

            // is there any better way to do this?
            white.r = 0xFF;
            white.g = 0xFF;
            white.b = 0xFF;
            white.a = 0xFF;

            textures = new List<IntPtr>();
            glyphInfos = new Dictionary<ushort, GlyphInfo>();

            font = SDL_ttf.TTF_OpenFont(path, size);
            if (font == IntPtr.Zero) throw new InvalidOperationException("Cannot open font: " + SDL.SDL_GetError());

            _lineHeight = SDL_ttf.TTF_FontLineSkip(font);
        }

        ~Font() {
            ClearCache();
            SDL_ttf.TTF_CloseFont(font);
        }

        internal GlyphInfo GetGlyphInfo(ushort glyph) {
            // return GlyphInfo for given glyph if it is already drawn
            if (glyphInfos.ContainsKey(glyph)) return glyphInfos[glyph];
            
            // if not: draw glyph, generate new GlyphInfo and return

            // this glyph is not supported by font - don't draw it
            if (SDL_ttf.TTF_GlyphIsProvided(font, glyph) == 0) {
                var emptyGi = new GlyphInfo();
                glyphInfos[glyph] = emptyGi;
                return emptyGi;
            }

            // get glyph texture
            var glyphSurface = SDL_ttf.TTF_RenderGlyph_Blended(font, glyph, white);
            var glyphTexture = SDL.SDL_CreateTextureFromSurface(Gfx.sdlRenderer, glyphSurface);
            SDL.SDL_SetTextureBlendMode(glyphTexture, SDL.SDL_BlendMode.SDL_BLENDMODE_NONE);
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
                }

                // if it's drawn in this page, skip checking pages after this
                if (isGlyphPlacedOnThisPage) {
                    isGlyphPlaced = true;
                    break;
                }

                // is this the best way to achieve right-first placement?
                foreach (var r in rectsOfThisPage) {
                    Rect testRect = new Rect(0, 0, w, h);

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
        /// Set the color of this font.
        /// </summary>
        /// <param name="newColor">New color</param>
        public void SetColor(Color newColor) {
            SetColor(newColor.r, newColor.g, newColor.b, newColor.a);
        }

        /// <summary>
        /// Set the color of this font.
        /// </summary>
        /// <param name="r">Red (0-1)</param>
        /// <param name="g">Green (0-1)</param>
        /// <param name="b">Blue (0-1)</param>
        /// <param name="a">Alpha (0-1)</param>
        public void SetColor(float r, float g, float b, float a) {
            SetColor((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
        }

        /// <summary>
        /// Set the color of this font.
        /// </summary>
        /// <param name="r">Red (0-255)</param>
        /// <param name="g">Green (0-255)</param>
        /// <param name="b">Blue (0-255)</param>
        /// <param name="a">Alpha (0-255)</param>
        public void SetColor(byte r, byte g, byte b, byte a) {
            color.r = r;
            color.g = g;
            color.b = b;
            color.a = a;
        }
        
        /// <summary>
        /// Sets scaling of this font.
        /// </summary>
        /// <param name="scale">New scale for this font</param>
        public void SetScale(float scale) {
            this.scale = scale;
        }

        /// <summary>
        /// Measure the size of given text with this font.
        /// </summary>
        /// <param name="text">Text to measure</param>
        /// <returns></returns>
        public (float, float) Measure(string text) {
            if (text == null || text.Length == 0) return (0, 0);
            
            // for easier detection of line break
            text = text.Replace("\r", "");

            var charArray = text.ToCharArray();
            var stringGlyphs = new ushort[charArray.Length];
            for (int i = 0; i < charArray.Length; i++) stringGlyphs[i] = charArray[i];

            float maxWidth = 0;
            float currentWidth = 0;
            float stackedHeight = _lineHeight;

            foreach (var glyph in stringGlyphs) {
                if (glyph == (ushort)'\n') {
                    currentWidth = 0;
                    stackedHeight += _lineHeight;
                    continue;
                }

                var glyphInfo = GetGlyphInfo(glyph);
                if (glyphInfo.page == -1) continue;

                currentWidth += glyphInfo.rect.w * scale;
                maxWidth = Math.Max(currentWidth, maxWidth);
            }

            return (maxWidth, stackedHeight);
        }

        /// <summary>
        /// Draw given text with this font.
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="x">X position of the text (top-left)</param>
        /// <param name="y">Y position of the text (top-left)</param>
        public void Draw(string text, float x, float y) {
            if (!Gfx.isDrawing) throw new InvalidOperationException("Draw must be called between Gfx.Begin and Gfx.End");

            if (text == null || text.Length == 0) return;

            // for easier detection of line break
            text = text.Replace("\r", "");

            var charArray = text.ToCharArray();
            var stringGlyphs = new ushort[charArray.Length];
            for (int i = 0; i < charArray.Length; i++) stringGlyphs[i] = charArray[i];

            float currentX = 0;
            float currentY = 0;

            foreach (var glyph in stringGlyphs) {
                if (glyph == (ushort)'\n') {
                    currentX = 0;
                    currentY += _lineHeight;
                    continue;
                }

                var glyphInfo = GetGlyphInfo(glyph);
                if (glyphInfo.page == -1) continue;

                var dstRect = new SDL.SDL_FRect();
                dstRect.x = x + currentX;
                dstRect.y = y + currentY;
                dstRect.w = glyphInfo.rect.w * scale;
                dstRect.h = glyphInfo.rect.h * scale;

                Gfx.DrawGlyph(textures[glyphInfo.page], glyphInfo.rect.ToSDLRect(), dstRect, 0, 0, 0, Gfx.FlipMode.None, color);

                currentX += glyphInfo.rect.w * scale;
            }
        }
    }
}