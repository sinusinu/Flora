using System;
using Flora;
using Flora.Gfx;
using Flora.Input;

namespace FloraExample {
    class InputCore : ApplicationCore, InputHandler {
        public override void Prepare() {
            Input.SetInputHandler(this);
        }

        public override void Pause() {
            
        }

        public override void Resume() {
            
        }

        public override void Resize(int width, int height) {
            
        }

        public override void Render(float delta) {
            Gfx.Begin(new Color(0x64, 0x95, 0xED, 0xFF));
            Gfx.End();
        }

        public override void Dispose() {
            
        }

        public void OnKeyDown(KeyCode keycode) {
            Console.WriteLine("KeyDown: {0}", Enum.GetName<KeyCode>(keycode));
        }

        public void OnKeyUp(KeyCode keycode) {
            Console.WriteLine("KeyUp: {0}", Enum.GetName<KeyCode>(keycode));
        }

        public void OnMouseDown(MouseButton button, int x, int y) {
            Console.WriteLine("MouseDown: {0} ({1}, {2})", Enum.GetName<MouseButton>(button), x, y);
        }

        public void OnMouseMove(int x, int y) {
            Console.WriteLine("MouseMove: ({0}, {1})", x, y);
        }

        public void OnMouseUp(MouseButton button, int x, int y) {
            Console.WriteLine("MouseUp: {0} ({1}, {2})", Enum.GetName<MouseButton>(button), x, y);
        }
    }
}