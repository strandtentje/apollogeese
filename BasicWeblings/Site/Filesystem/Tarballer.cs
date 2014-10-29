using System;
using Proc = System.Diagnostics.Process;
using System.Diagnostics;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Web;
using System.IO;
using BorrehSoft.Extensions.BasicWeblings.Server;
using BorrehSoft.ApolloGeese.Duckling.Http.Headers;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Filesystem
{
	public class Tarballer : Service
	{
		public override string Description {
			get {
				return "Returns tarball for directory at url";
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		string rootFilesystem;
		string tarCommand;

 		protected override void Initialize (Settings modSettings)
		{
			rootFilesystem = modSettings.GetString("rootpath", ".");
			tarCommand = modSettings.GetString("tarcmd", "tar");
		}

		protected override bool Process (IInteraction parameters)
		{		
			HttpInteraction httpParameters = parameters as HttpInteraction;

			string[] urlArray = httpParameters.URL.ToArray ();

			string coreUrl = string.Join ("/", urlArray);

			string decodedPathFromURL = HttpUtility.UrlDecode (Path.Combine (urlArray));

			string requestedPath = Path.Combine (rootFilesystem, decodedPathFromURL);

			httpParameters.ResponseHeaders.ContentType = new MimeType ("application/octet-stream");

			ProcessStartInfo pStart = new ProcessStartInfo (tarCommand, "-cO .");
			pStart.WorkingDirectory = requestedPath;
			pStart.RedirectStandardOutput = true;
			pStart.UseShellExecute = false;

			Proc p = Proc.Start (pStart);

			bool success = false;

			try {
				p.StandardOutput.BaseStream.CopyTo (httpParameters.OutgoingBody.BaseStream);
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

