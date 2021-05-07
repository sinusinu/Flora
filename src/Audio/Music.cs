using System;
using System.Reflection;
using System.Runtime.InteropServices;
using SDL2;
using SoLoud;

namespace Flora.Audio {
    /// <summary>
    /// Audio instance that suit better for playing long background music.
    /// </summary>
    public class Music : IDisposable {
        internal WavStream wavStream;
        internal uint handle = 0;

        public enum MusicState { Idle, Playing, Paused }
        
        /// <summary>
        /// State of the playback of this music.
        /// </summary>
        public MusicState state { get {
            if (handle == 0) return MusicState.Idle;
            if (Audio.soloud.isValidVoiceHandle(handle) == 0) { handle = 0; return MusicState.Idle; }
            if (Audio.soloud.getPause(handle) > 0) return MusicState.Paused;
            return MusicState.Playing;
        }}
        
        internal float _volume = 1f;
        public float Volume {
            get { return _volume / (float)SDL_mixer.MIX_MAX_VOLUME; }
            set {
                _volume = Math.Clamp(value, 0f, 1f);
                wavStream.setVolume(_volume);
            }
        }

        private bool disposed = false;

        internal Music(string path, float volume = 1f) {
            wavStream = new WavStream();
            wavStream.load(path);
            Volume = volume;
        }

        /// <summary>
        /// Play the music.<br/>
        /// If music is paused, playback will resume.<br/>
        /// If music is playing, nothing will happen.
        /// </summary>
        public void Play() {
            switch (state) {
                case MusicState.Idle:
                    handle = Audio.soloud.play(wavStream, _volume);
                    break;
                case MusicState.Paused:
                    Audio.soloud.setPause(handle, 0);
                    break;
            }
        }

        /// <summary>
        /// Pause the music.<br/>
        /// If music is not playing, nothing will happen.
        /// </summary>
        public void Pause() {
            if (Audio.soloud.getPause(handle) > 0) return;
            Audio.soloud.setPause(handle, 1);
        }

        /// <summary>
        /// Stop the music.<br/>
        /// If music is neither playing or paused, nothing will happen.
        /// </summary>
        public void Stop() {
            if (state == MusicState.Idle) return;
            Audio.soloud.stop(handle);
            handle = 0;
        }

        /// <summary>
        /// Set the position of this music.
        /// </summary>
        /// <param name="position">New position in seconds</param>
        public void SetPosition(float position) {
            if (position == 0f) { Stop(); Play(); return; }
            bool isPlaying = (state == MusicState.Playing);
            Audio.soloud.setPause(handle, 1);
            Audio.soloud.seek(handle, 0d);
            Audio.soloud.seek(handle, position);
            if (isPlaying) Audio.soloud.setPause(handle, 0);
        }

        /// <summary>
        /// Get the position of this music.
        /// </summary>
        /// <returns>Current position in seconds</returns>
        public float GetPosition() {
            switch (state) {
                case MusicState.Idle:
                    return 0f;
            }
            return (float)Audio.soloud.getStreamPosition(handle);
        }

        public void Dispose() {
            if (disposed) return;
            
            Stop();
            wavStream.Dispose();
            
            disposed = true;
            
            GC.SuppressFinalize(this);
        }

        ~Music() {
            Dispose();
        }
    }
}