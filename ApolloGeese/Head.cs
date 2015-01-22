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
		static Complinker loader;

		static void StartLog(string folder)
		{
			string time = DateTime.Now.ToString ("yyyy-MM-dd--THHmmsszz");

			Secretary logger = new Secretary (string.Format ("{0}/{1}.log",	folder, time));

			logger.globVerbosity = 10;

			logger.ReportHere (0, "Logfile Opened");
		}

		static void RunConfig (string config)
		{
			Settings configuration = null;

			try {
				configuration = Settings.FromFile (config);	
			} catch (Exception ex) {
				Secretary.Report (0, "Failure during reading of clon:\n", ex.Message);
			}

			if (configuration == null) {
				Environment.Exit (1);
			} else {
				foreach (object pluInFileObj in (configuration ["plugins"] as IEnumerable<object>))
					loader.AddPluginFile ((string)pluInFileObj);

				Settings instances = configuration.GetSubsettings("instances");

				foreach (Settings instance in instances.Dictionary.Values) {
					loader.GetServiceForSettings (instance);
				}

				Secretary.Report (5, "Loaded Branches");
			}
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
	}
}
