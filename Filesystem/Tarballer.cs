using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.ApolloGeese.Http.Headers;
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

		string rootFilesystem;
		string tarCommand;

 		protected override void Initialize (Settings modSettings)
		{
			rootFilesystem = modSettings.GetString("rootpath", ".");
			tarCommand = modSettings.GetString("tarcmd", "tar");
		}

		protected override bool Process (IInteraction parameters)
		{		
			IHttpInteraction httpParameters = (IHttpInteraction)parameters.GetClosest(typeof(IHttpInteraction));

			string[] urlArray = httpParameters.URL.ToArray ();

			string decodedPathFromURL = HttpUtility.UrlDecode (Path.Combine (urlArray));

			while (decodedPathFromURL.ToLower().EndsWith(".tar"))
				decodedPathFromURL = decodedPathFromURL.Remove(decodedPathFromURL.Length - 4);

			string requestedPath = Path.Combine (rootFilesystem, decodedPathFromURL);

			httpParameters.ResponseHeaders.ContentType = new MimeType ("application/tar");

			ProcessStartInfo pStart = new ProcessStartInfo (tarCommand, "-cO .");
			pStart.WorkingDirectory = requestedPath;
			pStart.RedirectStandardOutput = true;
			pStart.UseShellExecute = false;

			Proc p = Proc.Start (pStart);

			bool success = false;

			try {
				if (httpParameters.HasWriter())				
					throw new Exception ("can't serve files to outgoing stream that has a writer");

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

