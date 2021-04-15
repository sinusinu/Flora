using System;
using SDL2;

namespace Flora.Audio {
    public static class Audio {
        internal static bool isAudioInitialized = false;

        internal static void Init() {
            isAudioInitialized = true;
        }
        
        public static Sound CreateSound(string path) {
            if (!isAudioInitialized) throw new InvalidOperationException("Audio is not initialized");
            return new Sound(path);
        }

        public static Music CreateMusic(string path) {
            if (!isAudioInitialized) throw new InvalidOperationException("Audio is not initialized");
            return new Music(path);
        }
    }
}