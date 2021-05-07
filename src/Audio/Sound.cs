using System;
using SDL2;
using SoLoud;

namespace Flora.Audio {
    /// <summary>
    /// Audio instance that suit better for playing fire-and-forget type of short sound effects.
    /// </summary>
    public class Sound : IDisposable {
        internal Wav wav;

        private float _volume = 1f;
        public float Volume {
            get { return _volume; }
            set { 
                _volume = Math.Clamp(value, 0f, 1f);
                wav.setVolume(_volume);
            }
        }

        private bool disposed = false;

        internal Sound(string path, float volume = 1f) {
            wav = new Wav();
            wav.load(path);
            this.Volume = volume;
        }

        /// <summary>
        /// Play the sound.
        /// </summary>
        /// <param name="singleton">If true, any of this sound playing will be halted before playing.</param>
        public void Play(bool singleton = false) {
            if (singleton) Audio.soloud.stopAudioSource(wav);
            Audio.soloud.play(wav, _volume);
        }

        public void Dispose() {
            if (disposed) return;

            wav.Dispose();
            
            disposed = true;
            
            GC.SuppressFinalize(this);
        }

        ~Sound() {
            Dispose();
        }
    }
}