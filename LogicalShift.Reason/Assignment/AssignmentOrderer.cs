using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Assignment
{
    /// <summary>
    /// Orders sets of assignments
    /// </summary>
    public static class AssignmentOrderer
    {
        /// <summary>
        /// Returns a list of assignments ordered so that subterms come before terms (this is the order that they
        /// should be built in order to compile a query - ie, bottom-up order)
        /// </summary>
        public static IEnumerable<IAssignmentLiteral> OrderTermsAfterSubterms(this IEnumerable<IAssignmentLiteral> list)
        {
            // Terms that have been returned
            var seenTerms = new HashSet<ILiteral>();

            // Quick lookup for the term with a particular variable
            var termForVariable = list.ToDictionary(term => term.Variable);

            // Variabes left to process
            var toProcess = new Stack<ILiteral>(termForVariable.Keys);

            while (toProcess.Any())
            {
                var candidate = toProcess.Pop();

                // Don't return a term more than once
                if (seenTerms.Contains(candidate)) continue;

                var assignment = termForVariable[candidate];
                if (assignment.Value.Dependencies.All(seenTerms.Contains))
                {
                    // All the terms for this assignment have been seen
                    yield return assignment;
                    seenTerms.Add(candidate);
                }
                else
                {
                    // Return the dependencies first
                    toProcess.Push(candidate);
                    foreach (var dep in assignment.Value.Dependencies)
                    {
                        toProcess.Push(dep);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of assignments ordered so that terms come before subterms
        /// </summary>
        public static IEnumerable<IAssignmentLiteral> OrderSubtermsAfterTerms(this IEnumerable<IAssignmentLiteral> list)
        {
            return OrderTermsAfterSubterms(list).Reverse();
        }
    }
}
