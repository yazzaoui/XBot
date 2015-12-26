using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectsRefresher
{
    public partial class Refresher
    {
        /// <summary>
        /// Event fired whenever a new GameObject is loaded/created.
        /// </summary>
        public event EventHandler<GameObjectEventArgs> GameObjectCreated;
        
        /// <summary>
        /// Event fired whenever a current GameObject is released/destroyed.
        /// </summary>
        public event EventHandler<GameObjectEventArgs> GameObjectReleased;

        internal void OnGameObjectCreated(object sender, GameObjectEventArgs e)
        {
            if (GameObjectCreated != null)
            {
                GameObjectCreated(sender, e);
            }
        }

        internal void OnGameObjectReleased(object sender, GameObjectEventArgs e)
        {
            if (GameObjectReleased != null)
            {
                GameObjectReleased(sender, e);
            }
        }

    }

    public class GameObjectEventArgs : EventArgs
    {
        public UInt64 Guid { get; set; }
    }

    public class GameObjectChangedPositionEventArgs : GameObjectEventArgs
    {
        public float OldValue { get; set; }
        public float NewValue { get; set; }
    }
}
