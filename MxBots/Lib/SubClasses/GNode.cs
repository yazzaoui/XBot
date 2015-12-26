using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLib
{
     public class GNode : GObject
    {
        ///<summary>
        ///This class contains info on nodes etc
        ///</summary>

        public GNode(GObjectList objectList, uint ObjectPointer)
            : base(objectList, ObjectPointer)
        {
        }

        public string Print()
        {
            return "Name: " + Name + " X: " + XPosition.ToString() + " Y: " + YPosition.ToString() + " Z: " + ZPosition.ToString();
        }

        //public override float XPosition
        //{
        //    get 
        //    {
        //        try
        //        {
        //            return objectList.getDescriptors.GetKnownField<float>(Descriptors.WoWGameObjectFields.GAMEOBJECT_POS_X, ObjectPointer);
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
        //            return objectList.getDescriptors.GetKnownField<float>(Descriptors.WoWGameObjectFields.GAMEOBJECT_POS_Y, ObjectPointer);
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
        //            return objectList.getDescriptors.GetKnownField<float>(Descriptors.WoWGameObjectFields.GAMEOBJECT_POS_Z, ObjectPointer);
        //        }
        //        catch
        //        {
        //            return 0;
        //        }
        //    }
        //}

        //public void face()
        //{
        //    GMovement move = new GMovement(objectList);
        //    move.facePOS(Location);
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

        public uint DisplayId
        {
            get 
            {
                try
                {
                    return objectList.getDescriptors.GetKnownField<uint>(Descriptors.WoWGameObjectFields.GAMEOBJECT_DISPLAYID, ObjectPointer);
                }
                catch
                {
                    return 0;
                }
            }
        }

    }
}
