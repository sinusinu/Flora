using SDL;

namespace Flora;

public sealed unsafe class Window {
    private Application app;
    internal SDL_Window* sdlWindow;
    internal SDL_Renderer* sdlRenderer;

    internal Window(Application app, SDL_Window* window, SDL_Renderer* renderer) {
        this.app = app;
        sdlWindow = window;
        sdlRenderer = renderer;
    }
    
    public void SetWindowMode(Config.WindowModeOpts windowMode, int width, int height) {
        app.Config.WindowMode = windowMode;
        app.Config.WindowWidth = width;
        app.Config.WindowHeight = height;
        
        switch (windowMode) {
            case Config.WindowModeOpts.Windowed:
                SDL3.SDL_SetWindowFullscreen(sdlWindow, false);
                SDL3.SDL_SetWindowSize(sdlWindow, width, height);
                break;
            case Config.WindowModeOpts.Fullscreen:
                SDL3.SDL_SetWindowFullscreen(sdlWindow, true);
                break;
        }
    }
}