using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace InputProcessing
{
	/// <summary>
	/// Value field.
	/// </summary>
	public abstract class ValueField<T> : Field<T> where T : IComparable
	{
		
		[Instruction("Minimal value for this field")]
		public T Min {
			get;
			set;
		}

		[Instruction("Maximal value for this field")]
		public T Max {
			get;
			set;
		}

		/// <summary>
		/// Proxy for T.TryParse
		/// </summary>
		/// <returns><c>true</c>, if parse was successful, <c>false</c> otherwise.</returns>
		/// <param name="serial">Serial data.</param>
		/// <param name="data">Data.</param>
		public abstract bool TryParse (object serial, out T data);

		protected Service TooLow {
			get {
				return Branches.Get ("toolow", this.Failure);
			}
		}

		protected Service TooHigh {
			get {
				return Branches.Get ("toohigh", this.Failure);
			}
		}

		/// <summary>
		/// Finds the action for value.
		/// </summary>
		/// <returns>The action for value.</returns>
		/// <param name="valueCandidate">Value candidate.</param>
		/// <param name="value">Value.</param>
		private Service FindActionForValue(object valueCandidate, out T value) {
			Service action;
			bool gotValue = false;

			value = this.Default;

			if (valueCandidate is T) {
				value = (T)valueCandidate;
				gotValue = true;
			} else if (TryParse (valueCandidate, out value)) {
				gotValue = true;
			} else {
				action = this.BadFormat;
			}

			if (gotValue || !this.IsRequired) {
				if (this.Min.CompareTo(value) > 0) {
					action = this.TooLow;
				} else if (this.Max.CompareTo(value) < 0) {
					action = this.TooHigh;
				} else {
					action = this.Successful;
				}
			} else {
				action = this.Missing;
			}

			return action;
		}
	}
}
