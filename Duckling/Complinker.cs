using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Links and compiles into executable. Sort of. Mostly a rad name.
	/// I mean, it's got some plink plink going on.
	/// </summary>
	public class Complinker
	{
		/// <summary>
		/// Collection of plugin libraries; each lib may and probably does
		/// contain mulitple plugin services.
		/// </summary>
		private PluginCollection<Service> plugins = new PluginCollection<Service> ();

		/// <summary>
		/// The branch name matcher. Not so neat. Maybe parameterise this later?
		/// </summary>
		private Regex branchNameMatcher = new Regex ("(.+)_branch");

		/// <summary>
		/// Adds a plugin file.
		/// </summary>
		/// <param name="str">Filename.</param>
		public void AddPluginFile (string str)
		{
			plugins.AddFile (str);
		}

		/// <summary>
		/// Connects a branch service to a service
		/// </summary>
		/// <param name="service">Service.</param>
		/// <param name="branchname">Branchname.</param>
		/// <param name="branchdata">Branchdata.</param>
		private void ConnectBranch (Service service, string branchname, Settings branchdata) {
			service.Branches [branchname] = GetServiceForSettings (branchdata);
		}

		/// <summary>
		/// Loads a tree of services.
		/// </summary>
		/// <returns>The tree.</returns>
		/// <param name="config">Config.</param>
		public Service GetServiceForSettings (Settings config)
		{
			string type;
			Settings moduleConfiguration;
			Service newService;
			bool succesfulInit, log;
			string[] logparams;

			if (config.Tag is Service) {
				newService = config.Tag as Service;
			} else {
				type = config.GetString ("type", "StubService");
				moduleConfiguration = (Settings)config ["modconf"];

				log = config.GetBool("log", false);
				logparams = config.GetString("logparams", "").Split(',');

				newService = plugins.GetConstructed (type);
				succesfulInit = newService.SetSettings (moduleConfiguration);
				newService.PossibleSiblingTypes = plugins;
				newService.IsLogging = log;
				newService.LoggingParameters = logparams;

				foreach (KeyValuePair<string, object> nameAndBranch in config.Dictionary) {
					Match branchName = branchNameMatcher.Match (nameAndBranch.Key);

					if (branchName.Success) ConnectBranch(
						newService, 
						branchName.Groups [1].Value, 
						nameAndBranch.Value as Settings);
				}

				if (config.Has ("branches")) {
					Settings branches = config.GetSubsettings ("branches");

					foreach (KeyValuePair<string, object> nameAndBranch in branches.Dictionary) 
						ConnectBranch (newService, nameAndBranch.Key, nameAndBranch.Value as Settings);
				}

				if (!succesfulInit) 
					Secretary.Report (5, type, " produced an error on initialization: ", newService.InitErrorMessage);


				config.Tag = newService;
			}

			return newService;
		}
	}
}

