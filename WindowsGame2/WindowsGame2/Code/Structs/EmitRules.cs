using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace MiningGame.Code.Structs
{
    public struct EmitRules
    {
        public double emitDegrees;
        public int interval;
        public float scale;
        public float speed;
        public int particleLifetime;
        public Vector2 range;
        public EmitDirectionMode mode;

        public EmitRules(Vector2 range, int interval, float scale, float speed, EmitDirectionMode mode = EmitDirectionMode.Range, int particleLifetime = 100)
        {
            this.emitDegrees = 0;
            this.range = range;
            this.interval = interval;
            this.scale = scale;
            this.speed = speed;
            this.mode = mode;
            this.particleLifetime = particleLifetime;
        }

        public EmitRules(int interval, float scale, float speed, EmitDirectionMode mode = EmitDirectionMode.Random, int particleLifetime = 100)
        {
            this.emitDegrees = 0;
            this.range = Vector2.Zero;
            this.interval = interval;
            this.scale = scale;
            this.speed = speed;
            this.mode = mode;
            this.particleLifetime = particleLifetime;
        }

        public EmitRules(double degrees, int interval, float scale, float speed, EmitDirectionMode mode = EmitDirectionMode.Fixed, int particleLifetime = 100)
        {
            emitDegrees = degrees;
            this.range = Vector2.Zero;
            this.interval = interval;
            this.speed = speed;
            this.scale = scale;
            this.mode = mode;
            this.particleLifetime = particleLifetime;
        }
    }
    
}
