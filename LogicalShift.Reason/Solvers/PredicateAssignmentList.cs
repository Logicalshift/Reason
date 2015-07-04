using LogicalShift.Reason.Api;
using LogicalShift.Reason.Assignment;
using LogicalShift.Reason.Literals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Represents the assignments in the arguments of a predicate
    /// </summary>
    /// <remarks>
    /// This is used to generate the argument list used in <see cref="ISolver.Call"></see>. For example,
    /// if we want to call the predicate a(b(X), c(X)) we need to generate the two arguments b(X) and c(X)
    /// before calling a/2.
    /// </remarks>
    public class PredicateAssignmentList
    {
        /// <summary>
        /// The list of assignments that represent the arguments
        /// </summary>
        private readonly List<IAssignmentLiteral> _arguments = new List<IAssignmentLiteral>();

        /// <summary>
        /// Other assignments generated for this entry
        /// </summary>
        private readonly List<IAssignmentLiteral> _otherAssignments = new List<IAssignmentLiteral>();

        /// <summary>
        /// Adds a new argument to this object
        /// </summary>
        public void AddArgument(ILiteral argument)
        {
            // Get the assignments for this argument
            var assignments = argument.Flatten().ToArray();

            // The first assignment defines the argument value, and the rest are just other variables
            var rootAssignment = assignments[0];
            var variableAssignments = assignments.Skip(1);

            // If the root assignment is a variable, use a variable argument assignment instead
            if (rootAssignment.Value.UnificationKey == null)
            {
                var variableRoot = rootAssignment;
                variableAssignments = variableAssignments.Concat(new[] { variableRoot });
                rootAssignment = new ArgumentAssignment(new Variable(), variableRoot.Variable);
            }

            // Store the list of other assignments
            _arguments.Add(rootAssignment);
            _otherAssignments.AddRange(variableAssignments);
        }

        /// <summary>
        /// The number of arguments in this list (these are always the first set of assignments)
        /// </summary>
        public int CountArguments()
        {
            return _arguments.Count();
        }

        /// <summary>
        /// The complete list of assignments in this object, starting with the list of argument assignments
        /// </summary>
        public IEnumerable<IAssignmentLiteral> Assignments
        {
            get
            {
                // Eliminate any duplicates in the assignments
                var eliminator = new AssignmentEliminator(_otherAssignments);
                eliminator.Eliminate();

                // Result is the result of remapping the arguments and adding in the remaining assignments
                var result = _arguments
                    .Select(arg => arg.Remap(eliminator.MapVariable))
                    .Concat(eliminator.Assigments);

                return result;
            }
        }
    }
}
