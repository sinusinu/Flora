using Flora;

namespace FloraTest;

public class TestCore : ScreenCore {
    public Font font = null!;

    public override void Prepare() {
        font = Gfx.CreateFont("test.otf", 32);
        Gfx.SetViewport(1280, 720, Graphics.ViewportOpts.Overscan);
        
        SetScreen(new TestScreen(this));
    }

    public override void Cleanup() {
        base.Cleanup(); // for ScreenCore all overrides except Prepare must call its base function! (they delegate call to the active screen)
        font.Dispose();
    }
}