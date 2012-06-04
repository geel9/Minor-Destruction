﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Items
{
    public class ItemBow : Item
    {
        public ItemBow():
            base()
        {
            SetName("Bow").SetDescription("Broiiing").SetID(200).SetValue(1).SetAsset("bow");
        }

        public override void OnItemUsed(int x, int y)
        {
        }
    }
}
