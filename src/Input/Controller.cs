using System;
using System.Collections.Generic;
using SDL2;

namespace Flora.Input {
    public static class Controller {
        internal static bool isControllerInitialized = false;

        internal static List<ControllerInstance> instances;
        internal static ControllerHandler handler = null;

        internal static void Init() {
            instances = new List<ControllerInstance>();
            RefreshControllers();

            isControllerInitialized = true;
        }

        internal static int GetControllerIndex(int instanceId) {
            for (int i = 0; i < instances.Count; i++) {
                if (instances[i].id == instanceId) return i;
            }
            return -1;
        }

        internal static void RefreshControllers() {
            foreach (var inst in instances) inst.Close();
            instances.Clear();
            
            int num = SDL.SDL_NumJoysticks();

            for (int i = 0; i < num; i++) {
                var ctrl = SDL.SDL_GameControllerOpen(i);
                if (ctrl != IntPtr.Zero) instances.Add(new ControllerInstance(ctrl));
            }
        }

        /// <summary>
        /// Set ControllerHandler to receive controller input events.<br/>
        /// Any previously set ControllerHandler will no longer receive controller input events.
        /// </summary>
        /// <param name="handler"></param>
        public static void SetControllerHandler(ControllerHandler handler) {
            if (!isControllerInitialized) throw new InvalidOperationException("Controller is not initialized");
            Controller.handler = handler;
        }

        internal static float ShortToFloat(short value) {
            if (value == 0) return 0f;
            else if (value < 0) return value / -32768f;
            else return value / 32767f;
        }
    }
}