using LogicalShift.Reason.Api;
using LogicalShift.Reason.Solvers;
using LogicalShift.Reason.Unification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicalShift.Reason
{
    /// <summary>
    /// Methods for creating and altering solvers
    /// </summary>
    public static class Solver
    {
        /// <summary>
        /// Creates a goal solver that works against the specified knowledge base
        /// </summary>
        public static ISolver NewSolver(this IKnowledgeBase knowledge, SolverStyle style)
        {
            switch (style)
            {
                case SolverStyle.BackwardsChaining:
                    return new BackwardChainingSolver(knowledge);

                case SolverStyle.ForwardsChaining:
                    return new ForwardChainingSolver(knowledge);

                default:
                    throw new InvalidOperationException("Unknown type of solver");
            }
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

        /// <summary>
        /// Queries a solver for a goal
        /// </summary>
        public static Func<bool> Query(this ISolver solver, ILiteral goal)
        {
            // Result is false for something without a key
            if (goal.UnificationKey == null)
            {
                return () => false;
            }

            // Compile the query
            var unifier = new SimpleUnifier();
            var assignments = new PredicateAssignmentList();

            // Assume we have a basic predicate
            foreach (var arg in goal.Dependencies)
            {
                assignments.AddArgument(arg);
            }

            // Run through the unifier
            unifier.QueryUnifier.Compile(assignments.Assignments);

            // Call via the solver
            return solver.Call(goal.UnificationKey, unifier.GetArgumentVariables(assignments.CountArguments()));
        }
    }
}
