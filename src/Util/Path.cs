using System;

namespace Flora.Util {
    public static class Path {
        private static string executablePath = null;
        
        public static string Absolute(string path) {
            return path;
        }

        public static string Relative(string path) {
            if (executablePath == null) {
                executablePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            }

            return System.IO.Path.Join(executablePath, path);
        }
    }
}