using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Packets
{
    public class Packet12SCItemPickedUpIncomplete : Packet
    {
        public Packet12SCItemPickedUpIncomplete(short droppedItemID, byte pickerID)
        {
            WriteByte(12);
            WriteShort(droppedItemID);
            WriteByte(pickerID);
        }
    }
}
