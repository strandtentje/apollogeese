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

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	abstract class NamedExpression : IExpression
	{
		string expression;

		public string Name {
			get;
			set;
		}

		protected string Expression {
			get {
				return this.expression;
			}
			set {
				this.expression = value;

				
				this.Name = expression.Trim (new char[] { ' ', '\t', '}', '%' });
			}
		}

		public abstract bool TryWriteTo (StreamWriter writer, IInteraction context);
	}
}

