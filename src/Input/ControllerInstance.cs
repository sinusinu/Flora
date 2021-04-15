using System;
using SDL2;

namespace Flora.Input {
    internal class ControllerInstance {
        internal IntPtr ctrlInstance;
        internal int id;
        internal bool isClosed = false;

        internal ControllerInstance(IntPtr instance) {
            this.ctrlInstance = instance;
            id = SDL.SDL_JoystickInstanceID(instance);

            Console.WriteLine("New Controller: id {0}", id);
        }

        ~ControllerInstance() {
            if (!isClosed) Close();
        }

        internal void Close() {
            SDL.SDL_GameControllerClose(ctrlInstance);
            isClosed = true;
        }
    }
}