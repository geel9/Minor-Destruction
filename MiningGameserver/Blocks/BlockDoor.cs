using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer;
using MiningGameServer.ExtensionMethods;

namespace MiningGameServer.Blocks
{
    public class BlockDoor : Block
    {
        public BlockDoor()
        {
            SetBlockID(4).SetBlockName("Door").SetBlockPistonable(false).SetBlockMaxDamage(10);
        }

        public override Rectangle GetBlockBoundBox(int x, int y)
        {
            Vector2 pos = new Vector2(x * GameServer.BlockSize, y * GameServer.BlockSize);
            byte md = GameServer.GetBlockAt(x, y).MetaData;
            if (!md.BitSet(1))
                return new Rectangle((int)pos.X, (int)pos.Y, 7, GameServer.BlockSize);
            //It's open, so you can walk through it.
            return new Rectangle(0, 0, 0, 0);
        }
        public override void OnBlockUsed(int x, int y, NetworkPlayer user)
        {
            bool above = GameServer.GetBlockAt(x, y - 1).ID == 4;
            byte metaData = GameServer.GetBlockAt(x, y).MetaData;
            bool open = metaData.BitSet(1);
            if (open)
            {
                metaData = metaData.SetBit(1, false);
                GameServer.SetBlockMetaData(x, y, metaData);
                GameServer.SetBlockMetaData(x, y - (above ? 1 : -1), metaData);
                return;
            }
            metaData = metaData.SetBit(1, true);
            GameServer.SetBlockMetaData(x, y, metaData);
            GameServer.SetBlockMetaData(x, y - (above ? 1 : -1), metaData);
        }

        public override void OnBlockRemoved(int x, int y)
        {
            bool above = GameServer.GetBlockAt(x, y - 1).ID == 4;
            bool below = GameServer.GetBlockAt(x, y + 1).ID == 4;
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
