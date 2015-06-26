using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Represents a knowledge base
    /// </summary>
    public interface IKnowledgeBase
    {
        /// <summary>
        /// The set of clauses that make up this knowledge base
        /// </summary>
        Task<IEnumerable<IClause>> GetClauses();

        /// <summary>
        /// Retrieves the clauses that resolve to a particular literal (an empty list if there are none)
        /// </summary>
        Task<IEnumerable<IClause>> ClausesForLiteral(ILiteral literal);
    }
}
