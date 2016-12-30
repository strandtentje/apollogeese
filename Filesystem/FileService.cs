using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.IO;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Log;

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

		public bool UseVariable { get; set;	}

		protected Service Meanwhile = Stub;

		protected override void Initialize (Settings settings)
		{
			this.PathVariable = settings.GetString ("pathvariable", "filepath");
			this.ValidRootPath = settings.GetString ("rootpath");
			this.IsRelative = settings.GetBool ("isrelative", false);
			this.UseVariable = settings.GetBool ("usevariable", true);
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
			string fileName;

			if (this.UseVariable) {
				string providedFilename = Fallback<string>.From (parameters, this.PathVariable);
				string uncorrectedFilename = providedFilename;
				foreach (var invalidChar in Path.GetInvalidFileNameChars()) {
					providedFilename = providedFilename.Replace (invalidChar, '_');
				}
				if (providedFilename != uncorrectedFilename) {
					Secretary.Report (5, "Changed invalid provided filename", uncorrectedFilename, "into", providedFilename);
				}

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
			} else {
				fileName = this.ValidRootPath;
			}

			return new FileInfo (fileName);
		}
	}
}

