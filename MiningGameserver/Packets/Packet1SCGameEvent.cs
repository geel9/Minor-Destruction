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
            WriteByte(1);
            WriteByte(gameEventID);
            WriteAll(eventParams);
        }
        public Packet1SCGameEvent(GameServer.GameEvents gameEventID, params object[] eventParams)
        {
            WriteByte(1);
            WriteByte((byte)gameEventID);
            WriteAll(eventParams);
        }
    }
}