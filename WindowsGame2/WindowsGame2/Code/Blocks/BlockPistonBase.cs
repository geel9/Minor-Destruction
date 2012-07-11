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
    public class BlockPistonBase : Block
    {
        public BlockPistonBase()
        {
            SetBlockID(5).SetBlockName("Piston Base").SetBlockRenderSpecial(true);
        }

        public override BlockRenderer RenderBlock(int x, int y, Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            string textureString = "pistonbase";
            byte flags = GameWorld.GetBlockAt(x, y).MetaData;
            if ((flags & (int)PistonFlags.Open) != 0)
            {
                textureString = "pistonbase_open";
            }

            Texture2D Texture = AssetManager.GetTexture(textureString);
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
