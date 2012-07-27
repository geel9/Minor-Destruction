using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Blocks;
using MiningGame.Code.Entities;
using MiningGame.Code.Managers;
using MiningGameServer.Player;
using MiningGameServer.Shapes;

namespace MiningGame.Code.PlayerClasses
{
    public class PlayerClassDestroyer : PlayerClass
    {
        public bool PickingUpBlock;

        public short BlockInHand;

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

            if(BlockInHand != 0)
            {
                Block block = Block.GetBlock(BlockInHand);
                BlockRenderer renderer = block.RenderBlock(0, 0, sb);
                sb.Draw(renderer.Texture, Player.EntityPosition - new Vector2(5, 50) - CameraManager.cameraPosition, null, Color.White, renderer.Rotation, renderer.Origin, Vector2.One, renderer.Effects, 0f);
            }

            if(PickingUpBlock)
            {
                sb.DrawString(AssetManager.GetFont("Console"), "Picking up...", Player.EntityPosition - new Vector2(0, 50) - CameraManager.cameraPosition, Color.Black);
            }
        }

        public override void ReadState(MiningGameServer.Packets.Packet p)
        {
            byte state = p.ReadByte();

            if ((state & (byte)DestroyerUpdateFlags.Block_In_Hand) != 0)
            {
                BlockInHand = p.ReadShort();
            }

            if((state & (byte)DestroyerUpdateFlags.Picking_Up_Block) != 0)
            {
                PickingUpBlock = p.ReadBool();
            }
        }
    }
}
