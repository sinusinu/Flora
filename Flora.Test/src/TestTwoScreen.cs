using Flora;

namespace FloraTest;

public class TestTwoScreen : Screen {
    TestCore core;

    public TestTwoScreen(TestCore core) {
        this.core = core;
    }

    public override void Render(float delta) {
        var va = core.Gfx.VisibleArea;
        core.Gfx.Begin();
        core.font.Draw("so...\ncome here often?", va.x, va.y);
        core.Gfx.End();
    }

    public override void KeyDown(Keycode key, Scancode scan) {
        if (key == Keycode.Escape) {
            core.App.Exit();
        } else if (key == Keycode.Insert) {
            core.SetScreen(new TestScreen(core));
        }
    }
}