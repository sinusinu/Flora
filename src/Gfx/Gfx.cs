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

        /// <summary>
        /// Flip options for drawing textures.
        /// </summary>
        public enum FlipMode {
            None = SDL.SDL_RendererFlip.SDL_FLIP_NONE,
            Horizontal = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL,
            Vertical = SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL
        }
        
        internal static void Init(IntPtr w, IntPtr r) {
            sdlWindow = w;
            sdlRenderer = r;
            
            SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "linear");

            isGfxInitialized = true;
        }

        /// <summary>
        /// Set the current color. All textures drawn after this function call will be tinted with this color.
        /// </summary>
        /// <param name="r">Red (0-255)</param>
        /// <param name="g">Green (0-255)</param>
        /// <param name="b">Blue (0-255)</param>
        /// <param name="a">Alpha (0-255)</param>
        public static void SetColor(byte r, byte g, byte b, byte a) {
            currentColor.r = r;
            currentColor.g = g;
            currentColor.b = b;
            currentColor.a = a;
        }

        /// <summary>
        /// Clear screen with current color and get ready for drawing.
        /// </summary>
        public static void Begin() {
            Begin(currentColor);
        }
        
        /// <summary>
        /// Clear screen with given color and get ready for drawing.
        /// </summary>
        /// <param name="clearColor"></param>
        public static void Begin(Color clearColor) {
            isDrawing = true;

            SDL.SDL_Color prevColor = GetCurrentRenderColor();

            SetCurrentRenderColor(clearColor.ToSDLColor());

            SDL.SDL_RenderClear(sdlRenderer);

            SetCurrentRenderColor(prevColor);
        }
        
        /// <summary>
        /// Present drawn picture into display and finishes drawing.
        /// </summary>
        public static void End() {
            SDL.SDL_RenderPresent(sdlRenderer);

            isDrawing = false;
        }

        /// <summary>
        /// Draw texture on position (x, y).
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public static void Draw(Texture texture, int x, int y) {
            Draw(texture, x, y, texture.width, texture.height);
        }

        /// <summary>
        /// Draw texture on position (x, y) with size of (width, height).
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        public static void Draw(Texture texture, int x, int y, int width, int height) {
            Draw(texture, x, y, width, height, 0d, texture.width / 2, texture.height / 2);
        }

        /// <summary>
        /// Draw texture on position (x, y) with size of (width, height) rotated (rotation) radians with pivot of (pivotX, pivotY).
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in radians</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        public static void Draw(Texture texture, int x, int y, int width, int height, double rotation, int pivotX, int pivotY) {
            Draw(texture, x, y, width, height, rotation, pivotX, pivotY, FlipMode.None);
        }
        
        /// <summary>
        /// Draw texture on position (x, y) with size of (width, height) rotated (rotation) radians with pivot of (pivotX, pivotY) with flip option of (flip).
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in radians</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        /// <param name="flip">Flip option (use | to combine options)</param>
        public static void Draw(Texture texture, int x, int y, int width, int height, double rotation, int pivotX, int pivotY, FlipMode flip) {
            SDL.SDL_Rect drect;
            drect.x = x; drect.y = y; drect.w = width; drect.h = height;
            SDL.SDL_Point center;
            center.x = pivotX; center.y = pivotY;
            
            SetCurrentTextureColor(texture.sdlTexture, currentColor.ToSDLColor());
            SDL.SDL_RenderCopyEx(Gfx.sdlRenderer, texture.sdlTexture, IntPtr.Zero, ref drect, rotation, ref center, (SDL.SDL_RendererFlip)flip);
        }

        /// <summary>
        /// Draw texture region on position (x, y).
        /// </summary>
        /// <param name="region">TextureRegion to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public static void Draw(TextureRegion region, int x, int y) {
            Draw(region, x, y, region.rect.w, region.rect.h);
        }

        /// <summary>
        /// Draw texture region on position (x, y) with size of (width, height).
        /// </summary>
        /// <param name="region">TextureRegion to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        public static void Draw(TextureRegion region, int x, int y, int width, int height) {
            Draw(region, x, y, width, height, 0d, region.rect.w / 2, region.rect.h / 2);
        }

        /// <summary>
        /// Draw texture region on position (x, y) with size of (width, height) rotated (rotation) radians with pivot of (pivotX, pivotY).
        /// </summary>
        /// <param name="region">TextureRegion to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in radians</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        public static void Draw(TextureRegion region, int x, int y, int width, int height, double rotation, int pivotX, int pivotY) {
            Draw(region, x, y, width, height, rotation, pivotX, pivotY, FlipMode.None);
        }

        /// <summary>
        /// Draw texture region on position (x, y) with size of (width, height) rotated (rotation) radians with pivot of (pivotX, pivotY) with flip option of (flip).
        /// </summary>
        /// <param name="region">TextureRegion to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in radians</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        /// <param name="flip">Flip option (use | to combine options)</param>
        public static void Draw(TextureRegion region, int x, int y, int width, int height, double rotation, int pivotX, int pivotY, FlipMode flip) {
            SDL.SDL_Rect srect = region.rect.ToSDLRect();
            SDL.SDL_Rect drect;
            drect.x = x; drect.y = y; drect.w = width; drect.h = height;
            SDL.SDL_Point center;
            center.x = pivotX; center.y = pivotY;
            
            SDL.SDL_RenderCopyEx(Gfx.sdlRenderer, region.texture.sdlTexture, ref srect, ref drect, rotation, ref center, (SDL.SDL_RendererFlip)flip);
        }

        /// <summary>
        /// Draw primitive line.
        /// </summary>
        /// <param name="x1">X position of starting point</param>
        /// <param name="y1">Y position of starting point</param>
        /// <param name="x2">X position of ending point</param>
        /// <param name="y2">Y position of ending point</param>
        public static void DrawLine(int x1, int y1, int x2, int y2) {
            SDL.SDL_RenderDrawLine(Gfx.sdlRenderer, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw primitive rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="fill">Should rectangle be filled or not</param>
        public static void DrawRect(Util.Rect rect, bool fill) {
            SDL.SDL_Rect sr = rect.ToSDLRect();
            if (fill) SDL.SDL_RenderFillRect(Gfx.sdlRenderer, ref sr);
            else SDL.SDL_RenderDrawRect(Gfx.sdlRenderer, ref sr);
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