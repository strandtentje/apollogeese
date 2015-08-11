using System;
using System.Collections.Generic;
using System.Net;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;
using System.Threading;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	/// <summary>
	/// Abstract for service module implementing high level functionality.
	/// </summary>
	public abstract class Service : Configurables, IDisposable
	{
		public static Dictionary<int, Service> ModelLookup = new Dictionary<int, Service>();
		public PluginCollection<Service> PossibleSiblingTypes { get; set; }

		private WatchableMap<Service> watchedBranches = null;
		private static int modelIDCounter;
		private int modelID = -1;

		/// <summary>
		/// Numeric identity attribute for this service.
		/// </summary>
		/// <value>
		/// The model ID
		/// </value>
		public int ModelID { 
			get {
				if (modelID == -1)	{
					modelID = Interlocked.Increment (ref modelIDCounter);
					ModelLookup.Add(modelID, this);
				}

				return modelID;
			}
		}

		/// <summary>
		/// Gets a stub service. StubService.Instance does thesame.
		/// </summary>
		/// <value>The stub.</value>
		public static Service Stub {
			get {
				return StubService.Instance;
			}
		}

		/// <summary>
		/// Gets the branches.
		/// </summary>
		/// <value>The branches.</value>
		public WatchableMap<Service> Branches {
			get {
				if (watchedBranches == null) SetBranches (new WatchableMap<Service> ());

				return watchedBranches;
			}
		}

		/// <summary>
		/// Sets the branches.
		/// </summary>
		/// <param name="newBranches">New branches.</param>
		public void SetBranches (WatchableMap<Service> newBranches)
		{
			// we should also be raising events indicating that all branches have been cleared
			// if a watchablemap with branches previously existed. also handlers for the old 
			// watchablemap should be cleared. but this doesn't happen, so we won't.

			// instead we will throw an event we attempt to set the branches twice.

			if (this.watchedBranches != null)
				throw new InvalidOperationException ("Branches may only be set once");

			this.watchedBranches = newBranches;
			this.watchedBranches.ItemChanged += HandleBranchChanged;
		}

		public Service() { }

		protected virtual void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e) { }

        private bool InvokeProcess(IInteraction parameters)
        {
            bool successful; 

            successful = Process(parameters);
            if (!successful)
                Secretary.Report(4, "Service", this.Description, "reported in as unsuccesful");

            return successful;
        }

		/// <summary>
		/// Tries to process and leaves a ProcessErrorMessage set if applicable.
		/// When no error is produced, the errormessage will remain blank.
		/// </summary>
		/// <returns><c>true</c>, if process was tryed, <c>false</c> otherwise.</returns>
		/// <param name="context">Context.</param>
		/// <param name="parameters">Parameters.</param>
		public bool TryProcess(IInteraction parameters)
		{
			bool succesful = false;
			string ProcessErrorMessage;

            if (FailHard)
                return InvokeProcess(parameters);

			try
			{
                succesful = InvokeProcess(parameters);
                ProcessErrorMessage = "";
			}
			catch (Exception ex) {
				if (InitErrorMessage.Length > 0) {
					ProcessErrorMessage = string.Format (
						"Already initialized badly with the message:\n{1}.\nThe message for this failure was:\n{2}",
						Description, InitErrorMessage, ex.Message);
				} else {
					ProcessErrorMessage = ex.Message;
				}

				Secretary.Report (0, 
				                 string.Format (
					"Processing for Service {0} failed with the following message: \n{1}",
					Description, ProcessErrorMessage));

				for(Exception inner = ex; inner != null; inner = inner.InnerException)
					Secretary.Report(0, "Inner: ", inner.Message);
			}

			return succesful;
		}

		/// <summary>
		/// Process the specified request and parameters; doesn't avert errors gracefully catch errors.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="parameters">Parameters.</param>
		/// <returns>True when the Process was completed succesfully</returns>
        protected virtual bool Process(IInteraction parameters)
        {
            return this.FastProcess(parameters);
        }

        public virtual bool FastProcess(IFast parameter)
        {
            throw new NotImplementedException("This Service cannot be called from here for it requires to be executed safely.");
        }

		public virtual void Dispose() {
			foreach (Service service in Branches.Dictionary.Values)
				service.Dispose ();
		}
	}
}
