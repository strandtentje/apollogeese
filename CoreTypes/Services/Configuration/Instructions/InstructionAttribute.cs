using System;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class InstructionAttribute : Attribute
	{
		public InstructionAttribute(string explanation) {

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.CoreTypes.InstructionAttribute"/> class.
		/// </summary>
		/// <param name="explanation">Explanation.</param>
		/// <param name="defaultValue">Default value.</param>
		public InstructionAttribute (string explanation, object defaultValue)
		{
		}
	}
}

