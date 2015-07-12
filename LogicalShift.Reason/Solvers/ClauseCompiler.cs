﻿using LogicalShift.Reason.Api;
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

            // Get the assignments for this clause
            var allPredicates = new[] { clause.Implies }.Concat(clause.If).ToArray();
            var assignmentList = allPredicates
                .Select(predicate => new
                {
                    Assignments = PredicateAssignmentList.FromPredicate(predicate),
                    UnificationKey = predicate.UnificationKey
                })
                .ToArray();

            // TODO: allocate space for the arguments and any permanent variables

            // Unify with the predicate first
            unifier.ProgramUnifier.Bind(assignmentList[0].Assignments.Assignments);
            unifier.ProgramUnifier.Compile(assignmentList[0].Assignments.Assignments);

            // Call the clauses
            foreach (var assignment in assignmentList.Skip(1))
            {
                unifier.QueryUnifier.Bind(assignment.Assignments.Assignments);
                unifier.QueryUnifier.Compile(assignment.Assignments.Assignments);

                program.Write(Operation.Call, assignment.UnificationKey);
            }
        }
    }
}