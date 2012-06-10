
namespace MiningGameServer.Packets
{
    public class Packet1CSGameEvent : Packet
    {
        public Packet1CSGameEvent(byte gameEventID, params object[] eventParams)
        {
            WriteByte((byte)1);
            WriteByte(gameEventID);
            WriteAll(eventParams);
        }

        public Packet1CSGameEvent(GameServer.GameEvents gameEventID, params object[] eventParams)
        {
            WriteByte((byte)1);
            WriteByte((byte)gameEventID);
            WriteAll(eventParams);
        }
    }
}