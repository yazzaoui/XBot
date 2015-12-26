using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using MxBots.Bots.Actions;
using GLib;
using Magic;
using LuaInterface;
using System.Reflection;
using System.Windows.Forms;
using MxBots.Properties;
namespace MxBots
{
    public abstract class Bot
    {
        #region Variables Globales
        protected int hp;
        private int id;
      
        protected Classes classe;
        protected IntPtr handle, pid;
        public static int count = 0;
        protected LuaHelper LuaHelp;
        protected IntPtr WoWHandle;
        private GObjectList ObjectList;
        private GObjectList MainObjectList;
        public GUnit Cible;
        public double CibleI;
        private int curNode;
        public List<PrioStruct> TargetList { get; set; }
        public Stack<PrioStruct> MainTargetList { get; set; }

        private ArrayList GuidTarget;
        public List<HealthPrioStruct> HealList { get; set; }
        public List<RezPrioStruct> RezList { get; set; }
        public BotHelper BotHelper;
        private bool FinishedAction { get; set; }
        private string Name;
        private bool isplayer;
        public bool ContinuePatrol { get; set; }
        public bool BreakCurActionB {get;set;}
        protected GPlayerSelf localPlayer;
        public List<SAction> ActionList { get;set;} 
        public GMovement move;
        protected Lua LuaVM;
        protected ulong Guid;
        public bool Tank;
        public bool Dps;
        public bool Healer;
        public bool Ismain;
        private int waitTimePreCombat;
        private bool allowtargetmainstack;
        public int DistanceDuMain { get; set; }
        public int DistanceDeLaCibleMin { get; set; }
        public int DistanceDeLaCibleMax { get; set; }
        public int PreCombatTick { get; set; }

        public NavMesh nav { get; set; }
        public Point locClickRegBG { get; set; }
        public Rectangle ScreenRec { get; set; }
        
        private bool passif = Settings.Default.Passif;

        public bool IsBeingRez { get; set; }
        public event StringStatutTransfertEventHandler sendtext;
        #endregion

        #region Variables Virtuelles
        //rien parceque je veux
        #endregion

        #region dll
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd,
                                                StringBuilder text,
                                                int count);

        #endregion

        #region events
       
        #endregion

        #region FonctionsGlobales

        public void setMaintarget(ulong Guide)
        {
            PrioStruct prio = new PrioStruct();
            do
            {
                prio.Target = ObjectList.FindUnit(Guide);
                Thread.Sleep(10);
            } while (prio.Target == null);
            
            

            if (allowtargetmainstack || MainTargetList.Count == 0)
            {
                MainTargetList.Push(prio);
            }
            else
            {
                MainTargetList.Pop();
                MainTargetList.Push(prio);
            }

        }
        public void GotoNodeID(int id)
        {
            move.MoveToLocation(nav.Nodes[id].Loc);
            curNode = id;
            nav.Nodes[id].DejaPasser++;
        }
        public void Patrol()
        {
            Node nextnode;
            while (!this.BreakCurActionB)
            {
                nav.Nodes[curNode].SortNodesProches();
                nextnode = nav.Nodes[nav.Nodes[curNode].NodeProches[0].id];
                move.MoveToLocation(nextnode.Loc,1,true);
                nextnode.DejaPasser++;
                curNode = nextnode.id;

            }
        }
        public void addTarget(ulong Guid)
        {
            
            PrioStruct prio = GetStruct();
            do
            {
                prio.Target = ObjectList.FindUnit(Guid);
                Thread.Sleep(10);
            } while (prio.Target == null);
          
                bool deja = false;
                for (int i = 0; i < TargetList.Count; i++)
                {
                    if (TargetList[i].Target == prio.Target)
                    {
                        deja = true;
                       
                        
                    }
                }
                if (!deja)
                {
                    this.TargetList.Add(prio);
                }
               
                
          
            TargetList.Sort();
            showTargetlist();
        }
        protected virtual PrioStruct GetStruct()
        {
            return new PrioStruct();
        }
        public void addHealTarget(ulong Guid)
        {
            HealthPrioStruct prio = new HealthPrioStruct();
            do
            {
                prio.Target = ObjectList.FindUnit(Guid);
                Thread.Sleep(10);
            } while (prio.Target == null);

          

            bool deja = false;
            for (int i = 0; i < HealList.Count; i++)
            {

                if (HealList[i].Target == prio.Target)
                {
                    deja = true;
                    

                }
            }
            if (!deja)
            {
                this.HealList.Add(prio);
                SendConsole(prio.Target.Name + " will be healed ", ConsoleLvl.Medium);
            }



            HealList.Sort();
           // showTargetlist();
        }
        public void addRezTarget(ulong Guid,Bot b)
        {
            RezPrioStruct prio = new RezPrioStruct();
            do
            {
                prio.Target = ObjectList.FindUnit(Guid);
                Thread.Sleep(10);
            } while (prio.Target == null);



            bool deja = false;
            for (int i = 0; i < RezList.Count; i++)
            {

                if (RezList[i].Target == prio.Target)
                {
                    deja = true;


                }
            }
            for (int i = 0; i < HealList.Count; i++)
            {

                if (HealList[i].Target == prio.Target)
                {
                    HealList.RemoveAt(i);
                    break;


                }
            }
            
            if (!deja)
            {
                this.RezList.Add(prio);
                SendConsole(prio.Target.Name + " will be rez ", ConsoleLvl.Medium);
            }



            RezList.Sort();
            // showTargetlist();
        }
        protected void showTargetlist()
        {
           
            for (int j = 0; j < TargetList.Count; j++)
            {
                SendConsole((this.ID + 1).ToString() + ">>>" + TargetList[j].Target.GUID.ToString() + " - ", ConsoleLvl.High);
            }
          
        }
        public Bot(int i, Classes myc)
        {
            this.id = i;
            this.classe = myc;
            DistanceDuMain = (int)ClassSettings.Default.DistanceDuMain[(int)this.classe];
            //DistanceDuMain = ClassSettings.Default.
            DistanceDeLaCibleMin = (int)ClassSettings.Default.MinTargetDistance[(int)this.classe];
            DistanceDeLaCibleMax = (int)ClassSettings.Default.MaxTargetDistance[(int)this.classe];
            PreCombatTick = (int)ClassSettings.Default.PreCombatTick[(int)this.classe];
            allowtargetmainstack = Settings.Default.allowstacktarget;
            TargetList = new List<PrioStruct>();
            HealList = new List< HealthPrioStruct>();
            RezList = new List<RezPrioStruct>();
            MainTargetList = new Stack<PrioStruct>();
            IsBeingRez = false;
           // this.MainObjectList = main;
            if (i == 0)
            {
                this.Ismain = true;
            }
            else
            {
                this.Ismain = false;
            }
            ActionList = new List<SAction>();
            count++;

            LuaHelp = new LuaHelper(classe, this);
            LuaVM = LuaHelp.LuaVm;
            
        }
        public void DoString(string str)
       {
           Objectlist.DoString(str);
       }
        public void CastSpell(string spell)
        {
            DoString("CastSpellByName(\"" + spell + "\")");
        }
        public void BotStart(StringStatutTransfertEventHandler st, BotHelper BotHelper)
        {
            this.sendtext += st;
            InjectToWoW();
            Name = localPlayer.Name;
            hp = localPlayer.HealthPoints;
            Guid = localPlayer.GUID;
            this.BotHelper = BotHelper;
            //move.Jump();

            if (!passif)
            {
                Objectlist.HookEndScene();

                Objectlist.DoString("DEFAULT_CHAT_FRAME:AddMessage(\"Bot Injected ! \")");

                Objectlist.DoString("DEFAULT_CHAT_FRAME:AddMessage(\"Running XBot By Um3w Version " + SomeSettings.ProductVersion + "\")");
            }

            waitTimePreCombat = BotHelper.maxCombatTimeTick - this.PreCombatTick;
            BreakCurActionB = false;
            
        }
        public void refreshInfo()
        {

            hp = localPlayer.HealthPoints;
        }
        private void InjectToWoW()
        {

            ObjectList = new GObjectList(pid.ToInt32());
            //Get LocalPlayer
            localPlayer = ObjectList.LocalPlayer;
            //Load move
            move = new GMovement(ObjectList,this);
            move.sendtext += new StringStatutTransfertEventHandler(SendConsole);
            Objectlist.sendtext += new StringStatutTransfertEventHandler(SendConsole);

        }
        public void refreshList()
        {
            TargetList.RemoveAll(isDeadandNoCombat);
            CleanMainTargetList();
            HealList.RemoveAll(FullLife);
        }
        public Classes Classe
        {
            get { return classe; }
        }
        public void ReleaseKeys()
        {
            move.releaseKeys();
        }
        private void CleanMainTargetList()
        {
            if (MainTargetList.Count != 0)
            {
                while (MainTargetList.Peek().Target.IsDead)
                {
                    MainTargetList.Pop();
                    if (MainTargetList.Count == 0)
                    {
                        break;
                    }
                }
            }
        }
        public int HP
        {
            get {

                
                return hp;
            
            }
        }
        public ulong GUID
        {
            get { return Guid; }
        }
        public string Nom
        {
            get
            {
                return Name;
            }
        }
        public string GetClasse()
        {
            return ClassToString(classe);
        }
        public static string ClassToString(Classes c)
        {
            if (c == Classes.DK)
            {
                return "Death Knight";
            }
            else if(c == Classes.Chasseur)
            {
                return "Hunter";
            }
            else if (c == Classes.Démoniste)
            {
                return "Warlock";
            }
            else if (c == Classes.Druide)
            {
                return "Druid";
            }
            else if (c == Classes.Guerrier)
            {
                return "Warrior";
            }
            else if (c == Classes.Mage)
            {
                return "Mage";
            }
            else if (c == Classes.Paladin)
            {
                return "Paladin";
            }
            else if (c == Classes.Prêtre)
            {
                return "Priest";
            }
            else if (c == Classes.Shaman)
            {
                return "Shaman";
            }
            else if (c == Classes.Voleur)
            {
                return "Rogue";
            }
            else
            {
                return "OMFG BBQ ERROR";
            }
        }
        public static Classes StringToClass(string chaine)
        {
            chaine = chaine.ToLower();
            switch (chaine)
            {
                case "warrior":
                    return Classes.Guerrier;
                case "death knight":
                    return Classes.DK;
                case "warlock":
                    return Classes.Démoniste;
                case "druid":
                    return Classes.Druide;
                case"shaman":
                    return Classes.Shaman;
                case "mage":
                    return Classes.Mage;
                case "hunter":
                    return Classes.Chasseur;
                case "paladin":
                    return Classes.Paladin;
                case "priest":
                    return Classes.Prêtre;
                case "rogue":
                    return Classes.Voleur;
                default:
                    return Classes.None;

            }
        }
        public IntPtr Handle
        {
            get
            {
                return handle;
            }
            set
            {
                handle = value;
            }
        }
        
        public IntPtr PID
        {
            get
            {
                return pid;
            }
            set
            {
                pid = value;
            }
        }
        public int ID
        {
            get
            {
                return id;
            }
        }
        public bool Isplayer
        {
            get
            {
                return localPlayer.IsPlayer;
            }
        }
        public GObjectList Objectlist
        {
            get
            {
                return ObjectList;
            }
        }
        public void SendConsole(string what, ConsoleLvl cl)
        {
            if (sendtext != null)
            {
                sendtext(new StringStatutTransfertEventArg(what, cl));
            }
        }
        protected void SendConsole(StringStatutTransfertEventArg e)
        {
            if (sendtext != null)
            {
                sendtext(new StringStatutTransfertEventArg("Bot " + (this.id+1).ToString() + ": "+e.Chaine, e.Consolelvl));
            }
        }
        #endregion

        #region Fonctions d'actions
        public void deplacement(ulong Guid, GPlayer who)
        {
            if (Guid != this.Guid)
            {
                move.FollowMain(Guid, DistanceDuMain, who);

            }
        }
        public void breakCurAction()
        {
            this.BreakCurActionB = true;
        }
        public void deplacement(GUnit cible)
        {
            if (cible != null)
            {
                move.MoveToUnit(cible, DistanceDeLaCibleMin);
            }
        }
        public void deplacement(GUnit cible,int distance)
        {
            if (cible != null)
            {
                move.MoveToUnit(cible, distance);
            }
        }
        public void HealTarget()
        {
            HealList.Sort();
            Cible = HealList[0].Target;
            
            HealRoutine(HealList[0].Target);
        }
        public void GotAggro()
        {
            LuaVM.DoString(LuaHelp.GotAggro);
        }
        public void AttackTargets(bool Maintarg)
        {
            //SendConsole("first sort", ConsoleLvl.High);
           
           // showTargetlist();
            //SendConsole("--sort", ConsoleLvl.High);
           
            if (!Maintarg)
            {
                TargetList.Sort();
                
                Cible = TargetList[0].Target;
               
            }
            else
            {
               
                Cible = MainTargetList.Peek().Target;
               
            }
            if (Cible != null)
            {
                Cible.Target();
                deplacement(Cible);
                if (Cible.StartedCombat == false)
                {
                    if (waitTimePreCombat != 0 )
                    {
                        Thread.Sleep(waitTimePreCombat);
                    }
                    PreCombatRoutine();
                    Thread.Sleep(this.PreCombatTick);
                   
                }
                CombatRoutine(Cible);

            }
            else
            {
                SendConsole("No Cible Selected Error", ConsoleLvl.High);
            }
            Cible = null;
            CibleI = 9999;

        }

        public bool isDeadandNoCombat(PrioStruct c)
        {
            return (c.Target.IsDead || !(c.Target.inCombat));
        }
        public bool isDead(PrioStruct c)
        {
            return (c.Target.IsDead);
        }
        public bool FullLife(HealthPrioStruct c)
        {
            return (c.Target.Health == 1.0);
        }
        protected void Repositionnement(GUnit Cible)
        {

            double dist = localPlayer.Location.distanceFrom(Cible.Location);
            move.threadedFace(Cible);
            if (dist > this.DistanceDeLaCibleMax)
            {
                deplacement(Cible);
            }

        }
        public bool deplacement(GLocation Location)
        {
            return move.MoveToLocation(Location);

        }
        protected virtual void CombatRoutine(GUnit Cible)
        {
        }
        protected virtual void HealRoutine(GUnit Cible)
        {
            Cible.Target();

            while (Cible.Health != 1.0)
            {
                if (BreakCurActionB == true)
                {
                    BreakCurActionB = false;
                    break;
                }
                LuaVM.DoString(LuaHelp.HealingTarget);
                Thread.Sleep(200);
            }
        }
        protected  void PreCombatRoutine()
        {
            if (LuaHelp.PreCombat != string.Empty)
            {
                SendConsole("Starting PreCombat " +waitTimePreCombat.ToString(), ConsoleLvl.BotStatut);
                LuaVM.DoString(LuaHelp.PreCombat);

            }
        }
        #endregion
    }


}
