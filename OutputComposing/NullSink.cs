﻿using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class NullSink : SingleBranchService
	{
		public override string Description {
			get {
				return "nullsink outgoing data";
			}
		}

		bool log = false;

		protected override void Initialize (Settings settings)
		{
			log = settings.GetBool ("log", false);
		}

		protected override bool Process (IInteraction parameters)
		{
			StringComposeInteraction composer = new StringComposeInteraction (parameters, Encoding.UTF8);

			bool success = WithBranch.TryProcess (composer);

			if (log) {
				Secretary.Report (5, composer.ToString ());
			}

			return success;
		}
	}
}

