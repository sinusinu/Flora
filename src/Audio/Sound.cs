using System;
using SDL2;

namespace Flora.Audio {
    /// <summary>
    /// Audio instance that suit better for playing fire-and-forget type of short sound effects.
    /// </summary>
    public class Sound : IDisposable {
        internal IntPtr sdlSound;
        internal int channel;

        private byte _volume = SDL_mixer.MIX_MAX_VOLUME;
        public float Volume {
            get { return _volume / (float)SDL_mixer.MIX_MAX_VOLUME; }
            set {
                if (value < 0f || value > 1f) throw new ArgumentException("volume must be between 0 and 1");
                _volume = (byte)(value * SDL_mixer.MIX_MAX_VOLUME);
                SDL_mixer.Mix_Volume(channel, _volume);
            }
        }

        private bool disposed = false;

        internal Sound(string path, float volume = 1f) {
            Audio.mtxSound.WaitOne();

            int assignedChannel = -1;
            for (int i = 0; i < Audio.numChannels; i++) {
                if (Audio.sounds[i] == null) {
                    assignedChannel = i;
                    Audio.sounds[i] = this;
                    break;
                }
            }
            
            Audio.mtxSound.ReleaseMutex();

            if (assignedChannel == -1) throw new Exception("Too many sounds; No channel is available");
            else channel = assignedChannel;

            Volume = volume;

            sdlSound = SDL_mixer.Mix_LoadWAV(path);
            if (sdlSound == IntPtr.Zero) throw new ArgumentException("Mix_LoadWAV failed: " + SDL.SDL_GetError());
        }

        /// <summary>
        /// Play the sound.
        /// </summary>
        /// <param name="singleton">If true, any of this sound playing will be halted before playing.</param>
        public void Play(bool singleton = false) {
            if (singleton) SDL_mixer.Mix_HaltChannel(channel);
            SDL_mixer.Mix_PlayChannel(channel, sdlSound, 0);
        }

        public void Dispose() {
            if (disposed) return;
            
            Audio.sounds[channel] = null;

            SDL_mixer.Mix_FreeChunk(sdlSound);
            sdlSound = IntPtr.Zero;
            
            disposed = true;
            
            GC.SuppressFinalize(this);
        }

        ~Sound() {
            Dispose();
        }
    }
}