using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Threading.Tasks;

namespace Filesystem
{
	public class ReadFromFile : FileService
	{
		public override string Description {
			get {
				return "Read" + base.Description;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			var dataSink = Closest<IOutgoingBodiedInteraction>.From (parameters);
			FileInfo file = GetFileInfo (parameters);

			if (!file.FullName.StartsWith (this.ValidRootPath)) {
				return FailForException (parameters, new Exception ("File not in root path"));
			}

			try {				
				using (FileStream fileStream = file.OpenRead ()) {
					Task copyTask = fileStream.CopyToAsync (dataSink.OutgoingBody);
					Meanwhile.TryProcess (parameters);
					copyTask.Wait ();
				}

				return (Successful == null) || Successful.TryProcess (parameters);
			} catch(Exception ex) {
				return FailForException (parameters, ex);
			}
		}
	}
}

