using System;
using Flora;
using Flora.Gfx;

namespace FloraExample {
    class BasicCore : ApplicationCore {
        public override void Prepare() {
            Console.WriteLine("Prepare called");
        }

        public override void Pause() {
            Console.WriteLine("Pause called");
        }

        public override void Resume() {
            Console.WriteLine("Resume called");
        }

        public override void Resize(int width, int height) {
            Console.WriteLine("Resize called: {0} {1}", width, height);
        }

        public override void Render(float delta) {
            // Since render function is called 60 times per second by default, this will make output unreadable.
            //Console.WriteLine("Render called: {0}", delta.ToString("0.#######"));
            
            Gfx.Begin();
            Gfx.End();
        }
    }
}