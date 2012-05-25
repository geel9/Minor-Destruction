using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Structs;

namespace MiningGame.Code.Packets
{
    public class Packet5CSPlayerMovementFlagsAndAim : Packet
    {
        public Packet5CSPlayerMovementFlagsAndAim(byte flag, short aim)
        {
            writeByte(5);
            writeByte(flag);
            writeShort(aim);
        }
    }
}
