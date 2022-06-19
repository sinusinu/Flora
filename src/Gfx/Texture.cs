using System;
using System.IO;
using SDL2;
using StbiSharp;
using Flora.Util;
using System.Runtime.InteropServices;

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

        StbiImage stbiImage = null;
        byte[] imageData;

        using (var stream = File.OpenRead(path))
        using (var ms = new MemoryStream()) {
            stream.CopyTo(ms);
            stbiImage = Stbi.LoadFromMemory(ms, 4);
            imageData = stbiImage.Data.ToArray();   // TODO: any way to use ReadOnlySpan<byte> as is? this is probably making an unnecessary copy of it
        }

        GCHandle imageDataHandle = GCHandle.Alloc(imageData, GCHandleType.Pinned);

        sdlTexture = SDL.SDL_CreateTexture(Gfx.sdlRenderer, SDL.SDL_PIXELFORMAT_ABGR8888, 0, stbiImage.Width, stbiImage.Height);
        SDL.SDL_UpdateTexture(sdlTexture, IntPtr.Zero, imageDataHandle.AddrOfPinnedObject(), stbiImage.Width * 4);

        imageDataHandle.Free();

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