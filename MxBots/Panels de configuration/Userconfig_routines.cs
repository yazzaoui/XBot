using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LuaInterface;
using System.IO;

namespace MxBots
{
    public partial class Userconfig_routines : UserControl
    {
        public Classes Classe;
        private string[] Routines;
        string executableName;
        FileInfo executableFileInfo;
        string executableDirectoryName;

        public Userconfig_routines()
        {
            InitializeComponent();
            executableName = Application.ExecutablePath;
            executableFileInfo = new FileInfo(executableName);
            executableDirectoryName = executableFileInfo.DirectoryName;
            Routines = new string[(int)Classes.None];

            for (int i = 0; i < 11; i++)
            {
                string dir = executableDirectoryName + "\\Routines\\" + Bot.ClassToString((Classes)i) + ".xbot";
                Classes CurClass = (Classes)i;
                if (i != (int)Classes.None)
                {

                    if (File.Exists(dir))
                    {
                        StreamReader sr = new StreamReader(dir);

                        Routines[i] = sr.ReadToEnd();
                        sr.Close();
                    }
                    else
                    {

                        StreamWriter sw = File.CreateText(dir);
                        sw.WriteLine("TEST BY LVMEW.");
                        sw.WriteLine(" CAY POUR >> >> >> >> ");
                        for (int j = 0; j < 15; j++)
                        {
                            sw.WriteLine(Bot.ClassToString(CurClass));
                        }
                        sw.Close();

                    }

                }
            }
            Classe = Classes.None;
        }
        public void SwicthClass(string s)
        {
            label1.Text = s + " Routine Customization";
            SaveCurRoutine();
            //label1.Location = new Point(this.Width / 2, label1.Location.Y);
            Classe = Bot.StringToClass(s);
            richTextBox1.Text = Routines[(int)Classe];
        }
        public void SaveRoutines()
        {
            SaveCurRoutine();
            for (int i = 0; i <= (int)Classes.None; i++)
            {
                string dir = executableDirectoryName + "\\Routines\\" + Bot.ClassToString((Classes)i) + ".xbot";
                Classes CurClass = (Classes)i;
                if (i != (int)Classes.None)
                {



                        StreamWriter sw = new StreamWriter(dir);
                        sw.Write(Routines[i]);

                        sw.Close();
                        
       

                }
            }
        }
        private void SaveCurRoutine()
        {
            if (Classe != Classes.None)
            {
                Routines[(int)Classe] = this.richTextBox1.Text;
            }
        }
        private void Userconfig_routines_Load(object sender, EventArgs e)
        {
           
        }
    }
}
