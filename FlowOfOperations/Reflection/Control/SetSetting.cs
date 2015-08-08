using System;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Parsing;
using BorrehSoft.Utensils.Parsing.Parsers;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class SetSetting : ServiceMutator
	{
		public override string Description {
			get {
				return "Single setting setter";
			}
		}

		string serviceIdKey;

		static AnyParser valueParser = new AnyParser (new Parser[] {			
			new ValueParser<int> (int.TryParse), 
			new ValueParser<long> (long.TryParse),
			new ValueParser<float> (float.TryParse),
			new ValueParser<bool> (bool.TryParse, "(True|False|true|false)"),
			new ConcatenationParser ('[', ']', ','),
			new SettingsParser()
		});

		protected override void Initialize (Settings modSettings)
		{			
			serviceIdKey = modSettings.GetString ("serviceidkey", "serviceid");
		}

		private object GetParsedValue (string serialValue)
		{  
			ParsingSession session = new ParsingSession(serialValue, new WhitespaceParser());

			object value;

			if (valueParser.Run(session, out value) < 0) {	
				return serialValue;	
			} else {
				return value;		
			}
		}

		private KeyValuePair<string, object> GetKeyvalueFromContext (IInteraction parameters)
		{
			string key, serialValue;

			if (parameters.TryGetFallbackString("key", out key)) {
				if (parameters.TryGetFallbackString("value", out serialValue)) {
					return new KeyValuePair<string, object> (key, GetParsedValue(serialValue));
				} else {
					throw new ControlException(ControlException.Cause.NoCandidate, "value");
				}
			} else {
				throw new ControlException(ControlException.Cause.NoCandidate, "key");
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;

			try {
				int serviceId = GetServiceInt (parameters, serviceIdKey);
				Service service = GetServiceById(serviceId);

				KeyValuePair<string, object> pair = GetKeyvalueFromContext(parameters);
								
				service.GetSettings()[pair.Key] = pair.Value;

				successful &= Successful.TryProcess (parameters);
			} catch(ControlException ex) {
				successful &= Failure.TryProcess (new FailureInteraction (parameters, ex));
			}

			return successful;
		}
	}
}

