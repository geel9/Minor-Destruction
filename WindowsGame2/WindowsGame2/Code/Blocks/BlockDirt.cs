using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Managers;

namespace MiningGame.Code.Blocks
{
    public class BlockDirt : Block
    {
        public BlockDirt()
            : base()
        {
            this.SetBlockRenderSpecial(true).SetBlockID(2).SetBlockName("Dirt");
        }

        public override BlockRenderer RenderBlock(int x, int y, Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            return new BlockRenderer(AssetManager.GetTexture("dirt"));
        }
    }
}
