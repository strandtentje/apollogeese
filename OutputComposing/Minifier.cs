﻿using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using Yahoo.Yui.Compressor;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public abstract class Minifier : SingleBranchService
	{
		protected ICompressor compressor;

		protected override bool Process (IInteraction parameters)
		{
			var processor = StringProcessorInteraction.From (parameters);
			var success = WithBranch.TryProcess (processor);
			processor.Run (this.compressor.Compress);
			return success;
		}
	}
}

