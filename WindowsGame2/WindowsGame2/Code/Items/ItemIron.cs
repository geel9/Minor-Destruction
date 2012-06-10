using MiningGameServer;

namespace MiningGame.Code.Items
{
    public class ItemIron : Item
    {
        public ItemIron() : base()
        {
            SetName("Iron").SetDescription("I am iron man").SetValue(7).SetAsset("iron").SetID(4).SetBlockID(5);
        }
        public override void OnItemUsed(int x, int y)
        {
            GameServer.SetBlock(x, y, 200);
            //throw new NotImplementedException();
        }
    }
}
