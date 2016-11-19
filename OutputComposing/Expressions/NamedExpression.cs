using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utilities.Collections;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using BorrehSoft.Utilities.Log;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	abstract class NamedExpression : Expression
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
	}
}

