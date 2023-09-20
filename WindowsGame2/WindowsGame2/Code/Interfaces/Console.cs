using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeeUI.Managers;
using GeeUI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MiningGame.Code.CInterfaces;
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

        public static string input
        {
            get;
            set;
        }
        //public static int size = 15;
        public static int CurrentSelection = -1;
        //List<TextView> tdO = new List<TextView>();
        //List<TextView> tdP = new List<TextView>();
        private static TextFieldView _tI;
        private static TextFieldView _output;
        public List<string> PreviousCommands = new List<string>();
        public static List<string> PossibleCommands = new List<string>();
        SpriteFont f;

        public static View MainView;

        public Console()
        {
            MainView = new WindowView(GeeUI.GeeUI.RootView, Main.Center, AssetManager.GetFont("Console"));
            PanelView p = new PanelView(MainView, Vector2.Zero);
            MainView.Width = 505;
            MainView.Height = 400;
            p.Width = 505;
            p.Height = 400;
            MainView.Active = false;
            f = AssetManager.GetFont("Console");
            Color black = Color.Black;
            black.A = 200;
            active = false;
            _output = new TextFieldView(p, new Vector2(0, 0), AssetManager.GetFont("Console2"))
                         {Width = 490, Height = 330, Editable = false};
            _tI = new TextFieldView(p, new Vector2(0, _output.Height + 1), AssetManager.GetFont("Console"))
                     {Width = 490, Height = 20, MultiLine = false};

            // TODO: Fix me. I can't find any version of GeeUI where `OnEnterPressed` ever existed...
            /*_tI.OnEnterPressed += new View.MouseClickEventHandler((object sender, EventArgs e) =>
                                                                     {
                                                                         Enter();
                                                                     });*/

            UpdateConsole();

            InputManager.BindKey(() =>
            {
                if (PossibleCommands.Count == 0)
                    PossibleCommands = PreviousCommands;
                TrimCommands();
                if (CurrentSelection > 0)
                {
                    CurrentSelection--;
                    _tI.Text = PossibleCommands[CurrentSelection];
                }
            }, Keys.Up);
            InputManager.BindKey(() =>
            {
                if (PossibleCommands.Count == 0)
                    PossibleCommands = PreviousCommands;
                TrimCommands();
                if (CurrentSelection < PossibleCommands.Count - 1)
                {
                    CurrentSelection++;
                    _tI.Text = PossibleCommands[CurrentSelection];
                }
            }, Keys.Down);
            InputManager.BindMouse(() => { if (Shown) { ConsoleManager.offset -= ConsoleManager.offset > 0 ? 1 : 0; UpdateConsole(); } }, MouseButton.Scrollup);
            InputManager.BindMouse(() =>
            {
                if (Shown)
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

        public bool Shown = false;

        private void Enter()
        {
            if (!Shown) return;
            string inp = _tI.Text;

            // TODO: Fix me
            //_tI.ClearText();
            input = "";
            PossibleCommands = GetPossibleCommands(input);
            ConsoleManager.ConsoleInput(inp);
            PreviousCommands.Add(inp);
            TrimCommands();
        }

        public static void Write(string text)
        {
            if (_output == null) return;
            _output.AppendText(text + "\n");
            int then = _output.TextLines.Length;

            // TODO: fix me
            /*
            _output._cursorY = then - 1;
            _output.ReEvaluateOffset();*/
        }

        private void TrimCommands()
        {
            List<string> ls = new List<string>();
            foreach (string s in PreviousCommands)
            {
                ls.Remove(s);
                ls.Add(s);
            }
            PreviousCommands = ls;
        }

        public void ToggleShown()
        {
            Shown = !Shown;
            active = Shown;
            MainView.Active = Shown;
            _tI.Selected = Shown;
            input = "";
            PossibleCommands = GetPossibleCommands(input);
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

        public static void UpdateConsole()
        {
        }

        public override void Update(GameTime time)
        {
            input = _tI.Text;
            _tI.Update(time);
            if(Shown)
            {
                MainView.ParentView.BringChildToFront(MainView);
            }
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

        public static List<string> GetPossibleCommands(string input)
        {
            List<string> ret = new List<string>();
            CurrentSelection = -1;
            if (input != "")
            {
                foreach (ConCommand c in ConsoleManager.Commands)
                {
                    if (c.Name.ToLower().StartsWith(input.ToLower()) && (c.Flags & FLAG_HIDDEN) == 0)
                        ret.Add(c.Name);
                }
                foreach (Convar c in ConsoleManager.Variables)
                {
                    if (c.Name.ToLower().StartsWith(input.ToLower()) && (c.Flags & FLAG_HIDDEN) == 0)
                        ret.Add(c.Name);
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

        public static void ConsoleInit()
        {
            ConsoleManager.AddConCommand("toggleconsole", "Toggle the console", () =>
            {
                if (Main.console.Shown || (!Main.console.Shown && !InterfaceManager.blocking))
                    Main.console.ToggleShown();
            });
        }
    }
}
