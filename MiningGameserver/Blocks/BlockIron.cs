using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Blocks
{
    public class BlockIron : Block
    {
        public BlockIron()
            : base()
        {
            SetBlockID(3).SetBlockName("Iron");
        }

        public override byte GetItemDrop(int x, int y)
        {
            return 3;
        }

        public override int GetItemDropNum(int x, int y)
        {
            return 2;
            //return base.GetItemDropNum(x, y);
        }
    }
}
