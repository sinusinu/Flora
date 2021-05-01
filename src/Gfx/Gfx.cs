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
        internal static int activeViewCenterX;
        internal static int activeViewCenterY;
        internal static int activeViewOffsetX;
        internal static int activeViewOffsetY;
        internal static int activeViewWidth;
        internal static int activeViewHeight;
        internal static float activeViewZoom;
        internal static int activeViewRotation;

        /// <summary>
        /// Flip options for drawing textures.
        /// </summary>
        public enum FlipMode {
            None = SDL.SDL_RendererFlip.SDL_FLIP_NONE,
            Horizontal = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL,
            Vertical = SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL
        }

        /// <summary>
        /// Window mode.
        /// </summary>
        public enum WindowMode : uint {
            /// <summary>Windowed mode.</summary>
            Windowed = 0,
            /// <summary>Borderless Fullscreen mode (aka 'fake').</summary>
            Borderless = SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP,
            /// <summary>Exclusive Fullscreen mode.</summary>
            Exclusive = SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN
        }
        
        internal static void Init(IntPtr w, IntPtr r) {
            sdlWindow = w;
            sdlRenderer = r;

            SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "linear");
            SetCurrentRenderColor(currentColor.ToSDLColor());
            
            SDL.SDL_SetRenderDrawBlendMode(sdlRenderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            
            isGfxInitialized = true;
        }

        /// <summary>
        /// Set the current color. All textures drawn after this function call will be tinted with this color.
        /// </summary>
        /// <param name="color">New color</param>
        public static void SetColor(Color color) {
            SetColor(color.r, color.g, color.b, color.a);
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
        /// Get the current color.
        /// </summary>
        /// <returns></returns>
        public static Color GetColor() {
            return currentColor;
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

        /// <summary>
        /// Set window size. Does nothing when on fullscreen.
        /// </summary>
        /// <param name="width">New width of the window</param>
        /// <param name="height">New height of the window</param>
        public static void SetWindowSize(int width, int height) {
            SDL.SDL_SetWindowSize(sdlWindow, width, height);
        }

        /// <summary>
        /// Set new window mode.
        /// </summary>
        /// <param name="mode">New window mode</param>
        public static void SetWindowMode(WindowMode mode) {
            SDL.SDL_SetWindowFullscreen(sdlWindow, (uint)mode);
        }

        /// <summary>
        /// Clear screen with current color and get ready for drawing.
        /// </summary>
        public static void Begin() {
            Begin(currentColor);
        }

        /// <summary>
        /// Clear screen with current color and get ready for drawing.
        /// </summary>
        /// <param name="r">Color to clear screen: Red (0-255)</param>
        /// <param name="g">Color to clear screen: Green (0-255)</param>
        /// <param name="b">Color to clear screen: Blue (0-255)</param>
        /// <param name="a">Color to clear screen: Alpha (0-255)</param>
        public static void Begin(byte r, byte g, byte b, byte a) {
            Begin(new Color(r, g, b, a));
        }
        
        /// <summary>
        /// Clear screen with given color and get ready for drawing.
        /// </summary>
        /// <param name="clearColor">Color to clear screen</param>
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
        /// Draw texture on position (x, y) with size of (width, height) rotated (rotation) degrees with pivot of (pivotX, pivotY).
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in degrees</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        public static void Draw(Texture texture, int x, int y, int width, int height, double rotation, int pivotX, int pivotY) {
            Draw(texture, x, y, width, height, rotation, pivotX, pivotY, FlipMode.None);
        }
        
        /// <summary>
        /// Draw texture on position (x, y) with size of (width, height) rotated (rotation) degrees with pivot of (pivotX, pivotY) with flip option of (flip).
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in degrees</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        /// <param name="flip">Flip option (use | to combine options)</param>
        public static void Draw(Texture texture, int x, int y, int width, int height, double rotation, int pivotX, int pivotY, FlipMode flip) {
            if (!isDrawing) throw new InvalidOperationException("Draw must be called between Gfx.Begin and Gfx.End");
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
        /// Draw texture region on position (x, y) with size of (width, height) rotated (rotation) degrees with pivot of (pivotX, pivotY).
        /// </summary>
        /// <param name="region">TextureRegion to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in degrees</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        public static void Draw(TextureRegion region, int x, int y, int width, int height, double rotation, int pivotX, int pivotY) {
            Draw(region, x, y, width, height, rotation, pivotX, pivotY, FlipMode.None);
        }

        /// <summary>
        /// Draw texture region on position (x, y) with size of (width, height) rotated (rotation) degrees with pivot of (pivotX, pivotY) with flip option of (flip).
        /// </summary>
        /// <param name="region">TextureRegion to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in degrees</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        /// <param name="flip">Flip option (use | to combine options)</param>
        public static void Draw(TextureRegion region, int x, int y, int width, int height, double rotation, int pivotX, int pivotY, FlipMode flip) {
            if (!isDrawing) throw new InvalidOperationException("Draw must be called between Gfx.Begin and Gfx.End");
            SDL.SDL_Rect srect = region.rect.ToSDLRect();
            SDL.SDL_Rect drect;
            drect.x = (int)((x + activeViewCenterX - activeViewOffsetX) * activeViewZoom);
            drect.y = (int)((y + activeViewCenterY - activeViewOffsetY) * activeViewZoom);
            drect.w = (int)(width * activeViewZoom);
            drect.h = (int)(height * activeViewZoom);
            SDL.SDL_Point center;
            center.x = (int)(pivotX * activeViewZoom);
            center.y = (int)(pivotY * activeViewZoom);
            
            SetCurrentTextureColor(region.texture.sdlTexture, currentColor.ToSDLColor());
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
            if (!isDrawing) throw new InvalidOperationException("DrawLine must be called between Gfx.Begin and Gfx.End");
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
            if (!isDrawing) throw new InvalidOperationException("DrawRect must be called between Gfx.Begin and Gfx.End");
            SDL.SDL_Rect sr = rect.ToSDLRect();
            sr.x = (int)((sr.x + activeViewCenterX - activeViewOffsetX) * activeViewZoom);
            sr.y = (int)((sr.y + activeViewCenterY - activeViewOffsetY) * activeViewZoom);
            sr.w = (int)(sr.w * activeViewZoom);
            sr.h = (int)(sr.h * activeViewZoom);
            if (fill) SDL.SDL_RenderFillRect(Gfx.sdlRenderer, ref sr);
            else SDL.SDL_RenderDrawRect(Gfx.sdlRenderer, ref sr);
        }

#region Internal functions
        internal static void UpdateView() {
            var (clientWidth, clientHeight) = GetClientSize();
            if (activeView == null) {
                SDL.SDL_RenderSetLogicalSize(sdlRenderer, clientWidth, clientHeight);
            } else {
                activeView.CalculateAppliedSize();
                SDL.SDL_RenderSetLogicalSize(Gfx.sdlRenderer, activeView.ratioCorrectedWidth, activeView.ratioCorrectedHeight);
            }
            UpdateViewMetrics();
        }

        internal static void UpdateViewMetrics() {
            if (activeView == null) {
                activeViewCenterX = 0;
                activeViewCenterY = 0;
                activeViewOffsetX = 0;
                activeViewOffsetY = 0;
                activeViewWidth = 0;
                activeViewHeight = 0;
                activeViewZoom = 1f;
                activeViewRotation = 0;
            } else {
                activeViewCenterX = (int)(activeView.centerX);
                activeViewCenterY = (int)(activeView.centerY);
                activeViewOffsetX = (int)(activeView.offsetX);
                activeViewOffsetY = (int)(activeView.offsetY);
                activeViewWidth = activeView.ratioCorrectedWidth;
                activeViewHeight = activeView.ratioCorrectedHeight;
                activeViewZoom = activeView._zoom;
                activeViewRotation = activeView._rotation;
            }
        }

        internal static void DrawGlyph(IntPtr texture, SDL.SDL_Rect srcRect, SDL.SDL_Rect dstRect, double rotation, int pivotX, int pivotY, FlipMode flip, Color color) {
            SetCurrentTextureColor(texture, color.ToSDLColor());
            SDL.SDL_Rect drect;
            drect.x = (int)((dstRect.x + activeViewCenterX - activeViewOffsetX) * activeViewZoom);
            drect.y = (int)((dstRect.y + activeViewCenterY - activeViewOffsetY) * activeViewZoom);
            drect.w = (int)(dstRect.w * activeViewZoom);
            drect.h = (int)(dstRect.h * activeViewZoom);
            SDL.SDL_Point center;
            center.x = (int)(pivotX * activeViewZoom);
            center.y = (int)(pivotY * activeViewZoom);

            SDL.SDL_RenderCopyEx(Gfx.sdlRenderer, texture, ref srcRect, ref drect, rotation, ref center, (SDL.SDL_RendererFlip)flip);
        }
        
        internal static SDL.SDL_Color GetCurrentRenderColor() {
            SDL.SDL_Color currentRenderColor = new SDL.SDL_Color();
            SDL.SDL_GetRenderDrawColor(sdlRenderer, out currentRenderColor.r, out currentRenderColor.g, out currentRenderColor.b, out currentRenderColor.a);
            return currentRenderColor;
        }

        internal static void SetCurrentRenderColor(SDL.SDL_Color color) {
            SDL.SDL_SetRenderDrawColor(sdlRenderer, color.r, color.g, color.b, color.a);
        }
        
        internal static SDL.SDL_Color GetCurrentTextureColor(IntPtr texture) {
            SDL.SDL_Color currentTextureColor = new SDL.SDL_Color();
            SDL.SDL_GetTextureColorMod(texture, out currentTextureColor.r, out currentTextureColor.g, out currentTextureColor.b);
            SDL.SDL_GetTextureAlphaMod(texture, out currentTextureColor.a);
            return currentTextureColor;
        }

        internal static void SetCurrentTextureColor(IntPtr texture, SDL.SDL_Color color) {
            SDL.SDL_SetTextureColorMod(texture, color.r, color.g, color.b);
            SDL.SDL_SetTextureAlphaMod(texture, color.a);
        }
#endregion
    }
}