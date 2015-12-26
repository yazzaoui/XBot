using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLib;
using System.Threading;
namespace MxBots.Bots
{
   public class Shaman : Bot
    {

       public Shaman(int i)
            : base(i, Classes.Shaman)
        {

            Dps = true;
            Tank = false;
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
