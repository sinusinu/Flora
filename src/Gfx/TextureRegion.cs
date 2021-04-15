using Flora.Util;

namespace Flora.Gfx {
    /// <summary>
    /// Part of the texture that can be drawn on screen.
    /// </summary>
    public class TextureRegion {
        internal Texture texture;
        public Rect rect;

        public TextureRegion(Texture texture, int x, int y, int w, int h) {
            this.texture = texture;
            rect = new Rect(x, y, w, h);
        }
    }
}