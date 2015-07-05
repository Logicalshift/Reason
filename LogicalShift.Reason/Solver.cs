﻿using LogicalShift.Reason.Api;
using LogicalShift.Reason.Solvers;
using LogicalShift.Reason.Unification;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static IQueryResult Query(this ISolver solver, ILiteral goal)
        {
            // Result is false for something without a key
            if (goal.UnificationKey == null)
            {
                return new BasicQueryResult(false, new EmptyBinding(null));
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
            var freeVariableNames = unifier.QueryUnifier.Bind(assignments.Assignments);
            var freeVariables = freeVariableNames.Select(variable => unifier.GetVariable(variable).Dereference());
            unifier.QueryUnifier.Compile(assignments.Assignments);

            // Call via the solver
            var moveNext = solver.Call(goal.UnificationKey, unifier.GetArgumentVariables(assignments.CountArguments()));

            Func<IQueryResult> nextResult = () =>
                {
                    // Update the variables to the next result
                    var succeeded = moveNext();

                    // Nothing to do if we didn't succeed
                    if (!succeeded)
                    {
                        return new BasicQueryResult(false, new EmptyBinding(null));
                    }

                    // Bind the variables
                    var variableValues = freeVariables
                        .Select(varRef => varRef.Freeze())
                        .Zip(freeVariableNames, (value, name) => new { Value = value, Name = name }).ToArray();
                    var binding = new BasicBinding(null, variableValues.ToDictionary(val => val.Name, val => val.Value));

                    // Return the result
                    return new BasicQueryResult(true, binding);
                };

            // Chain to produce the final result
            return new ChainedResult(nextResult(), () => Task.FromResult(nextResult()));
        }
    }
}
