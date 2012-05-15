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
            SetName("Iron").SetDescription("I am iron man").SetValue(7).SetAsset("iron").SetID(4);
        }
        public override void OnItemUsed(int x, int y)
        {
            GameServer.SetBlock(x, y, 200);
            //throw new NotImplementedException();
        }
    }
}
