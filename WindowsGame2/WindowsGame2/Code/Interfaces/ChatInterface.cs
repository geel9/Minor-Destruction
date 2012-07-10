using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeeUI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Managers;
using MiningGameServer;
using MiningGameServer.Packets;
namespace MiningGame.Code.Interfaces
{
    public class ChatInterface : Interface
    {
        private static List<ChatEntry> chatEntries = new List<ChatEntry>();
        public static TextFieldView ChatEntryField = null;
        public static TextFieldView ChatLogField = null;
        public static bool ChatEntryMode = false;

        private static bool _shouldClearT = false;

        public static View MainView;

        public ChatInterface()
        {
            base.initialize(10000);
            MainView = new View(GeeUI.GeeUI.RootView) { Width = 290, Height = 200, X = 10, Y = 500 - 240 };
            ChatEntryField = new TextFieldView(MainView, new Vector2(0, 180), AssetManager.GetFont("Console")) { Width = 290, Height = 20, MultiLine = false, AutoWrap = false};
            ChatLogField = new TextFieldView(MainView, new Vector2(0, 95), AssetManager.GetFont("Console"))
                               {Width = 290, Height = 75, Editable = false, MultiLine = true};
            ChatEntryField.OnEnterPressed += (sender, e) =>
                                                 {
                                                     string text = ChatEntryField.Text;
                                                     ChatEntryField.ClearText();
                                                     HideChatEntry();

                                                     if (text == "") return;
                                                     Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_Chat, false, text);
                                                     Main.clientNetworkManager.SendPacket(pack);

                                                 };
            HideChatEntry();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (ChatEntryMode) return;
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
            //MainView.Active = ChatEntryMode;
            blocking = ChatEntryMode;
            if(ChatEntryMode && _shouldClearT)
            {
                ChatEntryField.ClearText();
                _shouldClearT = false;
            }
            base.Update(time);
        }

        public static void ShowChatEntry()
        {
            ChatEntryMode = true;
            MainView.Active = ChatEntryField.Selected = ChatEntryField.Active = ChatLogField.Active = true;
            ChatEntryField.ClearText();
            _shouldClearT = true;
        }

        public static void HideChatEntry()
        {
            ChatEntryMode = false;
            MainView.Active = ChatEntryField.Selected = ChatEntryField.Active = ChatLogField.Active = false;
            ChatEntryField.ClearText();
        }

        public static void AddChat(ChatEntry entry)
        {
            chatEntries.Add(entry);
            ChatLogField.AppendText(entry.playerName + ": " + entry.playerChat + "\n");
            int then = ChatLogField.TextLines.Length;
            ChatLogField._cursorY = then - 2;
            ChatLogField.ReEvaluateOffset();
        }

        public static void ConsoleInit()
        {
            ConsoleManager.AddConCommand("showchat", "Show the chat window", () =>
            {
                if (!InterfaceManager.blocking && Main.clientNetworkManager.NetClient != null)
                {
                    ShowChatEntry();
                }
            });
            ConsoleManager.AddConCommand("hidechat", "Hide the chat window", HideChatEntry);

            ConsoleManager.AddConCommandArgs("say", "Say something", ls =>
            {
                Packet1CSGameEvent pack = new Packet1CSGameEvent(GameServer.GameEvents.Player_Chat, false, ls[0]);
                Main.clientNetworkManager.SendPacket(pack);
            }, 1);
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
