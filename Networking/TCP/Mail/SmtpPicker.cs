using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Sockets;
using BorrehSoft.Utensils.Log;

namespace Networking
{
	public static class SmtpPicker
	{
		public static SmtpClient GetClient(IEnumerable<string> servers) {
			foreach (string hostname in servers) {
				using (TcpClient client = new TcpClient()) {
					IAsyncResult result = client.BeginConnect (
						hostname, 25, null, null);

					if (result.AsyncWaitHandle.WaitOne (300, false)) {
						Secretary.Report (5, "Found smtp on", hostname, "!");
						return new SmtpClient (hostname);
					} else {
						Secretary.Report (5, "No smtp on", hostname);
					}
				}
			}

			throw new MailException (servers);
		}
	}
}

