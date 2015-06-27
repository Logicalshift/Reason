using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason
{
    /// <summary>
    /// Helper methods for performing unification
    /// </summary>
    public static class BasicUnification
    {
        /// <summary>
        /// Returns the possible ways that a query term can unify with a program term
        /// </summary>
        public static IEnumerable<ILiteral> Unify(this ILiteral query, ILiteral program)
        {
            throw new NotImplementedException();
        }
    }
}
