using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module
{
	/// <summary>
	/// An interaction that is indicative for a jump from one module to another.
	/// </summary>
	public class JumpInteraction : QuickInteraction
	{
		/// <summary>
		/// Gets the branches of the originating module.
		/// </summary>
		/// <value>The branches.</value>
		public Map<Service> Branches { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.JumpInteraction"/> class using the
		/// originating interaction, the module call branches and the settings passed as parameters to this module.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="branches">Branches.</param>
		/// <param name="modSettings">Mod settings.</param>
		public JumpInteraction(IInteraction parent, Map<Service> branches, Settings modSettings) : base(parent, modSettings)
		{
			this.Branches = branches;

			object reassignObject = modSettings["reassignments"] ?? modSettings["remap"];

			if (reassignObject != null) {
				Settings reassignments = (Settings)reassignObject;
				object value;
				foreach (KeyValuePair<string, object> pair in reassignments.Dictionary) {
					string sourceName = pair.Value as string;
					if (parent.TryGetFallback (sourceName, out value)) 
						this [pair.Key] = value;
				}						
			}
		}

		public bool TryGetDeepBranch (string branchName, out Service deepBranch)
		{
			try {
				deepBranch = GetDeepBranch(branchName);
				return true;
			} catch(JumpException ex) {
				deepBranch = null;
				Secretary.Report (5, ex.Message);
			}

			return false;
		}

		public Service GetDeepBranch(string name) {
			Service branch = Branches [name];

			if (branch == null) {
				IInteraction nextJi;
				if (Parent.TryGetClosest (typeof(JumpInteraction), out nextJi)) {
					branch = ((JumpInteraction)nextJi).GetDeepBranch (name);
				} else {
					throw new JumpException (name);
				}
			} 

			return branch;
		}
	}
}
