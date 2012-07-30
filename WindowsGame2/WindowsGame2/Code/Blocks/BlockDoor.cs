using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
using MiningGame.ExtensionMethods;

namespace MiningGame.Code.Blocks
{
    public class BlockDoor : Block
    {
        public BlockDoor()
        {
            SetBlockID(4).SetBlockName("Door").SetBlockRenderSpecial(true);
        }

        private Texture2D GetTexture(int x, int y)
        {
            byte md = GameWorld.GetBlockAt(x, y).MetaData;
            bool above = GameWorld.GetBlockAt(x, y - 1).ID == 4;
            string doorName = "door" + (!above ? "top" : "");
            doorName += (md.BitSet(1)) ? "_open" : "";
            return AssetManager.GetTexture(doorName);
        }

        public override BlockRenderer RenderBlock(int x, int y, SpriteBatch sb)
        {
            return new BlockRenderer(GetTexture(x, y));
        }
    }
}
