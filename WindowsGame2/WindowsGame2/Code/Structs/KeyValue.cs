using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public struct KeyValue
    {
        public string name;
        public string value;

        public KeyValue(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
