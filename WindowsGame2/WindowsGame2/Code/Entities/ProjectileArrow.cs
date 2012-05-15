using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGame.Code.Managers;
using YogUILibrary.Managers;

namespace MiningGame.Code.Entities
{
    public class ProjectileArrow : EntityProjectile
    {
        public override byte  GetProjectileType()
        {
            return 1;
        }

        public ProjectileArrow(Vector2 Position, float angle, int strength = 10)
            :base ()
        {
            scale = 1;
            EntityPosition = Position;
            float strengthX = (float)(strength * Math.Cos(angle));
            float strengthY = (float)(strength * Math.Sin(angle));
            
            EntityVelocity = new Vector2(strengthX, strengthY);
            _lastPosition = Position - EntityVelocity;

            EffectOfGravity = 0.13f;

            SpriteTexture = AssetManager.GetTexture("ladder");
            alpha = 255;
        }
    }
}
