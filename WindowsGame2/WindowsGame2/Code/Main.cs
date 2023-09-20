using System;
using System.Collections.Generic;
using System.Linq;
using GeeUI.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using MiningGame.Code.Blocks;
using MiningGame.Code.GameModes;
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
        public const long FLAG_CHEATS = 1;
        public const long FLAG_HIDDEN = 2;
        public const long FLAG_LOCKED = 4;

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

            ConsoleManager.InitConsole();
            ConsoleManager.Log("Width: " + graphics.PreferredBackBufferWidth + " Height: " + graphics.PreferredBackBufferHeight);
            ConsoleManager.AddConVar("window_title", "Window title", "Mining thing");

            ConsoleManager.AddConCommand("reset_textures", "Reload the textures", LoadTextures);

            ConsoleManager.AddConCommandArgs("host", "Host a game", (string[] ls) =>
            {
                int port = Convert.ToInt32(ls[0]);
                GameServer = new GameServer(port);
            }, 1);

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

            ClientGameMode.GenerateGameModes();
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
            //AssetManager.Sounds.Clear(); TODO: Fix
            AssetManager.Textures.Clear();
            AssetManager.Effects.Clear();

            //SERVER Y U NO CLOSE THREAD
            Process.GetCurrentProcess().Kill();
        }

        protected override void Update(GameTime gameTime)
        {
            isActive = this.IsActive;
            Window.Title = ConsoleManager.GetVariableValue("window_title");
            int tileX = (int)InputManager.GetMousePosV().X / GameWorld.BlockSize;
            int tileY = (int)InputManager.GetMousePosV().Y / GameWorld.BlockSize;;
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
