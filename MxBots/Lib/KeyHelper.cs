using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Magic;
using System.Threading;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace GLib
{
    public class KeyHelper
    {
        //KeyName, ShiftState, BarState, Char
        private  IDictionary<string, Keys> KeysList ;
        private  string bar ;
        
        private BlackMagic Memory;
        
        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        private static extern bool _PostMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData,
          int dwExtraInfo);

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        public KeyHelper(BlackMagic Mem)
        {
            this.Memory = Mem;
            KeysList = new Dictionary<string, Keys>();
            bar = "Bar100";
            string executableName = Application.ExecutablePath;
            FileInfo executableFileInfo = new FileInfo(executableName);
            string  executableDirectoryName = executableFileInfo.DirectoryName;
            loadKeys(executableDirectoryName + "\\Settings\\Keys.xml");
        }

        public void click(uint x,uint y)
        {
            mouse_event((uint)MouseEventFlags.LEFTDOWN, x, y, 0, 0);
            mouse_event((uint)MouseEventFlags.LEFTUP, x, y, 0, 0);
        }
        public void loadKeys(string fileToLoad)
        {
            
            XmlDocument doc = new XmlDocument();
            string path = fileToLoad;
            doc.Load(path);
            
            //Load factions
            XmlNodeList Faction = doc.GetElementsByTagName("Key");

            foreach (XmlNode faction in Faction)
            {
                XmlAttributeCollection at = faction.Attributes;
                KeysList.Add(at[0].Value, new Keys(at[0].Value, at[1].Value, at[2].Value, at[3].Value,Memory));
            }
            
        }
        /// <summary>
        /// CastSpell
        /// </summary>
        /// <param name="name">
        /// Key name to send
        /// </param>
        public void CastSpell(string name)
        {
            if(KeysList.ContainsKey(name))
            {
                Keys key = KeysList[name];
                if (key.getbar != bar && key.getbar != "Indifferent")
                {
                    key.changeBar();
                    bar = key.getbar;
                    Thread.Sleep(90);
                }
                key.SendKey();
            }
        }

        /// <summary>
        /// Press and hold a key
        /// </summary>
        /// <param name="name">
        /// Key name to press and hold
        /// </param>
        public  void PressKey(string name)
        {
            if (KeysList.ContainsKey(name))
            {
                Keys key = KeysList[name];
                if (key.getbar != bar && key.getbar != "Indifferent")
                {
                    key.changeBar();
                    bar = key.getbar;
                    Thread.Sleep(90);
                }
                key.PressKey();
            }
        }

        /// <summary>
        /// Sendkey
        /// </summary>
        /// <param name="name">
        /// Key name to send
        /// </param>
        public void SendKey(string name)
        {
            CastSpell(name);
        }

        /// <summary>
        /// Release a held key. 
        /// </summary>
        /// <param name="name">
        /// Key name to release
        /// </param>
        public void ReleaseKey(string name)
        {
            if (KeysList.ContainsKey(name))
            {
                Keys key = KeysList[name];
                if (key.getbar != bar && key.getbar != "Indifferent")
                {
                    key.changeBar();
                    bar = key.getbar;
                    Thread.Sleep(90);
                }
                key.ReleaseKey();
            }
        }
    }
}