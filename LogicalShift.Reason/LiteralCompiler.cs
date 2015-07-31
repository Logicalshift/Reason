using LogicalShift.Reason.Api;
using LogicalShift.Reason.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason
{
    /// <summary>
    /// Compiles literals for unification
    /// </summary>
    public static class LiteralCompiler
    {
        /// <summary>
        /// Binds the variables in the unifier for a set of assignments
        /// </summary>
        /// <returns>
        /// The list of free variables in the assignments
        /// </returns>
        public static IEnumerable<ILiteral> Bind(this IBaseUnifier unifier, IEnumerable<IAssignmentLiteral> assignments, IEnumerable<ILiteral> permanentVariables = null)
        {
            if (unifier == null) throw new ArgumentNullException("unifier");
            if (assignments == null) throw new ArgumentNullException("assignments");
            if (permanentVariables == null) permanentVariables = new ILiteral[0];

            var indexForPermanent = new Dictionary<ILiteral, int>();
            var freeVariables = new List<ILiteral>();
            var index = 0;

            // Assign permanent variables to indexes starting from 0
            // TODO: what if a permanent variable appears in an argument location?
            foreach (var permVariable in permanentVariables)
            {
                indexForPermanent[permVariable] = index;
                ++index;
            }

            // Bind the assignments
            foreach (var assign in assignments)
            {
                int variableIndex;

                // If the assignment is of the form X1 = Y where Y is a permanent variable, use the permanent variable index
                if (!indexForPermanent.TryGetValue(assign.Value, out variableIndex))
                {
                    // For other assignments, use an increasing index
                    variableIndex = index;
                    ++index;
                }

                if (assign.Value.UnificationKey == null)
                {
                    // Values without a unification key are free variables (they can have any value)
                    freeVariables.Add(assign.Value);
                    unifier.BindVariable(variableIndex, assign.Value);
                    unifier.BindVariable(variableIndex, assign.Variable);
                }
                else
                {
                    // Values with a unification key are not free
                    unifier.BindVariable(variableIndex, assign.Value.UnificationKey);
                    unifier.BindVariable(variableIndex, assign.Variable);
                }
            }

            return freeVariables;
        }

        /// <summary>
        /// Compiles a literal for unification using a query unifier
        /// </summary>
        /// <returns>
        /// The list of variables in the literal
        /// </returns>
        public static IEnumerable<ILiteral> Compile(this IQueryUnifier unifier, ILiteral literal, IBindings bindings = null)
        {
            if (unifier == null) throw new ArgumentNullException("unifier");
            if (literal == null) throw new ArgumentNullException("literal");

            // Flatten the literal
            var assignments = literal.Flatten().ToList();

            // Bind the variables in order
            var freeVariables = unifier.Bind(assignments);

            // Rebind the assignments if necessary
            if (bindings != null)
            {
                assignments = assignments.BindVariables(bindings).ToList();
            }

            // Compile the result
            if (!unifier.Compile(assignments))
            {
                return null;
            }

            return freeVariables;
        }

        /// <summary>
        /// Compiles a literal for unification using a program unifier
        /// </summary>
        /// <returns>
        /// The list of variables in this literal
        /// </returns>
        public static IEnumerable<ILiteral> Compile(this IProgramUnifier unifier, ILiteral literal, IBindings bindings = null)
        {
            if (unifier == null) throw new ArgumentNullException("unifier");
            if (literal == null) throw new ArgumentNullException("literal");

            // Flatten the literal
            var assignments = literal.Flatten().ToList();

            // Bind the variables in order
            var freeVariables = unifier.Bind(assignments);

            // Rebind the assignments if necessary
            if (bindings != null)
            {
                assignments = assignments.BindVariables(bindings).ToList();
            }

            // Compile the result
            if (!unifier.Compile(assignments))
            {
                return null;
            }

            return freeVariables;
        }

        /// <summary>
        /// Compiles a series of assignments using a query unifier
        /// </summary>
        /// <returns>
        /// The list of variables in the literal
        /// </returns>
        public static bool Compile(this IQueryUnifier unifier, IEnumerable<IAssignmentLiteral> assignments)
        {
            if (unifier == null) throw new ArgumentNullException("unifier");
            if (assignments == null) throw new ArgumentNullException("assignments");

            // Compile subterms to terms
            foreach (var assign in assignments.OrderTermsAfterSubterms())
            {
                if (!assign.CompileQuery(unifier)) return false;
            }

            return true;
        }

        /// <summary>
        /// Compiles a series of assignments using a program unifier
        /// </summary>
        /// <returns>
        /// The list of variables in the literal
        /// </returns>
        public static bool Compile(this IProgramUnifier unifier, IEnumerable<IAssignmentLiteral> assignments)
        {
            if (unifier == null) throw new ArgumentNullException("unifier");
            if (assignments == null) throw new ArgumentNullException("assignments");

            // Compile subterms to terms
            foreach (var assign in assignments.OrderSubtermsAfterTerms())
            {
                if (!assign.CompileProgram(unifier)) return false;
            }

            return true;
        }
    }
}
