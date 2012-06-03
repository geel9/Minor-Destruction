using MiningGameserver.Blocks;

namespace MiningGameserver.Blocks
{
    public class BlockDirt : Block
    {
        public BlockDirt() : base()
        {
           this.SetBlockID(2).SetBlockName("Dirt");
        }
    }
}
