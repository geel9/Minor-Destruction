using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Managers
{
    public class SoundManager : Manager
    {

        public SoundManager()
        {
            base.AddToList();
        }

        public void PlaySound(string key, bool looping = false, int offset = 0)
        {
            // TODO: Fix (requires SFML)
            /*Sound s = AssetManager.GetSound(key);
            if (s != null)
            {
                s.Loop = looping;
                s.PlayingOffset = (float)offset;
                s.Play();
            }
            else
            {
                ConsoleManager.Log("Could not find sound " + key, Microsoft.Xna.Framework.Color.Red);
            }*/
        }

    }
}
