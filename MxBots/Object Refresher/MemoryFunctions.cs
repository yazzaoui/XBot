using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Magic;
using GLib;

namespace ObjectsRefresher
{
    internal class MemoryFunctions
    {

        const string PatternClientConnection = "EB 02 33 C0 8B D 00 00 00 00 64 8B 15 00 00 00 00 8B 34 8A 8B D 00 00 00 00 89 81 00 00 00 00";
        const string MaskClientConnection = "xxxxxx????xxx????xxxxx????xx????";
        //Offsets end 
        uint g_clientConnection, clientConnection, curMgr, curMgrOffset;
        uint dwCodeLoc;
        int ProcessIdOfWoW;

        protected const uint FirstObject = 0xac,
        GuidOffset = 0x30,
        NextObject = 0x3C;

        private BlackMagic Memory = new BlackMagic();

        public MemoryFunctions(int ProcessIdOfWoW)
        {
            this.ProcessIdOfWoW = ProcessIdOfWoW;
            //Lets Open wow for manipulation
            Memory.OpenProcessAndThread(ProcessIdOfWoW);
            //Lets find the Pattern - Shynd
            dwCodeLoc = SPattern.FindPattern(Memory.ProcessHandle, Memory.MainModule, PatternClientConnection, MaskClientConnection, ' ');
            //Lets find the s_curMgr
            try
            {
                g_clientConnection = Memory.ReadUInt(dwCodeLoc + 0x16);
                clientConnection = Memory.ReadUInt(g_clientConnection);
                curMgrOffset = Memory.ReadUInt(dwCodeLoc + 0x1C); //Lets find the CurMgr Offset
                curMgr = Memory.ReadUInt(clientConnection + curMgrOffset); //clientConnection + CurMgrOffest = Win
            }
            catch
            {
            }
        }

        public uint GetFirstObject()
        {
            try
            {
                return Memory.ReadUInt(curMgr + FirstObject);
            }
            catch
            {
                return 0;
            }
        }

        public uint GetNextObject(uint curObject)
        {
            try
            {
            return Memory.ReadUInt(curObject + 0x3C);
                                        }
            catch
            {
                return 0;
            }
        }

        public UInt64 GetObjectGuid(uint curObject)
        {
            try
            {
            return Memory.ReadUInt64(curObject + GuidOffset);
            }
            catch
            {
                return 0;
            }
        }
    }
}
