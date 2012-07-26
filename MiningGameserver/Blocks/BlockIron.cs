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
            SetBlockID(3).SetBlockName("Iron").SetBlockMaxDamage(5);
        }

        public override byte GetItemDrop(int x, int y)
        {
            return 4;
        }

        public override int GetItemDropNum(int x, int y)
        {
            return 1;
            //return base.GetItemDropNum(x, y);
        }
    }
}
