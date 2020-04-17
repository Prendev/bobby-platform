//---------------------------------------------------------------------------------------------------------------------
// <copyright file="FastBlockingCollection.cs" company="Trade System">
//   Copyright © Trade System 2018. All rights reserved. Confidential.
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

#region Usings

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

#endregion

namespace TradeSystem.Collections
{
    /// <summary>
    /// Represents a blocking collection, which is much more fast than the <see cref="BlockingCollection{T}"/> class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FastBlockingCollection<T> : IReadOnlyCollection<T>, IDisposable
    {
        #region Constants

        private const int cancellationCheckTimeout = 100;

        #endregion

        #region Fields

        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => queue.Count;

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Adds the specified item to the <see cref="FastBlockingCollection{T}"/>.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            queue.Enqueue(item);
            waitHandle.Set();
        }

        /// <summary>
        /// Takes an item from the <see cref="FastBlockingCollection{T}"/>
        /// </summary>
        public T Take()
        {
            T item;
            while (!queue.TryDequeue(out item))
                waitHandle.WaitOne();
            return item;
        }

        /// <summary>
        /// Takes an item from the <see cref="FastBlockingCollection{T}"/>
        /// </summary>
        public T Take(CancellationToken token)
        {
            T item;
            while (!queue.TryDequeue(out item))
            {
                waitHandle.WaitOne(cancellationCheckTimeout);
                token.ThrowIfCancellationRequested();
            }

            return item;
        }

        /// <summary>
        /// Tries to take an item from the <see cref="FastBlockingCollection{T}"/>
        /// </summary>
        public bool TryTake(out T item) => queue.TryDequeue(out item);

        /// <summary>
        /// Tries to take an item from the <see cref="FastBlockingCollection{T}"/>
        /// </summary>
        public bool TryTake(out T item, CancellationToken token)
        {
            while (!queue.TryDequeue(out item))
            {
                waitHandle.WaitOne(cancellationCheckTimeout);
                if (token.IsCancellationRequested)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to take an item from the <see cref="FastBlockingCollection{T}"/>
        /// </summary>
        public bool TryTake(out T item, TimeSpan timeout, CancellationToken token = default)
        {
            if (queue.TryDequeue(out item))
                return true;
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < timeout)
            {
                if (queue.TryDequeue(out item))
                    return true;
                if (token.IsCancellationRequested)
                    return false;
                var timeLeft = (timeout - stopwatch.Elapsed);
                if (timeLeft <= TimeSpan.Zero)
                    break;
                waitHandle.WaitOne(timeLeft);
            }

            return false;
		}

	    /// <summary>
	    /// Clears queue <see cref="FastBlockingCollection{T}"/>
	    /// </summary>
	    public void Clear()
	    {
		    while (queue.TryDequeue(out _))
		    {
		    }
	    }

	    /// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => waitHandle.Dispose();

        #endregion

        #region Explicitly Implemented Interface Methods

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #endregion
    }
}
