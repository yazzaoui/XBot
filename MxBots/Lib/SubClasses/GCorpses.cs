using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLib
{
     public class GCorpses : GObject
    {

        public GCorpses(GObjectList objectList, uint ObjectPointer)
            : base(objectList, ObjectPointer)
        {
        }

        //public override float XPosition
        //{
        //    get 
        //    {
        //        try
        //        {
        //            return objectList.getDescriptors.GetKnownField<float>(Descriptors.WoWCorpseFields.CORPSE_FIELD_POS_X, ObjectPointer);
        //        }
        //        catch
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public override float YPosition
        //{
        //    get 
        //    {
        //        try
        //        {
        //            return objectList.getDescriptors.GetKnownField<float>(Descriptors.WoWCorpseFields.CORPSE_FIELD_POS_Y, ObjectPointer);
        //        }
        //        catch
        //        {
        //            return 0;
        //        }
        //    }
        //}
        //public override float ZPosition
        //{
        //    get 
        //    {
        //        try
        //        {
        //            return objectList.getDescriptors.GetKnownField<float>(Descriptors.WoWCorpseFields.CORPSE_FIELD_POS_Z, ObjectPointer);
        //        }
        //        catch
        //        {
        //            return 0;
        //        }
        //    }
        //}


        //public override GLocation Location
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (ObjectPointer == 0)
        //            {
        //                return new GLocation(0f, 0f, 0f);
        //            }
        //            return new GLocation(XPosition, YPosition, ZPosition);
        //        }
        //        catch
        //        {
        //            return new GLocation(0f, 0f, 0f);
        //        }
        //    }
        //}

    }
}
