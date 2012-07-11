using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Items
{
    public class ItemRock : Item
    {
        public ItemRock() : base(){
            SetName("Rock").SetDescription("UGH BLUGH.").SetID(2).SetValue(2).SetBlockID(2).SetAsset("itemRock");
        }
        public override void OnItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
