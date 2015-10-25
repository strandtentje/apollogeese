using System;
using System.Net.Mail;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;

namespace Networking
{
	/// <summary>
	/// Send mail.
	/// </summary>
	public class SendMail : Service
	{
		Service body, sending, sent;
		SmtpClient smtpClient;
		IEnumerable<string> mailServers;

		[Instruction("E-mail address that gets used as sender address", null, true)]
		public string Sender { get; set; }

		[Instruction("E-mail address that e-mail will be sent to", null, true)]
		public string Recepient { get; set; }

		[Instruction("E-mail address that replies to the e-mail should be sent to", null, true)]
		public string ReplyTo { get; set; }

		[Instruction("Variable in context at which body will be stored")]
		public string BodyTypeName { get; set; }

		[Instruction("Subject of e-mail", null, true)]
		public string Subject { get; set; }

		public string MailServer {
			get {
				return smtpClient.Host;
			}
			set {

				if (Username.Length > 0) {
					smtpClient = new SmtpClient (value, 587);

					smtpClient.UseDefaultCredentials = false;

					smtpClient.Credentials = new System.Net.NetworkCredential (
						this.Username, this.Password);
				} else {
					smtpClient = new SmtpClient (value);
				}
			}
		}

		string Username {
			get;
			set;
		}

		string Password {
			get;
			set;
		}

		public override string Description {
			get {
				return string.Format (
					"send {0} from {1} to {2} via {3}", 
					BodyTypeName, 
					Sender ?? "contextual sender", 
					Recepient ?? "contextual recepient", 
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

			this.Sender = modSettings.GetString ("from", null);
			this.Recepient = modSettings.GetString ("to", null);
			this.Subject = modSettings.GetString ("subject", null);
			this.ReplyTo = modSettings.GetString ("replyto", null);
			this.BodyTypeName = modSettings.GetString ("bodyname", "emailbody");
			this.Username = modSettings.GetString ("username", "");
			this.Password = modSettings.GetString ("password", "");
			this.MailServer = modSettings.GetString ("server", "");

			smtpClient.SendCompleted += HandleSendCompleted;
		}

		protected override bool Process (IInteraction parameters)
		{
			// prepare composing interaction
			EmailInteraction composeInteraction = new EmailInteraction (
				parameters, BodyTypeName, Sender, Recepient, Subject, ReplyTo);

			// start composing
			bool success = body.TryProcess (composeInteraction);

			// start sending
			smtpClient.SendAsync (composeInteraction.GetFinishedMessage(), composeInteraction);

			// indicate sending
			return sending.TryProcess(parameters) && success;
		}

		void HandleSendCompleted (object sender, AsyncCompletedEventArgs e)
		{
			// phre.Release ();
			sent.TryProcess (e.UserState as IInteraction);
		}
	}
}

