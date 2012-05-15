using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Deployment;
using System.IO;

namespace MiningGame.Code.Managers
{
    public static class DirectoryManager
    {
        public static string ROOT = Directory.GetCurrentDirectory() + "\\";
        public static string CONTENT = ROOT + "Content\\";
        public static string TEXTURES = CONTENT + "textures\\";
        public static string SOUNDS = CONTENT + "sounds\\";
        public static string BLOCKS = CONTENT + "blocks\\";
        public static string RECIPES = CONTENT + "recipes\\";

        public static string CONFIG = ROOT + "config\\";
        public static string SAVEGAMES = CONFIG + "saves\\";

        public static string ANIMATIONS = CONTENT + "animations\\";
    }
}
