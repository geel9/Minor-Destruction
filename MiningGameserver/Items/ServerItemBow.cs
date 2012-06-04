using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameserver.Items
{
    public class ServerItemBow : ServerItem
    {
        public ServerItemBow()
            :base()
        {
            SetName("Bow").SetDescription("Broiiing").SetID(200).SetValue(1);
        }
    }
}
