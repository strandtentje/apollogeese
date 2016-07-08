using System;
using System.Collections.Generic;
using Parsing;
using System.IO;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;

namespace ExternalData
{
	public abstract class HttpForm<T> : NameValueService
	{
		TimeSpan parsingTimeout;

		protected List<string> StringFieldWhiteList { get; private set; }

		protected NameValuePiper<TextReader, T> ParserRunner { get; private set; }

		protected bool EmptyNull { get; private set; }

		public TimeSpan ParsingTimeout { 
			get {
				return this.parsingTimeout;
			}
			set {
				this.parsingTimeout = value;
				this.ParserRunner = new NameValuePiper<TextReader, T> (UrlParseReader, this.parsingTimeout);
			}
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);

			this.StringFieldWhiteList = settings.GetStringList ("fieldlist");
			this.ParsingTimeout = TimeSpan.Parse (settings.GetString ("timeout", "00:00:00.5"));
			this.EmptyNull = settings.GetBool ("emptynull", true);

			if (this.StringFieldWhiteList.Count == 0) {
				Secretary.Report (5, "Fieldlist Empty line:", this.ConfigLine.ToString());
			}
		}

		protected abstract void UrlParseReader(TextReader reader, NameValuePiper<TextReader, T>.NameValueCallback callback);
	}
}

