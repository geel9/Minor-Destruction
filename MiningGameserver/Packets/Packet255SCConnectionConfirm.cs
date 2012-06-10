
namespace MiningGameServer.Packets
{
    public class Packet255SCConnectionFirmed : Packet
    {
        public Packet255SCConnectionFirmed(int playerID)
        {
            WriteByte((byte)255);
            WriteInt(playerID);
        }
    }
}