namespace MiningGameServer.Items
{
    public class ServerItemDino : MiningGameServer.Items.ServerItem
    {
        public ServerItemDino()
            : base()
        {
            SetName("Dinosaur").SetDescription("Open the door!").SetID(7).SetValue(1).SetBlockID(20);
        }
    }
}
