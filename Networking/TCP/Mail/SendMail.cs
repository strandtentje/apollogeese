using System;
using System.Net.Mail;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.ComponentModel;
using System.Collections.Generic;

namespace Networking
{
	/// <summary>
	/// Send mail.
	/// </summary>
	public class SendMail : Service
	{
		IEnumerable<string> mailServers;
		string sender = null, recepient = null, replyto = null, bodytypename;
		string subject = null;
		Service body, sending, sent;
		SmtpClient smtpClient;

		public override string Description {
			get {
				return string.Format (
					"send {0} from {1} to {2} via {3}", 
					bodytypename, 
					sender ?? "contextual sender", 
					recepient ?? "contextual recepient", 
					smtpClient.Host);
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "body") body = e.NewValue ?? Stub;
			if (e.Name == "sending") sending = e.NewValue ?? Stub;
			if (e.Name == "sent") sent = e.NewValue ?? Stub;
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches ["body"] = Stub; Branches ["sending"] = Stub; Branches ["sent"] = Stub;

			if (!modSettings.TryGetString ("from", out sender))	sender = null;
			if (!modSettings.TryGetString ("to", out recepient)) recepient = null;
			if (!modSettings.TryGetString ("subject", out subject)) subject = null;
			if (!modSettings.TryGetString ("replyto", out replyto))	replyto = null;

			bodytypename = modSettings.GetString ("bodyname", "emailbody");

			smtpClient = SmtpPicker.GetClient (modSettings.GetStringList ("mailservers", "localhost"));
			smtpClient.SendCompleted += HandleSendCompleted;
		}

		protected override bool Process (IInteraction parameters)
		{
			// prepare composing interaction
			EmailInteraction composeInteraction = new EmailInteraction (
				parameters, bodytypename, sender, recepient, subject, replyto);

			// start composing
			bool success = body.TryProcess (composeInteraction);

			// start sending
			smtpClient.SendAsync (composeInteraction.GetFinishedMessage(), composeInteraction);

			// indicate sending
			return sending.TryProcess(parameters) && success;
		}

		void HandleSendCompleted (object sender, AsyncCompletedEventArgs e)
		{
			sent.TryProcess (e.UserState as IInteraction);
		}
	}
}

