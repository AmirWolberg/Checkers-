using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{
    public static class Program
    {
        // Setting up the UserInterface
        public static UserInterface UI = new UserInterface();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Set up and Run The User interface 
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(UI);


            // Terimnates all running threads and exists program when application window is closed
            System.Environment.Exit(1);
        }
    }
}
