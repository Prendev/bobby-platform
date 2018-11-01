using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace QvaDev.Common
{
	public class CustomThreadPool : IDisposable
	{
		private readonly BlockingCollection<Action> _queue = new BlockingCollection<Action>();
		private readonly ConcurrentDictionary<Action, TaskCompletionSource<object>> _taskCompletionManager =
			new ConcurrentDictionary<Action, TaskCompletionSource<object>>();

		private readonly int _size;
		private int _busyCount;


		public CustomThreadPool(int size, CancellationToken token)
		{
			_size = size;

			for (var i = 0; i < _size; i++)
				new Thread(() => Loop(token)) {IsBackground = true}.Start();
		}

		private void Loop(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				Action action = null;
				try
				{
					action = _queue.Take(token);
					if (token.IsCancellationRequested) break;

					Interlocked.Increment(ref _busyCount);
					action();
				}
				catch (OperationCanceledException)
				{
					break;
				}
				finally
				{
					Interlocked.Decrement(ref _busyCount);
					if (action != null && _taskCompletionManager.TryRemove(action, out var source))
						source.TrySetResult(null);
				}
			}
		}

		public async Task Run(Action action)
		{
			if (_busyCount >= _size)
				await Task.Run(action);
			else
			{
				var source = new TaskCompletionSource<object>();
				_taskCompletionManager[action] = source;
				_queue.Add(action);
				await source.Task;
			}
		}

		public void Dispose()
		{
			_queue?.Dispose();
		}
	}
}
