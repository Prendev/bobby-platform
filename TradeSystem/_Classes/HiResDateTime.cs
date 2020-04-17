//---------------------------------------------------------------------------------------------------------------------
// <copyright file="HiResDatetime.cs" company="Trade System">
//   Copyright © Trade System 2019. All rights reserved. Confidential.
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

#region Usings

using System;
using System.Diagnostics;

using Microsoft.Win32;

#endregion

namespace TradeSystem
{
    /// <summary>
    /// Represents a high resolution and fast <see cref="DateTime"/> service.
    /// </summary>
    public static class HiResDatetime
    {
        #region Fields

        private static readonly Stopwatch stopwatch = new Stopwatch();
	    private static long baseTime;
		private static long localTimeOffset;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the current UTC time.
		/// </summary>
		public static DateTime UtcNow => new DateTime(GetHiResTicks(), DateTimeKind.Utc);

        /// <summary>
        /// Gets the current local time.
        /// </summary>
        public static DateTime Now => new DateTime(GetHiResTicks() + localTimeOffset, DateTimeKind.Local);

        #endregion

        #region Constructors

        static HiResDatetime()
        {
			Reset();

            // In case the system clock gets updated
            SystemEvents.TimeChanged += SystemEvents_TimeChanged;
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Resets the baseline times. If the Windows message pump is running, then it is called automatically when system time is updated.
        /// </summary>
        public static void Reset()
		{
			baseTime = DateTime.UtcNow.Ticks;
			stopwatch.Restart();
			localTimeOffset = TimeZoneInfo.Local.BaseUtcOffset.Ticks;
		}

        #endregion

        #region Private Methods

		private static long GetHiResTicks()
		{
			var utcNow = DateTime.UtcNow.Ticks;
			if (utcNow > baseTime)
			{
				baseTime = utcNow;
				stopwatch.Restart();
			}
			else utcNow += stopwatch.Elapsed.Ticks;
			return utcNow;
		}

		#endregion

		#region Event handlers

		private static void SystemEvents_TimeChanged(object sender, EventArgs e)
		{
			// SystemEvents.TimeChanged can be slow to fire (3 secs), so allow forcing of reset
			Logger.Warn(cb => cb($"{nameof(HiResDatetime)}: TimeChanged event occured."));
			Reset();
		}

        #endregion

        #endregion
    }
}
