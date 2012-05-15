using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Packets;
using MiningGame.Code.Structs;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework;
namespace MiningGame.Code.Packets
{
    public class Packet0SCPlayerConnect : Packet
    {
        public Packet0SCPlayerConnect(string name, int playerID, Vector2 playerPos)
        {
            writeByte((byte)0);
            writeString(name);
            writeInt(playerID);
            writeInt((int)playerPos.X);
            writeInt((int)playerPos.Y);
        }
    }
}