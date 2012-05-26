namespace MiningGameServer.Packets
{
    public class Packet5CSPlayerMovementFlagsAndAim : Packet
    {
        public Packet5CSPlayerMovementFlagsAndAim(byte flag, short aim)
        {
            writeByte(5);
            writeByte(flag);
            writeShort(aim);
        }
    }
}
