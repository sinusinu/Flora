using System;
using SDL2;
using Flora.Util;

namespace Flora.Gfx {
    /// <summary>
    /// 2D image that can be drawn on screen.
    /// </summary>
    public class Texture {
        internal IntPtr sdlTexture { get; private set; }
        public int width;
        public int height;

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

            uint d1; int d2;    // dummies
            SDL.SDL_QueryTexture(sdlTexture, out d1, out d2, out width, out height);
        }

        ~Texture() {
            SDL.SDL_DestroyTexture(sdlTexture);
        }
    }
}