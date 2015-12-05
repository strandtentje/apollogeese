using System;
using System.Reflection;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using System.Web;
using BorrehSoft.Utensils.Log;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.Filesystem
{
	/// <summary>
	/// Serves files from a directory from the filesystem.
	/// </summary>
	public class Fileserver : SplitterService
	{
		private static Settings allExtensions;

		private static Settings ExtensionMimes {
			get {
				if (allExtensions == null) {
					string binLocation = Assembly.GetAssembly (typeof(Fileserver)).Location;
					string folder = (new FileInfo (binLocation)).DirectoryName;
					string extensionFile = Path.Combine (folder, "mimetypes.clon");
					if (File.Exists (extensionFile)) {
						allExtensions = SettingsParser.FromFile (extensionFile);
					} else {
						allExtensions = new Settings ();
					}
				}

				return allExtensions;
			}
		}

		/// <summary>
		/// Allowed mime types
		/// </summary>
		Settings mimeTypes;
		Service notFoundBranch, badRequestBranch, doneBranch;

		[Instruction("When set to true, files will be served, regardless of whether or not mime type is known.")]
		public bool optionalMimetypes { get; set; }

		[Instruction("Starting path for file server.")]
		public string rootPath { get; set; }

		[Instruction("Extensions that may be served.", new string[] { "jpg", "gif", "png" })]
		public IEnumerable<string> allowedExtensions { 
			get { 
				return mimeTypes.Dictionary.Keys; 
			} 
			set { 
				mimeTypes = new Settings ();

				foreach (string extension in value) {
					string dot_extension = string.Format ("dot_{0}", extension);

					mimeTypes [extension] = ExtensionMimes.GetString (dot_extension, "application/octet-stream");
				}
			}
		}

		public override string Description {
			get {
				return this.rootPath;
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] initStrings = defaultParameter.Split ('|');

			this.Settings ["rootpath"] = initStrings [0];

			if (initStrings.Length > 1) {
				this.Settings ["optionalmimetypes"] = false;

				this.Settings ["allowedextensions"] = initStrings [1].Split (',');
			} else {
				this.Settings ["optionalmimetypes"] = true;

				Secretary.Report (1, "Consider whitelisting extensions using |ext,ens,ion - notation");
			}
		}

		bool doSendFile {
			get;
			set;
		}

		protected override void Initialize (Settings modSettings)
		{
			mimeTypes = new Settings ();

			if (modSettings.Has ("allowedmimetypes")) {
				mimeTypes = modSettings["allowedmimetypes"] as Settings ?? new Settings();
			} else {
				foreach (string key in modSettings.Dictionary.Keys) 
					if (key.StartsWith ("dot_"))				
						mimeTypes [key.Substring (4)] = modSettings.GetString (key, "");
			}

			rootPath = modSettings.GetString("rootpath", ".");
			optionalMimetypes = modSettings.GetBool("optionalmimetypes", false);

			if (modSettings.Has("allowedextensions"))
				allowedExtensions = modSettings.GetStringList ("allowedextensions");

			doSendFile = modSettings.GetBool ("sendfile", true);

			if (modSettings.GetBool ("allowany", false)) {
				mimeTypes = ExtensionMimes;
				optionalMimetypes = true;
			}

			Branches["notfound"] = Stub;
			Branches["badrequest"] = Stub;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "notfound")
				notFoundBranch = e.NewValue;
			else if (e.Name == "badrequest")
				badRequestBranch = e.NewValue;
			else if (e.Name == "done")
				doneBranch = e.NewValue;
		}

		void sendFileToStream (FileInfo info, Stream outgoingBody)
		{
			using (FileStream sourceStream = info.OpenRead()) {																
				sourceStream.CopyTo (outgoingBody);		
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
					FileInfo info = new FileInfo (finalpath);

					if (doSendFile) {
						parameters.ResponseHeaders["Content-Type"] = mimeType;
						// parameters.ResponseHeaders.ContentLength = sourceStream.Length;

						sendFileToStream (info, parameters.OutgoingBody);					 
					}

					if ((doneBranch ?? Stub) != Stub) {
						doneBranch.TryProcess (
							new FilesystemChangeInteraction (
								info, 
								Splitter.Split (
									info.Name), 
								this.rootPath, 
								uncastParameters));
					}
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

