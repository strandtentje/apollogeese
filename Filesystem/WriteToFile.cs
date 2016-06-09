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

		protected override void Initialize (Settings settings)
		{
			this.PathVariable = settings.GetString ("pathvariable", "fullpath");
			this.ValidRootPath = settings.GetString ("rootpath");
		}

		protected override bool Process (IInteraction parameters)
		{
			IInteraction dataSource;

			var dataBody = Closest<IIncomingBodiedInteraction>.From (parameters);
			var fileName = Fallback<string>.From (parameters, this.PathVariable);

			using (FileStream fileStream = File.OpenWrite (fileName)) {
				dataBody.IncomingBody.CopyTo (fileStream);
			}
		}
	}
}

