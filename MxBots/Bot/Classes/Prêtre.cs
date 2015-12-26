using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLib;
using System.Threading;
namespace MxBots.Bots
{
   public class Prêtre : Bot
    {

       public Prêtre(int i)
            : base(i, Classes.Prêtre)
        {

   

            Dps = false;
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
       protected override void HealRoutine(GUnit Cible)
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
    }
}
