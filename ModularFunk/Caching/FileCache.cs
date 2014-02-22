using System;
using System.IO;
using System.Collections.Generic;

namespace BorrehSoft.Utensils
{
	/// <summary>
	/// Name-indexed cache of byte arrays.
	/// </summary>
	public class MemoryCache
	{
		struct Trash {
			int timeRemaining;
			string fileName;
		}

		List<Trash> garbageSchedule = new List<Trash>();

		public MemoryCache() {

		}

		/// <summary>
		/// The cache.
		/// </summary>
		Map<byte[]> cache = new Map<byte[]>();

		/// <summary>
		/// Gets a file.
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="fileName">File name.</param>
		/// <param name="lifeTime">Life time in minutes.</param>
		public byte[] GetFile(string fileName, int lifeTime = -1) {
			byte[] data = cache [fileName];
			if (data != null)
				return data;

			data = File.ReadAllBytes (fileName);
			cache [fileName] = data;
			return data;
		}


	}
}

