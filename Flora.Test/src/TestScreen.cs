using Flora;

namespace FloraTest;

public class TestScreen : Screen {
    private const float DegToRad = 0.0174533f;

    TestCore core;

    Texture texture = null!;

    Sound sound = null!;

    float dingus = 0f;
    bool spin = false;

    const int deltaSampleSize = 128;
    float[] deltaSamples = new float[deltaSampleSize];
    int deltaSampleIndex = 0;

    int dx = 0;
    int dy = 0;
    int dr = 0;

    float targetSX = 1f;
    float targetSY = 1f;
    float intermediateSX = 1f;
    float intermediateSY = 1f;

    public TestScreen(TestCore core) {
        this.core = core;
    }

    public override void Prepare() {
        texture = core.Gfx.CreateTexture("test.png");

        sound = core.Audio.CreateSound("test.wav");
        sound.Stopped += () => {
            Console.WriteLine("sound stopped");
        };

        core.Input.ActiveInputChanged += ActiveInputChanged;
    }

    private void ActiveInputChanged(Input.ActiveInputOpts newInput) {
        Console.WriteLine($"Active Input changed: {newInput}");
    }

    public override void Render(float delta) {
        deltaSamples[deltaSampleIndex] = delta;
        deltaSampleIndex++; if (deltaSampleIndex == deltaSampleSize) deltaSampleIndex = 0;
        // Console.WriteLine($"FPS: {float.Round(1f / deltaSamples.Average(), 1):00.0}");

        if (sound.Playing) Console.WriteLine($"{sound.Position} / {sound.Length}");

        dingus += delta * 4f;

        core.Gfx.Begin();

        core.Gfx.Camera.X += 720f * delta * dx / targetSX;
        core.Gfx.Camera.Y += 720f * delta * dy / targetSY;
        core.Gfx.Camera.Rotation += 180f * delta * dr * DegToRad;

        if (MathF.Abs(targetSX - intermediateSX) < 0.005f) intermediateSX = targetSX;
        else intermediateSX = targetSX * 0.2f + intermediateSX * 0.8f;
        if (MathF.Abs(targetSY - intermediateSY) < 0.005f) intermediateSY = targetSY;
        else intermediateSY = targetSY * 0.2f + intermediateSY * 0.8f;
        core.Gfx.Camera.ScaleX = intermediateSX;
        core.Gfx.Camera.ScaleY = intermediateSY;

        core.Gfx.Camera.PushState();

        core.Gfx.Camera.Rotation = MathF.Sin(dingus / 2f) * 40 * DegToRad;
        core.Gfx.Camera.X = MathF.Sin(dingus) * 200;
        core.Gfx.Camera.Y = 0;
        core.Gfx.Camera.ScaleX *= ((MathF.Sin(dingus) + 1f) * 0.125f) + 1f;
        core.Gfx.Camera.ScaleY *= ((MathF.Cos(dingus) + 1f) * 0.125f) + 1f;

        for (int y = -11; y < 11; y++) {
            for (int x = -12; x < 12; x++) {
                core.Gfx.RenderColor = new((x + 12) / 24f, (y + 11) / 22f, 1f, 1f);
                core.Gfx.Draw(texture, 128 * x, 128 * y, 128, 128, (spin ? dingus : 0) * (40 + ((x + y + 1) * 4)) * DegToRad, 64, 64);
            }
        }
        
        core.Gfx.Camera.PopState();

        core.Gfx.Draw(texture, 0, 0, 128, 128);
        core.font.Color = new Color(0f, 0f, 0f, 1f);
        core.font.Draw("(0, 0)", 2, 2);
        core.font.Color = new Color(1f, 1f, 1f, 1f);
        core.font.Draw("(0, 0)", 0, 0);

        float anchorSize = 64 / intermediateSX;
        var va = core.Gfx.VisibleArea;
        core.Gfx.Draw(texture, va.x,                     va.y,                     anchorSize, anchorSize);
        core.Gfx.Draw(texture, va.x + va.w - anchorSize, va.y,                     anchorSize, anchorSize);
        core.Gfx.Draw(texture, va.x,                     va.y + va.h - anchorSize, anchorSize, anchorSize);
        core.Gfx.Draw(texture, va.x + va.w - anchorSize, va.y + va.h - anchorSize, anchorSize, anchorSize);

        core.Gfx.End();
    }

    public override void PointerDown(PointerType type, int button, float x, float y) {
        (float wx, float wy) = core.Gfx.ScreenToWorld(x, y);
        Console.WriteLine($"[{type}] down {wx}, {wy}");
    }

    public override void KeyDown(Keycode key, Scancode scan) {
        if (key == Keycode.Escape) {
            core.App.Exit();
        } else if (key == Keycode.F11) {
            if (core.App.Window.WindowMode == Window.WindowModeOpts.Windowed) {
                core.App.Window.SetFullscreen();
            } else {
                core.App.Window.SetWindowed(640, 480);
            }
        } else if (key == Keycode.Q) {
            Console.WriteLine($"Camera is at {core.Gfx.Camera.X}, {core.Gfx.Camera.Y}");
            Console.WriteLine($"Visible area: {core.Gfx.VisibleArea}");
        } else if (key == Keycode.Up) {
            dy -= 1;
        } else if (key == Keycode.Down) {
            dy += 1;
        } else if (key == Keycode.Left) {
            dx -= 1;
        } else if (key == Keycode.Right) {
            dx += 1;
        } else if (key == Keycode.Delete) {
            dr -= 1;
        } else if (key == Keycode.PageDown) {
            dr += 1;
        } else if (key == Keycode.R) {
            spin = !spin;
            dingus = 0;
        } else if (key == Keycode.KeypadPlus) {
            targetSX *= 2;
            targetSY *= 2;
        } else if (key == Keycode.KeypadMinus) {
            targetSX /= 2;
            targetSY /= 2;
        } else if (key == Keycode.Keypad0) {
            core.Gfx.Camera.X = 0;
            core.Gfx.Camera.Y = 0;
            core.Gfx.Camera.Rotation = 0;
            targetSX = 1;
            targetSY = 1;
        } else if (key == Keycode.P) {
            sound.Volume = 1f;
            sound.Play();
        } else if (key == Keycode.O) {
            sound.Volume = 0.25f;
            sound.Play();
        } else if (key == Keycode.L) {
            if (sound.Paused) sound.Resume();
            else sound.Pause();
        } else if (key == Keycode.K) {
            sound.Stop();
        } else if (key == Keycode.LeftBracket) {
            sound.Volume = 0.5f;
        } else if (key == Keycode.RightBracket) {
            sound.Volume = 1f;
        } else if (key == Keycode.Semicolon) {
            core.Audio.MasterVolume = 0.5f;
        } else if (key == Keycode.Apostrophe) {
            core.Audio.MasterVolume = 1f;
        } else if (key == Keycode.Insert) {
            core.SetScreen(new TestTwoScreen(core));
        }
    }

    public override void KeyUp(Keycode key, Scancode scan) {
        if (key == Keycode.Up) {
            dy += 1;
        } else if (key == Keycode.Down) {
            dy -= 1;
        } else if (key == Keycode.Left) {
            dx += 1;
        } else if (key == Keycode.Right) {
            dx -= 1;
        } else if (key == Keycode.Delete) {
            dr += 1;
        } else if (key == Keycode.PageDown) {
            dr -= 1;
        }
    }

    public override void Cleanup() {
        core.Input.ActiveInputChanged -= ActiveInputChanged;

        sound.Dispose();
        texture.Dispose();
    }
}