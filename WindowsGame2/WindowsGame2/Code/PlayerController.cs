using System;
using System.Collections.Generic;
using System.Linq;
using GeeUI.Managers;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Items;
using MiningGame.Code.Interfaces;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MiningGame.Code.Entities;
using MiningGame.Code.PlayerClasses;
using MiningGame.ExtensionMethods;
using MiningGameServer;
using MiningGameServer.Packets;

namespace MiningGame.Code
{
    public class PlayerController : UpdatableAndDrawable
    {
        public PlayerInventory Inventory;

        public PlayerEntity PlayerEntity;
        private const int PlayerSpeedX = 2;
        public int DigPct = 0;
        public int JumpTimer = 0;
        public int DigTimer = 0;
        public int DigStrength = 100;
        public Vector2 CurMining;

        public short PlayerAimAngle = 0;

        private bool _leftPressed, _rightPressed, _jumpPressed, _attackPressed, _sprinting;

        public byte MovementFlags
        {
            get
            {
                byte ret = 0;
                if (_leftPressed)
                    ret |= (int)PlayerMovementFlag.Left_Pressed;
                if (_rightPressed)
                    ret |= (int)PlayerMovementFlag.Right_Pressed;
                if (_jumpPressed)
                    ret |= (int)PlayerMovementFlag.Jump_Pressed;
                if (_attackPressed)
                    ret |= (int)PlayerMovementFlag.Attack_Pressed;
                if (_sprinting)
                    ret |= (int)PlayerMovementFlag.Sprinting;
                return ret;
            }
        }

        public PlayerController()
        {
            PlayerEntity = new PlayerEntity(new Vector2(-100, -100), 0);
            Inventory = new PlayerInventory(this);
        }

        public void SendMovementFlags()
        {
            Packet packet;

            short angle = PlayerEntity.GetAimingAngle();
            if (angle != PlayerAimAngle)
            {
                packet = new Packet5CSPlayerMovementFlagsAndAim(MovementFlags, angle);
                PlayerAimAngle = PlayerEntity.GetAimingAngle();
            }
            else
            {
                packet = new Packet4CSPlayerMovementFlags(MovementFlags);
            }

            Main.clientNetworkManager.SendPacket(packet);
        }

        public void Start()
        {
            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                _leftPressed = true;
                SendMovementFlags();
            }, Keys.A);

            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                _rightPressed = true;
                SendMovementFlags();
            }, Keys.D);

            InputManager.BindMouse(() =>
                                       {
                                           short angle = (short)((float)PlayerAimAngle).RToD();
                                           if (angle != PlayerAimAngle)
                                               SendMovementFlags();
                                       }, MouseButton.Movement, true, true);

            InputManager.BindKey(() =>
                                     {
                                         _leftPressed = false;

                                         SendMovementFlags();
                                     }, Keys.A, true, false);

            InputManager.BindKey(() =>
                                     {
                                         _rightPressed = false;
                                         SendMovementFlags();
                                     }, Keys.D, true, false);

            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                _jumpPressed = true;
                SendMovementFlags();
            }, Keys.W, true);

            InputManager.BindKey(() =>
                                     {
                                         _jumpPressed = false;
                                         SendMovementFlags();
                                     }, Keys.W, false, false);


            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                short blockID = GameWorld.GetBlockAt(PlayerEntity.GetEntityTile().X, PlayerEntity.GetEntityTile().Y).ID;
                if (blockID == 6)
                {
                    Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 's', (bool)true);
                    Main.clientNetworkManager.SendPacket(pack);
                }
            }, Keys.S, true);
            // Duplicate was present

            InputManager.BindMouse(() =>
            {
                if (InterfaceManager.blocking || InputManager.GetMousePos().Y <= 50) return;
                _attackPressed = true;
                SendMovementFlags();
            }, MouseButton.Left);

            InputManager.BindMouse(() =>
                                       {
                                           _attackPressed = false;
                                           SendMovementFlags();
                                       }, MouseButton.Left, false);

            InputManager.BindMouse(() =>
            {
                if (InterfaceManager.blocking || InputManager.GetMousePos().Y <= 50) return;
                Item i = Inventory.GetPlayerItemInHand();
                if (i == null) return;
                Vector2 aim = PlayerEntity.GetBlockAimingAt();
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_Use_Item, (short)aim.X, (short)aim.Y);
                Main.clientNetworkManager.SendPacket(pack);
            }, MouseButton.Right);

            InputManager.BindKey(() =>
                                     {
                                         if (InterfaceManager.blocking) return;
                                         Packet1CSGameEvent p = new Packet1CSGameEvent(GameServer.GameEvents.Player_Drop_Item);
                                         Main.clientNetworkManager.SendPacket(p);
                                     }, Keys.F);

            InputManager.BindKey(() =>
            {
                _sprinting = true;
                SendMovementFlags();
            }, Keys.LeftShift);

            InputManager.BindKey(() =>
            {
                _sprinting = false;
                SendMovementFlags();
            }, Keys.LeftShift, true, false);

            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                Vector2 aim = PlayerEntity.GetBlockAimingAt();
                short id = GameWorld.GetBlockAt(aim.X, aim.Y).ID;
                if (id != 0)
                {
                    Packet1CSGameEvent p = new Packet1CSGameEvent(GameServer.GameEvents.Player_Use_Block, (short)aim.X, (short)aim.Y);
                    Main.clientNetworkManager.SendPacket(p);
                }
            }, Keys.E, false);

            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                if (PlayerEntity.PClass is PlayerClassDestroyer == false)
                    return;

                PlayerClassDestroyer c = (PlayerClassDestroyer)PlayerEntity.PClass;
                Vector2 aim = PlayerEntity.GetBlockAimingAt();
                short id = GameWorld.GetBlockAt(aim.X, aim.Y).ID;

                if(c.BlockInHand != 0)
                {
                    Packet1CSGameEvent p = new Packet1CSGameEvent(GameServer.GameEvents.Player_Place_Block);
                    Main.clientNetworkManager.SendPacket(p);
                    return;
                }
                if (id != 0) {
                    Packet1CSGameEvent p = new Packet1CSGameEvent(GameServer.GameEvents.Player_Pickup_Block, (short)aim.X, (short)aim.Y);
                    Main.clientNetworkManager.SendPacket(p);
                }
            }, Keys.Q);


            InputManager.BindMouse(() =>
            {
                if (InterfaceManager.blocking) return;
                return;
            }, MouseButton.Middle);
        }



        public int TimeSinceLastServerUpdate = 0;

        public void Update(GameTime time)
        {
            if (JumpTimer > 0) JumpTimer--;
            if (DigTimer > 0) DigTimer--;
            if (--TimeSinceLastServerUpdate <= 0)
            {
                TimeSinceLastServerUpdate = 2;
            }
            if (PlayerEntity.PlayerID != -1)
                PlayerEntity.Update(time);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (PlayerEntity.PlayerID != -1)
                PlayerEntity.Draw(sb);
        }


        #region interfaces
        public void addToUpdateList()
        {
            Main.updatables.Add(this);
        }

        public void addToList()
        {
            addToDrawList();
            addToUpdateList();
        }

        public void removeFromList()
        {
            removeFromDrawList();
            removeFromUpdateList();
        }

        public void removeFromUpdateList()
        {
            Main.updatables.Remove(this);
        }

        public void addToDrawList()
        {
            Main.drawables.Add(this);
        }

        public void removeFromDrawList()
        {
            Main.drawables.Remove(this);
        }

        public bool inCamera()
        {
            return true;
        }

        public static void ConsoleInit()
        {
            ConsoleManager.AddConVar("player_name", "Your name in a server", "Player_" + Main.R.Next(0, 1000), l =>
            {
                if (Main.clientNetworkManager.IsConnected())
                {
                    Packet1CSGameEvent packet = new Packet1CSGameEvent(GameServer.GameEvents.Player_Change_Name, l);
                    Main.clientNetworkManager.SendPacket(packet);
                }
            });

            ConsoleManager.AddConVar("player_team", "Choose a team", "0", ls =>
            {
                if (Main.clientNetworkManager.IsConnected())
                {
                    Packet1CSGameEvent packet = new Packet1CSGameEvent(GameServer.GameEvents.Player_Choose_Team, Convert.ToByte(ls));
                    Main.clientNetworkManager.SendPacket(packet);
                }                                                    
            });
        }

        #endregion
    }
}
