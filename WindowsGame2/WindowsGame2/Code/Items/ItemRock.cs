using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Items
{
    public class ItemRock : Item
    {
        public ItemRock() : base(){
            setName("Rock").setDescription("UGH BLUGH.").setID(2).setValue(2).setBlockID(3).setAsset("itemRock");
        }
        public override void onItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
