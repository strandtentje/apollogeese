using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Reflection;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text.RegularExpressions;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese
{
	/// <summary>
	/// Secretary. Fires up a webserver and loads in the modules according
	/// to configuration. Will keep logg (Log may be kept by calling Secretary.Report
	/// </summary>
	public class Head
	{
		static PluginCollection<Service> plugins = new PluginCollection<Service> ();

		static void StartLog(string folder)
		{
			string time = DateTime.Now.ToString ("yyyy-MM-dd--THHmmsszz");

			Secretary logger = new Secretary (string.Format ("{0}/{1}.log",	folder, time));

			logger.globVerbosity = 10;

			logger.ReportHere (0, "Logfile Opened");
		}

		static void RunConfig (string config)
		{
			Settings configuration = Settings.FromFile (config);

			foreach (object pluInFileObj in (configuration ["plugins"] as IEnumerable<object>))
				plugins.AddFile ((string)pluInFileObj);

			Settings instances = configuration.GetSubsettings("instances");

			foreach (Settings instance in instances.Dictionary.Values) {
				LoadTree (instance);
			}
						
			Secretary.Report (5, "Loaded Branches");
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		static void Main (string[] args)
		{
			Queue<string> CommandLineArguments = new Queue<string> (args);

			string config = "apollogeese.conf", logfolder = ".";

			while (CommandLineArguments.Count > 0) {
				string paramAhead = CommandLineArguments.Dequeue();
				if (paramAhead.ToLower() == "-c") 
					config = CommandLineArguments.Dequeue();
				else if (paramAhead.ToLower() == "-l")
					logfolder = CommandLineArguments.Dequeue();
				else 
					Secretary.Report(5, "Unknown command line parameter: ", paramAhead);
			}
		
			StartLog(logfolder);
			RunConfig(config);
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
			bool succesfulInit, log;
			string[] logparams;

			if (config.Tag is Service) {
				newService = config.Tag as Service;
			} else {
				type = (string)config ["type"];
				moduleConfiguration = (Settings)config ["modconf"];

				log = config.GetBool("log", false);
				logparams = config.GetString("logparams", "").Split(',');

				newService = plugins.GetConstructed (type);
				succesfulInit = newService.SetSettings (moduleConfiguration);
				newService.IsLogging = log;
				newService.LoggingParameters = logparams;

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
