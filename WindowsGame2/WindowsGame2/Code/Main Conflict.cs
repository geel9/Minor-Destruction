using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Windows;
using RatingTDS.Code;
using RatingTDS.Code.Structs;
using RatingTDS.Code.Entities;
using RatingTDS.Code.Managers;
using RatingTDS.Code.UIComponents;
using RatingTDS.Code.Interfaces;
using RatingTDS.Code.Server;
using RatingTDS.Code.CInterfaces;
using System.Text.RegularExpressions;
using System.Reflection;
//using FarseerPhysics;
//using FarseerPhysics.Dynamics;
namespace RatingTDS
{

    /// <summary>
    /// This is the main type for your games
    /// </summary>
    /// 

    public class Main : Microsoft.Xna.Framework.Game
    {
        const long FLAG_CHEATS = 1;
        const long FLAG_HIDDEN = 2;
        const long FLAG_LOCKED = 4;


        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        public static List<Drawable> drawables = new List<Drawable>();
        public static List<Updatable> updatables = new List<Updatable>();
        public static List<Manager> managers = new List<Manager>();
        public static List<Interface> interfaces = new List<Interface>();

        //        public static World gameWorld = new World(Vector2.Zero);

        public static MouseInputManager MouseInputManager = new MouseInputManager();
        public static KeyInputManager KeyInputManager = new KeyInputManager();
        public static SoundManager SoundManager = new SoundManager();
        public static MusicManager MusicManager = new MusicManager();
        public static PauseManager PauseManager;
        public static Color backColor = Color.White;

        public static short BUILDNUMBER = 1;

        public static List<EquationResult> equations = new List<EquationResult>();

        public static Random r = new Random();

        public bool inGame = false;

        public static Vector2 center;

        public static RatingTDS.Code.Entities.Console console;

        List<String> GateTypes = new List<String>();

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 1200;
            System.Windows.Forms.Control f = System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            System.Drawing.Point p = f.Location;
        }

        public static void RecomputeEquations(List<string> equ)
        {
            equations.Clear();
            foreach (string e in equ)
            {
                equations.Add(new EquationResult(xWindow, yWindow, xDist, yDist, e, incrementSize, center));
            }
        }

        protected override void Initialize()
        {
            //TODO: Add your initialization logic here
            ConsoleManager.Log("Width: " + graphics.PreferredBackBufferWidth + " Height: " + graphics.PreferredBackBufferHeight);
            ConsoleManager.addConVar("window_title", "Window title", "Totally not Zombeats again");
            ConsoleManager.addConVar("sv_cheats", "If 1, cheats may be used.", "1");
            ConsoleManager.addConVar("draw_hitboxes", "If 1, hitboxes are drawn.", "0", null, FLAG_CHEATS);

            ConsoleManager.addConCommand("connect", "Connect to a server", (List<string> ls) =>
            {
                if (ls.Count == 2)
                {
                    string ip = ls[0];
                    int port = Convert.ToInt32(ls[1]);
                    ClientNetworkManager.Connect(ip, port);
                }
            });

            ConsoleManager.addConCommand("disconnect", "Disconnect from the server", (List<string> ls) =>
            {
                ClientNetworkManager.Disconnect();
            });

            ConsoleManager.addConCommand("host", "Host a game", (List<string> ls) =>
            {
                int port = Convert.ToInt32(ls[0]);
                ServerNetworkManager.Host(port);
            });

            ConsoleManager.addConCommand("music", "Play music", (List<string> ls) =>
            {
                MusicManager.SetSong(ls[0]);
                MusicManager.Play();
            });

            ConsoleManager.addConCommand("togglepause", "Toggle pause", (List<string> ls) => { PauseManager.TogglePaused(); });

            ConsoleManager.addConCommand("list", "List console commands and variables", (List<string> ls) =>
            {
                ConsoleManager.Log("Name                Description                Flags");
                ConsoleManager.Log("----------------------------------------------------");
                foreach (ConCommand c in ConsoleManager.commands)
                {
                    string r = ConsoleManager.Align(c.name, c.description, 20);
                    string flags = ConsoleManager.getFlagString(c.name);
                    string r2 = ConsoleManager.Align(r, flags, 20);
                    if ((c.flags & FLAG_HIDDEN) == 0)
                        ConsoleManager.Log(r2);
                }
                foreach (Convar c in ConsoleManager.variables)
                {
                    string r = ConsoleManager.Align(c.name, c.description, 20);
                    string flags = ConsoleManager.getFlagString(c.name);
                    string r2 = ConsoleManager.Align(r, flags, 20);
                    if ((c.flags & FLAG_HIDDEN) == 0)
                        ConsoleManager.Log(r2);
                }
            });

            ConsoleManager.addConCommand("add_keyvalue", "Adds a keyvalue!", (List<string> ls) =>
            {
                string name = ls[0];
                string value = ls[1];
                SaveGameManager.SetValue(name, value);
            });

            ConsoleManager.addConCommand("change_keyvalue", "Changes a keyvalue!", (List<string> ls) =>
            {
                string name = ls[0];
                string value = ls[1];
                SaveGameManager.SetValue(name, value);
            });

            ConsoleManager.addConCommand("remove_keyvalue", "Removes a keyvalue!", (List<string> ls) =>
            {
                string name = ls[0];
                SaveGameManager.RemoveValue(name);
            });

            ConsoleManager.addConCommand("read_keyvalues", "Reads the keyvalues!", (List<string> ls) =>
            {
                List<KeyValue> kvs = SaveGameManager.GetKeyValues();
                foreach (KeyValue k in kvs)
                {
                    ConsoleManager.Log(k.name + ": " + k.value);
                }
            });

            ConsoleManager.addConCommand("teststuff", "test", (List<string> ls) =>
            {
                string regex = "([\\t ]+)?\"?(.+?)\"?";
                string input = "bind \"mouse1\" \"kill; help; I love you;\"";
                string output = "";
                Regex r = new Regex(regex);
                MatchCollection mc = r.Matches(input);
                foreach (Match m in mc)
                {
                    output += m.Groups[2].Value + " ";
                }
                ConsoleManager.Log(output);
            });

            ConsoleManager.addConVar("curEquation", "Current y equation", "y=x", (List<string> ls) =>
            {
                List<string> equ = ls.ToList<string>();
                RecomputeEquations(equ);
            });

            ConsoleManager.addConCommand("help", "help help help help help help help help", (List<string> ls) =>
            {
                if (ls.Count > 0)
                {
                    string name = ls[0];
                    string description = "";
                    bool isVar = ConsoleManager.isVariable(name);
                    long flags = ConsoleManager.getFlags(name);
                    bool cheats = (flags & FLAG_CHEATS) > 0;
                    bool hidden = (flags & FLAG_HIDDEN) > 0;
                    bool locked = (flags & FLAG_LOCKED) > 0;
                    if (isVar)
                    {
                        description = ConsoleManager.getVariable(name).description;
                    }
                    else if (ConsoleManager.isCommand(name))
                    {
                        description = ConsoleManager.getCommand(name).description;
                    }
                    ConsoleManager.Log(name + ": " + description + ((isVar) ? "(value: " + ConsoleManager.getVariableValue(name) + ")" : "") + " cheats: " + cheats + " hidden: " + hidden + " locked: " + locked);
                }
                else
                {
                    ConsoleManager.Log("Usage: help [command name]", Color.Red);
                }
            });


            center = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);


            MouseInputManager.BindMouse(() =>
            {
                //Box b = new Box(MouseInputManager.GetMousePosV(), 1f);
            }, MouseButton.Left, true);

            KeyInputManager.BindKey(() =>
            {
                CameraManager.moveCamera(-5, 0);
            }, Keys.Left, true);
            KeyInputManager.BindKey(() =>
            {
                CameraManager.moveCamera(5, 0);
            }, Keys.Right, true);

            KeyInputManager.BindKey(() =>
            {
                CameraManager.moveCamera(0, -5);
            }, Keys.Up, true);

            KeyInputManager.BindKey(() =>
            {
                CameraManager.moveCamera(0, 5);
            }, Keys.Down, true);

            KeyInputManager.BindKey(() =>
            {
                CameraManager.cameraZoom -= 0.1f;
            }, Keys.H, true);

            KeyInputManager.BindKey(() =>
            {
                CameraManager.cameraZoom += 0.1f;
            }, Keys.G, true);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            string path = DirectoryManager.TEXTURES;
            foreach (string f in Directory.GetFiles(path))
            {
                AssetManager.LoadAsset<Texture2D>(f.Replace(path, "").Replace(".png", ""), f.Replace(path, ""), Content);
            }


            AssetManager.LoadAsset<SpriteFont>("Console", "Console", Content);
            AssetManager.LoadAsset<SpriteFont>("default", "default", Content);

            AssetManager.LoadAsset<Effect>("CircleShader", "CircleEffect", Content);
            AssetManager.LoadAsset<Effect>("BS", "GoodButtonEffect", Content);

            PauseManager = new PauseManager();
            console = new RatingTDS.Code.Entities.Console();
        }

        protected override void UnloadContent()
        {
            AssetManager.Fonts.Clear();
            AssetManager.Sounds.Clear();
            AssetManager.Textures.Clear();
            AssetManager.Effects.Clear();
        }

        protected override void Update(GameTime gameTime)
        {
            Point mouse = MouseInputManager.GetMousePos();
            //   ConsoleManager.setVariableValue("window_title", "(" + mouse.X + ", " + mouse.Y + ")");
            Window.Title = ConsoleManager.getVariableValue("window_title");
            for (int i = 0; i < interfaces.Count; i++)
            {
                interfaces[i].Update(gameTime);
            }
            for (int i = 0; i < updatables.Count; i++)
            {
                updatables[i].Update(gameTime);
            }
            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].Update(gameTime);
            }
            console.Update(gameTime);

            //if (!PauseManager.paused) gameWorld.Step(1f);
            base.Update(gameTime);
        }

        public static Vector2 yWindow
        {
            get
            {
                return new Vector2(-100, 100);
            }
        }

        public static Vector2 xWindow
        {
            get
            {
                return new Vector2(-100, 100);
            }
        }

        public static Vector2 xDist = new Vector2();
        public static Vector2 yDist = new Vector2();

        public static double incrementSize
        {
            get
            {
                return ((-xDist.X + xDist.Y) / 20);
            }
        }
        public static double time = 0;
        public static double timeI = 0;
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(backColor);

            spriteBatch.Begin();

            timeI++;
            if (timeI >= 2)
            {
                time+= 0.1;
                timeI = 0;
            }

            List<string> equ = new List<string>();

            DrawManager.Draw_Line(new Vector2(0, center.Y), new Vector2(center.X * 2, center.Y), Color.Black, spriteBatch);
            DrawManager.Draw_Line(new Vector2(center.X, 0), new Vector2(center.X, center.Y * 2), Color.Black, spriteBatch);

            Vector2 bRight = center * 2;
            xDist = new Vector2((center.X / xWindow.X), center.X / xWindow.Y);
            yDist = new Vector2((center.Y / yWindow.X), center.Y / yWindow.Y);

            for (int i = 0; i < xWindow.Y; i++)
            {
                Vector2 newPt = new Vector2(xDist.Y * (i + 1), center.Y);
                newPt.X = center.X + newPt.X;
                DrawManager.Draw_Line(new Vector2(newPt.X, newPt.Y - 4), new Vector2(newPt.X, newPt.Y + 3), Color.Black, spriteBatch);
            }

            for (int i = (int)xWindow.X; i < 0; i++)
            {
                Vector2 newPt = new Vector2(xDist.X * (i + 1), center.Y);
                //  newPt.X = center.X - newPt.X;
                DrawManager.Draw_Line(new Vector2(newPt.X, newPt.Y - 4), new Vector2(newPt.X, newPt.Y + 3), Color.Black, spriteBatch);
            }

            for (int i = 0; i < yWindow.Y; i++)
            {
                Vector2 newPt = new Vector2(center.X, yDist.Y * (i + 1));
                newPt.Y = center.X - newPt.Y;
                DrawManager.Draw_Line(new Vector2(newPt.X - 3, newPt.Y), new Vector2(newPt.X + 4, newPt.Y), Color.Black, spriteBatch);
            }

            for (int i = (int)yWindow.X; i < 0; i++)
            {
                Vector2 newPt = new Vector2(center.X, yDist.X * (i + 1));
                //  newPt.X = center.X - newPt.X;
                DrawManager.Draw_Line(new Vector2(newPt.X - 3, newPt.Y), new Vector2(newPt.X + 4, newPt.Y), Color.Black, spriteBatch);
            }

            Vector2 startDraw = new Vector2(20, 0);
            foreach (EquationResult e in equations)
            {
                e.recomputeEquation();
                startDraw += new Vector2(0,30);
                DrawManager.Draw_Box(startDraw, 20, 10, e.drawColor, spriteBatch);
                DrawManager.Draw_Outline(startDraw, 20, 10, Color.Black, spriteBatch);
                spriteBatch.DrawString(AssetManager.GetFont("Console"), e.equation, new Vector2(startDraw.X + 13, startDraw.Y - 10), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Vector2 lastPoint = new Vector2(133777, 133777);
                foreach (Vector2 v in e.points)
                {
                    //DrawManager.Draw_Box(
                   // DrawManager.Draw_Box(new Vector2((float)(v.X), (float)(v.Y)), 1, 1, e.drawColor, spriteBatch);
                    if (lastPoint != new Vector2(133777, 133777))
                    {
                        DrawManager.Draw_Line(new Vector2((float)v.X, (float)v.Y), lastPoint, e.drawColor, spriteBatch);
                    }
                    lastPoint = new Vector2((float)v.X, (float)v.Y);
                }
            }


            for (int i = 0; i < drawables.Count; i++)
            {
                if (drawables[i].inCamera())
                    drawables[i].Draw(spriteBatch);
            }
            foreach (Interface i in interfaces.OrderBy(x => x.depth))
            {
                i.Draw(spriteBatch);
            }

            console.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public enum MouseButton
    {
        Left,
        Middle,
        Right,
        Scrollup,
        Scrolldown,
        Scroll,
        Movement
    }
    public enum EmitDirectionMode
    {
        Random,
        Fixed,
        Range
    }
    public enum TextAlign
    {
        Left,
        Center,
        Right
    }

    public enum Player
    {
        Attacker,
        Defender
    }
}
