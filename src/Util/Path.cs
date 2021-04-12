using System;

namespace Flora.Util {
    public interface Path {
        string Get();
    }

    public class AbsolutePath : Path {
        private string path;

        public AbsolutePath(string path) {
            this.path = path;
        }

        string Path.Get() {
            return path;
        }
    }

    public class RelativePath : Path {
        private static string executablePath = null;
        private string path;

        public RelativePath(string path) {
            if (executablePath == null) {
                executablePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            }

            this.path = System.IO.Path.Join(executablePath, path);
        }

        string Path.Get() {
            return path;
        }
    }
}