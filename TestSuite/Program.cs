using System;
using System.IO;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using System.Diagnostics;
using Testing.Diff;

namespace TestSuite
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			SettingsComposer composer = new SettingsComposer ();

			TestAgainst (EmptyCase (composer), "EmptyCase.conf");
			TestAgainst (TypeCase (composer), "TypeCase.conf");
			TestAgainst (SingleCase (composer), "SingleCase.conf");
			TestAgainst (ArrayCase (composer), "ArrayCase.conf");
			TestAgainst (SubmapCase (composer), "SubmapCase.conf");
			TestAgainst (EndBoss(composer), "EndBoss.conf");
		}

		static Stream EmptyCase (SettingsComposer composer)
		{
			MemoryStream stream = new MemoryStream ();
			composer.ToStream (new Settings (), stream);
			stream.Position = 0;
			return stream;
		}

		static Stream TypeCase (SettingsComposer composer)
		{
			Settings data = new Settings ();

			data ["intje"] = 33;
			data ["booltje"] = true;
			data ["floatje"] = 33.1f;
			data ["emptyarr"] = (IEnumerable<object>)(new List<object> ());
			data ["emptydict"] = new Settings ();

			MemoryStream stream = new MemoryStream ();
			composer.ToStream (data, stream);
			stream.Position = 0;
			return stream;
		}

		static Stream SingleCase (SettingsComposer composer)
		{
			Settings data = new Settings ();
		
			data ["taart"] = 2;

			MemoryStream stream = new MemoryStream ();
			composer.ToStream (data, stream);
			stream.Position = 0;
			return stream;
		}

		static Stream SubmapCase (SettingsComposer composer)
		{
			Settings data = new Settings ();

			Settings taart = new Settings ();
			taart ["appel"] = "lekker";
			taart ["appelkruimel"] = "superlekker";
			taart ["kersen"] = "meh";
			taart ["cholade"] = 1337;

			data ["taart"] = taart;

			MemoryStream stream = new MemoryStream ();
			composer.ToStream (data, stream);
			stream.Position = 0;
			return stream;
		}

		static Stream ArrayCase (SettingsComposer composer)
		{
			Settings data = new Settings ();

			IEnumerable<object> taarten = new string[] { "kersen", "appel", "chocolade" };

			data ["taarten"] = taarten;

			MemoryStream stream = new MemoryStream ();
			composer.ToStream (data, stream);
			stream.Position = 0;
			return stream;
		}

		static Stream EndBoss (SettingsComposer composer)
		{
			Settings data = new Settings ();

			IEnumerable<object> plugins = new string[] { "/usr/lib/BasicHttpServer.dll", "/usr/lib/OutputComposing.dll" };
			data ["plugins"] = plugins;
				Settings instances = new Settings ();
					Settings firstInstance = new Settings ();
					firstInstance ["type"] = "HttpService";
					Settings modConf = new Settings ();
						IEnumerable<object> prefixes = new string[] { "http://localhost:8080/" };
						modConf ["prefixes"] = prefixes;
					firstInstance ["modconf"] = modConf;
					Settings underlyingInstance = new Settings ();
						underlyingInstance ["type"] = "Template";
						Settings templateConf = new Settings ();
							templateConf ["templatefile"] = "/var/www/index.html";
						underlyingInstance ["modconf"] = templateConf;
					firstInstance ["http_branch"] = underlyingInstance;
				instances ["firstinstance"] = firstInstance;
			data ["instances"] = instances;
			
			MemoryStream stream = new MemoryStream ();
			composer.ToStream (data, stream);
			stream.Position = 0;
			return stream;
		}

		public static void TestAgainst(Stream data, string verifile) {			
			Console.WriteLine (verifile);
			DiffSession session = new DiffSession ();

			session.SetInput (verifile, data);

			while (!session.GetOutputReader().EndOfStream) {
				Console.WriteLine (session.GetOutputReader ().ReadLine ());
			}
		}
	}
}
