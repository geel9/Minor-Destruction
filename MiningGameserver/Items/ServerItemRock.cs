using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Items
{
    public class ServerItemRock : MiningGameServer.Items.ServerItem
    {
        public ServerItemRock() : base()
        {
            SetName("Rock").SetDescription("UGH BLUGH.").SetID(2).SetValue(2).SetBlockID(3);
        }
    }
}
