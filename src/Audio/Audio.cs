using System;
using System.Threading;
using SDL2;
using SoLoud;

namespace Flora.Audio {
    public static class Audio {
        internal static Soloud soloud;
        internal static bool isAudioInitialized = false;

        internal static void Init() {
            soloud = new Soloud();
            soloud.init(1, Soloud.SDL2);

            isAudioInitialized = true;
        }

        internal static void Deinit() {
            soloud.deinit();
        }
    }
}