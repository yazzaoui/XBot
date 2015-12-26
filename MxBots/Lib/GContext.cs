using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace GLib
{
   public class GContext
    {

        #region Constructor
      //  GMain form;

        //public GContext(GMain form)
        //{
        //    this.form = form;
        //}
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Other functions

        public int getPullDistance
        {
            get
            {
                return 30;
                //return form.getPullDistance;
            }
        }

        public bool getavoidAdds
        {
            get
            {
                return true;
                //return form.getavoidAdds;
            }
        }

        public double getRestHealth
        {
            get
            {
                return 20;
                //return form.getRestHealth;
            }
        }

        public int getAvoidAddDis
        {
            get
            {
                return 2;
                //return form.getAvoidAddDis;
            }
        }

        public double getRestMana
        {
            get
            {
                return 1.5;
                //return form.getRestMana;
            }
        }

        public IntPtr getWindowsHandle
        {
            get
            {
                return new IntPtr(30);
               // return form.getMemory.WindowHandle;
            }
        }

        #region Mouse Types
        /*
                Mouse Types:

        Decimal value = description
        1 = normal

        3 = banker/auctioneer in range (money bag)
        4 = attack in range
        5 = use (fishing bobber) in range
        6 = talk (cartoon chat bubble)

        8 = money bags in range (merchant)

        10 = trainer book in range
        11 = mining in range
        12 = skin in range
        13 = herbalism in range
        15 = mail in range
        16 = loot in range
        18 = repair in range

        23 = quest giver ! in range
        24 = repeatable quest giver in range (blue ?)

        28 = banker/auctioneer/guild bank out of range (money bags)
        29 = attack out of range
        30 = use - out of range
        31 = talk (cartoon chat bubble) - out of range

        33 = money bags out of range (merchant)

        35 = trainer book out of range
        36 = mining out of range
        37 = skin out of range
        38 = herbalism out of range
        40 = mail out of range
        41 = loot out of range
        43 = repair out of range

        48 = Quest giver ! out of range
        49 = repeatable quest giver out of range (blue ?)

        51 = ask for directions out of range AND in range (scroll over guards)
        */
        #endregion

        public int getType
        {
            get
            {
                return 3;
                //return form.getType;
            }
        }

        public bool shouldLoot
        {
            get
            {
                return true;
                //return form.getshouldLoot;
            }
        }

        public bool getHealbot
        {
            get
            {
                return false;
               // return form.getHealbot;
            }
        }

        public bool getPvP
        {
            get
            {
                return true;
                //return form.getPvP;
            }
        }

        public string getPlayerClass
        {
            get
            {
                return "lol";
                //return form.getPlayerClass;
            }
        }
        public String getProfileToLoad
        {
            get
            {
                return "non";
               // return form.getProfileToLoad;
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Farver

        public System.Drawing.Color gethojreFarve1
        {
            get
            {
                return System.Drawing.Color.Black;
               // return Position.hojreFarve1((int)form.getMemory.WindowHandle);
            }
        }

        public System.Drawing.Color gethojreFarve2
        {
            get
            {
                return System.Drawing.Color.Black;
                //return Position.hojreFarve2((int)form.getMemory.WindowHandle);
            }
        }

        public System.Drawing.Color gethojreFarve3
        {
            get
            {
                return System.Drawing.Color.Black;
                //return Position.hojreFarve3((int)form.getMemory.WindowHandle);
            }
        }

        public System.Drawing.Color gethojreFarve4
        {
            get
            {
                return System.Drawing.Color.Black;
                //return Position.hojreFarve4((int)form.getMemory.WindowHandle);
            }
        }

        public System.Drawing.Color gethojreFarve5
        {
            get
            {
                return System.Drawing.Color.Black;
                //return Position.hojreFarve5((int)form.getMemory.WindowHandle);
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Log
        public void log(string text)
        {
            //form.log(text);
        }

        public void logSystem(string text)
        {
            //form.logSystem(text);
        }
        #endregion

        //----------------------------------------------------------------------------------------------------

        //#region CastSpell, Sendkey, PressKey, ReleaseKey
        ///// <summary>
        ///// CastSpell
        ///// </summary>
        ///// <param name="name">
        ///// Key name to cast
        ///// </param>
        //public void CastSpell(string name)
        //{
        //    KeyHelper.CastSpell(name);
        //}

        ///// <summary>
        ///// Sendkey
        ///// </summary>
        ///// <param name="name">
        ///// Key name to send
        ///// </param>
        //public void SendKey(string name)
        //{
        //    KeyHelper.CastSpell(name);
        //}


        ///// <summary>
        ///// Press and hold a key
        ///// </summary>
        ///// <param name="name">
        ///// Key name to press and hold
        ///// </param>
        //public void PressKey(string name)
        //{
        //    KeyHelper.PressKey(name);
        //}

        ///// <summary>
        ///// Release a held key. 
        ///// </summary>
        ///// <param name="name">
        ///// Key name to release
        ///// </param>
        //public void ReleaseKey(string name)
        //{
        //    KeyHelper.ReleaseKey(name);
        //}
        //#endregion

        //----------------------------------------------------------------------------------------------------

        #region Config
        public double CleanInteger(int Index)
        {
            if (Index < 0)
            {
                Index = 0;
            }
            if (Index > 100)
            {
                Index = 100;
            }
            return (double)Index / 100;
        }

        public int GetConfigInt(string name, string className)
        {
            try
            {
                return Convert.ToInt32(GetConfigString(name, className));
            }
            catch { };
            return 0;
        }

        public bool GetConfigBool(string name, string className)
        {
            try
            {
                return Convert.ToBoolean(GetConfigString(name, className));
            }
            catch { };
            return false;
        }

        public bool doesExsist(string name, string className)
        {
            string executableName = Application.ExecutablePath;
            FileInfo executableFileInfo = new FileInfo(executableName);
            string executableDirectoryName = executableFileInfo.DirectoryName;

            doc = new XmlDocument();

            try
            {
                doc.Load(executableDirectoryName + "\\Class.config.xml");
            }
            catch { };

            XmlNodeList xmlnode = doc.GetElementsByTagName(className);
            bool foundOne = false;
            if (xmlnode.Count > 0)
            {
                foreach (XmlNode node in xmlnode)
                {
                    XmlNodeList childnode = node.ChildNodes;

                    foreach (XmlNode node2 in childnode)
                    {
                        if (node2.Name.Equals(name))
                        {
                            foundOne = true;
                        }
                    }
                }
            }
            if (foundOne)
                return true;
            else
                return false;
        }

        public void SetConfigValue(string name, string value, string className, bool replace)
        {
            string executableName = Application.ExecutablePath;
            FileInfo executableFileInfo = new FileInfo(executableName);
            string executableDirectoryName = executableFileInfo.DirectoryName;

            doc = new XmlDocument();

            try
            {
                doc.Load(executableDirectoryName + "\\Class.config.xml");
            }
            catch { };

            XmlNodeList xmlnode = doc.GetElementsByTagName(className);
            XmlElement element = null;
            bool foundOne = false;
            if (xmlnode.Count > 0)
            {
                foreach (XmlNode node in xmlnode)
                {
                    XmlNodeList childnode = node.ChildNodes;

                    foreach (XmlNode node2 in childnode)
                    {
                        if (node2.Name.Equals(name) && replace)
                        {
                            node2.InnerText = value;
                            foundOne = true;
                        }
                    }
                }
            }
            else
            {
                logSystem("Creating new element");
                element = doc.CreateElement(className);
            }
            if (!foundOne)
            {
                if (element != null)
                {
                    XmlNode ny = doc.CreateNode(XmlNodeType.Element, name, null);
                    ny.InnerText = value;
                    element.AppendChild(ny);
                    doc.DocumentElement.AppendChild(element);
                }
                else
                {
                    XmlNode temp = null;
                    foreach (XmlNode node in xmlnode)
                    {
                        if (node.Name.Equals(className))
                            temp = node;
                    }
                    if (temp != null)
                    {
                        XmlNode ny = doc.CreateNode(XmlNodeType.Element, name, null);
                        ny.InnerText = value;
                        temp.AppendChild(ny);
                    }
                    else
                        logSystem("Could not create node");
                }
            }
            if (foundOne && !replace)
            {
                //Do nothing
            }
            else
            {
                doc.Save(executableDirectoryName + "\\Class.config.xml");
            }
        }

        private XmlDocument doc;
        public string GetConfigString(string name, string className)
        {
            string executableName = Application.ExecutablePath;
            FileInfo executableFileInfo = new FileInfo(executableName);
            string executableDirectoryName = executableFileInfo.DirectoryName;

            doc = new XmlDocument();
            doc.Load(executableDirectoryName + "\\Class.config.xml");

            XmlNodeList xmlnode = doc.GetElementsByTagName(className);

            foreach (XmlNode node in xmlnode)
            {
                XmlNodeList childnode = node.ChildNodes;

                foreach (XmlNode node2 in childnode)
                {
                    if (node2.Name.Equals(name))
                    {
                        return node2.InnerText;
                    }
                }
            }
            return "none";
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Blacklist

        public Dictionary<string, GSpellTimer> Blacklisted = new Dictionary<string, GSpellTimer>();

        public void Blacklist(string name, int howlong_seconds)
        {
            try {
                GSpellTimer t = null;
                if (Blacklisted.TryGetValue(name, out t))
                {
                    Blacklisted.Remove(name);
                }
                t = new GSpellTimer((howlong_seconds * 10) * 1000);
                Blacklisted.Add(name, t);
                logSystem("Blacklisted " + name + " for " + (howlong_seconds) + "s");
            }
            catch { };
        }
        public void Blacklist(ulong GUID, int howlong_seconds)
        {
            Blacklist("GUID" + GUID, howlong_seconds);
        }

        public void Blacklist(GUnit unit, int howlong_seconds)
        {
            try
            {
                Blacklist(unit.GUID, howlong_seconds);
            }
            catch { };
        }

        public void UnBlacklist(string name)
        {
            try
            {
                Blacklisted.Remove(name);
                log("Un-Blacklisted " + name);
            }
            catch { };
        }

        public void UnBlacklist(ulong GUID)
        {
            try
            {
                UnBlacklist("GUID" + GUID);
            } catch { };
        }

        public void UnBlacklist(GUnit u)
        {
            try
            {
                UnBlacklist(u.GUID);
            } catch { };
        }

        public bool IsBlacklisted(string name)
        {
            try
            {
                GSpellTimer t = null;
                if (!Blacklisted.TryGetValue(name, out t))
                    return false;

                return !t.isReady;
            } catch { };
            return true;
        }

        public bool IsBlacklisted(ulong GUID)
        {
            return IsBlacklisted("GUID" + GUID);
        }

        public bool IsBlacklisted(GUnit unit)
        {
            try
            {
                return IsBlacklisted(unit.GUID);
            }
            catch { };
            return false;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------
    }
}
