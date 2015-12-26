using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;

namespace MxBots
{
    static class Program
    {
       
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
            if (SomeSettings.working == true)
            {
               Application.Run(new Form2());
            }
        }
        public static void restart()
        {
            Application.Restart();
        }
      
    }
}
