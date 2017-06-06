using System;
using System.Collections.Generic;
using System.Net;
using BorrehSoft.Utilities;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Log;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Collections.Maps;
using System.Text;
using System.Threading;
using BorrehSoft.Utilities.Log.Profiling;

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
		private ICallbackProfiler hog = new DummyProfiler ();

		public string ConfigLine { get; set; }

		public ICallbackProfiler Hog { get { return this.hog; } set { this.hog = value; } }

		/// <summary>
		/// Numeric identity attribute for this service.
		/// </summary>
		/// <value>
		/// The model ID
		/// </value>
		public int ModelID { 
			get;
			private set;
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

			// instead we will throw an exception we attempt to set the branches twice.

			if (this.watchedBranches != null)
				throw new InvalidOperationException ("Branches may only be set once");

			this.watchedBranches = newBranches;
			this.watchedBranches.ItemChanged += HandleAllBranchChanged;
		}

		public Service() {
			this.ModelID = Interlocked.Increment (ref modelIDCounter);
			ModelLookup.Add(this.ModelID, this);
		}

		void HandleAllBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "_successor") {
				Successor = e.NewValue;
				HasSuccessor = e.NewValue != null;
			}

			HandleBranchChanged (sender, e);
		}

		protected virtual void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e) { }

		private Service Successor {
			get;
			set;
		}

		private bool HasSuccessor {
			get;
			set;
		}

        private bool InvokeProcess(IInteraction parameters)
        {
            bool successful; 

            successful = Process(parameters);
            if (!successful)
				Secretary.Report(4, 
					"Service", this.Description, 
					"reported in as unsuccesful on line", 
					this.ConfigLine);

			if (HasSuccessor) {
				if (!Successor.TryProcess (parameters)) {
					successful = false;
					Secretary.Report (4, 
						"Service's successor", this.Description, 
						"reported in as unsuccesful on line", 
						Successor.ConfigLine);
				}
			}

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

			hog.Measure (delegate() {
				try
				{
					succesful = InvokeProcess(parameters);
					ProcessErrorMessage = "";
				}
				catch (Exception ex) {
					if (parameters.ExceptionHandler == null) {
						Secretary.Report (5, ex.Message, "unhandled by business logic");
					} else {
						parameters.ExceptionHandler (this, parameters, ex);
					}
				}	
			});

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
			Service.ModelLookup.Remove (this.ModelID);
			foreach (Service service in Branches.Dictionary.Values)
				service.Dispose ();
		}

        public virtual void OnReady()
        {
            
        }
    }
}
