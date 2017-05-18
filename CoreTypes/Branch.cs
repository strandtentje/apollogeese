using System;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class Branch
	{
		public IInteraction Interaction { get; private set; }

		public bool Success { get; private set; }

		public static Branch For(IInteraction interaction) 
		{
			return new Branch() { Interaction = interaction, Success = true };
		}

		public Branch Optional(params Service[] services)
		{
			foreach (var service in services) {
				Success &= (service == null) || service.TryProcess(Interaction);
			}
			return this;
		}

		public bool Finish() 
		{
			return Success;
		}
	}
}

