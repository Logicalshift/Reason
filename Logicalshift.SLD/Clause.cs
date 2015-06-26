using Logicalshift.SLD.Api;
using Logicalshift.SLD.Clauses;
using System;
using System.Linq;

namespace Logicalshift.SLD
{
    /// <summary>
    /// Methods for creating an altering clauses
    /// </summary>
    public static class Clause
    {
        /// <summary>
        /// Creates a new negative Horn clause
        /// </summary>
        public static IClause If(params ILiteral[] literals)
        {
            if (literals == null) throw new ArgumentNullException("literals");
            if (literals.Any(literal => literal == null)) throw new ArgumentException("Null literals are not allowed", "literals");

            return new NegativeClause(literals);
        }

        /// <summary>
        /// Adds a positive literal to a negative Horn clause
        /// </summary>
        public static IClause Then(this IClause negativeHornClause, ILiteral then)
        {
            if (negativeHornClause == null) throw new ArgumentNullException("negativeHornClause");
            if (negativeHornClause.Implies != null) throw new ArgumentException("Clause already has an implication", "negativeHornClause");
            if (then == null) throw new ArgumentNullException("then");

            return new PositiveClause(negativeHornClause, then);
        }
    }
}
