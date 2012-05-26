namespace MiningGameServer.Packets
{
    public class Packet4CSPlayerMovementFlags : Packet
    {
        public Packet4CSPlayerMovementFlags(byte flag)
        {
            writeByte(4);
            writeByte(flag);
        }
    }

    public enum PlayerMovementFlag
    {
        Left_Pressed = 1,
        Right_Pressed = 2,
        Jump_Pressed = 4,
        Attack_Pressed = 8
    }
}
