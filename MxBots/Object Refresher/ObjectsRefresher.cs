using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ObjectsRefresher
{
    public partial class Refresher
    {
        /// <summary>
        /// Gives you access to values of all GameObjects currently around you.
        /// The key is the GUID of the GameObject.
        /// </summary>
        public Dictionary<UInt64, GameObject> GameObjects
        { 
            get
            {
                return _gameObjects;
            }
        }

        /// <summary>
        /// The time in milliseconds to refresh GameObjects.
        /// Default is 100ms.
        /// </summary>
        public int RefreshTime = 100;
        
        private Dictionary<UInt64, GameObject> _gameObjects { get; set; }

        private MemoryFunctions wMem;
        Thread mainLoopThread;
        /// <summary>
        /// Create a new instance of the WowNetFramework to begin accessing values and subscribing to events.
        /// </summary>
        /// <param name="wowProcessId">The process ID of the World of Warcraft instance you would like to access.</param>
        public Refresher(int wowProcessId)
        {
            wMem = new MemoryFunctions(wowProcessId);
            mainLoopThread = new Thread(new ThreadStart(this.MainLoop));
            mainLoopThread.Start();
        }

        public void close()
        {
            if(mainLoopThread != null)
                mainLoopThread.Abort();
        }

        private void MainLoop()
        {
            // Define two lists, first is the previous list of GameObjects
            // the second is a list of the new GameObjects
            // We can then compare the two to find out what was added, removed, changed and what
            // has changed specifically.
            Dictionary<UInt64, GameObject> oldGameObjects = new Dictionary<UInt64, GameObject>();

            // May as well declare here, instead of redeclaring every loop.
            uint curObject, nextObject = 0;

            while (true)
            {
                // A new objects list, we're getting a fresh check of all GameObjects now
                Dictionary<UInt64, GameObject> newGameObjects = new Dictionary<UInt64, GameObject>();

                curObject = wMem.GetFirstObject();
                nextObject = curObject;

                // Run until the current object is no longer valid (ie. end of the list, or can no longer read the memory)
                while (curObject != 0 && (curObject & 1) == 0)
                {
                    GameObject newGameObject = new GameObject();
                    newGameObject.Address = curObject;
                    newGameObject.Guid = wMem.GetObjectGuid(curObject);

                    try
                    {
                        newGameObjects.Add(newGameObject.Guid, newGameObject);
                    }
                    catch
                    {

                    }
                    // Check if we're at the end of the list
                    if ((nextObject = wMem.GetNextObject(curObject)) == curObject)
                    {
                        break;
                    }
                    else
                    {
                        curObject = nextObject;
                    }
                }

                // Allow the front-end to read the new list of GameObjects now..
                this._gameObjects = newGameObjects;

                // Do the comparison
                this.CompareGameObjects(oldGameObjects, newGameObjects);

                // Now that we're done comparing, this is suddenly stale :p
                oldGameObjects = newGameObjects;

                // Check if we can no longer read anything..
                if (curObject == uint.MaxValue)
                {
                    Thread.CurrentThread.Abort();
                }

                // Wait..
                Thread.Sleep(this.RefreshTime);
            }
        }

        private void CompareGameObjects(Dictionary<UInt64, GameObject> oldGameObjects, Dictionary<UInt64, GameObject> newGameObjects)
        {
            // Find any removed GameObjects
            IEnumerable<UInt64> removedGameObjects = oldGameObjects.Keys.Except(newGameObjects.Keys);
            foreach (UInt64 removedGameObject in removedGameObjects)
            {
                this.OnGameObjectReleased(this, new GameObjectEventArgs { Guid = removedGameObject });
            }

            // Find any new GameObjects
            IEnumerable<UInt64> createdGameObjects = newGameObjects.Keys.Except(oldGameObjects.Keys);
            foreach (UInt64 createdGameObject in createdGameObjects)
            {
                this.OnGameObjectCreated(this, new GameObjectEventArgs { Guid = createdGameObject });
            }
        }
    }
}
