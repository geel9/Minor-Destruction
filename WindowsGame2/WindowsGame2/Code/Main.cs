using System;
using System.Collections.Generic;
using System.Linq;
using GeeUI.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using MiningGame.Code.Blocks;
using MiningGame.Code.Items;
using MiningGame.Code.Structs;
using MiningGame.Code.Managers;
using MiningGame.Code.Interfaces;
using MiningGame.Code.CInterfaces;
using System.Text.RegularExpressions;
using System.Diagnostics;
using MiningGameServer;
using MiningGameServer.Packets;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;
using MiningGameServer;
using GeeUI.Views;
using ConCommand = MiningGame.Code.Structs.ConCommand;
using Convar = MiningGame.Code.Structs.Convar;

namespace MiningGame.Code
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

        public static GameWorld theWorld = null;

        public static ClientNetworkManager clientNetworkManager = new ClientNetworkManager();

        public static GameServer GameServer = null;

        public static SoundManager SoundManager = new SoundManager();
        public static MusicManager MusicManager = new MusicManager();
        public static PauseManager PauseManager;
        public static Color BackColor = Color.SkyBlue;
        public static bool isActive = false;

        public static Random R = new Random();

        public bool InGame = false;

        public static Vector2 Center;

        public static MiningGame.Code.Entities.Console console;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferHeight = 500;
            graphics.PreferredBackBufferWidth = 800;
            isActive = IsActive;
            ServerConsole.OnConsoleOutput = s => ConsoleManager.Log("Server: " + s, Color.LightBlue);

            /* one = new ShapeLineSegment(new Vector2(0, 6),  new Vector2(10, 5));
            ShapeLineSegment two = new ShapeLineSegment(new Vector2(5, 10), new Vector2(6, 0));
            one.CollideLineSegment(two);*/
        }

        protected override void Initialize()
        {

            ConsoleManager.Log("Width: " + graphics.PreferredBackBufferWidth + " Height: " + graphics.PreferredBackBufferHeight);
            ConsoleManager.addConVar("window_title", "Window title", "Mining thing");
            ConsoleManager.addConVar("draw_hitboxes", "If 1, hitboxes are drawn.", "0", null, FLAG_CHEATS);

            ConsoleManager.addConCommand("connect", "Connect to a server", (string[] ls) =>
            {
                if (ls.Length == 2)
                {
                    string ip = ls[0];
                    int port = Convert.ToInt32(ls[1]);
                    clientNetworkManager.Connect(ip, port);
                }
            });

            ConsoleManager.addConCommand("disconnect", "Disconnect from the server", (string[] ls) => clientNetworkManager.Disconnect());

            ConsoleManager.addConVar("player_name", "Your name in a server", "Player_" + Main.R.Next(0, 1000), l =>
            {
                if (clientNetworkManager.IsConnected())
                {
                    Packet1CSGameEvent packet = new Packet1CSGameEvent(GameServer.GameEvents.Player_Change_Name, l[0]);
                    clientNetworkManager.SendPacket(packet);
                }
            });

            ConsoleManager.addConCommand("host", "Host a game", (string[] ls) =>
            {
                int port = Convert.ToInt32(ls[0]);
                GameServer = new GameServer(port);
            });

            ConsoleManager.addConCommand("exec", "Execute a file in the config folder", (string[] ls) =>
            {
                if (ls.Length == 0) { ConsoleManager.Log("usage: exec [filename(without .cfg)]", Color.Red); return; }
                string execContents = FileReaderManager.ReadFileContents(DirectoryManager.CONFIG + ls[0] + (!ls[0].Contains(".cfg") ? ".cfg" : ""));
                if (execContents != "")
                {
                    foreach (string command in execContents.Replace("\r", "").Split('\n'))
                    {
                        ConsoleManager.ConsoleInput(command, true);
                    }
                }
                else
                {
                    ConsoleManager.Log("Could not find file/file was empty: " + ls[0]);
                }
            });

            ConsoleManager.addConCommand("bind", "Bind a key", (string[] ls) =>
            {

                if (ls.Length == 1)
                {
                    string key = ls[0];
                    foreach (PlayerBind bind in PlayerBindManager.PlayerBinds)
                    {
                        if (bind.buttonBoundName.Replace("^", "") == key.Replace("^", ""))
                            ConsoleManager.Log(key + " is bound to: \"" + bind.consoleCommandOnPressed);
                    }
                }
                else if (ls.Length == 2)
                {
                    string key = ls[0];
                    string command = ls[1];
                    PlayerBindManager.BindButton(key, command);
                }
                else
                {
                    ConsoleManager.Log("Usage: bind [key] ([command])");
                    return;
                }

            });

            ConsoleManager.addConCommand("reset_textures", "Reload the textures", (string[] ls) => { LoadTextures(); });

            ConsoleManager.addConCommand("list", "List console commands and variables", (string[] ls) =>
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

            ConsoleManager.addConCommand("help", "RECURSION", (string[] ls) =>
            {
                if (ls.Length > 0)
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

            ConsoleManager.addConCommand("toggleconsole", "Toggle the console", (string[] ls) =>
            {
                if (console.shown || (!console.shown && !InterfaceManager.blocking))
                    console.toggleShown();
            });

            ConsoleManager.addConCommand("showchat", "Show the chat window", (string[] ls) =>
            {
                if (!InterfaceManager.blocking && clientNetworkManager.NetClient != null)
                {
                    ChatInterface.ShowChatEntry();
                }
            });
            ConsoleManager.addConCommand("hidechat", "Hide the chat window", (string[] ls) =>
            {
                ChatInterface.HideChatEntry();
            });

            ConsoleManager.addConCommand("say", "Say something", (string[] ls) =>
            {
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_Chat, false, ls[0]);
                clientNetworkManager.SendPacket(pack);
            });


            Center = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            PlayerBindManager.InitBinds();
            ConsoleManager.ConsoleInput("exec exec", true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GeeUI.GeeUI.Initialize(this);
            LoadTextures();

            AssetManager.LoadAsset<SpriteFont>("Console", "Console", Content);
            AssetManager.LoadAsset<SpriteFont>("Console2", "Console2", Content);
            AssetManager.LoadAsset<SpriteFont>("default", "default", Content);
            AssetManager.LoadAsset<SpriteFont>("TinyText", "TinyText", Content);
            AssetManager.LoadAsset<Effect>("CircleShader", "CircleEffect", Content);
            AssetManager.LoadAsset<Effect>("BS", "GoodButtonEffect", Content);
            AssetManager.LoadAsset<Effect>("test", "Effect1", Content);
            AssetManager.LoadAsset<Effect>("WaterLevel", "WaterLevelEffect", Content);

            GameWorld.LoadBlocks();
            Item.MakeItems();

            PauseManager = new PauseManager();
            ChatInterface chat = new ChatInterface();
            console = new MiningGame.Code.Entities.Console();
            console.initialize(100000);
            StartGameInterface sc = new StartGameInterface();
        }

        public void LoadTextures()
        {
            string path = DirectoryManager.TEXTURES;
            AssetManager.Textures.Clear();

            //Load blocks in folders
            foreach (string dir in Directory.GetDirectories(path))
            {
                foreach (string f in Directory.GetFiles(dir))
                {

                    AssetManager.LoadAsset<Texture2D>(f.Replace(dir + "\\", "").Replace(".png", ""), f.Replace(path, ""), Content);
                }
            }
            //Load blocks in the main textures folder
            foreach (string f in Directory.GetFiles(path))
            {
                AssetManager.LoadAsset<Texture2D>(f.Replace(path, "").Replace(".png", ""), f.Replace(path, ""), Content);
            }
            path = DirectoryManager.ANIMATIONS;
            AssetManager.Animations.Clear();
            foreach (string f in Directory.GetFiles(path))
            {
                AssetManager.LoadAsset<Animation>(f.Replace(path, "").Replace(".anm", ""), f.Replace(path, ""), Content);
            }
        }

        protected override void UnloadContent()
        {
            AssetManager.Fonts.Clear();
            AssetManager.Sounds.Clear();
            AssetManager.Textures.Clear();
            AssetManager.Effects.Clear();

            //SERVER Y U NO CLOSE THREAD
            Process.GetCurrentProcess().Kill();
        }

        protected override void Update(GameTime gameTime)
        {
            isActive = this.IsActive;
            Window.Title = ConsoleManager.getVariableValue("window_title");
            int tileX = (int)InputManager.GetMousePosV().X / GameWorld.BlockWidth;
            int tileY = (int)InputManager.GetMousePosV().Y / GameWorld.BlockHeight;;
            GeeUI.GeeUI.Update(gameTime);
            clientNetworkManager.Update(gameTime);

            if (GameServer != null)
                GameServer.Update(gameTime);

            for (int i = 0; i < interfaces.Count; i++)
            {
                interfaces[i].Update(gameTime);
            }
            for (int i = 0; i < updatables.Count; i++)
            {
                if (updatables[i] != null)
                    updatables[i].Update(gameTime);
            }

            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].Update(gameTime);
            }

            base.Update(gameTime);
        }

        private int numDone = 0;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackColor);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

            for (int i = 0; i < drawables.Count; i++)
            {
                if (drawables[i].inCamera() && drawables[i] != null)
                    drawables[i].Draw(spriteBatch);
            }

            foreach (Interface i in interfaces.OrderBy(x => x.depth))
            {
                i.Draw(spriteBatch);
            }

            GeeUI.GeeUI.Draw(spriteBatch);


            spriteBatch.End();

            //GraphicsDevice.SetRenderTarget(null);
            //spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null);
            //spriteBatch.Draw(rend, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            //spriteBatch.End();

            //rend.Dispose();

            base.Draw(gameTime);
        }
    }

    public enum EmitDirectionMode
    {
        Random,
        Fixed,
        Range
    }
}
