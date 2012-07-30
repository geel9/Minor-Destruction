using System.Linq;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using MiningGameServer.ExtensionMethods;
using MiningGameServer.Packets;
using MiningGameServer;

namespace MiningGameServer
{
    public class ServerNetworkManager
    {
        public NetServer NetServer = null;

        public int NumNetworkPlayers = 0;

        public ServerNetworkManager()
        {

        }

        public bool Host(int port)
        {
            try
            {
                NetPeerConfiguration np = new NetPeerConfiguration("MinorDestruction");
                np.Port = port;
                NetServer = new NetServer(np);
                NetServer.Start();
            }
            catch (NetException e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        public void Update()
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
                            NetworkPlayer player = new NetworkPlayer((byte)++NumNetworkPlayers, msg.SenderConnection, new Vector2(100, 70), "");
                            GameServer.NetworkPlayers.Add(player);
                            HandleClient(player);
                        }

                        if (status == NetConnectionStatus.Disconnected)
                        {
                            NetworkPlayer pl2 = GameServer.NetworkPlayers.Where(pl => pl.NetConnection.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier).FirstOrDefault();
                            GameServer.NetworkPlayers.Remove(pl2);
                            Packet1SCGameEvent p2 = new Packet1SCGameEvent(GameServer.GameEvents.Player_Leave, pl2.PlayerID);
                            SendPacket(p2);
                            Packet1SCGameEvent p3 = new Packet1SCGameEvent(GameServer.GameEvents.Player_Chat, (byte)0, false, "Player " + pl2.PlayerName + " has left the game.");
                            SendPacket(p3);

                            ServerConsole.Log("Player " + pl2.PlayerName + " has disconnected.");
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
            byte[] data = p.GetData();
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
            byte num = p.ReadByte();
            switch (num)
            {
                case 0:
                    string name = p.ReadString();
                    player.PlayerName = name;
                    Packet255SCConnectionFirmed packet2 = new Packet255SCConnectionFirmed(player.PlayerID);
                    SendPacket(packet2, player.NetConnection);
                    foreach (NetworkPlayer pl in GameServer.NetworkPlayers)
                    {
                        var packet = new Packet0SCPlayerConnect(pl.PlayerName, pl.PlayerID, pl.EntityPosition);
                        SendPacket(packet, player.NetConnection);

                        if (pl == player) continue;

                        packet = new Packet0SCPlayerConnect(player.PlayerName,
                                                            player.PlayerID, player.EntityPosition);
                        SendPacket(packet, pl.NetConnection);
                    }
                    ServerConsole.Log("New player \"" + name + "\" connected.");
                    break;
                //A game event, JC!
                case 1:
                    byte eventID = p.ReadByte();
                    GameServer.HandleGameEvent(eventID, p, player);
                    break;

                case 2:
                    int pX = p.ReadInt();
                    int pY = p.ReadInt();
                    GameServer.SetBlock(pX, pY, 0, true, 0);
                    break;

                case 4:
                    byte flags = p.ReadByte();
                    if (player.MovementFlags != flags)
                    {
                        player.UpdateMask |= (int)PlayerUpdateFlags.Player_Update;
                        player.UpdateMask |= (int)PlayerUpdateFlags.Player_Movement_Flags;
                    }
                    player.MovementFlags = flags;
                    break;

                case 5:
                    flags = p.ReadByte();
                    float angle = p.ReadShort().DToR();
                    if (player.MovementFlags != flags || angle != player.PlayerAimAngle)
                    {
                        player.UpdateMask |= (int)PlayerUpdateFlags.Player_Update;
                        player.UpdateMask |= (int)PlayerUpdateFlags.Player_Movement_Flags;
                    }
                    player.MovementFlags = flags;
                    player.PlayerAimAngle = angle;
                    break;
            }
        }

        public void HandleClient(NetworkPlayer player)
        {
        }
    }
}
