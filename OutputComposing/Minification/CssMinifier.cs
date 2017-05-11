using System;
using BorrehSoft.Utilities.Collections.Settings;
using Yahoo.Yui.Compressor;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class CssMinifier : Minifier
	{
		public override string Description {
			get {
				return "CSS Minifier";
			}
		}

		protected override void Initialize (Settings settings)
		{
			base.compressor = new CssCompressor ();
		}
	}
}

