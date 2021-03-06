﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
using MiningGame.ExtensionMethods;

namespace MiningGame.Code.Entities
{
    public class Particle : Entity
    {

        public double degrees = 0;
        public int lifetime = 100;
        public float speed = 1f;
        public bool active = true;


        public Particle(Vector2 position, double degrees, string assetname, float speed, float scale = 1f, int lifetime = 100, bool active = true, byte alpha = 255)
        {
            this.degrees = degrees;
            EntityPosition = position;
            this.active = active;
            base.Alpha = alpha;
            this.speed = speed;
            this.lifetime = lifetime;
            base.Scale = scale;
            Texture2D t = AssetManager.GetTexture(assetname);
            SpriteTexture = t;
            addToList();
        }

        public override void Update(GameTime time, bool serverContext = false)
        {
            if (active && !Main.PauseManager.Paused)
            {
                lifetime--;
                double Radians = degrees.DToR();
                base.Rotation = (float)Radians;
                float X = speed * (float)Math.Sin(Radians);
                float Y = speed * (float)Math.Cos(Radians);
                base.EntityPosition = Vector2.Add(base.EntityPosition, new Vector2(X, Y));
                if (this.EntityPosition.X - CameraManager.cameraPosition.X < 0 || this.EntityPosition.X - CameraManager.cameraPosition.X > Main.graphics.PreferredBackBufferWidth || this.EntityPosition.Y - CameraManager.cameraPosition.Y < 0 || this.EntityPosition.Y - CameraManager.cameraPosition.Y > Main.graphics.PreferredBackBufferHeight || lifetime <= 0)
                {
                    active = false;
                    base.removeFromList();
                }
                base.Update(time);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (active)
                base.Draw(sb);
        }

    }
}
