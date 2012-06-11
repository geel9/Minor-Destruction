using System.Collections.Generic;
using System.Linq;
using GeeUI.Managers;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Items;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MiningGame.Code.Entities;
using MiningGame.ExtensionMethods;
using MiningGameServer;
using MiningGameServer.Packets;

namespace MiningGame.Code
{
    public class PlayerController : UpdatableAndDrawable
    {
        public int PlayerInventorySelected = -1;
        public List<ItemStack> PlayerInventory = new List<ItemStack>();

        public PlayerEntity PlayerEntity;
        private const int PlayerSpeedX = 2;
        public int DigPct = 0;
        public int JumpTimer = 0;
        public int DigTimer = 0;
        public int DigStrength = 100;
        public Vector2 CurMining;

        public short PlayerAimAngle = 0;

        private bool leftPressed, rightPressed, jumpPressed, attackPressed;

        public byte MovementFlags
        {
            get
            {
                byte ret = 0;
                if (leftPressed)
                    ret |= (int)PlayerMovementFlag.Left_Pressed;
                if (rightPressed)
                    ret |= (int)PlayerMovementFlag.Right_Pressed;
                if (jumpPressed)
                    ret |= (int)PlayerMovementFlag.Jump_Pressed;
                if (attackPressed)
                    ret |= (int)PlayerMovementFlag.Attack_Pressed;
                return ret;
            }
        }

        public PlayerController()
        {
            PlayerEntity = new PlayerEntity(new Vector2(-100, -100), 0);
        }

        public void SendMovementFlags()
        {
            Packet packet;// = new Packet4CSPlayerMovementFlags(MovementFlags);

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
                leftPressed = true;
                SendMovementFlags();
               //PlayerEntity.TorsoAnimateable.StartLooping("player_run_start", "player_run_end");
              //  PlayerEntity.LegsAnimateable.StartLooping("player_run_start", "player_run_end");
            }, Keys.A);

            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                rightPressed = true;
                SendMovementFlags();
               // PlayerEntity.TorsoAnimateable.StartLooping("player_run_start", "player_run_end");
               // PlayerEntity.LegsAnimateable.StartLooping("player_run_start", "player_run_end");
                //PlayerEntity.FacingLeft = false;
            }, Keys.D);

            InputManager.BindMouse(() =>
                                       {
                                           short angle = (short) ((float) PlayerAimAngle).RToD();
                                           if(angle != PlayerAimAngle)
                                               SendMovementFlags();
                                       }, MouseButton.Movement, true, true);

            InputManager.BindKey(() =>
                                     {
                                         leftPressed = false;
                                         SendMovementFlags();
                                        // PlayerEntity.TorsoAnimateable.StartLooping("player_idle", "player_idle");
                                        // PlayerEntity.LegsAnimateable.StartLooping("player_idle", "player_idle");
                                     }, Keys.A, true, false);

            InputManager.BindKey(() =>
                                     {
                                         rightPressed = false;
                                         SendMovementFlags();
                                      //   PlayerEntity.TorsoAnimateable.StartLooping("player_idle", "player_idle");
                                      //   PlayerEntity.LegsAnimateable.StartLooping("player_idle", "player_idle");
                                     }, Keys.D, true, false);

            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                jumpPressed = true;
                SendMovementFlags();
            }, Keys.W, true);

            InputManager.BindKey(() =>
                                     {
                                         jumpPressed = false;
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

            InputManager.BindKey(() =>
            {
                //Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 's', (bool)false);
                // Main.clientNetworkManager.SendPacket(pack);
            }, Keys.S, true, false);

            InputManager.BindMouse(() =>
            {
                if (InterfaceManager.blocking || InputManager.GetMousePos().Y <= 50) return;
                attackPressed = true;
                SendMovementFlags();
            }, MouseButton.Left);

            InputManager.BindMouse(() =>
                                       {
                                           attackPressed = false;
                                           SendMovementFlags();
                                       }, MouseButton.Left, false);

            InputManager.BindMouse(() =>
            {
                if (InterfaceManager.blocking || InputManager.GetMousePos().Y <= 50) return;
                Item i = GetPlayerItemInHand();
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
                if (InterfaceManager.blocking) return;
                Vector2 aim = PlayerEntity.GetBlockAimingAt();
                short id = GameWorld.GetBlockAt(aim.X, aim.Y).ID;
                if (id != 0)
                {
                    Packet1CSGameEvent p = new Packet1CSGameEvent(GameServer.GameEvents.Player_Use_Block, (short)aim.X, (short)aim.Y);
                    Main.clientNetworkManager.SendPacket(p);
                }
            }, Keys.E, false);

            InputManager.BindMouse(() =>
            {
                if (InterfaceManager.blocking) return;
                return;
            }, MouseButton.Middle);
        }

        public bool HasItem(byte id)
        {
            return GetPlayerItemStackFromInventory(id).ItemID == id;
        }

        public ItemStack GetPlayerItemStackFromInventory(byte id)
        {
            return PlayerInventory.Where(x => x.ItemID == id).FirstOrDefault();
        }

        public int GetNumItemInInventory(byte id)
        {
            return PlayerInventory.Where(x => x.ItemID == id).FirstOrDefault().NumberItems;
        }

        public void RemoveItems(byte itemID, int numToRemove)
        {
            if (GetNumItemInInventory(itemID) < numToRemove) return;
            ItemStack i = GetPlayerItemStackFromInventory(itemID);
            int index = PlayerInventory.IndexOf(i);
            i.NumberItems -= numToRemove;
            if (i.NumberItems == 0)
            {
                if (index < PlayerInventorySelected) PlayerInventorySelected++;
                PlayerInventory.RemoveAt(index);
            }
            else
            {
                PlayerInventory[index] = i;
            }
        }

        public Item GetPlayerItemInHand()
        {
            if (PlayerInventorySelected >= PlayerInventory.Count) PlayerInventorySelected = -1;
            if (PlayerInventorySelected == -1) return null;
            return Item.GetItem(PlayerInventory[PlayerInventorySelected].ItemID);
        }

        public void PickupItem(ItemStack item)
        {
            for (int i = 0; i < PlayerInventory.Count; i++)
            {
                ItemStack it = PlayerInventory[i];
                if (it.ItemID == item.ItemID)
                {
                    PlayerInventory[i] = new ItemStack(it.NumberItems + item.NumberItems, it.ItemID);
                    //playerInventory.Clear();
                    return;
                }
            }
            PlayerInventory.Add(item);
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

        #endregion
    }
}
