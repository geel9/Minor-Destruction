using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Structs;
using MiningGame.Code.Entities;
using Microsoft.Xna.Framework;
using MiningGameServer;
using ConCommand = MiningGame.Code.Structs.ConCommand;
using Console = MiningGame.Code.Entities.Console;
using Convar = MiningGame.Code.Structs.Convar;


namespace MiningGame.Code.Managers
{
    public static class ConsoleManager
    {
        private const long FlagCheats = 1;
        private const long FlagHidden = 2;
        private const long FlagLocked = 4;

        public static List<ConCommand> Commands = new List<ConCommand>();
        public static List<Convar> Variables = new List<Convar>();

        public static string output = "";
        public static int lines = 10;
        public static int offset = 0;
        public static int logNum = 0;

        public static void AddConCommand(string name, string description, Action lambda, long flags = 0)
        {
            if (IsCommand(name)) return;
            //Using ConCommand for both argument and no argument concommands,
            //So it expects an Action<string>. So we just make a new Action that calls the passed in Action.
            //HOORAY!
            ConCommand cc = new ConCommand(l => lambda(), description, name, flags);
            Commands.Add(cc);
        }

        public static void AddConCommandArgs(string name, string description, Action<string[]> lambda, int numArguments, long flags = 0)
        {
            if (IsCommand(name)) return;

            ConCommand cc = new ConCommand(lambda, description, name, flags);
            cc.SetArgNumOne(numArguments);
            Commands.Add(cc);
        }

        public static void AddConCommandArgsMinMax(string name, string description, Action<string[]> lambda, int minArguments, int maxArguments, long flags = 0)
        {
            if (IsCommand(name)) return;

            ConCommand cc = new ConCommand(lambda, description, name, flags);
            cc.SetArgNumMinMax(minArguments, maxArguments);
            Commands.Add(cc);
        }

        public static void AddConCommandArgsMult(string name, string description, Action<string[]> lambda, int[] argumentNums, long flags = 0)
        {
            if (IsCommand(name)) return;

            ConCommand cc = new ConCommand(lambda, description, name, flags);
            cc.SetArgNumMultiple(argumentNums);
            Commands.Add(cc);
        }

        public static void AddConVar(string name, string description, string DefaultValue,
                                     Action<string> lambda = null, long flags = 0)
        {
            if (!IsVariable(name))
            {
                Convar cv = new Convar(lambda, description, DefaultValue, name, flags);
                Variables.Add(cv);
            }
        }

        public static bool IsCommand(string command)
        {
            foreach (ConCommand c in Commands)
            {
                if (command == c.Name)
                    return true;
            }
            return false;
        }

        public static bool IsVariable(string var)
        {
            foreach (Convar c in Variables)
            {
                if (var == c.Name)
                    return true;
            }
            return false;
        }

        public static void ExecuteCommand(string name, string[] arguments)
        {
            bool cheats = true;
            bool needsCheats = (GetFlags(name) & FlagCheats) > 0;
            if ((!needsCheats && !cheats) || cheats)
            {
                ConCommand command = GetCommand(name);
                if (!command.HasArgs || command.IsInArgNums(arguments.Length))
                {
                    command.Lambda(arguments);
                }
                else
                {
                    string amount = command.PossibleArgNums.Aggregate("", (current, argNum) => current + (argNum + "/"));
                    amount = amount.TrimEnd('/');
                    Log("Expected (" + amount + ") arguments.");
                }
            }
            else
            {
                Log("Cheats must be enabled for " + name);
            }
        }

        public static void ExecuteVariable(string name, string[] arguments)
        {
            bool cheats = true;
            bool needsCheats = (GetFlags(name) & FlagCheats) > 0;
            Convar c = Variables[Variables.IndexOf(GetVariable(name))];
            if (arguments.Length > 0)
            {
                if ((!needsCheats && !cheats) || cheats)
                {
                    SetVariableValue(name, ListToString(arguments, " "));
                    if (c.Lambda != null)
                        c.Lambda(arguments.Aggregate((current, argument) => current + argument));
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
            return GetVariable(name).Value;
        }

        public static bool GetVariableBool(string name)
        {
            return GetVariable(name).Value.Replace(" ", "") == "1" ? true : false;
        }

        public static float GetVariableFloat(string name)
        {
            return (float)Convert.ToDouble(GetVariable(name).Value);
        }

        public static double GetVariableDouble(string name)
        {
            return Convert.ToDouble(GetVariable(name).Value);
        }

        public static int GetVariableInt(string name)
        {
            return Convert.ToInt32(GetVariable(name).Value);
        }

        public static string GetFlagString(string name)
        {
            long flags = GetFlags(name);
            string ret = "";

            ret += (flags & FlagCheats) > 0 ? "FLAG_CHEATS " : "";
            ret += (flags & FlagHidden) > 0 ? "FLAG_HIDDEN " : "";
            ret += (flags & FlagLocked) > 0 ? "FLAG_LOCKED " : "";

            return ret;
        }

        public static long GetFlags(string name)
        {
            if (IsVariable(name))
            {
                return GetVariable(name).Flags;
            }
            else if (IsCommand(name))
            {
                return GetCommand(name).Flags;
            }
            else
                return 0;
        }

        public static void SetVariableValue(string name, string value)
        {
            Convar c = GetVariable(name);
            Convar c2 = c;
            c2.Value = value;
            Variables.Add(c2);
            Variables.Remove(c);
        }

        public static ConCommand GetCommand(string name)
        {
            foreach (ConCommand c in Commands)
            {
                if (c.Name == name)
                {
                    return c;
                }
            }
            return new ConCommand(l => { }, "", "");
        }

        public static Convar GetVariable(string name)
        {
            try
            {
                foreach (Convar c in Variables)
                {
                    if (c.Name == name)
                    {
                        return c;
                    }
                }
            }
            catch (Exception)
            {
                return new Convar(l => { }, "", "", "");
            }
            return new Convar(l => { }, "", "", "");
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

        public static string ListToString(IEnumerable<string> list, string delimiter, int limit = 1000,
                                          string breakString = "...")
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

                if (ServerConsole.IsCommand(command))
                {
                    MiningGameServer.ServerConsole.ExecuteCommand(command, inputs.Skip(1).ToArray());
                    continue;
                }

                if (ServerConsole.IsVariable(command))
                {
                    MiningGameServer.ServerConsole.ExecuteVariable(command, inputs.Skip(1).ToArray());
                    continue;
                }
                if (IsCommand(command))
                {
                    ExecuteCommand(command, inputs.Skip(1).ToArray());
                }
                else if (IsVariable(command))
                {
                    ExecuteVariable(command, inputs.Skip(1).ToArray());
                }
            }
        }

        public static void InitConsole()
        {
            /*This piece of code gets all the classes that have a 
              ConsoleInit() method and executes it. Essentially,
              this allows you to define console variables/commands in the classes
              that they relate to, instead of all in a big chunk somewhere.
              Before, they were all define in a big method in Main.cs. Ew. */
            Assembly a = Assembly.GetExecutingAssembly();
            foreach (Type t in a.GetTypes())
            {
                MethodInfo info = t.GetMethod("ConsoleInit");
                //t implements IConsoleExtender
                if (info != null)
                    info.Invoke(t, new object[0]);
            }
        }

        public static void ConsoleInit()
        {
            AddConCommandArgs("exec", "Execute a file in the config folder", ls =>
            {
                string execContents =
                    FileReaderManager.ReadFileContents(DirectoryManager.CONFIG + ls[0] + (!ls[0].Contains(".cfg") ? ".cfg" : ""));
                if (execContents != "")
                {
                    foreach (string command in execContents.Replace("\r", "").Split('\n'))
                    {
                        ConsoleInput(command, true);
                    }
                }
                else
                {
                    Log(
                        "Could not find file/file was empty: " +
                        ls[0]);
                }
            }, 1);

            AddConCommandArgsMinMax("bind", "Bind a key", ls =>
                                                    {

                                                        if (ls.Length == 1)
                                                        {
                                                            string key = ls[0];
                                                            foreach (PlayerBind bind in PlayerBindManager.PlayerBinds)
                                                            {
                                                                if (bind.buttonBoundName.Replace("^", "") ==
                                                                    key.Replace("^", ""))
                                                                    Log(key + " is bound to: \"" +
                                                                        bind.consoleCommandOnPressed);
                                                            }
                                                        }
                                                        else if (ls.Length == 2)
                                                        {
                                                            string key = ls[0];
                                                            string command = ls[1];
                                                            PlayerBindManager.BindButton(key, command);
                                                        }
                                                        else
                                                        {
                                                            Log("Usage: bind [key] ([command])");
                                                            return;
                                                        }
                                                    }, 1, 2);

            AddConCommand("list", "List console commands and variables", () =>
            {
                Log(
                    "Name                Description                Flags");
                Log(
                    "----------------------------------------------------");
                foreach (ConCommand c in Commands)
                {
                    string r = Align(c.Name,
                                    c.Description, 20);
                    string flags = GetFlagString(c.Name);
                    string r2 = Align(r, flags, 20);
                    if ((c.Flags & FlagHidden) == 0)
                        Log(r2);
                }
                foreach (Convar c in Variables)
                {
                    string r = Align(c.Name,
                                    c.Description, 20);
                    string flags = GetFlagString(c.Name);
                    string r2 = Align(r, flags, 20);
                    if ((c.Flags & FlagHidden) == 0)
                        Log(r2);
                }
            });

            AddConCommandArgs("help", "RECURSION", ls =>
            {
                string name = ls[0];
                string description = "";
                bool isVar = IsVariable(name);
                long flags = GetFlags(name);
                bool cheats = (flags & FlagCheats) > 0;
                bool hidden = (flags & FlagHidden) > 0;
                bool locked = (flags & FlagLocked) > 0;
                if (isVar)
                {
                    description = GetVariable(name).Description;
                }
                else if (IsCommand(name))
                {
                    description = GetCommand(name).Description;
                }
                Log(name + ": " + description +
                    ((isVar) ? "(value: " + GetVariableValue(name) + ")" : "") +
                    " cheats: " + cheats + " hidden: " + hidden + " locked: " +
                    locked);
            }, 1);
        }
    }
}
