using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module
{
	/// <summary>
	/// Interaction intended to hop off at a certain branch name
	/// </summary>
	public class DirectedInteraction : SimpleInteraction, IInteraction
	{
		/// <summary>
		/// Gets the name of the branch to hop off at.
		/// </summary>
		/// <value>The name of the branch.</value>
		public string BranchName { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.DirectedInteraction"/> class using
		/// the originating interaction and the branch name.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="branch">Branch.</param>
		public DirectedInteraction (IInteraction parent, string branch) : base(parent)
		{
			this["branchname"] = BranchName = branch;
		}
	}
}

