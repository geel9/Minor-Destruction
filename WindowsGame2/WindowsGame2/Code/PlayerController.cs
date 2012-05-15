using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MiningGame.Code.Entities;
using MiningGame.Code.Blocks;
using MiningGame.Code.Packets;
using MiningGame.Code.Server;
using YogUILibrary.Managers;
namespace MiningGame.Code
{
    public class PlayerController : UpdatableAndDrawable
    {
        public int playerInventorySelected = -1;
        public List<ItemStack> playerInventory = new List<ItemStack>();

        public PlayerEntity playerEntity;
        private const int playerSpeedX = 2;
        public int digPct = 0;
        public int jumpTimer = 0;
        public int digTimer = 0;
        public int digStrength = 100;
        public Vector2 curMining;

        public static Point curBlockConnecting = new Point(-1, -1);

        public PlayerController()
        {
            playerEntity = new PlayerEntity(new Vector2(-100, -100), 0);
        }

        public void Start()
        {
            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 'a', (bool)true);
                Main.clientNetworkManager.SendPacket(pack);
                playerEntity.FacingLeft = true;
                playerEntity.TorsoAnimateable.startLooping("player_run_start", "player_run_end");
                playerEntity.LegsAnimateable.startLooping("player_run_start", "player_run_end");
            }, Keys.A, true);

            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 'd', (bool)true);
                Main.clientNetworkManager.SendPacket(pack);
                playerEntity.TorsoAnimateable.startLooping("player_run_start", "player_run_end");
                playerEntity.LegsAnimateable.startLooping("player_run_start", "player_run_end");
                playerEntity.FacingLeft = false;
            }, Keys.D, true);

            InputManager.BindKey(() =>
            {
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 'a', (bool)false);
                Main.clientNetworkManager.SendPacket(pack);
                playerEntity.TorsoAnimateable.startLooping("player_idle", "player_idle");
                playerEntity.LegsAnimateable.startLooping("player_idle", "player_idle");
            }, Keys.A, true, false);

            InputManager.BindKey(() =>
            {
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 'd', (bool)false);
                Main.clientNetworkManager.SendPacket(pack);
                playerEntity.TorsoAnimateable.startLooping("player_idle", "player_idle");
                playerEntity.LegsAnimateable.startLooping("player_idle", "player_idle");
            }, Keys.D, true, false);

            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 'w', (bool)true);
                Main.clientNetworkManager.SendPacket(pack);
            }, Keys.W, true);

            InputManager.BindKey(() =>
            {
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 'w', (bool)false);
                Main.clientNetworkManager.SendPacket(pack);
            }, Keys.W, true, false);


            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                byte blockID = GameWorld.GetBlockIDAt(playerEntity.getEntityTile().X, playerEntity.getEntityTile().Y);
                if (blockID == 6)
                {
                    Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 's', (bool)true);
                    Main.clientNetworkManager.SendPacket(pack);
                }
            }, Keys.S, true);

            InputManager.BindKey(() =>
            {
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_KeyPress, 's', (bool)false);
                Main.clientNetworkManager.SendPacket(pack);
            }, Keys.S, true, false);

            InputManager.BindMouse(() =>
            {
                if (InterfaceManager.blocking) return;
            }, MouseButton.Left, true, true);

            InputManager.BindMouse(() =>
            {
            }, MouseButton.Left, false);

            InputManager.BindMouse(() =>
            {
                if (InterfaceManager.blocking) return;
                Item i = getPlayerItemInHand();
                if (i == null) return;
                Vector2 aim = playerEntity.GetBlockAimingAt();
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_Use_Item, (int)aim.X, (int)aim.Y);
                Main.clientNetworkManager.SendPacket(pack);
            }, MouseButton.Right);

            InputManager.BindKey(() =>
            {
                if (InterfaceManager.blocking) return;
                Vector2 aim = playerEntity.GetBlockAimingAt();
                byte id = GameWorld.GetBlockIDAt(aim.X, aim.Y);
                if (id != 0)
                {
                    Packet1CSGameEvent p = new Packet1CSGameEvent(GameServer.GameEvents.Player_Use_Block, (int)aim.X, (int)aim.Y);
                    Main.clientNetworkManager.SendPacket(p);
                }
            }, Keys.E, false);

            InputManager.BindMouse(() =>
            {
                if (InterfaceManager.blocking) return;
                return;
                /*if (curBlockConnecting == new Point(-1, -1))
                {
                    curBlockConnecting = ConversionManager.VToP(GameWorld.AbsoluteToTile(InputManager.GetMousePosV(true)));
                }
                else
                {
                    Vector2 curBlock = ConversionManager.PToV(curBlockConnecting);
                    Vector2 newBlock = GameWorld.AbsoluteToTile(InputManager.GetMousePosV(true));
                    if (curBlock != newBlock)
                    {
                        //GameServer.connectBlocks(curBlock, newBlock);
                    }
                    curBlockConnecting = new Point(-1, -1);
                }*/
            }, MouseButton.Middle);
        }

        public bool HasItem(byte id)
        {
            return getPlayerItemStackFromInventory(id).itemID == id;
        }

        public ItemStack getPlayerItemStackFromInventory(byte id)
        {
            return playerInventory.Where(x => x.itemID == id).FirstOrDefault();
        }

        public int getNumItemInInventory(byte id)
        {
            return playerInventory.Where(x => x.itemID == id).FirstOrDefault().numberItems;
        }

        public void removeItems(byte itemID, int numToRemove)
        {
            if (getNumItemInInventory(itemID) < numToRemove) return;
            ItemStack i = getPlayerItemStackFromInventory(itemID);
            int index = playerInventory.IndexOf(i);
            i.numberItems -= numToRemove;
            if (i.numberItems == 0)
            {
                if (index < playerInventorySelected) playerInventorySelected++;
                playerInventory.RemoveAt(index);
            }
            else
            {
                playerInventory[index] = i;
            }
        }

        public Item getPlayerItemInHand()
        {
            if (playerInventorySelected >= playerInventory.Count) playerInventorySelected = -1;
            if (playerInventorySelected == -1) return null;
            return Item.getItem(playerInventory[playerInventorySelected].itemID);
        }

        public void PickupItem(ItemStack item)
        {
            for (int i = 0; i < playerInventory.Count; i++)
            {
                ItemStack it = playerInventory[i];
                if (it.itemID == item.itemID)
                {
                    playerInventory[i] = new ItemStack(it.numberItems + item.numberItems, it.itemID);
                    //playerInventory.Clear();
                    return;
                }
            }
            playerInventory.Add(item);
        }

        public int timeSinceLastServerUpdate = 0;

        public void Update(GameTime time)
        {
            if (jumpTimer > 0) jumpTimer--;
            if (digTimer > 0) digTimer--;
            if (--timeSinceLastServerUpdate <= 0)
            {
                timeSinceLastServerUpdate = 2;
            }
            if (playerEntity.PlayerID != -1)
                playerEntity.Update(time);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (playerEntity.PlayerID != -1)
                playerEntity.Draw(sb);
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
