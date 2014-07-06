using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Reflection;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese
{
	/// <summary>
	/// Secretary. Fires up a webserver and loads in the modules according
	/// to configuration. Will keep logg (Log may be kept by calling Secretary.Report
	/// </summary>
	public class Head
	{
		static PluginCollection<Service> plugins = new PluginCollection<Service> ();

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		static void Main (string[] args)
		{
			(new Secretary (
				string.Format (
				"{0}\\ApolloGeese\\{1}.log",
				Environment.SpecialFolder.ApplicationData,
				DateTime.Now.ToString ("u"))) { globVerbosity = 10 }).ReportHere (
				0, "Logfile Opened");

			Settings configuration = Settings.FromFile ("apollogeese.conf");

			foreach (object pluInFileObj in (configuration ["plugins"] as IEnumerable<object>))
				plugins.AddFile ((string)pluInFileObj);

			Settings instances = configuration.GetSubsettings("instances");

			foreach (Settings instance in instances.Dictionary.Values) {
				LoadTree (instance);
			}
						
			Secretary.Report (5, "Loaded Branches");
		}

		static Regex branchNameMatcher = new Regex ("(.+)_branch");

		/// <summary>
		/// Loads a tree of services.
		/// </summary>
		/// <returns>The tree.</returns>
		/// <param name="config">Config.</param>
		static Service LoadTree (Settings config)
		{
			string type;
			Settings moduleConfiguration;
			Service newService;
			bool succesfulInit;

			if (config.Tag is Service) {
				newService = config.Tag as Service;
			} else {
				type = (string)config ["type"];
				moduleConfiguration = (Settings)config ["modconf"];
				newService = plugins.GetConstructed (type);
				succesfulInit = newService.TryInitialize (moduleConfiguration);

				foreach (KeyValuePair<string, object> nameAndBranch in config.Dictionary) {
					Match branchName = branchNameMatcher.Match (nameAndBranch.Key);

					if (branchName.Success) {
						Settings treeConf = nameAndBranch.Value as Settings;
						newService.Branches [branchName.Groups [1].Value] = LoadTree (treeConf);
					}
				}

				if (!succesfulInit) {
					Secretary.Report (5, type, " produced an error on initialization: ",
				                  newService.InitErrorMessage);
				}

				config.Tag = newService;
			}

			return newService;
		}
	}
}
