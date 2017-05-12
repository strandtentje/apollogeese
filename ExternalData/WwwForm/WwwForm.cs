using System;
using System.IO;
using Parsing;
using System.Text;
using System.Web;

namespace ExternalData
{
	public class WwwForm : BriefForm<string>
	{
		public override string Description {
			get {
				return "URL Encoded data parser";
			}
		}

		private const char Concatenator = '&';
		private const char Assigner = '=';

		protected override void UrlParseReader (TextReader reader, NameValuePiper<TextReader, string>.NameValueCallback callback)
		{
			char currentCharacter;

			StringBuilder nameBuilder = new StringBuilder ();
			StringBuilder valueBuilder = new StringBuilder ();
			StringBuilder currentBuilder = nameBuilder;

			Action callBackForValueBuilder = delegate() {
				string value = HttpUtility.UrlDecode (valueBuilder.ToString ());
				if ((value.Length == 0) && this.EmptyNull) {
					callback (nameBuilder.ToString (), null);
				} else {
					callback (nameBuilder.ToString (), value);
				}				
			};

			while (reader.Peek () > -1) {
				currentCharacter = (char)reader.Read ();

				switch (currentCharacter) {
				case Concatenator:						
					callBackForValueBuilder ();
					nameBuilder.Clear ();
					valueBuilder.Clear ();
					currentBuilder = nameBuilder;
					break;
				case Assigner:
					currentBuilder = valueBuilder;
					break;
				default:
					currentBuilder.Append (currentCharacter);
					break;
				}
			}

			if (nameBuilder.Length > 0) {
				callBackForValueBuilder ();
			}
		}

	}
}

