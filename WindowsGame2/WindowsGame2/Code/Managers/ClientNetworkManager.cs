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
                            onConnected();
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
                if (bp.idToFireOn == num)
                {
                    bp.toFire(p);
                }
            }
        }

        public void receive()
        {
            boundPackets.Add(new BoundPacket((Packet p) =>
            {
                int x = p.readInt();
                int y = p.readInt();

                byte blockID = p.readByte();
                byte meta = p.readByte();
                GameWorld.setBlock(x, y, blockID, false, meta);
            }, 3));

            boundPackets.Add(new BoundPacket((Packet p) =>
            {
                //A game event
                GameWorld.HandleGameEvent(p.readByte(), p);
            }, 1));

            boundPackets.Add(new BoundPacket((Packet p) =>
            {
                string pName = p.readString();
                byte id = p.readByte();
                int posX = p.readInt();
                int posY = p.readInt();
                if (GameWorld.otherPlayers.Where(pl => pl.PlayerID == id).Count() > 0) return;
                if (GameWorld.thePlayer.playerEntity.PlayerID != id)
                {
                    ConsoleManager.Log("New player: " + pName + " id: " + id + " x: " + posX + " y: " + posY);
                    GameWorld.otherPlayers.Add(new PlayerEntity(new Vector2(posX, posY), id, pName));
                }
                else
                {
                    GameWorld.thePlayer.playerEntity.entityPosition = new Vector2(posX, posY);
                }
            }, 0));

            boundPackets.Add(new BoundPacket((Packet p) =>
            {
                byte id = p.readByte();
                ConsoleManager.Log("My id is " + id);
                GameWorld.thePlayer.playerEntity = new PlayerEntity(new Microsoft.Xna.Framework.Vector2(0, 0), id, ConsoleManager.getVariableValue("player_name"));
            }, 255));
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

        public void onConnected()
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
    }
}
