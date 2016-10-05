using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Threading.Tasks;
using BorrehSoft.Utensils.Collections.Maps;

namespace Filesystem
{
	public class ReadFromFile : FileService
	{
		public override string Description {
			get {
				return "Read" + base.Description;
			}
		}

		private Service Header = Stub;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			switch (e.Name) {
			case "header":
				this.Header = e.NewValue ?? Stub;
				break;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			var dataSink = Closest<IOutgoingBodiedInteraction>.From (parameters);

			FileInfo file = GetFileInfo (parameters);

			if (!file.FullName.StartsWith (this.ValidRootPath)) {
				return FailForException (parameters, new Exception ("File not in root path"));
			}

			IInteraction fileParameters = new SimpleInteraction (
				                              parameters, "contentlength", file.Length);

			if (!this.Header.TryProcess (fileParameters)) {
				return FailForException (fileParameters, new Exception ("Header failed"));
			}

			try {	
				using (FileStream fileStream = file.OpenRead ()) {
					Task copyTask = fileStream.CopyToAsync (dataSink.OutgoingBody);
					Meanwhile.TryProcess (fileParameters);
					copyTask.Wait ();
				}

				return (Successful == null) || Successful.TryProcess (fileParameters);
			} catch(Exception ex) {
				return FailForException (fileParameters, ex);
			}
		}
	}
}

