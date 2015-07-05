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
            foreach (var clause in _clauseAssignments.Skip(1))
            {
                try
                {
                    // Put the arguments for this clause
                    unifier.QueryUnifier.Bind(clause.Assignments);
                    unifier.QueryUnifier.Compile(clause.Assignments);
                }
                catch (InvalidOperationException)
                {
                    // Failed to unify
                    return () => false;
                }

                // Call the clause
                var result = _subclauseSolver.Call(clause.PredicateName, unifier.GetArgumentVariables(clause.NumArguments))();

                // Stop if the clause doesn't resolve correctly
                if (!result)
                {
                    // Failed to resolve this clause
                    return () => false;
                }
            }

            // Success
            // Return just a single value for now
            // TODO: return other results
            var count = 0;
            return () =>
                {
                    ++count;
                    if (count > 1)
                    {
                        unifier.ResetTrail();
                    }
                    return count == 1;
                };
        }
    }
}
