using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Assignment
{
    /// <summary>
    /// Class that eliminates duplicate assignments from a list
    /// </summary>
    public class AssignmentEliminator
    {
        /// <summary>
        /// Maps input variables to output variables
        /// </summary>
        private readonly Dictionary<ILiteral, ILiteral> _variableMapping = new Dictionary<ILiteral,ILiteral>();

        /// <summary>
        /// The current set of assignments in this item
        /// </summary>
        private List<IAssignmentLiteral> _assignments;

        public AssignmentEliminator(IEnumerable<IAssignmentLiteral> assignments)
        {
            if (assignments == null) throw new ArgumentNullException("assignments");

            _assignments = assignments.ToList();
        }

        /// <summary>
        /// Performs a single pass through the list and removes any duplicate assignment values
        /// </summary>
        private bool EliminationPass()
        {
            // Find duplicate assignments
            var duplicateAssignments = _assignments
                .GroupBy(assignment => assignment.Value)
                .Where(group => group.Count() > 1)
                .ToList();

            // Remap duplicated variables
            foreach (var duplicate in duplicateAssignments)
            {
                var newVariable = duplicate.First().Variable;
                foreach (var reassign in duplicate.Skip(1))
                {
                    _variableMapping.Add(reassign.Variable, newVariable);
                }
            }

            // Rewrite the assignments
            if (duplicateAssignments.Any())
            {
                // Eliminate variables that are duplicated, rewrite the rest
                _assignments = _assignments
                    .Where(assign => !_variableMapping.ContainsKey(assign.Variable))
                    .Select(assign => assign.Remap(MapVariable))
                    .ToList();
            }

            // Result is true if there were any duplicates in this pass
            return duplicateAssignments.Any();
        }

        /// <summary>
        /// Eliminates any assignments in the list that assign to the same value
        /// </summary>
        public void Eliminate()
        {
            while (EliminationPass()) ;
        }

        /// <summary>
        /// Returns the mapped version of a particular variable
        /// </summary>
        public ILiteral MapVariable(ILiteral variable)
        {
            ILiteral result;
            if (_variableMapping.TryGetValue(variable, out result))
            {
                return result;
            }
            else
            {
                return variable;
            }
        }

        /// <summary>
        /// The set of assignments in this object (with duplicates eliminated)
        /// </summary>
        public IEnumerable<IAssignmentLiteral> Assigments
        {
            get { return _assignments; }
        }
    }
}
