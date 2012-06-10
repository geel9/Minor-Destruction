using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.Packets;

namespace MiningGameServer.Packets
{
    public class Packet7SCPlayerCurItemChanged : Packet
    {
        public Packet7SCPlayerCurItemChanged(byte playerID, byte itemID)
        {
            WriteByte(7);
            WriteByte(playerID);
            WriteByte(itemID);
        }
    }
}
