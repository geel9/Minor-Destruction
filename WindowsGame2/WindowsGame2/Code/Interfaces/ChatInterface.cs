using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
using MiningGameServer;
using MiningGameServer.Packets;
using YogUILibrary.UIComponents;
namespace MiningGame.Code.Interfaces
{
    public class ChatInterface : Interface
    {
        public static List<ChatEntry> chatEntries = new List<ChatEntry>();
        public TextField chatEntryField = null;
        public static bool ChatEntryMode = false;

        public ChatInterface()
        {
            base.initialize(10000);
            chatEntryField = new TextField(new Vector2(10, Main.graphics.PreferredBackBufferHeight - 40), 290, 20, Color.Black, AssetManager.GetFont("Console"), (string text) =>
            {
                if (!ChatEntryMode) return;
                if (text.Length > 0)
                {
                    Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_Chat, false, text);
                    Main.clientNetworkManager.SendPacket(pack);
                }
                chatEntryField.SetText("");
                ConsoleManager.ConsoleInput("hidechat", true);
            });
            
        }

        public override void Draw(SpriteBatch sb)
        {
            if (ChatEntryMode) chatEntryField.Draw(sb);
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
            chatEntryField.SetSelected(ChatEntryMode);
            if (ChatEntryMode)
            {
                chatEntryField.Update(time);
                chatEntryField.SetSelected(true);
            }
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
