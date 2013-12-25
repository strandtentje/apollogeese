using System;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils.Settings;

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
		/// Initialize the Service with the specified settings
		/// </summary>
		/// <param name="modSettings">Mod settings.</param>
		public abstract void Initialize(Settings modSettings);
		/// <summary>
		/// Process the specified request and parameters.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="parameters">Parameters.</param>
		/// <returns>True when the Process was completed succesfully</returns>
		public abstract bool Process (HttpListenerContext context, Parameters parameters);

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
		public bool RunBranch(string branch, HttpListenerContext context, Parameters parameters)
		{
			if (branches.ContainsKey (branch)) {
				Service branchService = branches [branch];
				return branchService.Process (context, parameters);
			}

			return false;
		}
	}
}

