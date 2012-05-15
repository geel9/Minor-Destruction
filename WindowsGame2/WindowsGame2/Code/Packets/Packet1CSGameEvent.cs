using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Packets;
using MiningGame.Code.Structs;
using MiningGame.Code.Managers;
using MiningGame.Code.Server;
namespace MiningGame.Code.Packets
{
    public class Packet1CSGameEvent : Packet
    {
        public Packet1CSGameEvent(byte gameEventID, params object[] eventParams)
        {
            writeByte((byte)1);
            writeByte(gameEventID);
            writeAll(eventParams);
        }

        public Packet1CSGameEvent(GameServer.GameEvents gameEventID, params object[] eventParams)
        {
            writeByte((byte)1);
            writeByte((byte)gameEventID);
            writeAll(eventParams);
        }
    }
}