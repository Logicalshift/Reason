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
        public static void Bind(this IBaseUnifier unifier, IEnumerable<IAssignmentLiteral> assignments)
        {
            foreach (var assign in assignments)
            {
                unifier.BindVariable(assign.Variable);
            }
        }

        /// <summary>
        /// Compiles a literal for unification using a query unifier
        /// </summary>
        public static void Compile(this IQueryUnifier unifier, ILiteral literal)
        {
            if (unifier == null) throw new ArgumentNullException("unifier");
            if (literal == null) throw new ArgumentNullException("literal");

            // Flatten the literal
            var assignments = literal.Flatten().ToList();

            // Bind the variables in order
            unifier.Bind(assignments);

            // Compile subterms to terms
            foreach (var assign in assignments.OrderTermsAfterSubterms())
            {
                assign.CompileQuery(unifier);
            }
        }

        /// <summary>
        /// Compiles a literal for unification using a program unifier
        /// </summary>
        public static void Compile(this IProgramUnifier unifier, ILiteral literal)
        {
            if (unifier == null) throw new ArgumentNullException("unifier");
            if (literal == null) throw new ArgumentNullException("literal");

            // Flatten the literal
            var assignments = literal.Flatten().ToList();

            // Bind the variables in order
            unifier.Bind(assignments);

            foreach (var assign in assignments.OrderSubtermsAfterTerms())
            {
                assign.CompileProgram(unifier);
            }
        }
    }
}
