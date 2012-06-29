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
        Player_Position = 2,
        Player_Movement_Flags = 4
    }
}
