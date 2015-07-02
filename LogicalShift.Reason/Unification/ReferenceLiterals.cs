using LogicalShift.Reason.Api;
using LogicalShift.Reason.Literals;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Helper classes for reference literals
    /// </summary>
    public static class ReferenceLiterals
    {
        /// <summary>
        /// Dereferences a reference literal, finding the point at which it refers to a structure or
        /// is self-referential (which means it's an unbound variable)
        /// </summary>
        public static IReferenceLiteral Dereference(this IReferenceLiteral literal)
        {
            var result = literal;

            while (IsReference(result) && !IsVariable(result))
            {
                result = result.Reference;
            }

            return result;
        }

        /// <summary>
        /// Returns true if a literal is a reference, false if it is a term
        /// </summary>
        public static bool IsReference(this IReferenceLiteral literal)
        {
            return literal.Term == null;
        }

        /// <summary>
        /// Returns true if a literal represents an unbound variable
        /// </summary>
        public static bool IsVariable(this IReferenceLiteral literal)
        {
            return IsReference(literal) && ReferenceEquals(literal, literal.Reference);
        }

        /// <summary>
        /// Freezes a literal
        /// </summary>
        public static ILiteral Freeze(this IReferenceLiteral literal)
        {
            var deref = literal.Dereference();

            if (!IsReference(deref))
            {
                var term = deref.Term;
                var arguments = new List<ILiteral>();

                // Freeze the arguments
                for (var structurePtr = deref.Reference; structurePtr != null; structurePtr = structurePtr.NextArgument)
                {
                    arguments.Add(structurePtr.Freeze());
                }

                // Rebuild the term with these arguments
                return term.RebuildWithParameters(arguments);
            }
            else
            {
                // Represents an unbound variable
                // TODO: re-use the same variable for the same literal address
                return new Variable();
            }
        }
    }
}
