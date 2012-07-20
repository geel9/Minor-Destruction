using System;
using GeeUI.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Entities;
using MiningGame.Code.Managers;
using MiningGameServer.Packets;
using MiningGameServer.Shapes;
using MiningGame.ExtensionMethods;

namespace MiningGame.Code.PlayerClasses
{
    public class PlayerClass
    {
        public PlayerEntity Player;
        public Animation AnimationTorso = AssetManager.GetAnimation("playertorso");
        public Animation AnimationLegs = AssetManager.GetAnimation("playerlegs");
        public virtual ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB(0, 0, 0, 0);
            }
        }

        //1 = destroyer, 0 = FUCK YOU GO DIE DON'T USE 0 IDIOT
        public static Type[] PlayerClasses = new Type[]{ typeof(PlayerClass), typeof(PlayerClassDestroyer)};

        public virtual void OnDeath()
        {
            
        }

        public virtual void OnSpawn()
        {
            
        }

        public short GetAimingAngle(Vector2 EntityPosition)
        {
            Vector2 aim = InputManager.GetMousePosV() + CameraManager.cameraPosition;
            aim -= EntityPosition;
            return (short)Math.Atan2(aim.Y, aim.X).RToD();
        }

        public virtual void Update(GameTime time)
        {
            
        }

        public virtual void Draw_Pre(SpriteBatch sb)
        {
            
        }

        public virtual void Draw_Post(SpriteBatch sb)
        {

        }

        //This is so that each class can have as much data defining their state as necessary.
        //Generally it'll only be 1-2 bytes. Read is on the client, Write is on the server side.
        public virtual void ReadState(Packet p)
        {
            
        }
    }
}
