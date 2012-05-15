using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGame.Code.Structs
{
    public struct LogText
    {
        public Color color;
        public string text;

        public LogText(string text, Color color)
        {
            this.color = color;
            this.text = text;
        }

    }
}
