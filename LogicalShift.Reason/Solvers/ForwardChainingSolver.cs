using Logicalshift.Reason.Api;
using Logicalshift.Reason.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logicalshift.Reason.Solvers
{
    /// <summary>
    /// Implements the forward-chaining solving algorihtm against a knowledge base
    /// </summary>
    public class ForwardChainingSolver : ISolver
    {
        private readonly IKnowledgeBase _knowledge;

        public ForwardChainingSolver(IKnowledgeBase knowledge)
        {
            if (knowledge == null) throw new ArgumentNullException("knowledge");

            _knowledge = knowledge;
        }

        /// <summary>
        /// True if all of the negative literals in a clause are solved
        /// </summary>
        private bool NegativesAreSolved(IClause clause, HashSet<ILiteral> solved)
        {
            return clause.If.All(literal => solved.Contains(literal));
        }

        /// <summary>
        /// Returns the index of the first element in an enumerable matching the predicate
        /// </summary>
        private int FirstIndexOf<TElement>(IEnumerable<TElement> values, Func<TElement, bool> predicate)
        {
            int index = 0;
            foreach (var item in values)
            {
                if (predicate(item))
                {
                    return index;
                }
                ++index;
            }

            return -1;
        }

        public Task<IQueryResult> Solve(IEnumerable<ILiteral> goals)
        {
            if (goals == null) throw new ArgumentNullException("goals");

            var remainingGoals = new HashSet<ILiteral>();
            var solved = new HashSet<ILiteral>();

            // Initially, all the clauses are unsolved
            var unsolved = new List<IClause>(_knowledge.Clauses);

            // Set up the initial set of goals
            remainingGoals.UnionWith(goals);

            // Initially, only 'true' is solved
            solved.Add(Literal.True());

            // Iterate until we run out of clauses
            for (;;)
            {
                // Pick a clause such that all its negatives are solved
                var firstSolvedIndex = FirstIndexOf(unsolved, clause => clause.Implies != null && NegativesAreSolved(clause, solved));

                if (firstSolvedIndex < 0)
                {
                    // The goal is not solvable
                    return Task.FromResult<IQueryResult>(new BasicQueryResult(false));
                }

                // This element is no longer unsolved
                var solvedElement = unsolved[firstSolvedIndex];
                solved.Add(solvedElement.Implies);
                unsolved.RemoveAt(firstSolvedIndex);

                // Remove from the goals if it matched
                if (remainingGoals.Contains(solvedElement.Implies))
                {
                    remainingGoals.Remove(solvedElement.Implies);
                    if (!remainingGoals.Any())
                    {
                        // If all the goals are solved, we're finished
                        return Task.FromResult<IQueryResult>(new BasicQueryResult(true));
                    }
                }
            }
        }
    }
}
