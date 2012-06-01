using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeeUI.Managers;
using GeeUI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MiningGame.Code.Structs;
using MiningGame.Code.Managers;
using MiningGame.Code.Interfaces;
namespace MiningGame.Code.Entities
{
    public class Console : Interface
    {

        const long FLAG_CHEATS = 1;
        const long FLAG_HIDDEN = 2;
        const long FLAG_LOCKED = 4;

        public string input
        {
            get;
            set;
        }
        //public static int size = 15;
        public int currentSelection = -1;
        //List<TextView> tdO = new List<TextView>();
        //List<TextView> tdP = new List<TextView>();
        private TextFieldView tI;
        private static TextFieldView output;
        public List<string> previousCommands = new List<string>();
        public List<string> possibleCommands = new List<string>();
        SpriteFont f;

        public View MainView;

        public Console()
        {
            MainView = new View(GeeUI.GeeUI.RootView);
            MainView.Width = 800;
            MainView.Height = 100;
            MainView.Active = false;
            f = AssetManager.GetFont("Console");
            Color black = Color.Black;
            black.A = 200;
            active = false;
            output = new TextFieldView(MainView, new Vector2(0, 0), AssetManager.GetFont("Console"))
                         {Width = 800, Height = 80, Editable = false};
            tI = new TextFieldView(MainView, new Vector2(0, output.Height + 1), AssetManager.GetFont("Console"))
                     {Width = 800, Height = 20, MultiLine = false};

            tI.OnEnterPressed += new View.MouseClickEventHandler((object sender, EventArgs e) =>
                                                                     {
                                                                         Enter();
                                                                     });

            UpdateConsole();

            InputManager.BindKey(() =>
            {
                if (possibleCommands.Count == 0)
                    possibleCommands = previousCommands;
                TrimCommands();
                if (currentSelection > 0)
                {
                    currentSelection--;
                    tI.Text = possibleCommands[currentSelection];
                }
            }, Keys.Up);
            InputManager.BindKey(() =>
            {
                if (possibleCommands.Count == 0)
                    possibleCommands = previousCommands;
                TrimCommands();
                if (currentSelection < possibleCommands.Count - 1)
                {
                    currentSelection++;
                    tI.Text = possibleCommands[currentSelection];
                }
            }, Keys.Down);
            InputManager.BindMouse(() => { if (shown) { ConsoleManager.offset -= ConsoleManager.offset > 0 ? 1 : 0; UpdateConsole(); } }, MouseButton.Scrollup);
            InputManager.BindMouse(() =>
            {
                if (shown)
                {
                    int offset = ConsoleManager.offset;
                    if (offset < ConsoleManager.logNum - ConsoleManager.lines)
                    {
                        ConsoleManager.offset++;
                        UpdateConsole();
                    }
                }

            }, MouseButton.Scrolldown);
            base.depth = 100;
        }

        public bool shown = false;

        private void Enter()
        {
            if (!shown) return;
            string inp = tI.Text;

            tI.ClearText();
            input = "";
            possibleCommands = GetPossibleCommands(input);
            ConsoleManager.ConsoleInput(inp);
            previousCommands.Add(inp);
            TrimCommands();
        }

        public static void Write(string text)
        {
            if (output == null) return;
            output.AppendText(text + "\n");
            int then = output.TextLines.Length;
            output._cursorY = then - 2;
            output.ReEvaluateOffset();
        }

        private void TrimCommands()
        {
            List<string> ls = new List<string>();
            foreach (string s in previousCommands)
            {
                ls.Remove(s);
                ls.Add(s);
            }
            previousCommands = ls;
        }

        public void toggleShown()
        {
            shown = !shown;
            active = shown;
            MainView.Active = shown;
            tI.Selected = shown;
            input = "";
            possibleCommands = GetPossibleCommands(input);
            while (true)
            {
                int offset = ConsoleManager.offset;
                if (offset < ConsoleManager.logNum - ConsoleManager.lines)
                {
                    ConsoleManager.offset++;
                    UpdateConsole();
                }
                else
                {
                    break;
                }
            }
        }

        public void UpdateConsole()
        {
        }

        public override void Update(GameTime time)
        {
            input = tI.Text;
            tI.Update(time);
            //tdP.Clear();
            int i = 0;
            /*
            foreach (string s in possibleCommands)
            {
                tdP.Add(new TextDrawer(AssetManager.GetFont("Console"), s, new Vector2(1, ((ConsoleManager.lines + i) * size) + 23), Color.DarkRed, TextAlign.Left));
                i++;
            }
            if (currentSelection < tdP.Count && currentSelection >= 0)
            {
                tdP[currentSelection].color = Color.White;
            }*/
            TrimCommands();
            base.Update(time);
        }

        public List<string> GetPossibleCommands(string input)
        {
            List<string> ret = new List<string>();
            currentSelection = -1;
            if (input != "")
            {
                foreach (ConCommand c in ConsoleManager.commands)
                {
                    if (c.name.ToLower().StartsWith(input.ToLower()) && (c.flags & FLAG_HIDDEN) == 0)
                        ret.Add(c.name);
                }
                foreach (Convar c in ConsoleManager.variables)
                {
                    if (c.name.ToLower().StartsWith(input.ToLower()) && (c.flags & FLAG_HIDDEN) == 0)
                        ret.Add(c.name);
                }
            }
            else
            {

            }
            ret.Reverse();
            return ret;
        }

        public override void Draw(SpriteBatch sb)
        {
            return;
            /*
            try
            {
                if (shown)
                {
                    Vector2 B1TL = new Vector2(0, 1);
                    Vector2 B1BR = new Vector2(Main.graphics.PreferredBackBufferWidth - 1, ((ConsoleManager.lines) * size) + 5);
                    Vector2 B2TL = new Vector2(0, B1BR.Y);
                    Vector2 B2BR = new Vector2(Main.graphics.PreferredBackBufferWidth - 1, ((ConsoleManager.lines) * size) + 23);
                    DrawManager.Draw_Box(B1TL, B1BR, Color.Black, sb, 0f, 200);
                    DrawManager.Draw_Outline(B1TL, B1BR, Color.White, sb, 255);

                    if (tdP.Count > 0)
                    {
                        Vector2 TL = new Vector2(0, B2BR.Y);
                        Vector2 BR = TL + GetBottomRight();
                        BR.X += 14;
                        BR.Y += 2;
                        Vector2 TR = new Vector2(BR.X, TL.Y);
                        Vector2 BL = new Vector2(TL.X, BR.Y);
                        DrawManager.Draw_Box(TL, BR, Color.Black, sb, 0f, 200);
                        DrawManager.Draw_Outline(TL, BR, Color.White, sb, 200);
                    }

                    tI.Draw(sb);
                    foreach (TextDrawer t in tdO)
                    {
                        t.Draw(sb);
                    }
                    foreach (TextDrawer t in tdP)
                    {
                        t.Draw(sb);
                    }
                }
            }
            catch
            {

            }*/
            //base.Draw();
        }

    }
}
