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
                if (state != MusicState.Idle) Audio.soloud.setVolume(handle, _volume);
            }
        }

        internal bool _looping = false;
        public bool Looping {
            get { return _looping; }
            set { 
                _looping = value;
                wavStream.setLooping(_looping ? 1 : 0);
                if (state != MusicState.Idle) Audio.soloud.setLooping(handle, _looping ? 1 : 0);
            }
        }

        /// <summary>
        /// Create a new music.<br/>
        /// Music is better suited for playing long background music.<br/>
        /// </summary>
        /// <param name="path">Path to the sound file. WAV/MP3/OGG are supported.</param>
        /// <param name="volume">Volume of the sound. must be between 0 to 1.</param>
        /// <returns></returns>
        public Music(string path, float volume = 1f) {
            if (Audio.isAudioInitialized) throw new InvalidOperationException("Audio is not initialized");

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
            bool isPlaying = (state == MusicState.Playing);
            if (position == 0f) { Stop(); if (isPlaying) Play(); return; }
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

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (_disposed) return;

            /* if (disposing) {} */
            
            wavStream.Dispose();

            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Music() => Dispose(false);
    }
}