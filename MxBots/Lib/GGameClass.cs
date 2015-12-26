using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using MxBots;

namespace GLib
{
    //This class handels the combat
    class GGameClass
    {

        bool killAll = false;
       
        #region Field
        public GContext GContext;
        public GPlayerSelf Me;
        public GObjectList ObjectList;
        public GMovement move;
        private List<GLocation> Waypoints;
        private GLocation changeToIfCloseWaypoint;
        private List<GUnit> lootList = new List<GUnit>();
        public GProfile profile;
        private KeyHelper keyhelper;
        private List<GProfile> groupProfiles;
        private List<GProfile> ChangeToIfClose;
        private List<GProfile> ChangeToOnEvent;
        GSpellTimer lootTimer = new GSpellTimer(50 * 1000);
        bool groupP = false;
        int i;
        Thread waypoint;

        //------------ If we are doing AB lets check the events ------------
        bool BSAlliance = false;
        bool LMAlliance = false;
        bool GMAlliance = false;
        bool FarmAlliance = false;
        bool STAlliance = false;

        bool BSHorde = false;
        bool LMHorde = false;
        bool GMHorde = false;
        bool FarmHorde = false;
        bool STHorde = false;

        bool Green = false, Purple = false, Blue = false, Red = false, Yellow = false;
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Constuctor
        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="objectList">
        /// Reference to the objectList
        /// </param>
        /// <param name="log">
        /// Reference to the context class
        /// </param>
        public GGameClass(GObjectList ObjectList, GContext context,KeyHelper Keyhelper ,Bot bot )
        {
            this.GContext = context;
            this.keyhelper = Keyhelper;
            if (ObjectList != null)
            {
                Me = ObjectList.LocalPlayer;
                move = new GMovement(ObjectList,bot);
                this.ObjectList = ObjectList;
                profile = new GProfile(context);
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Profile stuff
        private void populateGroupProfile(GProfile profile)
        {
            groupProfiles = new List<GProfile>();
            logSystem("Loading group profile");

            string executableName = Application.ExecutablePath;
            FileInfo executableFileInfo = new FileInfo(executableName);
            string executableDirectoryName = executableFileInfo.DirectoryName;

            XmlDocument doc = profile.getDoc;
            XmlElement elm = doc.DocumentElement;
            //try
            //{
                XmlNodeList rootChild = elm.ChildNodes;
                XmlNodeList StepChild = rootChild[0].ChildNodes;
                foreach (XmlNode nod in StepChild)
                {
                    XmlNodeList stepChildren = nod.ChildNodes;
                    GProfile temp = new GProfile(GContext, stepChildren[1].InnerText, stepChildren[2].InnerText, stepChildren[3].InnerText, stepChildren[4].InnerText, stepChildren[5].InnerText, stepChildren[6].InnerText, stepChildren[7].InnerText);
                    temp.loadFile(executableDirectoryName + "\\" + stepChildren[0].InnerText);
                    groupProfiles.Add(temp);
                }
            //}
            //catch (Exception e)
            //{
            //    logSystem("Error in profile group: " + e.Message);
            //}
        }

        public bool CheckProfileQuick()
        {
            profile.loadFile(GContext.getProfileToLoad);
            if (profile.isGroup)
            {
                groupP = true;

                populateGroupProfile(profile);

                double clostestway = groupProfiles[0].GetDistanceTo(Me.Location);
                profile = groupProfiles[0];
                foreach (GProfile pro in groupProfiles)
                {
                    if (pro.GetDistanceTo(Me.Location) < clostestway)
                    {
                        clostestway = pro.GetDistanceTo(Me.Location);
                        profile = pro;
                    }
                }
                ChangeToIfClose = new List<GProfile>();
                foreach (GProfile pro in groupProfiles)
                {
                    if (pro.getChangeToIfClose)
                    {
                        ChangeToIfClose.Add(pro);
                    }
                }
                ChangeToOnEvent = new List<GProfile>();
                foreach (GProfile pro in groupProfiles)
                {
                    if (!pro.getEvent.Equals("None"))
                    {
                        ChangeToOnEvent.Add(pro);
                    }
                }
            }
            return true;
        }

        public bool CheckProfile()
        {
            profile.loadFile(GContext.getProfileToLoad);
            if (profile.isGroup)
            {
                groupP = true;
                
                populateGroupProfile(profile);

                double clostestway = groupProfiles[0].GetDistanceTo(Me.Location);
                profile = groupProfiles[0];
                foreach (GProfile pro in groupProfiles)
                {
                    if (pro.GetDistanceTo(Me.Location) < clostestway)
                    {
                        clostestway = pro.GetDistanceTo(Me.Location);
                        profile = pro;
                    }   
                }
                ChangeToIfClose = new List<GProfile>();
                foreach (GProfile pro in groupProfiles)
                {
                    if (pro.getChangeToIfClose)
                    {
                        ChangeToIfClose.Add(pro);
                    }
                }
                ChangeToOnEvent = new List<GProfile>();
                foreach (GProfile pro in groupProfiles)
                {
                    if (!pro.getEvent.Equals("None"))
                    {
                        ChangeToOnEvent.Add(pro);
                    }
                }
            }
            if (profile.GetDistanceTo(Me.Location) > 40)
            {
                logSystem("Move " + profile.GetDistanceTo(Me.Location) + "yards in current direction");
                move.facePOS(profile.getClosestWaypoint(Me.Location));
                return false;
            }
            if (groupP)
            {
                logSystem("Closest: " + profile.profile);
                logSystem("Next profile: " + groupProfiles[profile.getNextStep].profile);
            }
            return true;
        }

        private bool reloadProfileAfterDead()
        {
            profile.loadFile(GContext.getProfileToLoad);
            groupP = true;
            
            populateGroupProfile(profile);

            logSystem("Checking closest profile");
            double clostestway = groupProfiles[0].GetDistanceTo(Me.Location);
            profile = groupProfiles[0];
            foreach (GProfile pro in groupProfiles)
            {
                if (pro.GetDistanceTo(Me.Location) < clostestway)
                {
                    clostestway = pro.GetDistanceTo(Me.Location);
                    profile = pro;
                }   
            }
            ChangeToIfClose = new List<GProfile>();
            foreach (GProfile pro in groupProfiles)
            {
                if (pro.getChangeToIfClose)
                {
                    ChangeToIfClose.Add(pro);
                }
            }
            ChangeToOnEvent = new List<GProfile>();
            foreach (GProfile pro in groupProfiles)
            {
                if (!pro.getEvent.Equals("None"))
                {
                    ChangeToOnEvent.Add(pro);
                }
            }
            if (clostestway > 45)
            {
                logSystem("Move " + profile.GetDistanceTo(Me.Location) + "yards in current direction");
                move.facePOS(profile.getClosestWaypoint(Me.Location));
                return false;
            }
            logSystem("Closest: " + profile.profile);
            logSystem("Next profile: " + groupProfiles[profile.getNextStep].profile);
            return true;
        }

        private double getDistanceNextProfile
        {
            get
            {
                return groupProfiles[profile.getNextStep].GetDistanceTo(Me.Location);
            }
        }

        private void changeToNextProfile()
        {
            profile = groupProfiles[profile.getNextStep];
            logSystem("Changed profile to: " + profile.profile);
            logSystem("Next profile: " + groupProfiles[profile.getNextStep].profile);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region updateEvents
        private void updateEvents()
        {
            #region AB  Events
            if (Me.ZoneID.Equals(529))
            {
                //Horde
                if (GContext.gethojreFarve1.R.Equals(255) && !STHorde)
                {
                    logSystem("Horde got the ST");
                    STHorde = true;
                    STAlliance = false;
                }
                if (GContext.gethojreFarve2.R.Equals(255) && !FarmHorde)
                {
                    logSystem("Horde got the Farm");
                    FarmHorde = true;
                    FarmAlliance = false;
                }
                if (GContext.gethojreFarve3.R.Equals(255) && !BSHorde)
                {
                    logSystem("Horde got the BS");
                    BSHorde = true;
                    BSAlliance = false;
                }
                if (GContext.gethojreFarve4.R.Equals(255) && !LMHorde)
                {
                    logSystem("Horde got the LM");
                    LMHorde = true;
                    LMAlliance = false;
                }
                if (GContext.gethojreFarve5.R.Equals(255) && !GMHorde)
                {
                    logSystem("Horde got the GM");
                    GMHorde = true;
                    GMAlliance = false;
                }
                //Alliance
                if (GContext.gethojreFarve1.B.Equals(255) && !STAlliance)
                {
                    logSystem("Alliance got the ST");
                    STAlliance = true;
                    STHorde = false;
                }
                if (GContext.gethojreFarve2.B.Equals(255) && !FarmAlliance)
                {
                    logSystem("Alliance got the Farm");
                    FarmAlliance = true;
                    FarmHorde = false;
                }
                if (GContext.gethojreFarve3.B.Equals(255) && !BSAlliance)
                {
                    logSystem("Alliance got the BS");
                    BSAlliance = true;
                    BSHorde = false;
                }
                if (GContext.gethojreFarve4.B.Equals(255) && !LMAlliance)
                {
                    logSystem("Alliance got the LM");
                    LMAlliance = true;
                    LMHorde = false;
                }
                if (GContext.gethojreFarve5.B.Equals(255) && !GMAlliance)
                {
                    logSystem("Alliance got the GM");
                    GMAlliance = true;
                    GMHorde = false;
                }
            }
            #endregion

            #region SOTA
            if (Me.ZoneID.Equals(607))
            {
                if (GContext.gethojreFarve1.G.Equals(255) && !Green)
                {
                    logSystem("Green gate destroyed");
                    Green = true;
                }
                if (GContext.gethojreFarve2.G.Equals(255) && !Purple)
                {
                    logSystem("Purple gate destroyed");
                    Purple = true;
                }
                if (GContext.gethojreFarve3.G.Equals(255) && !Blue)
                {
                    logSystem("Blue gate destroyed");
                    Blue = true;
                }
                if (GContext.gethojreFarve4.G.Equals(255) && Red)
                {
                    logSystem("Red gate destroyed");
                    Red = true;
                }
                if (GContext.gethojreFarve5.G.Equals(255) && Yellow)
                {
                    logSystem("Yellow gate destroyed");
                    Yellow = true;
                }
            }
            #endregion
        }
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Patrol
        /// <summary>
        /// Do all execution for this class. 
        /// If your class overrides this, it will get control as soon as the LBot starts. 
        /// You are responsible for ALL behavior such as finding which mobs to kill, killing them, 
        /// looting them, harvesting game objects, resting, avoiding other players, etc. 
        /// This method should never return. 
        /// </summary>
        #region Patrol field
        GSpellTimer timerProfile;
        GSpellTimer reversingWayPoints = new GSpellTimer(1 * 1000 * 8);
        GSpellTimer changeTimer = new GSpellTimer(5 * 1000);
        GSpellTimer pauseAtWaypoint = new GSpellTimer(0);
        GSpellTimer runningAc = new GSpellTimer(50 * 1000);
        int loop;
        int oldProfileDis = 45;
        public bool isMounted = false;
        bool shouldChange = true;
        bool hasChanged = false;
        bool hasChangedOnEvent = false;
        bool oldMount = false;
        #endregion

        public void Patrol()
        {
            waypoint = new Thread(new ThreadStart(waypointThread));
            runningAc.ForceReady();
            while (true)
            {
                Loot();

                if (runningAc.isReady)
                {
                    RunningAction();
                    runningAc.Reset();
                }

                if (GContext.getPvP)
                    updateEvents();

                //Lad os se om vi kan finde et mål vi kan nakke
                if (!GContext.getPvP)
                {
                    List<GUnit> units = ObjectList.getUnitsInRange(30);

                    if (units != null && units.Count > 0)
                    {
                        GUnit closestUnit = null;
                        double distance = 9999999999;

                        foreach (GUnit u in units)
                        {
                            if ((profile.Factions.Contains(u.Faction) || killAll) &&
                                u.Location.distanceFrom(Me.Location) < distance && !u.IsDead &&
                                !GContext.IsBlacklisted(u) && u.Level < profile.LevelMax &&
                                u.Level > profile.LevelMin)
                            {
                                distance = u.Location.distanceFrom(Me.Location);
                                closestUnit = u;
                            }
                        }

                        if (closestUnit != null && closestUnit.Location.distanceFrom(Me.Location) < GContext.getPullDistance && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                        {
                            waypoint.Abort();
                            move.faceUnit(closestUnit);
                            if (closestUnit.TargetEnemy(GContext, Me,keyhelper))
                            {
                                if (closestUnit.Location.distanceFrom(Me.Location) < GContext.getPullDistance && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                                {
                                    logSystem("Found a mob lets kill");
                                    evaluateCombatResult(KillTarget(closestUnit, false));
                                }
                                else
                                {
                                    waypoint.Start();
                                }
                            }
                            else
                            {
                                waypoint.Start();
                            }
                        }
                        else
                        {
                            if (closestUnit != null)
                            {
                                logSystem("Could not target closest unit");
                                GContext.Blacklist(closestUnit, 60);
                            }
                        }
                    }
                }
                else
                {
                    
                    keyhelper.SendKey("Common.TargetEnemy");

                    if (Me.GetTarget != null && !GContext.IsBlacklisted(Me.GetTarget))
                    {
                        if (Me.GetTarget.Location.distanceFrom(Me.Location) < GContext.getPullDistance)
                        {
                            waypoint.Abort();
                            logSystem("Found a player lets kill");
                            evaluateCombatResult(KillTarget(Me.GetTarget, false));
                        }
                    }
                }
                //Check om vi bliver nakket
                if (ObjectList.CheckForAttackers)
                    if (ObjectList.GetClosestAttacker != null && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                    {
                        waypoint.Abort();
                        logSystem("We are under attack, lets kill instead of patroling");
                        evaluateCombatResult(KillTarget(ObjectList.GetClosestAttacker, true));
                    }

                checkCombat();
                didWeDie();

                Thread.Sleep(1);

                if (waypoint.ThreadState.Equals(System.Threading.ThreadState.Unstarted))
                {
                    waypoint.Start();
                }
            }
        }

        #region waypointThread
        public void waypointThread()
        {
            i = 0;
            loop = 0;
            int reversedWaypoints = 0;
            Waypoints = profile.getListSortedAfterDistance(Me.Location);
            isMounted = false;
            //Hvis profilen skal køre efter tid så sætter vi en timer 
            if (profile.getCondition.Equals(3))
            {
                timerProfile = new GSpellTimer(profile.getParameter * 10000);
            }
            //Start while :)
            while (Waypoints.Count >= i)
            {

                //Lad os checke om vi skulle stopppe lidt
                if (Waypoints[i].X.Equals(profile.getPauseAtWaypoint.X) && Waypoints[i].Y.Equals(profile.getPauseAtWaypoint.Y) && Waypoints[i].Z.Equals(profile.getPauseAtWaypoint.Z))
                {
                    logSystem("We should stop and wait");
                    pauseAtWaypoint = new GSpellTimer((profile.getHowLong * 10) * 1000);

                    while (!pauseAtWaypoint.isReady)
                    {
                        checkCombat();
                        didWeDie();
                        Thread.Sleep(1);
                    }
                }

                #region Change to on if close and on event
                if (groupP)
                {
                    if (ChangeToIfClose != null)
                    {
                        foreach (GProfile p in ChangeToIfClose)
                        {
                            changeToIfCloseWaypoint = p.getClosestWaypoint(Me.Location);
                            if (Me.Location.distanceFrom(changeToIfCloseWaypoint) <= 8 && !hasChanged && changeTimer.isReady)
                            {
                                oldMount = profile.getMount;

                                logSystem("Closer than 8 yards to: " + p.profile + " changing to it");
                                profile = p;
                                Waypoints = p.waypoints;
                                i = 0;
                                shouldChange = false;
                                hasChanged = true;

                                if (oldMount && !profile.getMount)
                                {
                                    keyhelper.SendKey("Common.Mount");
                                    isMounted = false;
                                }
                                if (!oldMount && profile.getMount)
                                {
                                    move.releaseKeys();
                                    Thread.Sleep(50);
                                    keyhelper.SendKey("Common.Mount");
                                    Thread.Sleep(4500);
                                    isMounted = true;
                                }
                                //Check om vi skal starte forfra
                            }
                            if (Waypoints.Count - 1 == i && hasChanged)
                            {
                                shouldChange = true;
                                hasChanged = false;
                                changeTimer.Reset();
                            }
                        }
                    }
                    if (ChangeToOnEvent != null)
                    {
                        foreach (GProfile p in ChangeToOnEvent)
                        {
                            /* Gyldige events for AB
                             * HordeStables
                             * HordeBlackSmith
                             * HordeGoldMine
                             * HordeLumberMill
                             * HordeFarm
                             * 
                             * AllianceStables
                             * AllianceBlackSmith
                             * AllianceGoldMine
                             * AllianceLumberMill
                             * AllianceFarm
                             * 
                             * Gyldige events for SOTA
                             * Green
                             * Purple
                             * Blue
                             * Red
                             * Yellow
                            */
                            if (p.GetDistanceTo(Me.Location) < 45)
                            {
                                #region AB
                                if (Me.ZoneID.Equals(529))
                                {
                                    if (p.getEvent.Equals("AllianceStables") && STAlliance && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("AllianceBlackSmith") && BSAlliance && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("AllianceGoldMine") && GMAlliance && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("AllianceLumberMill") && LMAlliance && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("AllianceFarm") && FarmAlliance && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    //Horde
                                    if (p.getEvent.Equals("HordeStables") && STHorde && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("HordeBlackSmith") && BSHorde && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("HordeGoldMine") && GMHorde && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("HordeLumberMill") && LMHorde && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("HordeFarm") && FarmHorde && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                }
                                #endregion

                                #region SOTA
                                if (Me.ZoneID.Equals(607))
                                {
                                    if (p.getEvent.Equals("Green") && Green && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("Purple") && Purple && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("Blue") && Blue && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("Red") && Red && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                    if (p.getEvent.Equals("Yellow") && Yellow && !hasChangedOnEvent)
                                    {
                                        logSystem("On event activated");
                                        hasChangedOnEvent = true;
                                        profile = p;
                                        Waypoints = p.getListSortedAfterDistance(Me.Location);
                                        i = 0;
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }

                #endregion

                if (!isMounted && profile.getMount)
                {
                    move.releaseKeys();
                    Thread.Sleep(50);
                    
                    keyhelper.SendKey("Common.Mount");
                    Thread.Sleep(4500);
                    isMounted = true;
                }

                #region Conditions
                if (groupP)
                {
                    //Alt efter hvilken condition der er sat så gør vi det rigtige
                    int n = profile.getCondition;
                    if (shouldChange)
                    {
                        switch (n)
                        {
                            case 0:
                                if (profile.getDistance != 0)
                                {
                                    if (getDistanceNextProfile < profile.getDistance && groupProfiles[profile.getNextStep].profile != profile.profile)
                                    {
                                        oldMount = profile.getMount;
                                        oldProfileDis = profile.getDistance;
                                        hasChangedOnEvent = false;
                                        changeToNextProfile();
                                        Waypoints = profile.getListSortedAfterDistance(Me.Location);
                                        i = 0;

                                        if (oldMount && !profile.getMount)
                                        {
                                            keyhelper.SendKey("Common.Mount");
                                            isMounted = false;
                                        }
                                        if (!oldMount && profile.getMount)
                                        {
                                            move.releaseKeys();
                                            Thread.Sleep(50);
                                            keyhelper.SendKey("Common.Mount");
                                            Thread.Sleep(4500);
                                            isMounted = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (getDistanceNextProfile < 45 && groupProfiles[profile.getNextStep].profile != profile.profile)
                                    {
                                        oldMount = profile.getMount;
                                        oldProfileDis = 45;
                                        hasChangedOnEvent = false;
                                        changeToNextProfile();
                                        Waypoints = profile.getListSortedAfterDistance(Me.Location);
                                        i = 0;

                                        if (oldMount && !profile.getMount)
                                        {
                                            keyhelper.SendKey("Common.Mount");
                                            isMounted = false;
                                        }
                                        if (!oldMount && profile.getMount)
                                        {
                                            move.releaseKeys();
                                            Thread.Sleep(50);
                                            keyhelper.SendKey("Common.Mount");
                                            Thread.Sleep(4500);
                                            isMounted = true;
                                        }
                                    }
                                }
                                break;
                            case 1:
                                break;
                            case 2:
                                if (loop >= profile.getParameter)
                                {
                                    goto case 0;
                                }
                                break;
                            case 3:
                                if (timerProfile.isReady)
                                {
                                     goto case 0;
                                }
                                break;
                            default:
                                goto case 0;
                        }
                    }
                }
                #endregion

                checkCombat();
                didWeDie();

                #region distance ches and reverse
                //Check om det næste waypoint er for langt væk.
                if (profile.getDistance != 0)
                {
                    if (Waypoints[i].distanceFrom(Me.Location) > oldProfileDis + 15 && groupP)
                    {
                        if (reversedWaypoints > 3)
                        {
                            move.releaseKeys();
                            logSystem("Reversed waypoints to many times");
                            logSystem("Checking closest profile");
                            double clostestway = groupProfiles[0].GetDistanceTo(Me.Location);
                            profile = groupProfiles[0];
                            foreach (GProfile pro in groupProfiles)
                            {
                                if (pro.GetDistanceTo(Me.Location) < clostestway)
                                {
                                    clostestway = pro.GetDistanceTo(Me.Location);
                                    profile = pro;
                                }
                            }
                            Waypoints = profile.getListSortedAfterDistance(Me.Location);
                            i = 0;
                            //move.MoveToLocation(Waypoints[i]);
                            reversedWaypoints = 0;
                        }
                        reversingWayPoints.Reset();
                        move.releaseKeys();
                        logSystem("Next waypoint to fare away, non circle profile?");
                        logSystem("----- Reversing waypoints -----");
                        i = 0;
                        reversedWaypoints++;
                        Waypoints = profile.reverseWaypoint;
                    }
                }
                else
                {
                    if (Waypoints[i].distanceFrom(Me.Location) > 45)
                    {
                        if (reversedWaypoints > 3 && groupP)
                        {
                            move.releaseKeys();
                            logSystem("Reversed waypoints to many times");
                            logSystem("Checking closest profile");
                            double clostestway = groupProfiles[0].GetDistanceTo(Me.Location);
                            profile = groupProfiles[0];
                            foreach (GProfile pro in groupProfiles)
                            {
                                if (pro.GetDistanceTo(Me.Location) < clostestway)
                                {
                                    clostestway = pro.GetDistanceTo(Me.Location);
                                    profile = pro;
                                }
                            }
                            Waypoints = profile.getListSortedAfterDistance(Me.Location);
                            i = 0;
                           // move.MoveToLocation(Waypoints[i]);
                            reversedWaypoints = 0;
                        }
                        reversingWayPoints.Reset();
                        move.releaseKeys();
                        logSystem("Next waypoint to fare away, non circle profile?");
                        logSystem("----- Reversing waypoints -----");
                        i = 0;
                        reversedWaypoints++;
                        Waypoints = profile.reverseWaypoint;
                    }
                }

                #endregion

                checkCombat();
                didWeDie();

               // move.MoveToLocation(Waypoints[i]);
                Thread.Sleep(1);

                checkCombat();
                didWeDie();

                //Check om vi skal starte forfra
                if (Waypoints.Count - 1 == i)
                {
                    loop++;
                    logSystem("Starting from beginning of waypoint list");
                    i = 0;
                    Waypoints = profile.getListSortedAfterDistance(Me.Location);
                }
                i++;
            }
        }
        #endregion

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region checkCombat, didWeDie, evaluateCombatResult, checkBeforeRest, Loot
        private void checkCombat()
        {
            if (Me.inCombat)
            {
                if (ObjectList.GetClosestAttacker != null && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                {
                    move.releaseKeys();
                    waypoint.Abort();
                    evaluateCombatResult(KillTarget(ObjectList.GetClosestAttacker, true));
                }
                else if (GContext.getPvP)
                {
                    if (ObjectList.GetClosestPlayer != null && !GContext.IsBlacklisted(ObjectList.GetClosestPlayer))
                    {
                        move.releaseKeys();
                        waypoint.Abort();
                        evaluateCombatResult(KillTarget(ObjectList.GetClosestPlayer, true));
                    }

                }
            }
        }

        private void didWeDie()
        {
            if (Me.IsDead || Me.Health == 0.01 || Me.Health == 0)
            {
                logSystem("We died");
                waypoint.Abort();
                move.releaseKeys();
                if (GContext.getPvP)
                {
                    logSystem("PvP waiting for ress");
                    while (Me.Health <= 0.01)
                    {
                        Thread.Sleep(10);
                    }
                    if (!reloadProfileAfterDead())
                    {
                        logSystem("Error reloading profile, stopping");
                    }
                    else
                    {
                        logSystem("Reloaded the profile after death");
                        onRess();
                        Patrol();
                    }
                }
            }
        }

        private void evaluateCombatResult(eGCombatResult result)
        {
            if (result.Equals(eGCombatResult.Unknown))
            {
                logSystem("Returned Unknown - Not good, what are we going to do know?");
            }
            else if (result.Equals(eGCombatResult.Retry))
            {
                logSystem("Returned retry");
            }
            else if (result.Equals(eGCombatResult.RunAway))
            {
                logSystem("Returned RunAway - not working so lets continue fighting");
                if (ObjectList.CheckForAttackers)
                {
                    if (ObjectList.GetClosestAttacker != null  && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                    {
                        evaluateCombatResult(KillTarget(ObjectList.GetClosestAttacker, true));
                    }
                }
                else
                    result = eGCombatResult.Success;
            }
            else if (result.Equals(eGCombatResult.Vanished))
            {
                logSystem("Hmm monster vanished?");
            }
            else if (result.Equals(eGCombatResult.OtherPlayerTag))
            {
                logSystem("Other player got out target, stop killing");
                checkBeforeRest();
            }
            else if (result.Equals(eGCombatResult.Bugged))
            {
                logSystem("Mob bugged, add to bugged list");
                GContext.Blacklist(Me.TargetGUID, 120);
            }
            else if (result.Equals(eGCombatResult.Died))
            {
                didWeDie();
            }
            else if (result.Equals(eGCombatResult.SuccessWithAdd))
            {
                if (ObjectList.CheckForAttackers)
                {
                    logSystem("Success with add, lets kill the add");
                    if (ObjectList.CheckForAttackers)
                    {
                        if (ObjectList.GetClosestAttacker != null && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                        {
                            evaluateCombatResult(KillTarget(ObjectList.GetClosestAttacker, true));
                        }
                    }
                    else
                    {
                        logSystem("Could not find the add?");
                        result = eGCombatResult.Success;
                    }
                }
                else
                {
                    result = eGCombatResult.Success;
                }
            }
            else if (result.Equals(eGCombatResult.Success))
            {
                logSystem("Success! lets rest");
                checkBeforeRest();
            }
        }

        private void checkBeforeRest()
        {
            if (ObjectList.CheckForAttackers)
                if (ObjectList.GetClosestAttacker != null && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                {
                    logSystem("We are under attack, lets kill instead of resting");
                    evaluateCombatResult(KillTarget(ObjectList.GetClosestAttacker, true));
                }
            if (Rest())
            {
                logSystem("Need more rest");
                if (ObjectList.CheckForAttackers)
                    if (ObjectList.GetClosestAttacker != null && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                    {
                        logSystem("We are under attack, lets kill instead of resting");
                        evaluateCombatResult(KillTarget(ObjectList.GetClosestAttacker, true));
                    }
                Rest();
            }
            if (GContext.shouldLoot)
            {
                logSystem("All done lets loot");
                Loot();
            }

            logSystem("All done lets find new target");
            Patrol();
        }

        public bool doSmallLootSearch()
        {
            //Position.Findpos((int)ObjectList.getMemory().WindowHandle);
            //int xOffset = (Position.getWidth / 2) / 2;
            //int yOffset = (Position.getHeight / 2) / 2;
            ////GSpellTimer timeOut = new GSpellTimer(70 * 1000);
            //while (!GContext.getType.Equals(16))
            //{
            //    Cursor.Position = new Point(Position.getX + xOffset, (Position.getY + yOffset));
            //    Thread.Sleep(10);
            //    if (GContext.getType.Equals(16))
            //        break;

            //    xOffset = xOffset + Position.getWidth / 40;
            //    if (xOffset > ((Position.getWidth / 2) / 2) + Position.getWidth / 2)
            //    {
            //        yOffset = yOffset + Position.getHeight / 40;
            //        xOffset = (Position.getWidth / 2) / 2;
            //        if (yOffset > ((Position.getHeight / 2) / 2) + Position.getHeight / 2)
            //        {
            //            return false;
            //        }
            //    }
            //}
            return true;
        }

        public bool doFullLoot()
        {
            //Position.Findpos((int)ObjectList.getMemory().WindowHandle);
            //int xOffset = 0;
            //int yOffset = 20;
            ////GSpellTimer timeOut = new GSpellTimer(70 * 1000);
            //while (!GContext.getType.Equals(16))
            //{
            //    Cursor.Position = new Point(Position.getX + xOffset, Position.getY + yOffset);
            //    Thread.Sleep(10);
            //    if (GContext.getType.Equals(16))
            //        break;

            //    xOffset = xOffset + Position.getWidth / 40;
            //    if (xOffset > Position.getWidth)
            //    {
            //        yOffset = yOffset + Position.getHeight / 40;
            //        xOffset = 0;
            //        if (yOffset > Position.getHeight)
            //        {
            //            return false;
            //        }
            //    }
            //}
            return true;
        }

        #region DLL Imports
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string sClsName, string sWndName);
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr Hwnd);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr Hwnd, int iCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr Hwnd);
        private const int iRestore = 9;
        private const int iShow = 5;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        #endregion

        public void Loot()
        {
            lootList.Clear();

            //Lets find units that can be looted first.
            int counter = 0;
            while (ObjectList.GuidList.Count > counter)
            {
                if (ObjectList.ObjectList[ObjectList.GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                {
                    if (ObjectList.GUnitList[ObjectList.GuidList[counter]].IsLootable)
                        lootList.Add(ObjectList.GUnitList[ObjectList.GuidList[counter]]);
                }
                counter++;
            }
            //Lets approach the units and loot
            if (lootList.Count != 0 && !GContext.IsBlacklisted(lootList[0]))
            {
                logSystem("Trying to loot");

                //Sætter vinduet til aktiv!
                IntPtr Hwnd = GContext.getWindowsHandle;
                if (Hwnd.ToInt32() > 0)
                {
                    SetForegroundWindow(Hwnd);
                    if (IsIconic(Hwnd))
                        ShowWindow(Hwnd, iRestore);
                    else
                        ShowWindow(Hwnd, iShow);
                }
                Thread.Sleep(500);

                int i = 0;
                while (lootList.Count > i)
                {
                    if (ObjectList.CheckForAttackers)
                    {
                        if (ObjectList.GetClosestAttacker != null && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                        {
                            logSystem("We are under attack, lets kill instead of looting");
                            evaluateCombatResult(KillTarget(ObjectList.GetClosestAttacker, true));
                        }
                    }
                    if (!GContext.IsBlacklisted(lootList[i]))
                    {
                        move.MoveToUnit(lootList[i], 2);
                        if (!doSmallLootSearch())
                        {
                            logSystem("Damit did not find loot, doing fullscreen search");
                            if (!doFullLoot())
                            {
                                logSystem("No loot found, blacklisting");
                                GContext.Blacklist(lootList[i], 90000);
                            }
                        }
                        if (GContext.getType.Equals(16))
                        {
                            Thread.Sleep(100);
                            int X = Cursor.Position.X;
                            int Y = Cursor.Position.Y;
                            //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
                            Thread.Sleep(100);
                        }
                        lootTimer.Reset();
                        while (lootList[i].IsLootable && !lootTimer.isReady)
                        {
                            Thread.Sleep(50);
                        }
                        Thread.Sleep(50);
                        if (!lootList[i].IsLootable)
                            logSystem("Lootable gone");
                        else
                        {
                            logSystem("Loot did not work");
                            GContext.Blacklist(lootList[i], 90000);
                        }
                    }
                    i++;
                }
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Eat drink
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // WaitForSnack - Wait til we're done eating and/or drinking
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        bool bIsEating;
        bool bIsDrinking;
        GSpellTimer EatTimer = new GSpellTimer(300 * 1000); // added by Gray
        GSpellTimer DrinkTimer = new GSpellTimer(300 * 1000); // added by Gray
        protected bool ALWAYS_EATANDDRINK_TOGETHER = false;
        protected void EatSnack()
        {
            EatSomething();
            DrinkSomething();
            if(bIsDrinking || bIsEating)
                while (true)
                {
                    Thread.Sleep(101);
                    if (Me.IsDead)
                    {
                        break;
                    }
                    if (Me.inCombat)
                    {
                        break;
                    }
                    if (bIsEating && !bIsDrinking)
                    {
                        if (Me.Health == 1.0) break;
                    }
                    if (!bIsEating && bIsDrinking)
                    {
                        if (Me.Mana == 1.0) break;
                    }
                    if (EatTimer.isReady && bIsEating)
                    {
                        break;
                    }
                    if (DrinkTimer.isReady && bIsDrinking)
                    {
                        break;
                    }
                    if (Me.Health == 1.0 && Me.Mana == 1.0) break;
                    
                }
            bIsEating = false;
            bIsDrinking = false;
        }

        protected void EatSomething()
        {
            if (Me.IsDead) return;
            if (EatTimer.isReady && Me.Health < GContext.getRestHealth && !Me.inCombat) 
            {
                EatTimer.Reset();                   
                logSystem("Lets Eat!");
                keyhelper.CastSpell("Common.Eat");
                Thread.Sleep(1800);
                bIsEating = true;
            }
        }

        protected void DrinkSomething()
        {
            if (Me.IsDead) return;
            if (DrinkTimer.isReady && Me.Mana < GContext.getRestMana && !Me.inCombat)
            {
                DrinkTimer.Reset();                 
                logSystem("Lets Drink!");
                keyhelper.CastSpell("Common.Drink");
                Thread.Sleep(500);
                bIsDrinking = true;
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Log
        public void logClass(string text)
        {
            GContext.log(text);
        }

        public void logSystem(string text)
        {
            GContext.logSystem(text);
        }
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Overrides

        public virtual bool LEngine_LoadFromFile(string file)
        {
            return false;
        }

        public virtual void ShowConfig()
        {

        }

        /// <summary>
        /// Make sure health/mana are within limits
        /// </summary>
        /// <returns>
        /// True if we got interrupted and need more rest, False if we're good to go
        /// </returns>
        /// <remarks> 
        /// Called out of combat to restore the character to the required limits. GGameClass checks health and mana, 
        /// eating and drinking as appropriate. 
        /// Overrides should generally call the base class after doing their own healing/custom rest. 
        /// The base Rest method checks for bandages, attackers, and handles eating and drink as normal.
        /// </remarks>
        public virtual bool Rest()
        {
            EatSnack();

            if (Me.Health < GContext.getRestHealth || Me.Mana < GContext.getRestMana)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Called periodically while running. 
        /// </summary>
        /// <remarks>
        /// This method can be used to check expiring buffs or other randomness. 
        /// Do not use spells that require channeling here. This method is called very frequently, 
        /// so do not cast more than one spell per call.
        /// </remarks>
        public virtual void RunningAction()
        {

        }

        /// <summary>
        /// Botting is beginning. 
        /// </summary>
        /// <remarks>
        /// When done you should call the base function!
        /// </remarks>
        public virtual void OnStart()
        {
            LoadConfig();
            if (GContext.getPvP)
                updateEvents();

            logSystem("Rest Health: " + GContext.getRestHealth * 100 + "%");
            logSystem("Rest Mana: " + GContext.getRestMana * 100 + "%");
            EatTimer.ForceReady();
            DrinkTimer.ForceReady();
            if (Rest())
            {
                logSystem("Need more rest");
                if (ObjectList.CheckForAttackers)
                    if (ObjectList.GetClosestAttacker != null && !GContext.IsBlacklisted(ObjectList.GetClosestAttacker))
                    {
                        logSystem("We are under attack, lets kill instead of resting");
                        evaluateCombatResult(KillTarget(ObjectList.GetClosestAttacker, true));
                    }
                Rest();
            }
            if (GContext.shouldLoot)
            {
                Loot();
            }

        }

        /// <summary>
        /// Botting is stopping. 
        /// </summary>
        public virtual void OnStop()
        {
            if (waypoint != null)
                waypoint.Abort();
        }

        /// <summary>
        /// We ressed after dieing
        /// </summary>
        public virtual void onRess()
        {

        }

        /// <summary>
        /// Try to kill the target unit. 
        /// </summary>
        /// <param name="Gunit">
        /// Unit we should be killing
        /// </param>
        /// <param name="Bool">
        /// True if we were attacked first (non-pull) 
        /// </param>
        /// <returns>
        /// GCombatResult with what happened
        /// </returns>
        /// <remarks>
        /// This method is usually the biggest part of a GGameClass, 
        /// as it is responsible for combat. 
        /// When we invokes this method, it has already targeted the monster and approached 
        /// within the class's PullDistance range.
        /// Note that this method can be invoked on players or monsters.
        /// Use Target.IsPlayer and Target.IsUnit to find the difference.
        /// </remarks>
        public virtual eGCombatResult KillTarget(GUnit Target, bool IsAmbush)
        {
            return eGCombatResult.Unknown;
        }

        public virtual void CreateDefaultConfig()
        {

        }

        public virtual void LoadConfig()
        {

        }

        public virtual void ShowConfiguration()
        {

        }
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Other
        public bool bgBeforeStart()
        {
            //System.Drawing.Color right = Position.rightBox((int)ObjectList.getMemory().WindowHandle);
            System.Drawing.Color right = Color.Black;
            //Check om vi er: i bg før start, 
            if ((right.R == 0) && (right.G == 128) && (right.B == 0))
            {
                return true;
            }
            return false;
        }
        #endregion

        //----------------------------------------------------------------------------------------------------
    }
}
