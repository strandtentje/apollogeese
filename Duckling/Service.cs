using System;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// A service that may resolve a http-request on it's own or 
	/// be part of a series of services that involve with
	/// resolving an http-request.
	/// </summary>
	public abstract class Service
	{
		private Dictionary<string, Service> branches = new Dictionary<string, Service> ();

		/// <summary>
		/// Gets the description of this service. (Cool bonus: May change! Woo!)
		/// May be used as page titles
		/// </summary>
		/// <value>The name of this service</value>
		public abstract string Description { get; }
		/// <summary>
		/// Gets an array of branch names this service advertises with.
		/// This is primarly of use in the SFC GUI tool, which doesn't
		/// exist yet at the time of writing this.
		/// </summary>
		/// <value>The advertised branches.</value>
		public abstract string[] AdvertisedBranches { get; }
		/// <summary>
		/// Gets the Services this Service may branch into
		/// </summary>
		/// <value>The branches.</value>
		public Dictionary<string, Service> ConnectedBranches { get { return branches; } }

		/// <summary>
		/// Gets the error message that caused 'TryInitialize' to fail.
		/// </summary>
		/// <value>The error message.</value>
		public string InitErrorMessage { get; private set; }

		/// <summary>
		/// Gets the error message that caused 'TryProcess' to fail.
		/// </summary>
		/// <value>The error message.</value>
		public string ProcessErrorMessage { get; private set; }

		/// <summary>
		/// Tries to Initialize and leaves the an InitErrorMessage set if applicable.
		/// When no error is produced, the errormessage will remain blank.
		/// </summary>
		/// <returns><c>true</c>, if initialize was succesful, <c>false</c> otherwise.</returns>
		/// <param name="modSettings">Mod settings.</param>
		public bool TryInitialize(Settings modSettings)
		{
			bool succesful;

			try
			{
				Initialize(modSettings);
				InitErrorMessage = "";
				succesful = true;
			}
			catch(Exception ex) {
				InitErrorMessage = ex.Message;
				Secretary.Report (0, 
				                 string.Format (
					"Initialization for Service {0} failed with the following message:\n{1}", 
					Description, InitErrorMessage));
				succesful = false;
			}

			return succesful;
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

			try
			{
				succesful = Process(parameters);
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
		/// Gets a value indicating whether this instance is deaf for incoming requests
		/// </summary>
		/// <value><c>true</c> if this instance is deaf; otherwise, <c>false</c>.</value>
		public virtual bool IsDeaf { get { return false; } }

		/// <summary>
		/// Registers a branch on the specified pin name.
		/// If another branch already exists on this pin, 
		/// this branch will be disconnected.
		/// </summary>
		/// <param name="pin">Pin name.</param>
		/// <param name="branch">Branch.</param>
		public void RegisterBranch(string pin, Service branch)
		{
			if (branches.ContainsKey (pin))
				branches.Remove (pin);

			branches.Add (pin, branch);
		}

		/// <summary>
		/// Gets the branch count.
		/// </summary>
		/// <value>The branch count.</value>
		public int BranchCount {
			get { return branches.Count; }
		}

		/// <summary>
		/// Runs a branch-operation.
		/// </summary>
		/// <returns><c>true</c>, if branch was ran succesfully, <c>false</c> otherwise.</returns>
		/// <param name="branch">Branchname.</param>
		/// <param name="request">Request.</param>
		/// <param name="parameters">Parameters.</param>
		public bool RunBranch(string branch, IInteraction parameters)
		{
			if (branches.ContainsKey (branch)) {
				Service branchService = branches [branch];
				return branchService.TryProcess (parameters);
			}

			return false;
		}
	}
}
