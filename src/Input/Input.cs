using System;

namespace Flora.Input {
    public static class Input {
        internal static WeakReference handler = new WeakReference(null);
        internal static bool isInputInitialized = false;

        internal static void Init() {
            isInputInitialized = true;
        }

        /// <summary>
        /// Set InputHandler to receive user input events.<br/>
        /// Any previously set InputHandler will no longer receive user input events.
        /// </summary>
        /// <param name="handler"></param>
        public static void SetInputHandler(IInputHandler handler) {
            if (!isInputInitialized) throw new InvalidOperationException("Input is not initialized");
            Input.handler.Target = handler;
        }
    }
}