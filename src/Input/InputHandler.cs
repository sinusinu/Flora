namespace Flora.Input {
    public interface InputHandler {
        void OnKeyDown(int keycode);
        void OnKeyUp(int keycode);
        void OnMouseDown(int pointer, int x, int y);
        void OnMouseUp(int pointer, int x, int y);
        void OnMouseMove(int x, int y);
    }
}