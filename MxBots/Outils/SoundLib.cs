using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrrKlang;


namespace MxBots
{
    public class SoundLib
    {

        public ISoundEngine engine {get;set;}
        
        public SoundLib()
        {
            engine = new ISoundEngine();
        }

        public void play()
        {
            engine.Play2D("Sounds/misc/satell.s3m", true);
        }

    }
}
