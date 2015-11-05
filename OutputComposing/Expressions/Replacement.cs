using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utensils.Collections;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.ApolloGeese.Http.Headers;
using BorrehSoft.Utensils.Log;
using System.Collections.Generic;
using System.Web;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	class Replacement : NamedExpression
	{
		public Func<string, string> Transcode;

		public Replacement(string rawExpression)
		{
			this.Expression = rawExpression;
			this.Transcode = delegate(string poop) {
				return poop;
			};
		}

		public Replacement (string rawExpression, Func<string, string> transcode)
		{
			this.Expression = rawExpression;
			this.Transcode = transcode;
		}

		public override bool TryWriteTo (StreamWriter writer, IInteraction context) {
			object value;
			if (context.TryGetFallback (this.Name, out value)) {
				writer.Write (Transcode (value.ToString ()));
				writer.Flush ();
				return true;
			}
			return false;
		}
	}
}

