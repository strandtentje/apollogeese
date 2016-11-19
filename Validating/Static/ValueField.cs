using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;

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
		protected override bool CheckValid (object rawInput)
		{
			bool isValid = true;

			T processedValue;

			if (rawInput is T) {
				processedValue = (T)rawInput;
			} else if (!TryParse (rawInput, out processedValue)) {
				isValid = false;
			}

			if (isValid) {
				if (this.Min.CompareTo(processedValue) > 0) {
					isValid = false;
				} else if (this.Max.CompareTo(processedValue) < 0) {
					isValid = false;
				}
			}

			return isValid;
		}
	}
}

