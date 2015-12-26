using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;

namespace MxBots
{
   public static class SomeSettings
    {
       private static string productVersion = "0.75";

       private static string lastEdited = "20/2/10";
       
    //   public static bool debug = false;
       public static int botnumber;
       public static string[] bots;
       public static bool bot_firstuse = false;
       public static bool working = false;
       public static bool consoleHiden = false;
       public static bool showballon = true;
       public static HotKeyManager hkm;
       public static BotMode BotMod;
       public static XmlNodeList Faction;
       public static ArrayList UserHotkey;
       public static string NavFile="";
       public static NavMesh NavMesh;
       public static string ProductVersion
       {
           get
           {    
               return productVersion;

           }
       }

       public static string getBotModstring(BotMode c)
       {
           switch (c)
           {
               case BotMode.Classic:
                   return "Classic";
               case BotMode.Multibot:
                   return "MultiBot";
               case BotMode.Semi:
                   return "Semi-Automated";
               case BotMode.Xplayback:
                   return "XPlayBack Mod";
               case BotMode.BG:
                   return "BG Farming";
               case BotMode.Instance:
                   return "Instance Farming";
               default:
                   return "None";
           }
       }
    }
}
