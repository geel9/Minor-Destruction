using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace MiningGame.Code.Managers
{
    public static class FormManager
    {

        public static void ShowMessageBox(string title, string message)
        {
            MessageBox.Show(message, title);
        }

        public static bool ShowQuestionBox(string title, string message)
        {
            DialogResult dr = MessageBox.Show(message, title, MessageBoxButtons.YesNo);
            return dr == DialogResult.Yes;
        }

        public static string ShowFileSaveDialog(string defName = "", string defExt = "")
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.DefaultExt = defExt;
            sf.FileName = defName;
            sf.ShowDialog();
            return sf.FileName;
        }
        public static string ShowFileOpenDialog(bool safe = false)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.ShowDialog();
            if (File.Exists(of.FileName))
            {
                return safe ? of.SafeFileName : of.FileName;

            }
            else
            {
                return "ERROR";
            }
        }

    }
}
