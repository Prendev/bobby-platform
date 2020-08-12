using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TradeSystem.Data.Models;

namespace TradeSystem.Strategies
{
	/// <summary>
	/// Strategy service base class
	/// </summary>
	/// <typeparam name="T">Strategy entity type</typeparam>
	public abstract class StrategyServiceBase<T> : IStrategyService<T> where T : StrategyEntityBase
	{
		/// <summary>
		/// Cancellation toke source map
		/// </summary>
		private readonly ConcurrentDictionary<T, CancellationTokenSource> _sources =
			new ConcurrentDictionary<T, CancellationTokenSource>();

		/// <inheritdoc/>
		public void Start(T strategy)
		{
			lock (strategy)
			{
				if (strategy.Running) return;
				strategy.Running = true;
			}

			var source = _sources.AddOrUpdate(strategy,
				st => new CancellationTokenSource(),
				(st, cts) =>
				{
					cts?.Dispose();
					return new CancellationTokenSource();
				});

			new Thread(() => RunnerLoop(strategy, source.Token))
			{
				Name = $"{typeof(T).Name}_{strategy.Id}",
				IsBackground = true
			}
				.Start();
		}

		/// <inheritdoc/>
		public void Suspend(T strategy)
		{
			if (!_sources.TryGetValue(strategy, out var cts)) return;
			cts?.Cancel(true);
			strategy.WaitHandle.Set();
		}

		/// <inheritdoc/>
		public void SuspendAll() => _sources.ToList().ForEach(s => Suspend(s.Key));

		/// <summary>
		/// Thread runner function
		/// </summary>
		/// <param name="strategy">Set of a trading strategy</param>
		/// <param name="token"> Cancellation token</param>
		private void RunnerLoop(T strategy, CancellationToken token)
		{
			var subscribed = false;
			strategy.NewTick -= Strategy_NewTick;

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (!strategy.Run || !strategy.IsConnected)
					{
						Thread.Sleep(1);
						continue;
					}

					if (!subscribed)
					{
						strategy.NewTick += Strategy_NewTick;
						subscribed = true;
						strategy.Subscribe();
					}

					strategy.WaitHandle.WaitOne();
					if (token.IsCancellationRequested)
						break;

					Check(strategy, token);
					OnTickProcessed(strategy);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error($"{strategy} strategy set exception", e);
					break;
				}
			}

			strategy.NewTick -= Strategy_NewTick;
			strategy.Running = false;
			Logger.Info($"{strategy} strategy set is stopped");
		}

		/// <summary>
		/// Strategy new tick event handler
		/// </summary>
		/// <param name="sender">Set of a trading strategy</param>
		/// <param name="e">New tick event</param>
		private void Strategy_NewTick(object sender, Common.Integration.NewTick e)
		{
			var strategy = (T)sender;
			if (!_sources.TryGetValue(strategy, out var token) || token.IsCancellationRequested)
				return;

			strategy.WaitHandle.Set();
		}

		/// <summary>
		/// Check for trading action
		/// </summary>
		/// <param name="strategy">Set of a trading strategy</param>
		/// <param name="token"> Cancellation token</param>
		protected abstract void Check(T strategy, CancellationToken token);

		/// <summary>
		/// Is backtester
		/// </summary>
		/// <param name="strategy">Set of a trading strategy</param>
		/// <returns>True or false</returns>
		protected abstract bool IsBackTester(T strategy);

		/// <summary>
		/// Tick processed event handler
		/// </summary>
		/// <param name="strategy">Set of a trading strategy</param>
		protected abstract void OnTickProcessed(T strategy);
	}
}
