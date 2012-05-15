using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using MiningGame.Code.Items;
using MiningGame.Code.Structs;
using MiningGame.Code.Managers;
using MiningGame.Code.Interfaces;
using MiningGame.Code.Server;
using MiningGame.Code.CInterfaces;
using System.Text.RegularExpressions;
using MiningGame.Code.Packets;
using System.Diagnostics;
using YogUILibrary;
using YogUILibrary.Managers;

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
        public static ServerNetworkManager serverNetworkManager = new ServerNetworkManager();
        // public static World gameWorld = new World(new Vector2(0f, 0.1f));

        public static SoundManager SoundManager = new SoundManager();
        public static MusicManager MusicManager = new MusicManager();
        public static PauseManager PauseManager;
        public static Color backColor = Color.SkyBlue;
        public static bool isActive = false;

        public static Random r = new Random();

        public bool inGame = false;

        public static Vector2 center;

        public static MiningGame.Code.Entities.Console console;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferHeight = 500;
            graphics.PreferredBackBufferWidth = 800;
            isActive = this.IsActive;
        }

        protected override void Initialize()
        {
            //TODO: Add your initialization logic here
            ConsoleManager.Log("Width: " + graphics.PreferredBackBufferWidth + " Height: " + graphics.PreferredBackBufferHeight);
            ConsoleManager.addConVar("window_title", "Window title", "Mining thing");
            ConsoleManager.addConVar("sv_cheats", "If 1, cheats may be used.", "1");
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

            ConsoleManager.addConCommand("disconnect", "Disconnect from the server", (string[] ls) =>
            {
                clientNetworkManager.Disconnect();
            });

            ConsoleManager.addConVar("player_name", "Your name in a server", "Player_" + Main.r.Next(0, 1000));

            ConsoleManager.addConCommand("qhc", "Quick Host Connect", (string[] ls) =>
            {
                ConsoleManager.ConsoleInput("host 870", true);
                ConsoleManager.ConsoleInput("connect 127.0.0.1 870", true);
            });

            ConsoleManager.addConCommand("rcp", "recipes", (string[] ls) =>
            {
                GameServer.LoadRecipes();
            });

            ConsoleManager.addConCommand("host", "Host a game", (string[] ls) =>
            {
                int port = Convert.ToInt32(ls[0]);
                serverNetworkManager.Host(port);
                GameServer server = new GameServer();

            });

            ConsoleManager.addConCommand("music", "Play music", (string[] ls) =>
            {
                MusicManager.SetSong(ls[0]);
                MusicManager.Play();
            });

            ConsoleManager.addConCommand("log", "Log to the console", (string[] ls) =>
            {
                string fullLog = "";
                foreach (string s in ls) { fullLog += s; }
                ConsoleManager.Log(fullLog);
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

            ConsoleManager.addConCommand("togglepause", "Toggle pause", (string[] ls) => { PauseManager.TogglePaused(); });

            ConsoleManager.addConVar("player_height", "playerheight", "14");
            ConsoleManager.addConVar("player_width", "playerwidth", "10");

            ConsoleManager.addConVar("curblock", "curblock", "0");

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

            ConsoleManager.addConCommand("add_keyvalue", "Adds a keyvalue!", (string[] ls) =>
            {
                string name = ls[0];
                string value = ls[1];
                ConsoleManager.Log("Name: " + name + " Value: " + value);
            });

            ConsoleManager.addConCommand("change_keyvalue", "Changes a keyvalue!", (string[] ls) =>
            {
                string name = ls[0];
                string value = ls[1];
                SaveGameManager.SetValue(name, value);
            });

            ConsoleManager.addConCommand("reset_blocks", "Reload the blocks", (string[] s) => GameWorld.LoadBlocks());

            ConsoleManager.addConVar("do_update", "update", "1");

            ConsoleManager.addConCommand("remove_keyvalue", "Removes a keyvalue!", (string[] ls) =>
            {
                string name = ls[0];
                SaveGameManager.RemoveValue(name);
            });

            ConsoleManager.addConCommand("give", "Give a player an item", (string[] ls) =>
            {
                string playerName = ls[0];
                int itemID = Convert.ToInt32(ls[1]);
                int itemAmount = Convert.ToInt32(ls[2]);

                foreach (NetworkPlayer s in GameServer.NetworkPlayers)
                {
                    if (s.PlayerEntity.PlayerName == playerName)
                    {
                        s.PickupItem(new ItemStack(itemAmount, (byte)itemID));
                    }
                }
            });

            ConsoleManager.addConCommand("read_keyvalues", "Reads the keyvalues!", (string[] ls) =>
            {
                List<KeyValue> kvs = SaveGameManager.GetKeyValues();
                foreach (KeyValue k in kvs)
                {
                    ConsoleManager.Log(k.name + ": " + k.value);
                }
            });

            ConsoleManager.addConCommand("teststuff", "test", (string[] ls) =>
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
            ConsoleManager.addConCommand("help", "help help help help help help help help", (string[] ls) =>
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
                if (!InterfaceManager.blocking && clientNetworkManager.netClient != null)
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


            center = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            PlayerBindManager.InitBinds();
            ConsoleManager.ConsoleInput("exec exec", true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            YogUI.YogUI_LoadContent(this);
            LoadTextures();

            YogUI.btn_normal.LoadFromTexture(AssetManager.GetTexture("btn_default_normal_holo.9"));
            YogUI.btn_hover.LoadFromTexture(AssetManager.GetTexture("btn_default_focused_holo.9"));

            AssetManager.LoadAsset<SpriteFont>("Console", "Console", Content);
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
            int tileY = (int)InputManager.GetMousePosV().Y / GameWorld.BlockHeight;
            byte MD = GameWorld.GetBlockMDAt(tileX, tileY);
            byte bid = GameWorld.GetBlockIDAt(tileX, tileY);
            //Window.Title = "Tile X: " + tileX + " y: " + tileY + " ID: " + bid + " MD: " + MD + " update scheduled: " + GameServer.updateScheduled(tileX, tileY);
            YogUI.YogUI_Update(gameTime);
            clientNetworkManager.Update(gameTime);
            serverNetworkManager.Update(gameTime);

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
            //RenderTarget2D rend = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 1, RenderTargetUsage.PreserveContents);

            //GraphicsDevice.SetRenderTarget(rend);


            GraphicsDevice.Clear(backColor);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null);

             for (int i = 0; i < drawables.Count; i++)
             {
                 if (drawables[i].inCamera() && drawables[i] != null)
                     drawables[i].Draw(spriteBatch);
             }
             foreach (Interface i in interfaces.OrderBy(x => x.depth))
             {
                 i.Draw(spriteBatch);
             }

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
