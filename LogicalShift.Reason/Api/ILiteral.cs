using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Interface implemented by objects representing a literal
    /// </summary>
    public interface ILiteral : IEquatable<ILiteral>
    {
        /// <summary>
        /// Generates a unification program for this literal when treated as a query
        /// </summary>
        void UnifyQuery(IQueryUnifier unifier);

        /// <summary>
        /// Generates a unification program for this literal when treated as a program
        /// </summary>
        void UnifyProgram(IProgramUnifier unifier);

        /// <summary>
        /// Flattens this literal into a series of assignments (starting with the assignment that
        /// resolves this literal)
        /// </summary>
        IEnumerable<IAssignmentLiteral> Flatten();

        /// <summary>
        /// Rebuilds this literal with the specified parameters
        /// </summary>
        ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters);

        /// <summary>
        /// Returns the literals that this literal depends on (eg, the parameters)
        /// </summary>
        IEnumerable<ILiteral> Dependencies { get; }

        /// <summary>
        /// Retrieves a value that is the same for all types of literal that this can be unified with,
        /// or null if this can be unified with any literal.
        /// </summary>
        ILiteral UnificationKey { get; }
    }
}
