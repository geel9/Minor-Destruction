using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using MiningGame.Code;

namespace MiningGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new Main())
            {
                game.Run();
                
            }
        }
    }
}

