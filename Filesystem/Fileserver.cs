using System;
using System.Reflection;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.ApolloGeese.Http.Headers;
using System.Web;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.Filesystem
{
	/// <summary>
	/// Serves files from a directory from the filesystem.
	/// </summary>
	public class Fileserver : Service
	{
		private static Settings allExtensions;

		private static Settings ExtensionMimes {
			get {
				if (allExtensions == null) {
					string binLocation = Assembly.GetAssembly (typeof(Fileserver)).Location;
					string folder = (new FileInfo (binLocation)).DirectoryName;
					string extensionFile = Path.Combine (folder, "mimetypes.clon");
					if (File.Exists (extensionFile)) {
						allExtensions = Settings.FromFile (extensionFile);
					} else {
						allExtensions = new Settings ();
					}
				}

				return allExtensions;
			}
		}

		public Fileserver ()
		{
		}

		/// <summary>
		/// Allowed mime types
		/// </summary>
		Settings mimeTypes;
		Service notFoundBranch, badRequestBranch;
		/// <summary>
		/// Indicates whether mime-type matching is optional.
		/// </summary>
		bool optionalMimetypes;
		/// <summary>
		/// The root path to serve from
		/// </summary>
		string rootPath;

		public override string Description {
			get {
				return this.rootPath;
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			mimeTypes = new Settings ();

			if (modSettings.Has ("default")) {
				string[] initStrings = modSettings.GetString ("default").Split('|');

				rootPath = initStrings [0];

				if (initStrings.Length > 1) {
					optionalMimetypes = false;

					string[] allowedExtensions = initStrings [1].Split (',');

					foreach (string extension in allowedExtensions) {
						string dot_extension = string.Format ("dot_{0}", extension);

						mimeTypes [extension] = ExtensionMimes.GetString (dot_extension, "application/octet-stream");
					}
				} else {
					optionalMimetypes = true;

					Secretary.Report (1, "Consider whitelisting extensions using |ext,ens,ion - notation");
				}
			} else {
				if (modSettings.Has ("allowedmimetypes")) {
					mimeTypes = modSettings["allowedmimetypes"] as Settings ?? new Settings();
				} else {
					foreach (string key in modSettings.Dictionary.Keys) 
						if (key.StartsWith ("dot_"))				
							mimeTypes [key.Substring (4)] = modSettings.GetString (key, "");
				}

				rootPath = modSettings.GetString("rootpath", ".");
				optionalMimetypes = modSettings.GetBool("optionalmimetypes", false);
			}

			Branches["notfound"] = Stub;
			Branches["badrequest"] = Stub;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "notfound") notFoundBranch = e.NewValue;
			if (e.Name == "badrequest") badRequestBranch  = e.NewValue;
		}

		void sendFileToStream (string finalpath, Stream outgoingBody)
		{
			using (FileStream sourceStream = new FileStream (finalpath, FileMode.Open, FileAccess.Read)) {																
				sourceStream.CopyTo (outgoingBody, 4096);		
			}
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters;
			string trimmedrootpath, trimmedurl, finalpath, extension, mimeType = "application/octet-stream";

			parameters = (IHttpInteraction)uncastParameters.GetClosest (typeof(IHttpInteraction));
			trimmedrootpath = rootPath.TrimEnd ('/');
			trimmedurl = HttpUtility.UrlDecode(parameters.URL.ReadToEnd ().TrimStart ('/'));
			finalpath = string.Format ("{0}/{1}", trimmedrootpath, trimmedurl);

			FileInfo sourcefile = new FileInfo (finalpath);

			extension = sourcefile.Extension.TrimStart ('.').ToLower();

			if (mimeTypes.TryGetString(extension, out mimeType) || optionalMimetypes) {
				if (sourcefile.Exists) {
					if (parameters.HasWriter ()) {
						parameters.GetOutgoingBodyWriter ().Flush ();
					} else {
						parameters.ResponseHeaders.ContentType = new MimeType (mimeType);
						// parameters.ResponseHeaders.ContentLength = sourceStream.Length;
					}

					sendFileToStream (finalpath, parameters.OutgoingBody);
				} else {
					parameters.SetStatuscode (404);
					notFoundBranch.TryProcess(uncastParameters);
				}
			} else {
				parameters.SetStatuscode (410);
				badRequestBranch.TryProcess(uncastParameters);
			}

			return true;
		}
	}
}

