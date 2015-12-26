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
using System.Threading;
using System.IO;

using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using MxBots.Bots.Actions;
using GLib;


namespace MxBots
{
    public class BotHub
    {
        #region events
        public event StringStatutTransfertEventHandler sendtext;
        #endregion

        #region vars
        private Bot[] UserBots;
        public int MainCharacter;
        uint camBase;
        static EventWaitHandle synchro = new AutoResetEvent(false); // synchr entre les thread lors sendtext
        private Thread[] Threadbot;
        private Thread BotWatcher;
        private Thread BotHealer;
        private Thread BotCentralizer;
        private GObjectList MainObjectList;
        public  static bool follow;
       
        private int MainFastSwitch;
        private int LastSwitch;
        private bool switchloc;
        private bool CameraHack;
        private float ZoomValue;
        private BotHelper BotHelper;
        public  const int followMainPriority=50;
        public  const int runToLocationPriority = 80;
        public  const int attackMainTargetPriority = 70;
        public  const int AgroPriority = 60;
        public  const int HealPriority = 90;
        public  const int GotAggroPriority = 65;
        public  const int PatrolPriority = 20;
        #endregion

        #region bizzareries
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr Hwnd);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr SetFocus(IntPtr hwnd);
        [DllImport("user32.dll")]
         public static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

        #endregion

        #region functions
        public BotHub(string[] bot)
        {
           // this.Bots = bot;
            UserBots = new Bot[bot.Length];
            Threadbot = new Thread[bot.Length];
            MainFastSwitch = Settings.Default.fastmainswitch - 1;
            switchloc = Settings.Default.switchloc;
            MainCharacter = 0;
            ZoomValue = 60;
            CameraHack = false;
            for(int i = 0;i<bot.Length;i++)
            {
                switch(bot[i])
                {
                 case "Mage":
                    UserBots[i] = new Bots.Mage(i);
                    
                    break;
                case "Druid":
                    UserBots[i] = new Bots.Druide(i);
                    break;
                case "Priest":
                    UserBots[i] = new Bots.Prêtre(i);
                    break;
                case "Warrior":
                    UserBots[i] = new Bots.Guerrier(i);
                    break;
                case "Warlock":
                    UserBots[i] = new Bots.Démoniste(i);
                    break;
                case "Death Knight":
                    UserBots[i] = new Bots.DeathKnight(i);
                    break;
                case "Rogue":
                    UserBots[i] = new Bots.Voleur(i);
                    break;
                case "Shaman":
                    UserBots[i] = new Bots.Shaman(i);
                    break;
                case "Hunter":
                    UserBots[i] = new Bots.Chasseur(i);
                    break;
                case "Paladin":
                    UserBots[i] = new Bots.Paladin(i);
                    break;
                }
               
            } 
            
            follow = true;
     
        }
        public string readmemory(int adress, int bot)
        {
            string retour;
            
                retour = "Les variables de retour sont : \n UINT: " + 
                    UserBots[bot-1].Objectlist.Memory.ReadUInt((uint)adress).ToString() +
                    "\nINT " + UserBots[bot-1].Objectlist.Memory.ReadInt((uint)adress).ToString() +
                    "\nASCII " + UserBots[bot - 1].Objectlist.Memory.ReadASCIIString((uint)adress,10)+
                    "\nUNICODE " + UserBots[bot - 1].Objectlist.Memory.ReadUnicodeString((uint)adress, 10); 
            return retour;
        }
        public void war3(int i)
        {
            UserBots[MainCharacter].Objectlist.SetMouseCoordFlag();
            MxBots.Hook.cliked += new Hook.MouseTransfertEvent(saclick);
        }
        public void switchCameraHack(int i)
        {
            camBase = UserBots[MainCharacter].Objectlist.Memory.ReadUInt(UserBots[MainCharacter].Objectlist.Memory.ReadUInt(0x00B0d544) + 0x7E20);
            if (!CameraHack)
            {
                
                sendconsole("CamZ: " + UserBots[MainCharacter].Objectlist.Memory.ReadFloat(camBase + 0x118), ConsoleLvl.Debug);
                sendconsole("Enabling Camera hack !", ConsoleLvl.Medium);
                CameraHack = true;
               Thread changeOffsetThread = new Thread(new ThreadStart(this.changeOffSetZooMValue));

                changeOffsetThread.Start();
                       // UserBots[MainCharacter].Objectlist.Memory.WriteFloat(0xA6FDDC , UserBots[MainCharacter].Objectlist.LocalPlayer.Location.X);
                       // UserBots[MainCharacter].Objectlist.Memory.WriteFloat(0xA6FDDC+0x4, UserBots[MainCharacter].Objectlist.LocalPlayer.Location.Y);
                       //UserBots[MainCharacter].Objectlist.Memory.WriteFloat(0xA6FDDC+0x8 , UserBots[MainCharacter].Objectlist.LocalPlayer.Location.Z +30);
                       //UserBots[MainCharacter].Objectlist.Memory.WriteFloat(UserBots[MainCharacter].Objectlist.LocalPlayer.ObjectPointer + 0x1BA0, 255);
                       //UserBots[MainCharacter].Objectlist.Memory.WriteFloat(UserBots[MainCharacter].Objectlist.LocalPlayer.curMgr + 0x1BA0, 255);
            }
            else
            {
                CameraHack = false;
                sendconsole("CamZ: " + UserBots[MainCharacter].Objectlist.Memory.ReadFloat(camBase + 0x118), ConsoleLvl.Debug);
                UserBots[MainCharacter].Objectlist.Memory.WriteFloat(camBase + 0x118, 30);
                sendconsole("Disabling Camera hack !", ConsoleLvl.Medium);
            }
        }
        private void ActionToAllBot(SAction Action, int j,bool replace)
        {
            //j: 1 - All Bot , 2 - Not Main ,3- Main If semiMod
            bool replaced = false;
            for (int i = 0; i < SomeSettings.botnumber; i++)
            {
                if (i != MainCharacter || (j == 1 || (j == 3 && SomeSettings.BotMod == BotMode.Semi)))
                {
                    if (replace == true)
                    {
                        for (int k = 0; k < UserBots[i].ActionList.Count; k++)
                        {
                            if (UserBots[i].ActionList[k].Action == BotAction.GoToLocation)
                            {
                                UserBots[i].ActionList[k].Location =Action.Location;
                                replaced = true;
                            }
                        }
                        if (UserBots[i].ActionList.Count == 0 || replaced==false)
                        {
                            UserBots[i].ActionList.Add(Action);
                        }

                    }
                    else
                    {
                        UserBots[i].ActionList.Add(Action);
                    }
                }

            }
            //getActionList();
        }
        public void model()
        {
            try
            {
                sendconsole(UserBots[MainCharacter].Objectlist.LocalPlayerTarget.getDisplayID.ToString(), ConsoleLvl.BotHub);
                Thread.Sleep(3000);
                UserBots[MainCharacter].Objectlist.LocalPlayerTarget.getDisplayID = UserBots[MainCharacter].Objectlist.LocalPlayer.getDisplayID;
                UserBots[MainCharacter].Objectlist.UpdateModel();

            }
            catch
            {
            };

        }
        private void DelActionToAllBot(SAction Action)
        {
            for (int i = 0; i < SomeSettings.botnumber; i++)
            {
                for (int k=0;k< UserBots[i].ActionList.Count;k++)
                {
                    if (UserBots[i].ActionList[k].Action == Action.Action)
                    {
                        UserBots[i].ActionList.RemoveAt(k);
                    }
                }
            }
        }
        private void changeOffSetZooMValue()
        {
            while (CameraHack)
            {
                UserBots[MainCharacter].Objectlist.Memory.WriteFloat(camBase + 0x118, ZoomValue);
                Thread.Sleep(100);
            }
            Thread.CurrentThread.Abort();
        }
        public void saclick(MouseEventArgs e)
        {           
            if (e.Button == MouseButtons.Left)
            {
                sendconsole("X:"+UserBots[MainCharacter].Objectlist.MouseX+" Y:"+UserBots[MainCharacter].Objectlist.MouseY+" Z:"+UserBots[MainCharacter].Objectlist.MouseZ,ConsoleLvl.Debug);
               
                MxBots.Hook.cliked -= new Hook.MouseTransfertEvent(saclick);
                moveBotToLocation(new GLocation(UserBots[MainCharacter].Objectlist.MouseX,UserBots[MainCharacter].Objectlist.MouseY,UserBots[MainCharacter].Objectlist.MouseZ));
            }
            else if (e.Button == MouseButtons.Right)
            {
                MxBots.Hook.cliked -= new Hook.MouseTransfertEvent(saclick);
            }

        }
        private void moveBotToLocation(GLocation location)
        {

            sendconsole("Bot are moving to location", ConsoleLvl.Medium);
            follow = false;
            DelActionToAllBot(new SAction(BotAction.FollowMain));
            SAction todo = new SAction();
            todo.Action = BotAction.GoToLocation;
            todo.Priority = runToLocationPriority;
            todo.Location = location;
            ActionToAllBot(todo,3,true);
            
        }
        public string getpos( int bot)
        {
            string retour;

            retour = "X: " +
                UserBots[bot - 1].Objectlist.LocalPlayer.XPosition.ToString() +
                "\nY:" + UserBots[bot - 1].Objectlist.LocalPlayer.YPosition.ToString() +
                "\nZ:" + UserBots[bot - 1].Objectlist.LocalPlayer.ZPosition.ToString();
            return retour;
        }
        public void changeMain(int i)
        {
            if(UserBots.Length > i)

            {
                int old = MainCharacter;
                MainCharacter = i;
                UserBots[old].Ismain = false;
                UserBots[MainCharacter].Ismain = true;
                MainObjectList = UserBots[MainCharacter].Objectlist;
                BotHelper.Main = MainCharacter;

                sendconsole("Main Successfully changed !" , ConsoleLvl.BotHub);
                if (switchloc)
                {
                    RECT rct1 = new RECT();
                    RECT rct2 = new RECT();
                    GetWindowRect(UserBots[old].Handle, ref rct1);
                    GetWindowRect(UserBots[MainCharacter].Handle, ref rct2);
                   // sendconsole(rct2.Left.ToString() + " " + rct2.Top.ToString() + " " + rct2.Right.ToString() + " " + rct2.Bottom.ToString(), ConsoleLvl.BotHub);
                    SetWindowPos(UserBots[old].Handle.ToInt32(), 0, rct2.Left, rct2.Top, rct2.Right - rct2.Left, rct2.Bottom - rct2.Top, 0);
                    SetWindowPos(UserBots[MainCharacter].Handle.ToInt32(), 0, rct1.Left, rct1.Top, rct1.Right - rct1.Left, rct1.Bottom - rct1.Top, 0);
                }
                SetForegroundWindow(UserBots[MainCharacter].Handle);
                SetFocus(UserBots[MainCharacter].Handle);
                UserBots[MainCharacter].ReleaseKeys();
                
            }
            else
            {
                sendconsole("Error , Bot " + i.ToString() + " not existing!", ConsoleLvl.BotHub);
            }

        }
        public void getAllBotList()
        {
            
                for (int i = 0; i < UserBots.Length; i++)
                {
                    if (MainCharacter == i) 
                    {
                        sendconsole("Bot " + (UserBots[i].ID + 1).ToString() + ": " + UserBots[i].GetClasse() + " *Main*", ConsoleLvl.BotHub);
                    }
                    else
                    {
                        sendconsole("Bot " + (UserBots[i].ID + 1).ToString() + ": " +UserBots[i].GetClasse(), ConsoleLvl.BotHub);
              
                    }
                }
            
        }
        private void botStart()
        {
            BotHelper = new BotHelper(UserBots);
            BotHelper.sendtext += new StringStatutTransfertEventHandler(sendconsole);
            BotHelper.GotAggro += new AggroTransfertEventHandler(gotaggro);
            foreach (Bot b in UserBots)
            {
                b.BotStart(new StringStatutTransfertEventHandler(sendconsole),BotHelper);
               
            }
           
        }
        private void gotaggro(AggroTransfertEventArg g)
        {
            bool protect = false;
            
            if (!UserBots[g.Id].Tank && (g.Id != MainCharacter && SomeSettings.BotMod == BotMode.Semi))
            {
                protect = true;
                UserBots[g.Id].ActionList.Add(new SAction(BotAction.GotAggro,GotAggroPriority));
            }
            foreach (Bot b in UserBots)
            {
                b.addTarget(g.Guid);
      
            }

       
         //   ActionToAllBot(new SAction(BotAction.Attack,AgroPriority), 1, true);
        }
        public void letsgoandgetsomeMESCALINE()
        {
            botStart();
            MainObjectList = UserBots[MainCharacter].Objectlist;

            if (SomeSettings.BotMod == BotMode.Semi || SomeSettings.BotMod == BotMode.Classic)
            {
                ClassicEtSemiStart();
            }
            else if (SomeSettings.BotMod == BotMode.BG)
            {
                BgStart();
            }
            else if (SomeSettings.BotMod == BotMode.Multibot)
            {
                MultiBotStart();
            }

            //while (true)
            //{
            //    Thread.Sleep(100);
            //    VerifyBotStatut();
            //}
            
        }
        private void MultiBotStart()
        {
            nav = getMesh(SomeSettings.NavFile);
            SomeSettings.NavMesh = nav;
            foreach (Bot b in UserBots)
            {
                b.ActionList.Add(new SAction(BotAction.Patrol, PatrolPriority));
                Threadbot[b.ID] = new Thread(new ParameterizedThreadStart(MultiStartTH));
                Threadbot[b.ID].Start(b.ID);
               
            }
            BotWatcher = new Thread(new ThreadStart(BotHelper.WatcherOoO));
            BotWatcher.Start();

            BotHealer = new Thread(new ThreadStart(BotHelper.WatchHealth));
            BotHealer.Start();

            BotCentralizer = new Thread(new ThreadStart(BotHelper.CentralizeOoO));
            BotCentralizer.Start();

        }
        NavMesh nav;

        private void MultiStartTH(object id)
        {
            Bot Curbot = UserBots[(int)id];
           int wp = getClosetWp(Curbot.Objectlist.LocalPlayer.Location);
           Curbot.nav = nav;
           Curbot.GotoNodeID(wp);
           MainThreadLoop(id);
     
        }
        private int getClosetWp(GLocation loc)
        {
            int id=0;
            double distance = 99999999;
            foreach (Node n in nav.Nodes)
            {
                if (NavMesh.distance(n, loc) < distance)
                {
                    id = n.id;
                    distance = NavMesh.distance(n, loc);
                }

            }
            return id;
        }
        private NavMesh getMesh(string path)
        {
            NavMesh nav;

            FileStream myStream = new FileStream(path, FileMode.Open);
            IFormatter formatter = new BinaryFormatter();
            nav = formatter.Deserialize(myStream) as NavMesh;
            myStream.Close();
            return nav;
        }
        private void BgStart()
        {
          //inscription au bg avec ouverture du machin puis click
                
                foreach (Bot b in UserBots)
                {
                    Threadbot[b.ID] = new Thread(new ParameterizedThreadStart(BgBoucle));
                    Threadbot[b.ID].Start(b.ID);
                    sendconsole("zone id : " + b.Objectlist.LocalPlayer.ZoneID.ToString(), ConsoleLvl.BotHub);
                }
               

            //InscriptionBG();
            //enregistrement de lendroi duclick

            // regarder si ya un circuit predefini d'attente sinon en proposer d'en creer un

            //lancer le circuit 
            // si le truc pop accepeter
            //go lancer la boucle bg

        }
        int DecompteBg;
        private void BgBoucle(object data)
        {
            int id = (int)data;
            Bot Curbot = UserBots[id];
            int zoneid;
            while (true)
            { 
                zoneid = Curbot.Objectlist.LocalPlayer.ZoneID;
                if (zoneid == 3277)
                {
                    inBg("wsg");
                }
                else if (zoneid == 3277)
                {
                }
                else if (zoneid == 3277)
                {
                }
                else if (zoneid == 3277)
                {
                }
                else if (zoneid == 3277)
                {
                }
                else
                {
                }
            }
        }
        private void inBg(string bg)
        {
            NavMesh nav = getMesh(Directory.GetCurrentDirectory()+"\\Waypoints\\"+ bg+".xnav");


        }
        private void InscriptionBG()
        {
            foreach (Bot b in UserBots)
            {
                b.move.OpenPvP();

                Rectangle rect = new Rectangle();
                GetWindowRect(b.Handle, ref rect);
                b.ScreenRec = rect;
            }
            DecompteBg = UserBots.Length;
            sendconsole("Please click to enter in bg", ConsoleLvl.System);
            MxBots.Hook.StartMouseHook();
            MxBots.Hook.cliked += new Hook.MouseTransfertEvent(GetMouseForBg);
            
        }

        public void stopitnow()
        {
            foreach (Thread t in Threadbot)
            {
                t.Abort();
            }
        }

        private void GetMouseForBg(MouseEventArgs e)
        {
            foreach (Bot b in UserBots)
            {
                if(b.ScreenRec.Contains(e.X,e.Y))
                {
                    b.locClickRegBG = new Point(e.X, e.Y);

                    DecompteBg--;
                    break;
                }
            }
            if (DecompteBg == 0)
            {
                MxBots.Hook.StopMouseHook();
                
            }
        }
        private void ClassicEtSemiStart()
        {
            foreach (Bot b in UserBots)
            {
                //Memory.InjectDllToWindow(b.Handle,
                //sendconsole("pid b:" + b.PID.ToString());
                Threadbot[b.ID] = new Thread(new ParameterizedThreadStart(this.MainThreadLoop));
                Threadbot[b.ID].Start(b.ID);

            }
            BotWatcher = new Thread(new ThreadStart(BotHelper.WatcherOoO));
            BotWatcher.Start();

            BotHealer = new Thread(new ThreadStart(BotHelper.WatchHealth));
            BotHealer.Start();

            BotCentralizer = new Thread(new ThreadStart(BotHelper.CentralizeOoO));
            BotCentralizer.Start();

            ActionToAllBot(new SAction(BotAction.FollowMain, followMainPriority), 1, false);

            HKMDB AttackMainHK = SomeSettings.UserHotkey[2] as HKMDB;
            HKMDB FollowMainHK = SomeSettings.UserHotkey[3] as HKMDB;
            HKMDB ChangeMainHK = SomeSettings.UserHotkey[1] as HKMDB;
            HKMDB GoToLocHK = SomeSettings.UserHotkey[4] as HKMDB;
            HKMDB CamHackHK = SomeSettings.UserHotkey[5] as HKMDB;
            HKMDB AddCamHK = SomeSettings.UserHotkey[6] as HKMDB;
            HKMDB LowCamHK = SomeSettings.UserHotkey[7] as HKMDB;

            MxBots.Hook.StartMouseHook();

            SomeSettings.hkm.Register(AttackMainHK.MainKey(), (HotKeyModifier)AttackMainHK.Modifiers(), 1, new HotKeyEventHandler(AttackMainTarget));
            SomeSettings.hkm.Register(FollowMainHK.MainKey(), (HotKeyModifier)FollowMainHK.Modifiers(), 2, new HotKeyEventHandler(SwitchFollow));
            SomeSettings.hkm.Register(ChangeMainHK.MainKey(), (HotKeyModifier)ChangeMainHK.Modifiers(), 3, new HotKeyEventHandler(ChangeMainPress));
            SomeSettings.hkm.Register(GoToLocHK.MainKey(), (HotKeyModifier)GoToLocHK.Modifiers(), 4, new HotKeyEventHandler(war3));
            SomeSettings.hkm.Register(CamHackHK.MainKey(), (HotKeyModifier)CamHackHK.Modifiers(), 5, new HotKeyEventHandler(switchCameraHack));
            SomeSettings.hkm.Register(AddCamHK.MainKey(), (HotKeyModifier)AddCamHK.Modifiers(), 6, new HotKeyEventHandler(addCam));
            SomeSettings.hkm.Register(LowCamHK.MainKey(), (HotKeyModifier)LowCamHK.Modifiers(), 7, new HotKeyEventHandler(lowCam));

        }
        private void VerifyBotStatut()
        {

        }
        private void addCam(int i)
        {
            ZoomValue = ZoomValue + 5;
        }
        private void lowCam(int i)
        {
            ZoomValue = ZoomValue - 5;
        }
        private void ChangeMainPress(int i)
        {
            if (MainCharacter == MainFastSwitch)
            {
                changeMain(LastSwitch);
            }
            else if (MainFastSwitch >= SomeSettings.botnumber)
            {
                sendconsole("Error : Bot "+ ( MainFastSwitch + 1).ToString() +" doesn't exist !  Check the Global Configuration tab",ConsoleLvl.Error);
            }
            else
            {
                LastSwitch = MainCharacter;
                changeMain(MainFastSwitch);
                
            }

        }
        private void SwitchFollow(int i)
        {
            if (follow == true)
            {
                follow = false;
                sendconsole("Bot Following Paused ", ConsoleLvl.BotHub);
                DelActionToAllBot(new SAction(BotAction.FollowMain));
                
            }
            else
            {
                follow = true;
                sendconsole("Bot Following ", ConsoleLvl.BotHub);
                ActionToAllBot(new SAction(BotAction.FollowMain, followMainPriority), 1, false);
            }
        }
        public void getActionList()
        {
            foreach (Bot b in UserBots)
            {
                foreach (SAction sa in b.ActionList)
                {
                    sendconsole("Bot" + b.ID.ToString() + ":" + sa.Action.ToString(), ConsoleLvl.System);
                }
            }
        }
        private void DaBossMainThread()
        {
            while (true)
            {
                
                Thread.Sleep(10);
            }
        }
        private void MainThreadLoop(object data)
        {

            int id = (int)data; 
            Bot CurrentBot = UserBots[id];
            
           // CurrentBot.BotStart();
            
            while (Thread.CurrentThread.IsAlive)
            {
                Thread.Sleep(50);
                if ((CurrentBot.ID != MainCharacter || SomeSettings.BotMod != BotMode.Classic)&& CurrentBot.Objectlist.LocalPlayer.IsAlive)
                {
                    CurrentBot.refreshInfo();
                    CurrentBot.refreshList();
                    SAction AttackM = new SAction();
                    SAction AttackN = new SAction();
                    Thread CurrentActionThread;
                    AttackN.Action = BotAction.Attack;
                    AttackN.Priority = AgroPriority;
                    AttackM.Priority = attackMainTargetPriority;
                    AttackM.Action = BotAction.AttackMain;
                    SAction healA = new SAction();
                    bool AttackNb = false;
                    bool AttackMb =false;
                    bool healAt = false;
                    if (CurrentBot.TargetList.Count != 0)
                    {
                        CurrentBot.ActionList.Add(AttackN);
                        AttackNb = true;
                    }
                    if (CurrentBot.MainTargetList.Count != 0)
                    {
                        CurrentBot.ActionList.Add(AttackM);
                        AttackMb = true;
                    }
                    SAction MaxAction = new SAction();

                    if (CurrentBot.HealList.Count != 0)
                    {
                       
                        healA.Action = BotAction.Heal;
                        healA.Priority = HealPriority;
                        CurrentBot.ActionList.Add(healA);
                        healAt = true;
                    }

                    CurrentBot.ActionList.Sort();
                  //  bool hasAttackAction;
                    CurrentBot.BreakCurActionB = false;
                    if (CurrentBot.ActionList.Count != 0)
                    {
                       
                        MaxAction = CurrentBot.ActionList[0];
                        if (MaxAction.Action == BotAction.GoToLocation)
                        {

                            CurrentActionThread = new Thread(delegate()
                            {
                                threadMoveToLoc(id, CurrentBot.ActionList[0].Location);
                            });
                            GLocation Curloc = CurrentBot.ActionList[0].Location;
                            CurrentActionThread.Start();

                            while (CurrentActionThread.IsAlive)
                            {

                                Thread.Sleep(40);
                            }
                            CurrentBot.ActionList.Remove(MaxAction);
                            

                        }
                        else if (MaxAction.Action == BotAction.FollowMain)
                        {
                            CurrentActionThread = new Thread(delegate()
                            {
                                threadMoveToMain(id);
                            });
                            CurrentActionThread.Start();
                            while (CurrentActionThread.IsAlive)
                            {

                                Thread.Sleep(40);
                            }


                        }
                        else if (MaxAction.Action == BotAction.Patrol)
                        {
                            CurrentActionThread = new Thread(delegate()
                            {
                                threadPatrol(id);
                            });
                            CurrentActionThread.Start();
                            while (CurrentActionThread.IsAlive)
                            {

                                Thread.Sleep(40);
                            }


                        }
                        else if (MaxAction.Action == BotAction.Attack)
                        {                           
                           
                                sendconsole("Bot Attacking!", ConsoleLvl.High);

                                CurrentActionThread = new Thread(delegate()
                                {
                                    threadAttack(id);
                                });
                                CurrentActionThread.Start();
                                while (CurrentActionThread.IsAlive)
                                {

                                    Thread.Sleep(40);
                                }

                        }
                        else if (MaxAction.Action == BotAction.AttackMain)
                        {

                            sendconsole("Bot Attacking!", ConsoleLvl.High);

                            CurrentActionThread = new Thread(delegate()
                            {
                                threadAttackMain(id);
                            });
                            CurrentActionThread.Start();
                            while (CurrentActionThread.IsAlive)
                            {

                                Thread.Sleep(40);
                            }

                        }
                        else if (MaxAction.Action == BotAction.Heal)
                        {

                            sendconsole("Bot Healing!", ConsoleLvl.High);

                            CurrentActionThread = new Thread(delegate()
                            {
                                threadHeal(id);
                            });
                            CurrentActionThread.Start();
                            while (CurrentActionThread.IsAlive)
                            {

                                Thread.Sleep(40);
                            }

                        }
                        else if (MaxAction.Action == BotAction.GotAggro)
                        {

                            sendconsole("Bot Got Aggro!", ConsoleLvl.High);

                            CurrentActionThread = new Thread(delegate()
                            {
                                threadAggro(id);
                            });
                            CurrentActionThread.Start();
                            while (CurrentActionThread.IsAlive)
                            {

                                Thread.Sleep(40);
                            }
                            CurrentBot.ActionList.Remove(MaxAction);

                        }
                        if (AttackMb)
                        {
                            CurrentBot.ActionList.Remove(AttackM);
                        }
                        if (AttackNb)
                        {
                            CurrentBot.ActionList.Remove(AttackN);
                        }
                        if (healAt)
                        {
                            CurrentBot.ActionList.Remove(healA);
                        }

                    }



                }
            }

        }
        private void threadAttack(int i)
        {
           
           UserBots[i].AttackTargets(false);
           
           // UserBots[i].Cibler(UserBots[i].GUID);
            Thread.CurrentThread.Abort();
        }
        private void threadPatrol(int i)
        {

            UserBots[i].Patrol();

            // UserBots[i].Cibler(UserBots[i].GUID);
            Thread.CurrentThread.Abort();
        }
        private void threadAttackMain(int i)
        {

            UserBots[i].AttackTargets(true);

            // UserBots[i].Cibler(UserBots[i].GUID);
            Thread.CurrentThread.Abort();
        }
        private void threadHeal(int i)
        {

            UserBots[i].HealTarget();

            // UserBots[i].Cibler(UserBots[i].GUID);
            Thread.CurrentThread.Abort();
        }
        private void threadAggro(int i)
        {
            UserBots[i].GotAggro();
            Thread.CurrentThread.Abort();
        }
        private void threadMoveToMain(int id)
        {

                UserBots[id].deplacement(UserBots[MainCharacter].GUID, MainObjectList.LocalPlayer);
                Thread.CurrentThread.Abort();
  
        }
        private void threadMoveToLoc(int id,GLocation loc)
        {
            UserBots[id].deplacement(loc);
            Thread.CurrentThread.Abort();
        }
        private void checkThreadDistance(int i,GLocation loc)
        {
            while(UserBots[i].Objectlist.LocalPlayer.Location.distanceFrom(loc) >1)
            {
                sendconsole("Distance: " + UserBots[i].Objectlist.LocalPlayer.Location.distanceFrom(loc).ToString(),ConsoleLvl.Debug);
                Thread.Sleep(100);
            }
            Thread.CurrentThread.Abort();
        }
        private void AttackMainTarget(int i)
        {
            if (UserBots[MainCharacter].Objectlist.LocalPlayerTarget !=null )
            {
                sendconsole("Attacking Main target", ConsoleLvl.BotHub);
                sendconsole("Target Guid:" + UserBots[MainCharacter].Objectlist.LocalPlayerTarget.GUID.ToString(), ConsoleLvl.Debug);
                foreach (Bot b in UserBots)
                {
                    b.setMaintarget(UserBots[MainCharacter].Objectlist.LocalPlayerTarget.GUID);
                }

               // ActionToAllBot(new SAction(BotAction.Attack, attackMainTargetPriority, UserBots[MainCharacter].Objectlist.LocalPlayerTarget.GUID), 1, true);
      
            }
            else
            {
                sendconsole("No target selected!", ConsoleLvl.BotHub);
            }
        }
        private void sendconsole(string s,ConsoleLvl c)
        {
            if(sendtext != null)
            {
               
                sendtext(new StringStatutTransfertEventArg(s,c));
            }
        }
        private void sendconsole(StringStatutTransfertEventArg e)
        {
            if (sendtext != null)
            {

                sendtext(e);
            }
        }
        public string getClasse(int i)
        {
            return Bot.ClassToString(UserBots[i].Classe);
        }
        public void setHWND(int i,IntPtr hwnd)
        {
            UserBots[i].Handle = hwnd;
        }
        public void setPID(int i, IntPtr Pid)
        {
            UserBots[i].PID = Pid;
            
        }
        public void StopAllThread()
        {
            foreach (Thread t in Threadbot)
            {

                if (t != null)
                {
                    t.Abort();
                }
            }
        }
        #endregion

        #region accesseurs
  
        #endregion
    }

    public class StringStatutTransfertEventArg : EventArgs
    {
        private string chaine;
        private ConsoleLvl statut;

        public StringStatutTransfertEventArg()
            : this(string.Empty,ConsoleLvl.BotStatut)
        {
        }

        public StringStatutTransfertEventArg(string chaine,ConsoleLvl statut)
            : base()
        {
            this.chaine = chaine;
            this.statut = statut;
        }
        public string Chaine
        {
            get
            {
                return chaine;
            }
        }
        public ConsoleLvl Consolelvl
        {
            get
            {
                return statut;
            }
        }

    }
    public delegate void StringStatutTransfertEventHandler(StringStatutTransfertEventArg e);
    public class AggroTransfertEventArg : EventArgs
    {
        private ulong guid;
        private int id;

        public AggroTransfertEventArg()
            : this(0,0)
        {
        }

        public AggroTransfertEventArg(ulong guid,int id)
            : base()
        {
            this.guid = guid;
            this.id = id;
    
        }
        public ulong Guid
        {
            get
            {
                return guid;
            }
        }
        public int Id
        {
            get
            {
                return id;
            }
        }
 
 

    }
    public delegate void AggroTransfertEventHandler(AggroTransfertEventArg e);
}
