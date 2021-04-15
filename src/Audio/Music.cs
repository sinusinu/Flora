using System;
using System.Reflection;
using System.Runtime.InteropServices;
using SDL2;

namespace Flora.Audio {
    public class Music {
        internal IntPtr sdlMusic;

        public enum MusicState { Idle, Playing, Paused }
        public MusicState state { get; internal set; }
        
        internal static bool isMixer205;

        // Music position tracking prior to 2.0.5
        // FIXME: SDL_mixer 2.0.5 will have SDL_GetMusicPosition(Mix_Music*), but current stable release is 2.0.4.
        internal ulong timeLastPlay;
        internal ulong timeLastPause;
        internal static ulong timerFreq;

        internal Music(string path) {
            sdlMusic = SDL_mixer.Mix_LoadMUS(path);
            if (sdlMusic == IntPtr.Zero) throw new ArgumentException("Mix_LoadMUS failed: " + SDL.SDL_GetError());

            try {
                SDL_mixer.Mix_GetMusicPosition(IntPtr.Zero);
                isMixer205 = true;
            } catch (EntryPointNotFoundException) {
                Console.WriteLine("Warning: SDL_mixer 2.0.4 or lower found. Music position tracking might be imprecise.");
                isMixer205 = false;
            }

            if (!isMixer205) {
                timerFreq = SDL.SDL_GetPerformanceFrequency();
            }
        }

        public void Play() {
            if (state == MusicState.Paused) {
                SDL_mixer.Mix_ResumeMusic();
                state = MusicState.Playing;

                if (!isMixer205) timeLastPlay += (SDL.SDL_GetPerformanceCounter() - timeLastPause);
            } else if (state == MusicState.Idle) {
                SDL_mixer.Mix_PlayMusic(sdlMusic, 0);
                state = MusicState.Playing;

                if (!isMixer205) timeLastPlay = SDL.SDL_GetPerformanceCounter();
            }
        }

        public void Pause() {
            if (state == MusicState.Paused) return;
            SDL_mixer.Mix_PauseMusic();
            state = MusicState.Paused;
            
            if (!isMixer205) timeLastPause = SDL.SDL_GetPerformanceCounter();
        }

        public void Stop() {
            if (state == MusicState.Idle) return;
            SDL_mixer.Mix_HaltMusic();
            state = MusicState.Idle;
        }

        public void SetPosition(float position) {
            SDL_mixer.Mix_SetMusicPosition(position);

            if (!isMixer205) {
                ulong cnt = SDL.SDL_GetPerformanceCounter();
                timeLastPlay = cnt - (ulong)(position * timerFreq);
                if (state == MusicState.Paused) timeLastPause = cnt;
            }
        }

        public float GetPosition() {
            if (state == MusicState.Idle) return 0f;
            if (isMixer205) {
                return (float)SDL_mixer.Mix_GetMusicPosition(sdlMusic);
            } else {
                if (state == MusicState.Paused) return (timeLastPause - timeLastPlay) / (float)timerFreq;
                else return (SDL.SDL_GetPerformanceCounter() - timeLastPlay) / (float)timerFreq;
            }
        }

        ~Music() {
            SDL_mixer.Mix_FreeMusic(sdlMusic);
        }
    }
}