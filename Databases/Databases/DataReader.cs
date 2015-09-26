using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Data;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Databases
{
	public class DataReader : DataQuerier
	{
		Service first = Stub;
		Service iterator = Stub;
		Service none = Stub;
		Service single = Stub;

		protected override void HandleBranchChanged (object sender, BorrehSoft.Utensils.Collections.Maps.ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			if (e.Name == "first")
				this.first = e.NewValue;
			else if (e.Name == "iterator")
				this.iterator = e.NewValue;
			else if (e.Name == "none")
				this.none = e.NewValue;
			else if (e.Name == "single")
				this.single = e.NewValue;

		}

		public override IQueryStats ConsumeResults (IInteraction parent, IDataReader reader)
		{
			IInteraction result, firstResult; 
			bool success = true, isFirst = true;
			int resultCount = 0;

			while(reader.Read()) {
				result = new ResultInteraction(parent, reader);

				if (isFirst) {
					isFirst = false;
					firstResult = result;
				}

				success &= iterator.TryProcess(result);

				resultCount++;
			}

			return new QueryStats (resultCount, reader.RecordsAffected, firstResult);
		}

		protected override IQueryStats AttemptQuery (IInteraction ParentParameters)
		{
			bool success = true;

			IQueryStats stats = base.AttemptQuery (ParentParameters);


			success &= first.TryProcess(result);

			if (stats.RowCount == 0)
				success &= none.TryProcess (ParentParameters);

			if ((resultCount == 0) && (none != Stub)) success = none.TryProcess (ParentParameters);

			else if ((resultCount == 1) && (single != Stub)) success = single.TryProcess (firstResult);	


			if (recordsAffected == 0)
				success &= noneAffected.TryProcess (ParentParameters);
			if (recordsAffected == 1)
				success &= oneAffected.TryProcess (ParentParameters);
			if (recordsAffected > 1)
				success &= someAffected.TryProcess (ParentParameters);

			return success;
		}
	}
}

