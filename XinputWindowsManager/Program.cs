using System;
using System.Windows.Forms;

namespace XinputWindowsManager
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SystemTrayApplicationContext appContext = new SystemTrayApplicationContext();
            Application.ApplicationExit += appContext.HandleExit;
            Application.Run(appContext);
        }
    }
}
