using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Items
{
    public class ServerItemCoal : MiningGameServer.Items.ServerItem
    {
        public ServerItemCoal() : base(){
            SetName("Coal").SetDescription("Useful to those who like fire.").SetID(3).SetValue(7).SetBlockID(4);
        }
    }
}
