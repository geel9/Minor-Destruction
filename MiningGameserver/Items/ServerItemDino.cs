namespace MiningGameServer.Items
{
    public class ServerItemDino : ServerItem
    {
        public ServerItemDino()
            : base()
        {
            SetName("Piston").SetDescription("Very pistony").SetID(7).SetValue(1).SetBlockID(5);
        }
    }
}
