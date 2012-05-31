using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeeUI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
using MiningGameServer;
using MiningGameServer.Packets;
namespace MiningGame.Code.Interfaces
{
    public class ChatInterface : Interface
    {
        public static List<ChatEntry> chatEntries = new List<ChatEntry>();
        public TextFieldView chatEntryField = null;
        public static bool ChatEntryMode = false;

        public View MainView;

        public ChatInterface()
        {
            base.initialize(10000);
            MainView = new View(GeeUI.GeeUI.RootView) { Width = 290, Height = 200, X = 10, Y = 500 - 240 };
            chatEntryField = new TextFieldView(MainView, new Vector2(0, 180), AssetManager.GetFont("Console")) { Width = 290, Height = 20 };

        }

        public override void Draw(SpriteBatch sb)
        {
            Vector2 startDraw = new Vector2(10, Main.graphics.PreferredBackBufferHeight - 40);
            foreach (ChatEntry ce in chatEntries.Where(cel => cel.framesUntilDeath-- > 0 || ChatEntryMode).OrderByDescending(cel => cel.framesUntilDeath))
            {
                startDraw.Y -= 20;
                string firstEntry = ce.playerName + ": ";
                SpriteFont sf = AssetManager.GetFont("Console");
                Vector2 firstEntryWidth = sf.MeasureString(firstEntry);
                sb.DrawString(sf, firstEntry, startDraw, ce.playerColor);
                sb.DrawString(sf, ce.playerChat, new Vector2(startDraw.X + firstEntryWidth.X, startDraw.Y), Color.White);
            }
            base.Draw(sb);
        }

        public override void Update(GameTime time)
        {
            MainView.Active = ChatEntryMode;
            blocking = ChatEntryMode;
            base.Update(time);
        }

        public static void ShowChatEntry()
        {
            ChatEntryMode = true;
        }

        public static void HideChatEntry()
        {
            ChatEntryMode = false;
        }

    }

    public class ChatEntry
    {
        public string playerName;
        public string playerChat;
        public Color playerColor;
        public bool teamChat;
        public int framesUntilDeath;

        public ChatEntry(string name, string text, Color playercolor, bool team)
        {
            playerName = name;
            playerColor = playercolor;
            teamChat = team;
            framesUntilDeath = 500;
            playerChat = text;
        }
    }
}
