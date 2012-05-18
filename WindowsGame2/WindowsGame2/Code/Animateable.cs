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
        public Animation CurAnimation = new Animation();

        public bool Looping = false;
        public bool Playing = false;
        private int _loopingStart = 0;
        private int _loopingEnd = 0;
        public int CurrentFrame = 0;
        private int _frameRateLimit = 0;

        public void SetAnimation(Animation a)
        {
            CurAnimation = a;
            StartLooping(0, a.numberFrames);
            Playing = true;
        }

        public void AnimationUpdate()
        {
            if (!Playing) return;
            if (_frameRateLimit++ <= CurAnimation.frames[CurrentFrame].delay) return;
            CurrentFrame++;
            _frameRateLimit = 0;
            if (Looping)
            {
                if (CurrentFrame > _loopingEnd) CurrentFrame = _loopingStart;
            }
            if (CurrentFrame >= CurAnimation.numberFrames) CurrentFrame = 0;
        }

        public Texture2D GetCurrentFrame()
        {
            return CurAnimation.numberFrames > 0 ? CurAnimation.getFrame(CurrentFrame).frameTexture() : AssetManager.GetTexture("error");
        }

        public void Start()
        {
            Playing = true;
        }

        public void Stop()
        {
            Playing = false;
        }

        public void GotoAndStart(int frame)
        {
            CurrentFrame = frame;
            Playing = true;
        }

        public void GotoAndStop(int frame)
        {
            CurrentFrame = frame;
            Playing = false;
        }

        public AnimationControlPoint GetControlPoint(string name)
        {
            List<AnimationControlPoint> cps = CurAnimation.frames[CurrentFrame].controlPoints;
            if (cps == null) cps = new List<AnimationControlPoint>();
            Texture2D frame = GetCurrentFrame();
            foreach (AnimationControlPoint cp in cps)
            {
                if (cp.name == name)
                {
                    return cp;
                }
            }
            return null;
        }

        public int IndexOfFrame(string name)
        {
            AnimationFrame frame = CurAnimation.getFrame(name);
            if (frame != null)
            {
                return CurAnimation.frames.IndexOf(frame);
            }
            return -1;
        }

        public void StartLooping(int start, int end)
        {
            Looping = true;
            _loopingStart = start;
            _loopingEnd = end;
        }

        public void StartLooping(string start, string end)
        {

            int s = IndexOfFrame(start);
            int e = IndexOfFrame(end);
            if (s != -1 && e != -1)
            {
                _loopingStart = s;
                _loopingEnd = e;
                Looping = true;
                Playing = true;
            }
        }

        public void StopLooping()
        {
            _loopingStart = 0;
            _loopingEnd = 0;
            Looping = false;
            Playing = false;
        }
    }
}
