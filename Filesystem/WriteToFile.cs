using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.IO;
using System.Threading.Tasks;
using BorrehSoft.Utilities.Collections.Maps;

namespace Filesystem
{
	public class WriteToFile : FileService
	{
		public override string Description {
			get {
				return "Write" + base.Description;
			}
		}

		public bool MayOverwrite { get; set; }

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);

			this.MayOverwrite = settings.GetBool ("overwrite", false);
		}

		protected override bool Process (IInteraction parameters)
		{
			var dataSource = Closest<IIncomingBodiedInteraction>.From (parameters);
			FileInfo file = GetFileInfo (parameters);

			if (!file.FullName.StartsWith (this.ValidRootPath)) {
				return FailForException (parameters, new Exception ("File not in root path"));
			}

			if (file.Exists && !MayOverwrite) {
				return FailForException (parameters, new Exception ("File already exists"));
			}

			try {				
				using (FileStream fileStream = file.OpenWrite()) {					
					Task copyTask = dataSource.IncomingBody.CopyToAsync (fileStream);
					this.Meanwhile.TryProcess (parameters);
					copyTask.Wait ();
				}
				return (Successful == null) || Successful.TryProcess (parameters);				
			} catch (Exception ex) {
				return FailForException (parameters, ex);
			}
		}
	}
}

