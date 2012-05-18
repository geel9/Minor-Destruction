using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
using YogUILibrary.UIComponents;
using YogUILibrary.Managers;
namespace MiningGame.Code.Interfaces
{
    public class StartGameInterface : Interface
    {
        public Button HostGameButton = null;
        public Button JoinGameButton = null;
        public Button AnimationEditorButton = null;
        public TextField PlayerNameTextField = null;
        public TextField ServerIPTextField = null;
        public TextField ServerPortTextField = null;

        public StartGameInterface()
        {
            base.initialize(1);
            base.blocking = true;
            HostGameButton = new Button(new Vector2(Main.Center.X - 30, Main.Center.Y + 100), "Host", AssetManager.GetFont("Console"), () => { Host(); });
            JoinGameButton = new Button(new Vector2(Main.Center.X + 30, Main.Center.Y + 100), "Connect", AssetManager.GetFont("Console"), () => { Connect(); });
            PlayerNameTextField = new TextField(new Vector2(Main.Center.X - 90, Main.Center.Y), 180, 20, Color.Black, AssetManager.GetFont("Console"), null);
            ServerIPTextField = new TextField(new Vector2(Main.Center.X - 90, Main.Center.Y - 100), 180, 20, Color.Black, AssetManager.GetFont("Console"), null);
            ServerIPTextField.stringPattern = "^\\d+?\\.\\d+?\\.\\d+?\\.\\d+?$";
            ServerPortTextField = new TextField(new Vector2(Main.Center.X - 90, Main.Center.Y - 50), 180, 20, Color.Black, AssetManager.GetFont("Console"), null);
            ServerPortTextField.stringPattern = "^\\d*?$";
            AnimationEditorButton = new Button(Main.Center - new Vector2(0, 200), "Animation editor", AssetManager.GetFont("Console"), () => { destroy(); AnimationEditorInterface ae = new AnimationEditorInterface(); ae.initialize(1); });
            PlayerNameTextField.SetText("Player1");
            ServerIPTextField.SetText("127.0.0.1");
            ServerPortTextField.SetText("870");
        }

        public void Host()
        {
            ConsoleManager.setVariableValue("player_name", PlayerNameTextField.GetText());
            ConsoleManager.ConsoleInput("host " + ServerPortTextField.GetText());
            ConsoleManager.ConsoleInput("connect 127.0.0.1 " + ServerPortTextField.GetText());
            destroy();
        }

        public void Connect()
        {
            ConsoleManager.setVariableValue("player_name", PlayerNameTextField.GetText());
            ConsoleManager.ConsoleInput("connect " + ServerIPTextField.GetText() + " " + ServerPortTextField.GetText());
            destroy();
        }

        public override void destroy()
        {
            HostGameButton.active = false;
            JoinGameButton.active = false;
            PlayerNameTextField.active = false;
            ServerIPTextField.active = false;
            ServerPortTextField.active = false;
            AnimationEditorButton.active = false;
            HostGameButton = null;
            JoinGameButton = null;
            PlayerNameTextField = null;
            ServerIPTextField = null;
            ServerPortTextField = null;
            AnimationEditorButton = null;
            base.destroy();
        }

        public override void Update(GameTime time)
        {
            HostGameButton.Update(time);
            JoinGameButton.Update(time);
            PlayerNameTextField.Update(time);
            ServerIPTextField.Update(time);
            ServerPortTextField.Update(time);
            AnimationEditorButton.Update(time);
            base.Update(time);
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawManager.Draw_Box(Main.Center, 200, 300, Color.Black, sb, 0f, 200);
            DrawManager.Draw_Outline(Main.Center, 200, 300, Color.Black, sb);
            HostGameButton.Draw(sb);
            JoinGameButton.Draw(sb);
            PlayerNameTextField.Draw(sb);
            ServerIPTextField.Draw(sb);
            ServerPortTextField.Draw(sb);
            AnimationEditorButton.Draw(sb);
            base.Draw(sb);
        }
    }
}
