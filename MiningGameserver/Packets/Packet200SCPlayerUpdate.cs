namespace MiningGameServer.Packets
{
    class Packet200SCPlayerUpdate : Packet
    {
        public Packet200SCPlayerUpdate()
        {
            WriteByte(200);
        }
    }

    public enum PlayerUpdateFlags
    {
        Player_Update = 1,
        Player_Position_X = 2,
        Player_Position_Y = 4,
        Player_Movement_Flags = 8
    }
}
