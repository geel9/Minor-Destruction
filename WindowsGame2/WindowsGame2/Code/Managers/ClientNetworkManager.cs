using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Structs;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using MiningGame.Code.Interfaces;
using MiningGame.Code.Entities;
using MiningGame.Code.Packets;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace MiningGame.Code.Managers
{
    public class ClientNetworkManager
    {
        public NetClient netClient = null;

        public List<BoundPacket> boundPackets = new List<BoundPacket>();

        public bool isConnected()
        {
            return !(netClient == null) && (netClient.ConnectionStatus == NetConnectionStatus.Connected || netClient.ConnectionStatus == NetConnectionStatus.InitiatedConnect);
        }

        public ClientNetworkManager()
        {
            receive();
        }

        public void Update(GameTime theTime)
        {
            if (netClient == null) return;
            NetIncomingMessage msg;
            while ((msg = netClient.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        ConsoleManager.Log(msg.ReadString());
                        break;
                    case NetIncomingMessageType.Data:
                        handlePacket(new Packet(msg.m_data));
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            OnConnected();
                        }
                        break;
                }
                //netClient.Recycle(msg);
            }
        }

        public void SendPacket(Packet p, NetDeliveryMethod method = NetDeliveryMethod.ReliableOrdered)
        {
            //if (!isConnected()) return;
            byte[] data = p.getData();
            NetOutgoingMessage om = netClient.CreateMessage();
            om.Write(data);
            netClient.SendMessage(om, method);
            //netClient.Recycle(om);
        }

        public void handlePacket(Packet p)
        {
            byte num = p.readByte();
            foreach (BoundPacket bp in boundPackets)
            {
                if (bp.IDToFireOn == num)
                {
                    bp.ToFire(p);
                }
            }
        }

        public void receive()
        {
            boundPackets.Add(new BoundPacket((Packet p) =>
            {
                //A game event
                GameWorld.HandleGameEvent(p.readByte(), p);
            }, 1));

            boundPackets.Add(new BoundPacket((Packet p) =>
            {
                byte type = p.readByte();
                byte ID = p.readByte();
                short X = p.readShort();
                short Y = p.readShort();
                float angle = p.readFloat();
                GameWorld.GameProjectiles.Add(new ProjectileArrow(new Vector2(X, Y), angle) { ProjectileID = ID });
            }, 2));

            boundPackets.Add(new BoundPacket((Packet p) =>
                                                 {
                                                     byte toRemove = p.readByte();
                                                     EntityProjectile proj = null;
                                                     foreach (EntityProjectile projectile in GameWorld.GameProjectiles)
                                                     {
                                                         if (projectile.ProjectileID == toRemove)
                                                         {
                                                             proj = projectile;
                                                             break;
                                                         }
                                                     }
                                                     if (proj != null) GameWorld.GameProjectiles.Remove(proj);
                                                 }, 3));

            boundPackets.Add(new BoundPacket((Packet p) =>
            {
                string pName = p.readString();
                byte id = p.readByte();
                
                int posX = p.readInt();
                int posY = p.readInt();
                if (GameWorld.OtherPlayers.Where(pl => pl.PlayerID == id).Count() > 0) return;
                if (GameWorld.ThePlayer.PlayerEntity.PlayerID != id)
                {
                    ConsoleManager.Log("New player: " + pName + " id: " + id + " x: " + posX + " y: " + posY);
                    GameWorld.OtherPlayers.Add(new PlayerEntity(new Vector2(posX, posY), id, pName));
                }
                else
                {
                    GameWorld.ThePlayer.PlayerEntity.EntityPosition = new Vector2(posX, posY);
                }
            }, 0));

            boundPackets.Add(new BoundPacket(p =>
            {
                byte id = p.readByte();
                ConsoleManager.Log("My id is " + id);
                GameWorld.ThePlayer.PlayerEntity = new PlayerEntity(new Vector2(0, 0), id, ConsoleManager.getVariableValue("player_name"));
            }, 255));

            boundPackets.Add(new BoundPacket(PlayerUpdating, 200));
        }

        public void Connect(string ip, int port)
        {
            netClient = new NetClient(new NetPeerConfiguration("MinorDestruction"));
            netClient.Start();
            ConsoleManager.Log("Connecting to " + ip + ":" + port);

            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                netClient.Connect(serverEndPoint);

            }
            catch (Exception)
            {
                ConsoleManager.Log("Could not connect!", Microsoft.Xna.Framework.Color.Red);
            }
        }

        public void OnConnected()
        {
            Main.theWorld = new GameWorld();
            InGameInterface i = new InGameInterface();
            i.initialize(1);
            string name = ConsoleManager.getVariableValue("player_name");
            ConsoleManager.Log("Connected! Name:" + name);
            Packet0CSPlayerConnect packet = new Packet0CSPlayerConnect(name);
            SendPacket(packet);
        }

        public void Disconnect()
        {
            try
            {
                if (isConnected())
                {
                    netClient.Disconnect("Disconnected.");
                    ConsoleManager.Log("Disconnected from server");
                }
            }
            catch (Exception)
            {
            }
        }

        public void PlayerUpdating(Packet p)
        {
            int numToUpdate = p.readByte();
            for (int i = 0; i < numToUpdate; i++)
            {
                int playerID = p.readByte();
                PlayerEntity player;
                if (playerID == GameWorld.ThePlayer.PlayerEntity.PlayerID)
                    player = GameWorld.ThePlayer.PlayerEntity;
                else
                {
                    player = GameWorld.OtherPlayers.Where(pl => pl.PlayerID == playerID).FirstOrDefault();
                }
                if(player == null) player = new PlayerEntity(Vector2.Zero, (byte)playerID);
                byte updateMask = p.readByte();

                if ((updateMask & (int)PlayerUpdateFlags.Player_Position_X) != 0)
                {
                    short x = p.readShort();
                    player.EntityPosition.X = x;
                }
                if ((updateMask & (int)PlayerUpdateFlags.Player_Position_Y) != 0)
                {
                    short y = p.readShort();
                    player.EntityPosition.Y = y;
                }
                player.FacingLeft = (updateMask & (int)PlayerUpdateFlags.Player_Facing_Left) != 0;
            }
        }
    }
}
