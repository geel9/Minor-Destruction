using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Items
{
    public class ServerItemIron : MiningGameserver.Items.ServerItem
    {
        public ServerItemIron() : base(){
            SetName("Iron").SetDescription("I am iron man").SetValue(7).SetID(4);
        }
        public override void OnItemUsed(int x, int y, NetworkPlayer user)
        {
            GameServer.SetBlock(x, y, 200);
            //throw new NotImplementedException();
        }
    }
}
