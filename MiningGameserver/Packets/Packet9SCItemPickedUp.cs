using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Packets
{
    public class Packet9SCItemPickedUp : Packet
    {
        public Packet9SCItemPickedUp(short droppedItemID, byte pickerID)
        {
            WriteByte(9);
            WriteShort(droppedItemID);
            WriteByte(pickerID);
        }
    }
}
