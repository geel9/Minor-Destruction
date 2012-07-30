using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Entities;
using MiningGame.Code.Managers;
using MiningGameServer.Packets;

namespace MiningGame.Code.GameModes
{
    public class ClientGameMode
    {
        public static List<Type> GameModes = new List<Type>();

        public static void GenerateGameModes()
        {
            Type[] types = ReflectionManager.GetAllSubClassesOf<ClientGameMode>();
            GameModes.AddRange(types);
        }

        public static Type GetGameMode(String name, string version = "-1")
        {
            foreach (Type t in GameModes)
            {
                ClientGameMode g = (ClientGameMode)ReflectionManager.CallConstructor(t);
                string gmName = g.GetName();
                string gmVersion = g.GetVersion();
                if (gmName == name && (version == "-1" || gmVersion == version))
                {
                    return t;
                }
            }
            return typeof(ClientGameMode);
        }

        public virtual string GetName()
        {
            return "GameMode";
        }

        public virtual string GetVersion()
        {
            return "1";
        }

        public virtual void OnPlayerConnect(PlayerEntity player)
        {

        }

        public virtual void OnPlayerDisconnect(PlayerEntity player)
        {

        }

        public virtual void OnPlayerChooseTeam(PlayerEntity player, int team)
        {

        }

        public virtual void OnPlayerChooseClass(PlayerEntity player, int classChosen)
        {

        }

        public virtual void OnPlayerDeath(PlayerEntity player)
        {
            
        }

        public virtual void OnRoundStart()
        {
            
        }

        public virtual void OnRoundEnd()
        {
            
        }

        public virtual void Update(GameTime time)
        {
            
        }

        public virtual void Draw_PreWorld(SpriteBatch sb)
        {

        }

        public virtual void Draw_PostWorld(SpriteBatch sb)
        {

        }

        public virtual void OnPlayerUpdate(PlayerEntity player)
        {
            
        }

        public virtual void OnPlayerPreDraw(PlayerEntity player, SpriteBatch sb)
        {
            
        }

        public virtual void OnPlayerPostDraw(PlayerEntity player, SpriteBatch sb)
        {
            
        }

        public virtual void OnGameModeEvent(string eventName, Packet data)
        {
            
        }
    }
}
