using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Items
{
    class ItemSword : Item
    {
        public ItemSword():
            base()
        {
            SetName("Sword").SetDescription("Shawing!").SetID(201).SetValue(1).SetAsset("sword");
        }

        public override void OnItemUsed(int x, int y)
        {
        }
    }
}
