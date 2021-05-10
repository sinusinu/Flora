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
        
        /// <summary>
        /// Create a new sound.<br/>
        /// Sound is better suited for playing fire-and-forget type of short sound clips.
        /// </summary>
        /// <param name="path">Path to the sound file. WAV/MP3/OGG are supported.</param>
        /// <returns></returns>
        public static Sound CreateSound(string path, float volume = 1f) {
            if (!isAudioInitialized) throw new InvalidOperationException("Audio is not initialized");
            return new Sound(path, volume);
        }

        /// <summary>
        /// Create a new music.<br/>
        /// Music is better suited for playing long background music.<br/>
        /// Note: while you can create multiple music objects, Only one music can be played at the same time.
        /// </summary>
        /// <param name="path">Path to the sound file. WAV/MP3/OGG are supported.</param>
        /// <returns></returns>
        public static Music CreateMusic(string path, float volume = 1f) {
            if (!isAudioInitialized) throw new InvalidOperationException("Audio is not initialized");
            return new Music(path, volume);
        }
    }
}