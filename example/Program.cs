using System;
using Flora;

namespace FloraExample {
    class Program {
        static void Main(string[] args) {
            ApplicationConfiguration config = new ApplicationConfiguration();
            
            //BasicCore core = new BasicCore();
            //TextureCore core = new TextureCore();
            //InputCore core = new InputCore();
            AudioCore core = new AudioCore();
            
            new FloraApplication(core, config);
        }
    }
}
