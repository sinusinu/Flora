namespace Flora.Audio {
    public static class AudioFactory {
        
        public static Sound CreateSound(string path) {
            return new Sound(path);
        }

        public static Music CreateMusic(string path) {
            return new Music(path);
        }
    }
}