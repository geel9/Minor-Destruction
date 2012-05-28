using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer;
using MiningGameServer.ExtensionMethods;

namespace MiningGameserver.ServerCommands
{
    public static class ServerCommands
    {
        public static void Initialize()
        {
            ServerConsole.commands.Clear();
            ServerConsole.variables.Clear();
            ServerConsole.AddConCommand("test", "TESTIFICATE", args => ServerConsole.Log("HI!"));
            ServerConsole.AddConCommand("say", "Send a message to all players", args => GameServer.SendMessageToAll(args.ConcatString(" ")));
            ServerConsole.AddConCommand("quit", "Quit the server", args =>
            {
                Console.WriteLine("Quitting...");
                GameServer.ServerNetworkManager.NetServer.Shutdown("bye");
            });
        }
    }
}
