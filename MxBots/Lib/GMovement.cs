using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magic;
using MxBots;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GLib
{
   public class GMovement
    {
        GObjectList objectList;
        private bool IKeepOnFollowing;
        BlackMagic Memory;
        KeyHelper KeyHelper;
        Bot CurrentBot;
        bool startedpatrol = false;
        public event StringStatutTransfertEventHandler sendtext;

        public GMovement(GObjectList objectList,Bot Cb)
        {
            if (objectList != null)
            {
                this.objectList = objectList;
                this.Memory = objectList.getMemory();
                KeyHelper = new KeyHelper(Memory);
            }
            IKeepOnFollowing = true;
            CurrentBot = Cb;
        }

        public void Jump()
        {
            KeyHelper.SendKey("Common.Jump");
        }

        public void OpenPvP()
        {
            KeyHelper.SendKey("Common.PvP");
        }
        public void Backwards(bool start)
        {
            if(start)
            {
                Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Down);
            } else {
                Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Down);
            }
        }

        public void releaseKeys()
        {
            Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Up);
            Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Down);
            Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Right);
            Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Left);
        }

        #region Facing code
        public void threadedFace(GUnit PlayerTarget)
        {
           Thread t = new Thread(new ParameterizedThreadStart(this.tface));
           t.Start(PlayerTarget);
        }
        private void tface(object b)
        {
            GLocation position = ((GUnit)b).Location;
            GLocation localPlayerPosition = objectList.LocalPlayer.Location;
            //Find the facing we need to get
            float wowFacing = negativeAngle((float)Math.Atan2((double)(position.Y - localPlayerPosition.Y), (double)(position.X - localPlayerPosition.X)));

            float face;
            //Lets calculate the new facing against out current facing
            float angle = negativeAngle(wowFacing - objectList.LocalPlayer.Facing);
            if (angle < Math.PI && angle > (Math.PI/4))
            {
                //The position we want to face is reached fastest by turning to the left
                face = negativeAngle((wowFacing - objectList.LocalPlayer.Facing));
                faceWithTimer(face, Post.ArrowKeys.Left);
            }
            else if(angle >= Math.PI && angle < (7* Math.PI /4))
            {
                //Lets turn to the right
                face = negativeAngle((objectList.LocalPlayer.Facing - wowFacing));
                //Console.WriteLine("Right is the shortest way, lets face");
                faceWithTimer(face, Post.ArrowKeys.Right);
            }
           
        }
        public void faceUnit(GUnit PlayerTarget)
        {
            this.facePOS(PlayerTarget.Location);
        }

        public void facePOS(GLocation position)
        {
            GLocation localPlayerPosition = objectList.LocalPlayer.Location;
            //Find the facing we need to get
            float wowFacing = negativeAngle((float)Math.Atan2((double)(position.Y - localPlayerPosition.Y), (double)(position.X - localPlayerPosition.X)));

            float face;
            //Lets calculate the new facing against out current facing
            if (negativeAngle(wowFacing - objectList.LocalPlayer.Facing) < Math.PI)
            {
                //The position we want to face is reached fastest by turning to the left
                face = negativeAngle((wowFacing - objectList.LocalPlayer.Facing));
                faceWithTimer(face, Post.ArrowKeys.Left);
            }
            else
            {
                //Lets turn to the right
                face = negativeAngle((objectList.LocalPlayer.Facing - wowFacing));
                //Console.WriteLine("Right is the shortest way, lets face");
                faceWithTimer(face, Post.ArrowKeys.Right);
            }
        }

        private void faceWithTimer(double face, Post.ArrowKeys key)
        {
            GSpellTimer timer = new GSpellTimer(face * 1000 * Math.PI);
            Post.ArrowKeyDown(Memory.WindowHandle, key);
            timer.Reset();
            while (!timer.isReady)
            {
                Thread.Sleep(1);
            }
            Post.ArrowKeyUp(Memory.WindowHandle, key);
        }

        private float negativeAngle(float angle)
        {
            //if the turning angle is negative
            if (angle < 0)
                //add the maximum possible angle (PI x 2) to normalize the negative angle
                angle += (float)(Math.PI * 2);
            return angle;
        }
        #endregion

        #region Movement code
        public bool MoveToLocation(GLocation position)
        {
            return MoveToLocation(position,1,false);
        }

        
        public bool MoveToLocation(GLocation position, double distance,bool patrol)
        {
            GSpellTimer timer = new GSpellTimer(1 * 1000 * 30);
            GSpellTimer timerWaypoint = new GSpellTimer(1 * 1000 * 45);
            GSpellTimer facetimer = new GSpellTimer(3 * 1000);
            

            if(distance > 45)
                timerWaypoint = new GSpellTimer(1 * 1000 * 65);

            if (distance < 45 && distance > 40)
                timerWaypoint = new GSpellTimer(1 * 1000 * 60);

            if (distance < 40 && distance > 35)
                timerWaypoint = new GSpellTimer(1 * 1000 * 55);

            if (distance < 35 && distance > 30)
                timerWaypoint = new GSpellTimer(1 * 1000 * 55);

            GLocation posP = objectList.LocalPlayer.Location;
            GLocation oldPos = objectList.LocalPlayer.Location;
            double dist = posP.distanceFrom(position);
            double lastFace = dist;
            if ((!startedpatrol && patrol==true )|| patrol == false)
            {
                Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Up);
                startedpatrol = true;
            }
            bool faced35 = false;
            bool alerted = false;
            facePOSMove(position);
            //&& !timerWaypoint.isReady
            while (dist > distance )
            {
                posP = objectList.LocalPlayer.Location;
                dist = posP.distanceFrom(position);

                if (shouldBreak)
                    break;

                if (posP.distanceFrom(oldPos) > 2)
                {
                    oldPos = objectList.LocalPlayer.Location;
                    timer.Reset();
                }



                if (posP.distanceFrom(oldPos) < 2 && timer.isReady)
                {


                    if (!alerted)
                    {
                        //objectList.Form.logSystem("Think we are stuck");
                        //objectList.Form.logSystem("Trying something funny, hang on");
                        alerted = true;
                    }


                    releaseKeys();
                    doSomethingCrazy();
                    timer.Reset();
                    timerWaypoint.Reset();
                    facePOS(position);
                    Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Up);

                }

                if (facetimer.isReady)
                {
                     facePOSMove(position);
                     facetimer.Reset();

                }


                Thread.Sleep(1);
            }

            if (timerWaypoint.isReady && dist > distance)
            {
                SendConsole("Error Time Expired", ConsoleLvl.Low);
                Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Up);
                facePOS(position);
                return false;
            }

            if (patrol == false || shouldBreak)
            {
                Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Up);
                facePOS(position);
            }
            return true;
        }

        private void SendConsole(string what, ConsoleLvl cl)
        {
            if (sendtext != null)
            {
                sendtext(new StringStatutTransfertEventArg(what, cl));
            }
        }
        private bool shouldBreak
        {
           get{
            if (objectList.LocalPlayer.IsDead || objectList.LocalPlayer.Health == 0.01 || objectList.LocalPlayer.Health == 0)
            {
                releaseKeys();
                SendConsole("Stop Moving because Im Dead :(", ConsoleLvl.High);
                return true;
            }
            if (CurrentBot.BreakCurActionB == true)
            {
                SendConsole("Should Break", ConsoleLvl.High);
                return true;
            }
            //else if (objectList.LocalPlayer.inCombat)
            //{
            //    releaseKeys();
            //    return true;
            //}
            return false;

           }
   

        }
       
        public GLocation oldLocation;
        public bool FollowMain(ulong GuidTarget, int disctance,GPlayer main)
        {
            //Start by facing
            //go to main stuveu
            if ((main.Location.X != oldLocation.X || main.Location.Y != oldLocation.Y || main.Location.Z != oldLocation.Z || objectList.LocalPlayer.Location.distanceFrom(main.Location) > disctance) && BotHub.follow == true)
            {
                oldLocation = main.Location;
                GUnit TargetObject = main;

                faceUnit(TargetObject);
                GLocation posP = objectList.LocalPlayer.Location;
                GLocation oldPos = objectList.LocalPlayer.Location;
                GLocation posT = main.Location;
                double dist = posP.distanceFrom(posT);
                Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Up);
                int lastFace = 15;
                bool alerted = false;
                double oldX = posT.X;
                double oldY = posT.Y;
                GSpellTimer facetimer = new GSpellTimer(1 * 1000);

                GSpellTimer timer = new GSpellTimer(1 * 1000 * 10);
                GSpellTimer timerWaypoint = new GSpellTimer(1 * 1000 * 45);
                while (dist > disctance  && CurrentBot.Ismain == false && BotHub.follow == true)
                {
                    if (shouldBreak)
                        break;
                    posP = objectList.LocalPlayer.Location;
                    posT = TargetObject.Location;
                    dist = posP.distanceFrom(posT);
                    if (facetimer.isReady)
                    {
                        faceUnit(TargetObject);
                        facetimer.Reset();
                    }
                    float angle = negativeAngle((float)Math.Atan2((double)(posT.Y - posP.Y), (double)(posT.X - posP.X)));
                    if (timer.isReady)
                    {

                        if (posP.distanceFrom(oldPos) < 2)
                        {


                            if (!alerted)
                            {
                                //objectList.Form.logSystem("Think we are stuck");
                                SendConsole("Think I'm stuck", ConsoleLvl.Medium);
                                //objectList.Form.logSystem("Trying something funny, hang on");
                                KeyHelper.SendKey("Common.Jump");
                                Thread.Sleep(1000);
                                alerted = true;

                            }
                            else
                            {
                                releaseKeys();
                                doSomethingCrazy();

                                timerWaypoint.Reset();
                                faceUnit(TargetObject);
                                Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Up);
                            }



                        }
                        else
                        {
                            oldPos = objectList.LocalPlayer.Location;
                        }
                        timer.Reset();
                    }

                    CurrentBot.ActionList.Sort();

                    Thread.Sleep(10);
                }
                if (timerWaypoint.isReady && dist > disctance)
                {
                    SendConsole("Move to unit timed out", ConsoleLvl.High);
                    Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Up);
                    return false;
                }
                Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Up);
                faceUnit(TargetObject);
            }
            else
            {
               // SendConsole("Bug ?", ConsoleLvl.High);
            }
            return true;
        }
       
        public bool MoveToUnit(GUnit Target, int distance)
        {
            //Start by facing
            
            
            faceUnit(Target);
            GLocation posP = objectList.LocalPlayer.Location;
            GLocation oldPos = objectList.LocalPlayer.Location;
            GLocation posT = Target.Location;
            double dist = posP.distanceFrom(posT);
            Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Up);
            
           
            double oldX = posT.X;
            double oldY = posT.Y;
            GSpellTimer timer = new GSpellTimer(1 * 10000 * 2);
            GSpellTimer FaceTimer = new GSpellTimer(500);
            GSpellTimer timerWaypoint = new GSpellTimer(1 * 10000 * 45); //45 secondes max 
            while (dist > distance && !timerWaypoint.isReady)
            {
                if (shouldBreak)
                {
                    break;
                }
                posP = objectList.LocalPlayer.Location;
                posT = Target.Location;
                dist = posP.distanceFrom(posT);

                if (FaceTimer.isReady)
                {
                    facePOSMove(Target.Location);
                    FaceTimer.Reset();
                }
                if (posP.distanceFrom(oldPos) < 2 && timer.isReady)
                {

                    releaseKeys();
                    doSomethingCrazy();
                    timer.Reset();
                    timerWaypoint.Reset();
                    faceUnit(Target);
                    Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Up);
                }
                else if (timer.isReady)
                {
                    oldPos = objectList.LocalPlayer.Location;
                    timer.Reset();
                }
                Thread.Sleep(10);
                
            }
            if (timerWaypoint.isReady && dist > distance)
            {
                SendConsole("Can't Reach Target", ConsoleLvl.Low);

                Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Up);
                return false;
            }
            else
            {
                if (dist > distance)
                {
                    SendConsole("Target Reached >> " + Target.Name, ConsoleLvl.Medium);
                    SendConsole("Distance From T :  " + dist, ConsoleLvl.High);
                    SendConsole("Min Dist :  " + distance + " > ratio : " + Math.Abs((distance - dist) * 100 / distance).ToString() + "%", ConsoleLvl.High);
                }
                Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Up);
                faceUnit(Target);
                return true;
            }
        }
       
        public void facePOSMove(GLocation position)
        {
            GLocation localPlayerPosition = objectList.LocalPlayer.Location;
            //Find the facing we need to get
            float wowFacing = negativeAngle((float)Math.Atan2((double)(position.Y - localPlayerPosition.Y), (double)(position.X - localPlayerPosition.X)));

            float face;
            //Lets calculate the new facing against out current facing
            if (negativeAngle(wowFacing - objectList.LocalPlayer.Facing) < Math.PI)
            {
                //The position we want to face is reached fastest by turning to the left
                face = negativeAngle((wowFacing - objectList.LocalPlayer.Facing));

                if (face > 1.2)
                {
                    //objectList.Form.logSystem("We should not run while facing");
                    Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Up);
                    faceWithTimer(face, Post.ArrowKeys.Left);
                    Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Up);
                }
                else
                {
                    faceWithTimer(face, Post.ArrowKeys.Left);
                }
            }
            else
            {
                //Lets turn to the right
                face = negativeAngle((objectList.LocalPlayer.Facing - wowFacing));
                //Console.WriteLine("Right is the shortest way, lets face");
                if (face > 1.2)
                {
                    //objectList.Form.logSystem("We should not run while facing");
                    Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Up);
                    faceWithTimer(face, Post.ArrowKeys.Right);
                    Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Up);
                }
                else
                {
                    faceWithTimer(face, Post.ArrowKeys.Right);
                }
            }
        }

        #endregion

        private void doSomethingCrazy()
        {
            System.Random random = new System.Random();
            int r = random.Next(1, 5);
            switch (r)
            {
                case 1:
                    Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Down);
                    Thread.Sleep(1000);
                    Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Down);
                    KeyHelper.PressKey("Common.StrafeRight");
                    KeyHelper.SendKey("Common.Jump");
                    Thread.Sleep(1000);
                    KeyHelper.ReleaseKey("Common.StrafeRight");
                    break;
                case 2:
                    KeyHelper.PressKey("Common.StrafeRight");
                    KeyHelper.SendKey("Common.Jump");
                    Thread.Sleep(1000);
                    KeyHelper.ReleaseKey("Common.StrafeRight");
                    break;
                case 3:
                    Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Down);
                    Thread.Sleep(1000);
                    Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Down);
                    KeyHelper.PressKey("Common.StrafeLeft");
                    KeyHelper.SendKey("Common.Jump");
                    Thread.Sleep(1000);
                    KeyHelper.ReleaseKey("Common.StrafeLeft");
                    break;
                case 4:
                    KeyHelper.PressKey("Common.StrafeLeft");
                    KeyHelper.SendKey("Common.Jump");
                    Thread.Sleep(1000);
                    KeyHelper.ReleaseKey("Common.StrafeLeft"); ;
                    break;
                default:
                    KeyHelper.PressKey("Common.StrafeRight");
                    Thread.Sleep(250);
                    KeyHelper.ReleaseKey("Common.StrafeRight");
                    break;
            }
        }

        #region Test + Old facing code - do not use live!

        //My best shot was when i started at 0 and stopped at 0
        //This took: 2,0159 sec so i assume that it takes 2 sec to turn from facing 0 to facing 0 again.
        //So to set the facing we need to turn (face - LocalPlayer.getFacing()) * 1000 * 2PI / 2
        //public void calcFaceTime(GUnit LocalPlayer)
        //{
        //    GSpellTimer timer = new GSpellTimer();
        //    Console.WriteLine("Facing 0");
        //    setFacing(0);
        //    Console.ReadLine();
        //    //Lets turn left until we face 0 again();
        //    Post.ArrowKeyDown(Memory.WindowHandle, Post.ArrowKeys.Left);
        //    timer.Reset();
        //    Thread.Sleep(300);
        //    while (LocalPlayer.Facing > 0.09)
        //    {
        //        Thread.Sleep(1);
        //    }
        //    Console.WriteLine("Time : " + timer.Peek());
        //    Console.WriteLine(LocalPlayer.Facing);
        //    Post.ArrowKeyUp(Memory.WindowHandle, Post.ArrowKeys.Left);
        //}
        #endregion
    }
}
