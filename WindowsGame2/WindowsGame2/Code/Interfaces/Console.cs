using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using YogUILibrary.UIComponents;
using YogUILibrary.Managers;
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
        public static int size = 15;
        public int currentSelection = -1;
        List<TextDrawer> tdO = new List<TextDrawer>();
        List<TextDrawer> tdP = new List<TextDrawer>();
        private TextField tI;
        public List<string> previousCommands = new List<string>();
        public List<string> possibleCommands = new List<string>();
        SpriteFont f;

        public Console()
        {
            f = AssetManager.GetFont("Console");
            Color black = Color.Black;
            black.A = 200;
            active = false;
            tI = new TextField(new Vector2(0, ((ConsoleManager.lines) * size) + 3), Main.graphics.PreferredBackBufferWidth - 1, 20, black, AssetManager.GetFont("Console"), (string s) => { Enter(); }, (string text) => { possibleCommands = getPossibleCommands(text); });
            tI.SetActive(shown);
            tI.SetSelected(shown);
            UpdateConsole();

            InputManager.BindKey(() =>
            {
                if (possibleCommands.Count == 0)
                    possibleCommands = previousCommands;
                TrimCommands();
                if (currentSelection > 0)
                {
                    currentSelection--;
                    tI.SetText(possibleCommands[currentSelection]);
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
                    tI.SetText(possibleCommands[currentSelection]);
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
            if (shown)
            {
                string inp = tI.GetText();
                tI.SetText("");
                input = "";
                possibleCommands = getPossibleCommands(input);
                ConsoleManager.ConsoleInput(inp);
                previousCommands.Add(inp);
                TrimCommands();
            }
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
            tI.SetActive(shown);
            tI.SetSelected(shown);
            input = "";
            //tI.SetText("");
            possibleCommands = getPossibleCommands(input);
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
            tdO.Clear();
            int i = 0;
            foreach (LogText t in ConsoleManager.GetLog())
            {
                tdO.Add(new TextDrawer(f, t.text, new Vector2(0, i * 15), t.color, TextAlign.Left));
                i++;
            }
        }

        public override void Update(GameTime time)
        {
            input = tI.GetText();
            tI.Update(time);
            //tI.position = new Vector2(0, ((ConsoleManager.lines) * size) + 3);
            tdP.Clear();
            int i = 0;
            foreach (string s in possibleCommands)
            {
                tdP.Add(new TextDrawer(AssetManager.GetFont("Console"), s, new Vector2(1, ((ConsoleManager.lines + i) * size) + 23), Color.DarkRed, TextAlign.Left));
                i++;
            }
            if (currentSelection < tdP.Count && currentSelection >= 0)
            {
                tdP[currentSelection].color = Color.White;
            }
            TrimCommands();
            base.Update(time);
        }

        public List<string> getPossibleCommands(string input)
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

            }
            //base.Draw();
        }

        public Vector2 GetBottomRight()
        {
            Vector2 ret = Vector2.Zero;
            float width = 0;
            float height = possibleCommands.Count * 15;
            ret.Y = height;
            SpriteFont f = AssetManager.GetFont("Console");
            foreach (string s in possibleCommands)
            {
                float w = f.MeasureString(s).X;
                if (w > width)
                    width = w;
            }
            ret.X = width;
            return ret;
        }


    }
}
