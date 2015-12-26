using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size=1)]
internal struct eChatLanguage
{
    public static string COMMON;
    public static string DARNASSIAN;
    public static string DWARVEN;
    public static string DRAENEI;
    public static string TAURAHE;
    public static string ORCHISH;
    public static string GUTTERSPEAK;
    public static string DEMONIC;
    public static string DRACONIC;
    public static string KALIMAG;
    public static string TITAN;
    public static string GNOMISH;
    public static string TROLL;
    static eChatLanguage()
    {
        COMMON = "COMMON";
        DARNASSIAN = "DARNASSIAN";
        DWARVEN = "DWARVEN";
        DRAENEI = "DRAENEI";
        TAURAHE = "TAURAHE";
        ORCHISH = "ORCISH";
        GUTTERSPEAK = "GUTTERSPEAK";
        DEMONIC = "DEMONIC";
        DRACONIC = "DRACONIC";
        KALIMAG = "KALIMAG";
        TITAN = "TITAN";
        GNOMISH = "GNOMISH";
        TROLL = "TROLL";
    }
}

