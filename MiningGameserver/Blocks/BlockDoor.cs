using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer;

namespace MiningGameserver.Blocks
{
    public class BlockDoor : Block
    {
        public BlockDoor()
        {
            SetBlockID(11).SetBlockName("Door").SetBlockPistonable(false);
        }

        public override Rectangle GetBlockBoundBox(int x, int y)
        {
            Vector2 pos = new Vector2(x * GameServer.BlockWidth, y * GameServer.BlockHeight);
            byte md = GameServer.GetBlockAt(x, y).MetaData;
            if (md != 1)
                return new Rectangle((int)pos.X, (int)pos.Y, 7, GameServer.BlockHeight);
            //It's open, so you can walk through it.
            return new Rectangle(0, 0, 0, 0);
        }
        public override void OnBlockUsed(int x, int y)
        {
            bool above = GameServer.GetBlockAt(x, y - 1).ID == 11;
            bool open = GameServer.GetBlockAt(x, y).MetaData == 1;
            if (open)
            {
                GameServer.SetBlockMetaData(x, y, 0);
                GameServer.SetBlockMetaData(x, y - (above ? 1 : -1), 0);
                return;
            }
            GameServer.SetBlockMetaData(x, y, 1);
            GameServer.SetBlockMetaData(x, y - (above ? 1 : -1), 1);
        }

        public override void OnBlockRemoved(int x, int y)
        {
            bool above = GameServer.GetBlockAt(x, y - 1).ID == 11;
            bool below = GameServer.GetBlockAt(x, y + 1).ID == 11;
            if (above)
            {
                GameServer.SetBlock(x, y - 1, 0);
            }
            else if (below)
            {
                GameServer.SetBlock(x, y + 1, 0);
            }
        }
    }
}
