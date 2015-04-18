using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BorrehSoft.ApolloGeese.Duckling
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
