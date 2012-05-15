using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fastJSON;

namespace MiningGame.Code.Managers
{
    public static class JSONManager
    {
        public static string Serialize(object toSerialize)
        {
            return fastJSON.JSON.Instance.ToJSON(toSerialize);
        }

        public static object Deserialize(string toUnserialize)
        {
            return fastJSON.JSON.Instance.ToObject(toUnserialize);
        }
    }
}
