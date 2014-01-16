using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Settings;
using L = BorrehSoft.Utensils.Log.Secretary;
using OL = BorrehSoft.Utensils.List<object>;
using System.Collections.Generic;
using System.Reflection;

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

		/// <summary>
		/// Loads a tree of services.
		/// </summary>
		/// <returns>The tree.</returns>
		/// <param name="config">Config.</param>
		static Service LoadTree (Settings config)
		{
			string type = (string)config ["type"];

			Secretary.Report (6, "Entered branch for", type);

			Settings modconf = (Settings)config ["modconf"];

			Secretary.Report (7, "Constructing and Initializing node for", type);
			Service svc = plugins.GetConstructed (type);

			if (!svc.TryInitialize (modconf)) {
				Secretary.Report (6, "This module produced an error on initialization: ",
				                  svc.InitErrorMessage, 
				                  " but we're continuing anyways, because we've balls.");
			}

			Secretary.Report (8, "Aforementioned finished.");

			Secretary.Report (6, "Branching within...");

			foreach (string branch in config.GetKeys()) {
				int brIx = branch.IndexOf ("_branch");

				if (brIx > -1) {
					Settings treeConf = (Settings)config [branch];
					svc.RegisterBranch (branch.Remove(brIx), LoadTree (treeConf));
				}
			}

			Secretary.Report (7, type, "now has", svc.BranchCount.ToString(), "branches.");

			return svc;
		}
	}
}
