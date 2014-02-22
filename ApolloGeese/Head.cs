using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using OL = BorrehSoft.Utensils.Collections.List<object>;
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

			foreach (object pluInFileObj in (OL)configuration ["plugins"])
				plugins.AddFile ((string)pluInFileObj);

			Secretary.Report (5, "Loading Branches");
			foreach (object config in (OL)configuration["trees"]) {
				LoadTree ((Settings)config);
			}

			Console.ReadLine();
		}

		static Regex branchNameMatcher = new Regex ("(.+)_branch");

		/// <summary>
		/// Loads a tree of services.
		/// </summary>
		/// <returns>The tree.</returns>
		/// <param name="config">Config.</param>
		static Service LoadTree (Settings config)
		{
			string type = (string)config ["type"];

			Secretary.Report (6, "Entered branch for", type);

			Settings moduleConfiguration = (Settings)config ["modconf"];

			Secretary.Report (7, "Constructing and Initializing node for", type);
			Service newService = plugins.GetConstructed (type);

			if (!newService.TryInitialize (moduleConfiguration)) {
				Secretary.Report (6, "This module produced an error on initialization: ",
				                  newService.InitErrorMessage, 
				                  " but we're continuing anyways, because we've balls.");
			}

			Secretary.Report (8, "Aforementioned finished.");

			Secretary.Report (6, "Branching within...");

			foreach (KeyValuePair<string, object> nameAndBranch in config.BackEnd) {
				Match branchName = branchNameMatcher.Match (nameAndBranch.Key);

				if (branchName.Success) {
					Settings treeConf = nameAndBranch.Value as Settings;
					newService.Branches [branchName.Groups[1]] = LoadTree (treeConf);
				}
			}

			Secretary.Report (7, type, "now has", newService.Branches.BackEnd.Count, "branches.");

			return newService;
		}
	}
}
