using Logicalshift.SLD.Api;
using Logicalshift.SLD.Solvers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logicalshift.SLD
{
    /// <summary>
    /// Methods for creating and altering solvers
    /// </summary>
    public static class Solver
    {
        /// <summary>
        /// Creates a goal solver that works against the specified knowledge base
        /// </summary>
        public static ISolver NewSolver(this IKnowledgeBase knowledge)
        {
            return new BackwardChainingSolver(knowledge);
        }

        /// <summary>
        /// Uses a solver to query for a specific goal
        /// </summary>
        public static Task<IQueryResult> Solve(this ISolver solver, ILiteral goal)
        {
            if (goal == null) throw new ArgumentNullException("goal");

            return solver.Solve(new[] { goal });
        }

        /// <summary>
        /// Uses a solver to query for a specific goal
        /// </summary>
        public static Task<IQueryResult> Solve(this ISolver solver, params ILiteral[] goals)
        {
            if (goals == null) throw new ArgumentNullException("goals");

            return solver.Solve((IEnumerable<ILiteral>) goals);
        }
    }
}
