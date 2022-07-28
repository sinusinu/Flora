using System;
using System.Threading;
using SDL2;

namespace Flora.Audio;

public static class Audio {
    internal static bool isAudioInitialized = false;

    internal static void Init() {

        isAudioInitialized = true;
    }

    internal static void Deinit() {
        
    }
}