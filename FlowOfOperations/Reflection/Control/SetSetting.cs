using System;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Parsing;
using BorrehSoft.Utilities.Parsing.Parsers;
using System.Collections.Generic;
using BorrehSoft.Utilities.Parsing.Parsers.SettingsParsers;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class SetSetting : ServiceMutator
	{
		public override string Description {
			get {
				return "Single setting setter";
			}
		}

		[Instruction("Name of variable in context where id to target service is stored.", "serviceid")]
		public string ServiceIdKey { get; set; }

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
			ServiceIdKey = modSettings.GetString ("serviceidkey", "serviceid");
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
				int serviceId = GetServiceInt (parameters, ServiceIdKey);
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

