namespace MiningGameServer.Items
{
    public class ServerItemPlank : MiningGameServer.Items.ServerItem
    {
        public ServerItemPlank()
            : base()
        {
            SetName("Plank").SetDescription("Planks!").SetID(6).SetValue(1).SetBlockID(0);
        }
    }
}
