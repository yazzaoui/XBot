namespace GLib
{
    using System;
    using System.Threading;

    public class GPlayerSelf : GPlayer
    {
        public GPlayerSelf(GObjectList objectList, uint ObjectPointer)
            : base(objectList, ObjectPointer)
        {

        }

        public int XP
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWPlayerFields.PLAYER_XP, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int ZoneID
        {
            get
            {
                try
                {
                    return Memory.ReadInt(0x00BD080C);
                    //logSystem(Convert.ToString(ObjectList.getMemory().ReadInt(0x0FC1440)));
                    //logSystem(Convert.ToString(ObjectList.getMemory().ReadInt(0x0FDC1BC)));
                    //logSystem(Convert.ToString(ObjectList.getMemory().ReadInt(0x0FF31A4)));
                    //logSystem(Convert.ToString(ObjectList.getMemory().ReadInt(0x010A69A0)));
                    //logSystem(Convert.ToString(ObjectList.getMemory().ReadInt(0x010A6E94)));
                    //logSystem(Convert.ToString(ObjectList.getMemory().ReadInt(0x011EE9F0)));
                    //logSystem(Convert.ToString(ObjectList.getMemory().ReadInt(0x012E2C44)));
                }
                catch {
                    return 999;
                };
            }
        }

        public int XPToLevel
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWPlayerFields.PLAYER_NEXT_LEVEL_XP, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

    }
}

