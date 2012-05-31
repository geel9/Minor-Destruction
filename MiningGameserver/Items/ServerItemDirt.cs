namespace MiningGameServer.Items
{
    public class ServerItemDirt : MiningGameserver.Items.ServerItem
    {
        public ServerItemDirt() : base()
        {
            SetName("Dirt").SetDescription("Dirty boy!").SetID(1).SetValue(1).SetBlockID(2);
        }
    }
}
