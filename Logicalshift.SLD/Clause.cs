using Logicalshift.SLD.Api;
using System;

namespace Logicalshift.SLD
{
    /// <summary>
    /// Methods for creating an altering clauses
    /// </summary>
    public static class Clause
    {
        public static IClause If(params ILiteral[] literals)
        {
            throw new NotImplementedException();
        }

        public static IClause Then(this IClause negativeHornClause, ILiteral then)
        {
            throw new NotImplementedException();
        }
    }
}
