using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.ApolloGeese.Duckling.Http.Headers;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class Fileserver : Service
	{
		public Fileserver ()
		{
		}

		Settings mimeTypes;
		Service notFoundBranch, badRequestBranch;
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
			string trimmedrootpath, trimmedurl, finalpath, extension, mimeType;

			parameters = uncastParameters as IHttpInteraction;
			trimmedrootpath = rootPath.TrimEnd ('/');
			trimmedurl = parameters.URL.ReadToEnd ().TrimStart ('/');
			finalpath = string.Format ("{0}/{1}", trimmedrootpath, trimmedurl);

			FileInfo sourcefile = new FileInfo (finalpath);

			extension = sourcefile.Extension.TrimStart ('.').ToLower();

			if (mimeTypes.TryGetString(extension, out mimeType)) {
				if (sourcefile.Exists) {
					parameters.ResponseHeaders.ContentType = new MimeType(mimeType);
					parameters.ResponseHeaders.ContentLength = sourcefile.Length;

					FileStream sourceStream = sourcefile.OpenRead();
					sourceStream.CopyTo(parameters.ResponseBody.BaseStream);
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

