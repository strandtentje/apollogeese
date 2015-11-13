using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class Buffer : Service
	{
		public override string Description {
			get {
				return "buffers";
			}
		}

		public override void LoadDefaultParameters (object defaultParameter)
		{
			Settings ["buffersize"] = defaultParameter;
		}

		IInteraction[] buffer = new IInteraction[1];

		public int BufferSize {
			get { return buffer.Length;}
			set {
				buffer = new IInteraction[value];
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.BufferSize = settings.GetInt ("buffersize");
		}
	}
}

