
namespace MiningGameServer.Packets
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