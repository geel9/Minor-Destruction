using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiningGameserver
{
    public static class ServerConsole
    {
        const long FLAG_CHEATS = 1;
        const long FLAG_HIDDEN = 2;
        const long FLAG_LOCKED = 4;

        public static Action<string> OnConsoleOutput = s =>
                                                           {
                                                               Console.WriteLine(s);
                                                           };

        public static List<ConCommand> commands = new List<ConCommand>();
        public static List<Convar> variables = new List<Convar>();

        public static void AddConCommand(string name, string description, Action<string[]> lambda, long flags = 0)
        {
            if (!IsCommand(name))
            {
                ConCommand cc = new ConCommand(lambda, description, name, flags);
                commands.Add(cc);
            }
        }

        public static void AddConVar(string name, string description, string DefaultValue, Action<string[]> lambda = null, long flags = 0)
        {
            if (!IsVariable(name))
            {
                Convar cv = new Convar(lambda, description, DefaultValue, name, flags);
                variables.Add(cv);
            }
        }

        public static bool IsCommand(string command)
        {
            foreach (ConCommand c in commands)
            {
                if (command == c.name)
                    return true;
            }
            return false;
        }

        public static bool IsVariable(string var)
        {
            foreach (Convar c in variables)
            {
                if (var == c.name)
                    return true;
            }
            return false;
        }

        public static bool Consume(string input)
        {
            return IsCommand(input) || IsVariable(input);
        }

        public static void ExecuteCommand(string name, string[] arguments)
        {
            bool cheats = GetVariableBool("sv_cheats");
            bool needsCheats = (GetFlags(name) & FLAG_CHEATS) > 0;
            if ((!needsCheats && !cheats) || cheats)
            {
                GetCommand(name).lambda(arguments);
            }
            else
            {
                Log("Cheats must be enabled for " + name);
            }
        }

        public static void ExecuteVariable(string name, string[] arguments)
        {
            bool cheats = GetVariableBool("sv_cheats");
            bool needsCheats = (GetFlags(name) & FLAG_CHEATS) > 0;
            Convar c = variables[variables.IndexOf(GetVariable(name))];
            if (arguments.Length > 0)
            {
                if ((!needsCheats && !cheats) || cheats)
                {
                    SetVariableValue(name, ListToString(arguments, " "));
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

        public static string GetVariableValue(string name)
        {
            return GetVariable(name).value;
        }

        public static bool GetVariableBool(string name)
        {
            return GetVariable(name).value.Replace(" ", "") == "1" ? true : false;
        }

        public static float GetVariableFloat(string name)
        {
            return (float)Convert.ToDouble(GetVariable(name).value);
        }

        public static double GetVariableDouble(string name)
        {
            return Convert.ToDouble(GetVariable(name).value);
        }

        public static int GetVariableInt(string name)
        {
            return Convert.ToInt32(GetVariable(name).value);
        }

        public static string GetFlagString(string name)
        {
            long flags = GetFlags(name);
            string ret = "";

            ret += (flags & FLAG_CHEATS) > 0 ? "FLAG_CHEATS " : "";
            ret += (flags & FLAG_HIDDEN) > 0 ? "FLAG_HIDDEN " : "";
            ret += (flags & FLAG_LOCKED) > 0 ? "FLAG_LOCKED " : "";

            return ret;
        }

        public static long GetFlags(string name)
        {
            if (IsVariable(name))
            {
                return GetVariable(name).flags;
            }
            else if (IsCommand(name))
            {
                return GetCommand(name).flags;
            }
            else
                return 0;
        }

        public static void SetVariableValue(string name, string value)
        {
            Convar c = GetVariable(name);
            Convar c2 = c;
            c2.value = value;
            variables.Add(c2);
            variables.Remove(c);
        }

        public static ConCommand GetCommand(string name)
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

        public static Convar GetVariable(string name)
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

        public static string[] RemoveFromList(string[] list, string remove)
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
            OnConsoleOutput(log);
        }


        public static string[] SplitConsoleInputSemicolons(string input)
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

        public static string[] SplitConsoleInputArguments(string input)
        {
            List<string> ret = new List<string>();
            string curCommand = "";
            char stringChar = 's';

            foreach (char c in input)
            {
                bool addChar = true;
                if (c == '"')
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
                if (addChar)
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

        public static void ConsoleInput(string input2, bool silent = false)
        {
            string[] cons = SplitConsoleInputSemicolons(input2);
            foreach (string input in cons)
            {
                string[] inputs = SplitConsoleInputArguments(input);
                inputs = RemoveFromList(inputs, " ");
                string command = inputs[0];

                if (!silent)
                    Log(">" + input);
                if (IsCommand(command))
                {
                    ExecuteCommand(command, inputs.Skip(1).ToArray<string>());
                }
                else if (IsVariable(command))
                {
                    ExecuteVariable(command, inputs.Skip(1).ToArray<string>());
                }
            }
        }
    }

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
