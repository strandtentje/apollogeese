using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using Proc = System.Diagnostics.Process;

namespace BorrehSoft.ApolloGeese.Extensions.Filesystem
{
	/// <summary>
	/// Tarballer. Turns directories into balls of tar.
	/// </summary>
	public class Tarballer : Service
	{
		public override string Description {
			get {
				return "tarballs";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		[Instruction("Root path for tar command")]
		public string RootPath { get; set; }
		[Instruction("Tar command", "tar")]
		public string TarCommand { get; set; }

 		protected override void Initialize (Settings modSettings)
		{
			RootPath = modSettings.GetString("rootpath", ".");
			TarCommand = modSettings.GetString("tarcmd", "tar");
		}

		protected override bool Process (IInteraction parameters)
		{		
			IHttpInteraction httpParameters = (IHttpInteraction)parameters.GetClosest(typeof(IHttpInteraction));

			string[] urlArray = httpParameters.URL.ToArray ();

			string decodedPathFromURL = HttpUtility.UrlDecode (Path.Combine (urlArray));

			while (decodedPathFromURL.ToLower().EndsWith(".tar"))
				decodedPathFromURL = decodedPathFromURL.Remove(decodedPathFromURL.Length - 4);

			string requestedPath = Path.Combine (RootPath, decodedPathFromURL);

			httpParameters.ResponseHeaders ["Content-Type"] = "application/tar";

			ProcessStartInfo pStart = new ProcessStartInfo (TarCommand, "-cO .");
			pStart.WorkingDirectory = requestedPath;
			pStart.RedirectStandardOutput = true;
			pStart.UseShellExecute = false;

			Proc p = Proc.Start (pStart);

			bool success = false;

			try {
				p.StandardOutput.BaseStream.CopyTo (httpParameters.OutgoingBody);
				success = true;
			} catch (Exception ex) {
				Secretary.Report (5, "Tarring for", requestedPath, " failed with message ", ex.Message);
			} 

			if (!success) {
				p.StandardOutput.Close ();
				p.Kill ();
				p.Dispose ();
			}

			return success;
		}
	}
}

