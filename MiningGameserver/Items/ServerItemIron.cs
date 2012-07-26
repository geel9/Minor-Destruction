using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Items
{
    public class ServerItemIron : MiningGameServer.Items.ServerItem
    {
        public ServerItemIron() : base()
        {
            SetName("Iron").SetDescription("I am iron man").SetValue(7).SetID(4).SetBlockID(3);
        }
    }
}
