using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using PushSharp.Core;

namespace PushNotifications
{
	public class PushToApple : SingleBranchService
    {
		private ApnsConfiguration.ApnsServerEnvironment PushEnvironment;

		public override string Description {
			get {
				return "Push notification";	
			}
		}

		public string CertificateFile { get; private set; }
		public IServiceConnection<ApnsNotification> Connection { get; private set; }
		public List<string> PassVariables { get; private set; }
		public bool ContentAvailable { get; private set; }

		public override void LoadDefaultParameters(string defaultParameter)
		{
			
		}

		protected override void Initialize(Settings settings)
		{
			this.CertificateFile = settings.GetString("certfile");            
			Enum.TryParse<ApnsConfiguration.ApnsServerEnvironment>(settings.GetString("environment"), out this.PushEnvironment);


			X509Certificate2 certificate;

			if (settings.Has("passphrase")) {
				certificate = new X509Certificate2(CertificateFile, settings.GetString("passphrase"));
			} else {
				certificate = new X509Certificate2(CertificateFile);
			}

			this.ContentAvailable = settings.GetBool("content-available", false);

			var config = new ApnsConfiguration(this.PushEnvironment, certificate);

            var connectionFactory = new ApnsServiceConnectionFactory(config);
			this.Connection = connectionFactory.Create();
			this.PassVariables = settings.GetStringList("variablenames");

		}

		protected override bool Process(IInteraction parameters)
		{
			string devicetoken;
			if (parameters.TryGetFallbackString("devicetoken", out devicetoken)) {
				if (this.Branches.Has("_with")) {
					var composer = new StringComposeInteraction(parameters, Encoding.UTF8);
					if (WithBranch.TryProcess(composer)) {
						var finalNotification = new ApnsNotification(
							devicetoken, 
							JObject.Load(
								new JsonTextReader(
									new StringReader(
										composer.ToString()))));
						Connection.Send(finalNotification).Wait(-1);
				    }
				}            
			}               

			return true;
		}
	}
}
