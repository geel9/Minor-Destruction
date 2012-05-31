using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;

namespace MiningGame.Code.Blocks
{
    public class BlockDoor : Block
    {
        public BlockDoor()
            : base()
        {
            this.SetBlockID(11).SetBlockName("Door").SetBlockRenderSpecial(true);
        }

        private Texture2D GetTexture(int x, int y)
        {
            byte md = GameWorld.GetBlockMDAt(x, y);
            bool above = GameWorld.GetBlockIDAt(x, y - 1) == 11;
            string doorName = "door" + (!above ? "top" : "");
            doorName += (md == 1) ? "_open" : "";
            return AssetManager.GetTexture(doorName);
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D RenderBlock(int x, int y, Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            return GetTexture(x, y);
        }
    }
}
