using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameserver.Items
{
    public class ServerItemSword : ServerItem
    {
        public ServerItemSword()
            :base()
        {
            SetName("Sword").SetDescription("Shawing!").SetID(201).SetValue(1);
        }
    }
}
