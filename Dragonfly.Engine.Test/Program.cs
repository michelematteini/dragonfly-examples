using System;
using System.Windows.Forms;

namespace Dragonfly.Engine.Test
{
    class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Form mainForm = new FrmTestGUI();
            if (!mainForm.IsDisposed)
            {
                Application.Run(mainForm);
            }
        }
    }
}
