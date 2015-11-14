using System;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
    /// <summary>
    /// For objects with three (no more, no less) float parameters. Typically
    /// Services but do whatever.
    /// </summary>
    public interface I3DParameterized
    {
        void SetParameters(float P, float Q, float R);
    }
}
