using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Managers;

namespace MiningGame.Code.Blocks
{
    public class BlockFlag : Block
    {
        public BlockFlag()
        {
            SetBlockID(9001).SetBlockName("Flag");
        }

        public override BlockRenderer RenderBlock(int x, int y, Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            BlockData data = GameWorld.GetBlockAt(x, y);
            string asset = "redflag";
            if (data.MetaData == 0) asset = "blueflag";
            return new BlockRenderer(AssetManager.GetTexture(asset));
        }
    }
}
