using System;
using Flora;
using Flora.Gfx;
using Flora.Input;

namespace FloraExample {
    class ControllerCore : ApplicationCore, ControllerHandler {
        public override void Prepare() {
            Controller.SetControllerHandler(this);
        }

        public override void Pause() {}
        public override void Resume() {}
        public override void Resize(int width, int height) {}
        
        public override void Render(float delta) {
            Gfx.Begin();
            Gfx.End();
        }

        public void OnAxisMotion(int which, ControllerAxis axis, float value) {
            Console.WriteLine("AxisMotion\tID {0}\tAxis {1}\t{2}", which, Enum.GetName<ControllerAxis>(axis), value);
        }

        public void OnButtonDown(int which, ControllerButton button) {
            Console.WriteLine("ButtonDown\tID {0}\tButton {1}", which, Enum.GetName<ControllerButton>(button));
        }

        public void OnButtonUp(int which, ControllerButton button) {
            Console.WriteLine("ButtonUp\tID {0}\tButton {1}", which, Enum.GetName<ControllerButton>(button));
        }
    }
}