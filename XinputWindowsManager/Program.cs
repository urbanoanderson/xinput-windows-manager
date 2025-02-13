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

            try
            {
                ManagerConfiguration managerConfiguration = new ManagerConfiguration();
                SystemTrayApplicationContext appContext = new SystemTrayApplicationContext(managerConfiguration);
                Application.ApplicationExit += appContext.HandleExit;
                Application.Run(appContext);
            }
            catch (Exception e)
            {
                MessageBox.Show(text: $"{e.Message}", caption: "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
