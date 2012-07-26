using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Blocks
{
    public class BlockRock : Block
    {
        public BlockRock(): base()
        {
            this.SetBlockID(2).SetBlockName("Rock").SetBlockMaxDamage(3);
        }
    }
}
