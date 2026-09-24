using SDL;

namespace Flora;

public unsafe sealed class Input {
    private Application app;

    public enum ActiveInputOpts { None, Keyboard, Gamepad }
    private ActiveInputOpts _activeInput = ActiveInputOpts.None;
    /// <summary>
    /// Last active input, one of: Keyboard or Gamepad.<br/>
    /// Check this for e.g. showing appropriate input prompts.
    /// </summary>
    public ActiveInputOpts ActiveInput {
        get => _activeInput;
        internal set {
            if (_activeInput != value) {
                _activeInput = value;
                ActiveInputChanged?.Invoke(value);
            }
        }
    }

    /// <summary>
    /// Called when the last active input changes.
    /// </summary>
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