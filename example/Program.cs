using System;
using Flora;

namespace FloraExample {
    class Program {
        static void Main(string[] args) {
            ApplicationConfiguration config = new ApplicationConfiguration();
            
            //BasicCore core = new BasicCore();
            TextureCore core = new TextureCore();
            
            new FloraApplication(core, config);
        }
    }
}
