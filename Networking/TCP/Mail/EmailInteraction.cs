using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;
using System.Net.Mail;

namespace Networking
{
	/// <summary>
	/// Email interaction.
	/// </summary>
	public class EmailInteraction : QuickOutgoingInteraction
	{
		/// <summary>
		/// Gets or sets the name of the body type, for identifying bodies in context
		/// </summary>
		/// <value>The name of the body type.</value>
		private string BodyTypeName { get; set; }
		/// <summary>
		/// Gets the key at which the sender of this email will be stored
		/// </summary>
		/// <value>From key.</value>
		private string FromKey { get { return string.Format ("{0}.from", BodyTypeName);}}
		/// <summary>
		/// Gets the key at which the recepient of this email will be stored
		/// </summary>
		/// <value>To key.</value>
		private string ToKey { get { return string.Format ("{0}.to", BodyTypeName);}}
		/// <summary>
		/// Gets the date key.
		/// </summary>
		/// <value>The date key.</value>
		private string DateKey { get { return string.Format ("{0}.date", BodyTypeName); } }
		/// <summary>
		/// Gets the subject key.
		/// </summary>
		/// <value>The subject key.</value>
		private string SubjectKey { get { return string.Format ("{0}.subject", BodyTypeName); } }

		public EmailInteraction(IInteraction parent, string emailTypeName, string sender, string recepient, string subject) : base(new MemoryStream(), parent)
		{
			this.BodyTypeName = emailTypeName;
			// pull sender and recepient from context, but only if they weren't given in constructor
			this [FromKey] = GetDefaultOrFallback ("from", sender);
			this [ToKey] = GetDefaultOrFallback ("to", recepient);
			this [SubjectKey] = GetDefaultOrFallback ("subject", subject);
			this [DateKey] = DateTime.Now;
		}

		private string GetDefaultOrFallback(string id, string def) {
			if (def == null) {
				if (TryGetFallbackString (id, out def))
					return def;
				else
					throw new MailException (id);
			} else {
				return def;
			}
		}

		/// <summary>
		/// Gets or sets the plain-text body of this email
		/// </summary>
		/// <value>The body.</value>
		public string Body {
			get { return this [BodyTypeName] as string; }
			set { this [BodyTypeName] = value; }
		}

		/// <summary>
		/// Gets the sender of this email
		/// </summary>
		/// <value>The sender.</value>
		public string Sender {
			get {
				return this [FromKey] as string;
			}
		}

		/// <summary>
		/// Gets the recepient of this email
		/// </summary>
		/// <value>The recepient.</value>
		public string Recepient {
			get {
				return this [ToKey] as string;
			}
		}

		/// <summary>
		/// Gets the subject.
		/// </summary>
		/// <value>The subject.</value>
		public string Subject {
			get {
				return this [SubjectKey] as string;
			}
		}

		/// <summary>
		/// Indicate all work with this instance is done. Write data to body.
		/// </summary>
		public override void Done ()
		{
			base.Done ();
			OutgoingBody.Position = 0;

			using (StreamReader reader = new StreamReader(OutgoingBody)) 
				this.Body = reader.ReadToEnd ();
		}

		/// <summary>
		/// Same as done, but return body.
		/// </summary>
		/// <returns>The finished body</returns>
		public string GetDone() {
			Done ();
			return this.Body;
		}

		/// <summary>
		/// Same as get done, but return a MailMessage with the sender and recepient set.
		/// </summary>
		/// <returns>The finished message.</returns>
		public MailMessage GetFinishedMessage() {
			MailMessage message = new MailMessage (Sender, Recepient);
			message.Subject = Subject;
			message.Body = GetDone ();
			return message;
		}
	}
}

