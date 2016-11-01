using System;
using BorrehSoft.Utensils.Collections.Settings;
using Yahoo.Yui.Compressor;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public class JsMinifier : Minifier
	{
		public override string Description {
			get {
				return "JS Minifier";
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.compressor = new JavaScriptCompressor ();
		}
	}
}

