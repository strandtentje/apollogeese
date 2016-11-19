using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utilities.Log;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module
{
	/// <summary>
	/// An interaction that is indicative for a jump from one module to another.
	/// </summary>
	public class JumpInteraction : SimpleInteraction
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
		public JumpInteraction(IInteraction parent, Map<Service> branches, Map<string> variableOverrides, Map<object> injectedVariables) : base(parent, injectedVariables.Clone())
		{
			this.Branches = branches;

			foreach (KeyValuePair<string, string> overrideVariable in variableOverrides.Dictionary) {
				string sourceName = overrideVariable.Value;
				object newValue;
				if (parent.TryGetFallback (sourceName, out newValue)) 
					this [overrideVariable.Key] = newValue;
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
