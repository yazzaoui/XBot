namespace GLib
{
    using System;
    using System.Collections.Generic;

    public class GItem : GObject
    {
        public GItem(GObjectList objectList, uint ObjectPointer)
            : base(objectList, ObjectPointer)
        {
        }

        public ulong getContained()
        {
            try
            {
                return objectList.getDescriptors.GetKnownField<ulong>(Descriptors.WoWItemFields.ITEM_FIELD_CONTAINED, ObjectPointer);
            }
            catch
            {
                return 0;
            }
        }

        public int GetDurability()
        {
            try
            {
                return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWItemFields.ITEM_FIELD_DURABILITY, ObjectPointer);
            }
            catch
            {
                return 0;
            }
        }

        public float GetDurabilityPercentage()
        {
            try
            {
                return (((float)this.GetDurability()) / ((float)this.GetMaxDurability()));
            }
            catch
            {
                return 0;
            }
        }

        public int GetMaxDurability()
        {
            try
            {
                return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWItemFields.ITEM_FIELD_MAXDURABILITY, ObjectPointer);
            }
            catch
            {
                return 0;
            }
        }

        public int GetStackCount()
        {
            try
            {
                return objectList.getDescriptors.GetKnownField<int>(Descriptors.WoWItemFields.ITEM_FIELD_STACK_COUNT, ObjectPointer);
            }
            catch
            {
                return 0;
            }
        }
    }
}

