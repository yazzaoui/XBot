using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magic;
using MxBots;

namespace GLib
{
    class CanRead
    {
        BlackMagic tempMem = new BlackMagic();
        uint LocalGuidOffset = 0xc0;
        //Uint
        private uint localPlayerObject,
        Holder,
        TempHolder,
        GuidOffset = 0x30,
        XPositionOffset = 0x7D0,
        FirstObject = 0xac,
        NextObject = 0x3C;
        Descriptors getDescriptors;
        uint g_clientConnection, clientConnection, curMgr, curMgrOffset;
        uint dwCodeLoc;
        const string PatternClientConnection = "EB 02 33 C0 8B D 00 00 00 00 64 8B 15 00 00 00 00 8B 34 8A 8B D 00 00 00 00 89 81 00 00 00 00";
        const string MaskClientConnection = "xxxxxx????xxx????xxxxx????xx????";

        public bool tryAttach(int dwProcessId)
        {
            try
            {
                //Lets Open wow for manipulation
                tempMem.OpenProcessAndThread(dwProcessId);
                //Lets find the Pattern - Shynd
                dwCodeLoc = SPattern.FindPattern(tempMem.ProcessHandle, tempMem.MainModule, PatternClientConnection, MaskClientConnection, ' ');
                //Lets find the s_curMgr
                g_clientConnection = tempMem.ReadUInt(dwCodeLoc + 0x16);
                clientConnection = tempMem.ReadUInt(g_clientConnection);
                curMgrOffset = tempMem.ReadUInt(dwCodeLoc + 0x1C); //Lets find the CurMgr Offset
                curMgr = tempMem.ReadUInt(clientConnection + curMgrOffset); //clientConnection + CurMgrOffest = Win
                getDescriptors = new Descriptors(tempMem);
                localPlayerObject = getObjectByGUID(tempMem.ReadUInt64(curMgr + LocalGuidOffset));

                if (XP != -10)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        public int XP
        {
            get
            {
                try
                {
                    return getDescriptors.GetKnownField<int>(Descriptors.WoWPlayerFields.PLAYER_XP, localPlayerObject);
                }
                catch
                {
                    return -10;
                }
            }
        }

        /// <summary>
        ///Return the Object of an GUID
        /// </summary>
        public uint getObjectByGUID(ulong GUID)
        {
            try
            {
                Holder = getFirstObject();
                TempHolder = Holder;
                while ((Holder != 0) && ((Holder & 1) == 0))
                {
                    if (tempMem.ReadUInt64(Holder + GuidOffset) == GUID)
                    {
                        return Holder;
                    }
                    TempHolder = this.getNextObject(Holder);
                    if ((TempHolder == 0) || (TempHolder == Holder))
                    {
                        break;
                    }
                    Holder = TempHolder;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        //---------- Private functions ------------
        private uint getFirstObject()
        {
            try
            {
                return tempMem.ReadUInt(curMgr + FirstObject);
            }
            catch
            {
                return 0;
            }
        }

        private uint getNextObject(uint CurrentObject)
        {
            try
            {
                return tempMem.ReadUInt(CurrentObject + NextObject);
            }
            catch
            {
                return 0;
            }
        }
    }
}
