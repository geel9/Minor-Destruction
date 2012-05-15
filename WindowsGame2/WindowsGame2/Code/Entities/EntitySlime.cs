using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGame.Code.Managers;
using YogUILibrary.Managers;

namespace MiningGame.Code.Entities
{
    class EntitySlime : EntityMoveable
    {
        private int jumpTimer = 0, slimeWidth = 10, slimeHeight = 10;

        public override Microsoft.Xna.Framework.Rectangle BoundBox
        {
            get
            {
                return new Rectangle((int)entityPosition.X - (slimeWidth / 2), (int)entityPosition.Y - (slimeHeight / 2), (slimeWidth), (slimeHeight));
            }
        }

        public override void Update(GameTime time)
        {
            entityMovement();
            if (jumpTimer > 0) jumpTimer--;
            int xDist = (int)(entityPosition.X - GameWorld.thePlayer.playerEntity.entityPosition.X);
            if (Math.Abs(xDist) <= 500 && Math.Abs(xDist) >= 20 && jumpTimer == 0)
            {
                entityVelocity.Y -= 10;
                jumpTimer = 100 + Main.r.Next(0, 100);
                if (xDist > 0) entityVelocity.X -= 10;
                else if (xDist < 0) entityVelocity.X += 10;
            }
            base.Update(time);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            DrawManager.Draw_Box(ConversionManager.PToV(BoundBox.Center) - CameraManager.cameraPosition, BoundBox.Width, BoundBox.Height, Color.Green, sb, 0f, 200);
            base.Draw(sb);
        }
    }
}
