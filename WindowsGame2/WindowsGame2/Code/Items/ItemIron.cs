using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Server;

namespace MiningGame.Code.Items
{
    public class ItemIron : Item
    {
        public ItemIron() : base(){
            setName("Iron").setDescription("I am iron man").setValue(7).setAsset("iron").setID(4);
        }
        public override void onItemUsed(int x, int y)
        {
            GameServer.SetBlock(x, y, 200);
            //throw new NotImplementedException();
        }
    }
}
