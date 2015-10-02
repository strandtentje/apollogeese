using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;
using BorrehSoft.Utensils.Collections.Settings;

namespace BetterData
{
	class Connector : Service
	{
		public string Name {
			get;
			set;
		}

		public override string Description {
			get {
				return string.Format(
					"Definition for connector {0}", this.Name);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["name"] = defaultParameter;
		}

		protected override void Initialize (Settings settings)
		{

		}

		public static IDbConnection Find (string name)
		{

		}
	}


}

