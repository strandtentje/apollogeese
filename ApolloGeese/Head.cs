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

			foreach (object config in (configuration["trees"] as IEnumerable<object>)) {
				LoadTree ((Settings)config);
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
			string type; Settings moduleConfiguration;
			Service newService; bool succesfulInit;

			type = (string)config ["type"];
			moduleConfiguration = (Settings)config ["modconf"];
			newService = plugins.GetConstructed (type);
			succesfulInit = newService.TryInitialize (moduleConfiguration);

			foreach (KeyValuePair<string, object> nameAndBranch in config.BackEnd) {
				Match branchName = branchNameMatcher.Match (nameAndBranch.Key);

				if (branchName.Success) {
					Settings treeConf = nameAndBranch.Value as Settings;
					newService.Branches [branchName.Groups[1]] = LoadTree (treeConf);
				}
			}

			Secretary.Report (5, type, "now has", newService.Branches.BackEnd.Count, "branches.");
			if (succesfulInit) return newService;
			Secretary.Report (5, 
			                  type,
			                  " produced an error on initialization: ",
			                  newService.InitErrorMessage);
			return NewsStyleUriParser;
		}
	}
}
