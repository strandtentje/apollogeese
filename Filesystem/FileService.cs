using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.IO;
using BorrehSoft.Utensils.Collections.Maps;

namespace Filesystem
{
	public abstract class FileService : TwoBranchedService
	{
		public override string Description {
			get {
				return string.Format (
					"Accessess files {0} {1} based on {2}",
					(this.IsRelative ? "relative to" : "in"),
					this.ValidRootPath,
					this.PathVariable);
			}
		}

		public string PathVariable { get; set; }

		public string ValidRootPath { get; set; }

		public bool IsRelative { get; set; }

		protected Service Meanwhile = Stub;

		protected override void Initialize (Settings settings)
		{
			this.PathVariable = settings.GetString ("pathvariable", "filepath");
			this.ValidRootPath = settings.GetString ("rootpath");
			this.IsRelative = settings.GetBool ("isrelative", false);
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);
			if (e.Name == "meanwhile") {
				this.Meanwhile = e.NewValue ?? Stub;
			}
		}

		protected FileInfo GetFileInfo(IInteraction parameters)
		{
			string providedFilename = Fallback<string>.From (parameters, this.PathVariable);
			string fileName;

			if (IsRelative) {
				fileName = string.Format (
					"{0}{1}{2}",
					this.ValidRootPath.TrimEnd ('\\', '/'),
					Path.DirectorySeparatorChar,
					providedFilename
				);
			} else {
				fileName = providedFilename;
			}

			return new FileInfo (fileName);
		}
	}
}

