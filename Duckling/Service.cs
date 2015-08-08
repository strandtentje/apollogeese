using System;
using System.Collections.Generic;
using System.Net;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Abstract for service module implementing high level functionality.
	/// </summary>
	public abstract class Service : IDisposable
	{
		public PluginCollection<Service> PossibleSiblingTypes { get; set; }

		public virtual IEnumerable<ConfigHint> GetConfigHints() {
			return new ConfigHint[] { };
		}

		public static Dictionary<int, Service> ModelLookup = new Dictionary<int, Service>();
		private int modelID = -1;
		private static int modelIDCounter;

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

		/// <summary>
		/// The configuration of this service
		/// </summary>
		private Settings configuration;

        public bool FailHard = false;

		/// <summary>
		/// Gets the description of this service. (Cool bonus: May change! Woo!)
		/// May be used as page titles
		/// </summary>
		/// <value>The name of this service</value>
		public abstract string Description { get; }

		/// <summary>
		/// Gets the error message that caused 'SetSettings' to fail.
		/// </summary>
		/// <value>The error message.</value>
		public string InitErrorMessage { get; private set; }

		/// <summary>
		/// Gets more info on the error that caused SetSettings to fail.
		/// </summary>
		/// <value>The init error detail.</value>
		public string InitErrorDetail {
			get;
			private set;
		}

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

		public Settings Settings {
			get {
				return configuration;
			}
		}

		public virtual void LoadDefaultParameters(object defaultParameter) {
			LoadDefaultParameters ((string)defaultParameter);
		}

		public virtual void LoadDefaultParameters(string defaultParameter) {

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
				if (modSettings.Has("default")) {
					LoadDefaultParameters(modSettings["default"]);
				}
				Initialize(modSettings);
				configuration.IsLoaded = true;
				InitErrorMessage = "";
				succesful = true;
			}
			catch(Exception ex) {
				InitErrorMessage = ex.Message;
				Secretary.Report (0, 
				                  string.Format (
					"Initialization for {2} {0} failed with the following message:\n{1}", 
					Description, InitErrorMessage, this.GetType().Name));

				StringBuilder errorDetail = new StringBuilder ();

				for (Exception inner = ex; inner != null; inner = inner.InnerException)
					errorDetail.AppendLine (inner.Message);

				InitErrorDetail = errorDetail.ToString ();

				Secretary.Report (0, errorDetail.ToString ());
				
				succesful = false;
			}

			return succesful;
		}

		protected virtual void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e) {

		}

        private bool DoProcess(IInteraction parameters)
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
                return DoProcess(parameters);

			try
			{
                succesful = DoProcess(parameters);
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
		protected virtual void Initialize(Settings modSettings)
		{

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
