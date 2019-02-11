using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TradeSystem.Collections;

namespace TradeSystem.Common
{
	public class CustomThreadPool
	{
		private readonly FastBlockingCollection<Func<Task>> _queue = new FastBlockingCollection<Func<Task>>();
		private readonly ConcurrentDictionary<Func<Task>, TaskCompletionSource<object>> _taskCompletionManager =
			new ConcurrentDictionary<Func<Task>, TaskCompletionSource<object>>();

		private readonly int _size;
		private int _busyCount;


		public CustomThreadPool(int size, string description, CancellationToken token)
		{
			_size = size;

			for (var i = 0; i < _size; i++)
				new Thread(() => Loop(token)) { Name = $"{description}_{i}", IsBackground = true}.Start();
		}

		private void Loop(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				// Getting next action
				Func<Task> action = null;
				try
				{
					action = _queue.Take(token);
				}
				catch (Exception e)
				{
					if (action != null && _taskCompletionManager.TryRemove(action, out var source)) source.SetException(e);
					continue;
				}

				// Executing next action
				try
				{
					Interlocked.Increment(ref _busyCount);
					var task = action();
					task.Wait(token);
					if (_taskCompletionManager.TryRemove(action, out var source)) source.TrySetResult(null);
				}
				catch (Exception e)
				{
					if (_taskCompletionManager.TryRemove(action, out var source)) source.SetException(e);
				}
				finally
				{
					Interlocked.Decrement(ref _busyCount);
				}
			}
		}

		public async Task Run(Func<Task> action)
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
	}


	public class CustomThreadPool<T>
	{
		private readonly BufferBlock<Func<Task<T>>> _queue = new BufferBlock<Func<Task<T>>>();
		private readonly ConcurrentDictionary<object, TaskCompletionSource<T>> _taskCompletionManager =
			new ConcurrentDictionary<object, TaskCompletionSource<T>>();

		private readonly int _size;
		private int _busyCount;


		public CustomThreadPool(int size, string description, CancellationToken token)
		{
			_size = size;

			for (var i = 0; i < _size; i++)
				new Thread(() => Loop(token)) {Name = $"{description}_{i}", IsBackground = true}.Start();
		}

		private void Loop(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				// Getting next action
				Func<Task<T>> action = null;
				try
				{
					action = _queue.Receive(token);
				}
				catch (Exception e)
				{
					if (action != null && _taskCompletionManager.TryRemove(action, out var source)) source.SetException(e);
					continue;
				}

				// Executing next action
				try
				{
					Interlocked.Increment(ref _busyCount);
					// We use .Result intentionally so the original thread stays intact
					var task = action();
					task.Wait(token);
					if (_taskCompletionManager.TryRemove(action, out var source)) source.TrySetResult(task.Result);
				}
				catch (Exception e)
				{
					if (_taskCompletionManager.TryRemove(action, out var source)) source.SetException(e);
				}
				finally
				{
					Interlocked.Decrement(ref _busyCount);
				}
			}
		}

		public async Task<T> Run(Func<Task<T>> action)
		{
			if (_busyCount >= _size)
				return await Task.Run(action);

			var source = new TaskCompletionSource<T>();
			_taskCompletionManager[action] = source;
			_queue.Post(action);
			return await source.Task;
		}
	}
}
