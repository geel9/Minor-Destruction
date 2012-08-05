using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Packets;
using MiningGameServer.Shapes;

namespace MiningGameServer.Player
{
    public class PlayerClass
    {
        public NetworkPlayer NetworkPlayer;

        public virtual ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB(0, 0, 0, 0);
            }
        }

        //1 = destroyer, 0 = FUCK YOU GO DIE DON'T USE 0 IDIOT
        public static Type[] PlayerClasses = new Type[] { typeof(PlayerClass), typeof(PlayerClassDestroyer) };

        public virtual void OnDeath()
        {
            
        }

        public virtual void OnSpawn()
        {
            
        }

        public virtual int GetPlayerInventorySize()
        {
            return 25;
        }
        
        public virtual float GetPlayerWalkVelocity()
        {
            return 3;
        }
        public virtual float GetPlayerSprintVelocity()
        {
            return 5;
        }

        public virtual void Update_PrePhys(GameTime time)
        {
            
        }

        public virtual void Update_PostPhys(GameTime time, bool movedSince = false)
        {

        }

        protected void MarkClassFlagsUpdate()
        {
            NetworkPlayer.UpdateMask |= (byte) PlayerUpdateFlags.Player_Update;
            NetworkPlayer.UpdateMask |= (byte)PlayerUpdateFlags.Player_Class_Update;
        }

        public virtual void ClearUpdateMask()
        {
            
        }

        //This is so that each class can have as much data defining their state as necessary.
        //Generally it'll only be 1-2 bytes. Read is on the client, Write is on the server side.
        public virtual void WriteState(Packet p){
        }
    }
}
