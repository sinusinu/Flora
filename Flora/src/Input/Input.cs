using SDL;

namespace Flora;

public unsafe sealed class Input {
    private Application app;

    public enum ActiveInputOpts { None, Keyboard, Gamepad }
    public ActiveInputOpts _activeInput = ActiveInputOpts.None;
    public ActiveInputOpts ActiveInput {
        get => _activeInput;
        internal set {
            if (_activeInput != value) {
                _activeInput = value;
                ActiveInputChanged?.Invoke(value);
            }
        }
    }

    public Action<ActiveInputOpts>? ActiveInputChanged = null;

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