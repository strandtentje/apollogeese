using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections;
using System.Data;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	/// <summary>
	/// Interaction for query results.
	/// </summary>
	public class ResultInteraction : QuickInteraction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.Data.ResultInteraction"/> class.
		/// </summary>
		/// <param name="Parent">Parent.</param>
		/// <param name="reader">Reader.</param>
		public ResultInteraction (IInteraction Parent, IDataReader reader) : base(Parent)
		{
			for(int i = 0; i < reader.FieldCount; i++)
				this[reader.GetName(i)] = reader.GetValue(i);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="BorrehSoft.Extensions.BasicWeblings.Data.ResultInteraction"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="BorrehSoft.Extensions.BasicWeblings.Data.ResultInteraction"/>.</returns>
		public override string ToString ()
		{
			string[] keys = new string[base.Dictionary.Count];
			base.Dictionary.Keys.CopyTo(keys, 0);
			return string.Join(",", keys);
		}	
	}
}

