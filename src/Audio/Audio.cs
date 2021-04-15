using System;
using SDL2;

namespace Flora.Audio {
    public static class Audio {
        internal static bool isAudioInitialized = false;

        internal static void Init() {
            isAudioInitialized = true;
        }
        
        /// <summary>
        /// Create a new sound.<br/>
        /// Sound is better suited for playing fire-and-forget type of short sound clips.
        /// </summary>
        /// <param name="path">Path to the sound file. WAV/MP3/OGG are supported.</param>
        /// <returns></returns>
        public static Sound CreateSound(string path) {
            if (!isAudioInitialized) throw new InvalidOperationException("Audio is not initialized");
            return new Sound(path);
        }

        /// <summary>
        /// Create a new music.<br/>
        /// Music is better suited for playing long background music.<br/>
        /// Note: while you can create multiple music objects, Only one music can be played at the same time.
        /// </summary>
        /// <param name="path">Path to the sound file. WAV/MP3/OGG are supported.</param>
        /// <returns></returns>
        public static Music CreateMusic(string path) {
            if (!isAudioInitialized) throw new InvalidOperationException("Audio is not initialized");
            return new Music(path);
        }
    }
}