namespace Flora.Input {
    public static class Input {
        public static InputHandler handler = null;

        static void SetInputHandler(InputHandler handler) {
            Input.handler = handler;
        }
    }
}