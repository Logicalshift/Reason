using Logicalshift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logicalshift.Reason.Knowledge
{
    /// <summary>
    /// Knowledge base that appends a single clause to an existing knowledge base
    /// </summary>
    public class ListKnowledgeBase : IKnowledgeBase
    {
        private readonly IClause _clause;
        private readonly ListKnowledgeBase _next;
        private Dictionary<ILiteral, IClause[]> _clausesForLiteral;

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
            if (_clausesForLiteral != null) return;

            _clausesForLiteral = Clauses
                .GroupBy(clause => clause.Implies)
                .ToDictionary(group => group.Key, group => group.ToArray());
        }

        public Task<IEnumerable<IClause>> GetClauses()
        {
            return Task.FromResult(Clauses);
        }

        public Task<IEnumerable<IClause>> ClausesForLiteral(ILiteral literal)
        {
            FillClauseCache();

            IClause[] result;

            if (!_clausesForLiteral.TryGetValue(literal, out result))
            {
                result = new IClause[0];
            }

            return Task.FromResult<IEnumerable<IClause>>(result);
        }
    }
}
