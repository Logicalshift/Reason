using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Methods for compiling clauses to a bytecode program
    /// </summary>
    public static class ClauseCompiler
    {
        /// <summary>
        /// Compiles a clause to a bytecode program
        /// </summary>
        public static void Compile(this IClause clause, ByteCodeProgram program)
        {
            var unifier = new ByteCodeUnifier(program);

            // Take each part of the clause and retrieve the assignments for it, with the 'implies' set at the start
            var allPredicates = new[] { clause.Implies }.Concat(clause.If).ToArray();
            var assignmentList = allPredicates
                .Select(predicate => new
                {
                    Assignments = PredicateAssignmentList.FromPredicate(predicate),
                    UnificationKey = predicate.UnificationKey
                })
                .ToArray();

            // Allocate space for the arguments and any permanent variables
            var permanentVariables = PermanentVariableAssignments.PermanentVariables(assignmentList.Select(assign => assign.Assignments));
            var numArguments = assignmentList[0].Assignments.CountArguments();

            if (permanentVariables.Count > 0)
            {
                program.Write(Operation.Allocate, permanentVariables.Count, numArguments);
            }

            // Unify with the predicate first
            unifier.ProgramUnifier.Bind(assignmentList[0].Assignments.Assignments, permanentVariables);
            unifier.ProgramUnifier.Compile(assignmentList[0].Assignments.Assignments);

            // Call the clauses
            foreach (var assignment in assignmentList.Skip(1))
            {
                unifier.QueryUnifier.Bind(assignment.Assignments.Assignments, permanentVariables);
                unifier.QueryUnifier.Compile(assignment.Assignments.Assignments);

                program.Write(Operation.Call, assignment.UnificationKey, assignment.UnificationKey.Dependencies.Count());
            }

            // Deallocate any permanent variables that we might have found
            if (permanentVariables.Count > 0)
            {
                program.Write(Operation.Deallocate);
            }
        }
    }
}
