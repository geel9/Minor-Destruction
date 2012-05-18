using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
using MiningGame.Code.Structs;

namespace MiningGame.Code
{
    public class Animation
    {
        public List<AnimationFrame> frames
        {
            get;
            set;
        }

        internal int numberFrames
        {
            get
            {
                return frames.Count;
            }
            
        }

        public Animation()
        {
            frames = new List<AnimationFrame>();
        }

        public void AddFrame(string assetName, string name = "")
        {
            frames.Add(new AnimationFrame(name, assetName, 2));
        }

        public AnimationFrame getFrame(int frame)
        {
            return frame < numberFrames ? frames[frame] : null;
        }

        public AnimationFrame getFrame(string frame)
        {
            IEnumerable<AnimationFrame> query = frames.Where(x => x.frameName == frame);
            return query.Count() > 0 ? query.FirstOrDefault() : null;
        }
    }

    public class AnimationFrame
    {
        public Texture2D frameTexture()
        {
                return AssetManager.GetTexture(assetName);
        }
        public string frameName { get; set; }
        public string assetName { get; set; }
        public int delay { get; set; }
        public List<AnimationControlPoint> controlPoints { get; set; }

        public AnimationFrame(string s, string asset, int delay)
        {
            frameName = s;
            assetName = asset;
            this.delay = delay;
            controlPoints = new List<AnimationControlPoint>();
        }
        public AnimationFrame()
        {

        }
    }
}
