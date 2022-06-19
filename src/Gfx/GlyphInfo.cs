using Flora.Util;

namespace Flora.Gfx;

internal class GlyphInfo {
    public int page;
    public Rect rect;

    internal GlyphInfo() {
        page = -1;
        rect = null;
    }

    internal GlyphInfo(int page, Rect rect) {
        this.page = page;
        this.rect = rect;
    }
}