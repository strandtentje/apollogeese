using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.ApolloGeese.Duckling.Http.Headers;
using System.Web;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Filesystem
{
	public class Fileserver : Service
	{
		public Fileserver ()
		{
		}

		Settings mimeTypes;
		Service notFoundBranch, badRequestBranch;
		bool optionalMimetypes;
		string rootPath;

		public override string Description {
			get {
				return "Serves file specified by the remaining url relative to the path set in the configuration.";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			mimeTypes = (modSettings["allowedmimetypes"] as Settings) ?? new Settings();
			rootPath = modSettings.GetString("rootpath", ".");
			optionalMimetypes = modSettings.GetBool("optionalmimetypes", false);

			Branches["notfound"] = Stub;
			Branches["badrequest"] = Stub;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "notfound") notFoundBranch = e.NewValue;
			if (e.Name == "badrequest") badRequestBranch  = e.NewValue;
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction parameters;
			string trimmedrootpath, trimmedurl, finalpath, extension, mimeType = "application/octet-stream";

			parameters = uncastParameters as IHttpInteraction;
			trimmedrootpath = rootPath.TrimEnd ('/');
			trimmedurl = HttpUtility.UrlDecode(parameters.URL.ReadToEnd ().TrimStart ('/'));
			finalpath = string.Format ("{0}/{1}", trimmedrootpath, trimmedurl);

			FileInfo sourcefile = new FileInfo (finalpath);

			extension = sourcefile.Extension.TrimStart ('.').ToLower();

			if (mimeTypes.TryGetString(extension, out mimeType) || optionalMimetypes) {
				if (sourcefile.Exists) {
					parameters.ResponseHeaders.ContentType = new MimeType(mimeType);
					parameters.ResponseHeaders.ContentLength = sourcefile.Length;

					FileStream sourceStream = sourcefile.OpenRead();
					sourceStream.CopyTo(parameters.OutgoingBody.BaseStream);
					sourceStream.Close();
				} else {
					parameters.StatusCode = 404;
					notFoundBranch.TryProcess(uncastParameters);
				}
			} else {
				parameters.StatusCode = 410;
				badRequestBranch.TryProcess(uncastParameters);
			}

			return true;
		}
	}
}

