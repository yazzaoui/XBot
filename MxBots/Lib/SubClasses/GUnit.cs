namespace GLib
{
    using System;
    using System.Threading;

    public class GUnit : GObject
    {
        public GUnit(GObjectList objectList, uint ObjectPointer)
            : base(objectList, ObjectPointer)
        {
            TargetingSomeTank = false;
            if (inCombat)
            {
                StartedCombat = true;
                StartCombat = DateTime.Now;
            }
            else
            {
                StartedCombat = false;
            }
        }

        public bool TargetFriendly(GContext context, GPlayerSelf me,KeyHelper keyhelper)
        {
            GSpellTimer timer = new GSpellTimer(60 * 1000);
            keyhelper.SendKey("Common.TargetFriend");
            Thread.Sleep(1000);
            while (!me.TargetGUID.Equals(GUID) && !timer.isReady)
            {
                keyhelper.SendKey("Common.TargetFriend");
                Thread.Sleep(1000);
            }
            if (me.TargetGUID.Equals(GUID))
                return true;
            else
                return false;
        }

        public DateTime StartCombat{get;set;}
        public bool StartedCombat { get; set; }
        public bool TargetingSomeTank { get; set; }
        public bool TargetEnemy(GContext context, GPlayerSelf me, KeyHelper keyhelper)
        {
            GSpellTimer timer = new GSpellTimer(60 * 1000);
            keyhelper.SendKey("Common.TargetEnemy");
            Thread.Sleep(1000);
            while (!me.TargetGUID.Equals(GUID) && !timer.isReady)
            {
                keyhelper.SendKey("Common.TargetEnemy");
                Thread.Sleep(1000);
            }
            if (me.TargetGUID.Equals(GUID))
                return true;
            else
                return false;
        }

        /// <summary>
        /// GUID of the unit/player who created this unit (good for totems) 
        /// </summary>
        public ulong CreatedBy
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<ulong>(Descriptors.WoWUnitFields.UNIT_FIELD_CREATEDBY, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns Energy
        /// </summary>
        public int Energy
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWUnitFields.UNIT_FIELD_POWER4, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns max Energy
        /// </summary>
        public int EnergyMax
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWUnitFields.UNIT_FIELD_MAXPOWER4, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Faces the unit
        /// </summary>
        public void Face()
        {
            //GMovement move = new GMovement(objectList);
            //move.facePOS(Location);
        }

        /// <summary>
        /// Faction template id of this unit  
        /// </summary>
        public long Faction
        {
            get 
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<long>(Descriptors.WoWUnitFields.UNIT_FIELD_FACTIONTEMPLATE, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Percentage of health this unit has 
        /// </summary>
        public double Health
        {
            get
            {
                return Math.Round((((double)this.HealthPoints) / ((double)this.HealthMax)), 2);
            }
        }

        /// <summary>
        /// Actual health this unit has 
        /// </summary>
        public int HealthPoints
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWUnitFields.UNIT_FIELD_HEALTH, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Max health this unit has 
        /// </summary>
        public int HealthMax
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWUnitFields.UNIT_FIELD_MAXHEALTH, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Returns true if the UNIT is alive 
        /// </summary>
        /// <remarks>
        /// To do: Make a better function (Use flags instead?)
        /// </remarks>
        public bool IsAlive
        {
            get
            {
                return (this.HealthPoints > 0);
            }
        }
        /// <summary>
        /// True if this unit is dead 
        /// </summary>
        /// <remarks>
        /// To do: Make a better function (Use flags instead?)
        /// </remarks>
        public bool IsDead
        {
            get
            {
                return (this.HealthPoints <= 0 || this.HealthPoints == 0.01 || this.Health == 0 || this.Health == 0.01);
            }
        }
        /// <summary>
        /// True if this unit is targeting me. 
        /// </summary>
        public bool IsTargetingMe 
        {
            get
            {
                if (objectList.LocalPlayer.GUID.Equals(TargetGUID))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// True if I this is a player 
        /// </summary>
        public bool IsPlayer
        {
            get
            {
                if (base.Type.Equals((int)eObjectType.G_PLAYER))
                    return true;
                else return false;
            }
        }

        /// <summary>
        /// True if I this is a unit 
        /// </summary>
        public bool IsUnit
        {
            get
            {
                if (base.Type.Equals((int)eObjectType.G_UNIT))
                    return true;
                else return false;
            }
        }

        /// <summary>
        /// True if I have a pet and this unit is targeting it. 
        /// </summary>
        public bool IsTargetingMyPet
        {
            get
            {
                if (objectList.LocalPlayer.HasLivePet)
                {
                    if (objectList.LocalPlayer.PetGUID.Equals(TargetGUID))
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Game level of this unit 
        /// </summary>
        public int Level
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWUnitFields.UNIT_FIELD_LEVEL, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Percentage of mana this unit has 
        /// </summary
        public double Mana
        {
            get
            {
                if (ManaPoints != 0)
                    return Math.Round((((double)this.ManaPoints) / ((double)this.ManaMax)), 2);
                else
                    return 0;
            }
        }
        /// <summary>
        /// Max mana this unit has 
        /// </summary
        public int ManaMax
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWUnitFields.UNIT_FIELD_MAXPOWER1, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Actual mana this unit has 
        /// </summary
        public int ManaPoints
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWUnitFields.UNIT_FIELD_POWER1, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Returns Rage
        /// </summary>
        public int Rage
        {
            get
            {
                try
                {
                    int RageTemp = objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWUnitFields.UNIT_FIELD_POWER2, ObjectPointer);
                    return (int)(Math.Floor((double)(RageTemp / 10)));
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Returns the GUID the current object is summond by
        /// </summary>
        public ulong SummonedBy
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<ulong>(Descriptors.WoWUnitFields.UNIT_FIELD_SUMMONEDBY, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Returns the GUID of the current target. 
        /// </summary>
        public ulong TargetGUID
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<ulong>(Descriptors.WoWUnitFields.UNIT_FIELD_TARGET, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// True if this unit is lootable right now. 
        /// </summary>
        public bool IsLootable {
            get
            {
                try
                {
                    if (getDynFlags == 13 || getDynFlags == 1)
                        return true;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// True if this monster has been tagged by another player
        /// </summary>
        public bool IsTagged
        {
            get
            {
                if (getDynFlags == 4)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// True if this monster has been tagged by you
        /// </summary>
        public bool IsTaggedByYou
        {
            get
            {
                if (getDynFlags == 8 || getDynFlags == 13 || getDynFlags == 1 || getDynFlags == 12)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Returns the current flags of this unit
        /// </summary>
        /// <remarks>
        /// The flags can be seen as:
        /// 01: has loot
        /// 02: ???
        /// 04: locked to a player
        /// 08: locked to you
        /// If the function returns 13 (01 * 04 * 08) the mob is locked to you and is lootable.
        /// If it is returns 12 (04 & 08) you are either still fighting it, or you have already looted it.
        /// </remarks>
        public uint getDynFlags
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<uint>(Descriptors.WoWUnitFields.UNIT_DYNAMIC_FLAGS, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }

        }

        //Todo works?
        public uint getDisplayID
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<uint>(Descriptors.WoWUnitFields.UNIT_FIELD_DISPLAYID, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                objectList.getDescriptors.setKnownField(Descriptors.WoWUnitFields.UNIT_FIELD_DISPLAYID, ObjectPointer, value);
            }
        }

        //Todo works?
        public uint getCharmedBy
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<uint>(Descriptors.WoWUnitFields.UNIT_FIELD_CHARMEDBY, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns true if the unit is in combat
        /// </summary>
        public bool inCombat
        {
            get
            {
                return Convert.ToBoolean(Flags & 0x0080000);
            }
        }

        /// <summary>
        /// Returns true if auto attack in enabled
        /// </summary>
        public bool autoAttack
        {
            get
            {
                return Convert.ToBoolean(Flags & 0x0000800);
            }
        }


        /// <summary>
        /// Returns true if the unit is skinnable
        /// </summary>
        public bool Skinnable
        {
            get
            {
                return Convert.ToBoolean(Flags & 0x4000000);
            }
        }
        /// <summary>
        /// Returns the current unit field flag.
        /// </summary>
        /// <value>
        ///8: PVP Enabled
        ///10: totem?!
        ///40: elite? 
        ///800: fighing
        ///1000: in pvp
        ///8000: ???
        ///40000: immobile (player dead / stunned = C0000)
        ///80000:  in melee
        ///4000000: skinnable
        ///20000000: dazed
        /// </value>
        public long Flags
        {
            get
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<long>(Descriptors.WoWUnitFields.UNIT_FIELD_FLAGS, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public GUnit GetTarget
        {
            get
            {
                GUnit temp = new GUnit(objectList, objectList.getObjectByGUID(TargetGUID));
                return temp;
            }
        }

        #region Old Mouse Code
        ///// <summary>
        ///// Hover the mouse over this object. 
        ///// </summary>
        ///// <returns>
        ///// True if we managed to put the mouse on the object, False if not
        ///// </returns>
        //public bool Hover()
        //{
        //    return(Mouse.Hover(XPosition, YPosition, ZPosition, objectList));
        //}

        ///// <summary>
        ///// Click with the mouse
        ///// </summary>
        ///// <param name="rightClick">
        ///// Send True to right Click
        ///// </param>
        //public void DoMouseClick(bool rightClick)
        //{
        //    Mouse.DoMouseClick(rightClick);
        //}


        ///// <summary>
        ///// Right-click on this object. 
        ///// </summary>
        ///// <returns>
        ///// True if we positioned the mouse and clicked, False if not
        ///// </returns>
        //public bool Interact()
        //{
        //    if(Mouse.Interact(XPosition, YPosition, ZPosition, objectList))
        //    {
        //        if (objectList.LocalPlayer.TargetGUID != this.ObjectGUID)
        //            return false;
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// Left-click on this object. 
        ///// </summary>
        ///// <returns>
        ///// True if we positioned the mouse and clicked, False if not
        ///// </returns>
        //public bool Select()
        //{
        //    if (Mouse.Interact(XPosition, YPosition, ZPosition, objectList))
        //    {
        //        if (objectList.LocalPlayer.TargetGUID != this.ObjectGUID)
        //            return false;
        //        return true;
        //    }
        //    return false;
        //}
        #endregion
    }
}

