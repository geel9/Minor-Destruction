using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public struct Convar
    {
        public Action<string[]> lambda;
        public string description;
        public string value;
        public string name;
        public long flags;

        public Convar(Action<string[]> lambda, string description, string defaultValue, string name, long flags = 0)
        {
            this.lambda = lambda;
            this.description = description;
            this.value = defaultValue;
            this.name = name;
            this.flags = flags;
        }

    }
}
