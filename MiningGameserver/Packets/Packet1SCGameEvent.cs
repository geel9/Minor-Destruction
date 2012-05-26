using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace MiningGameServer.Packets
{
    public class Packet1SCGameEvent : Packet
    {
        public Packet1SCGameEvent(byte gameEventID, params object[] eventParams)
        {
            writeByte(1);
            writeByte(gameEventID);
            writeAll(eventParams);
        }
        public Packet1SCGameEvent(GameServer.GameEvents gameEventID, params object[] eventParams)
        {
            writeByte(1);
            writeByte((byte)gameEventID);
            writeAll(eventParams);
        }
    }
}