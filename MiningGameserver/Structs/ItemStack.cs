using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameserver.Structs
{
    public struct ItemStack
    {
        public int NumberItems;
        public byte ItemID;

        public ItemStack(int number = 0, byte id = 0)
        {
            NumberItems = number;
            ItemID = id;
        }
    }
}
