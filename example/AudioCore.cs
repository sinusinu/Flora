using System;
using Flora;
using Flora.Audio;
using Flora.Gfx;
using Flora.Input;
using Flora.Util;

namespace FloraExample {
    class AudioCore : ApplicationCore, InputHandler {
        Sound sound;
        Music music;

        public override void Prepare() {
            sound = Audio.CreateSound(Path.Relative("res/se.mp3"));
            music = Audio.CreateMusic(Path.Relative("res/bgm.mp3"));

            Input.SetInputHandler(this);
        }

        public override void Pause() {
            
        }

        public override void Resume() {
            
        }

        public override void Resize(int width, int height) {
            
        }

        public override void Render(float delta) {
            Console.WriteLine("Music position: {0}", music.GetPosition());

            Gfx.Begin();
            Gfx.End();
        }

        public override void Dispose() {
            
        }

        public void OnKeyDown(KeyCode keycode) {
            if (keycode == KeyCode.Q) sound.Play();
            else if (keycode == KeyCode.A) music.Play();
            else if (keycode == KeyCode.S) music.Pause();
            else if (keycode == KeyCode.D) music.Stop();
        }

        public void OnKeyUp(KeyCode keycode) {
            
        }

        public void OnMouseDown(MouseButton button, int x, int y) {
            
        }

        public void OnMouseMove(int x, int y) {
            
        }

        public void OnMouseUp(MouseButton button, int x, int y) {
            
        }
    }
}