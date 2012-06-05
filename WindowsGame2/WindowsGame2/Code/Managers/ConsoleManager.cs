using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Structs;
using MiningGame.Code.Entities;
using Microsoft.Xna.Framework;
using MiningGameServer;
using Console = MiningGame.Code.Entities.Console;


namespace MiningGame.Code.Managers
{
    public static class ConsoleManager
    {
        const long FLAG_CHEATS = 1;
        const long FLAG_HIDDEN = 2;
        const long FLAG_LOCKED = 4;

        public static List<ConCommand> commands = new List<ConCommand>();
        public static List<Convar> variables = new List<Convar>();
        public static List<LogText> logs = new List<LogText>();


        public static string output = "";
        public static int lines = 10;
        public static int offset = 0;
        public static int logNum = 0;

        public static void addConCommand(string name, string description, Action<string[]> lambda, long flags = 0)
        {
            if (!isCommand(name))
            {
                ConCommand cc = new ConCommand(lambda, description, name, flags);
                commands.Add(cc);
            }
        }

        public static void addConVar(string name, string description, string DefaultValue, Action<string[]> lambda = null, long flags = 0)
        {
            if (!isVariable(name))
            {
                Convar cv = new Convar(lambda, description, DefaultValue, name, flags);
                variables.Add(cv);
            }
        }

        public static bool isCommand(string command)
        {
            foreach (ConCommand c in commands)
            {
                if (command == c.name)
                    return true;
            }
            return false;
        }

        public static bool isVariable(string var)
        {
            foreach (Convar c in variables)
            {
                if (var == c.name)
                    return true;
            }
            return false;
        }

        public static void executeCommand(string name, string[] arguments)
        {
            bool cheats = getVariableBool("sv_cheats");
            bool needsCheats = (getFlags(name) & FLAG_CHEATS) > 0;
            if ((!needsCheats && !cheats) || cheats)
            {
                getCommand(name).lambda(arguments);
            }
            else
            {
                Log("Cheats must be enabled for " + name);
            }
        }

        public static void executeVariable(string name, string[] arguments)
        {
            bool cheats = getVariableBool("sv_cheats");
            bool needsCheats = (getFlags(name) & FLAG_CHEATS) > 0;
            Convar c = variables[variables.IndexOf(getVariable(name))];
            if (arguments.Length > 0)
            {
                if ((!needsCheats && !cheats) || cheats)
                {
                    setVariableValue(name, ListToString(arguments, " "));
                    if (c.lambda != null)
                        c.lambda(arguments);
                }
                else
                {
                    Log("Cheats must be enabled to change " + name);
                }
            }
            else
            {
                ConsoleInput("help " + name, true);
            }
        }

        public static string getVariableValue(string name)
        {
            return getVariable(name).value;
        }

        public static bool getVariableBool(string name)
        {
            return getVariable(name).value.Replace(" ", "") == "1" ? true : false;
        }

        public static float getVariableFloat(string name)
        {
            return (float)Convert.ToDouble(getVariable(name).value);
        }

        public static double getVariableDouble(string name)
        {
            return Convert.ToDouble(getVariable(name).value);
        }

        public static int getVariableInt(string name)
        {
            return Convert.ToInt32(getVariable(name).value);
        }

        public static string getFlagString(string name)
        {
            long flags = getFlags(name);
            string ret = "";

            ret += (flags & FLAG_CHEATS) > 0 ? "FLAG_CHEATS " : "";
            ret += (flags & FLAG_HIDDEN) > 0 ? "FLAG_HIDDEN " : "";
            ret += (flags & FLAG_LOCKED) > 0 ? "FLAG_LOCKED " : "";

            return ret;
        }

        public static long getFlags(string name)
        {
            if (isVariable(name))
            {
                return getVariable(name).flags;
            }
            else if (isCommand(name))
            {
                return getCommand(name).flags;
            }
            else
                return 0;
        }

        public static void setVariableValue(string name, string value)
        {
            Convar c = getVariable(name);
            Convar c2 = c;
            c2.value = value;
            variables.Add(c2);
            variables.Remove(c);
        }

        public static ConCommand getCommand(string name)
        {
            foreach (ConCommand c in commands)
            {
                if (c.name == name)
                {
                    return c;
                }
            }
            return new ConCommand((string[] l) => { }, "", "");
        }

        public static Convar getVariable(string name)
        {
            try
            {
                foreach (Convar c in variables)
                {
                    if (c.name == name)
                    {
                        return c;
                    }
                }
            }
            catch (Exception)
            {
                return new Convar((string[] l) => { }, "", "", "");
            }
            return new Convar((string[] l) => { }, "", "", "");
        }

        public static string Align(string one, string two, int dist, string delimiter = " ")
        {
            string ret = one;
            int amount = dist - one.Length;
            if (amount < 0)
                amount = 1;
            for (int i = 0; i < amount; i++)
            {
                ret += " ";
            }
            ret += two;
            return ret;
        }

        public static List<LogText> GetLog()
        {
            List<LogText> ll = new List<LogText>();
            for (int i = 0; i < lines; i++)
            {
                //Get the lines according to the offset; we'd get the first 4 lines here if offset = 0, but if offset = 1, we'd get lines 2-5.
                if ((i + offset) < logs.Count)
                {
                    ll.Add(logs[i + offset]);
                }
            }

            return ll;
        }

        public static string[] removeFromList(string[] list, string remove)
        {
            List<string> ret = new List<string>();
            foreach (string s in list)
            {
                if (s != remove)
                    ret.Add(s);
            }
            return ret.ToArray();
        }

        public static string ListToString(IEnumerable<string> list, string delimiter, int limit = 1000, string breakString = "...")
        {
            string ret = "";
            int num = 0;
            foreach (string s in list)
            {
                num++;
                if (num > limit)
                {
                    ret += breakString;
                    break;
                }
                ret += s + delimiter;
            }
            return ret;

        }

        public static void Log(string log)
        {
            Console.Write(log);
        }

        public static void Log(string log, Color color)
        {
            Log(log);
            /*int numAllowed = ((Main.graphics.PreferredBackBufferWidth - 5) / 8);
            int size = log.Length;
            if (size > numAllowed)
            {
                string first = log.Substring(0, numAllowed - 1);
                string second = log.Substring(numAllowed - 1);
                Log(first + "-", color);
                Log("-" + second, color);
            }
            else
            {
                logNum++;
                logs.Add(new LogText(log, color));
                if (logNum - lines > offset)
                    offset++;
            }
            if (Main.console != null)
                Main.console.UpdateConsole();*/

        }

        public static string[] splitConsoleInput_Semicolons(string input)
        {
            List<string> ret = new List<string>();
            string curCommand = "";
            char stringChar = '"';
            bool inString = false;

            foreach (char c in input)
            {
                curCommand += c;
                if (c == '"' || c == '\'')
                {
                    if (inString)
                    {
                        if (stringChar == c)
                        {
                            inString = false;
                        }
                    }
                    else
                    {
                        inString = true;
                        stringChar = c;
                    }
                }
                if (c == ';' && !inString)
                {
                    ret.Add(curCommand.Trim().TrimEnd(';'));
                    curCommand = "";
                }
            }
            curCommand = curCommand.Trim();
            if (curCommand != "")
            {
                ret.Add(curCommand);
            }

            return ret.ToArray();
        }

        public static string[] splitConsoleInput_Arguments(string input)
        {
            List<string> ret = new List<string>();
            string curCommand = "";
            char stringChar = 's';

            foreach (char c in input)
            {
                bool addChar = true;
                if (c == '"' || c == '\'')
                {
                    if (stringChar != 's')
                    {
                        if (stringChar == c)
                        {
                            stringChar = 's';
                            addChar = false;
                        }
                    }
                    else
                    {
                        addChar = false;
                        stringChar = c;
                    }
                }
                else if (c == ' ' && stringChar == 's')
                {
                    addChar = false;
                    ret.Add(curCommand.Trim());
                    curCommand = "";
                }
                if(addChar)
                {
                    curCommand += c;
                }
            }
            curCommand = curCommand.Trim();
            if (curCommand != "")
            {
                ret.Add(curCommand);
            }

            return ret.ToArray();
        }

        private static bool BelongsToServer(string command)
        {
            /*if(GameServer.ServerNetworkManager.NetServer.Status != MiningGameServer.lNetPeerStatus.Running)
            {
                return false;
            }*/
            return MiningGameserver.ServerConsole.Consume(command);
        }

        public static void ConsoleInput(string input2, bool silent = false)
        {
            string[] cons = splitConsoleInput_Semicolons(input2);
            foreach (string input in cons)
            {
                string[] inputs = splitConsoleInput_Arguments(input);
                inputs = removeFromList(inputs, " ");
                string command = inputs[0];
                
                if (!silent)
                    Log(">" + input);

                if(MiningGameserver.ServerConsole.IsCommand(command))
                {
                    MiningGameserver.ServerConsole.ExecuteCommand(command, inputs.Skip(1).ToArray());
                    continue;
                }

                else if (MiningGameserver.ServerConsole.IsVariable(command))
                {
                    MiningGameserver.ServerConsole.ExecuteVariable(command, inputs.Skip(1).ToArray());
                    continue;
                }

                else if (isCommand(command))
                {
                    executeCommand(command, inputs.Skip(1).ToArray<string>());
                }
                else if (isVariable(command))
                {
                    executeVariable(command, inputs.Skip(1).ToArray<string>());
                }
            }
        }
    }
}
