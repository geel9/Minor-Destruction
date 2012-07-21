using System;
using MiningGameServer.ExtensionMethods;
using MiningGameServer.Structs;

namespace MiningGameServer
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

            ServerConsole.AddConVar("break", "break", "0");
            ServerConsole.AddConVar("sv_cheats", "If 1, cheats may be used.", "1");

            ServerConsole.AddConCommand("hurt", "Hurts a player", args =>
                                                                      {

                                                                          try
                                                                          {
                                                                              string player = args[0];
                                                                              int damage = Convert.ToInt32(args[1]);

                                                                              foreach (NetworkPlayer p in GameServer.NetworkPlayers)
                                                                              {
                                                                                  if (p.PlayerName.ToLower() == player)
                                                                                  {
                                                                                      p.HurtPlayer(damage);
                                                                                  }
                                                                              }
                                                                          }
                                                                          catch
                                                                          {
                                                                              ServerConsole.Log("Usage: hurt [player] [damage]");
                                                                          }
                                                                      });

            ServerConsole.AddConCommand("give", "Give a player an item", args =>
            {
                if (args.Length < 2)
                {
                    return;
                }
                string playerName = args[0].ToLower();
                int ID = Convert.ToInt32(args[1]);

                int amount = 1;
                if (args.Length > 2)
                {
                    amount = Convert.ToInt32(args[2]);
                }

                foreach (NetworkPlayer p in GameServer.NetworkPlayers)
                {
                    if (p.PlayerName.ToLower() == playerName)
                    {
                        p.Inventory.PickupItem(new ItemStack(amount, (byte)ID));
                    }
                }

            });
        }
    }
}
