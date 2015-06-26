using LogicalShift.Reason.Api;
using LogicalShift.Reason.Literals;
using LogicalShift.Reason.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Depth-first solver that works backwards from the goal
    /// </summary>
    /// <remarks>
    /// This can get stuck in an infinite loop if presented with a self-referential loop of clauses
    /// (the forward chaining solver can't)
    /// </remarks>
    public class BackwardChainingSolver : ISolver
    {
        /// <summary>
        /// The knowledge base used by this solver
        /// </summary>
        private readonly IKnowledgeBase _knowledge;

        public BackwardChainingSolver(IKnowledgeBase knowledge)
        {
            if (knowledge == null) throw new ArgumentNullException("knowledge");

            _knowledge = knowledge;
        }

        public async Task<IQueryResult> Solve(IEnumerable<ILiteral> goals)
        {
            if (goals == null) throw new ArgumentNullException("goals");

            // Try each goal in turn
            foreach (var goal in goals)
            {
                var result = await SolveGoal(goal);

                if (!result.Success)
                {
                    return new BasicQueryResult(false);
                }
            }

            // Result is successful if all the goals could be resolved
            return new BasicQueryResult(true);
        }

        /// <summary>
        /// Attempts to solve for a single goal
        /// </summary>
        private async Task<IQueryResult> SolveGoal(ILiteral goal)
        {
            if (goal == null) new BasicQueryResult(false);

            // True is always true
            if (Equals(goal, TrueLiteral.Value))
            {
                return new BasicQueryResult(true);
            }

            // Solve for this goal
            var clauseList = await _knowledge.ClausesForLiteral(goal);
            foreach (var clause in clauseList)
            {
                // If we can solve the entire 'if' side of this clause, then the goal is true
                bool solved = true;
                foreach (var ifGoal in clause.If)
                {
                    var solution = await SolveGoal(ifGoal);
                    if (!solution.Success)
                    {
                        solved = false;
                        break;
                    }
                }

                if (solved)
                {
                    return new BasicQueryResult(true);
                }
            }

            // None of the goals worked out
            return new BasicQueryResult(false);
        }
    }
}
