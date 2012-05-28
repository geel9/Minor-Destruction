using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Lidgren.Network;
using MiningGameServer;
using System.Threading;
using MiningGameserver;

namespace MiningGameDedicatedServer
{
    class Program
    {
        public static GameServer TheServer;
        static int Main(string[] args)
        {

            Console.Title = "Dedicated server";
            int port = 870;
            if (args.Length > 0)
            {
                port = Convert.ToInt32(args[0]);
            }
            try
            {
                TheServer = new GameServer(port);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not host server on port {0}.", port);
                return -1;
            }

            new Thread(RunServer).Start();

            while (true)
            {
                string command = Console.ReadLine();
                ServerConsole.ConsoleInput(command);
                if (GameServer.ServerNetworkManager.NetServer.Status == NetPeerStatus.NotRunning || GameServer.ServerNetworkManager.NetServer.Status == NetPeerStatus.ShutdownRequested)
                    break;
            }

            return 0;
        }

        public static void RunServer()
        {
            while (true)
            {
                if (GameServer.ServerNetworkManager.NetServer.Status == NetPeerStatus.NotRunning) break;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                TheServer.Update(null);

                stopWatch.Stop();

                TimeSpan ts = stopWatch.Elapsed;

                double sixtyFPS = (1000 / 60);
                if (ts.TotalMilliseconds < sixtyFPS)
                {
                    Thread.Sleep((int)(sixtyFPS - ts.Milliseconds));
                }
            }
        }
    }
}
