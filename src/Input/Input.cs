namespace Flora.Input {
    public static class Input {
        internal static InputHandler handler = null;

        static void SetInputHandler(InputHandler handler) {
            Input.handler = handler;
        }
    }
}