using System.IO.Pipelines;
using System.Text;
using SDL;

namespace Flora;

public unsafe class Font : IDisposable {
    private const int TextureSize = 2048;

    private Application app;

    private TTF_Font* font;
    private List<nint> glyphAtlases = new();
    private Dictionary<uint, GlyphInfo?> glyphInfos = new(); // key is unicode codepoint
    
    public float Scale { get; set; } = 1f;
    public Color Color { get; set; } = new Color(1f, 1f, 1f, 1f);
    internal float _rawLineHeight;
    public float LineHeight => _rawLineHeight * Scale;

    private readonly SDL_Color white = new SDL_Color() { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF };

    internal Font(Application app, string path, float size) {
        if (size < 2) throw new ArgumentException("Font size must be larger than 1");
        if (size > 255) throw new ArgumentException("Font size must be smaller than 256");

        this.app = app;

        font = SDL3_ttf.TTF_OpenFont(path, size);
        if (font == null) throw new InvalidOperationException($"Failed to open font: {SDL3.SDL_GetError()}");

        _rawLineHeight = SDL3_ttf.TTF_GetFontLineSkip(font);
    }

    internal GlyphInfo? GetGlyphInfo(uint glyph) {
        if (glyphInfos.ContainsKey(glyph)) return glyphInfos[glyph];
        
        // return empty glyphinfo if the font does not have this glyph
        if (!SDL3_ttf.TTF_FontHasGlyph(font, glyph)) {
            glyphInfos[glyph] = null;
            return null;
        }

        // create glyph texture
        var glyphSurface = SDL3_ttf.TTF_RenderGlyph_Blended(font, glyph, white);
        var glyphTexture = SDL3.SDL_CreateTextureFromSurface(app.Window.sdlRenderer, glyphSurface);
        SDL3.SDL_SetTextureBlendMode(glyphTexture, SDL_BlendMode.SDL_BLENDMODE_NONE);
        SDL3.SDL_DestroySurface(glyphSurface);

        float w = 0f; float h = 0f;
        SDL3.SDL_GetTextureSize(glyphTexture, &w, &h);

        // will return this
        GlyphInfo? ret = null;

        // take the fast track for the very first glyph
        if (glyphAtlases.Count == 0) {
            CreateNewAtlas();
            PlaceGlyph((SDL_Texture*)glyphAtlases[0], glyphTexture, new Rect(1, 1, w, h));
            ret = new(0, new Rect(0, 0, w + 2f, h + 2f));
            glyphInfos.Add(glyph, ret);
            return ret;
        }

        // TODO: laugh at this abomination and do nothing
        bool isGlyphPlaced = false;
        // for each page...
        for (int i = 0; i < glyphAtlases.Count; i++) {
            bool isGlyphPlacedOnThisPage = false;
            List<Rect> rectsOfThisPage = new();
            foreach (var j in glyphInfos) if (j.Value?.page == i) rectsOfThisPage.Add(j.Value.paddedRect);

            if (rectsOfThisPage.Count == 0) {
                // this page is empty, place glyph on top left
                PlaceGlyph((SDL_Texture*)glyphAtlases[i], glyphTexture, new Rect(1, 1, w, h));
                ret = new(i, new Rect(0, 0, w + 1f, h + 1f));
                glyphInfos.Add(glyph, ret);
                isGlyphPlaced = true;
                break;
            } else {
                Rect testRect = new Rect(0, 0, w + 1f, h + 1f);

                // check right of each rects
                foreach (var r in rectsOfThisPage) {
                    testRect.x = r.x + r.w;
                    testRect.y = r.y;
                    // if testrect fit within texture...
                    if (testRect.x + testRect.w <= TextureSize && testRect.y + testRect.h <= TextureSize) {
                        // and if it does not intersect with existing rects...
                        bool isVacant = true;
                        foreach (var rr in rectsOfThisPage) if (Rect.Intersect(testRect, rr)) { isVacant = false; break; }
                        if (isVacant) {
                            // can be placed here
                            PlaceGlyph((SDL_Texture*)glyphAtlases[i], glyphTexture, new(testRect.x + 1f, testRect.y + 1f, testRect.w - 1f, testRect.h - 1f));
                            ret = new(i, testRect);
                            glyphInfos.Add(glyph, ret);
                            isGlyphPlacedOnThisPage = true;
                            break;
                        }
                    }
                }

                // break if succeeded on right side
                if (isGlyphPlacedOnThisPage) {
                    isGlyphPlaced = true;
                    break;
                }

                // check below of each rects
                foreach (var r in rectsOfThisPage) {
                    testRect.x = r.x;
                    testRect.y = r.y + r.h;
                    // if testrect fit within texture...
                    if (testRect.x + testRect.w <= TextureSize && testRect.y + testRect.h <= TextureSize) {
                        // and if it does not intersect with existing rects...
                        bool isVacant = true;
                        foreach (var rr in rectsOfThisPage) if (Rect.Intersect(testRect, rr)) { isVacant = false; break; }
                        if (isVacant) {
                            // can be placed here
                            PlaceGlyph((SDL_Texture*)glyphAtlases[i], glyphTexture, new(testRect.x + 1f, testRect.y + 1f, testRect.w - 1f, testRect.h - 1f));
                            ret = new(i, testRect);
                            glyphInfos.Add(glyph, ret);
                            isGlyphPlacedOnThisPage = true;
                            break;
                        }
                    }
                }

                // break if succeeded on below side
                if (isGlyphPlacedOnThisPage) {
                    isGlyphPlaced = true;
                    break;
                }

                // loop: check next page
            }
        }

        if (!isGlyphPlaced) {
            // couldn't place on all atlases - means we need a new one
            int newAtlasIndex = CreateNewAtlas() - 1;
            PlaceGlyph((SDL_Texture*)glyphAtlases[newAtlasIndex], glyphTexture, new(1, 1, w, h));
            ret = new(newAtlasIndex, new Rect(0, 0, w + 1f, h + 1f));
            glyphInfos.Add(glyph, ret);
        }

        SDL3.SDL_DestroyTexture(glyphTexture);

        return ret;
    }

    private int CreateNewAtlas() {
        var newAtlasTexture = SDL3.SDL_CreateTexture(app.Window.sdlRenderer, SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, TextureSize, TextureSize);
        SDL3.SDL_SetTextureBlendMode(newAtlasTexture, SDL_BlendMode.SDL_BLENDMODE_BLEND);
        glyphAtlases.Add((nint)newAtlasTexture);
        return glyphAtlases.Count;
    }

    private void PlaceGlyph(SDL_Texture* texture, SDL_Texture* glyph, Rect rect) {
        SDL_FRect dstRect = (SDL_FRect)rect;
        SDL3.SDL_SetRenderTarget(app.Window.sdlRenderer, texture);
        SDL3.SDL_RenderTexture(app.Window.sdlRenderer, glyph, null, &dstRect);
        SDL3.SDL_SetRenderTarget(app.Window.sdlRenderer, null);
    }

    public (float, float) Measure(string? text) {
        if (string.IsNullOrEmpty(text)) return (0, 0);
        
        // simplify line break detection
        text = text.Replace("\r", "");

        uint[] utf32 = ToUtf32(text);

        float maxWidth = 0;
        float currentWidth = 0;
        float stackedHeight = LineHeight;

        foreach (var glyph in utf32) {
            if (glyph == '\n') {
                currentWidth = 0;
                stackedHeight += LineHeight;
                continue;
            }

            var glyphInfo = GetGlyphInfo(glyph);
            if (glyphInfo == null) continue;

            currentWidth += glyphInfo.paddedRect.w * Scale;
            if (currentWidth > maxWidth) maxWidth = currentWidth;
        }

        return (maxWidth, stackedHeight);
    }

    public void Draw(string? text, float x, float y) {
        if (!app.Gfx.isDrawing) throw new InvalidOperationException("Draw calls must be placed inbetween Begin and End calls");
        
        if (string.IsNullOrWhiteSpace(text)) return;

        var originalRenderColor = app.Gfx.RenderColor;
        app.Gfx.RenderColor = Color;

        // simplify line break detection
        text = text.Replace("\r", "");

        uint[] utf32 = ToUtf32(text);

        float currentX = 0;
        float currentY = 0;

        foreach (var glyph in utf32) {
            if (glyph == '\n') {
                currentX = 0;
                currentY += LineHeight;
                continue;
            }

            var glyphInfo = GetGlyphInfo(glyph);
            if (glyphInfo == null) continue;

            var dstRect = new SDL.SDL_FRect() {
                x = x + currentX,
                y = y + currentY,
                w = glyphInfo.paddedRect.w * Scale,
                h = glyphInfo.paddedRect.h * Scale
            };

            app.Gfx.DrawRawTexture((SDL_Texture*)glyphAtlases[glyphInfo.page], x + currentX, y + currentY, glyphInfo.paddedRect.w * Scale, glyphInfo.paddedRect.h * Scale, 0, 0, 0, (int)glyphInfo.paddedRect.x, (int)glyphInfo.paddedRect.y, (int)glyphInfo.paddedRect.w, (int)glyphInfo.paddedRect.h);

            currentX += glyphInfo.paddedRect.w * Scale;
        }
        
        app.Gfx.RenderColor = originalRenderColor;
    }

    // internal void DebugDrawFirstAtlas(float x, float y) {
    //     app.Gfx.DrawRawTexture((SDL_Texture*)glyphAtlases[0], x, y, TextureSize, TextureSize, 0, 0, 0, 0, 0, TextureSize, TextureSize);
    // }

    private uint[] ToUtf32(string text) {
        byte[] utf32Bytes = new UTF32Encoding(!BitConverter.IsLittleEndian, false).GetBytes(text);
        uint[] codepoints = new uint[utf32Bytes.Length / 4];
        Buffer.BlockCopy(utf32Bytes, 0, codepoints, 0, utf32Bytes.Length);
        return codepoints;
    }

    internal record GlyphInfo(int page, Rect paddedRect);

#region Dispose
    private bool _disposed = false;

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;

        /* if (disposing) {} */

        SDL3_ttf.TTF_CloseFont(font);

        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Font() => Dispose(false);
#endregion Dispose
}