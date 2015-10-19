using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;

namespace InputProcessing
{
	public interface IRawInputInteraction : IIncomingReaderInteraction
	{
		/// <summary>
		/// Gets or sets the amount of inputs counted
		/// </summary>
		/// <value>The input count.</value>
		int InputCount { get; set; }

		/// <summary>
		/// Advances cursor to the next key-value pair.
		/// </summary>
		/// <returns><c>true</c>, if name was  read, <c>false</c> otherwise.</returns>
		bool ReadNextName ();

		/// <summary>
		/// Reads the raw input value that belongs to the current name
		/// </summary>
		/// <returns>The input.</returns>
		object ReadInput();

		/// <summary>
		/// Skips the input.
		/// </summary>
		void SkipInput();

		/// <summary>
		/// Gets or sets a value indicating whether this instance has values available.
		/// </summary>
		/// <value><c>true</c> if this instance has values available; otherwise, <c>false</c>.</value>
		bool HasValuesAvailable { get; set; }

		/// <summary>
		/// Gets or sets the name of the current field.
		/// </summary>
		/// <value>The name of the current.</value>
		string CurrentName { get; set; }

		/// <summary>
		/// Sets the processed value for the current name
		/// </summary>
		/// <param name="value">Value.</param>
		void SetProcessedValue (object value);

		/// <summary>
		/// Gets or sets feedback for input names
		/// </summary>
		/// <value>The feedback.</value>
		Map<Service> Feedback { get; set; }
	}

}

