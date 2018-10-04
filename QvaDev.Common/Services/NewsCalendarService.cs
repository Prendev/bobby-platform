using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Timers;
using System.Xml.Serialization;

namespace QvaDev.Common.Services
{
	public interface INewsCalendarService
	{
		void Start();
		List<NewsEvent> GetWeeklyEvents();
	}

	public class NewsCalendarService : INewsCalendarService
	{
		private const string ForexFactoryUrl = "https://www.forexfactory.com/ffcal_week_this.xml";
		private const double TimerInterval = 1000 * 60 * 60; //1 hour

		private DateTime _lastDownload = DateTime.UtcNow;
		private WeeklyEvents _weeklyEvents;

		public void Start()
		{
			var timer = new Timer(TimerInterval) {AutoReset = true};
			timer.Elapsed += (sender, args) => Do();
			timer.Start();
			Do();
		}

		public List<NewsEvent> GetWeeklyEvents()
		{
			lock (this)
			{
				return _weeklyEvents?.Events ?? new List<NewsEvent>();
			}
		}

		private bool IsDownloadNeeded()
		{
			if (_weeklyEvents == null) return true;
			if (_lastDownload.Date != DateTime.UtcNow.Date) return true;
			return false;
		}

		private void Do()
		{
			if (!IsDownloadNeeded()) return;

			lock (this)
			{
				string xml;
				using (var webClient = new WebClient())
				{
					xml = webClient.DownloadString(ForexFactoryUrl);
				}

				var serializer = new XmlSerializer(typeof(WeeklyEvents));
				using (var reader = new StringReader(xml))
				{
					_weeklyEvents = (WeeklyEvents)serializer.Deserialize(reader);
				}
				_lastDownload = DateTime.UtcNow;
			}
		}
	}
}
