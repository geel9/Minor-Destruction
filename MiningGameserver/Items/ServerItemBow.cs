using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.ItemAttributes;

namespace MiningGameServer.Items
{
    public class ServerItemBow : ServerItem
    {
        public ServerItemBow()
            :base()
        {
            SetName("Bow").SetDescription("Broiiing").SetID(200).SetValue(1);
        }

        public override ItemAttribute[] GetDefaultAttributes()
        {
            return new ItemAttribute[0];
            return new ItemAttribute[] { ItemAttribute.GetAttributeOfID(1, 25), ItemAttribute.GetAttributeOfID(3, 200), ItemAttribute.GetAttributeOfID(2, 1) };
        }
    }
}
