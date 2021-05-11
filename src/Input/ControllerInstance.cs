using System;
using SDL2;

namespace Flora.Input {
    internal class ControllerInstance : IDisposable {
        internal IntPtr ctrlInstance;
        internal int id;
        
        internal ControllerInstance(IntPtr instance) {
            this.ctrlInstance = instance;
            id = SDL.SDL_JoystickInstanceID(instance);
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (_disposed) return;
            
            /* if (disposing) {} */
            
            SDL.SDL_GameControllerClose(ctrlInstance);

            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ControllerInstance() => Dispose(false);
    }
}