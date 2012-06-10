using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.Packets;

namespace MiningGameServer.Interfaces
{
    public interface INetTransferable<T>
    {
        void Write(Packet p);
        T Read(Packet p);
    }
}
