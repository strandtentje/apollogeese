using System;

namespace Services.Simple
{
	public class Now : Future
	{
		public override void LoadDefaultParameters (string defaultParameter)
		{
			base.LoadDefaultParameters (string.Format("00:00:00>{0}", defaultParameter));
		}
	}
}

