using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using MxBots.Properties;
using System.Runtime.InteropServices;

namespace MxBots
{
    public partial class Form1 : Form
    {

        #region variables
        public const int maxbot = 10;
        public ComboBox[] arg;
        private Label[] ALabel;//Array des labels avant les combobox
        public int nombrebot;
        public string[] bot;
        private Bitmap bmp;
        private Graphics graph;
        private Point LocationInitialeDuCurseur;
        private bool cliksurtest;
        SoundLib sound = new SoundLib();
        public static string executableDirectoryName;
        #endregion

        #region var temp
        private bool olol = false;
        private Timer transtime = new Timer();
        private bool allé = false, retour = false;
        #endregion

        public Form1()
        {
         //Settings.Default.Reset();
            
         //  Settings.Default.Save();
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            nombrebot = Settings.Default.nombre_choix;
            if (Settings.Default.first_use == true)
            {
                SomeSettings.bot_firstuse = true;
            }
      
            if (nombrebot > maxbot)
            {
                nombrebot = maxbot; ;
                Settings.Default.nombre_choix = maxbot;
            }
            creation_combobox();
            creation_label();

          
            if (Settings.Default.Xbot_mode == "Classic")
            {
                for (int i = 2; i <= maxbot; i++)
                {
                    comboBox1.Items.Add(i.ToString());
                }

                comboBox1.SelectedIndex = nombrebot - 2;
            }
            else
            {
                for (int i = 1; i <= maxbot; i++)
                {
                    comboBox1.Items.Add(i.ToString());
                }

                comboBox1.SelectedIndex = nombrebot - 1;
            }
            comboBox2.SelectedItem = Settings.Default.Xbot_mode;
           
            combobox_refresh();
            
            this.TopMost = false;
            cliksurtest = false;
            string executableName = Application.ExecutablePath;
            FileInfo executableFileInfo = new FileInfo(executableName);
            executableDirectoryName = executableFileInfo.DirectoryName;
            getKeycommon();

            this.Opacity = 0;
            transtime.Interval = 13;

            transtime.Tick += new EventHandler(transtimer_Tick);
            transtime.Enabled = true;

         
              if(Settings.Default.music)
              {
            sound.play();
              }
            


        }
        
        private void transtimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity != 1)
            {
                this.Opacity += 0.01;
               // sound.engine.SoundVolume += 0.01f;
            }
            else
            {
                transtime.Enabled = false;
            }
        }
        private void getKeycommon()
        {

       
            XmlDocument doc = new XmlDocument();
            string path = executableDirectoryName + "\\Settings\\Keys.xml";
            
            doc.Load(path);
           
            //Load factions
            SomeSettings.Faction = doc.GetElementsByTagName("Key");


            
        }
        private void saveKeys()
        {
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            StringBuilder output = new StringBuilder();

            XmlTextWriter writer = new XmlTextWriter(executableDirectoryName + "\\Settings\\Keys.xml", null);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            // Add elements to the file
            writer.WriteStartElement("Keys");


            foreach (XmlNode faction in SomeSettings.Faction)
            {

                XmlAttributeCollection at = faction.Attributes;
                writer.WriteStartElement("Key");

                //Add an attribute to the previously created element 
                writer.WriteAttributeString("KeyName", faction.Attributes[0].InnerText);
                writer.WriteAttributeString("ShiftState", faction.Attributes[1].InnerText);
                writer.WriteAttributeString("BarState", faction.Attributes[2].InnerText);
                writer.WriteAttributeString("Char", faction.Attributes[3].InnerText);
                //add sub-elements


                //End the item element
                writer.WriteEndElement();  // end item



            }
            // Ends the document
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();  
        }
        private void creation_combobox()
        {
            bool ok = true;
            //si la premiere fois que l'utilisateur lance le logiciel
            if ( Settings.Default.bots == null || Settings.Default.bots.Count != maxbot)
            {
                Settings.Default.bots = new System.Collections.ArrayList();

                Settings.Default.bots.Capacity = maxbot;
                ok = false;
            }
            
            System.Collections.ArrayList bots = Settings.Default.bots;
         
            arg = new ComboBox[maxbot];
            for (int q = 0; q < (maxbot); q++)
            {
                arg[q] = new ComboBox();
            }
            int x = 180;
            int y = 115;
            int i = 1;         
            foreach (ComboBox c in arg)
            {                            
                c.Items.AddRange(new object[] {
                "Warrior",
                "Mage",
                "Priest",
                "Warlock",
                "Rogue",
                "Paladin",
                "Death Knight",
                "Shaman",
                "Hunter",
                "Druid"});
                c.Name = "Warrior" + i.ToString(); ;
                c.Location = new System.Drawing.Point(x, y);
               // string classes = Settings.Default.bots;
               // classes.
              
                c.Visible = true;
                c.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                c.FormattingEnabled = true;
                c.Size = new System.Drawing.Size(101, 21);
                if (i > nombrebot)
                {
                    c.Enabled = false;
                }
                if (ok == true)
                { c.SelectedIndex = (int)bots[i - 1]; }
                else
                {
                    c.SelectedIndex = 0;
                }

                if (c.SelectedIndex == -1)
                {
                    c.SelectedIndex = 0;
                }
               // this.Controls.Add(c);
                this.tabPage1.Controls.Add(c);
         
                if (i%5 == 0 && i != 0)
                {
                    x = 155 + x;
                    y = 115;
                }
                else
                {
                    y = y + 24;
                }
                i++;
            }
        }
        private void creation_label()
        {
            ALabel = new Label[arg.Length];
            for(int i = 0;i< arg.Length;i++)
            {

                ALabel[i] = new Label();
                ALabel[i].Visible = true;
                ALabel[i].ForeColor = Color.White;
                ALabel[i].BackColor = Color.Transparent;
                ALabel[i].TextAlign = ContentAlignment.MiddleRight;
                ALabel[i].Location = new Point(arg[i].Location.X- ALabel[i].Size.Width,arg[i].Location.Y);

        
                this.tabPage1.Controls.Add(ALabel[i]);
                
            }
        }
        private void label_refresh()
        {
            for (int i = 0; i < ALabel.Length; i++)
            {
                
                    if (Settings.Default.Xbot_mode == "Classic")
                    {
                        if (i == 0)
                        {
                            ALabel[i].Text = "Main: ";
                        }
                        else
                        {
                            ALabel[i].Text = "Bot " + i.ToString() + ":";
                        }
                    }
                    else
                    {
                        ALabel[i].Text = "Bot " + (i + 1).ToString() + ":";
                    }
                    if (i < nombrebot)
                    {
                        ALabel[i].Visible = true;
                    }
                    else
                    {
                        ALabel[i].Visible = true;
                    }
                 
               
              
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = 0;


        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void combobox_refresh()
        {
            
            int i = 0;
            foreach (ComboBox c in arg)
            {
                if (i < nombrebot)
                {
                    c.Enabled = true; //visible
                }
                else
                {
                    c.Enabled = false;
                }
                i++;
            }

            label_refresh();
         
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nombrebot = int.Parse(comboBox1.SelectedItem.ToString());
            combobox_refresh();
   
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            savesetting();
            
        }
        private void savesetting()
        {
            Settings.Default.nombre_choix = nombrebot;
            int i = 0;
            if (Settings.Default.bots.Count == 0)
            {
                foreach (ComboBox c in arg)
                {


                    Settings.Default.bots.Add(c.SelectedIndex);
                    i++;
                }
            }
            else
            {
                foreach (ComboBox c in arg)
                {


                    Settings.Default.bots[i] = c.SelectedIndex;
                    i++;
                }
            }
            HKMDB.serialize(hotkeymanager1.listOfHotkeys);

            saveKeys();
            this.combatSettings1.save();
            Settings.Default.first_use = false;
            Settings.Default.Save();

            ClassSettings.Default.Save();
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
              switch (comboBox2.SelectedItem.ToString())
            {
                case "Classic":
                    SomeSettings.BotMod = BotMode.Classic;
                    break;
                case "Semi-Automated":
                    SomeSettings.BotMod = BotMode.Semi;
                    break;
                case "MultiBot":
                    SomeSettings.BotMod = BotMode.Multibot;
                    break;
                case "XPlayBack Mode":
                    SomeSettings.BotMod = BotMode.Xplayback;
                    break;
                case "Battleground Farming":
                    SomeSettings.BotMod = BotMode.BG;
                    break;
                case "Instance Farming":
                    SomeSettings.BotMod = BotMode.Instance;
                    break;

            }
            bool test = true;
            if (SomeSettings.BotMod == BotMode.Multibot && SomeSettings.NavFile == "")
            {
                MessageBox.Show("No path selected :(");

                test = false;
            }
            if (test)
            {
                tabPage1.Show();
                bot = new string[nombrebot];

                int i = 0;
                foreach (ComboBox c in arg)
                {
                    if (c.Enabled == true)
                    {
                        bot[i] = c.SelectedItem.ToString();

                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                SomeSettings.bots = bot;
                SomeSettings.botnumber = nombrebot;
          
                SomeSettings.working = true;
                savesetting();
                sound.engine.StopAllSounds();
                this.Dispose();
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            cliksurtest = true;
            LocationInitialeDuCurseur = e.Location;

          
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(cliksurtest == true)
            {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            int width=0,height=0;
            int lx = 0,ly =0;
                
             graph = Graphics.FromImage(bmp);
             if ((e.Location.X - LocationInitialeDuCurseur.X) > 0)
             {
                 width = e.Location.X - LocationInitialeDuCurseur.X;
                 lx = LocationInitialeDuCurseur.X;
             }
             if ((e.Location.X - LocationInitialeDuCurseur.X) < 0)
             {
                 width = Math.Abs(LocationInitialeDuCurseur.X - e.Location.X);
                 lx = e.Location.X;
             }
             if ((e.Location.Y - LocationInitialeDuCurseur.Y) > 0)
             {
                 height = e.Location.Y - LocationInitialeDuCurseur.Y;
                 ly = LocationInitialeDuCurseur.Y;
             }
             if ((e.Location.Y - LocationInitialeDuCurseur.Y) < 0)
             {
                 height = Math.Abs(LocationInitialeDuCurseur.Y - e.Location.Y);
                 ly = e.Location.Y;
             }
                Rectangle Rectangle = new Rectangle( lx, ly,width, height);


                graph.FillRectangle(new SolidBrush(Color.Fuchsia), Rectangle);
                //byte lol = 1;
                
            pictureBox1.Image = bmp;
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            cliksurtest = false;


            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (olol == false)
            {
                this.TransparencyKey = Color.White;
                this.BackColor = Color.White;
                olol = true;
            }
            else
            {

                this.BackColor = Color.FromName("control");
                this.TransparencyKey = Color.Empty;
                olol = false;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            transtime = new Timer();
              this.TransparencyKey = Color.Fuchsia;
            this.BackColor = Color.Fuchsia;
          
            transtime.Enabled = true;
            allé = true;
            transtime.Interval = 10;
           
            transtime.Tick += new EventHandler(transtime_Tick);
            transtime_Tick(sender, e);
        }
        private void transtime_Tick(object sender, EventArgs e)
        {
            if (allé == true && retour == false)
            {
                this.Opacity -= 0.01;
                if (this.Opacity == 0)
                {
                    allé = false;
                    retour = true;
                }

            }
            else
            {
                this.Opacity += 0.01;
                if (this.Opacity == 1)
                {
                    allé = true;
                    retour = false;
                    transtime.Enabled = false;
                }
            }
           
        }
        private void combatSettings1_Load(object sender, EventArgs e)
        {

        }
        private void button5_Click(object sender, EventArgs e)
        {
            savesetting();
        }
        private void invisibleBotConfigPanels()
        {
            foreach (UserControl c in splitContainer1.Panel2.Controls)
            {
                c.Visible = false;
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            invisibleBotConfigPanels();

            switch (listBox1.SelectedItem.ToString())
            {
                case"XBot Application Settings":
                    xC_XbConfig1.Visible = true;
                    break;
                case"Global Bots Settings":
                    xC_GlobalBotSettings1.Visible = true;
                    break;
                case"Fun Stuff":
                    xC_fun1.Visible = true;
                    break;
                case "Console Customisation":
                    xC_CustomConsole1.Visible = true;
                    break;
                case "General Binds":
                    xC_CommonBinds1.Visible = true;
                    break;
            }
        }
        private void xC_GlobalBotSettings1_Load(object sender, EventArgs e)
        {

        }
        public static void ModifyFaction(KeyEventArgs e,string quoi)
        {
            foreach (XmlNode faction in SomeSettings.Faction)
            {
                if (faction.Attributes[0].InnerText == quoi)
                {
                    faction.Attributes[3].InnerText = e.KeyCode.ToString();
                    faction.Attributes[1].InnerText = e.Modifiers.ToString();
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.Xbot_mode = comboBox2.SelectedItem.ToString();
            object index = comboBox1.SelectedItem;
            if (Settings.Default.Xbot_mode != "Classic" && Settings.Default.Xbot_mode != "Semi-Automated" && Settings.Default.Xbot_mode != "Battleground Farming"  && Settings.Default.Xbot_mode != "MultiBot")
            {
                label5.Visible = true;
                button1.Enabled = false;
             
            }
            else
            {
                label5.Visible = false;
                button1.Enabled = true;
            }

            if (Settings.Default.Xbot_mode == "Classic")
            {
                selectedComboBox1Classic();
            }
            else
            {
                comboBox1.Items.Clear();
                for (int i = 1; i <= maxbot; i++)
                {
                    comboBox1.Items.Add(i.ToString());
                }

                comboBox1.SelectedItem = index;
            }
            if (Settings.Default.Xbot_mode == "MultiBot")
            {
                button3.Visible = true;
                label6.Visible = true;
            }
            else
            {
                button3.Visible = false;
                label6.Visible = false;
            }
            label_refresh();

        }
        private void selectedComboBox1Classic()
        {
            object index = comboBox1.SelectedItem;
            comboBox1.Items.Clear();
            for (int i = 2; i <= maxbot; i++)
            {
                comboBox1.Items.Add(i.ToString());
            }

            if (index == null)
            {
                index = "2";
            }
            else if (int.Parse(index.ToString()) == 1 || int.Parse(index.ToString()) == 0)
            {
                index = "2";
            }
            comboBox1.SelectedItem = index;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private bool move = false;
        private Point start_point = new Point(0, 0);

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //ReleaseCapture();
                //SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                move = true;
                this.start_point = new Point(e.X, e.Y);

            }

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                Point p1 = new Point(e.X, e.Y);
                Point p2 = this.PointToScreen(p1);
                Point p3 = new Point(p2.X - this.start_point.X,
                                     p2.Y - this.start_point.Y);
                this.Location = p3;

            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\Waypoints";
            openFileDialog1.Filter = "nav files (*.xnav)|*.xnav";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SomeSettings.NavFile = openFileDialog1.FileName;
                label6.Text = "Nav file : " + openFileDialog1.SafeFileName;
            }
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }
        
    }
  
}
