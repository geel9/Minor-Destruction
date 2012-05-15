using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using YogUILibrary.Managers;

namespace MiningGame.Code.Managers
{
    public class PlayerBindManager
    {
        public static List<PlayerBind> PlayerBinds = new List<PlayerBind>();
        public static List<string> buttonsBeingPressed = new List<string>();

        public static void InitBinds()
        {
            #region boundPress
            InputManager.BindKey(() => { BindPressed("a"); }, Keys.A, true);
            InputManager.BindKey(() => { BindPressed("b"); }, Keys.B, true);
            InputManager.BindKey(() => { BindPressed("c"); }, Keys.C, true);
            InputManager.BindKey(() => { BindPressed("d"); }, Keys.D, true);
            InputManager.BindKey(() => { BindPressed("e"); }, Keys.E, true);
            InputManager.BindKey(() => { BindPressed("f"); }, Keys.F, true);
            InputManager.BindKey(() => { BindPressed("g"); }, Keys.G, true);
            InputManager.BindKey(() => { BindPressed("h"); }, Keys.H, true);
            InputManager.BindKey(() => { BindPressed("i"); }, Keys.I, true);
            InputManager.BindKey(() => { BindPressed("j"); }, Keys.J, true);
            InputManager.BindKey(() => { BindPressed("k"); }, Keys.K, true);
            InputManager.BindKey(() => { BindPressed("l"); }, Keys.L, true);
            InputManager.BindKey(() => { BindPressed("m"); }, Keys.M, true);
            InputManager.BindKey(() => { BindPressed("n"); }, Keys.N, true);
            InputManager.BindKey(() => { BindPressed("o"); }, Keys.O, true);
            InputManager.BindKey(() => { BindPressed("p"); }, Keys.P, true);
            InputManager.BindKey(() => { BindPressed("q"); }, Keys.Q, true);
            InputManager.BindKey(() => { BindPressed("r"); }, Keys.R, true);
            InputManager.BindKey(() => { BindPressed("s"); }, Keys.S, true);
            InputManager.BindKey(() => { BindPressed("t"); }, Keys.T, true);
            InputManager.BindKey(() => { BindPressed("u"); }, Keys.U, true);
            InputManager.BindKey(() => { BindPressed("v"); }, Keys.V, true);
            InputManager.BindKey(() => { BindPressed("w"); }, Keys.W, true);
            InputManager.BindKey(() => { BindPressed("x"); }, Keys.X, true);
            InputManager.BindKey(() => { BindPressed("y"); }, Keys.Y, true);
            InputManager.BindKey(() => { BindPressed("z"); }, Keys.Z, true);
            InputManager.BindKey(() => { BindPressed("`"); }, Keys.OemTilde, true);
            InputManager.BindKey(() => { BindPressed("0"); }, Keys.D0, true);
            InputManager.BindKey(() => { BindPressed("1"); }, Keys.D1, true);
            InputManager.BindKey(() => { BindPressed("2"); }, Keys.D2, true);
            InputManager.BindKey(() => { BindPressed("3"); }, Keys.D3, true);
            InputManager.BindKey(() => { BindPressed("4"); }, Keys.D4, true);
            InputManager.BindKey(() => { BindPressed("5"); }, Keys.D5, true);
            InputManager.BindKey(() => { BindPressed("6"); }, Keys.D6, true);
            InputManager.BindKey(() => { BindPressed("7"); }, Keys.D7, true);
            InputManager.BindKey(() => { BindPressed("8"); }, Keys.D8, true);
            InputManager.BindKey(() => { BindPressed("9"); }, Keys.D9, true);
            InputManager.BindKey(() => { BindPressed("num0"); }, Keys.NumPad0, true);
            InputManager.BindKey(() => { BindPressed("num1"); }, Keys.NumPad1, true);
            InputManager.BindKey(() => { BindPressed("num2"); }, Keys.NumPad2, true);
            InputManager.BindKey(() => { BindPressed("num3"); }, Keys.NumPad3, true);
            InputManager.BindKey(() => { BindPressed("num4"); }, Keys.NumPad4, true);
            InputManager.BindKey(() => { BindPressed("num5"); }, Keys.NumPad5, true);
            InputManager.BindKey(() => { BindPressed("num6"); }, Keys.NumPad6, true);
            InputManager.BindKey(() => { BindPressed("num7"); }, Keys.NumPad7, true);
            InputManager.BindKey(() => { BindPressed("num8"); }, Keys.NumPad8, true);
            InputManager.BindKey(() => { BindPressed("num9"); }, Keys.NumPad9, true);
            InputManager.BindKey(() => { BindPressed("f1"); }, Keys.F1, true);
            InputManager.BindKey(() => { BindPressed("f2"); }, Keys.F2, true);
            InputManager.BindKey(() => { BindPressed("f3"); }, Keys.F3, true);
            InputManager.BindKey(() => { BindPressed("f4"); }, Keys.F4, true);
            InputManager.BindKey(() => { BindPressed("f5"); }, Keys.F5, true);
            InputManager.BindKey(() => { BindPressed("f6"); }, Keys.F6, true);
            InputManager.BindKey(() => { BindPressed("f7"); }, Keys.F7, true);
            InputManager.BindKey(() => { BindPressed("f8"); }, Keys.F8, true);
            InputManager.BindKey(() => { BindPressed("f9"); }, Keys.F9, true);
            InputManager.BindKey(() => { BindPressed("f10"); }, Keys.F10, true);
            InputManager.BindKey(() => { BindPressed("f11"); }, Keys.F11, true);
            InputManager.BindKey(() => { BindPressed("f12"); }, Keys.F12, true);
            InputManager.BindKey(() => { BindPressed("space"); }, Keys.Space, true);
            InputManager.BindKey(() => { BindPressed("."); }, Keys.OemPeriod, true);
            InputManager.BindKey(() => { BindPressed(","); }, Keys.OemComma, true);
            InputManager.BindKey(() => { BindPressed("/"); }, Keys.OemQuestion, true);
            InputManager.BindKey(() => { BindPressed("-"); }, Keys.OemMinus, true);
            InputManager.BindKey(() => { BindPressed("="); }, Keys.OemPlus, true);
            #endregion
      
            #region boundRelease
            InputManager.BindKey(() => { BindPressed("a", false); }, Keys.A, false, false);
            InputManager.BindKey(() => { BindPressed("b", false); }, Keys.B, false, false);
            InputManager.BindKey(() => { BindPressed("c", false); }, Keys.C, false, false);
            InputManager.BindKey(() => { BindPressed("d", false); }, Keys.D, false, false);
            InputManager.BindKey(() => { BindPressed("e", false); }, Keys.E, false, false);
            InputManager.BindKey(() => { BindPressed("f", false); }, Keys.F, false, false);
            InputManager.BindKey(() => { BindPressed("g", false); }, Keys.G, false, false);
            InputManager.BindKey(() => { BindPressed("h", false); }, Keys.H, false, false);
            InputManager.BindKey(() => { BindPressed("i", false); }, Keys.I, false, false);
            InputManager.BindKey(() => { BindPressed("j", false); }, Keys.J, false, false);
            InputManager.BindKey(() => { BindPressed("k", false); }, Keys.K, false, false);
            InputManager.BindKey(() => { BindPressed("l", false); }, Keys.L, false, false);
            InputManager.BindKey(() => { BindPressed("m", false); }, Keys.M, false, false);
            InputManager.BindKey(() => { BindPressed("n", false); }, Keys.N, false, false);
            InputManager.BindKey(() => { BindPressed("o", false); }, Keys.O, false, false);
            InputManager.BindKey(() => { BindPressed("p", false); }, Keys.P, false, false);
            InputManager.BindKey(() => { BindPressed("q", false); }, Keys.Q, false, false);
            InputManager.BindKey(() => { BindPressed("r", false); }, Keys.R, false, false);
            InputManager.BindKey(() => { BindPressed("s", false); }, Keys.S, false, false);
            InputManager.BindKey(() => { BindPressed("t", false); }, Keys.T, false, false);
            InputManager.BindKey(() => { BindPressed("u", false); }, Keys.U, false, false);
            InputManager.BindKey(() => { BindPressed("v", false); }, Keys.V, false, false);
            InputManager.BindKey(() => { BindPressed("w", false); }, Keys.W, false, false);
            InputManager.BindKey(() => { BindPressed("x", false); }, Keys.X, false, false);
            InputManager.BindKey(() => { BindPressed("y", false); }, Keys.Y, false, false);
            InputManager.BindKey(() => { BindPressed("z", false); }, Keys.Z, false, false);
            InputManager.BindKey(() => { BindPressed("`", false); }, Keys.OemTilde, false, false);
            InputManager.BindKey(() => { BindPressed("0", false); }, Keys.D0, false, false);
            InputManager.BindKey(() => { BindPressed("1", false); }, Keys.D1, false, false);
            InputManager.BindKey(() => { BindPressed("2", false); }, Keys.D2, false, false);
            InputManager.BindKey(() => { BindPressed("3", false); }, Keys.D3, false, false);
            InputManager.BindKey(() => { BindPressed("4", false); }, Keys.D4, false, false);
            InputManager.BindKey(() => { BindPressed("5", false); }, Keys.D5, false, false);
            InputManager.BindKey(() => { BindPressed("6", false); }, Keys.D6, false, false);
            InputManager.BindKey(() => { BindPressed("7", false); }, Keys.D7, false, false);
            InputManager.BindKey(() => { BindPressed("8", false); }, Keys.D8, false, false);
            InputManager.BindKey(() => { BindPressed("9", false); }, Keys.D9, false, false);
            InputManager.BindKey(() => { BindPressed("num0", false); }, Keys.NumPad0, false, false);
            InputManager.BindKey(() => { BindPressed("num1", false); }, Keys.NumPad1, false, false);
            InputManager.BindKey(() => { BindPressed("num2", false); }, Keys.NumPad2, false, false);
            InputManager.BindKey(() => { BindPressed("num3", false); }, Keys.NumPad3, false, false);
            InputManager.BindKey(() => { BindPressed("num4", false); }, Keys.NumPad4, false, false);
            InputManager.BindKey(() => { BindPressed("num5", false); }, Keys.NumPad5, false, false);
            InputManager.BindKey(() => { BindPressed("num6", false); }, Keys.NumPad6, false, false);
            InputManager.BindKey(() => { BindPressed("num7", false); }, Keys.NumPad7, false, false);
            InputManager.BindKey(() => { BindPressed("num8", false); }, Keys.NumPad8, false, false);
            InputManager.BindKey(() => { BindPressed("num9", false); }, Keys.NumPad9, false, false);
            InputManager.BindKey(() => { BindPressed("f1", false); }, Keys.F1, false, false);
            InputManager.BindKey(() => { BindPressed("f2", false); }, Keys.F2, false, false);
            InputManager.BindKey(() => { BindPressed("f3", false); }, Keys.F3, false, false);
            InputManager.BindKey(() => { BindPressed("f4", false); }, Keys.F4, false, false);
            InputManager.BindKey(() => { BindPressed("f5", false); }, Keys.F5, false, false);
            InputManager.BindKey(() => { BindPressed("f6", false); }, Keys.F6, false, false);
            InputManager.BindKey(() => { BindPressed("f7", false); }, Keys.F7, false, false);
            InputManager.BindKey(() => { BindPressed("f8", false); }, Keys.F8, false, false);
            InputManager.BindKey(() => { BindPressed("f9", false); }, Keys.F9, false, false);
            InputManager.BindKey(() => { BindPressed("f10", false); }, Keys.F10, false, false);
            InputManager.BindKey(() => { BindPressed("f11", false); }, Keys.F11, false, false);
            InputManager.BindKey(() => { BindPressed("f12", false); }, Keys.F12, false, false);
            InputManager.BindKey(() => { BindPressed("space", false); }, Keys.Space, false, false);
            InputManager.BindKey(() => { BindPressed(".", false); }, Keys.OemPeriod, false, false);
            InputManager.BindKey(() => { BindPressed(",", false); }, Keys.OemComma, false, false);
            InputManager.BindKey(() => { BindPressed("/", false); }, Keys.OemQuestion, false, false);
            InputManager.BindKey(() => { BindPressed("-", false); }, Keys.OemMinus, false, false);
            InputManager.BindKey(() => { BindPressed("=", false); }, Keys.OemPlus, false, false);
            #endregion
        }

        public static void BindPressed(string button, bool pressing = true)
        {
            
            bool contains = buttonsBeingPressed.Contains(button);
            if (pressing)
            {
                if (!contains) buttonsBeingPressed.Add(button);
                PlayerBind pb = PlayerBinds.Where(x => x.buttonBoundName == button).FirstOrDefault();
                if (pb.buttonBoundName != null)
                {
                    bool doCommand = (!contains || pb.constant);
                    if (doCommand) ConsoleManager.ConsoleInput(pb.consoleCommandOnPressed, true);
                }
            }
            else
            {
                buttonsBeingPressed.Remove(button);
            }
        }

        public static void BindButton(string button, string action)
        {
            button = button.ToLower();
            bool contains = false;
            PlayerBind p = new PlayerBind("derp", "derp", true);
            int index = 0;
            foreach (PlayerBind pb in PlayerBinds) { if (pb.buttonBoundName.Replace("^", "") == button) { p = pb; contains = true; index = PlayerBinds.IndexOf(pb); } }

            if (contains)
            {
                p.consoleCommandOnPressed = action;
                p.constant = (button[0] == '^');
                PlayerBinds[index] = p;
                ConsoleManager.Log("Changing index " + index);
            }
            else
            {
                PlayerBinds.Add(new PlayerBind(button, action, (button[0] == '^')));
                ConsoleManager.Log("Binding key " + button);
            }
        }
    }


    public struct PlayerBind
    {
        public string buttonBoundName;
        public string consoleCommandOnPressed;
        public bool constant;

        public PlayerBind(string buttonBoundName, string consoleCommandOnPressed, bool constant)
        {
            this.buttonBoundName = buttonBoundName;
            this.consoleCommandOnPressed = consoleCommandOnPressed;
            this.constant = constant;
        }
    }
}
