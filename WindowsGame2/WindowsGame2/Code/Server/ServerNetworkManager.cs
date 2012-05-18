using System.Linq;
using MiningGame.Code.Structs;
using MiningGame.Code.Managers;
using MiningGame.Code.Packets;
using Microsoft.Xna.Framework;
using Lidgren.Network;
namespace MiningGame.Code.Server
{
    public class ServerNetworkManager
    {
        public NetServer NetServer = null;

        public int NumNetworkPlayers = 0;

        public ServerNetworkManager()
        {

        }

        public void Host(int port)
        {
            NetPeerConfiguration np = new NetPeerConfiguration("MinorDestruction");
            np.Port = port;
            NetServer = new NetServer(np);
            NetServer.Start();
        }
        public void Update(GameTime thetime)
        {
            if (NetServer == null) return;
            NetIncomingMessage msg;
            while ((msg = NetServer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            //
                            // A new player just connected!
                            //
                            ConsoleManager.Log("Client " + (++NumNetworkPlayers));
                            NetworkPlayer player = new NetworkPlayer((byte)NumNetworkPlayers, msg.SenderConnection, new Microsoft.Xna.Framework.Vector2(100, 70), "");
                            GameServer.NetworkPlayers.Add(player);
                            handleClient(player);
                        }

                        if (status == NetConnectionStatus.Disconnected)
                        {
                            NetworkPlayer pl2 = GameServer.NetworkPlayers.Where(pl => pl.NetConnection.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier).FirstOrDefault();
                            GameServer.NetworkPlayers.Remove(pl2);
                            Packet1SCGameEvent p2 = new Packet1SCGameEvent(GameServer.GameEvents.Player_Leave, pl2.PlayerEntity.PlayerID);
                            SendPacket(p2);
                            Packet1SCGameEvent p3 = new Packet1SCGameEvent(GameServer.GameEvents.Player_Chat, (byte)0, false, "Player " + pl2.PlayerEntity.PlayerName + " has left the game.");
                            SendPacket(p3);
                        }

                        break;
                    case NetIncomingMessageType.Data:
                        NetworkPlayer p = GameServer.NetworkPlayers.Where(pl => pl.NetConnection == msg.SenderConnection).FirstOrDefault();
                        HandlePacket(new Packet(msg.m_data), p);
                        break;
                }
                NetServer.Recycle(msg);
            }
            
        }

        public void SendPacket(Packet p, NetConnection stream = null, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered)
        {
            byte[] data = p.getData();
            try
            {
                if (stream == null)
                {
                    foreach (NetworkPlayer pl in GameServer.NetworkPlayers)
                    {
                        NetOutgoingMessage msg = NetServer.CreateMessage();
                        msg.Write(data);
                        NetConnection clientStream = pl.NetConnection;
                        NetServer.SendMessage(msg, clientStream, method);
                    }
                }
                else if (stream != null)
                {
                    NetOutgoingMessage msg = NetServer.CreateMessage();
                    msg.Write(data);
                    NetServer.SendMessage(msg, stream, method);
                }
            }
            catch
            {
            }
        }

        public void HandlePacket(Packet p, NetworkPlayer player)
        {
            byte num = p.readByte();
            switch (num)
            {
                case 0:
                    string name = p.readString();
                    player.PlayerEntity.PlayerName = name;
                    Packet255SCConnectionFirmed packet2 = new Packet255SCConnectionFirmed(player.PlayerEntity.PlayerID);
                    SendPacket(packet2, player.NetConnection);
                    foreach (NetworkPlayer pl in GameServer.NetworkPlayers)
                    {
                        if (player.PlayerEntity != null)
                        {
                            var packet = new Packet0SCPlayerConnect(pl.PlayerEntity.PlayerName, pl.PlayerEntity.PlayerID, pl.Position);
                            SendPacket(packet, player.NetConnection);
                        }
                        if(pl != player)
                        {
                            var packet = new Packet0SCPlayerConnect(player.PlayerEntity.PlayerName,
                                                                    player.PlayerEntity.PlayerID, player.Position);
                            SendPacket(packet, pl.NetConnection);
                        }
                    }
                    break;
                //A game event, JC!
                case 1:
                    byte eventID = p.readByte();
                    GameServer.HandleGameEvent(eventID, p, player);
                    break;

                case 2:
                    int pX = p.readInt();
                    int pY = p.readInt();
                    GameServer.SetBlock(pX, pY, 0, true, 0);
                    break;
            }
        }

        public void handleClient(NetworkPlayer player)
        {
            for (int x = 0; x < GameServer.WorldSizeX; x++)
            {
                for (int y = 0; y < GameServer.WorldSizeY; y++)
                {
                    Packet1SCGameEvent pack = new Packet1SCGameEvent((byte)GameServer.GameEvents.Block_Set, x, y, (byte)GameServer.WorldBlocks[x, y], (byte)GameServer.WorldBlocksMetaData[x, y]);
                    SendPacket(pack, player.NetConnection, NetDeliveryMethod.ReliableUnordered);
                }
            }
        }
    }
}
