namespace MiningGameServer.Items
{
    public class ServerItemDoor : MiningGameserver.Items.ServerItem
    {
        public ServerItemDoor(): base()
        {
            SetName("Door").SetDescription("Doorange you glad I didn't say banana?").SetID(5).SetValue(1);
        }
        public override void OnItemUsed(int x, int y)
        {
            byte blockID = GameServer.GetBlockIDAt(x, y);
            byte blockUpID = GameServer.GetBlockIDAt(x, y - 1);
            byte blockDownID = GameServer.GetBlockIDAt(x, y + 1);
            if (blockID != 0) return;

            if (blockUpID != 0 && blockUpID != 11 && blockDownID == 0)
            {
                GameServer.SetBlock(x, y, 11);
                GameServer.SetBlock(x, y + 1, 11);
            }
            if (blockDownID != 0 && blockDownID != 11 && blockUpID == 0)
            {
                GameServer.SetBlock(x, y, 11);
                GameServer.SetBlock(x, y - 1, 11);
            }
            //throw new NotImplementedException();
        }
    }
}
