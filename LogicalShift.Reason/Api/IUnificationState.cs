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
        /// True if the state is consistent, false if it's inconsistent
        /// </summary>
        bool Consistent { get; }
    }
}
