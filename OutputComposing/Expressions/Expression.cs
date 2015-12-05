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
using BorrehSoft.Utensils.Log;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public abstract class Expression
	{
		public abstract bool TryWriteTo (StreamWriter writer, IInteraction context);

		public static Expression FromString(string data, ref int position, Func<string, Expression> callback) {			
			Expression result;
			string expression = "";
			char opener = '\0';
			int nextSymbol;

			nextSymbol = data.IndexOf ("{% ", position);
			if (position > nextSymbol) {
				result = new Literal (data.Substring (position));
				position = data.Length;
			} else if (nextSymbol > position) {
				result = new Literal (data.Substring (position, nextSymbol - position));
				position = nextSymbol;
			} else {
				position += 3;
				nextSymbol = data.IndexOf (" %}", position);
				if (position > nextSymbol) {
					throw new Exception (string.Format ("Missing closing %} for position {0}", position));
				} else if (nextSymbol > position) {
					expression = data.Substring (position, nextSymbol - position);
					position = nextSymbol + 3;
					result = callback (expression);
				} else {
					throw new Exception (string.Format ("Missing name for replacement at {0}", position));
				}
			}

			return result;
		}
	}
}

