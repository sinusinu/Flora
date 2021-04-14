namespace Flora.Input {
    public static class Input {
        internal static InputHandler handler = null;

        public static void SetInputHandler(InputHandler handler) {
            Input.handler = handler;
        }
    }
}