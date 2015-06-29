using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Knowledge
{
    /// <summary>
    /// A knowledge base with no information in it
    /// </summary>
    public class EmptyKnowledgeBase : IKnowledgeBase
    {
        public IEnumerable<IClause> Clauses
        {
            get { yield break; }
        }

        public Task<IEnumerable<IClause>> CandidatesForLiteral(ILiteral literal)
        {
            return Task.FromResult<IEnumerable<IClause>>(new IClause[0]);
        }

        public Task<IEnumerable<IClause>> GetClauses()
        {
            return Task.FromResult(Clauses);
        }
    }
}
