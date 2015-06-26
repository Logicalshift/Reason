using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Represents a possible unification state
    /// </summary>
    public interface IUnificationState
    {
        /// <summary>
        /// Attempts to discover the binding for a variable (identified by a literal), returns
        /// false if it could not be located
        /// </summary>
        bool TryGetBindingForVariable(ILiteral variable, out ILiteral binding);

        /// <summary>
        /// Returns a new unification state with the specified binding added to it
        /// </summary>
        IUnificationState StateWithBinding(ILiteral variable, ILiteral value);
    }
}
