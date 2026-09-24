namespace Flora;

public class Core {
    public Application App { get; internal set; } = null!;
    public Graphics Gfx { get; internal set; } = null!;
    public Input Input { get; internal set; } = null!;
    public Audio Audio { get; internal set; } = null!;

    // basics
    public virtual void Prepare() {}
    public virtual void Pause() {}
    public virtual void Resume() {}
    public virtual void Resize(int width, int height) {}
    public virtual void Render(float delta) {}
    public virtual void Cleanup() {}

    // interaction events
    public virtual void OnKeyDown(Keycode key, Scancode scan) {}
    public virtual void OnKeyUp(Keycode key, Scancode scan) {}
    public virtual void OnTextInput(string text) {}
    public virtual void OnPointerDown(PointerType type, int index, int button, float x, float y) {}
    public virtual void OnPointerUp(PointerType type, int index, int button, float x, float y) {}
    public virtual void OnPointerMove(PointerType type, int index, float x, float y, float dx, float dy) {}
    public virtual void OnPointerWheel(PointerType type, int index, float x, float y, float dx, float dy) {}
    public virtual void OnGamepadAxis(uint which, GamepadAxis axis, short value) {}
    public virtual void OnGamepadDown(uint which, GamepadButton button) {}
    public virtual void OnGamepadUp(uint which, GamepadButton button) {}
    public virtual void OnGamepadAdded(uint which) {}
    public virtual void OnGamepadRemoved(uint which) {}
}
