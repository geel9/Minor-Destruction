using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer;
using MiningGameServer.ExtensionMethods;
using MiningGameserver.Structs;

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

            ServerConsole.AddConCommand("give", "Give a player an item", args =>
                                                                             {
                                                                                 string playerName = args[0].ToLower();
                                                                                 int ID = Convert.ToInt32(args[1]);
                                                                                 int amount = Convert.ToInt32(args[2]);

                                                                                 foreach(NetworkPlayer p in GameServer.NetworkPlayers)
                                                                                 {
                                                                                     if(p.PlayerName.ToLower() == playerName)
                                                                                     {
                                                                                         p.PickupItem(new ItemStack(amount, (byte) ID));
                                                                                     }
                                                                                 }

                                                                             });
        }
    }
}
