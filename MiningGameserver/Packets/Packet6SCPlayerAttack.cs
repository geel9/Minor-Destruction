using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.Packets;

namespace MiningGameServer.Packets
{
    public class Packet6SCPlayerAttack : Packet
    {
        public Packet6SCPlayerAttack(byte playerID)
        {
            WriteByte(6);
            WriteByte(playerID);
        }
    }
}
