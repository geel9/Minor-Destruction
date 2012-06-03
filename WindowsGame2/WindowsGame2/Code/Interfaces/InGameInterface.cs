using System;
using System.Collections.Generic;
using MiningGame.Code.Items;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeeUI.Views;
using MiningGameServer;
using MiningGameServer.Packets;

namespace MiningGame.Code.Interfaces
{
    class InGameInterface : Interface
    {
        List<ButtonView> buttons = new List<ButtonView>();
        //ProgressBar healthBar;
        //ProgressBar manaBar;

        public PanelView MainView;

        public int offset = 0;

        public InGameInterface()
        {
            MainView = new PanelView(GeeUI.GeeUI.RootView, Vector2.Zero);
            MainView.Width = 800;
            MainView.Height = 50;
            MainView.Draggable = true;

            Vector2 start = new Vector2(0, 0);
            start.X += 10;
            start.Y += 10;

            base.blocking = false;

            start.Y += 20;
            start.Y = 10;
            start.X = 50;

            for (int i = 0; i < 9; i++)
            {
                ButtonView b = new ButtonView(MainView, new ImageView(null, AssetManager.GetTexture("white")), start) { Width = 25, Height = 25 };
                int i1 = i;
                b.OnMouseClick += new View.MouseClickEventHandler((object sender, EventArgs e) =>
                                                                      {
                                                                          BlockPressed(i1);
                                                                      });
                buttons.Add(b);
                start.X += 70;
            }
        }

        public void RecomputeBoxes()
        {
            for (int i = 0; i < 9; i++)
            {
                ((ImageView)buttons[i].ButtonContentview).Texture = AssetManager.GetTexture("white");
            }

            for (int i = offset; i < GameWorld.ThePlayer.PlayerInventory.Count; i++)
            {
                ItemStack itemStack = GameWorld.ThePlayer.PlayerInventory[i];
                Item item = Item.GetItem(itemStack.itemID);
                Texture2D tex = AssetManager.GetTexture(item.GetAsset());
                ((ImageView)buttons[i - offset].ButtonContentview).Texture = tex;
            }
        }

        public override void Update(GameTime time)
        {
            RecomputeBoxes();

            //foreach (ImageButton im in buttons)
            //{
            //    im.Update(time);
            // }

            base.Update(time);
        }

        public override void Draw(SpriteBatch sb)
        {
            //DrawManager.Draw_Box(new Vector2(2, 2), new Vector2(Main.graphics.PreferredBackBufferWidth - 2, 55), Color.Black, sb, 0f, 200);
            //DrawManager.Draw_Outline(new Vector2(1, 1), new Vector2(Main.graphics.PreferredBackBufferWidth - 1, 56), Color.White, sb, 200);

            // foreach (ImageButton im in buttons)
            // {
            //     im.Draw(sb);
            // }

            //healthBar.Draw(sb);
            //manaBar.Draw(sb);
            base.Draw(sb);
        }

        public void BlockPressed(int i)
        {
            int b = i + offset;
            GameWorld.ThePlayer.PlayerInventorySelected = b;
            Packet p = new Packet1CSGameEvent(GameServer.GameEvents.Player_Inventory_Selection_Change, (byte)b);
            Main.clientNetworkManager.SendPacket(p);

            for (int j = 0; j < 9; j++)
            {
                buttons[j].NinePatchNormal = GeeUI.GeeUI.NinePatchBtnDefault;
            }
            buttons[i].NinePatchNormal = GeeUI.GeeUI.NinePatchBtnClicked;
        }
    }
}
