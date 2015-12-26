using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GLib
{
   class GProfile
    {
        private List<GLocation> Waypoints = new List<GLocation>();
        private List<GLocation> returnList = new List<GLocation>();
        private List<Double> factions = new List<Double>();
        private int LevelMinVar;
        private int LevelMaxVar;
        private double distance;
        private int placeInList;
        private int i, h;
        private GContext main;
        private bool groupProfile = false;
        private XmlDocument doc;
        private string NextStep;
        private string filename;
        private string mount;
        private string condition;
        private string parameter;
        private string ChangeToIfClose;
        private string Event;
        private string Distance;
        private GLocation PauseAtWaypoint;
        private int HowLong;

        public GProfile(GContext main)
        {
            this.main = main;
        }

        public GProfile(GContext main, string NextStep, string mount, string condition, string parameter, string ChangeToIfClose, string Event, string Distance)
        {
            this.mount = mount;
            this.condition = condition;
            this.parameter = parameter;
            this.NextStep = NextStep;
            this.main = main;
            this.ChangeToIfClose = ChangeToIfClose;
            this.Event = Event;
            this.Distance = Distance;
        }

        public void loadFile(string fileName)
        {
            filename = fileName;
            doc = new XmlDocument();
            doc.Load(fileName);

            XmlElement elm = doc.DocumentElement;

            if(elm.Name.Equals("ProfileGroup"))
            {
                groupProfile = true;
            } else {
 
                    //Load min and max level
                    XmlNodeList MinLevel = doc.GetElementsByTagName("MinLevel");
                    LevelMinVar = Convert.ToInt32(MinLevel[0].ChildNodes[0].Value);
                    XmlNodeList MaxLevel = doc.GetElementsByTagName("MaxLevel");
                    LevelMaxVar = Convert.ToInt32(MaxLevel[0].ChildNodes[0].Value);

                    //Load factions
                    XmlNodeList Faction = doc.GetElementsByTagName("Factions");

                    foreach (XmlNode faction in Faction)
                    {
                        string temp = faction.ChildNodes[0].Value;

                        string[] split = temp.Split(new Char[] { ' ' });

                        foreach (string f in split)
                        {
                            factions.Add(Convert.ToDouble(f));
                        }
                    }

                    //Load PauseAtWaypoint
                    XmlNodeList PauseAt = doc.GetElementsByTagName("PauseAtWaypoint");

                    foreach (XmlNode at in PauseAt)
                    {
                        string temp = at.ChildNodes[0].Value;
                        string CorrectString = temp.Replace(".", ",");

                        string[] split = CorrectString.Split(new Char[] { ' ' });
                        if (!split.Count().Equals(1))
                        {
                            PauseAtWaypoint = new GLocation((float)Convert.ToDouble(split[0]), (float)Convert.ToDouble(split[1]), (float)Convert.ToDouble(split[2]));
                        }
                        else
                        {
                            PauseAtWaypoint = new GLocation(0, 0, 0);
                        }
                    }
                    //Load How Long
                    XmlNodeList Long = doc.GetElementsByTagName("HowLong");
                    HowLong = Convert.ToInt32(Long[0].ChildNodes[0].Value);



                    //Load waypoints
                    XmlNodeList Waypoint = doc.GetElementsByTagName("Waypoint");
                    int i = -1;
                    foreach (XmlNode point in Waypoint)
                    {
                        i++;
                        string temp = point.ChildNodes[0].Value;
                        string CorrectString = temp.Replace(".", ",");

                        string xyz = CorrectString;
                        string[] split = xyz.Split(new Char[] { ' ' });
                        if (split.Length > 2)
                        {
                            GLocation wayPointPos = new GLocation((float)Convert.ToDouble((split[0])), (float)Convert.ToDouble((split[1])), (float)Convert.ToDouble((split[2])));
                            Waypoints.Add(wayPointPos);
                        }
                        else
                        {
                            GLocation wayPointPos = new GLocation((float)Convert.ToDouble((split[0])), (float)Convert.ToDouble((split[1])), (float)Convert.ToDouble((0)));
                            Waypoints.Add(wayPointPos);
                        }
                    }
                    //lastWaypoint 
            }
        }

        public GLocation getPauseAtWaypoint
        {
            get
            {
                return PauseAtWaypoint;
            }
        }

        public int getHowLong
        {
            get
            {
                return HowLong;
            }
        }

        public int getDistance
        {
            get
            {
                return Convert.ToInt32(Distance);
            }
        }

        public string getEvent
        {
            get
            {
                return Event;
            }
        }

        public bool getChangeToIfClose
        {
            get
            {
                return Convert.ToBoolean(ChangeToIfClose);
            }
        }

        public bool getMount
        {
            get
            {
                return Convert.ToBoolean(mount);
            }
        }

        public int getCondition
        {
            get
            {
                return Convert.ToInt32(condition);
            }
        }

        public int getParameter
        {
            get
            {
                return Convert.ToInt32(parameter);
            }
        }

        public int getNextStep
        {
            get 
            {
                return Convert.ToInt32(NextStep);
            }
        }

        public XmlDocument getDoc
        {
            get
            {
                return doc;
            }
        }

        public List<GLocation> waypoints
        {
            get
            {
                return Waypoints;
            }
        }

        public int LevelMin
        {
            get 
            {
                return LevelMinVar;
            }
        }

        public int LevelMax
        {
            get
            {
                return LevelMaxVar;
            }
        }

        public List<Double> Factions
        {
            get
            {
                return factions;
            }
        }

        public void FactionsAdd(int faction)
        {
            factions.Add(faction);
        }

        public void saveFile(string filename)
        {
        }

        public void addSingleWayPoint(GLocation position)
        {

        }

        public void startRecordWayPoints(GPlayerSelf localPlayer)
        {
            //To do make a new thread that records the waypoints
        }

        public void stopRecordWayPoints(GPlayerSelf localPlayer)
        {
            //To do terminate the thread that records waypoints
        }

        public void clarWaypoints()
        {
            Waypoints.Clear();
        }

        /// <summary>
        /// Returns true if the profile loaded is a group profile
        /// </summary>
        public bool isGroup
        {
            get
            {
                return groupProfile;
            }
        }

        /// <summary>
        /// Returns the distance to the closest waypoint
        /// </summary>
        public double GetDistanceTo(GLocation location)
        {
            GLocation wayPointPos = Waypoints[0];
            distance = location.distanceFrom(wayPointPos);

            i = 0;
            while (Waypoints.Count > i)
            {
                wayPointPos = Waypoints[i];
                if (location.distanceFrom(wayPointPos) < distance)
                    distance = location.distanceFrom(wayPointPos);
                i++;
            }
            return Math.Round(distance, 2);
        }

        public string profile
        {
            get
            {
                return filename;
            }
        }
        /// <summary>
        /// Get the waypoint closest to the location
        /// </summary>
        public GLocation getClosestWaypoint(GLocation location)
        {
            if (Waypoints.Count == 0)
                throw new Exception("No waypoints loaded");
            GLocation closestPos = Waypoints[0];
            distance = location.distanceFrom(closestPos);
            i = 0;
            while (Waypoints.Count > i)
            {
                GLocation wayPointPos = Waypoints[i];

                if (location.distanceFrom(wayPointPos) < distance)
                {
                    distance = location.distanceFrom(wayPointPos);
                    closestPos = wayPointPos;
                }             
                i++;
            }
            return closestPos;
        }

        public List<GLocation> reverseWaypoint
        {
            get
            {
                List<GLocation> temp = new List<GLocation>();
                temp = Waypoints;
                temp.Reverse();
                return temp;
            }
        }

        /// <summary>
        /// Returns a list with Waypoints sorted after distance to the location
        /// </summary>
        public List<GLocation> getListSortedAfterDistance(GLocation location)
        {
            if (Waypoints.Count == 0)
                return null;

            returnList = new List<GLocation>();

            GLocation closestPosition = getClosestWaypoint(location);
            i = 0;
            while (Waypoints.Count > i)
            {
                GLocation wayPointPos = Waypoints[i];

                if (closestPosition.Equals(wayPointPos))
                {
                    placeInList = i;
                }
                i++;
            }

            i = 0;
            h = placeInList;
            while (Waypoints.Count > placeInList && h < Waypoints.Count)
            {
                returnList.Add(Waypoints[h]);
                h++;
            }
            while (i < placeInList)
            {
                returnList.Add(Waypoints[i]);
                i++;
            }
            return returnList;
        }
    }
}
