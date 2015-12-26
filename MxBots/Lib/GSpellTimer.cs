using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GLib
{
   public class GSpellTimer
    {

        #region -- Private Variables --

        /// <summary>
        /// Holds the value of the StartTime property.
        /// </summary>
        private long m_StartTime;

        /// <summary>
        /// Holds the value of the Frequency property.
        /// </summary>
        private long m_Frequency;
        private double countDowntime;
        private bool varforceReady = false; 
        #endregion

        #region Dll import
        /// <summary>
        /// The QueryPerformanceCounter function retrieves the current 
        /// value of the high-resolution performance counter.
        /// </summary>
        /// <param name="x">
        /// Pointer to a variable that receives the 
        /// current performance-counter value, in counts.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is 
        /// nonzero.
        /// </returns>
        [DllImport("kernel32.dll")]
        extern static int QueryPerformanceCounter(ref long x);

        /// <summary>
        /// The QueryPerformanceFrequency function retrieves the 
        /// frequency of the high-resolution performance counter, 
        /// if one exists. The frequency cannot change while the 
        /// system is running.
        /// </summary>
        /// <param name="x">
        /// Pointer to a variable that receives 
        /// the current performance-counter frequency, in counts 
        /// per second. If the installed hardware does not support 
        /// a high-resolution performance counter, this parameter 
        /// can be zero.
        /// </param>
        /// <returns>
        /// If the installed hardware supports a 
        /// high-resolution performance counter, the return value 
        /// is nonzero.
        /// </returns>
        [DllImport("kernel32.dll")]
        extern static int QueryPerformanceFrequency(ref long x);
        #endregion

        /// <summary>
        /// Initializes a new instance of the StopWatch class.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The system does not have a high-resolution 
        /// performance counter.
        /// </exception>
        /// 

        public GSpellTimer()
        {
            this.countDowntime = 0;
            varforceReady = false;
            Frequency = GetFrequency();
            Reset();

        }

        public GSpellTimer(double countDowntime) 
        {
            this.countDowntime = countDowntime;
            varforceReady = false;
            Frequency = GetFrequency();
            Reset();
        }

        /// <summary>
        /// Returns true if the timer is ready
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The system does not have a high-resolution 
        /// performance counter.
        /// </exception>
        /// 
        public bool isReady
        {
            get {
                try
                {
                    if (varforceReady) 
                        return true;
                    
                    if (Peek() > countDowntime)
                        return true;
                }
                catch
                {

                }
                return false;
            }     
        }

        /// <summary>
        /// Resets the stopwatch. This method should be called 
        /// when you start measuring.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The system does not have a high-resolution 
        /// performance counter.
        /// </exception>
        public void Reset() 
        {
            StartTime = GetValue();
            varforceReady = false;
        }

        /// <summary>
        /// Returns the time that has passed since the Reset() 
        /// method was called.
        /// </summary>
        /// <remarks>
        /// The time is returned in tenths-of-a-millisecond. 
        /// If the Peek method returns '10000', it means the interval 
        /// took exactely one second.
        /// </remarks>
        /// <returns>
        /// A long that contains the time that has passed 
        /// since the Reset() method was called.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The system does not have a high-resolution performance counter.
        /// </exception>
        public long Peek() 
        {
            return (long)(((GetValue() - StartTime)/ (double)Frequency) * 10000);
        }

        /// <summary>
        /// Retrieves the current value of the high-resolution 
        /// performance counter.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The system does not have a high-resolution 
        /// performance counter.
        /// </exception>
        /// <returns>
        /// A long that contains the current performance-counter 
        /// value, in counts.
        /// </returns>
        private long GetValue() 
        {
            long ret = 0;
            if (QueryPerformanceCounter(ref ret) == 0) throw new NotSupportedException("Error while querying " + "the high-resolution performance counter.");
            return ret;
        }

        /// <summary>
        /// Retrieves the frequency of the high-resolution performance 
        /// counter, if one exists. The frequency cannot change while 
        /// the system is running.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The system does not have a high-resolution 
        /// performance counter.
        /// </exception>
        /// <returns>
        /// A long that contains the current performance-counter 
        /// frequency, in counts per second.
        /// </returns>
        private long GetFrequency() 
        {
         long ret = 0;
         if (QueryPerformanceFrequency(ref ret) == 0)
            throw new NotSupportedException(
               "Error while querying "
               + "the performance counter frequency.");
         return ret;
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// A long that holds the start time.
        /// </value>
        private long StartTime 
        {
             get
             {
                return m_StartTime;
             }
             set
             {
                m_StartTime = value;
             }
        }

        public void ForceReady()
        {
            varforceReady = true;
        }

        /// <summary>
        /// Gets or sets the frequency of the high-resolution 
        /// performance counter.
        /// </summary>
        /// <value>
        /// A long that holds the frequency of the 
        /// high-resolution performance counter.
        /// </value>
        private long Frequency 
        {
         get
         {
            return m_Frequency;
         }
         set
         {
            m_Frequency = value;
         }
        }
    }
}
