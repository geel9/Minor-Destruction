using System;
using System.Collections.Generic;
using System.Linq;
using MiningGame.Code.Structs;
using System.Net;
using MiningGame.Code.Interfaces;
using MiningGame.Code.Entities;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using MiningGameServer.Packets;

namespace MiningGame.Code.Managers
{
    public class ClientNetworkManager
    {
        public NetClient netClient = null;

        public List<BoundPacket> boundPackets = new List<BoundPacket>();

        public bool isConnected()
        {
            return netClient != null && (netClient.ConnectionStatus == NetConnectionStatus.Connected || netClient.ConnectionStatus == NetConnectionStatus.InitiatedConnect);
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
                byte strength = p.readByte();
                byte owner = p.readByte();
                GameWorld.GameProjectiles.Add(new ProjectileArrow(new Vector2(X, Y), angle, owner, strength) { ProjectileID = ID });
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
            List<byte> playersUpdated = new List<byte>();
            List<byte> allPlayers = GameWorld.OtherPlayers.Select(pl => pl.PlayerID).ToList();
            for (int i = 0; i < numToUpdate; i++)
            {
                int playerID = p.readByte();
                playersUpdated.Add((byte)playerID);
                PlayerEntity player;
                if (playerID == GameWorld.ThePlayer.PlayerEntity.PlayerID)
                    player = GameWorld.ThePlayer.PlayerEntity;
                else
                {
                    player = GameWorld.OtherPlayers.Where(pl => pl.PlayerID == playerID).FirstOrDefault();
                }
                if (player == null) player = new PlayerEntity(Vector2.Zero, (byte)playerID);
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
                if ((updateMask & (int)PlayerUpdateFlags.Player_Movement_Flags) != 0)
                {
                    byte flags = p.readByte();
                    player.OtherPlayerNetworkFlags = flags;
                    bool leftPress = (flags & (int) PlayerMovementFlag.Left_Pressed) != 0;
                    bool rightPress = (flags & (int)PlayerMovementFlag.Right_Pressed) != 0;
                    if(leftPress || rightPress)
                    {
                        player.FacingLeft = leftPress && !rightPress;
                        if (!(leftPress && rightPress))
                        {
                            player.TorsoAnimateable.StartLooping("player_run_start", "player_run_end");
                            player.LegsAnimateable.StartLooping("player_run_start", "player_run_end");
                        }
                        else
                        {
                            player.TorsoAnimateable.StartLooping("player_idle", "player_idle");
                            player.LegsAnimateable.StartLooping("player_idle", "player_idle");
                        }
                    }
                    else
                    {
                        player.TorsoAnimateable.StartLooping("player_idle", "player_idle");
                        player.LegsAnimateable.StartLooping("player_idle", "player_idle");
                    }
                                        
                }
                //player.FacingLeft = (updateMask & (int)PlayerUpdateFlags.Player_Facing_Left) != 0;
            }

            //Hide all non-updated players from view so that we don't get issues regarding position.
            //For instance, if a player goes outside the view, their last known position on the client
            //Will NOT be their current position. So if the client can see where they last were known to be
            //But not where they currently are, they will seem to be in the wrong position.
            //And yes, this is hacky as hell.
            List<byte> nonUpdatedPlayers = allPlayers.Except(playersUpdated).ToList();
            /*foreach(byte playerID in nonUpdatedPlayers)
            {
                PlayerEntity player;
                if (playerID == GameWorld.ThePlayer.PlayerEntity.PlayerID)
                    player = GameWorld.ThePlayer.PlayerEntity;
                else
                {
                    player = GameWorld.OtherPlayers.Where(pl => pl.PlayerID == playerID).FirstOrDefault();
                }
                if (player == null) continue;
                player.EntityPosition.X = -100;
                player.EntityPosition.Y = -100;
            }*/
        }
    }
}
