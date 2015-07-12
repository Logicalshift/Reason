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
    /// The ordering is arguments, permanent variables, then the rest. Arguments can thus
    /// be bound to variables starting from 0 and permanent variables can be bound to the
    /// set after the arguments.
    /// </remarks>
    public static class PermanentVariableAssignments
    {
        /// <summary>
        /// Orders assignments so that the list of 'arguments' is first, followed by any assignments for 'permanent' variables, followed by any remaining assignments
        /// </summary>
        public static IEnumerable<IAssignmentLiteral> OrderPermanentVariablesFirst(this IEnumerable<IAssignmentLiteral> assignments, int numArguments, IEnumerable<ILiteral> permanentVariables)
        {
            // Convert the list of permanent variables into a hash set
            var isPermanent = permanentVariables as HashSet<ILiteral> ?? new HashSet<ILiteral>(permanentVariables);

            // ICollections can be iterated over multiple times
            var assignmentCollection = assignments as ICollection<IAssignmentLiteral> ?? assignments.ToArray();

            // Perform the ordering
            var arguments = assignmentCollection.Take(numArguments);
            var remainder = assignmentCollection.Skip(numArguments);
            var permanentFirst = remainder.OrderBy(assignment => isPermanent.Contains(assignment.Value) ? 0 : 1);

            // Result is arguments followed by the list with the permanent variables first
            return arguments.Concat(permanentFirst);
        }
    }
}
