using System;
using BorrehSoft.Utilities.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	public class Cache : AnonymousCache
	{
		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["name"] = defaultParameter;
		}

		string Name {
			get;
			set;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.Name = modSettings.GetString ("name");
			base.Initialize (modSettings);
		}

		private Map<byte[]> storage = new Map<byte[]> ();

		protected override byte[] Data {
			get {
				return storage [this.Name];
			}
			set {
				storage [this.Name] = value;
			}
		}
	}
}

