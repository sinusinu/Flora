namespace Flora.Input;

/// <summary>
/// Interface for handling controller input.
/// </summary>
public interface IControllerHandler {
    void OnAxisMotion(int which, ControllerAxis axis, float value);
    void OnButtonDown(int which, ControllerButton button);
    void OnButtonUp(int which, ControllerButton button);
    void OnControllerAdded(int which);
    void OnControllerRemoved(int which);
}