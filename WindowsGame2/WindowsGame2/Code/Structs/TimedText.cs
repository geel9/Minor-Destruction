using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public struct TimedText
    {
        public string text;
        public int delay;

        public TimedText(string text, int delay)
        {
            this.text = text;
            this.delay = delay;
        }
    }
}
