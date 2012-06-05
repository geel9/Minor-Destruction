using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeeUI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
namespace MiningGame.Code.Interfaces
{
    public class StartGameInterface : Interface
    {
        public ButtonView HostGameButton = null;
        public ButtonView JoinGameButton = null;
        public ButtonView AnimationEditorButton = null;
        public TextFieldView PlayerNameTextField = null;
        public TextFieldView ServerIPTextField = null;
        public TextFieldView ServerPortTextField = null;

        public WindowView MainView;

        public StartGameInterface()
        {
            base.initialize(1);
            base.blocking = false;

            MainView = new WindowView(GeeUI.GeeUI.RootView, Main.Center - new Vector2(200, 300),
                                      AssetManager.GetFont("Console")) { WindowText = "Start game" };

            PanelView p = new PanelView(MainView, Vector2.Zero);

            MainView.Width = 200;
            MainView.Height = 300;
            p.Width = 200;
            p.Height = 300;

            HostGameButton = new ButtonView(p, "Host", new Vector2(20, 200), AssetManager.GetFont("Console"));
            JoinGameButton = new ButtonView(p, "Connect", new Vector2(100, 200), AssetManager.GetFont("Console"));

            JoinGameButton.OnMouseClick += (sender, e) =>
            {
                Connect();
            };

            HostGameButton.OnMouseClick += (sender, e) =>
            {
                Host();
            };

            PlayerNameTextField = new TextFieldView(p, new Vector2(5, 10),
                                                        AssetManager.GetFont("Console"))
                                          {
                                              Width = 180,
                                              Height = 20,
                                              MultiLine = false
                                          };
            ServerIPTextField = new TextFieldView(p, new Vector2(5, 110), AssetManager.GetFont("Console"))
                                    {
                                        Width = 180,
                                        Height = 20,
                                        MultiLine = false
                                    };
            ServerPortTextField = new TextFieldView(p, new Vector2(5, 160), AssetManager.GetFont("Console"))
                                      {
                                          Width = 180,
                                          Height = 20,
                                          MultiLine = false
                                      };

            //AnimationEditorButton = new ButtonView(Main.Center - new Vector2(0, 200), "Animation editor", AssetManager.GetFont("Console"), () => { destroy(); AnimationEditorInterface ae = new AnimationEditorInterface(); ae.initialize(1); });

            PlayerNameTextField.Text = "Player1";
            ServerIPTextField.Text = "127.0.0.1";
            ServerPortTextField.Text = "870";
        }

        public void Host()
        {
            ConsoleManager.setVariableValue("player_name", PlayerNameTextField.Text);
            ConsoleManager.ConsoleInput("host " + ServerPortTextField.Text);
            ConsoleManager.ConsoleInput("connect 127.0.0.1 " + ServerPortTextField.Text);
            destroy();
        }

        public void Connect()
        {
            ConsoleManager.setVariableValue("player_name", PlayerNameTextField.Text);
            ConsoleManager.ConsoleInput("connect " + ServerIPTextField.Text + " " + ServerPortTextField.Text);
            destroy();
        }

        public override void destroy()
        {
            HostGameButton = null;
            JoinGameButton = null;
            PlayerNameTextField = null;
            ServerIPTextField = null;
            ServerPortTextField = null;
            AnimationEditorButton = null;
            GeeUI.GeeUI.RootView.RemoveChild(MainView);
            base.destroy();
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}
