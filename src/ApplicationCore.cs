using System;

namespace Flora {
    /// <summary>
    /// Core of the application.
    /// </summary>
    public class ApplicationCore {
        // will be set in ctor of FloraApplication
        internal FloraApplication _floraApplication;

        /// <summary>
        /// Called when Flora is ready to start your application.<br/>
        /// It is recommended to load resources from this function.
        /// </summary>
        public virtual void Prepare() {}

        /// <summary>
        /// Called when application lost focus.
        /// </summary>
        public virtual void Pause() {}

        /// <summary>
        /// Called when application regained lost focus.
        /// </summary>
        public virtual void Resume() {}

        /// <summary>
        /// Called when the window has been resized.
        /// </summary>
        public virtual void Resize(int width, int height) {}

        /// <summary>
        /// Main logic of the application should be placed here.
        /// </summary>
        public virtual void Render(float delta) {}

        /// <summary>
        /// Any disposable resources must be disposed at here. NOTE: calling manners of this function might differ slightly from typical IDisposable.Dispose() manners!
        /// </summary>
        public virtual void Dispose() {}

        /// <summary>
        /// Schedule exit of the application. Note that exit will not happen immediately.
        /// </summary>
        public void Exit() { _floraApplication.Exit(); }
    }
}
