using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size=1)]
internal struct eChatChannel
{
    public static string SAY;
    public static string EMOTE;
    public static string YELL;
    public static string PARTY;
    public static string GUID;
    public static string OFFICER;
    public static string RAID;
    public static string RAIDWARNING;
    public static string BATTLEGROUND;
    public static string WHISPER;
    public static string CHANNEL;
    public static string AFK;
    public static string DND;
    static eChatChannel()
    {
        SAY = "SAY";
        EMOTE = "EMOTE";
        YELL = "YELL";
        PARTY = "PARTY";
        GUID = "GUILD";
        OFFICER = "OFFICER";
        RAID = "RAID";
        RAIDWARNING = "RAID_WARNING";
        BATTLEGROUND = "BATTLEGROUND";
        WHISPER = "WHISPER";
        CHANNEL = "CHANNEL";
        AFK = "AFK";
        DND = "DND";
    }
}

