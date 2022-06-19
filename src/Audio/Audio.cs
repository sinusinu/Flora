using System;
using System.Threading;
using SDL2;
using SoLoud;

namespace Flora.Audio;

public static class Audio {
    internal static Soloud soloud;
    internal static bool isAudioInitialized = false;

    internal static void Init() {
        soloud = new Soloud();
        int result = soloud.init(1, Soloud.SDL2);
        if (result != 0) {
            SDL_ttf.TTF_Quit();
            SDL.SDL_Quit();
            throw new Exception("Failed to initialize Flora: Soloud::init failed (result: " + result + ")");
        }

        isAudioInitialized = true;
    }

    internal static void Deinit() {
        soloud.deinit();
    }
}