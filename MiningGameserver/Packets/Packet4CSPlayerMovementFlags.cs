namespace MiningGameServer.Packets
{
    public class Packet4CSPlayerMovementFlags : Packet
    {
        public Packet4CSPlayerMovementFlags(byte flag)
        {
            WriteByte(4);
            WriteByte(flag);
        }
    }

    public enum PlayerMovementFlag
    {
        Left_Pressed = 1,
        Right_Pressed = 2,
        Jump_Pressed = 4,
        Idle = 8,
        Attack_Pressed = 16,
        Sprinting = 32
    }
}
