namespace Flora;

interface IScreen {
    public void Prepare();
    public void Pause();
    public void Resume();
    public void Resize(int width, int height);
    public void Render(double delta);
    public void Cleanup();

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