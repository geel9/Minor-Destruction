using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Packets
{
    public class Packet13SCBlockDamage : Packet
    {
        public Packet13SCBlockDamage(short x, short y, byte newDamage)
        {
            WriteByte(13);
            WriteShort(x);
            WriteShort(y);
            WriteByte(newDamage);
        }
    }
}
