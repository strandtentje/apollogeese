using System;
using System.IO;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

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

		static string EmptyCase (SettingsComposer composer)
		{
			return composer.Serialize (new Settings ());
		}

		static string TypeCase (SettingsComposer composer)
		{
			Settings data = new Settings ();

			data ["intje"] = 33;
			data ["booltje"] = true;
			data ["floatje"] = 33.0f;
			data ["emptyarr"] = (IEnumerable<object>)(new List<object> ());
			data ["emptydict"] = new Settings ();

			throw composer.Serialize (data);
		}

		static string SingleCase (SettingsComposer composer)
		{
			Settings data = new Settings ();
		
			data ["taart"] = 2;

			return composer.Serialize (data);
		}

		static string SubmapCase (SettingsComposer composer)
		{
			Settings data = new Settings ();

			Settings taart = new Settings ();
			taart ["appel"] = "lekker";
			taart ["appelkruimel"] = "superlekker";
			taart ["kersen"] = "meh";
			taart ["cholade"] = 1337;

			data ["taart"] = taart;

			return composer.Serialize (data);
		}

		static string ArrayCase (SettingsComposer composer)
		{
			Settings data = new Settings ();

			IEnumerable<object> taarten = new string[] { "kersen", "appel", "chocolade" };

			data ["taarten"] = taarten;

			return composer.Serialize (data);
		}

		static string EndBoss (SettingsComposer composer)
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

			return composer.Serialize (data);
		}

		public static void TestAgainst(string data, string verifile) {			
			Console.WriteLine (verifile);

			using (StreamReader reader = new StreamReader(File.OpenRead(verifile))) {
				string verificationData = reader.ReadToEnd ();

				if (data == verificationData) {
					Console.WriteLine ("All is good");
				} else {
					Console.WriteLine ("Burp");
				}
			}
		}
	}
}
