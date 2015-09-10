using System;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	/// <summary>
	/// Service mutator.
	/// </summary>
	public abstract class ServiceMutator : TwoBranchedService
	{
		/// <summary>
		/// Gets the service id int from an interaction
		/// </summary>
		/// <returns>The service int.</returns>
		/// <param name="parameters">Parameters.</param>
		/// <param name="key">Key.</param>
		protected int GetServiceInt (IInteraction parameters, string key)
		{
			object candidateValue;

			if (parameters.TryGetFallback (key, out candidateValue)) {
				if (candidateValue is int) {
					return (int)candidateValue;
				} else {
					if (candidateValue is string) {
						int candidateInt;
						if (int.TryParse ((string)candidateValue, out candidateInt)) {
							return candidateInt;
						} else {
							throw new ControlException (ControlException.Cause.IntParse, key);
						}
					} else {
						throw new ControlException (ControlException.Cause.TypeMismatch, key);
					}
				}
			} else {
				throw new ControlException (ControlException.Cause.NoCandidate, key);
			}
		}

		/// <summary>
		/// Gets a service by its numeric id.
		/// </summary>
		/// <returns>The service.</returns>
		/// <param name="serviceID">Service ID</param>
		protected Service GetServiceById(int serviceID) {
			if (ModelLookup.ContainsKey (serviceID)) {
				if (ModelLookup [serviceID].GetSettings () == null) {
					throw new ControlException (ControlException.Cause.Uninitialized, serviceID.ToString ());
				} else {
					return ModelLookup [serviceID];
				}
			} else {
				throw new ControlException (ControlException.Cause.NoService, serviceID.ToString());
			}
		}
	}
}

