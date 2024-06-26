﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace TradeSystem.Common.Services
{
	public interface INewsCalendarService
	{
		void Start();
		List<NewsEvent> GetWeeklyEvents();
		bool IsHighRiskTime(DateTime dt, int minutes);
	}

	public class NewsCalendarService : INewsCalendarService
	{
		private string _forexFactoryUrl;
		private const double TimerInterval = 1000 * 60 * 60; //1 hour

		private DateTime _lastDownload = HiResDatetime.UtcNow;
		private WeeklyEvents _weeklyEvents;
		private List<NewsEvent> _weeklyHighImpactEvents;

		private int? _firstKey;
		private int? _lastKey ;
		private readonly Dictionary<int, int> _weeklyHighImpactDistances = new Dictionary<int, int>();

		public void Start()
		{
			_forexFactoryUrl = ConfigurationManager.AppSettings["ForexFactoryUrl"];
			if (string.IsNullOrWhiteSpace(_forexFactoryUrl)) return;

			var timer = new Timer(TimerInterval) {AutoReset = true};
			timer.Elapsed += (sender, args) => Do();
			timer.Start();
			Task.Run(() => Do());
		}

		public List<NewsEvent> GetWeeklyEvents()
		{
			return _weeklyEvents?.Events ?? new List<NewsEvent>();
		}

		public bool IsHighRiskTime(DateTime dt, int minutes)
		{
			if (!_firstKey.HasValue) return false;
			if (!_lastKey.HasValue) return false;

			int distance;
			var key = GetKey(dt);
			if (_weeklyHighImpactDistances.ContainsKey(key)) distance = _weeklyHighImpactDistances[key];
			else if (key < _firstKey.Value) distance = _firstKey.Value - key;
			else if (key > _lastKey.Value) distance = key - _lastKey.Value;
			else return false;

			return distance <= minutes;
		}

		private bool IsDownloadNeeded()
		{
			if (_weeklyEvents == null) return true;
			if (_lastDownload.Date != HiResDatetime.UtcNow.Date) return true;
			return false;
		}

		private void Do()
		{
			try
			{
				if (!IsDownloadNeeded()) return;

				string xml;
				using (var webClient = new WebClient())
					xml = webClient.DownloadString(_forexFactoryUrl);

				using (var reader = new StringReader(xml))
					_weeklyEvents = (WeeklyEvents)new XmlSerializer(typeof(WeeklyEvents)).Deserialize(reader);
				_weeklyEvents.Parse();

				GenerateOptimizedDictionary();
				Logger.Debug("NewsCalendarService update SUCCESS");
			}
			catch(Exception e)
			{
				Logger.Error("NewsCalendarService exception", e);
			}
			finally
			{
				_lastDownload = HiResDatetime.UtcNow;
			}
		}

		private void GenerateOptimizedDictionary()
		{
			_weeklyHighImpactEvents = _weeklyEvents.Events.Where(e => e.ImpactType == NewsEvent.ImpactTypes.High).ToList();

			_firstKey = null;
			_lastKey = null;
			_weeklyHighImpactDistances.Clear();

			if (!_weeklyHighImpactEvents.Any()) return;

			_firstKey = GetKey(_weeklyHighImpactEvents.First().EventTimeUtc);
			_lastKey = GetKey(_weeklyHighImpactEvents.Last().EventTimeUtc);

			for (var i = _firstKey.Value; i <= _lastKey.Value; i++)
			{
				_weeklyHighImpactDistances[i] =
					_weeklyHighImpactEvents.Min(e => Math.Abs((int)TimeSpan.FromTicks(e.EventTimeUtc.Ticks).TotalMinutes - i));
			}
		}


		private int GetKey(DateTime dt)
		{
			var roundTo = TimeSpan.FromMinutes(1);
			var ticks = (long) (Math.Round(dt.ToUniversalTime().Ticks / (double) roundTo.Ticks) * roundTo.Ticks);
			return (int) TimeSpan.FromTicks(ticks).TotalMinutes;
		}
	}
}
