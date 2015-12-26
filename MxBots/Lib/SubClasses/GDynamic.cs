using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLib
{
   public class GDynamic : GObject
    {
        public GDynamic(GObjectList objectList, uint ObjectPointer)
            : base(objectList, ObjectPointer)
        {
        }
    }
}
