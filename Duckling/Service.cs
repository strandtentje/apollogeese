using System;
using System.Collections.Generic;
using System.Net;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// A service that may resolve a http-request on it's own or 
	/// be part of a series of services that involve with
	/// resolving an http-request.
	/// </summary>
	public abstract class Service : IDisposable
	{
		public Map<object> Tags = new Map<object>();

		public PluginCollection<Service> PossibleSiblingTypes {
			get;
			set;
		}

		public Service Parent {
			get;
			internal set;
		}

		public Service Root {
			get; 
			internal set;
		}

		/// <summary>
		/// Sets ancestory, useful on newly constructed services.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public void SetAncestory (Service parent)
		{
			if (Root == null) {				
				Parent = parent;

				if (Parent != null)
					Root = Parent.Root;
				else
					Root = this;
			} else {
				throw new MethodAccessException ("Ancestory can only be set once");
			}
		}

		public static Dictionary<int, Service> ModelLookup = new Dictionary<int, Service>();
		private int modelID = -1;
		private static int modelIDCounter;
		private bool allBranchesAreLoaded = false;

		public void InvokeAllBranchesLoaded ()
		{
			if (allBranchesAreLoaded) {
				throw new MethodAccessException ("This has already been done upon initialization");
			} else {
				allBranchesAreLoaded = true;
				if (AllBrancesLoaded != null) AllBrancesLoaded (this, new EventArgs ());
			}
		}

		/// <summary>
		/// Occurs when branches loaded.
		/// </summary>
		public event EventHandler AllBrancesLoaded;

		/// <summary>
		/// Numeric shorthand for this service, intended purpose: designer.
		/// </summary>
		/// <value>
		/// The model ID
		/// </value>
		public int ModelID { 
			get {
				if (modelID == -1)	{
					lock (Stub) 	
						modelID = modelIDCounter++;
					ModelLookup.Add(modelID, this);
				}

				return modelID;
			} 
			set {
				lock (Stub)
				{
					if (value >= modelIDCounter) {
						modelIDCounter = value + 1;
						ModelLookup.Add(modelIDCounter, this);
					}
				}
				modelID = value;
			}
		}

		private Settings configuration;

		public bool IsLogging {
			get;
			set;
		}

		public string[] LoggingParameters {
			get;
			set;
		}

		/// <summary>
		/// Gets the description of this service. (Cool bonus: May change! Woo!)
		/// May be used as page titles
		/// </summary>
		/// <value>The name of this service</value>
		public abstract string Description { get; }

		/// <summary>
		/// Gets the error message that caused 'TryInitialize' to fail.
		/// </summary>
		/// <value>The error message.</value>
		public string InitErrorMessage { get; private set; }

		/// <summary>
		/// Gets the settings.
		/// </summary>
		/// <returns>
		/// The settings.
		/// </returns>
		public Settings GetSettings ()
		{
			return configuration;
		}		

		/// <summary>
		/// Tries to Initialize and leaves the an InitErrorMessage set if applicable.
		/// When no error is produced, the errormessage will remain blank.
		/// </summary>
		/// <returns><c>true</c>, if initialize was succesful, <c>false</c> otherwise.</returns>
		/// <param name="modSettings">Mod settings.</param>
		public bool SetSettings(Settings modSettings)
		{
			bool succesful;

			configuration = modSettings = modSettings ?? new Settings();

			try
			{
				Branches.ItemChanged += HandleBranchChanged;
				Initialize(modSettings);
				InitErrorMessage = "";
				succesful = true;
			}
			catch(Exception ex) {
				InitErrorMessage = ex.Message;
				Secretary.Report (0, 
				                  string.Format (
					"Initialization for {2} {0} failed with the following message:\n{1}", 
					Description, InitErrorMessage, this.GetType().Name));

				for(Exception inner = ex; inner != null; inner = inner.InnerException)
					Secretary.Report(0, "Inner: ", inner.Message);
				
				succesful = false;
			}

			return succesful;
		}

		protected virtual void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e) {

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

			try
			{
				if (IsLogging)		
				{
					Secretary.Report(5, "Arrived at: ", this.Description);

					if (LoggingParameters != null) {
						Secretary.Report(5, "Parameters: ");
						foreach(string parName in LoggingParameters)
						{
							string parValue;
							if (parameters.TryGetString(parName, out parValue)) {
								Secretary.Report(5, parName, parValue);
							}
						}
					}
				}

				succesful = Process(parameters);
				if (!succesful)
					Secretary.Report(4, "Service", this.Description, "reported in as unsuccesful");
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
		/// Initialize the Service with the specified settings
		/// </summary>
		/// <param name="modSettings">Mod settings.</param>
		protected abstract void Initialize(Settings modSettings);
		/// <summary>
		/// Process the specified request and parameters.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="parameters">Parameters.</param>
		/// <returns>True when the Process was completed succesfully</returns>
		protected abstract bool Process (IInteraction parameters);

		/// <summary>
		/// The branches.
		/// </summary>
		public WatchableMap<Service> Branches = new WatchableMap<Service>();

		/// <summary>
		/// Gets a stub service. StubService.Instance does thesame.
		/// </summary>
		/// <value>The stub.</value>
		public static Service Stub {
			get {
				return StubService.Instance;
			}
		}

		public virtual void Dispose() {
			foreach (Service service in Branches.Dictionary.Values)
				service.Dispose ();
		}
	}
}
