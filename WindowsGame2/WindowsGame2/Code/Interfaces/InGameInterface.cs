using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Managers;
using MiningGame.Code.Structs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Server;
using YogUILibrary.UIComponents;
using YogUILibrary.Managers;
namespace MiningGame.Code.Interfaces
{
    class InGameInterface : Interface
    {
        //List<ImageButton> buttons = new List<ImageButton>();
        ProgressBar healthBar;
        ProgressBar manaBar;

        public int offset = 0;

        public InGameInterface()
        {
            Vector2 start = new Vector2(0, 0);
            start.X += 10;
            start.Y += 10;

            base.blocking = false;

            healthBar = new ProgressBar(start, 100, 15, 100);
            healthBar.setBackgroundColor(Color.Green);
            healthBar.setBorderColor(Color.White);
            start.Y += 20;
            manaBar = new ProgressBar(start, 100, 15, 100);
            manaBar.setBackgroundColor(Color.Blue);
            healthBar.setBorderColor(Color.White);

            start.Y = 20;
            start.X += 150;
            for (int i = 0; i < 9; i++)
            {
                int blug = i;
                //buttons.Add(new ImageButton(start, "node", "node", () => { blockPressed(blug); }));
                start.X += 70;
            }
        }

        public void recomputeBoxes()
        {
            for (int i = 0; i < 9; i++)
            {
                //buttons[i].setImage(AssetManager.GetTexture("white"), 25, 25);
                //buttons[i].setLabel("");
            }

            for (int i = offset; i < GameWorld.thePlayer.playerInventory.Count; i++)
            {
                ItemStack itemStack = GameWorld.thePlayer.playerInventory[i];
                Item item = Item.getItem(itemStack.itemID);
                //buttons[i - offset].setLabel(itemStack.numberItems + " " + item.getName());
                Texture2D tex = AssetManager.GetTexture(item.getAsset());
                float width = 25 - tex.Width;
                float height = 25 - tex.Height;
                //buttons[i - offset].setImage(tex, width, height);
            }
        }

        public override void Update(GameTime time)
        {
            recomputeBoxes();

            healthBar.Update(time);
            manaBar.Update(time);

            //foreach (ImageButton im in buttons)
            //{
            //    im.Update(time);
           // }

            base.Update(time);
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawManager.Draw_Box(new Vector2(2, 2), new Vector2(Main.graphics.PreferredBackBufferWidth - 2, 55), Color.Black, sb, 0f, 200);
            DrawManager.Draw_Outline(new Vector2(1, 1), new Vector2(Main.graphics.PreferredBackBufferWidth - 1, 56), Color.White, sb, 200);

           // foreach (ImageButton im in buttons)
           // {
           //     im.Draw(sb);
           // }

            healthBar.Draw(sb);
            manaBar.Draw(sb);
            base.Draw(sb);
        }

        public void blockPressed(int i)
        {
            int b = i + offset;
            //if (buttons[i].getLabel() != "")
           // {
           //     GameWorld.thePlayer.playerInventorySelected = b;
           //     Packet p = new Packets.Packet1CSGameEvent(GameServer.GameEvents.Player_Inventory_Selection_Change, (byte)b);
           //     Main.clientNetworkManager.SendPacket(p);
          //  }
        }
    }
}
