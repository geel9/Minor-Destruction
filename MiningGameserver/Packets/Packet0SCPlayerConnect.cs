
using Microsoft.Xna.Framework;
namespace MiningGameServer.Packets
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