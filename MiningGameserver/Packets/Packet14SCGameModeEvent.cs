using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Packets
{
    /// <summary>
    /// Used to allow gamemodes to send packets without having to specify their own GameEvents or packet IDs
    /// </summary>
    public class Packet14SCGameModeEvent : Packet
    {
        public Packet14SCGameModeEvent(string name, params object[] data)
        {
            WriteByte(14);
            WriteString(name);
            WriteAll(data);
        }
    }
}
