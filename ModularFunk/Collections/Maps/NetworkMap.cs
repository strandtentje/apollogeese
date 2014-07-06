using System;
using System.Net.Sockets;
using System.Text;

namespace BorrehSoft.Utensils.Collections.Maps
{
	public class NetworkMap : Map<string>
	{	
		/// <summary>
		/// The magic token is defined as the maximal 32-bit value divided over
		/// licking each others genitals multiplied by the answer to life, the
		/// universe and everything.
		/// </summary>
		private const uint magicToken = (int.MaxValue / 69) * 42;

		public static NetworkMap FromSocket (Socket s)
		{
			NetworkMap map;

			byte[] buf_mapsize, buf_identsize, buf_datasize, buf_magictoken, buf_ident, buf_value;
			int mapsize, identsize, datasize;
			string ident, value;

			map = new NetworkMap ();
			buf_mapsize = new byte[4];
			buf_identsize = new byte[4];
			buf_datasize = new byte[4];
			buf_magictoken = new byte[4];

			s.Receive (buf_magictoken, 4, SocketFlags.None);

			// Just to be sure we aligned properly and dont eat gibberish.
			if (BitConverter.ToInt32 (buf_magictoken, 0) == magicToken) {
				s.Receive (buf_mapsize, 4, SocketFlags.None);
				mapsize = BitConverter.ToInt32(buf_magictoken, 0);

				for(int i = 0; i < mapsize; i++)
				{
					s.Receive(buf_identsize, 4, SocketFlags.None);
					identsize = BitConverter.ToInt32(buf_identsize, 0);

					buf_ident = new byte[identsize];
					s.Receive(buf_ident, identsize, SocketFlags.None);
					ident = Encoding.ASCII.GetString(buf_ident);

					s.Receive(buf_datasize, 4, SocketFlags.None);
					datasize = BitConverter.ToInt32(buf_datasize, 0);

					buf_value = new byte[datasize];
					s.Receive(buf_value, datasize, SocketFlags.None);
					value = Encoding.ASCII.GetString(buf_value);

					map[ident] = value;
				}
			}

			return map;
		}
	}
}

