using SDL;

namespace Flora;

public sealed unsafe class Window {
    public enum WindowModeOpts { Windowed, Fullscreen }

    private Application app;
    internal SDL_Window* sdlWindow;
    internal SDL_Renderer* sdlRenderer;

    private string _windowTitle = "A Flora Application";
    public string WindowTitle {
        get => _windowTitle;
        set {
            _windowTitle = value;
            SDL3.SDL_SetWindowTitle(sdlWindow, value);
        }
    }

    public int WindowWidth { get { int width = 0; SDL3.SDL_GetWindowSize(sdlWindow, &width, null); return width; } }
    public int WindowHeight { get { int height = 0; SDL3.SDL_GetWindowSize(sdlWindow, null, &height); return height; } }
    public (int, int) WindowSize { get { int width = 0; int height = 0; SDL3.SDL_GetWindowSize(sdlWindow, &width, &height); return (width, height); } }

    public WindowModeOpts WindowMode { get; internal set; }

    internal Window(Application app, SDL_Window* window, SDL_Renderer* renderer) {
        this.app = app;
        sdlWindow = window;
        sdlRenderer = renderer;
    }
    
    public void SetWindowed(int width, int height) {
        if (width <= 0 || height <= 0) throw new InvalidOperationException("Width and Height must be >0");

        WindowMode = WindowModeOpts.Windowed;
        
        SDL3.SDL_SetWindowFullscreen(sdlWindow, false);
        SDL3.SDL_SetWindowSize(sdlWindow, width, height);
        SDL3.SDL_SyncWindow(sdlWindow);
    }
    
    public void SetFullscreen() {
        WindowMode = WindowModeOpts.Fullscreen;
        
        SDL3.SDL_SetWindowFullscreen(sdlWindow, true);
        SDL3.SDL_SyncWindow(sdlWindow);
    }
}