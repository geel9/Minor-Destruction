using System;
using System.Collections.Generic;
using GeeUI.Managers;
using MiningGame.Code.Items;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeeUI.Views;
using MiningGameServer;
using MiningGameServer.ItemAttributes;
using MiningGameServer.Packets;

namespace MiningGame.Code.Interfaces
{
    class InGameInterface : Interface
    {
        List<ButtonView> buttons = new List<ButtonView>();
        //ProgressBar healthBar;
        //ProgressBar manaBar;

        public PanelView MainView;
        public PanelView AttributesView;

        public int offset = 0;
        public int curSelected = 0;

        public InGameInterface()
        {
            MainView = new PanelView(GeeUI.GeeUI.RootView, Vector2.Zero);
            MainView.Width = 800;
            MainView.Height = 50;
            MainView.Draggable = false;

            AttributesView = new PanelView(GeeUI.GeeUI.RootView, InputManager.GetMousePosV());
            AttributesView.Width = 200;
            AttributesView.Height = 200;
            AttributesView.Active = false;

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
                new TextView(b, "10", new Vector2(0, 21), AssetManager.GetFont("Console2")) { TextColor = Color.White, Width = 25, Height = 10, TextJustification = TextJustification.Center };
                int i1 = i;
                b.OnMouseClick += (sender, e) =>
                {
                    BlockPressed(i1);
                };

                b.OnMouseOver += (sender, e) =>
                {
                    return;
                    AttributesView.Active = false;
                    int pos = offset + i1;
                    if (pos >= GameWorld.ThePlayer.PlayerInventory.Count) return;

                    AttributesView.Active = true;
                    AttributesView.Position = InputManager.GetMousePosV();
                    foreach (View v in AttributesView.Children)
                    {
                        AttributesView.RemoveChild(v);
                    }

                    ItemStack itemStack = GameWorld.ThePlayer.PlayerInventory[pos];
                    Item item = Item.GetItem(itemStack.ItemID);
                    int yPos = 0;
                    foreach (ItemAttribute attribute in item.GetDefaultAttributes())
                    {
                        TextView textview = new TextView(AttributesView, "", new Vector2(0, yPos),  AssetManager.GetFont("Console"));
                        textview.Width = 190;
                        textview.Height = 70;
                        yPos += 75;
                        textview.Text = attribute.GetDescription();
                        switch (attribute.GetAffinity())
                        {
                            case AttributeAffinity.Invisible:
                                break;
                            case AttributeAffinity.Negative:
                                textview.TextColor = Color.Red;
                                break;
                            case AttributeAffinity.Positive:
                                textview.TextColor = Color.Green;
                                break;
                        }
                    }
                };

                b.OnMouseOff += (sender, e) =>
                                    {
                                        return;
                                         AttributesView.Active = false;
                                         foreach (View v in AttributesView.Children)
                                         {
                                             AttributesView.RemoveChild(v);
                                         }
                                     };
                buttons.Add(b);
                start.X += 70;
            }

            InputManager.BindMouse(() =>
            {
                if (++curSelected >= 9)
                    curSelected = 0;
                BlockPressed(curSelected);
            }, MouseButton.Scrollup);
            InputManager.BindMouse(() =>
            {
                if (--curSelected < 0)
                    curSelected = 8;
                BlockPressed(curSelected);
            }, MouseButton.Scrolldown);

        }

        public void RecomputeBoxes()
        {
            for (int i = 0; i < 9; i++)
            {
                ((ImageView)buttons[i].ButtonContentview).Texture = AssetManager.GetTexture("white");
                ((TextView)buttons[i].Children[1]).Text = "";
            }

            for (int i = offset; i < GameWorld.ThePlayer.PlayerInventory.Count; i++)
            {
                ItemStack itemStack = GameWorld.ThePlayer.PlayerInventory[i];
                Item item = Item.GetItem(itemStack.ItemID);
                Texture2D tex = AssetManager.GetTexture(item.GetAsset());
                ((ImageView)buttons[i - offset].ButtonContentview).Texture = tex;
                ((TextView)buttons[i].Children[1]).Text = itemStack.NumberItems > 0
                                                               ? itemStack.NumberItems.ToString()
                                                               : "";
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
            curSelected = b;
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
