using System;
using SDL2;

namespace Flora {
    public class Music {
        internal IntPtr sdlMusic;

        internal Music(string path) {
            sdlMusic = SDL_mixer.Mix_LoadMUS(path);
            if (sdlMusic == IntPtr.Zero) throw new ArgumentException("Mix_LoadMUS failed: " + SDL.SDL_GetError());
        }

        public void Play() {
            SDL_mixer.Mix_PlayMusic(sdlMusic, 0);
        }

        ~Music() {
            SDL_mixer.Mix_FreeMusic(sdlMusic);
        }
    }
}