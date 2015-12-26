namespace GLib
{
    using Magic;
    using System;
    using System.Collections.Generic;
    using ObjectsRefresher;
    using System.Windows.Forms;
    using System.Threading;
    using MxBots;

    public class GObjectList
    {

        #region Field
        //Offsets
        protected const uint FirstObject = 0xac,
        GuidOffset = 0x30,
        NextObject = 0x3C,
        LocalGuidOffset = 0xc0,
        VMT_GetName = 52;
        
        //References
        public  BlackMagic Memory ;
        private GPlayerSelf localPlayer;
        private Refresher objectRefresher;
        private  System.Text.Encoding utf_8 ;
        public event StringStatutTransfertEventHandler sendtext;

        //Uint
        private uint localPlayerObject,
        
        Holder,
        TempHolder,
        curObject,
        curObj,
        nextObj;
        //IDictionary + GuidList
        public  Descriptors Descriptor;
        public  IDictionary<ulong, GItem> GItemList = new Dictionary<ulong, GItem>(); //Type 1
        public  IDictionary<ulong, GUnit> GUnitList = new Dictionary<ulong, GUnit>(); //Type 3
        public  IDictionary<ulong, GPlayer> GPlayerList = new Dictionary<ulong, GPlayer>(); //Type 4
        public  IDictionary<ulong, GNode> GNodeList = new Dictionary<ulong, GNode>(); //Type 5
        public  IDictionary<ulong, GDynamic> GDynamicList = new Dictionary<ulong, GDynamic>(); //Type 6
        public  IDictionary<ulong, GCorpses> GCorpsesList = new Dictionary<ulong, GCorpses>(); //Type 7
        public  IDictionary<ulong, GObject> ObjectList = new Dictionary<ulong, GObject>(); //Default
        public  List<ulong> GuidList = new List<ulong>();
        //List
        List<uint> list = new List<uint>();
        List<uint> listAllObjects = new List<uint>();
        List<GUnit> returnList = new List<GUnit>();

        const string PatternClientConnection = "EB 02 33 C0 8B D 00 00 00 00 64 8B 15 00 00 00 00 8B 34 8A 8B D 00 00 00 00 89 81 00 00 00 00";
        const string MaskClientConnection = "xxxxxx????xxx????xxxxx????xx????";
        //Offsets end 
        uint g_clientConnection, clientConnection, curMgr, curMgrOffset;
        uint dwCodeLoc;
        int ProcessIdOfWoW;
        //GMain form;

        //Other
        ulong LocalGUIDVariable;
        public float MouseX,MouseY,MouseZ;
        private uint codeCave;

        #endregion

        public GObjectList(int ProcessIdOfWoW)
        {
            bool didit = false;
            this.ProcessIdOfWoW = ProcessIdOfWoW;
            while (didit == false)
            {
                    Memory = new BlackMagic();
               
                    //this.form = form;
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

                        LocalGUIDVariable = Memory.ReadUInt64(curMgr + LocalGuidOffset);
                        localPlayerObject = getObjectByGUID(LocalGUIDVariable);
                        localPlayer = new GPlayerSelf(this, localPlayerObject);

               
                    }
                    catch
                    {

                    }
               
                Descriptor = new Descriptors(Memory);
                //Lets start the object refresher
                objectRefresher = new Refresher(ProcessIdOfWoW);
                objectRefresher.GameObjectCreated += new EventHandler<GameObjectEventArgs>(addToLists);
                objectRefresher.GameObjectReleased += new EventHandler<GameObjectEventArgs>(removeFromLists);
                

                    didit = true;
                

            }
        }

        public void endObjectRefresher()
        {
            try
            {
                objectRefresher.GameObjectCreated -= new EventHandler<GameObjectEventArgs>(addToLists);
                objectRefresher.GameObjectReleased -= new EventHandler<GameObjectEventArgs>(removeFromLists);
                objectRefresher.close();
            }
            catch
            {

            }
        }

        public Descriptors getDescriptors
        {
            get
            {
                return Descriptor;
            }
        }

        public int getProcessId()
        {
            return this.ProcessIdOfWoW;
        }
        public UInt32 getCurMgr()
        {
            return this.curMgr;
        }

        public BlackMagic getMemory()
        {
            return Memory;
        }

        //public GMain Form
        //{
        //    get
        //    {
        //        return form;
        //    }
        //}

        #region List stuff
        private void addToLists(object sender, GameObjectEventArgs e)
        {
            GObject ObjectToAdd = PopulateObject(e.Guid);
            if (ObjectToAdd != null)
            {
                try
                {
                    if (ObjectToAdd.Type.Equals((int)eObjectType.G_ITEM))
                        GItemList.Add(ObjectToAdd.ObjectGUID, new GItem(this, ObjectToAdd.ObjectPointer));
                    if (ObjectToAdd.Type.Equals((int)eObjectType.G_UNIT))
                        GUnitList.Add(ObjectToAdd.ObjectGUID, new GUnit(this, ObjectToAdd.ObjectPointer));
                    if (ObjectToAdd.Type.Equals((int)eObjectType.G_PLAYER))
                        GPlayerList.Add(ObjectToAdd.ObjectGUID, new GPlayer(this, ObjectToAdd.ObjectPointer));
                    if (ObjectToAdd.Type.Equals((int)eObjectType.G_NODE))
                        GNodeList.Add(ObjectToAdd.ObjectGUID, new GNode(this, ObjectToAdd.ObjectPointer));
                    if (ObjectToAdd.Type.Equals((int)eObjectType.G_DYNAMICOBJECT))
                        GDynamicList.Add(ObjectToAdd.ObjectGUID, new GDynamic(this, ObjectToAdd.ObjectPointer));
                    if (ObjectToAdd.Type.Equals((int)eObjectType.G_CORSPE))
                        GCorpsesList.Add(ObjectToAdd.ObjectGUID, new GCorpses(this, ObjectToAdd.ObjectPointer));
                    ObjectList.Add(ObjectToAdd.ObjectGUID, new GObject(this, ObjectToAdd.ObjectPointer));
                    GuidList.Add(e.Guid);
                }
                catch { };
            }

        }

        private void removeFromLists(object sender, GameObjectEventArgs e)
        {
            GItemList.Remove(e.Guid);
            GUnitList.Remove(e.Guid);
            GPlayerList.Remove(e.Guid);
            GNodeList.Remove(e.Guid);
            GDynamicList.Remove(e.Guid);
            GCorpsesList.Remove(e.Guid);
            ObjectList.Remove(e.Guid);
            GuidList.Remove(e.Guid);
        }

        public GObject findBgNPC(string name)
        {
            try
            {
                curObj = getFirstObject();
                nextObj = curObj;
                while (curObj != 0 && (curObj & 1) == 0)
                {
                    GObject temp = new GObject(this, curObj);

                    if (temp.Name.Equals(name))
                    {
                        return temp;
                    }
                    nextObj = this.getNextObject(curObj);
                    if (nextObj == curObj)
                        break;
                    else
                        curObj = nextObj;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// This function is used to Populate the lists.
        /// </summary>    
        /// <remarks>
        /// When framework is done this should (hopefully) be the only class that uses 
        /// curObj and nextObj besides the refresh framework
        /// </remarks>
        private GObject PopulateObject(ulong GUID)
        {
            try
            {
                curObj = getFirstObject();
                nextObj = curObj;
                while (curObj != 0 && (curObj & 1) == 0)
                {
                    if (Memory.ReadUInt64(curObj + GuidOffset) == GUID)
                    {
                        return new GObject(this, curObj);
                    }

                    nextObj = this.getNextObject(curObj);
                    if (nextObj == curObj)
                        break;
                    else
                        curObj = nextObj;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
#endregion

        /// <summary>
        /// Returns the LocalPlayer Target
        /// </summary>
        public GUnit LocalPlayerTarget
        {
            get
            {
                if (LocalPlayer.TargetGUID != 0)
                {
                    return new GUnit(this, getObjectByGUID(LocalPlayer.TargetGUID));
                }
                return null;
            }
        }
                public BlackMagic memoire
        {
            get
            {
                return Memory;
            }
        }

        /// <summary>
        /// Returns the LocalPlayer GUID
        /// </summary>
        public ulong LocalGUID
        {
            get { return LocalGUIDVariable; }
        }

        /// <summary>
        ///  Returns the GPlayerSelf
        /// </summary>
        public GPlayerSelf LocalPlayer
        {
            get { return localPlayer; } 
        }

        /// <summary>
        /// Checks to see if you are being attacked - returns bool 
        /// </summary>
        public bool CheckForAttackers
        {
            get
            {
                List<GUnit> attackers = GetAttackers;
                if (attackers.Count != 0)
                    return true;
                else
                    return false;
            }
        }


        /// <summary>
        /// Given a GUID, find the identified object (any type) in the game. 
        /// </summary>
        public GObject FindObject(ulong GUID)
        {
            int counter = 0;
            while (GuidList.Count > counter)
            {
                if (ObjectList[GuidList[counter]].ObjectGUID.Equals(GUID))
                {
                    return ObjectList[GuidList[counter]];
                }
                counter++;
            }
            return null;
        }

        /// <summary>
        /// Given a GUID, find the identified unit (monster or player) in the game. 
        /// </summary>
        public GUnit FindUnit(ulong GUID)
        {
            int counter = 0;
            while (GuidList.Count > counter)
            {
                if (ObjectList[GuidList[counter]].ObjectGUID.Equals(GUID))
                {
                    if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                    {
                        if (GUnitList.ContainsKey(GuidList[counter]))
                            return GUnitList[GuidList[counter]];
                    }
                    else if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_PLAYER))
                    {
                        if (GPlayerList.ContainsKey(GuidList[counter]))
                            return GPlayerList[GuidList[counter]];
                    }
                }
                counter++;
            }
            return null;
        }

        /// <summary>
        /// Given a display name, find the identified unit (monster or player) in the game.
        /// </summary>
        public GUnit FindUnit(String DisplayName)
        {
            Holder = this.getFirstObject();
            try
            {
                while ((Holder != 0) && ((Holder & 1) == 0))
                {
                    if (getObjectName(Holder).Contains(DisplayName))
                    {
                        return new GUnit(this, Holder);
                    }
                    TempHolder = this.getNextObject(Holder);
                    if ((TempHolder == 0) || (TempHolder == Holder))
                    {
                        break;
                    }
                    Holder = TempHolder;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return null;
        }

        public List<GUnit> getUnitsInRange(int range)
        {
            int counter = 0;
            List<GUnit> toReturn = new List<GUnit>();
            GUnit unit;
            GLocation location = localPlayer.Location;
            while (GuidList.Count > counter)
            {
                if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                {
                    unit = new GUnit(this, ObjectList[GuidList[counter]].ObjectPointer);
                    if (location.distanceFrom(unit.Location) < range && !unit.IsDead)
                    {
                        toReturn.Add(unit);
                    }
                }
                counter++;
            }
            if (toReturn .Count != 0)
                return toReturn;
            return null;
        }

        /// <summary>
        /// Returns a list with all players
        /// </summary>
        public List<GUnit> GetPlayers()
        {
           int counter = 0;
           List<GUnit> toReturn = new List<GUnit>();
           while (GuidList.Count > counter)
           {
               if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_PLAYER))
               {
                    toReturn.Add(GPlayerList[GuidList[counter]]);
               }
               counter++;
           }
           return toReturn;
        }

        /// <summary>
        /// Find the closest unit that is targeting the specified GUID. 
        /// </summary
        public GUnit FindUnitByTarget(ulong TargetGUID)
        {
            int counter = 0;
            double closestUnit = 9999999;
            GLocation location = localPlayer.Location;
            GUnit unit;
            GUnit returnUnit = null;
            while (GuidList.Count > counter)
            {
                if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                {
                    unit = new GUnit(this, ObjectList[GuidList[counter]].ObjectPointer);
                    if (unit.TargetGUID.Equals(TargetGUID))
                    {
                        if (location.distanceFrom(unit.Location) < closestUnit)
                        {
                            returnUnit = unit;
                            closestUnit = location.distanceFrom(unit.Location);
                        }
                    }
                }
                counter++;
            }
            if (returnUnit != null)
                return returnUnit;
            return null;
        }

        /// <summary>
        /// Find the closest unit that is targeting the specified GUID: Exclude the the second GUID. 
        /// </summary>
        public GUnit FindUnitByTarget(ulong TargetGUID, ulong ExcludeGUID)
        {
            int counter = 0;
            double closestUnit = 9999999;
            GLocation location = localPlayer.Location;
            GUnit unit;
            GUnit returnUnit = null;
            while (GuidList.Count > counter)
            {
                if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                {
                    unit = new GUnit(this, ObjectList[GuidList[counter]].ObjectPointer);
                    if (unit.TargetGUID.Equals(TargetGUID) || unit.TargetGUID != ExcludeGUID)
                    {
                        if (location.distanceFrom(unit.Location) < closestUnit)
                        {
                            returnUnit = unit;
                            closestUnit = location.distanceFrom(unit.Location);
                        }
                    }
                }
                counter++;
            }
            if (returnUnit != null)
                return returnUnit;
            return null;
        }


        /// <summary>
        ///Returns the closest hostile player
        /// </summary>
        public GUnit GetClosestPlayer
        {
            get
            {
                int counter = 0;
                double closestUnit = 9999999;
                GLocation location = localPlayer.Location;
                GUnit unit;
                GUnit returnUnit = null;
                while (GuidList.Count > counter)
                {
                    if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_PLAYER))
                    {
                        unit = new GUnit(this, ObjectList[GuidList[counter]].ObjectPointer);
                        if (location.distanceFrom(unit.Location) < closestUnit)
                        {
                            returnUnit = unit;
                            closestUnit = location.distanceFrom(unit.Location);
                        }          
                    }
                    counter++;
                }
                if (returnUnit != null)
                    return returnUnit;
                return null;
            }
        }

        public GUnit GetNearestHostile()
        {
            int counter = 0;
            double closestUnit = 9999999;
            GLocation location = localPlayer.Location;
            GUnit unit;
            GUnit returnUnit = null;
            while (GuidList.Count > counter)
            {
                if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                {
                    unit = new GUnit(this, ObjectList[GuidList[counter]].ObjectPointer);
                    if (location.distanceFrom(unit.Location) < closestUnit && !unit.IsTagged)
                    {
                        returnUnit = unit;
                        closestUnit = location.distanceFrom(unit.Location);
                    }
                }
                counter++;
            }
            if (returnUnit != null)
                return returnUnit;
            return null;
        }


        public GUnit GetNearestHostile(GLocation Location, ulong ExcludeGUID)
        {
            int counter = 0;
            double closestUnit = 9999999;
            GLocation location = Location;
            GUnit unit;
            GUnit returnUnit = null;
            while (GuidList.Count > counter)
            {
                if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                {
                    unit = new GUnit(this, ObjectList[GuidList[counter]].ObjectPointer);
                    if (location.distanceFrom(unit.Location) < closestUnit && !unit.IsTagged && unit.GUID != ExcludeGUID)
                    {
                        returnUnit = unit;
                        closestUnit = location.distanceFrom(unit.Location);
                    }
                }
                counter++;
            }
            if (returnUnit != null)
                return returnUnit;
            return null;
        }

        /// <summary>
        ///Returns the closest GUnit attacking you or you pet
        /// </summary>
        public GUnit GetClosestAttacker
        {
            get
            {
                int counter = 0;
                double closestUnit = 9999999;
                GLocation location = localPlayer.Location;
                GUnit unit;
                GUnit returnUnit = null;
                while (GuidList.Count > counter)
                {
                    if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                    {
                        unit = new GUnit(this, ObjectList[GuidList[counter]].ObjectPointer);
                        if (localPlayer.HasLivePet)
                        {
                            if (unit.TargetGUID.Equals(LocalGUIDVariable) || unit.TargetGUID.Equals(localPlayer.Pet.ObjectGUID))
                            {
                                if (location.distanceFrom(unit.Location) < closestUnit)
                                {
                                    returnUnit = unit;
                                    closestUnit = location.distanceFrom(unit.Location);
                                }
                            }
                        }
                        else
                        {
                            if (unit.TargetGUID.Equals(LocalGUIDVariable))
                            {
                                if (location.distanceFrom(unit.Location) < closestUnit)
                                {
                                    returnUnit = unit;
                                    closestUnit = location.distanceFrom(unit.Location);
                                  }
                            }
                        }
                    }
                    counter++;
                }
                if (returnUnit != null)
                    return returnUnit;
                return null;
            }
        }
        uint TrucVert1 = 0xD5F984;
        uint TrucVert2 = 0xD5F988;
        public void SetMouseCoordFlag()
        {
            //spour la mouse vite fais avec cheat engine fo enleve et le remettre


            Memory.WriteInt(TrucVert1,64);
            Thread.Sleep(20);
            Memory.WriteInt(TrucVert2,438906840);
            //spour le graph , retrouver avec ida grace a SetChatWindowName
            Thread.Sleep(20);
            Memory.WriteByte(0x049C8B3, 0);
            //Memory.WriteInt(0x0049c743, 0);
            MxBots.Hook.cliked += new MxBots.Hook.MouseTransfertEvent(getcoordmousse);
            
        }
        private void findpatterns()
        {
           
            
        }
        private void getcoordmousse(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseX = Memory.ReadFloat(0xb0d558);
                MouseZ = Memory.ReadFloat(0xB0d560);
                MouseY = Memory.ReadFloat(0xB0d55C);
                MxBots.Hook.cliked -= new MxBots.Hook.MouseTransfertEvent(getcoordmousse);
                Thread bizarlethread = new Thread(new ThreadStart(this.bizzarerie));
                bizarlethread.Start();
            }
            else if (e.Button == MouseButtons.Right)
            {
                Memory.WriteInt(TrucVert1, 0);
                Thread.Sleep(20);
                Memory.WriteInt(TrucVert2, 0);
                MxBots.Hook.cliked -= new MxBots.Hook.MouseTransfertEvent(getcoordmousse);
            }
        }
        private void bizzarerie()
        {
            byte x=3;
            for (int i = 0; i < 6; i++)
            {
                x = (byte)(3 - x);
                Memory.WriteByte(0x04A025c, x);
                Thread.Sleep(150);
            }

            Thread.CurrentThread.Abort();

        }

        /// <summary>
        ///Returns a list of all units (monsters and players) that are targeting you or your pet.
        /// </summary>
        public List<GUnit> GetAttackers
        {
            get
            {
                int counter = 0;
                GUnit unit;
                bool alreadyThere = false;
                returnList.Clear();
                while (GuidList.Count > counter)
                {
                    try
                    {
                        if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                        {
                            unit = new GUnit(this, ObjectList[GuidList[counter]].ObjectPointer);
                            if (localPlayer.HasLivePet)
                            {
                                if (unit.TargetGUID.Equals(LocalGUIDVariable) || unit.TargetGUID.Equals(localPlayer.Pet.ObjectGUID) && !unit.IsDead)
                                {
                                    foreach (GUnit un in returnList)
                                    {
                                        if (un.ObjectGUID.Equals(unit.ObjectGUID))
                                            alreadyThere = true;
                                    }
                                    if (!alreadyThere)
                                        returnList.Add(unit);
                                    alreadyThere = false;
                                }
                            }
                            else
                            {
                                if (unit.TargetGUID.Equals(LocalGUIDVariable) && !unit.IsDead)
                                {
                                    foreach (GUnit un in returnList)
                                    {
                                        if (un.ObjectGUID.Equals(unit.ObjectGUID))
                                            alreadyThere = true;
                                    }
                                    if (!alreadyThere)
                                        returnList.Add(unit);
                                    alreadyThere = false;
                                }
                            }
                        }
                    }
                    catch (
                        Exception e
                        ) { };
                    counter++;
                }
                return returnList;
            }
        }

        //---------- Private functions ------------
        private uint getFirstObject()
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

        private uint getNextObject(uint CurrentObject)
        {
            try
            {
                return Memory.ReadUInt(CurrentObject + NextObject);
            }
            catch
            {
                return 0;
            }
        }
        //----------End Private functions ------------ 

        /// <summary>
        ///Get all monsters in the game that we can see. 
        /// </summary>
        public List<GMonster> GetMonsters()
        {
            return null;
        }

        /// <summary>
        ///Returns the GUID of an object
        /// </summary>
        public ulong getGUIDByObject(uint Object)
        {
            try
            {
                return Memory.ReadUInt64(Object + GuidOffset);
            }
            catch
            {
                return 0;
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
                    if (Memory.ReadUInt64(Holder + GuidOffset) == GUID)
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

        /// <summary>
        ///Returns a list with Objects if they matches the search string
        /// </summary>
        public List<uint> getObjectsByString(string zString)
        {
            list.Clear();
            curObject = this.getFirstObject();
            TempHolder = curObject;
            try
            {
                while ((curObject != 0) && ((curObject & 1) == 0))
                {
                    if (getObjectName(curObject).Contains(zString))
                    {
                        list.Add(curObject);
                    }
                    TempHolder = this.getNextObject(curObject);
                    if ((TempHolder == 0) || (TempHolder == curObject))
                    {
                        return list;
                    }
                    curObject = TempHolder;
                }
                return list;
            }
            catch 
            {
                return null;
            }
        }

        //Returning Object name if you supply the object
        //To do: Redo this function to use offsets based on object Types instead of injection!
        public string getObjectName(uint curObject)
        {
            if (curObject == 0)
            {
                return "Not a valid object";
            }

            try
            {
                uint codecave = Memory.AllocateMemory();
                uint VMT = Memory.ReadUInt(curObject);

                Memory.Asm.Clear();
                Memory.Asm.AddLine("fs mov eax, [0x2C]");
                Memory.Asm.AddLine("mov eax, [eax]");
                Memory.Asm.AddLine("add eax, 8");
                Memory.Asm.AddLine("mov dword [eax], {0}", curMgr);
                Memory.Asm.AddLine("mov ecx, {0}", curObject);
                Memory.Asm.AddLine("call {0}", Memory.ReadUInt(VMT + VMT_GetName)); //read pointer to GetName method
                Memory.Asm.AddLine("retn");

                uint pCurName = Memory.Asm.InjectAndExecute(codecave);
                string curName;

                if (pCurName != uint.MaxValue)
                    curName = Memory.ReadASCIIString(pCurName, 100);
                else
                    curName = String.Empty;
                return curName;
            }
            catch 
            {
                return null;
            }
        }
        public void UpdateModel()
        {


  
                uint codecave = Memory.AllocateMemory();
              

                Memory.Asm.Clear();
                Memory.Asm.AddLine("push 1");
                Memory.Asm.AddLine("call {0}", 0x0063F640); //read pointer to GetName method
                Memory.Asm.AddLine("retn");

                Memory.Asm.InjectAndExecute(codecave);


        }

        public void DoString(string lua)
        {
            Thread.Sleep(75);
            utf_8 = System.Text.Encoding.UTF8;
            lua += '\0';
            // Convert a string to utf-8 bytes.
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(lua);
           


            Memory.WriteBytes(codeCave + 0x1024,utf8Bytes);
           // Memory.WriteASCIIString(codeCave + 0x1024, lua+'\0');
           // Memory.WriteUnicodeString(codeCave + 0x1024, lua + '\0');
            Memory.WriteInt(codeCave + 256, 0xBABABA);
            

        }
       
        public void HookEndScene()
        {
            ThreadManager.suspendMainThread(this.getProcessId());
            uint pDevice = Memory.ReadUInt(0x00BB672C);
            uint pEnd = Memory.ReadUInt(pDevice + 0x397C);
            uint pScene = Memory.ReadUInt(pEnd);
            uint pEndScene = Memory.ReadUInt(pScene + 0xA8);

            SendConsole("EndScene Offset : " + pEndScene.ToString("X"),ConsoleLvl.Debug);
            if (Memory.ReadByte(pEndScene) != 0xe9) // check if not already hooked
            {
                codeCave = Memory.AllocateMemory(0x2048);
                Memory.Asm.Clear();
                //Demerdation de laddresse de endscene mon amour :)))



                byte[] Backup = Memory.ReadBytes(pEndScene, 25);

                int size = Memory.Asm.GetMemorySize();
                Memory.Asm.AddLine("pushad");
                Memory.Asm.AddLine("pushfd");

                Memory.Asm.AddLine("mov esi, " + (codeCave + 256).ToString("X") + "h");
                Memory.Asm.AddLine("cmp dword [esi], 0");
                Memory.Asm.AddLine("je " + (codeCave + 0x1D).ToString("X") + "h");
                //DO STRING
                Memory.Asm.AddLine("push {0}", 0);
                Memory.Asm.AddLine("mov eax, {0}", codeCave + 0x1024);
                Memory.Asm.AddLine("push eax");
                Memory.Asm.AddLine("push eax");
                Memory.Asm.AddLine("call {0}", (uint)0x004B32B0);
                Memory.Asm.AddLine("add esp, 0xC");

                //EXIT
                Memory.Asm.AddLine("mov dword[" + (codeCave + 256).ToString("X") + "h], 0");
                Memory.Asm.AddLine("popfd");
                Memory.Asm.AddLine("popad");

                Memory.Asm.Inject(codeCave);
                Memory.WriteBytes(codeCave + 0x29, Backup);

                Memory.Asm.Clear();
                Memory.Asm.AddLine("jmp " + (pEndScene + 25).ToString("X") + "h");

                //REMPLACEMENT POUR NOBUG


                Memory.Asm.Inject(codeCave + 0x29 + 25);


                // Okay on a le pointeur , que les choses serieuses commencent : YOUMEW EN MODE EXTRA BOUISSINCE 
                Memory.Asm.Clear();
                Memory.Asm.AddLine("jmp " + codeCave.ToString("X") + "h");
                Memory.Asm.Inject(pEndScene);
            }
            else
            {
                codeCave= Memory.ReadUInt(pEndScene + 1) + 4 + pEndScene - 0xffffffff;
                
            }
            ThreadManager.resumeMainThread(this.getProcessId());
            // ENDSCENE IS NOW HOOKED 
            // HOOK BY LMEW
            // LA BOUISINCE A LETAT PURE
           
        }
        public void SendConsole(string what, ConsoleLvl cl)
        {
            if (sendtext != null)
            {
                sendtext(new StringStatutTransfertEventArg(what, cl));
            }
        }

    }
}

