using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Settings;
using L = BorrehSoft.Utensils.Log.Secretary;
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
		static PluginCollection<Service> plugins;

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
				DateTime.Now.ToString ("u")))).ReportHere (0, "Logfile Opened");

			Settings configuration = Settings.FromFile ("apollogeese.conf");

			plugins = PluginCollection<Service>.FromFiles (
					(string[])configuration ["plugins"]);

			foreach (Settings config in (Settings[])configuration["trees"])
				LoadTree (config);

			Console.ReadLine();
		}

		/// <summary>
		/// Loads a tree of services.
		/// </summary>
		/// <returns>The tree.</returns>
		/// <param name="config">Config.</param>
		static Service LoadTree (Settings config)
		{
			string type = (string)config ["_type"];
			Settings modconf = (Settings)config ["_modconf"];

			Service svc = plugins.GetConstructed (type);
			svc.Initialize (modconf);

			foreach (string branch in config.GetKeys()) {
				if (!branch.StartsWith ("_")) {
					svc.RegisterBranch (
						branch,
						(Settings)LoadTree (config [branch]));
				}
			}
		}
	}
}
