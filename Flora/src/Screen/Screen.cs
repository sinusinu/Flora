namespace Flora;

public class Screen {
    // same as core
    public virtual void Prepare() {}
    public virtual void Pause() {}
    public virtual void Resume() {}
    public virtual void Resize(int width, int height) {}
    public virtual void Render(float delta) {}
    public virtual void Cleanup() {}

    public virtual void KeyDown(Keycode key, Scancode scan) {}
    public virtual void KeyUp(Keycode key, Scancode scan) {}
    public virtual void TextInput(string text) {}
    public virtual void PointerDown(PointerType type, int button, float x, float y) {}
    public virtual void PointerUp(PointerType type, int button, float x, float y) {}
    public virtual void PointerMove(PointerType type, float x, float y, float dx, float dy) {}
    public virtual void PointerWheel(PointerType type, float x, float y, float dx, float dy) {}
    public virtual void GamepadAxis(uint which, GamepadAxis axis, short value) {}
    public virtual void GamepadDown(uint which, GamepadButton button) {}
    public virtual void GamepadUp(uint which, GamepadButton button) {}
    public virtual void GamepadAdded(uint which) {}
    public virtual void GamepadRemoved(uint which) {}
}