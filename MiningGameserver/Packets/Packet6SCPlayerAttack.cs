using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.Packets;

namespace MiningGameserver.Packets
{
    public class Packet6SCPlayerAttack : Packet
    {
        public Packet6SCPlayerAttack(byte playerID)
        {
            writeByte(6);
            writeByte(playerID);
        }
    }
}
