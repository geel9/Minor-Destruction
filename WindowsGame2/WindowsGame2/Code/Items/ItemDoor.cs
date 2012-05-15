using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Entities;
using MiningGame.Code.Server;
using Microsoft.Xna.Framework.Graphics;
namespace MiningGame.Code.Items
{
    public class ItemDoor : Item
    {
        public ItemDoor(): base()
        {
            setName("Door").setDescription("Doorange you glad I didn't say banana?").setID(5).setValue(1).setAsset("door_open");
        }
        public override void onItemUsed(int x, int y)
        {
            byte blockID = GameServer.GetBlockIDAt(x, y);
            byte blockUpID = GameServer.GetBlockIDAt(x, y - 1);
            byte blockDownID = GameServer.GetBlockIDAt(x, y + 1);
            if (blockID != 0) return;

            if (blockUpID != 0 && blockDownID == 0)
            {
                GameServer.SetBlock(x, y, 11);
                GameServer.SetBlock(x, y + 1, 11);
            }
            if (blockDownID != 0 && blockUpID == 0)
            {
                GameServer.SetBlock(x, y, 11);
                GameServer.SetBlock(x, y - 1, 11);
            }
            //throw new NotImplementedException();
        }
    }
}
