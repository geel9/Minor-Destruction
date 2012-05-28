using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public struct ConCommand
    {
        public Action<string[]> lambda;
        public string description;
        public string name;
        public long flags;

        public ConCommand(Action<string[]> lambda, string description, string name, long flags = 0)
        {
            this.lambda = lambda;
            this.description = description;
            this.name = name;
            this.flags = flags;
        }
    }
}
