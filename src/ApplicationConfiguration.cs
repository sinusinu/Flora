namespace Flora {
    public class ApplicationConfiguration {
        /// <summary>
        /// Flags for window.
        /// </summary>
        public enum FloraWindowFlags : uint {
            /// <summary>
            /// Show window on create.
            /// </summary>
            Shown = SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN,
            /// <summary>
            /// Do not show window on create.
            /// </summary>
            Hidden = SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN,
            /// <summary>
            /// Create window as maximized.
            /// </summary>
            Maximized = SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_MAXIMIZED,
            /// <summary>
            /// Create window as minimized.
            /// </summary>
            Minimized = SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_MINIMIZED,
            /// <summary>
            /// Make window resizable.
            /// </summary>
            Resizable = SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE,
            /// <summary>
            /// Make window borderless.
            /// </summary>
            Borderless = SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS
        }

        /// <summary>
        /// Flags for renderer.
        /// </summary>
        public enum FloraRenderFlags : uint {
            /// <summary>
            /// Use software renderer. Only use when the hardware accelerated renderer is not available.
            /// </summary>
            Software = SDL2.SDL.SDL_RendererFlags.SDL_RENDERER_SOFTWARE,
            /// <summary>
            /// Use hardware accelerated renderer. Recommended.
            /// </summary>
            Accelerated = SDL2.SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED,
            /// <summary>
            /// Render in sync with display's vertical sync. Recommended.
            /// </summary>
            Vsync = SDL2.SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
        }

        /// <summary>
        /// Initial width of the window.
        /// </summary>
        public int width;

        /// <summary>
        /// Initial height of the window.
        /// </summary>
        public int height;

        /// <summary>
        /// Title of the window.
        /// </summary>
        public string windowTitle;

        /// <summary>
        /// Flags for window.
        /// </summary>
        public FloraWindowFlags windowFlags;

        /// <summary>
        /// Flags for renderer.
        /// </summary>
        public FloraRenderFlags renderFlags;

        /// <summary>
        /// Target FPS.
        /// </summary>
        public int targetFps;

        public ApplicationConfiguration() {
            width = 640;
            height = 480;
            windowTitle = "Flora";
            windowFlags = FloraWindowFlags.Shown;
            renderFlags = FloraRenderFlags.Accelerated | FloraRenderFlags.Vsync;
            targetFps = 60;
        }
    }
}
