using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLib;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace MxBots
{
    public enum Classes
    {
        Guerrier,
        Démoniste,
        Mage,
        Paladin,
        Voleur,
        Druide,
        Shaman,
        Prêtre,
        DK,
        Chasseur,
        None
    }
    public enum BotAction
    {
        Attack,
        Heal,
        Rez,
        GotAggro,
        FollowMain,
        GoToLocation,
        Protecting,
        AttackMain,
        Patrol
    }
    public enum ConsoleLvl
    {
        Low,
        Medium,
        High,
        Debug,
        System,
        Error,
        BotStatut,
        BotHub
    }
    public enum BotMode
    {
        Classic,
        Semi,
        Multibot,
        Xplayback,
        BG,
        Instance
    }
    public enum formType
    {
        Combobox,
        CheckBox
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    [Serializable]
    public struct BotConfig
    {
         public  int FollowDistance;

       public int minDistance;
        public int maxDistance;
    }

    public struct StringConsole
    {
        public string chaine;
        public ConsoleLvl consolelvl;


    }
    public class SAction : IComparable
    {

        public BotAction Action;
        public int Priority;
        public GLocation Location;
        public ulong Guid;
        int IComparable.CompareTo(object obj)
        {

            SAction c2 = (SAction)obj;
            if (this.Priority > c2.Priority)
                return -1;
            if (this.Priority < c2.Priority)
                return 1;
            else
                return 0;

        }
        public SAction(BotAction ba, int p, GLocation g)
        {
            Action = ba;
            Priority = p;
            Location = g;
        }
        public SAction(BotAction ba, int p, ulong g)
        {
            Action = ba;
            Priority = p;
            Guid = g;
        }
        public SAction(BotAction ba, int p)
        {
            Action = ba;
            Priority = p;
         
        }
        public SAction(BotAction ba)
        {
            Action = ba;
      

        }
        public SAction()
        {


        }


    }

}

