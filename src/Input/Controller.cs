using System;
using System.Collections.Generic;
using SDL2;

namespace Flora.Input {
    public static class Controller {
        internal static bool isControllerInitialized = false;

        internal static List<ControllerInstance> instances;
        internal static WeakReference handler = new WeakReference(null);

        private static int[] rncCtrlIds;

        internal static void Init() {
            instances = new List<ControllerInstance>();
            RefreshControllers();

            isControllerInitialized = true;
        }

        internal static void RefreshControllers() {
            foreach (var inst in instances) inst.Dispose();
            instances.Clear();
            
            int num = SDL.SDL_NumJoysticks();

            for (int i = 0; i < num; i++) {
                var ctrl = SDL.SDL_GameControllerOpen(i);
                if (ctrl != IntPtr.Zero) instances.Add(new ControllerInstance(ctrl));
            }
        }

        // workaround for SDL_CONTROLLERDEVICEADDED event data being inaccurate - do not call this otherwise!
        internal static void ReadyReportNewController() {
            rncCtrlIds = GetControllerIds();
        }

        // workaround for SDL_CONTROLLERDEVICEADDED event data being inaccurate - do not call this otherwise!
        internal static void ReportNewController() {
            var nrncCtrlIds = GetControllerIds();
            foreach (var i in nrncCtrlIds) {
                bool isFound = false;
                foreach (var j in rncCtrlIds) {
                    if (i == j) isFound = true;
                }
                if (!isFound && handler.IsAlive) {
                    (handler as IControllerHandler).OnControllerAdded(i);
                }
            }
        }

        /// <summary>
        /// Get currently available controller IDs.
        /// </summary>
        public static int[] GetControllerIds() {
            if (!isControllerInitialized) throw new InvalidOperationException("Controller is not initialized");

            int[] ids = new int[instances.Count];
            for (int i = 0; i < instances.Count; i++) {
                ids[i] = SDL.SDL_JoystickInstanceID(instances[i].joyId);
            }
            return ids;
        }

        /// <summary>
        /// Set ControllerHandler to receive controller input events.<br/>
        /// Any previously set ControllerHandler will no longer receive controller input events.
        /// </summary>
        /// <param name="handler"></param>
        public static void SetControllerHandler(IControllerHandler handler) {
            if (!isControllerInitialized) throw new InvalidOperationException("Controller is not initialized");
            Controller.handler.Target = handler;
        }

        internal static float ShortToFloat(short value) {
            if (value < 0) return value / 32768f;
            else return value / 32767f;
        }
    }
}