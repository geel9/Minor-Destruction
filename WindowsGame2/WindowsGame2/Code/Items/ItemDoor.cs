using MiningGameServer;

namespace MiningGame.Code.Items
{
    public class ItemDoor : Item
    {
        public ItemDoor()
            : base()
        {
            SetName("Door").SetDescription("Doorange you glad I didn't say banana?").SetID(5).SetValue(1).SetAsset("door_open");
        }
        public override void OnItemUsed(int x, int y)
        {
        }
    }
}
