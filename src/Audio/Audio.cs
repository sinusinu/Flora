namespace Flora {
    public static class Audio {
        
        public static Sound CreateSound(string path) {
            return new Sound(path);
        }

        public static Music CreateMusic(string path) {
            return new Music(path);
        }
    }
}