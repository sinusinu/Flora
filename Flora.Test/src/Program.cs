using Flora;

namespace FloraTest;

class Program {
    static void Main(string[] args) {
        Application.Run(new TestCore(), new Config() {
            // VSync = Config.VSyncOpts.Disabled
        });
    }
}