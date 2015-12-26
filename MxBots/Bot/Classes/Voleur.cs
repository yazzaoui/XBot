using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GLib;
namespace MxBots.Bots
{
   public class Voleur : Bot
    {

       public Voleur(int i)
            : base(i, Classes.Voleur)
        {


            Dps = true;
            Tank = false;
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
