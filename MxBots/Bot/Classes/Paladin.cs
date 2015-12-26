using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using MxBots.Bots.Actions;
using GLib;
using Magic;
using LuaInterface;
using System.Reflection;
using System.Windows.Forms;
using MxBots.Properties;

namespace MxBots.Bots
{
   public class Paladin : Bot
    {

       public Paladin(int i)
            : base(i, Classes.Paladin)
        {


            Dps = false;
            Tank = true;
            Healer = true;

        }


       protected override void CombatRoutine(GUnit Cible)
       {

           while (Cible.IsAlive && localPlayer.IsAlive)
           {
               if (BreakCurActionB == true)
               {
                   BreakCurActionB = false;
                   break;
               }
               LuaVM.DoString(LuaHelp.AttackingTarget);

               Repositionnement(Cible);
               if (!Cible.inCombat || !localPlayer.inCombat)
               {
                   Cible.Target();
                   Thread.Sleep(100);

               }
               Thread.Sleep(200);

               TargetList.Sort();
           }

       }
    }
}
