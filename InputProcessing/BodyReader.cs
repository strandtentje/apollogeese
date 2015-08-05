using System;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections.Maps;
using System.Web;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	/// <summary>
	/// Reads field values from user input suffixed to the request URl, after the question mark (?)
	/// </summary>
	public class BodyReader : FieldReader
	{
		static char assigner = '=';
		static char concatenator = '&';

		public override Map<object> GetParseableInput (IInteraction parameters)
		{
			IInteraction incomingInteraction;
			Map<object> input = new Map<object> ();

			if (parameters.TryGetClosest (typeof(IIncomingBodiedInteraction), out incomingInteraction)) {
				IIncomingBodiedInteraction incomingBody = (IIncomingBodiedInteraction)incomingInteraction;

				MapParser.ReadIntoMap (incomingBody.IncomingBody, '=', '&', ref input);
			} else {
				throw new Exception ("No incoming body found to read from.");
			}

			return input;
		}

		protected override string GetSourceName (IInteraction parameters)
		{
			return "http-post-form";
		}
	}
}

