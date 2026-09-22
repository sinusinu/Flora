using Flora;

namespace FloraTest;

public class TestCore : Core {
    Texture texture = null!;
    Font font = null!;

    float dingus = 0f;

    const int deltaSampleSize = 128;
    float[] deltaSamples = new float[deltaSampleSize];
    int deltaSampleIndex = 0;

    public override void Prepare() {
        texture = Gfx.CreateTexture("test.png");
        font = Gfx.CreateFont("test.otf", 24);

        Gfx.SetViewport(1280, 720, Graphics.ViewportOpts.Overscan);
    }

    public override void Render(float delta) {
        deltaSamples[deltaSampleIndex] = delta;
        deltaSampleIndex++; if (deltaSampleIndex == deltaSampleSize) deltaSampleIndex = 0;
        Console.WriteLine($"FPS: {float.Round(1f / deltaSamples.Average(), 1):00.0}");

        dingus += delta * 4f;

        Gfx.Begin();

        Gfx.Camera.PushState();

        Gfx.Camera.Rotation = MathF.Sin(dingus / 2f) * 40;
        Gfx.Camera.X = MathF.Sin(dingus) * 200;
        Gfx.Camera.ScaleX = ((MathF.Sin(dingus) + 1f) * 0.125f) + 1f;
        Gfx.Camera.ScaleY = ((MathF.Cos(dingus) + 1f) * 0.125f) + 1f;

        for (int y = -6; y < 6; y++) {
            for (int x = -7; x < 7; x++) {
                Gfx.RenderColor = new((x + 7) / 13f, (y + 6) / 11f, 1f, 1f);
                Gfx.Draw(texture, 128 * x, 128 * y, 128, 128, dingus * (40 + ((x + y + 1) * 4)), 64, 64);
            }
        }
        
        font.Color = new Color(1f, 1f, 0f, 1f);
        font.Draw("이 텍스트는 카메라의\n영향을 받습니다.\nThe quick brown fox\njumps over the lazy dog", -200, -200);

        Gfx.Camera.PopState();

        font.Color = new Color(0f, 1f, 1f, 0.8f);
        font.Draw("이 텍스트는 카메라의\n영향을 받지 않습니다.", 0, 150);

        Gfx.End();
    }

    public override void Cleanup() {
        font.Dispose();
        texture.Dispose();
    }
}