namespace GLib
{
    using Magic;
    using System;
    using System.Threading;
    using MxBots;
    using System.Text;
    public class GObject
    {
        //Offsets
        protected const uint DescriptorOffset = 0x8,
        GuidOffset = 0x30,
        VMT_GetName = 54,
        //Type Offset = OBJECT_FIELD_PADDING=0x5, check om det også er rigtigt i næste patch!
        TypeOffset = 0x5,
        VMT_INTERACT = 44*4;

        /*
        For rotation, x, y, z i use
        X : PBase + 2000 + 0
        Y : PBase + 2000 + 4
        Z : PBase + 2000 + 8
        R : PBase + 2000 + 12
         * */
      
        #region Field
        //References
        public BlackMagic Memory;
        public GObjectList objectList;
        //Uint
        public uint curMgr;
        public uint ObjectPointer;
        public uint Descriptor;
        public uint VirtualMethodTable;
        public ulong ObjectGUID;
        #endregion

        public GObject(GObjectList objectList, uint ObjectPointer)
        {
            this.objectList = objectList;
            curMgr = objectList.getCurMgr();
            Memory = objectList.getMemory();
            this.ObjectPointer = ObjectPointer;
            
            if (ObjectPointer != 0)
            {
                try
                {
                    Descriptor = Memory.ReadUInt(ObjectPointer + DescriptorOffset);
                    ObjectGUID = Memory.ReadUInt64(ObjectPointer + GuidOffset);
                    VirtualMethodTable = Memory.ReadUInt(ObjectPointer);
                }
                catch
                {

                }
            }
        }

        public string tag;
        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }

        /// <summary>
        /// Returns the GUID
        /// </summary>
        public ulong GUID
        {
            get
            {
                return ObjectGUID;
            }
        }

        /// <summary>
        /// Returns the type as unint
        /// </summary>
        public uint Type
        {
            get
            {
                try
                {
                    return Memory.ReadUInt(ObjectPointer + TypeOffset * 4);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the facing in radins
        /// </summary>
        public float Facing
        {
            get
            {
                try
                {
                    return Memory.ReadFloat(ObjectPointer + 0x7A8);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns the X position.
        /// </summary>
        public virtual float XPosition
        {
            get 
            {
                try
                {
                    return Memory.ReadFloat(ObjectPointer + 0x798);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Returns the Y position.
        /// </summary>
        public virtual float YPosition
        {
            get 
            {
                try
                {
                    return Memory.ReadFloat(ObjectPointer + 0x79C);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Returns the Z position.
        /// </summary>
        public virtual float ZPosition
        {
            get 
            {
                try
                {
                    return Memory.ReadFloat(ObjectPointer + 0x7A0);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns the position as GLocation
        /// </summary>
        public virtual GLocation Location
        {
            get
            {
                return new GLocation(XPosition, YPosition, ZPosition);
            }
        }

        /// <summary>
        /// Gets the name of the object
        /// </summary>
        public string Name
        {
            get{
                try
                {
                    uint codecave = Memory.AllocateMemory();
                    uint VMT = Memory.ReadUInt(ObjectPointer);

                    Memory.Asm.Clear();
                    Memory.Asm.AddLine("fs mov eax, [0x2C]");
                    Memory.Asm.AddLine("mov eax, [eax]");
                    Memory.Asm.AddLine("add eax, 0x10");
                    Memory.Asm.AddLine("mov dword [eax], {0}", curMgr);
                    Memory.Asm.AddLine("mov ecx, {0}", ObjectPointer);
                    Memory.Asm.AddLine("call {0}", Memory.ReadUInt(VMT + (54*4))); //read pointer to GetName method
                    Memory.Asm.AddLine("retn");
                    uint pCurName = Memory.Asm.InjectAndExecute(codecave);
                    string curName;

                    if (pCurName != uint.MaxValue)
                        curName = ReadUTF8String(pCurName, 100);
                    else
                        curName = String.Empty;
                   
                    this.Memory.FreeMemory(codecave);
                    return curName;
                }
                catch 
                {
                    return null;
                }
            }
        }

        uint UnitNameoffset1 = 0x964;
        uint UnitNameoffset2 = 0x5c;
        public string Name2
        {
            get
            {
                try
                {
                    uint a = Memory.ReadUInt(this.ObjectPointer + UnitNameoffset1);
                    uint b = Memory.ReadUInt(a + UnitNameoffset2);
                    return ReadUTF8String(b, 0x60); // dont think anything is larger than 60 chars?
                }
                catch (Exception E)
                {
                    return "UNKNOWN";
                }
            }
        }

        public string ReadUTF8String(uint Address, int Length)
        {

            byte[] buffer = Memory.ReadBytes(Address, Length);

            string ret = Encoding.UTF8.GetString(buffer);

            if (ret.IndexOf('\0') != -1)
            {
                ret = ret.Remove(ret.IndexOf('\0'));
            }

            return ret;
        }
        /// <summary>
        /// Interacts with the object, loot, target etc.
        /// </summary>
        public void Interact()
        {
            if (this.ObjectPointer != 0)
            {
                try
                {
                   ThreadManager.suspendMainThread(objectList.getProcessId());

                    uint codecave = this.Memory.AllocateMemory();
                    this.Memory.Asm.Clear();
                    this.Memory.Asm.AddLine("fs mov eax, [0x2C]");
                    this.Memory.Asm.AddLine("mov eax, [eax]");
                    this.Memory.Asm.AddLine("add eax, 8");
                    this.Memory.Asm.AddLine("mov eax, {0}", new object[] { this.VirtualMethodTable });
                    this.Memory.Asm.AddLine("mov ecx, {0}", new object[] { this.ObjectPointer });
                    this.Memory.Asm.AddLine("mov edx, " + (0x005D9A10).ToString("X") + "h");
                    this.Memory.Asm.AddLine("call {0}", new object[] { this.Memory.ReadUInt(this.VirtualMethodTable + VMT_INTERACT) });
                    this.Memory.Asm.AddLine("retn");

                    this.Memory.Asm.InjectAndExecute(codecave);
                   
                    Thread.Sleep(10);
                    //this.Memory.FreeMemory(codecave);

                    ThreadManager.resumeMainThread(objectList.getProcessId());
                    
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Target an player/mob usefull for friendly players and bg peps.
        /// </summary>
        public void Target()
        {
            try
            {


                uint CodeLocation = 0x0725AA0;

                uint codeCave = Memory.AllocateMemory(0x108);

                ThreadManager.suspendMainThread(objectList.getProcessId());

                Memory.WriteUInt64(codeCave + 0x100, GUID);

                Memory.Asm.Clear();
                Memory.Asm.AddLine("MOV EAX,[0x{0}]", (codeCave + 0x100 + 0x4).ToString("X"));
                Memory.Asm.AddLine("PUSH EAX");
                Memory.Asm.AddLine("MOV EAX,[0x{0}]", (codeCave + 0x100 + 0x0).ToString("X"));
                Memory.Asm.AddLine("PUSH EAX");
                Memory.Asm.AddLine("CALL 0x{0}", CodeLocation.ToString("X"));
                Memory.Asm.AddLine("ADD ESP, 0x08");
                Memory.Asm.AddLine("RETN");
                Memory.Asm.InjectAndExecute(codeCave);

                Memory.FreeMemory(codeCave);
                Thread.Sleep(50);
                ThreadManager.resumeMainThread(objectList.getProcessId());

                objectList.DoString("TargetUnit(\"playertarget\")");
            }
            catch { };
        }
        //CGGameUI__Target

    }
}

