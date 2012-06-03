using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameserver.Blocks
{
    public class BlockPistonArm : Block
    {
        public BlockPistonArm() :base()
        {
            this.SetBlockName("Piston arm").SetBlockPistonable(false).SetBlockID(21);
        }
    }
}
