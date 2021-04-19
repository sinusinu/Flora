using System;
using Flora;
using Flora.Gfx;
using Flora.Util;

namespace FloraExample {
    class FontCore : ApplicationCore {
        Font font;

        public override void Prepare() {
            // WARNING: This file is missing from git repository for legal reasons.
            //          put any of yours accordingly to test this core.
            font = new Font(Path.Relative("res\\font.ttf"), 48);

            font.SetHinting(FontHinting.Mono);
        }

        public override void Pause() {}
        public override void Resume() {}
        public override void Resize(int width, int height) {}
        
        public override void Render(float delta) {
            Gfx.Begin(new Color(0x64, 0x95, 0xEB, 0xFF));

            font.SetScale(1f);
            font.SetColor(255, 255, 255, 255);
            font.Draw("The quick brown fox jumps over the lazy dog.", 50, 80);

            font.SetScale(2f);
            font.SetColor(255, 255, 0, 255);
            font.Draw("이것은 테스트입니다.", 50, 130);

            Gfx.End();
        }
        
        public override void Dispose() {}
    }
}