using System;
using Flora;
using Flora.Gfx;
using Flora.Util;

namespace FloraExample {
    class TextureCore : ApplicationCore {
        Texture texture;
        TextureRegion region;

        double rot = 0d;

        public override void Prepare() {
            texture = new Texture(new RelativePath("test.png"));
            region = new TextureRegion(texture, 50, 50, 100, 100);
        }

        public override void Pause() {

        }

        public override void Resume() {
            
        }

        public override void Resize(int width, int height) {
            
        }

        public override void Render(float delta) {
            rot += delta * (1 / Math.PI);

            Gfx.Begin(new Color(0x64, 0x95, 0xED, 0xFF));   // Clear with Cornflower Blue

            // You can draw the whole texture...
            Gfx.Draw(texture, 0, 0);

            // or part of it by using TextureRegion...
            Gfx.Draw(region, 260, 0);

            // or scaled...
            Gfx.Draw(texture, 260, 110, 150, 150);

            // or with all kinds of crazy options
            Gfx.Draw(region, 50, 275, 150, 150, rot, 75, 75, Gfx.FlipMode.Horizontal);

            // with color!
            Gfx.SetColor(0xB9, 0xD9, 0xEB, 0xFF);
            Gfx.Draw(texture, 300, 275, 250, 100, rot * 0.25d, 200, 50, Gfx.FlipMode.Horizontal | Gfx.FlipMode.Vertical);
            Gfx.SetColor(0xFF, 0xFF, 0xFF, 0xFF);

            Gfx.End();
        }

        public override void Dispose() {
            
        }
    }
}