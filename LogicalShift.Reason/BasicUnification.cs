using LogicalShift.Reason.Api;
using LogicalShift.Reason.Unification;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static IBindings Unify(this ILiteral query, ILiteral program)
        {
            var simpleUnifier = new SimpleUnifier();
            var freeVariables = new HashSet<ILiteral>();

            // Run the unifier
            try
            {
                var queryFreeVars = simpleUnifier.QueryUnifier.Compile(query);
                simpleUnifier.PrepareToRunProgram();
                simpleUnifier.ProgramUnifier.Compile(program);

                freeVariables.UnionWith(queryFreeVars);
            }
            catch (InvalidOperationException)
            {
                // No results
                // TODO: really should report failure in a better way
                return null;
            }

            // Retrieve the unified value for the program
            var result = simpleUnifier.UnifiedValue(query.UnificationKey ?? query);
            
            // If the result was valid, return as the one value from this function
            if (result != null)
            {
                var variableBindings = freeVariables.ToDictionary(variable => variable,
                    variable => simpleUnifier.UnifiedValue(variable));

                return new BasicBinding(result, variableBindings);
            }
            else
            {
                return null;
            }
        }
    }
}
