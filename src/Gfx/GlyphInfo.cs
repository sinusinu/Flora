using Flora.Util;

namespace Flora.Gfx {
    internal class GlyphInfo {
        public int page;
        public Rect rect;

        internal GlyphInfo() {
            page = -1;
            rect = null;
        }
    }
}