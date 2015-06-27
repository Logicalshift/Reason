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
        /// Rebuilds this literal with the specified parameters
        /// </summary>
        ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters);
    }
}
