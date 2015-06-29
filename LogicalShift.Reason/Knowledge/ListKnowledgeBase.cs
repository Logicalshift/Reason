using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Knowledge
{
    /// <summary>
    /// Knowledge base that appends a single clause to an existing knowledge base
    /// </summary>
    public class ListKnowledgeBase : IKnowledgeBase
    {
        private readonly IClause _clause;
        private readonly ListKnowledgeBase _next;
        private Dictionary<ILiteral, IClause[]> _unificationCandidates;

        private ListKnowledgeBase(IClause ourClause, ListKnowledgeBase next)
        {
            _clause = ourClause;
            _next = next;
        }

        private ListKnowledgeBase CreateKnowledgeList(IKnowledgeBase notList)
        {
            var result = notList as ListKnowledgeBase;
            if (result != null)
            {
                return result;
            }

            result = null;
            foreach (var clause in notList.GetClauses().Result)
            {
                result = new ListKnowledgeBase(clause, result);
            }

            return result;
        }

        public ListKnowledgeBase(IClause newClause, IKnowledgeBase next)
        {
            if (newClause == null) throw new ArgumentNullException("newClause");
            if (next == null) throw new ArgumentNullException("next");

            _clause = newClause;
            _next = CreateKnowledgeList(next);
        }

        public IEnumerable<IClause> Clauses
        {
            get 
            {
                for (var current = this; current != null; current = current._next)
                {
                    yield return current._clause;
                }
            }
        }

        private void FillClauseCache()
        {
            if (_unificationCandidates != null) return;

            _unificationCandidates = Clauses
                .Where(clause => clause.Implies.UnificationKey != null)
                .GroupBy(clause => clause.Implies.UnificationKey)
                .ToDictionary(group => group.Key, group => group.ToArray());
        }

        public Task<IEnumerable<IClause>> GetClauses()
        {
            return Task.FromResult(Clauses);
        }

        public Task<IEnumerable<IClause>> CandidatesForLiteral(ILiteral literal)
        {
            if (literal == null) throw new ArgumentNullException("literal");

            // Index by unification key
            FillClauseCache();

            // Result is all clauses if there is no unification key
            if (literal.UnificationKey == null)
            {
                return Task.FromResult(Clauses);
            }

            // Result is the clauses for this value's unification key
            IClause[] result;

            if (!_unificationCandidates.TryGetValue(literal.UnificationKey, out result))
            {
                result = new IClause[0];
            }

            return Task.FromResult<IEnumerable<IClause>>(result);
        }
    }
}
