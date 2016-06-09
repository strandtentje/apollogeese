using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.IO;

namespace Filesystem
{
	public class WriteToFile : TwoBranchedService
	{
		public string PathVariable { get; set; }

		public string ValidRootPath { get; set; }

		public bool IsRelative { get; set; }

		protected override void Initialize (Settings settings)
		{
			this.PathVariable = settings.GetString ("pathvariable", "fullpath");
			this.ValidRootPath = settings.GetString ("rootpath");
			this.IsRelative = settings.GetBool ("isrelative", false);
		}

		protected override bool Process (IInteraction parameters)
		{
			IInteraction dataSource;

			var dataBody = Closest<IIncomingBodiedInteraction>.From (parameters);
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

			FileInfo file = new FileInfo (fileName);

			if (!file.FullName.StartsWith (this.ValidRootPath)) {
				return Failure.TryProcess (parameters);
			}

			using (FileStream fileStream = File.OpenWrite (fileName)) {
				dataBody.IncomingBody.CopyTo (fileStream);
			}

			return Successful.TryProcess (parameters);
		}
	}
}

