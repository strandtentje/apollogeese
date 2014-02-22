using System;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	/// <summary>
	/// Html attribute.
	/// </summary>
	public class HtmlAttribute : BorrehSoft.Utensils.Tuple<string, string>
	{
		public HtmlAttribute (string key, string value) : base(key, value)
		{

		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="BorrehSoft.ApolloGeese.Duckling.HtmlAttribute"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="BorrehSoft.ApolloGeese.Duckling.HtmlAttribute"/>.</returns>
		public override string ToString ()
		{
			return string.Format (" {0}={1}", Key, Value);
		}
	}
}

