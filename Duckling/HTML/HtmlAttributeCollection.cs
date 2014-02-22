using System;
using BorrehSoft.Utensils;
using System.Text;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	/// <summary>
	/// Html attribute collection.
	/// </summary>
	public class HtmlAttributeCollection : List<HtmlAttribute>
	{
		public HtmlAttributeCollection (HtmlAttribute[] attributes)
		{
			this.AddRange(attributes);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="BorrehSoft.ApolloGeese.Duckling.HtmlAttributeCollection"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="BorrehSoft.ApolloGeese.Duckling.HtmlAttributeCollection"/>.</returns>
	    public override string ToString ()
		{
			StringBuilder attListBuilder = new StringBuilder ();
			foreach (HtmlAttribute attribute in this)
				attListBuilder.Append (attribute.ToString ());
			return attListBuilder.ToString ();
		}
	}
}
