using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using Microsoft.Xna.Framework;
using MiningGame.Code.Entities;
using System.Threading;
namespace MiningGame.Code.Managers
{
    public class MusicManager : Manager
    {

        public Sound Sound;
        public string SongName;
        public MusicManager()
        {
            Sound = new Sound();
            base.AddToList();
        }

        public void Play()
        {
            Sound.Play();
        }

        public void Pause()
        {
            Sound.Pause();
        }

        public override void Update(GameTime time)
        {
            // if (Main.PauseManager.paused && sound.Status == SoundStatus.Playing)
            //      sound.Pause();
            //  if (!Main.PauseManager.paused && sound.Status == SoundStatus.Paused)
            //      sound.Play();

            base.Update(time);
        }




        public void Stop()
        {
            Sound.Stop();
        }

        public void SetSong(string key)
        {
            if (Sound.Status == SoundStatus.Playing)
                Sound.Stop();
            Sound = AssetManager.GetSound(key);
            Sound.PlayingOffset = 0f;
            Sound.Loop = false;
        }



        public void SetPlayingOffset(float offset)
        {
            Sound.PlayingOffset = offset;
        }



    }
}
