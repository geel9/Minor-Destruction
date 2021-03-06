﻿namespace MiningGameServer.Items
{
    public class ServerItemDoor : ServerItem
    {
        public ServerItemDoor(): base()
        {
            SetName("Door").SetDescription("Doorange you glad I didn't say banana?").SetID(5).SetValue(1);
            InventorySection = 1;
        }
        public override void OnItemUsed(int x, int y, NetworkPlayer player)
        {
            short blockID = GameServer.GetBlockAt(x, y).ID;
            short blockUpID = GameServer.GetBlockAt(x, y - 1).ID;
            short blockDownID = GameServer.GetBlockAt(x, y + 1).ID;
            byte metaData = (byte)player.PlayerTeam;
            if (blockID != 0) return;

            if (blockUpID != 0 && blockUpID != 11 && blockDownID == 0)
            {
                GameServer.SetBlock(x, y, 4, true, metaData);
                GameServer.SetBlock(x, y + 1, 4, true, metaData);
            }
            if (blockDownID != 0 && blockDownID != 11 && blockUpID == 0)
            {
                GameServer.SetBlock(x, y, 4, true, metaData);
                GameServer.SetBlock(x, y - 1, 4, true, metaData);
            }
            //throw new NotImplementedException();
        }
    }
}
