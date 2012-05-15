using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Items
{
    public class ItemCoal : Item
    {
        public ItemCoal() : base(){
            setName("Coal").setDescription("Useful to those who like fire.").setID(3).setValue(7).setAsset("coal").setBlockID(4);
        }

        public override void onItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
