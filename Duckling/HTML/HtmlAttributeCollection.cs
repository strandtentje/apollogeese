using System;
using System.Text;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	/// <summary>
	/// Html attribute collection.
	/// </summary>
	public class HtmlAttributeCollection : SerializingMap<object>
	{
		/// <summary>
		/// Writes the attributes using a callback
		/// </summary>
		/// <param name='WriteMethod'>
		/// Write method.
		/// </param>
		public void WriteUsingCallback(FormattedWriter WriteMethod)
		{
			WriteUsingCallback(WriteMethod, " {0}=\"{1}\"");		
		}

		/// <summary>
		/// Gets or sets the attribute with the specified name. 
		/// </summary>
		/// <param name='name'>
		/// Name
		/// </param>
		new public object this [string name] {
			get {
				return base[name.ToLower()];
			}
			set {
				base[name.ToLower()] = value;
			}
		}
	}
}
