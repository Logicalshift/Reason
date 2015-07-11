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
        /// Identifiers of literals representing permanent variables (variables referenced in multiple subclauses).
        /// These assignments are assigned to lower identifiers, after the assignments representing the arguments.
        /// </summary>
        private readonly HashSet<ILiteral> _permanentVariables = new HashSet<ILiteral>();

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
        /// Adds a permanent variable to this object
        /// </summary>
        public void AddPermanentVariable(ILiteral variable)
        {
            _permanentVariables.Add(variable);
        }

        /// <summary>
        /// The number of arguments in this list (these are always the first set of assignments)
        /// </summary>
        public int CountArguments()
        {
            return _arguments.Count();
        }

        /// <summary>
        /// Counts how many permanent variables have been added to this object
        /// </summary>
        public int CountPermanentVariables()
        {
            return _permanentVariables.Count;
        }

        /// <summary>
        /// A list of variables that are used in these assignments
        /// </summary>
        public IEnumerable<ILiteral> Variables
        {
            get
            {
                var variables = _arguments.Concat(_otherAssignments)
                    .Where(assignment => assignment.Value.UnificationKey == null)
                    .Select(assignment => assignment.Value)
                    .Distinct();
                
                return variables;
            }
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

                // Order the permanent variables first
                var permanentFirst = eliminator.Assigments
                    .OrderBy(assignment => _permanentVariables.Contains(assignment.Value)?0:1);

                // Result is the result of remapping the arguments and adding in the remaining assignments
                var result = _arguments
                    .Select(arg => arg.Remap(eliminator.MapVariable))
                    .Concat(permanentFirst);

                return result;
            }
        }

        /// <summary>
        /// Retrieves an object representing the assignments for a particular literal when used as a predicate
        /// </summary>
        public static PredicateAssignmentList FromPredicate(ILiteral predicate)
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
    }
}
