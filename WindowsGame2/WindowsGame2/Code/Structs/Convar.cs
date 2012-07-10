using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public struct Convar
    {
        public Action<string> Lambda;
        public string Description;
        public string Value;
        public string Name;
        public long Flags;

        public Convar(Action<string> lambda, string description, string defaultValue, string name, long flags = 0)
        {
            this.Lambda = lambda;
            this.Description = description;
            this.Value = defaultValue;
            this.Name = name;
            this.Flags = flags;
        }

    }
}
