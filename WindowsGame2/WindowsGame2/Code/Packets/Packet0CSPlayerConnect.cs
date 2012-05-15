using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Packets;
using MiningGame.Code.Structs;
using MiningGame.Code.Managers;
namespace MiningGame.Code.Packets
{
    public class Packet0CSPlayerConnect : Packet
    {
        public Packet0CSPlayerConnect(string name)
        {
            writeByte((byte)0);
            writeString(name);
        }
    }
}