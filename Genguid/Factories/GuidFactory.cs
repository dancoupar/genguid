﻿using Genguid.Counting;
using Genguid.FactoryObservers;

namespace Genguid.Factories
{
	/// <summary>
	/// Describes a factory for generating GUIDs.
	/// </summary>
	public abstract class GuidFactory : IGuidFactoryObservable
	{
		private GuidPacket currentGuid;
		private readonly GuidDispatcher dispatcher;
		private readonly GuidCounter counter;

		/// <summary>
		/// Creates a new instance of a GUID factory.
		/// </summary>
		public GuidFactory()
		{
			this.dispatcher = new GuidDispatcher();
			this.counter = new GuidCounter(new JsonFileGuidCountStore());
		}

		/// <summary>
		/// Gets the most recent GUID generated by the factory.
		/// </summary>
		public GuidPacket CurrentGuid
		{
			get
			{
				return this.currentGuid;
			}
		}

		/// <summary>
		/// Gets the list of currently registered observers.
		/// </summary>
		public IReadOnlyCollection<IGuidFactoryObserver> Observers
		{
			get
			{
				return this.dispatcher.Observers;
			}
		}

		/// <summary>
		/// Generates a new GUID, replacing the current one.
		/// </summary>
		public void GenerateNextGuid()
		{
			this.counter.Increment();
			this.currentGuid = new GuidPacket(this.counter.Count(), this.Generate());
			this.dispatcher.NotifyObservers(this.currentGuid);
		}

		/// <summary>
		/// Registers the specified observer, to be notified when a new GUID is generated.
		/// </summary>
		/// <param name="observer">The observer to be notified.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		public void RegisterObserver(IGuidFactoryObserver observer)
		{
			this.dispatcher.RegisterObserver(observer);
		}

		/// <summary>
		/// Removes the specified observer, so that it will no longer be notified when a new GUID
		/// is generated.
		/// </summary>
		/// <param name="observer">The observer to remove.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void RemoveObserver(IGuidFactoryObserver observer)
		{
			this.dispatcher.RemoveObserver(observer);
		}

		/// <summary>
		/// Generates and returns a new GUID.
		/// </summary>
		protected abstract Guid Generate();

		/// <summary>
		/// Restores the current GUID using the specified log.
		/// </summary>
		/// <param name="log">
		/// The log from which to restore the current GUID.
		/// </param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public void Restore(GuidGenerationLog log)
		{
			if (log is null)
			{
				throw new ArgumentNullException(nameof(log), "Argument cannot be null.");
			}

			this.currentGuid = log.Fetch(this.counter.Count());
		}
	}
}
