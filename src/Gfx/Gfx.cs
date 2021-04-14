using System;
using SDL2;

namespace Flora.Gfx {
    public static class Gfx {
        // sdl stuff
        public static bool isGfxInitialized { get; private set; }
        public static IntPtr sdlWindow { get; private set; }
        public static IntPtr sdlRenderer { get; private set; }

        // drawing stuff
        public static bool isDrawing = false;
        public static Color currentColor = new Color(0xFF, 0xFF, 0xFF, 0xFF);

        public enum FlipMode {
            None = SDL.SDL_RendererFlip.SDL_FLIP_NONE,
            Horizontal = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL,
            Vertical = SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL
        }
        
        public static void Init(IntPtr w, IntPtr r) {
            sdlWindow = w;
            sdlRenderer = r;
            
            SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "linear");

            isGfxInitialized = true;
        }

        public static void SetColor(byte r, byte g, byte b, byte a) {
            currentColor.r = r;
            currentColor.g = g;
            currentColor.b = b;
            currentColor.a = a;
        }

        public static void Begin() {
            Begin(currentColor);
        }

        public static void Begin(Color clearColor) {
            isDrawing = true;

            SDL.SDL_Color prevColor = GetCurrentRenderColor();

            SetCurrentRenderColor(clearColor.ToSDLColor());

            SDL.SDL_RenderClear(sdlRenderer);

            SetCurrentRenderColor(prevColor);
        }
        
        public static void End() {
            SDL.SDL_RenderPresent(sdlRenderer);

            isDrawing = false;
        }

        public static void Draw(Texture texture, int x, int y) {
            Draw(texture, x, y, texture.width, texture.height);
        }

        public static void Draw(Texture texture, int x, int y, int width, int height) {
            Draw(texture, x, y, width, height, 0d, texture.width / 2, texture.height / 2);
        }

        public static void Draw(Texture texture, int x, int y, int width, int height, double rotation, int pivotX, int pivotY) {
            Draw(texture, x, y, width, height, rotation, pivotX, pivotY, FlipMode.None);
        }
        
        public static void Draw(Texture texture, int x, int y, int width, int height, double rotation, int pivotX, int pivotY, FlipMode flip) {
            SDL.SDL_Rect drect;
            drect.x = x; drect.y = y; drect.w = width; drect.h = height;
            SDL.SDL_Point center;
            center.x = pivotX; center.y = pivotY;
            
            SetCurrentTextureColor(texture.sdlTexture, currentColor.ToSDLColor());
            SDL.SDL_RenderCopyEx(Gfx.sdlRenderer, texture.sdlTexture, IntPtr.Zero, ref drect, rotation, ref center, (SDL.SDL_RendererFlip)flip);
        }

        public static void Draw(TextureRegion region, int x, int y) {
            Draw(region, x, y, region.rect.w, region.rect.h);
        }

        public static void Draw(TextureRegion region, int x, int y, int width, int height) {
            Draw(region, x, y, width, height, 0d, region.rect.w / 2, region.rect.h / 2);
        }

        public static void Draw(TextureRegion region, int x, int y, int width, int height, double rotation, int pivotX, int pivotY) {
            Draw(region, x, y, width, height, rotation, pivotX, pivotY, FlipMode.None);
        }

        public static void Draw(TextureRegion region, int x, int y, int width, int height, double rotation, int pivotX, int pivotY, FlipMode flip) {
            SDL.SDL_Rect srect = region.rect.ToSDLRect();
            SDL.SDL_Rect drect;
            drect.x = x; drect.y = y; drect.w = width; drect.h = height;
            SDL.SDL_Point center;
            center.x = pivotX; center.y = pivotY;
            
            SDL.SDL_RenderCopyEx(Gfx.sdlRenderer, region.texture.sdlTexture, ref srect, ref drect, rotation, ref center, (SDL.SDL_RendererFlip)flip);
        }

#region Private functions
        private static SDL.SDL_Color GetCurrentRenderColor() {
            SDL.SDL_Color currentRenderColor = new SDL.SDL_Color();
            SDL.SDL_GetRenderDrawColor(sdlRenderer, out currentRenderColor.r, out currentRenderColor.g, out currentRenderColor.b, out currentRenderColor.a);
            return currentRenderColor;
        }

        private static void SetCurrentRenderColor(SDL.SDL_Color color) {
            SDL.SDL_SetRenderDrawColor(sdlRenderer, color.r, color.g, color.b, color.a);
        }
        
        private static SDL.SDL_Color GetCurrentTextureColor(IntPtr texture) {
            SDL.SDL_Color currentTextureColor = new SDL.SDL_Color();
            SDL.SDL_GetTextureColorMod(texture, out currentTextureColor.r, out currentTextureColor.g, out currentTextureColor.b);
            SDL.SDL_GetTextureAlphaMod(texture, out currentTextureColor.a);
            return currentTextureColor;
        }

        private static void SetCurrentTextureColor(IntPtr texture, SDL.SDL_Color color) {
            SDL.SDL_SetTextureColorMod(texture, color.r, color.g, color.b);
            SDL.SDL_SetTextureAlphaMod(texture, color.a);
        }
#endregion
    }
}