using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Settings;
using L = BorrehSoft.Utensils.Log.Secretary;

namespace BorrehSoft.ApolloGeese
{
	/// <summary>
	/// Secretary. Fires up a webserver and loads in the modules according
	/// to configuration. Will keep logg (Log may be kept by calling Secretary.Report
	/// </summary>
	public class Head
	{
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

			HttpServer servicePort = new HttpServer("http://*:8080/");
			Settings configuration = Settings.FromFile ("apollogeese.conf");

			foreach (string dll in (List<string>)configuration["plugins"]) {
				foreach (Service service in ExternalMod.GetInitiatedTypes<Service> (dll)) {
					servicePort.AddService (service);
				}
			}

			Console.ReadLine();
		}
	}
}
