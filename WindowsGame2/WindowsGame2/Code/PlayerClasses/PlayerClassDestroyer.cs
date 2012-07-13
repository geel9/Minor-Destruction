using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Entities;
using MiningGame.Code.Managers;
using MiningGameServer.Shapes;

namespace MiningGame.Code.PlayerClasses
{
    public class PlayerClassDestroyer : PlayerClass
    {

        public override ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB(0, 0, 24, 48);
            }
        }

        public PlayerClassDestroyer(PlayerEntity thePlayer)
        {
            Player = thePlayer;
        }

        public override void Draw_Post(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Texture2D texture = AssetManager.GetTexture("destroyer");
            Vector2 drawPos = Player.EntityPosition - CameraManager.cameraPosition;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            //origin = Vector2.Zero;
            sb.Draw(texture, drawPos, null, Color.White, 0f, origin, 1f, Player.FacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
    }
}
