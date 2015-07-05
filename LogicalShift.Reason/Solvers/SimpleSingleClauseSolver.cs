using LogicalShift.Reason.Api;
using LogicalShift.Reason.Assignment;
using LogicalShift.Reason.Literals;
using LogicalShift.Reason.Unification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Represents a solver that can solve a single clause
    /// </summary>
    public class SimpleSingleClauseSolver : ISolver
    {
        /// <summary>
        /// The clause that this will solve
        /// </summary>
        private readonly IClause _clause;

        /// <summary>
        /// The solver that will be used for subclauses
        /// </summary>
        private readonly ISolver _subclauseSolver;

        private class AssignmentData
        {
            public ILiteral PredicateName { get; set; }
            public int NumArguments { get; set; }
            public IAssignmentLiteral[] Assignments { get; set; }
        }

        /// <summary>
        /// The assignments for each predicate in the clause (starting with the 'Implies' predicate)
        /// </summary>
        private readonly List<AssignmentData> _clauseAssignments;

        public SimpleSingleClauseSolver(IClause clause, ISolver subclauseSolver)
        {
            if (clause == null) throw new ArgumentNullException("clause");
            if (subclauseSolver == null) throw new ArgumentNullException("subclauseSolver");

            _clause = clause;
            _subclauseSolver = subclauseSolver;

            // Compile each part of the clause to its subclauses
            _clauseAssignments = new[] { clause.Implies }.Concat(clause.If)
                .Select(predicate => 
                {
                    var assignments = GetAssignmentsFromPredicate(predicate);
                    return new AssignmentData
                    {
                        PredicateName = predicate.UnificationKey,
                        NumArguments = assignments.CountArguments(),
                        Assignments = assignments.Assignments.ToArray()
                    };
                })
                .ToList();
        }

        /// <summary>
        /// Retrieves an object representing the assignments for a particular literal when used as a predicate
        /// </summary>
        private PredicateAssignmentList GetAssignmentsFromPredicate(ILiteral predicate)
        {
            var result = new PredicateAssignmentList();

            if (predicate.UnificationKey != null)
            {
                foreach (var argument in predicate.Dependencies)
                {
                    result.AddArgument(argument);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a function that solves a list of subclauses
        /// </summary>
        private Func<bool> SolveAllSubclauses(IEnumerable<AssignmentData> assignments, SimpleUnifier unifier)
        {
            // The first solution is always true once
            bool solvedOnce = false;
            Func<bool> solve = () =>
            {
                if (solvedOnce)
                {
                    // Reset the unifier to its original state and give up
                    unifier.ResetTrail();
                    return false;
                }
                else
                {
                    solvedOnce = true;
                    return true;
                }
            };

            // Chain the solutions for each subclause to generate the final result
            foreach (var clause in assignments)
            {
                solve = SolveSubclause(clause, unifier, solve);
            }

            // This generates the result
            return solve;
        }

        /// <summary>
        /// Solves a subclause
        /// </summary>
        private Func<bool> SolveSubclause(AssignmentData clause, SimpleUnifier unifier, Func<bool> solvePreviousClause)
        {
            // Create a new trail
            unifier.PushTrail();

            Func<bool> nextInThisClause = null;

            return () =>
            {
                if (nextInThisClause == null || !nextInThisClause())
                {
                    // Reset the state and get the solution for the previous clause
                    unifier.ResetTrail();

                    if (!solvePreviousClause())
                    {
                        return false;
                    }

                    // Push a new trail for this clause
                    unifier.PushTrail();

                    // Solve this clause
                    try
                    {
                        // Put the arguments for this clause
                        unifier.QueryUnifier.Bind(clause.Assignments);
                        unifier.QueryUnifier.Compile(clause.Assignments);
                    }
                    catch (InvalidOperationException)
                    {
                        // Failed to unify
                        return false;
                    }

                    // Call the predicate
                    nextInThisClause = _subclauseSolver.Call(clause.PredicateName, unifier.GetArgumentVariables(clause.NumArguments));

                    // Result depends on the next item in this clause
                    return nextInThisClause();
                }
                else
                {
                    // nextInThisClause() returned true, so this clause was solved
                    return true;
                }
            };
        }

        public Func<bool> Call(ILiteral predicate, params IReferenceLiteral[] arguments)
        {
            // Assume that predicate is correct

            // Load the arguments into a simple unifier
            var unifier = new SimpleUnifier();
            unifier.LoadArguments(arguments);

            // Unify using the predicate
            try
            {
                unifier.ProgramUnifier.Bind(_clauseAssignments[0].Assignments);
                unifier.ProgramUnifier.Compile(_clauseAssignments[0].Assignments);
            }
            catch (InvalidOperationException)
            {
                // Fail if we can't unify
                return () => false;
            }

            // Call using the clauses
            return SolveAllSubclauses(_clauseAssignments.Skip(1), unifier);
        }
    }
}
