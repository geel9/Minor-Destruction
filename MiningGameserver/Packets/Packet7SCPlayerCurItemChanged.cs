using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.Packets;

namespace MiningGameserver.Packets
{
    public class Packet7SCPlayerCurItemChanged : Packet
    {
        public Packet7SCPlayerCurItemChanged(byte playerID, byte itemID)
        {
            writeByte(7);
            writeByte(playerID);
            writeByte(itemID);
        }
    }
}
