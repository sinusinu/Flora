namespace Flora.Input {
    /// <summary>
    /// Interface for handling controller input.
    /// </summary>
    public interface ControllerHandler {
        void OnAxisMotion(int which, ControllerAxis axis, float value);
        void OnButtonDown(int which, ControllerButton button);
        void OnButtonUp(int which, ControllerButton button);
    }
}