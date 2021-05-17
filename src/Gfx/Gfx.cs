using System;
using System.Runtime.InteropServices;
using Flora.Util;
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
        internal static float activeViewCenterX = 0;
        internal static float activeViewCenterY = 0;
        internal static float activeViewOffsetX = 0;
        internal static float activeViewOffsetY = 0;
        internal static int activeViewWidth = 0;
        internal static int activeViewHeight = 0;
        internal static float activeViewZoom = 1f;
        internal static float activeViewRotation = 0f;

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
        /// <param name="r">Red (0-1)</param>
        /// <param name="g">Green (0-1)</param>
        /// <param name="b">Blue (0-1)</param>
        /// <param name="a">Alpha (0-1)</param>
        public static void SetColor(float r, float g, float b, float a) {
            r = Math.Clamp(r, 0f, 1f);
            g = Math.Clamp(g, 0f, 1f);
            b = Math.Clamp(b, 0f, 1f);
            a = Math.Clamp(a, 0f, 1f);
            SetColor((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
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
            return new Color(currentColor);
        }

        /// <summary>
        /// Return the size of the window.
        /// </summary>
        public static (int, int) GetClientSize() {
            int w, h;
            SDL.SDL_GetWindowSize(sdlWindow, out w, out h);
            return (w, h);
        }

        /// <summary>
        /// Set the size of the window. Does nothing when on fullscreen.
        /// </summary>
        /// <param name="width">New width of the window</param>
        /// <param name="height">New height of the window</param>
        public static void SetClientSize(int width, int height) {
            SDL.SDL_SetWindowSize(sdlWindow, width, height);
        }

        /// <summary>
        /// Set new window mode.
        /// </summary>
        /// <param name="mode">New window mode</param>
        public static void SetWindowMode(WindowMode mode) {
            SDL.SDL_SetWindowFullscreen(sdlWindow, (uint)mode);
            UpdateView();
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
        public static void Begin(byte r, byte g, byte b) {
            Begin(new Color(r, g, b, (byte)255));
        }

        /// <summary>
        /// Clear screen with current color and get ready for drawing.
        /// </summary>
        /// <param name="r">Color to clear screen: Red (0-1)</param>
        /// <param name="g">Color to clear screen: Green (0-1)</param>
        /// <param name="b">Color to clear screen: Blue (0-1)</param>
        public static void Begin(float r, float g, float b) {
            Begin(new Color((byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f), (byte)255));
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
        public static void Draw(Texture texture, float x, float y) {
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
        public static void Draw(Texture texture, float x, float y, float width, float height) {
            Draw(texture, x, y, width, height, 0d);
        }

        /// <summary>
        /// Draw texture on position (x, y) with size of (width, height) rotated (rotation) degrees with central pivot.
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in degrees</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        public static void Draw(Texture texture, float x, float y, float width, float height, double rotation) {
            Draw(texture, x, y, width, height, rotation, width / 2f, height / 2f);
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
        public static void Draw(Texture texture, float x, float y, float width, float height, double rotation, float pivotX, float pivotY) {
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
        public static void Draw(Texture texture, float x, float y, float width, float height, double rotation, float pivotX, float pivotY, FlipMode flip) {
            if (!isDrawing) throw new InvalidOperationException("Draw must be called between Gfx.Begin and Gfx.End");

            SDL.SDL_Rect srect;
            srect.x = 0;
            srect.y = 0;
            srect.w = texture.width;
            srect.h = texture.height;

            float rx = x - activeViewOffsetX + pivotX;
            float ry = y - activeViewOffsetY + pivotY;
            float d = MathF.Sqrt((rx * rx) + (ry * ry));
            float r = MathF.Atan2(ry, rx);  // in radians
            float xp = d * MathF.Cos(r + MathUtils.DegToRad(activeViewRotation));
            float yp = d * MathF.Sin(r + MathUtils.DegToRad(activeViewRotation));
            
            SDL.SDL_FRect drect;
            drect.x = (xp - pivotX) * activeViewZoom + activeViewCenterX;
            drect.y = (yp - pivotY) * activeViewZoom + activeViewCenterY;
            drect.w = width * activeViewZoom;
            drect.h = height * activeViewZoom;
            
            SDL.SDL_FPoint center;
            center.x = pivotX * activeViewZoom;
            center.y = pivotY * activeViewZoom;
            
            // workaround for SDL.SDL_RenderCopyExF missing overload of (IntPtr, IntPtr, ref SDL_Rect, ref SDL_FRect, double, ref SDL_FPoint, SDL_RendererFlip)
            // hope this doesn't make too much performance hit...
            GCHandle centerHandle = GCHandle.Alloc(center);
            Marshal.StructureToPtr<SDL.SDL_FPoint>(center, (IntPtr)centerHandle, false);
            
            SetCurrentTextureColor(texture.sdlTexture, currentColor.ToSDLColor());
            SDL.SDL_RenderCopyExF(Gfx.sdlRenderer, texture.sdlTexture, ref srect, ref drect, rotation + activeViewRotation, (IntPtr)centerHandle, (SDL.SDL_RendererFlip)flip);

            centerHandle.Free();
        }

        /// <summary>
        /// Draw texture region on position (x, y).
        /// </summary>
        /// <param name="region">TextureRegion to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public static void Draw(TextureRegion region, float x, float y) {
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
        public static void Draw(TextureRegion region, float x, float y, float width, float height) {
            Draw(region, x, y, width, height, 0d);
        }

        /// <summary>
        /// Draw texture region on position (x, y) with size of (width, height) rotated (rotation) degrees with central pivot.
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="rotation">Rotation angle in degrees</param>
        /// <param name="pivotX">X Pivot</param>
        /// <param name="pivotY">Y Pivot</param>
        public static void Draw(TextureRegion region, float x, float y, float width, float height, double rotation) {
            Draw(region, x, y, width, height, rotation, width / 2f, height / 2f);
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
        public static void Draw(TextureRegion region, float x, float y, float width, float height, double rotation, float pivotX, float pivotY) {
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
        public static void Draw(TextureRegion region, float x, float y, float width, float height, double rotation, float pivotX, float pivotY, FlipMode flip) {
            if (!isDrawing) throw new InvalidOperationException("Draw must be called between Gfx.Begin and Gfx.End");

            SDL.SDL_Rect srect = region.rect.ToSDLRect();

            float rx = x - activeViewOffsetX + pivotX;
            float ry = y - activeViewOffsetY + pivotY;
            float d = MathF.Sqrt((rx * rx) + (ry * ry));
            float r = MathF.Atan2(ry, rx);  // in radians
            float xp = d * MathF.Cos(r + MathUtils.DegToRad(activeViewRotation));
            float yp = d * MathF.Sin(r + MathUtils.DegToRad(activeViewRotation));
            
            SDL.SDL_FRect drect;
            drect.x = (xp - pivotX) * activeViewZoom + activeViewCenterX;
            drect.y = (yp - pivotY) * activeViewZoom + activeViewCenterY;
            drect.w = width * activeViewZoom;
            drect.h = height * activeViewZoom;

            SDL.SDL_FPoint center;
            center.x = pivotX * activeViewZoom;
            center.y = pivotY * activeViewZoom;
            
            // workaround for SDL.SDL_RenderCopyExF missing overload of (IntPtr, IntPtr, ref SDL_Rect, ref SDL_FRect, double, ref SDL_FPoint, SDL_RendererFlip)
            // hope this doesn't make too much performance hit...
            GCHandle centerHandle = GCHandle.Alloc(center);
            Marshal.StructureToPtr<SDL.SDL_FPoint>(center, (IntPtr)centerHandle, false);
            
            SetCurrentTextureColor(region.texture.sdlTexture, currentColor.ToSDLColor());
            SDL.SDL_RenderCopyExF(Gfx.sdlRenderer, region.texture.sdlTexture, ref srect, ref drect, rotation + activeViewRotation, GCHandle.ToIntPtr(centerHandle), (SDL.SDL_RendererFlip)flip);

            centerHandle.Free();
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

            float rx1 = x1 - activeViewOffsetX;
            float ry1 = y1 - activeViewOffsetY;
            float rx2 = x2 - activeViewOffsetX;
            float ry2 = y2 - activeViewOffsetY;
            float d1 = MathF.Sqrt((rx1 * rx1) + (ry1 * ry1));
            float d2 = MathF.Sqrt((rx2 * rx2) + (ry2 * ry2));
            float r1 = MathF.Atan2(ry1, rx1);  // in radians
            float r2 = MathF.Atan2(ry2, rx2);  // in radians
            float x1p = d1 * MathF.Cos(r1 + MathUtils.DegToRad(activeViewRotation));
            float y1p = d1 * MathF.Sin(r1 + MathUtils.DegToRad(activeViewRotation));
            float x2p = d2 * MathF.Cos(r2 + MathUtils.DegToRad(activeViewRotation));
            float y2p = d2 * MathF.Sin(r2 + MathUtils.DegToRad(activeViewRotation));

            SDL.SDL_RenderDrawLineF(
                Gfx.sdlRenderer,
                ((x1p) * activeViewZoom) + activeViewCenterX,
                ((y1p) * activeViewZoom) + activeViewCenterY,
                ((x2p) * activeViewZoom) + activeViewCenterX,
                ((y2p) * activeViewZoom) + activeViewCenterY
            );
        }

        /// <summary>
        /// Translate screen point (relative to window) to view point (relative to view).
        /// </summary>
        /// <param name="x">X of screen point</param>
        /// <param name="y">Y of screen point</param>
        /// <returns>(x, y) of view point</returns>
        public static (int, int) TranslateScreenPointToViewPoint(int x, int y) {
            if (activeViewZoom == 0f) return (0, 0);
            if (activeView == null) return (x, y);

            float xp = (x - activeViewCenterX) / activeViewZoom;
            float yp = (y - activeViewCenterY) / activeViewZoom;

            float d = MathF.Sqrt((xp * xp) + (yp * yp));
            float r = MathF.Atan2(yp, xp);  // in radians
            float ox = d * MathF.Cos(r - MathUtils.DegToRad(activeViewRotation)) + activeViewOffsetX;
            float oy = d * MathF.Sin(r - MathUtils.DegToRad(activeViewRotation)) + activeViewOffsetY;

            return ((int)ox, (int)oy);
        }

#region Internal functions
        internal static void UpdateView() {
            if (activeView == null) {
                var (clientWidth, clientHeight) = GetClientSize();
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
                activeViewCenterX = activeView.centerX;
                activeViewCenterY = activeView.centerY;
                activeViewOffsetX = activeView.offsetX;
                activeViewOffsetY = activeView.offsetY;
                activeViewWidth = activeView.ratioCorrectedWidth;
                activeViewHeight = activeView.ratioCorrectedHeight;
                activeViewZoom = activeView._zoom;
                activeViewRotation = activeView._rotation;
            }
        }

        internal static void DrawGlyph(IntPtr texture, SDL.SDL_Rect srcRect, SDL.SDL_FRect dstRect, double rotation, int pivotX, int pivotY, FlipMode flip, Color color) {
            SetCurrentTextureColor(texture, color.ToSDLColor());

            float rx = dstRect.x - activeViewOffsetX + pivotX;
            float ry = dstRect.y - activeViewOffsetY + pivotY;
            float d = MathF.Sqrt((rx * rx) + (ry * ry));
            float r = MathF.Atan2(ry, rx);  // in radians
            float xp = d * MathF.Cos(r + MathUtils.DegToRad(activeViewRotation));
            float yp = d * MathF.Sin(r + MathUtils.DegToRad(activeViewRotation));
            
            SDL.SDL_FRect drect;
            drect.x = (xp - pivotX) * activeViewZoom + activeViewCenterX;
            drect.y = (yp - pivotY) * activeViewZoom + activeViewCenterY;
            drect.w = dstRect.w * activeViewZoom;
            drect.h = dstRect.h * activeViewZoom;
            SDL.SDL_FPoint center;
            center.x = pivotX * activeViewZoom;
            center.y = pivotY * activeViewZoom;
            
            // workaround for SDL.SDL_RenderCopyExF missing overload of (IntPtr, IntPtr, ref SDL_Rect, ref SDL_FRect, double, ref SDL_FPoint, SDL_RendererFlip)
            // hope this doesn't make too much performance hit...
            GCHandle centerHandle = GCHandle.Alloc(center);
            Marshal.StructureToPtr<SDL.SDL_FPoint>(center, (IntPtr)centerHandle, false);

            SDL.SDL_RenderCopyExF(Gfx.sdlRenderer, texture, ref srcRect, ref drect, rotation + activeViewRotation, GCHandle.ToIntPtr(centerHandle), (SDL.SDL_RendererFlip)flip);

            centerHandle.Free();
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