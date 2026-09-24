using Flora;

namespace FloraTest;

public class TestCore : Core {
    Texture texture = null!;
    Font font = null!;

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

    public override void Prepare() {
        texture = Gfx.CreateTexture("test.png");
        font = Gfx.CreateFont("test.otf", 32);

        sound = Audio.CreateSound("test.wav");
        sound.Stopped += () => {
            Console.WriteLine("sound stopped");
        };

        Gfx.SetViewport(1280, 720, Graphics.ViewportOpts.Overscan);
    }

    public override void Render(float delta) {
        deltaSamples[deltaSampleIndex] = delta;
        deltaSampleIndex++; if (deltaSampleIndex == deltaSampleSize) deltaSampleIndex = 0;
        // Console.WriteLine($"FPS: {float.Round(1f / deltaSamples.Average(), 1):00.0}");

        if (sound.Playing) Console.WriteLine($"{sound.Position} / {sound.Length}");

        dingus += delta * 4f;

        Gfx.Begin();

        Gfx.Camera.X += 720f * delta * dx / targetSX;
        Gfx.Camera.Y += 720f * delta * dy / targetSY;
        Gfx.Camera.Rotation += 180f * delta * dr;

        if (MathF.Abs(targetSX - intermediateSX) < 0.005f) intermediateSX = targetSX;
        else intermediateSX = targetSX * 0.2f + intermediateSX * 0.8f;
        if (MathF.Abs(targetSY - intermediateSY) < 0.005f) intermediateSY = targetSY;
        else intermediateSY = targetSY * 0.2f + intermediateSY * 0.8f;
        Gfx.Camera.ScaleX = intermediateSX;
        Gfx.Camera.ScaleY = intermediateSY;

        Gfx.Camera.PushState();

        Gfx.Camera.Rotation = MathF.Sin(dingus / 2f) * 40;
        Gfx.Camera.X = MathF.Sin(dingus) * 200;
        Gfx.Camera.ScaleX *= ((MathF.Sin(dingus) + 1f) * 0.125f) + 1f;
        Gfx.Camera.ScaleY *= ((MathF.Cos(dingus) + 1f) * 0.125f) + 1f;

        for (int y = -11; y < 11; y++) {
            for (int x = -12; x < 12; x++) {
                Gfx.RenderColor = new((x + 12) / 24f, (y + 11) / 22f, 1f, 1f);
                Gfx.Draw(texture, 128 * x, 128 * y, 128, 128, (spin ? dingus : 0) * (40 + ((x + y + 1) * 4)), 64, 64);
            }
        }
        
        Gfx.Camera.PopState();

        Gfx.Draw(texture, 0, 0, 128, 128);
        font.Color = new Color(0f, 0f, 0f, 1f);
        font.Draw("(0, 0)", 2, 2);
        font.Color = new Color(1f, 1f, 1f, 1f);
        font.Draw("(0, 0)", 0, 0);

        float anchorSize = 64 / intermediateSX;
        var va = Gfx.VisibleArea;
        Gfx.Draw(texture, va.x,                     va.y,                     anchorSize, anchorSize);
        Gfx.Draw(texture, va.x + va.w - anchorSize, va.y,                     anchorSize, anchorSize);
        Gfx.Draw(texture, va.x,                     va.y + va.h - anchorSize, anchorSize, anchorSize);
        Gfx.Draw(texture, va.x + va.w - anchorSize, va.y + va.h - anchorSize, anchorSize, anchorSize);

        Gfx.End();
    }

    public override void OnPointerDown(PointerType type, int button, float x, float y) {
        (float wx, float wy) = Gfx.ScreenToWorld(x, y);
        Console.WriteLine($"[{type}] down {wx}, {wy}");
    }

    public override void OnKeyDown(Keycode key, Scancode scan) {
        if (key == Keycode.Escape) {
            App.Exit();
        } else if (key == Keycode.F11) {
            if (App.Window.WindowMode == Window.WindowModeOpts.Windowed) {
                App.Window.SetFullscreen();
            } else {
                App.Window.SetWindowed(640, 480);
            }
        } else if (key == Keycode.Q) {
            Console.WriteLine($"Camera is at {Gfx.Camera.X}, {Gfx.Camera.Y}");
            Console.WriteLine($"Visible area: {Gfx.VisibleArea}");
        } else if (key == Keycode.Up) {
            dy -= 1;
        } else if (key == Keycode.Down) {
            dy += 1;
        } else if (key == Keycode.Left) {
            dx -= 1;
        } else if (key == Keycode.Right) {
            dx += 1;
        } else if (key == Keycode.Delete) {
            dr += 1;
        } else if (key == Keycode.PageDown) {
            dr -= 1;
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
            Gfx.Camera.X = 0;
            Gfx.Camera.Y = 0;
            Gfx.Camera.Rotation = 0;
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
            Audio.MasterVolume = 0.5f;
        } else if (key == Keycode.Apostrophe) {
            Audio.MasterVolume = 1f;
        }
    }

    public override void OnKeyUp(Keycode key, Scancode scan) {
        if (key == Keycode.Up) {
            dy += 1;
        } else if (key == Keycode.Down) {
            dy -= 1;
        } else if (key == Keycode.Left) {
            dx += 1;
        } else if (key == Keycode.Right) {
            dx -= 1;
        } else if (key == Keycode.Delete) {
            dr -= 1;
        } else if (key == Keycode.PageDown) {
            dr += 1;
        }
    }

    public override void Cleanup() {
        sound.Dispose();
        font.Dispose();
        texture.Dispose();
    }
}