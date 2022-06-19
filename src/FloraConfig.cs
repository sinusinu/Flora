using System.Reflection;

namespace Flora;

public struct FloraConfig {
    /// <summary>
    /// Flags for window.
    /// </summary>
    public enum FloraWindowFlags : uint {
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
        /// Remove window decorations. Do not confuse with borderless fullscreen.
        /// </summary>
        Borderless = SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS
    }

    /// <summary>
    /// Flags for renderer.
    /// </summary>
    public enum FloraRenderFlags : uint {
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
    /// Create ApplicationConfiguration with default settings.
    /// </summary>
    public FloraConfig() {
        width = 640;
        height = 480;
        windowTitle = Assembly.GetCallingAssembly().GetName().Name;
        windowFlags = 0;
        renderFlags = FloraRenderFlags.Vsync;
    }
}