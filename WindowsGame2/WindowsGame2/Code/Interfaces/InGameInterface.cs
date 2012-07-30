using System;
using System.Collections.Generic;
using GeeUI.Managers;
using GeeUI.ViewLayouts;
using Microsoft.Xna.Framework.Input;
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
        List<ButtonView> _buttons = new List<ButtonView>();
        List<ButtonView> _expandedButtons = new List<ButtonView>();

        public PanelView MainView;
        public PanelView ExpandedView;

        public int CurSelected = 0;

        public InGameInterface()
        {
            MainView = new PanelView(GeeUI.GeeUI.RootView, Vector2.Zero) { Width = 800, Height = 50, Draggable = false };

            //Create a new invisible view to limit the draggable range of the Expanded Inventory view.
            View ContainerView = new View(GeeUI.GeeUI.RootView)
            {
                Width = 800,
                Height = 450,
                X = 0,
                Y = 50
            };
            ExpandedView = new PanelView(ContainerView, new Vector2(0, 55))
                               {
                                   Width = 200,
                                   Height = 200,
                                   Active = false,
                                   ChildrenLayout = new HorizontalViewLayout(20, true)
                               };


            BagSizeChange(20);

            Vector2 start = new Vector2(0, 0);
            base.blocking = false;
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


                _buttons.Add(b);
                start.X += 70;
            }

            InputManager.BindMouse(() =>
            {
                if (++CurSelected >= 9)
                    CurSelected = 0;
                BlockPressed(CurSelected);
            }, MouseButton.Scrollup);
            InputManager.BindMouse(() =>
            {
                if (--CurSelected < 0)
                    CurSelected = 8;
                BlockPressed(CurSelected);
            }, MouseButton.Scrolldown);

            InputManager.BindKey(() =>
            {
                if(!blocking && InterfaceManager.blocking) return;
                blocking = !blocking;
                ExpandedView.Active = blocking;
            }, Keys.C);
        }

        public void BagSizeChange(int newBagSize)
        {
            _expandedButtons.Clear();
            for (int i = ExpandedView.Children.Length - 1; i >= 0; i--)
            {
                ExpandedView.RemoveChild(ExpandedView.Children[i]);
            }
            for (int i = 0; i < newBagSize - 10; i++)
            {
                ButtonView b = new ButtonView(ExpandedView, new ImageView(null, AssetManager.GetTexture("white")), Vector2.Zero) { Width = 25, Height = 25 };
                new TextView(b, "10", new Vector2(0, 21), AssetManager.GetFont("Console2")) { TextColor = Color.White, Width = 25, Height = 10, TextJustification = TextJustification.Center };
                _expandedButtons.Add(b);
            }
        }

        public void RecomputeBoxes()
        {
            for (int i = 0; i < 9; i++)
            {
                ((ImageView)_buttons[i].ButtonContentview).Texture = AssetManager.GetTexture("white");
                ((TextView)_buttons[i].Children[1]).Text = "";
            }

            for (int i = 3; i < 12; i++)
            {
                ItemStack itemStack = GameWorld.ThePlayer.Inventory.Inventory[i];
                Item item = Item.GetItem(itemStack.ItemID);
                if (item == null) continue;
                Texture2D tex = AssetManager.GetTexture(item.GetAsset());
                ((ImageView)_buttons[i - 3].ButtonContentview).Texture = tex;
                ((TextView)_buttons[i - 3].Children[1]).Text = itemStack.NumberItems > 0
                                                               ? itemStack.NumberItems.ToString()
                                                               : "";
            }
        }

        public void RecomputeExpandedBoxes()
        {
            if (!blocking) return;

            int bagSize = _expandedButtons.Count;

            for (int i = 0; i < bagSize; i++)
            {
                ((ImageView)_expandedButtons[i].ButtonContentview).Texture = AssetManager.GetTexture("white");
                ((TextView)_expandedButtons[i].Children[1]).Text = "";
            }

            for (int i = 0; i < bagSize; i++)
            {
                ItemStack itemStack = GameWorld.ThePlayer.Inventory.Inventory[i + 12];
                Item item = Item.GetItem(itemStack.ItemID);
                if (item == null) continue;
                Texture2D tex = AssetManager.GetTexture(item.GetAsset());
                ((ImageView)_expandedButtons[i].ButtonContentview).Texture = tex;
                ((TextView)_expandedButtons[i].Children[1]).Text = itemStack.NumberItems > 0
                                                               ? itemStack.NumberItems.ToString()
                                                               : "";
            }
        }

        public override void Update(GameTime time)
        {
            RecomputeBoxes();
            RecomputeExpandedBoxes();

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
            int b = i + 3;
            CurSelected = b;
            GameWorld.ThePlayer.Inventory.PlayerInventorySelected = b;
            Packet p = new Packet1CSGameEvent(GameServer.GameEvents.Player_Inventory_Selection_Change, (byte)b);
            Main.clientNetworkManager.SendPacket(p);

            for (int j = 0; j < 9; j++)
            {
                _buttons[j].NinePatchNormal = GeeUI.GeeUI.NinePatchBtnDefault;
            }
            _buttons[i].NinePatchNormal = GeeUI.GeeUI.NinePatchBtnClicked;
        }
    }
}
