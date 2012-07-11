using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
using MiningGame.ExtensionMethods;
using MiningGameServer.Blocks;

namespace MiningGame.Code.Blocks
{
    public class BlockPistonArm : Block
    {
        public BlockPistonArm()
            : base()
        {
            this.SetBlockName("Piston arm").SetBlockID(6).SetBlockRenderSpecial(true);
        }

        public override BlockRenderer RenderBlock(int x, int y, Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            byte flags = GameWorld.GetBlockAt(x, y).MetaData;

            Texture2D Texture = AssetManager.GetTexture("pistonarm");
            float rotation = 0f;

            if ((flags & (int)PistonFlags.Right) != 0)
                rotation = 180f.DToR();
            if ((flags & (int)PistonFlags.Up) != 0)
                rotation = 90f.DToR();
            if ((flags & (int)PistonFlags.Down) != 0)
                rotation = 270f.DToR();

            return new BlockRenderer(Texture, rotation);
        }
    }
}
