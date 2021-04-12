namespace Flora {
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
        /// Your render logic should be placed here.
        /// </summary>
        void Render(float delta);

        /// <summary>
        /// Called when your resources should dispose.
        /// </summary>
        void Dispose();
    }
}