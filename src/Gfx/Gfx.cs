using System;
using SDL2;

namespace Flora.Gfx {
    public static class Gfx {
        // sdl stuff
        internal static bool isGfxInitialized { get; private set; }
        internal static IntPtr sdlWindow { get; private set; }
        internal static IntPtr sdlRenderer { get; private set; }

        // drawing stuff
        internal static bool isDrawing = false;
        internal static Color currentColor = new Color(0xFF, 0xFF, 0xFF, 0xFF);

        // view
        internal static View activeView = null;
        internal static int activeViewCenterX { get { return activeView == null ? 0 : (int)(activeView.centerX); } }
        internal static int activeViewCenterY { get { return activeView == null ? 0 : (int)(activeView.centerY); } }
        internal static int activeViewOffsetX { get { return activeView == null ? 0 : (int)(activeView.offsetX); } }
        internal static int activeViewOffsetY { get { return activeView == null ? 0 : (int)(activeView.offsetY); } }
        internal static int activeViewWidth { get { return activeView == null ? 0 : activeView.ratioCorrectedWidth; } }
        internal static int activeViewHeight { get { return activeView == null ? 0 : activeView.ratioCorrectedHeight; } }
        internal static float activeViewZoom { get { return activeView == null ? 1f : activeView.zoom; } }

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
            SetCurrentRenderColor(currentColor.ToSDLColor());

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

            SetCurrentRenderColor(currentColor.ToSDLColor());
        }

        /// <summary>
        /// Return the size of client region.
        /// </summary>
        public static (int, int) GetClientSize() {
            int w, h;
            SDL.SDL_GetWindowSize(sdlWindow, out w, out h);
            return (w, h);
        }

        /// <summary>
        /// Apply a view.
        /// </summary>
        /// <param name="view">View to apply</param>
        public static void SetView(View view) {
            activeView = view;

            UpdateView();
        }

        internal static void UpdateView() {
            var (clientWidth, clientHeight) = GetClientSize();
            if (activeView == null) {
                SDL.SDL_RenderSetLogicalSize(sdlRenderer, clientWidth, clientHeight);
            } else {
                activeView.CalculateAppliedSize();
                SDL.SDL_RenderSetLogicalSize(Gfx.sdlRenderer, activeView.ratioCorrectedWidth, activeView.ratioCorrectedHeight);
            }
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

            if (clearColor == currentColor) {
                SDL.SDL_RenderClear(sdlRenderer);
            } else {
                SDL.SDL_Color prevColor = GetCurrentRenderColor();
                SetCurrentRenderColor(clearColor.ToSDLColor());
                SDL.SDL_RenderClear(sdlRenderer);
                SetCurrentRenderColor(prevColor);
            }
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
            drect.x = (int)((x + activeViewCenterX - activeViewOffsetX) * activeViewZoom);
            drect.y = (int)((y + activeViewCenterY - activeViewOffsetY) * activeViewZoom);
            drect.w = (int)(width * activeViewZoom);
            drect.h = (int)(height * activeViewZoom);
            SDL.SDL_Point center;
            center.x = (int)(pivotX * activeViewZoom);
            center.y = (int)(pivotY * activeViewZoom);
            
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
            drect.x = (int)((x + activeViewCenterX - activeViewOffsetX) * activeViewZoom);
            drect.y = (int)((y + activeViewCenterY - activeViewOffsetY) * activeViewZoom);
            drect.w = (int)(width * activeViewZoom);
            drect.h = (int)(height * activeViewZoom);
            SDL.SDL_Point center;
            center.x = (int)(pivotX * activeViewZoom);
            center.y = (int)(pivotY * activeViewZoom);
            
            SDL.SDL_RenderCopyEx(Gfx.sdlRenderer, region.texture.sdlTexture, ref srect, ref drect, rotation, ref center, (SDL.SDL_RendererFlip)flip);
        }

        internal static void DrawGlyph(IntPtr texture, SDL.SDL_Rect srcRect, ref SDL.SDL_Rect dstRect, double rotation, int pivotX, int pivotY, FlipMode flip) {
            dstRect.x = (int)((dstRect.x + activeViewCenterX - activeViewOffsetX) * activeViewZoom);
            dstRect.y = (int)((dstRect.y + activeViewCenterY - activeViewOffsetY) * activeViewZoom);
            dstRect.w = (int)(dstRect.w * activeViewZoom);
            dstRect.h = (int)(dstRect.h * activeViewZoom);
            SDL.SDL_Point center;
            center.x = (int)(pivotX * activeViewZoom);
            center.y = (int)(pivotY * activeViewZoom);

            SDL.SDL_RenderCopyEx(Gfx.sdlRenderer, texture, ref srcRect, ref dstRect, rotation, ref center, (SDL.SDL_RendererFlip)flip);
        }

        /// <summary>
        /// Draw primitive line.
        /// </summary>
        /// <param name="x1">X position of starting point</param>
        /// <param name="y1">Y position of starting point</param>
        /// <param name="x2">X position of ending point</param>
        /// <param name="y2">Y position of ending point</param>
        public static void DrawLine(int x1, int y1, int x2, int y2) {
            SDL.SDL_RenderDrawLine(
                Gfx.sdlRenderer,
                (int)((x1 + activeViewCenterX - activeViewOffsetX) * activeViewZoom),
                (int)((y1 + activeViewCenterY - activeViewOffsetY) * activeViewZoom),
                (int)((x2 + activeViewCenterX - activeViewOffsetX) * activeViewZoom),
                (int)((y2 + activeViewCenterY - activeViewOffsetY) * activeViewZoom)
            );
        }

        /// <summary>
        /// Draw primitive rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="fill">Should rectangle be filled or not</param>
        public static void DrawRect(Util.Rect rect, bool fill) {
            SDL.SDL_Rect sr = rect.ToSDLRect();
            sr.x = (int)((sr.x + activeViewCenterX - activeViewOffsetX) * activeViewZoom);
            sr.y = (int)((sr.y + activeViewCenterY - activeViewOffsetY) * activeViewZoom);
            sr.w = (int)(sr.w * activeViewZoom);
            sr.h = (int)(sr.h * activeViewZoom);
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