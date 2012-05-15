using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
using MiningGame.Code.Structs;
using Microsoft.Xna.Framework;

namespace MiningGame.Code
{
    public class Animateable
    {
        public Animation curAnimation = new Animation();

        public bool looping = false;
        public bool playing = false;
        private int loopingStart = 0;
        private int loopingEnd = 0;
        public int currentFrame = 0;
        private int frameRateLimit = 0;

        public void setAnimation(Animation a)
        {
            curAnimation = a;
            startLooping(0, a.numberFrames);
            playing = true;
        }

        public void AnimationUpdate()
        {
            if (!playing) return;
            if (frameRateLimit++ <= curAnimation.frames[currentFrame].delay) return;
            currentFrame++;
            frameRateLimit = 0;
            if (looping)
            {
                if (currentFrame > loopingEnd) currentFrame = loopingStart;
            }
            if (currentFrame >= curAnimation.numberFrames) currentFrame = 0;
        }

        public Texture2D getCurrentFrame()
        {
            return curAnimation.numberFrames > 0 ? curAnimation.getFrame(currentFrame).frameTexture() : AssetManager.GetTexture("error");
        }

        public void start()
        {
            playing = true;
        }

        public void stop()
        {
            playing = false;
        }

        public void gotoAndStart(int frame)
        {
            currentFrame = frame;
            playing = true;
        }

        public void gotoAndStop(int frame)
        {
            currentFrame = frame;
            playing = false;
        }

        public AnimationControlPoint getControlPoint(string name)
        {
            List<AnimationControlPoint> cps = curAnimation.frames[currentFrame].controlPoints;
            if (cps == null) cps = new List<AnimationControlPoint>();
            Texture2D frame = getCurrentFrame();
            foreach (AnimationControlPoint cp in cps)
            {
                if (cp.name == name)
                {
                    return cp;
                }
            }
            return null;
        }

        public int indexOfFrame(string name)
        {
            AnimationFrame frame = curAnimation.getFrame(name);
            if (frame != null)
            {
                return curAnimation.frames.IndexOf(frame);
            }
            return -1;
        }

        public void startLooping(int start, int end)
        {
            looping = true;
            loopingStart = start;
            loopingEnd = end;
        }

        public void startLooping(string start, string end)
        {

            int s = indexOfFrame(start);
            int e = indexOfFrame(end);
            if (s != -1 && e != -1)
            {
                loopingStart = s;
                loopingEnd = e;
                looping = true;
                playing = true;
            }
        }

        public void stopLooping()
        {
            loopingStart = 0;
            loopingEnd = 0;
            looping = false;
            playing = false;
        }
    }
}
