using System;

namespace Flora.Util {
    /// <summary>
    /// Helper class for generating file paths.
    /// </summary>
    public static class PathUtils {
        /// <summary>
        /// Convert executable-relative path to accessible absolute path. This function does not get affected by working directory.
        /// </summary>
        /// <param name="path">executable-relative path</param>
        /// <returns>absolute path</returns>
        public static string Relative(string path) {
            return System.IO.Path.Join(AppContext.BaseDirectory, path);
        }
    }
}