namespace GLib
{
    using System;

    public class GPlayer : GUnit
    {

        public GPlayer(GObjectList objectList, uint ObjectPointer)
            : base(objectList, ObjectPointer)
        {

        }
        /// <summary>
        /// Returns PlayerClass
        /// </summary>
        /// <remarks>
        /// Does not work yet!
        /// </remarks>
        public int PlayerClass
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns true if the player is flaged for PvP
        /// </summary>
        public bool PVP
        {
            get
            {
                return Convert.ToBoolean(base.Flags & 0x0000008);
            }
        }

        /// <summary>
        /// Returns true if the player is in pvp combat
        /// </summary>
        public bool InPVP
        {
            get
            {
                return Convert.ToBoolean(base.Flags & 0x0001000);
            }
        }

        /// <summary>
        /// Returns Player race
        /// </summary>
        public string PlayerRace
        {
            get
            {
                long faction = base.Faction;
                if (faction.Equals((long)ePlayerFactions.Human))
                    return "Human";
                else if (faction.Equals((long)ePlayerFactions.BloodElf))
                    return "Blood Elf";
                else if (faction.Equals((long)ePlayerFactions.Dwarf))
                    return "Dwarf";
                else if (faction.Equals((long)ePlayerFactions.Gnome))
                    return "Gnome";
                else if (faction.Equals((long)ePlayerFactions.NightElf))
                    return "Night Elf";
                else if (faction.Equals((long)ePlayerFactions.Orc))
                    return "Orc";
                else if (faction.Equals((long)ePlayerFactions.Tauren))
                    return "Tauren";
                else if (faction.Equals((long)ePlayerFactions.Troll))
                    return "Troll";
                else if (faction.Equals((long)ePlayerFactions.Undead))
                    return "Undead";
                else if (faction.Equals((long)ePlayerFactions.Draenei))
                    return "Draenei";
                else return "Unknown";
            }
        }

        /// <summary>
        /// Returns faction group (Alliance || Horde)
        /// </summary>
        public string PlayerFaction
        {
            get
            {
                switch (PlayerRace)
                {
                    case "Human":
                    case "Dwarf":
                    case "Gnome":
                    case "Night Elf":
                    case "Draenei":
                        return "Alliance";
                    case "Orc":
                    case "Undead":
                    case "Tauren":
                    case "Troll":
                    case "Blood Elf":
                        return "Horde";
                }
                return "Unknown";
            }
        }

        /// <summary>
        ///  Returns the GUID of our pet
        /// </summary>
        /// <remarks>
        /// Does not look at non combat pets!
        /// </remarks>
        public virtual ulong PetGUID
        {
            get
            {
                if (HasLivePet)
                {
                    return Pet.ObjectGUID;
                }
                return 0;
            }
        }
        /// <summary>
        ///  Returns true if one of the objects is summond by the player 
        /// </summary>
        /// <remarks>
        /// Does not look at non combat pets!
        /// </remarks>
        public bool HasLivePet
        {
            get
            {
                if (Pet != null)
                    return true;
                return false;
            }
        }

        /// <summary>
        ///  Returns the localplayer pet
        /// </summary>
        /// <remarks>
        /// Does not return non combat pets!
        /// </remarks>
        public GUnit Pet
        {
            get
            {
                int counter = 0;
                GUnit unit;
                while (objectList.GuidList.Count > counter)
                {
                    if (objectList.ObjectList[objectList.GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                    {
                        unit = new GUnit(objectList, objectList.ObjectList[objectList.GuidList[counter]].ObjectPointer);

                        if (unit.SummonedBy.Equals(base.ObjectGUID))
                        {
                            return unit;
                        }
                    }
                    counter++;
                }
                return null;
            }
        }
    }
}

