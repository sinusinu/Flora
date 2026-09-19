using SDL;

namespace Flora;

public unsafe sealed class Input {
    private Application app;

    internal Input(Application application) {
        app = application;
    }

    public void EnterTextInputMode() {
        SDL3.SDL_StartTextInput(app.Window.sdlWindow);
    }

    public void ExitTextInputMode() {
        SDL3.SDL_StopTextInput(app.Window.sdlWindow);
    }
}