//---------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskCompletionManager.cs" company="Trade System">
//   Copyright © Trade System 2018. All rights reserved. Confidential.
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

#endregion

namespace TradeSystem
{
    /// <summary>
    /// Represents a manager for completable <see cref="Task"/> instances, which automatically timed out if their result is not set in a specified time interval.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public sealed class TaskCompletionManager<TKey> : IDisposable
    {
        #region Entry class

        private sealed class Entry
        {
            #region Fields

            internal DateTime TimeStamp;
            internal Task Task;
            internal Action SetCanceled;
            internal Action<Exception> SetError;
            internal Action<object> SetResult;
            internal Action<TimeSpan> Timeout;

            #endregion
        }

        #endregion

        #region Fields

        private readonly bool throwErrorOnTimeout;
        private readonly ConcurrentDictionary<TKey, Entry> completions = new ConcurrentDictionary<TKey, Entry>();
        private readonly Timer timer;
        private readonly TimeSpan timeout;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCompletionManager{TKey}"/> class.
        /// </summary>
        /// <param name="interval">The interval, in milliseconds, for checking task statuses.</param>
        /// <param name="timeout">The interval, in milliseconds, after an uncompleted task will timeout.</param>
        /// <param name="throwErrorOnTimeout">If <c>true</c>, a <see cref="TimeoutException"/> will be thrown on timeout. If <c>false</c>, the created task will be completed by setting the default result.</param>
        public TaskCompletionManager(int interval, int timeout, bool throwErrorOnTimeout = true)
        {
            if (timeout <= 0)
                throw new ArgumentOutOfRangeException(nameof(timeout));
            this.throwErrorOnTimeout = throwErrorOnTimeout;
            this.timeout = TimeSpan.FromMilliseconds(timeout);
            timer = new Timer(interval) { AutoReset = true };
            timer.Elapsed += Timer_Elapsed;
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets whether the <see cref="Task"/> of the specified <paramref name="key"/> is completed.
        /// </summary>
        /// <param name="key">The key of the <see cref="Task"/>.</param>
        /// <returns><see langword="null"/> if no <see cref="Task"/> found with the specified <paramref name="key"/>,
        /// <c>true</c> if the corresponding <see cref="Task"/> is completed,
        /// <c>false</c> if the corresponding <see cref="Task"/> is not completed.</returns>
        public bool? IsCompleted(TKey key) => completions.TryGetValue(key, out Entry value) ? value.Task.IsCompleted : default(bool?);

        /// <summary>
        /// Creates a <see cref="Task{TResult}"/>, which can be completed by using the public methods of this <see cref="TaskCompletionManager{TKey}"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="key">The key, which can be used to complete the result <see cref="Task{TResult}"/> by calling the methods of this <see cref="TaskCompletionManager{TKey}"/> instance.
        /// If the same key is re-used multiple times, the possibly not completed previous task will be canceled.</param>
        /// <returns>A not completed <see cref="Task{TResult}"/> instance.</returns>
        public Task<TResult> CreateCompletableTask<TResult>(TKey key) => CreateCompletionSource<TResult>(key, e => e.SetCanceled()).Task;

        /// <summary>
        /// Creates a <see cref="Task"/>, which can be completed by using the public methods of this <see cref="TaskCompletionManager{TKey}"/> instance.
        /// </summary>
        /// <param name="key">The key, which can be used to complete the result <see cref="Task"/> by calling the methods of this <see cref="TaskCompletionManager{TKey}"/> instance.
        /// If the same key is re-used multiple times, the possibly not completed previous task will be canceled.</param>
        /// <returns>A not completed <see cref="Task"/> instance.</returns>
        public Task CreateCompletableTask(TKey key) => CreateCompletionSource<TKey>(key, e => e.SetCanceled()).Task;

        /// <summary>
        /// Creates a <see cref="Task{TResult}"/>, which can be completed by using the public methods of this <see cref="TaskCompletionManager{TKey}"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="key">The key, which can be used to complete the result <see cref="Task{TResult}"/> by calling the methods of this <see cref="TaskCompletionManager{TKey}"/> instance.
        /// If the same key is re-used multiple times, the possibly not completed previous task will be canceled.</param>
        /// <param name="oldTaskResult">If there is an old task to replace, then </param>
        /// <returns>A not completed <see cref="Task{TResult}"/> instance.</returns>
        public Task<TResult> ReplaceCompletableTask<TResult>(TKey key, TResult oldTaskResult = default, Exception oldTaskError = null)
            => CreateCompletionSource<TResult>(key, e =>
            {
                if (oldTaskError != null)
                    e.SetError(oldTaskError);
                else
                    e.SetResult(oldTaskResult);
            }).Task;

        /// <summary>
        /// Creates a <see cref="Task{TResult}"/>, which can be completed by using the public methods of this <see cref="TaskCompletionManager{TKey}"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="key">The key, which can be used to complete the result <see cref="Task{TResult}"/> by calling the methods of this <see cref="TaskCompletionManager{TKey}"/> instance.
        /// If the same key is re-used multiple times, the possibly not completed previous task will be canceled.</param>
        /// <returns>A not completed <see cref="Task{TResult}"/> instance.</returns>
        public Task ReplaceCompletableTask(TKey key, Exception oldTaskError = null)
            => CreateCompletionSource<TKey>(key, e =>
            {
                if (oldTaskError != null)
                    e.SetError(oldTaskError);
                else
                    e.SetResult(null);
            }).Task;

        /// <summary>
        /// Removes all <see cref="Task"/>s from the <see cref="TaskCompletionManager{TKey}"/> selected by the provided <paramref name="predicate"/>
        /// and cancels all of the removed unfinished tasks.
        /// </summary>
        /// <param name="predicate">The predicate, which selects the entries to remove.</param>
        /// <param name="error">If <see langword="null"/>, then the unfinished tasks to remove will be canceled; otherwise, they will be finished with error.</param>
        public void RemoveAll(Func<TKey, bool> predicate, Exception error = null)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            foreach (KeyValuePair<TKey, Entry> entry in completions)
            {
                if (!predicate.Invoke(entry.Key))
                    continue;

                if (!completions.TryRemove(entry.Key, out Entry value) || value.Task.IsCompleted)
                    continue;

                if (error == null)
                    value.SetCanceled();
                else
                    value.SetError(error);
            }

            if (completions.IsEmpty)
                timer.Enabled = false;
        }

        /// <summary>
        /// Removes all <see cref="Task"/>s from the <see cref="TaskCompletionManager{TKey}"/> and cancels all of the removed unfinished tasks.
        /// </summary>
        public void Clear() => RemoveAll(_ => true);

        /// <summary>
        /// Disposes this <see cref="TaskCompletionManager{TKey}"/> instance.
        /// </summary>
        public void Dispose()
        {
            timer?.Dispose();
            Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="TaskCompletionManager{TKey}"/> contains a <see cref="Task"/> with specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><c>true</c> if the specified <paramref name="key"/> exists in current <see cref="TaskCompletionManager{TKey}"/>; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(TKey key) => completions.ContainsKey(key);

        /// <summary>
        /// Tries the get a <see cref="Task"/> with specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the <see cref="Task"/> to get.</param>
        /// <param name="task">The result <see cref="Task"/> if the return value is <c>true</c>;otherwise, <see langword="null"/>.</param>
        /// <returns><c>true</c>, if <paramref name="key"/> exists in current <see cref="TaskCompletionManager{TKey}"/>; otherwise, <c>false</c>.</returns>
        public bool TryGetTask(TKey key, out Task task)
        {
            if (completions.TryGetValue(key, out Entry entry))
            {
                task = entry.Task;
                return true;
            }

            task = null;
            return false;
        }

        /// <summary>
        /// Tries the get a <see cref="Task{TResult}"/> with specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the <see cref="Task{TResult}"/> to get.</param>
        /// <param name="task">The result <see cref="Task{TResult}"/> if the return value is <c>true</c>;otherwise, <see langword="null"/>.</param>
        /// <returns><c>true</c>, if <paramref name="key"/> exists in current <see cref="TaskCompletionManager{TKey}"/>; otherwise, <c>false</c>.</returns>
        public bool TryGetTask<TResult>(TKey key, out Task<TResult> task)
        {
            if (completions.TryGetValue(key, out Entry entry))
            {
                task = (Task<TResult>)entry.Task;
                return true;
            }

            task = null;
            return false;
        }

        /// <summary>
        /// Sets a <see cref="Task"/> completed of the specified <paramref name="key"/>, which was created by the <see cref="CreateCompletableTask"/> method.
        /// </summary>
        /// <param name="key">The key of the <see cref="Task"/> to complete.</param>
        /// <param name="remove"><c>true</c> to remove the completed <see cref="Task"/> from the <see cref="TaskCompletionManager{TKey}"/>; otherwise, <c>false</c>.</param>
        public void SetCompleted(TKey key, bool remove = false)
        {
            Entry entry;
            if (((remove && completions.TryRemove(key, out entry))
                   || completions.TryGetValue(key, out entry))
                && !entry.Task.IsCompleted)
            {
                entry.SetResult(null);
            }

            if (completions.IsEmpty)
                timer.Enabled = false;
        }

        /// <summary>
        /// Sets the result of a <see cref="Task{TResult}"/> of the specified <paramref name="key"/>, which was created by the <see cref="CreateCompletableTask{TResult}"/> method.
        /// </summary>
        /// <param name="key">The key of the <see cref="Task{TResult}"/> to complete.</param>
        /// <param name="result">The desired result of the <see cref="Task{TResult}"/> to set.</param>
        /// <param name="remove"><c>true</c> to remove the completed <see cref="Task{TResult}"/> from the <see cref="TaskCompletionManager{TKey}"/>; otherwise, <c>false</c>.</param>
        public void SetResult<TResult>(TKey key, TResult result, bool remove = false)
        {
            Entry entry;
            if (((remove && completions.TryRemove(key, out entry))
                   || completions.TryGetValue(key, out entry))
                && !entry.Task.IsCompleted)
            {
                entry.SetResult(result);
            }

            if (completions.IsEmpty)
                timer.Enabled = false;
        }

        /// <summary>
        /// Cancels the <see cref="Task"/> of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the <see cref="Task"/> to cancel.</param>
        /// <param name="remove"><c>true</c> to remove the completed <see cref="Task{TResult}"/> from the <see cref="TaskCompletionManager{TKey}"/>; otherwise, <c>false</c>.</param>
        public void SetCanceled(TKey key, bool remove = false)
        {
            Entry entry;
            if (((remove && completions.TryRemove(key, out entry))
                   || completions.TryGetValue(key, out entry))
                && !entry.Task.IsCompleted)
            {
                entry.SetCanceled();
            }
            if (completions.IsEmpty)
                timer.Enabled = false;
        }

        /// <summary>
        /// Fails the <see cref="Task"/> of the specified <paramref name="key"/> with the provided <paramref name="error"/>.
        /// </summary>
        /// <param name="key">The key of the <see cref="Task"/> to fail.</param>
        /// <param name="error">The <see cref="Exception"/> to set.</param>
        /// <param name="remove"><c>true</c> to remove the completed <see cref="Task{TResult}"/> from the <see cref="TaskCompletionManager{TKey}"/>; otherwise, <c>false</c>.</param>
        public void SetError(TKey key, Exception error, bool remove = false)
        {
            Entry entry;
            if (((remove && completions.TryRemove(key, out entry))
                   || completions.TryGetValue(key, out entry))
                && !entry.Task.IsCompleted)
            {
                entry.SetError(error);
            }
            if (completions.IsEmpty)
                timer.Enabled = false;
        }

        #endregion

        #region Private Methods

        private TaskCompletionSource<TResult> CreateCompletionSource<TResult>(TKey key, Action<Entry> handleOldEntry)
        {
            var completionSource = new TaskCompletionSource<TResult>();
            var entry = new Entry
            {
                Task = completionSource.Task,
                TimeStamp = HiResDatetime.UtcNow,
                SetResult = result => completionSource.TrySetResult((TResult)result),
                SetCanceled = () => completionSource.TrySetCanceled(),
                SetError = error => completionSource.TrySetException(error),
                Timeout = throwErrorOnTimeout ?
                    new Action<TimeSpan>(to => completionSource.TrySetException(new TimeoutException($"The task was not completed for {to.TotalMilliseconds:N0} milliseconds")))
                    : _ => completionSource.TrySetResult(default)
            };

            // we use AddorUpdate because if we replace an old one we cancel the removed item
            completions.AddOrUpdate(
                key,
                entry,
                (_, old) =>
                {
                    handleOldEntry.Invoke(old);
                    return entry;
                });

            timer.Enabled = true;
            return completionSource;
        }

        #endregion

        #region Event handlers

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = HiResDatetime.UtcNow;
            foreach (KeyValuePair<TKey, Entry> entry in completions)
            {
                if (now - entry.Value.TimeStamp >= timeout)
                    entry.Value.Timeout(timeout);
            }
        }

        #endregion

        #endregion
    }
}
