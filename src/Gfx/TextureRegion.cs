using Flora.Util;

namespace Flora.Gfx {
    public class TextureRegion {
        internal Texture texture;
        public Rect rect;

        public TextureRegion(Texture texture, int x, int y, int w, int h) {
            this.texture = texture;
            rect = new Rect(x, y, w, h);
        }
    }
}