namespace MiningGameServer.Items
{
    public class ServerItemDino : MiningGameserver.Items.ServerItem
    {
        public ServerItemDino()
            : base()
        {
            SetName("Dinosaur").SetDescription("Open the door!").SetID(7).SetValue(1).SetBlockID(20);
        }
    }
}
