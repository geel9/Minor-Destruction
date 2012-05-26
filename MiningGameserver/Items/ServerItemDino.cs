namespace MiningGameServer.Items
{
    public class ServerItemDino : MiningGameserver.Items.ServerItem
    {
        public ServerItemDino()
            : base()
        {
            SetName("Dinosaur").SetDescription("Open the door!").SetID(7).SetValue(1).SetBlockID(201);
        }
        public override void OnItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
