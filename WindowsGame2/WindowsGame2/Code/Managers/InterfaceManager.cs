using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Interfaces;
namespace MiningGame.Code.Managers
{
    public static class InterfaceManager
    {
        public static List<Interface> shownInterfaces = new List<Interface>();

        public static bool blocking
        {
            get
            {
                foreach (Interface i in shownInterfaces)
                {
                    if (i.blocking && i.active) return true;
                }
                return false;
            }
        }
    }


}
