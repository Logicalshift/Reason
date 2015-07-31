using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Methods for ordering assignments so that permanent variables are ordered first
    /// </summary>
    /// <remarks>
    /// The ordering is permanent variables, assignments, then the rest. Permanent variables can thus be
    /// allocated starting at 0 (displacing arguments, which can be moved up the list)
    /// </remarks>
    public static class PermanentVariableAssignments
    {
        /// <summary>
        /// Returns the names of the variables that get assigned in a list of assignments
        /// </summary>
        public static IEnumerable<ILiteral> VariablesAssigned(this IEnumerable<IAssignmentLiteral> assignments)
        {
            return assignments
                .Where(assign => assign.Value.UnificationKey == null)
                .Select(assign => assign.Value);
        }

        /// <summary>
        /// Given a list of predicate assignment lists (representing the assignments for the initial predicate and
        /// the clauses, in order), finds the names of the 'permanent' variables: that is, the list of variables
        /// that are used in more than one clause.
        /// </summary>
        public static HashSet<ILiteral> PermanentVariables(this IEnumerable<PredicateAssignmentList> assignmentLists)
        {
            // The predicate and the first clause don't have a call between them, so variables used in both places 
            // aren't considered to be permanent
            var usedVariables = new HashSet<ILiteral>();
            var permanentVariables = new HashSet<ILiteral>();
            int pos = 0;

            foreach (var assignmentList in assignmentLists)
            {
                var assignmentVariables = VariablesAssigned(assignmentList.Assignments).ToArray();

                // After the predicate (pos = 0) and first clause (pos = 1), re-used variables need to be marked as permanent
                if (pos > 1)
                {
                    permanentVariables.UnionWith(assignmentVariables.Where(variable => usedVariables.Contains(variable)));
                }

                // Mark this set of variables as used
                usedVariables.UnionWith(assignmentVariables);

                ++pos;
            }

            return permanentVariables;
        }
    }
}
