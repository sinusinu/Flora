using System;

namespace Flora.Input {
    public static class Input {
        internal static InputHandler handler = null;
        internal static bool isInputInitialized = false;

        internal static void Init() {
            isInputInitialized = true;
        }

        public static void SetInputHandler(InputHandler handler) {
            if (!isInputInitialized) throw new InvalidOperationException("Input is not initialized");
            Input.handler = handler;
        }
    }
}