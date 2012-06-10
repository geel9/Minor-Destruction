using MiningGameServer.Blocks;

namespace MiningGameServer.Blocks
{
    public class BlockDirt : Block
    {
        public BlockDirt() : base()
        {
           this.SetBlockID(2).SetBlockName("Dirt");
        }
        public override byte GetItemDrop(int x, int y)
        {
            return 1;
        }
        public override int GetItemDropNum(int x, int y)
        {
            return 1;
        }
    }
}
