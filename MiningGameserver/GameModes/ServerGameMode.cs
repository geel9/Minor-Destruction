using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Managers;
using MiningGameServer.Packets;

namespace MiningGameServer.GameModes
{
    public class ServerGameMode
    {
        public static List<Type> GameModes = new List<Type>();

        public static void GenerateGameModes()
        {
            Type[] types = Managers.ReflectionManager.GetAllSubClassesOf<ServerGameMode>();
            GameModes.AddRange(types);
        }

        public static Type GetGameMode(String name, string version = "-1")
        {
            foreach(Type t in GameModes)
            {
                ServerGameMode g = (ServerGameMode)ReflectionManager.CallConstructor(t);
                string gmName = g.GetName();
                string gmVersion = g.GetVersion();
                if(gmName == name && (version == "-1" || gmVersion == version))
                {
                    return t;
                }
            }
            return typeof (ServerGameMode);
        }

        public virtual string GetName()
        {
            return "GameMode";
        }

        public virtual string GetVersion()
        {
            return "1";
        }

        public ServerGameMode()
        {

        }

        public virtual void OnGameModeChosen()
        {

        }

        public virtual void OnPlayerConnect(NetworkPlayer player)
        {

        }

        public virtual void OnPlayerDisconnect(NetworkPlayer player)
        {

        }

        public virtual void OnPlayerChooseTeam(NetworkPlayer player, int team)
        {

        }

        public virtual void OnPlayerChooseClass(NetworkPlayer player, int classChosen)
        {

        }

        public virtual void OnPlayerSpawn(NetworkPlayer player)
        {
            
        }

        public virtual void OnPlayerDeath(NetworkPlayer player)
        {
            
        }

        public virtual void OnRoundStart()
        {

        }

        public virtual void OnRoundEnd()
        {

        }

        public virtual void Update_PreAll(GameTime time)
        {

        }

        public virtual void Update_PostAll(GameTime time)
        {
            
        }

        public virtual void OnPlayerPrePhysicsUpdate(NetworkPlayer player)
        {

        }

        public virtual void OnPlayerPostPhysicsUpdate(NetworkPlayer player, bool movedSince = false)
        {

        }

        protected void SendGameModeEvent(string eventName, Packet bytes, NetworkPlayer playerReceiving = null)
        {
            Packet14SCGameModeEvent packet = new Packet14SCGameModeEvent(eventName);
            packet.WriteBytes(bytes.GetData());
            GameServer.ServerNetworkManager.SendPacket(packet, playerReceiving != null ? playerReceiving.NetConnection : null);
        }
    }
}