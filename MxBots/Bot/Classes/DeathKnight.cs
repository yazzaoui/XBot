using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLib;
using System.Threading;
namespace MxBots.Bots
{
   public class DeathKnight : Bot
    {
     
       public DeathKnight(int i)
            : base(i, Classes.DK)
        {

            Dps = false;
            Tank = true;
            Healer = false;


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
