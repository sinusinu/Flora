namespace Flora {
    /// <summary>
    /// Stub for utilizing multiple screens.
    /// </summary>
    public interface Screen {
        /// <summary>
        /// Called when application lost focus.
        /// </summary>
        void Pause();

        /// <summary>
        /// Called when application regained lost focus.
        /// </summary>
        void Resume();

        /// <summary>
        /// Called when the window has been resized.
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        void Resize(int width, int height);

        /// <summary>
        /// Main logic of the screen should be placed here.
        /// </summary>
        void Render(float delta);
    }
}