using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Items
{
    public class ItemCoal : Item
    {
        public ItemCoal() : base(){
            SetName("Coal").SetDescription("Useful to those who like fire.").SetID(3).SetValue(7).SetAsset("coal").SetBlockID(2);
        }

        public override void OnItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
