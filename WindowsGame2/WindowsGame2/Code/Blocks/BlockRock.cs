using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Managers;

namespace MiningGame.Code.Blocks
{
    public class BlockRock : Block
    {
        public BlockRock() : base()
        {
            this.SetBlockRenderSpecial(true).SetBlockID(3).SetBlockName("Rock");
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D RenderBlock(int x, int y, Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            return AssetManager.GetTexture("rock");
        }
    }


}
