using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Packets
{
    public class Packet10SCItemDeleted : Packet
    {
        public Packet10SCItemDeleted(short droppedItemID)
        {
            WriteByte(10);
            WriteShort(droppedItemID);
        }
    }
}
