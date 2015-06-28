using LogicalShift.Reason.Api;
using LogicalShift.Reason.Unification;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason
{
    /// <summary>
    /// Helper methods for performing unification
    /// </summary>
    public static class BasicUnification
    {
        /// <summary>
        /// Returns the possible ways that a query term can unify with a program term
        /// </summary>
        public static IEnumerable<ILiteral> Unify(this ILiteral query, ILiteral program)
        {
            var simpleUnifier = new SimpleUnifier();

            // Run the unifier
            try
            {
                simpleUnifier.QueryUnifier.Compile(query);
                simpleUnifier.PrepareToRunProgram();
                simpleUnifier.ProgramUnifier.Compile(program);
            }
            catch (InvalidOperationException)
            {
                // No results
                // TODO: really should report failure in a better way
                yield break;
            }

            // Retrieve the unified value for the program
            var result = simpleUnifier.UnifiedValue(query.UnificationKey ?? query);
            
            // If the result was valid, return as the one value from this function
            if (result != null)
            {
                yield return result;
            }
        }
    }
}
