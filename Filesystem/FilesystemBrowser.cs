using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using System.Web;

namespace BorrehSoft.ApolloGeese.Extensions.Filesystem
{
	/// <summary>
	/// Branches for each file and directory in the root path + a certain folder.
	/// </summary>
	public class FilesystemBrowser : Service
	{
		bool useHttp;

		string rootFilesystem;
		Service 
			dirNotFound = Stub,
			directoryItem = Stub,
			fileItem = Stub;

		public override string Description {
			get {
				return this.rootFilesystem;
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "notfound") dirNotFound = e.NewValue;
			if (e.Name == "diritem") directoryItem = e.NewValue;
			if (e.Name == "fileitem") fileItem = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			rootFilesystem = modSettings.GetString("rootpath", ".");
			useHttp = modSettings.GetBool ("usehttp", true);
		}

		protected override bool Process (IInteraction parameters)
		{
			IInteraction uncastParameters;
			IHttpInteraction httpParameters = null;
			string requestedPath = rootFilesystem;
			string coreUrl = "";

			if (parameters.TryGetClosest (typeof(IHttpInteraction), out uncastParameters) && useHttp) {
				httpParameters = (IHttpInteraction)uncastParameters;

				string[] urlArray = httpParameters.URL.ToArray ();

				coreUrl = string.Join ("/", urlArray);

				string decodedPathFromURL = HttpUtility.UrlDecode (Path.Combine (urlArray));

				requestedPath = Path.Combine (requestedPath, decodedPathFromURL);
			}

			DirectoryInfo requestedInfo = new DirectoryInfo (requestedPath);

			if (requestedInfo.Exists) {
				FilesystemItemInteraction itemInteraction = new FilesystemItemInteraction(parameters, rootFilesystem, coreUrl);

				foreach(DirectoryInfo info in requestedInfo.GetDirectories())
				{				
					if (info.FullName.StartsWith(rootFilesystem))
					{
						itemInteraction.Assume(info);
						directoryItem.TryProcess(itemInteraction);
					}
				}

				foreach(FileInfo info in requestedInfo.GetFiles())
				{
					if (info.FullName.StartsWith(rootFilesystem))
					{
						itemInteraction.Assume(info);
						fileItem.TryProcess(itemInteraction);
					}
				}

				return true;
			} else {
				if (httpParameters != null) 
					httpParameters.StatusCode = 404;
				dirNotFound.TryProcess(httpParameters);
				return false;
			}
		}
	}
}

