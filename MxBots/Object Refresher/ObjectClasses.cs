using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectsRefresher
{
    public class GameObject
    {
        /// <summary>
        /// Get the memory address for this GameObject.
        /// </summary>
        public uint Address { get; set; }

        /// <summary>
        /// Get the GUID for this GameObject.
        /// </summary>
        public UInt64 Guid { get; set; }
    }
}
