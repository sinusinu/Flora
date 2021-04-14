using System;
using SDL2;

namespace Flora {
    public class Sound {
        internal IntPtr sdlSound;

        internal Sound(string path) {
            sdlSound = SDL_mixer.Mix_LoadWAV(path);
            if (sdlSound == IntPtr.Zero) throw new ArgumentException("Mix_LoadWAV failed: " + SDL.SDL_GetError());
        }

        public void Play() {
            SDL_mixer.Mix_PlayChannel(-1, sdlSound, 0);
        }

        ~Sound() {
            SDL_mixer.Mix_FreeChunk(sdlSound);
        }
    }
}