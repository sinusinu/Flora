namespace Flora;

/// <summary>
/// Core with screens.
/// </summary>
public class ScreenCore : Core {
    private Screen? pendingScreen = null;
    private Screen? activeScreen = null;

    /// <summary>
    /// Schedule the change of active screen. Note that the screen change will not happen immediately.<br/>
    /// <br/>
    /// Do not call <c>Screen.Prepare</c> or <c>Screen.Cleanup</c> when changing screen! <c>ScreenCore</c> will call <c>Screen.Prepare</c> and <c>Screen.Cleanup</c> automatically.
    /// </summary>
    public void SetScreen(Screen newScreen) {
        pendingScreen = newScreen;
    }

    public override void Prepare() {}
    public override void Pause() { activeScreen?.Pause(); }
    public override void Resume() { activeScreen?.Resume(); }
    public override void Resize(int width, int height) { activeScreen?.Resize(width, height); }
    public override void Render(float delta) {
        activeScreen?.Render(delta);
        // if pending screen exists, set them active
        if (pendingScreen is not null) {
            activeScreen?.Cleanup();
            pendingScreen.Prepare();
            activeScreen = pendingScreen;
            pendingScreen = null;
        }
    }
    public override void Cleanup() { activeScreen?.Cleanup(); }
    public override void KeyDown(Keycode key, Scancode scan) { activeScreen?.KeyDown(key, scan); }
    public override void KeyUp(Keycode key, Scancode scan) { activeScreen?.KeyUp(key, scan); }
    public override void TextInput(string text) { activeScreen?.TextInput(text); }
    public override void PointerDown(PointerType type, int button, float x, float y) { activeScreen?.PointerDown(type, button, x, y); }
    public override void PointerUp(PointerType type, int button, float x, float y) { activeScreen?.PointerUp(type, button, x, y); }
    public override void PointerMove(PointerType type, float x, float y, float dx, float dy) { activeScreen?.PointerMove(type, x, y, dx, dy); }
    public override void PointerWheel(PointerType type, float x, float y, float dx, float dy) { activeScreen?.PointerWheel(type, x, y, dx, dy); }
    public override void GamepadAxis(uint which, GamepadAxis axis, short value) { activeScreen?.GamepadAxis(which, axis, value); }
    public override void GamepadDown(uint which, GamepadButton button) { activeScreen?.GamepadDown(which, button); }
    public override void GamepadUp(uint which, GamepadButton button) { activeScreen?.GamepadUp(which, button); }
    public override void GamepadAdded(uint which) { activeScreen?.GamepadAdded(which); }
    public override void GamepadRemoved(uint which) { activeScreen?.GamepadRemoved(which); }
}