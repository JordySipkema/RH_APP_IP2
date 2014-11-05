using System;
using System.Windows.Forms;

namespace RH_APP
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new GUI.chooseYourBikeUI());
            Application.Run(new GUI.LoginScreen());
            

            
        }
    }


}
