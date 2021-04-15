namespace Flora.Input {
    /// <summary>
    /// Interface for handling user input.
    /// </summary>
    public interface InputHandler {
        void OnKeyDown(KeyCode keycode);
        void OnKeyUp(KeyCode keycode);
        void OnMouseDown(MouseButton button, int x, int y);
        void OnMouseUp(MouseButton button, int x, int y);
        void OnMouseMove(int x, int y);
    }
}