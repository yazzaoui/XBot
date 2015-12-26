using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Xml.Serialization; 
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MxBots.Properties;

namespace MxBots
{
    [Serializable]
    public class HKMDB
    {
        #region variables
         [XmlIgnoreAttribute]
        private KeyEventArgs KeySauvé;
         [XmlIgnoreAttribute]
        private int id;
        [XmlIgnoreAttribute]
       // private static XmlSerializer ser = new XmlSerializer(typeof(HKMDB));
         private static XmlSerializer ser = new XmlSerializer(typeof(ArrayList), new Type[] { typeof(HKMDB) }); 
        [XmlIgnoreAttribute]
        private const string FILE = "Settings\\Hotkeys.xcfg"; 
        #endregion

        #region methodes

        public HKMDB()
        {
            
        }
        public HKMDB(KeyEventArgs K1,int id)
        {
            this.KeySauvé = K1;
            
           
            this.id = id;
        }
        public string DonnemoiladéfinitionConnard()
        {
            switch (id)
            {
                case 1:
                    return "Selecting WoW Windows";
                    
                case 2:
                    return "Fast Main Switch";

                case 3:
                    return "Attack Main's Target";

                case 4:
                    return "Follow/Stop Following Main";

                case 5:
                    return "Go To Location";

                case 6:
                    return "Enable/Disable Camera Hack";

                case 7:
                    return "Camera Zoom Out ";

                case 8:
                    return "Camera Zoom In";

                default:
                    return "No Def";
                   

            }
        }
        public static string KeyToString(KeyEventArgs e)
        {
            if (e != null)
            {
                string keytostring = e.KeyCode.ToString();
                switch (keytostring)
                {
                    case "Return":
                        keytostring = "Enter";
                        break;
                    case "D1":
                        keytostring = "1";
                        break;
                    case "D2":
                        keytostring = "2";
                        break;
                    case "D3":
                        keytostring = "3";
                        break;
                    case "D4":
                        keytostring = "4";
                        break;
                    case "D5":
                        keytostring = "5";
                        break;
                    case "D6":
                        keytostring = "6";
                        break;
                    case "D7":
                        keytostring = "7";
                        break;
                    case "D8":
                        keytostring = "8";
                        break;
                    case "D9":
                        keytostring = "9";
                        break;
                    case "D0":
                        keytostring = "0";
                        break;
                }
                if (e.Modifiers == Keys.None)
                {
                    return keytostring;
                }
                else if (!IsOnlyKeyModiFier(e))
                {
                    return e.Modifiers.ToString() + " + " + keytostring;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }

            
        }
        public string KeyToString()
        {
           return KeyToString(KeySauvé);
        }
        public static bool IsOnlyKeyModiFier(KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.Menu)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void serialize(System.Collections.ArrayList list)
        {
            using (FileStream strm = new FileStream(FILE, FileMode.Create, FileAccess.Write))
            {
                ser.Serialize(strm, list);
            } 
        }
        public static ArrayList deserialize()
        {
            try
            {
                using (FileStream strm = new FileStream(FILE, FileMode.Open, FileAccess.Read))
                {
                    ArrayList list = ser.Deserialize(strm) as ArrayList;
                    return list;
                }
            }
            catch
            {
                MessageBox.Show("Error in Hotkeys.cfg");
                Application.Exit();
                return null;
            }
        }

        public Keys MainKey()
        {
            return KeySauvé.KeyCode;
        }
        public int Modifiers()
        {
            int i = 0x00;
            if ((KeySauvé.Modifiers & Keys.Control) == Keys.Control)
            {
                i += 0x0002;
            }
            if ((KeySauvé.Modifiers & Keys.Alt) == Keys.Alt)
            {
                i += 0x0001;
            }
            if ((KeySauvé.Modifiers & Keys.Shift) == Keys.Shift)
            {
                i += 0x0004;

            }
         
            return i;
        }
        #endregion

        [XmlIgnoreAttribute]
        public KeyEventArgs Touche
        {
            get
            {
                return KeySauvé;
            }
            set
            {
                KeySauvé = value;
            }
        }
        public Keys KeyData
        {
            get
            {
                return KeySauvé.KeyData;
               

            }
            set
            {
                KeySauvé = new KeyEventArgs(value);
                
            }
        }
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        
      
    

    }
    public static class HKMLIST
    {
        private static HKMDB[] HKMLISTE;
        public const int NOMBREDEHOTKEYS = 8;
        static HKMLIST()
        {
            HKMLISTE = new HKMDB[NOMBREDEHOTKEYS]
            {
                new HKMDB(new KeyEventArgs(Keys.Control|Keys.Enter),1),
                new HKMDB(new KeyEventArgs(Keys.Control|Keys.F),2),
                new HKMDB(new KeyEventArgs(Keys.Control|Keys.R),3),
                new HKMDB(new KeyEventArgs(Keys.Control|Keys.C),4),
                new HKMDB(new KeyEventArgs(Keys.Control|Keys.G),5),
                new HKMDB(new KeyEventArgs(Keys.Control|Keys.NumPad1),6),
                new HKMDB(new KeyEventArgs(Keys.Control|Keys.NumPad2),7),
                new HKMDB(new KeyEventArgs(Keys.Control|Keys.NumPad3),8)
            };
            

        }
        public static HKMDB[] HKMLISTDEF
        {
            get
            {
                return HKMLISTE;
            }
        }
       
    }
}
