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

        public ItemStack(int n = 0, byte b = 0)
        {
            NumberItems = n;
            ItemID = b;
        }
    }
}
