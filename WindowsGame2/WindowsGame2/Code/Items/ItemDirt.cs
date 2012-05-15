using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Entities;
using MiningGame.Code.Server;
using Microsoft.Xna.Framework.Graphics;
namespace MiningGame.Code.Items
{
    public class ItemDirt : Item
    {
        public ItemDirt() : base(){
            setName("Dirt").setDescription("Dirty boy!").setID(1).setValue(1).setAsset("dirt");
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
