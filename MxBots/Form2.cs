using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Drawing.Drawing2D;
using MxBots.Properties;

namespace MxBots
{
   
    public partial class Form2 : Form
    {
        
        #region vars
         private Keys keypressedinrtb;
         public int botnumber;
         public string[] botstring;
         public bool debug;
         private int analyzencour;
         private bool analyzfini;//sert pas a grand chose pour linstant 
         private IntPtr[] hwnd;//temp pour hwnd lors de l'init
         private IntPtr[] pid; //temp pour pid lors de l'init
         private int currentKeyup = 0;//Pour les fonction keyup et keydown sur textbox1
         private string[] lastconsoleinput = new string[10];//10 dernieres commandes de l'user
         
         BotHub botHub;
        #endregion

        #region DLLimports
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(HandleRef hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("User32.dll")]
        public static extern string GetWindowText(int hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr handle, IntPtr lpdwProcessId);
        #endregion

        #region fonctions
        public Form2()
        {            
            InitializeComponent();
            
        }
        private void botHub_sendtext(StringStatutTransfertEventArg e)
        {
            sendtext(e.Chaine,e.Consolelvl);
        }         
        
        private void Form2_Load(object sender, EventArgs e)
        {
            
           SomeSettings.UserHotkey = HKMDB.deserialize();
            this.DoubleBuffered = true;
            this.TopMost = true;
            if (Settings.Default.consolesavelocation == true && SomeSettings.bot_firstuse == false && Settings.Default.consolesize != null)
            {             
                    this.Size = Settings.Default.consolesize;
                    this.Location = Settings.Default.consolelocation;           
            }
            else
            {
                 this.Location = new Point(SystemInformation.PrimaryMonitorSize.Width - this.Size.Width, 0);           
            }
            SomeSettings.consoleHiden = !Settings.Default.showconsole;
            SomeSettings.showballon = Settings.Default.showballoon;
            this.BackColor = Settings.Default.consolebackgroundcolor;
            textBox1.BackColor = Settings.Default.consolebackgroundcolor;
            richTextBox1.BackColor = Settings.Default.consolebackgroundcolor;
            textBox1.Font = Settings.Default.consolefont;
            richTextBox1.Font = Settings.Default.consolefont;
            label1.Font = Settings.Default.consolefont;
            label1.ForeColor = Settings.Default.consolefontcolor;
            textBox1.ForeColor = Settings.Default.consolefontcolor;
            richTextBox1.ForeColor = Settings.Default.consolefontcolor;
            textBox1.Size = new Size(textBox1.Size.Width,(int)Math.Truncate(label1.Font.Size) + 3);
            textBox1.Location = new Point(label1.Location.X + label1.Size.Width, textBox1.Location.Y );
            
            //appstart.InterceptKeys.Launch();
            //appstart.InterceptKeys.appuyé += new KeyTransfertEventHandler(InterceptKeys_appuyé);
           SomeSettings.hkm = new HotKeyManager();
            if (SomeSettings.working == false)
            {
                this.Dispose();
                Application.Exit();
                Application.ExitThread();
            }
            else
            {
                analyzencour = 0;
                analyzfini = false;
                botnumber = SomeSettings.botnumber;
              
                if (botnumber == 0)
                    this.Dispose();
                debug = Settings.Default.debug_mode;
                sendtext("     XBot By Um3w  ");
               // richTextBox1.ap
                sendtext("     Version: " + SomeSettings.ProductVersion);
                sendtext("");
                sendtext("User settings saved");
                sendtext("XBot mode: " + SomeSettings.getBotModstring(SomeSettings.BotMod));
                sendtext("->Bots number: " + SomeSettings.botnumber.ToString());
                launchingthread();
            }
        }
        private void launchingthread()
        {

            botHub = new BotHub(SomeSettings.bots);
            botHub.sendtext += new StringStatutTransfertEventHandler(botHub_sendtext);
            analyzehwnd();
        }
        private void analyzehwnd()
        {
            analyzencour = 0;
            analyzfini = false;
            hwnd = new IntPtr[botnumber];
            pid = new IntPtr[botnumber];
            //HotKeyModifier HKM = 
            HKMDB ConsoleHK =SomeSettings.UserHotkey[0] as HKMDB;
            sendtext("->Entering windows handle configuration",ConsoleLvl.Medium);
            sendtext("Click on the WoW window of the character asked then press " + ConsoleHK.KeyToString());
       //   sendtext(ConsoleHK.MainKey().ToString() + "-" + ConsoleHK.Modifiers().ToString());
           SomeSettings.hkm.Register(ConsoleHK.MainKey() ,(HotKeyModifier)ConsoleHK.Modifiers() , 1, new HotKeyEventHandler(analyzethwnd));
            sendtext("Main character: " +botHub.getClasse(analyzencour)+"...");
        }
        private void analyzethwnd(int i)
        {
            hwnd[analyzencour] = GetForegroundWindow();


            int capacity = GetWindowTextLength(new HandleRef(this, hwnd[analyzencour])) * 2;
            StringBuilder stringBuilder = new StringBuilder(capacity);
            GetWindowText(new HandleRef(this, hwnd[analyzencour]), stringBuilder, stringBuilder.Capacity);
            bool already = false;


            pid[analyzencour] = new IntPtr(Magic.SProcess.GetProcessFromWindow(hwnd[analyzencour]));
            for (int j = 0; j < botnumber; j++)
            {
                if (pid[j] == pid[analyzencour] && j != analyzencour)
                {
                    already = true;
                    sendtext("Windows already Selected !" ,ConsoleLvl.Error);
                }
            }
            if (already == false)
            {
                if (stringBuilder.ToString() == "World of Warcraft")
                {

                    if (debug == true)
                    {
                        sendtext("Ok - hwnd: " + hwnd[analyzencour].ToString() + "-pid: " + pid[analyzencour].ToString(), ConsoleLvl.Debug);
                    }
                    else
                    {
                        sendtext("Ok");
                    }
                    botHub.setHWND(analyzencour, hwnd[analyzencour]);
                    botHub.setPID(analyzencour, pid[analyzencour]);


                    analyzencour++;
                    if (analyzencour == botnumber)
                    {
                        sendtext("Analyse finie");
                        analyzfini = true;
                        SomeSettings.hkm.Unregister(1);

                        botHub.letsgoandgetsomeMESCALINE();

                    }
                    else
                    {
                        sendtext("Bot " + analyzencour.ToString() + ": " + botHub.getClasse(analyzencour) + "...");
                    }
                    //
                }
                else
                {
                    sendtext("Not a WoW Window(" + stringBuilder + ")", ConsoleLvl.Error);
                }
            }
           
        }       
        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
        }
        private void Form2_Enter(object sender, EventArgs e)
        {
            textBox1.Focus();
        }       
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (textBox1.Text != string.Empty)
                {
                    sendtext(">>" + textBox1.Text);                              
                    notifyIcon1.BalloonTipText = "Command Executed Successfully";
                    notifyIcon1.ShowBalloonTip(3000);
                    command(textBox1.Text);
                    if (textBox1.Text != lastconsoleinput[0])
                    {
                        for (int i = lastconsoleinput.Length - 1; i > 0; i--)
                        {
                            lastconsoleinput[i] = lastconsoleinput[i - 1];
                        }
                        lastconsoleinput[0] = textBox1.Text;
                    }
                    textBox1.Text = string.Empty;                   
                    currentKeyup = 0;
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == Keys.Up)
            {
                e.Handled = true;

                textBox1.Text = lastconsoleinput[currentKeyup];
                 if (currentKeyup != (lastconsoleinput.Length - 1))
                 {
                     currentKeyup++;
                 }
                    this.textBox1.SelectionStart = this.textBox1.Text.Length;
                    this.textBox1.ScrollToCaret();

              

            }
            else if (e.KeyData == Keys.Down && currentKeyup!=0)
            {
                e.Handled = true;
                if(currentKeyup != 1)
                {
                currentKeyup--;
                }

                textBox1.Text = lastconsoleinput[currentKeyup - 1];
                    this.textBox1.SelectionStart = this.textBox1.Text.Length;
                    this.textBox1.ScrollToCaret();
                
              
                
            }

            
        }
        private void ChangeConsoleMisc(string Icolor,Misc type)
        {
            if (type == Misc.BGCOLOR || type == Misc.FONTCOLOR)
            {
                ColorConverter CC = new ColorConverter();
                Color Colore = new Color(); ;
                try
                {
                    Colore = (Color)CC.ConvertFromString(Icolor);
                    if (type == Misc.FONTCOLOR)
                    {
                        richTextBox1.ForeColor = Colore;
                        textBox1.ForeColor = Colore;
                        label1.ForeColor = Colore;
                    }
                    else if (type == Misc.BGCOLOR)
                    {
                        richTextBox1.BackColor = Colore;
                        textBox1.BackColor = Colore;
                        label1.BackColor = Colore;
                        this.BackColor = Colore;
                    }

                }
                catch
                {
                    sendtext("Invalid Color");
                }

           
            }
            else if (type == Misc.ALPHA)
            {
                double Transparency;
                if(double.TryParse(Icolor,out Transparency))
                {
                    if(Transparency >=1 && Transparency <=100)
                    {
                        

                        this.Opacity = Transparency / 100;
                        
                    }
                    else
                    {
                        sendtext("Transparency must be set between 1 and 100 ");
                    }
                }
                else
                {
                    sendtext("Not a number");
                }
            }
          
        }
        private enum Misc{
            BGCOLOR,
            FONTCOLOR,
            ALPHA
        }
        private void command(string commande)
        {
            int numbot;
            commande = commande.ToLower();
            string[] command = commande.Split(' ');
       
           
                switch (command[0])
                {
                    case "help":
                        help();
                        break;
                        

                    case "youmew was here":
                        sendtext("Um3w is w4tching U");
                        break;

                    case "ALLERCHAMPAS":
                        sendtext("ALLER LEMAAAAAAAAAAAN");
                        break;

                    case "<<":
                        sendtext("");
                        sendtext("   (-(-_(-_-)_-)-)",ConsoleLvl.BotStatut);
                        sendtext("");
                        sendtext("BoRreD??? OloL");
                        break;

                    case "clr":
                        richTextBox1.Text = string.Empty;
                        break;

                    case "exit":
                        botHub.stopitnow();
                        this.Dispose();
                        break;

                    case "bot_main":
                        int result;
                        if (command.Length > 1 && int.TryParse(command[1], out result))
                        {

                            botHub.changeMain(result - 1);
                        }
                        else
                        {
                            sendtext("Error Arguments ( bot_main [bot id] )",ConsoleLvl.Error);
                        }
                        break;

                    case "bot_list":
                        botHub.getAllBotList();
                        break;

                    case "war3":
                        botHub.war3(0);
                        break;

                    case "bot_reset":
                        if (botHub != null)
                        {
                            botHub.StopAllThread();
                        }
                        sendtext("->Reseting Bots Handles", ConsoleLvl.System);
                        analyzehwnd();
                        break;

                    case "count":
                        sendtext(Bot.count.ToString());
                        break;

                    case "restart":
                        Settings.Default.Save();
                        Program.restart();
                        break;
                    case "actionlist":
                        botHub.getActionList();
                         break;

                    case "readmem":

                        if (command.Length < 1)
                        {

                            sendtext("Error Arguments");


                        }
                        else
                        {
                            if (command.Length == 2)
                            {
                                numbot = 1;
                            }
                            else 
                            {

                                numbot = int.Parse(command[2]);
                            }

                            ReadMemory(int.Parse(command[1], System.Globalization.NumberStyles.HexNumber), numbot);
                        }
                  
                        break;

                    case "getpos":
                        
                        if (command.Length == 2)
                        {
                            numbot = int.Parse(command[1]) ;
                            sendtext(botHub.getpos(numbot), ConsoleLvl.System);
                        }
                        else
                        {
                            sendtext("Error Arguments");


                        }
                        break;

                    case "model":
                        //botHub.ActionSurTousLesBots(Statut.Attaque);
                        botHub.model();
                        break;
                        
                    case "font_color":
                        if (command.Length > 1)
                        {
                            ChangeConsoleMisc(command[1],Misc.FONTCOLOR);
                        }
                        else
                        {
                            sendtext("Error Arguments");

                           
                        }
                        break;

                    case "console_bgcolor":
                        if (command.Length > 1)
                        {
                            ChangeConsoleMisc(command[1], Misc.BGCOLOR);
                        }
                        else
                        {
                            sendtext("Error Arguments");


                        }
                        break;


                    case "console_transparency":
                        if (command.Length > 1)
                        {
                            ChangeConsoleMisc(command[1], Misc.ALPHA);
                        }
                        else
                        {
                            sendtext("Error Arguments");


                        }
                        break;

               

                    default:
                        sendtext("Invalid Command");
                        break;
                }
            
        }
        private void help()
        {
            sendtext("help - get help");
            sendtext("clr  - clear the console");
            sendtext("bot_reset  - reset XBot");
            sendtext("restart  - restart XBot");
            sendtext("bot_main [bot id] - change main");
            sendtext("bot_list - get a list of current running bot");
            sendtext("font_color [color] - change console font color");
            sendtext("console_bgcolor [color] - change console background color");
            sendtext("console_transparency [1-100] - change console transparency");
            sendtext("actionlist - get current action list");
            sendtext("getpos - get bots position");
            sendtext("exit - quit");
        }
        private delegate void MaMethodeCallBack(string text,ConsoleLvl Consolelvl);
        private void sendtext(string text)
        {

     
                richTextBox1.AppendText(text + "\r\n");
                this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length;
                this.richTextBox1.ScrollToCaret();
            

       
        }
        private void ReadMemory(int adress, int wbot)
        {
            sendtext(botHub.readmemory(adress, wbot), ConsoleLvl.System);
        }
        private void sendtext(string text,ConsoleLvl lvl)
        {
            if (this.InvokeRequired)
            {
               // object[]
                this.Invoke(new MaMethodeCallBack(sendtext),text,lvl);
            }
            else
            {

                bool ok = true;
                Color couleur = Settings.Default.consolefontcolor;
                if (lvl == ConsoleLvl.Debug)
                {
                    if (debug == true)
                    {
                        couleur = Color.White;
                    }
                    else
                    {
                        ok = false;
                    }

                }

                else if (lvl == ConsoleLvl.System)
                {
                    couleur = Color.Gray;
                }
                else if (lvl == ConsoleLvl.BotStatut)
                {
                    couleur = Color.SlateBlue;
                }
                else if (lvl == ConsoleLvl.BotHub)
                {
                    couleur = Color.RoyalBlue;
                }
                else if (lvl == ConsoleLvl.Error)
                {
                    couleur = Color.Red;
                }
                else if (lvl == ConsoleLvl.Medium)
                {
                    couleur = Color.Azure;
                }
                else if (lvl == ConsoleLvl.High)
                {
                    couleur = Color.LightBlue;
                }
                if (ok == true)
                {
                    int textlenghtbefore = richTextBox1.TextLength;
                    richTextBox1.AppendText(text + "\r\n");
                    this.richTextBox1.Select(textlenghtbefore, textlenghtbefore + text.Length);
                    this.richTextBox1.SelectionColor = couleur;
                    this.richTextBox1.ScrollToCaret();
                }
            }
        }

        private void sendtextalasuite(string text)
        {
            richTextBox1.Text.TrimEnd(char.Parse("\n"));
            richTextBox1.Text.TrimEnd(char.Parse("\r"));
            richTextBox1.Text += text;
            this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length;
            this.richTextBox1.ScrollToCaret();
        }      
        private void richTextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            keypressedinrtb = e.KeyCode;
  
    
        }
        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Modifiers == Keys.None)
            {
                KeysConverter kc = new KeysConverter();

                textBox1.Text += kc.ConvertToString(keypressedinrtb);
                e.Handled = true;
                e.SuppressKeyPress = true;


                textBox1.Focus();
                this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                this.textBox1.SelectionStart = this.textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox1_Enter(object sender, EventArgs e)
        {

            richTextBox1.ScrollBars = RichTextBoxScrollBars.None;
            this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }      
        private void oéoéToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                this.Hide();
                oéoéToolStripMenuItem.Text = "Show Console";

            }
            else
            {
                this.Show();
                oéoéToolStripMenuItem.Text = "Hide Console";
            }
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.consolelocation = this.Location;
            Settings.Default.consolesize = this.Size;
            Settings.Default.Save();
            botHub.StopAllThread();
           SomeSettings.hkm.destructall();
        }
        private void Form2_Shown(object sender, EventArgs e)
        {
            if (SomeSettings.consoleHiden == false)
            {
                oéoéToolStripMenuItem.Text = "Hide Console";
            }
            else
            {
                this.Visible = false;
                oéoéToolStripMenuItem.Text = "Show Console";
            }
        }
        #endregion

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
    }
}
