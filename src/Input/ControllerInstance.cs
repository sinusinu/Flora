using System;
using SDL2;

namespace Flora.Input {
    internal class ControllerInstance : IDisposable {
        internal IntPtr ctrlInstance;
        internal int id;
        
        private bool disposed = false;

        internal ControllerInstance(IntPtr instance) {
            this.ctrlInstance = instance;
            id = SDL.SDL_JoystickInstanceID(instance);

            Console.WriteLine("New Controller: id {0}", id);
        }

        public void Dispose() {
            if (disposed) return;
            
            SDL.SDL_GameControllerClose(ctrlInstance);
            
            disposed = true;
            
            GC.SuppressFinalize(this);
        }
        
        ~ControllerInstance() {
            Dispose();
        }
    }
}