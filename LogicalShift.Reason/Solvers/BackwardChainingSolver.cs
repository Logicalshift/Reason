using LogicalShift.Reason.Api;
using LogicalShift.Reason.Literals;
using LogicalShift.Reason.Results;
using LogicalShift.Reason.Unification;
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
                var result = await SolveGoal(goal, null);

                if (!result.Success)
                {
                    return new BasicQueryResult(false, null);
                }
            }

            // Result is successful if all the goals could be resolved
            return new BasicQueryResult(true, null);
        }

        /// <summary>
        /// Finds the solutions for a goal when applied against a particular clause (returns null if there are
        /// no solutions)
        /// </summary>
        private async Task<IQueryResult> SolveClause(ILiteral goal, IClause clause, IBindings inputBindings)
        {
            // Create the initial set of bindings from the clause
            var bindings = goal.Unify(clause.Implies, inputBindings);

            // Can't solve this clause if we can't unify
            if (bindings == null)
            {
                return null;
            }

            // Bind to the predicates
            foreach (var predicate in clause.If)
            {
                // Solve the new goal
                var solved = await SolveGoal(predicate, bindings);
                if (solved == null || !solved.Success)
                {
                    return null;
                }

                // Update the bindings to match the solution
                bindings = solved.Bindings;

                // TODO: if there are more results in the IQueryResult object, add extra solutions that continue with them
            }

            // Success
            return new BasicQueryResult(true, bindings);
        }

        /// <summary>
        /// Attempts to solve for a single goal
        /// </summary>
        private async Task<IQueryResult> SolveGoal(ILiteral goal, IBindings inputBindings)
        {
            if (goal == null) new BasicQueryResult(false, null);

            // True is always true
            if (Equals(goal, TrueLiteral.Value))
            {
                return new BasicQueryResult(true, new EmptyBinding(goal));
            }

            // Solve for this goal
            var clauseList = await _knowledge.CandidatesForLiteral(goal);
            foreach (var clause in clauseList)
            {
                var solved = await SolveClause(goal, clause, inputBindings);

                if (solved != null && solved.Success)
                {
                    return solved;
                    // TODO: continue with other clauses?
                }
            }

            // None of the goals worked out
            return new BasicQueryResult(false, null);
        }
    }
}
