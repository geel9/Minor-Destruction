using MiningGameServer;

namespace MiningGame.Code.Items
{
    public class ItemDirt : Item
    {
        public ItemDirt() : base()
        {
            SetName("Dirt").SetDescription("Dirty boy!").SetID(1).SetValue(1).SetAsset("dirt").SetBlockID(2);
        }
        public override void OnItemUsed(int x, int y)
        {
        }
    }
}
