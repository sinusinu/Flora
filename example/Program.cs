using System;
using Flora;

namespace FloraExample {
    class Program {
        static void Main(string[] args) {
            ApplicationConfiguration config = new ApplicationConfiguration();
            config.windowFlags = ApplicationConfiguration.FloraWindowFlags.Shown | ApplicationConfiguration.FloraWindowFlags.Resizable;
            config.width = 1280;
            config.height = 720;
            
            BasicCore core = new BasicCore();
            //TextureCore core = new TextureCore();
            //InputCore core = new InputCore();
            //AudioCore core = new AudioCore();
            //ViewCore core = new ViewCore();
            //ControllerCore core = new ControllerCore();
            //FontCore core = new FontCore();
            
            new FloraApplication(core, config);
        }
    }
}
