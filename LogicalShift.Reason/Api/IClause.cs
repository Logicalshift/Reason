using System.Collections.Generic;

namespace Logicalshift.Reason.Api
{
    /// <summary>
    /// Interface implemented by objects representing a Horn clause
    /// </summary>
    /// <remarks>
    /// A Horn clause is of the form 'not p1 or not p2 or p3', which can also be written as 'p1, p2 => p3'
    /// or 'if p1 and p2 then p3'.
    /// </remarks>
    public interface IClause
    {
        /// <summary>
        /// Literals that must be true for this clause
        /// </summary>
        IEnumerable<ILiteral> If { get; }

        /// <summary>
        /// Literal that is true if the 'If' literals are also true. Can be null to specify a negative Horn clause.
        /// </summary>
        ILiteral Implies { get; }
    }
}
