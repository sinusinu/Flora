using System;
using SDL2;
using Flora.Util;

namespace Flora.Gfx;

/// <summary>
/// 2D image that can be drawn on screen.
/// </summary>
public class Texture : IDisposable {
    internal IntPtr sdlTexture { get; private set; }
    public int width { get; private set; }
    public int height  { get; private set; }

    public Texture(string path) {
        if (!Gfx.isGfxInitialized) throw new InvalidOperationException("Gfx is not initialized");

        var imgSurface = SDL_image.IMG_Load(path);
        if (imgSurface == IntPtr.Zero) {
            string error = SDL_image.IMG_GetError();
            throw new ArgumentException("Failed to load image " + path + ": " + error);
        }
        sdlTexture = SDL.SDL_CreateTextureFromSurface(Gfx.sdlRenderer, imgSurface);
        if (sdlTexture == IntPtr.Zero) {
            string error = SDL.SDL_GetError();
            throw new InvalidOperationException("Failed to create texture from loaded surface: " + error);
        }
        SDL.SDL_FreeSurface(imgSurface);

        SDL.SDL_SetTextureBlendMode(sdlTexture, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

        uint d1; int d2; int w; int h;
        SDL.SDL_QueryTexture(sdlTexture, out d1, out d2, out w, out h);
        width = w; height = h;
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;
        
        /* if (disposing) {} */
        
        SDL.SDL_DestroyTexture(sdlTexture);

        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Texture() => Dispose(false);
}