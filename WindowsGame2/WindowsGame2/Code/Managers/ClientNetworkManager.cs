using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Items;
using MiningGame.Code.Structs;
using System.Net;
using MiningGame.Code.Interfaces;
using MiningGame.Code.Entities;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using MiningGame.ExtensionMethods;
using MiningGameServer.Packets;

namespace MiningGame.Code.Managers
{
    public class ClientNetworkManager
    {
        public NetClient NetClient = null;

        public bool IsConnected()
        {
            return NetClient != null && (NetClient.ConnectionStatus == NetConnectionStatus.Connected || NetClient.ConnectionStatus == NetConnectionStatus.InitiatedConnect);
        }

        public ClientNetworkManager()
        {
            receive();
        }

        public void Update(GameTime theTime)
        {
            if (NetClient == null) return;
            NetIncomingMessage msg;
            while ((msg = NetClient.ReadMessage()) != null)
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
                        HandlePacket(new Packet(msg.m_data));
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
            NetOutgoingMessage om = NetClient.CreateMessage();
            om.Write(data);
            NetClient.SendMessage(om, method);
            //netClient.Recycle(om);
        }

        public void HandlePacket(Packet p)
        {
            byte num = p.ReadByte();
            GameWorld.HandlePacket(num, p);
        }

        public void receive()
        {

        }

        public void Connect(string ip, int port)
        {
            Regex regex = new Regex("\\d+?\\.\\d+?\\.\\d+?\\.\\d+?");
            if (!regex.IsMatch(ip))
            {
                IPHostEntry entry = Dns.GetHostEntry(ip);
                if (entry.AddressList.Length > 0)
                    ip = entry.AddressList[0].ToString();
            }
            NetClient = new NetClient(new NetPeerConfiguration("MinorDestruction"));
            NetClient.Start();
            ConsoleManager.Log("Connecting to " + ip + ":" + port);

            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                NetClient.Connect(serverEndPoint);

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
            string name = ConsoleManager.GetVariableValue("player_name");
            ConsoleManager.Log("Connected! Name:" + name);
            Packet0CSPlayerConnect packet = new Packet0CSPlayerConnect(name);
            SendPacket(packet);
        }

        public void Disconnect()
        {
            try
            {
                if (IsConnected())
                {
                    NetClient.Disconnect("Disconnected.");
                    ConsoleManager.Log("Disconnected from server");
                }
            }
            catch (Exception)
            {
            }
        }

        public static void ConsoleInit()
        {
            ConsoleManager.AddConCommandArgs("connect", "Connect to a server", ls =>
            {

                string ip = ls[0];
                int port = Convert.ToInt32(ls[1]);
                Main.clientNetworkManager.Connect(ip, port);
            }, 2);

            ConsoleManager.AddConCommand("disconnect", "Disconnect from the server", Main.clientNetworkManager.Disconnect);
        }

    }
}
